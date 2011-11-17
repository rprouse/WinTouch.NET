#region Copyright (c) Microsoft Corporation.
//  
// The source code of UnmanagedLibrary.cs is quoted from Mike Stall's article: 
//  
// Type-safe Managed wrappers for kernel32!GetProcAddress 
// http://blogs.msdn.com/jmstall/archive/2007/01/06/Typesafe-GetProcAddress.aspx 
//  
// This source is subject to the Microsoft Public License. 
// See http://www.opensource.org/licenses/MS-PL. 
// All other rights reserved. 
//  
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,  
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED  
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
//
#endregion

#region Using Directives

using System;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

#endregion

namespace Alteridem.WinTouch
{
    /// <summary>
    /// Utility class to wrap an unmanaged DLL and be responsible for freeing it.
    /// </summary>
    /// <remarks>This is a managed wrapper over the native LoadLibrary, GetProcAddress, and
    /// FreeLibrary calls.
    /// </remarks>
    public sealed class UnmanagedLibrary : IDisposable
    {
        #region Safe Handles and Native imports

        // See http://msdn.microsoft.com/msdnmag/issues/05/10/Reliability/ for more about safe handles.
        [SecurityPermission( SecurityAction.LinkDemand, UnmanagedCode = true )]
        sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            private SafeLibraryHandle() : base( true ) { }

            protected override bool ReleaseHandle()
            {
                return NativeMethods.FreeLibrary( handle );
            }
        }

        static class NativeMethods
        {
            const string KERNEL32 = "kernel32";
            [DllImport( KERNEL32, CharSet = CharSet.Auto, BestFitMapping = false, SetLastError = true )]
            public static extern SafeLibraryHandle LoadLibrary( string fileName );

            [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
            [DllImport( KERNEL32, SetLastError = true )]
            [return: MarshalAs( UnmanagedType.Bool )]
            public static extern bool FreeLibrary( IntPtr hModule );

            [DllImport( KERNEL32 )]
            public static extern IntPtr GetProcAddress( SafeLibraryHandle hModule, String procname );
        }

        #endregion // Safe Handles and Native imports

        /// <summary>
        /// Constructor to load a dll and be responible for freeing it.
        /// </summary>
        /// <param name="fileName">full path name of dll to load</param>
        /// <exception cref="FileNotFoundException">if fileName can't be found</exception>
        /// <remarks>Throws exceptions on failure. Most common failure would be file-not-found, or
        /// that the file is not a  loadable image.</remarks>
        public UnmanagedLibrary( string fileName )
        {
            m_hLibrary = NativeMethods.LoadLibrary( fileName );
            if ( m_hLibrary.IsInvalid )
            {
                int hr = Marshal.GetHRForLastWin32Error();
                Marshal.ThrowExceptionForHR( hr );
            }
        }

        /// <summary>
        /// Dynamically lookup a function in the dll via kernel32!GetProcAddress.
        /// </summary>
        /// <param name="functionName">raw name of the function in the export table.</param>
        /// <returns>null if function is not found. Else a delegate to the unmanaged function.
        /// </returns>
        /// <remarks>GetProcAddress results are valid as long as the dll is not yet unloaded. This
        /// is very very dangerous to use since you need to ensure that the dll is not unloaded
        /// until after you're done with any objects implemented by the dll. For example, if you
        /// get a delegate that then gets an IUnknown implemented by this dll,
        /// you can not dispose this library until that IUnknown is collected. Else, you may free
        /// the library and then the CLR may call release on that IUnknown and it will crash.</remarks>
        public TDelegate GetUnmanagedFunction<TDelegate>( string functionName ) where TDelegate : class
        {
            IntPtr p = NativeMethods.GetProcAddress( m_hLibrary, functionName );

            // Failure is a common case, especially for adaptive code.
            if ( p == IntPtr.Zero )
            {
                return null;
            }
            Delegate function = Marshal.GetDelegateForFunctionPointer( p, typeof( TDelegate ) );

            // Ideally, we'd just make the constraint on TDelegate be
            // System.Delegate, but compiler error CS0702 (constrained can't be System.Delegate)
            // prevents that. So we make the constraint system.object and do the cast from object-->TDelegate.
            object o = function;

            return (TDelegate)o;
        }

        #region IDisposable Members
        /// <summary>
        /// Call FreeLibrary on the unmanaged dll. All function pointers
        /// handed out from this class become invalid after this.
        /// </summary>
        /// <remarks>This is very dangerous because it suddenly invalidate
        /// everything retrieved from this dll. This includes any functions
        /// handed out via GetProcAddress, and potentially any objects returned
        /// from those functions (which may have an implemention in the
        /// dll).
        /// </remarks>
        public void Dispose()
        {
            if ( !m_hLibrary.IsClosed )
            {
                m_hLibrary.Close();
            }
        }

        // Unmanaged resource. CLR will ensure SafeHandles get freed, without requiring a finalizer on this class.
        SafeLibraryHandle m_hLibrary;

        #endregion
    }
}