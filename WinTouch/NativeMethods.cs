#region The MIT License (MIT)
//
// Copyright (c) 2011 Robert Prouse http://www.alteridem.net
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of 
// this software and associated documentation files (the "Software"), to deal in 
// the Software without restriction, including without limitation the rights to use, 
// copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
// Software, and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

#region Using Directives

using System;
using System.Runtime.InteropServices;

#endregion

namespace Alteridem.WinTouch
{
    internal static class NativeMethods
    {
        #region Private Delegates

        [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
        private delegate bool GetGestureInfoPtr( IntPtr gestureInfoHandle, ref GestureInfo pGestureInfo );

        [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
        private delegate bool CloseGestureInfoHandlePtr( IntPtr gestureInfoHandle );

        [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
        private delegate bool SetGestureConfigPtr( IntPtr hwnd, uint reserved, uint ids, GestureConfig[] configs, uint size );

        private static readonly GetGestureInfoPtr _pGetGestureInfoPtr;
        private static readonly CloseGestureInfoHandlePtr _pCloseGestureInfoHandle;
        private static readonly SetGestureConfigPtr _pSetGestureConfig;

        #endregion

        #region Static Construction

        static NativeMethods()
        {
            var user32 = new UnmanagedLibrary( "user32" );
            _pGetGestureInfoPtr = user32.GetUnmanagedFunction<GetGestureInfoPtr>( "GetGestureInfo" );
            _pCloseGestureInfoHandle = user32.GetUnmanagedFunction<CloseGestureInfoHandlePtr>( "CloseGestureInfoHandle" );
            _pSetGestureConfig = user32.GetUnmanagedFunction<SetGestureConfigPtr>( "SetGestureConfig" );
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// Sets the gesture config.
        /// </summary>
        /// <remarks>
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/dd353231%28v=vs.85%29.aspx
        /// </remarks>
        /// <param name="hwnd">The HWND.</param>
        /// <param name="id">The id.</param>
        /// <param name="want">The want.</param>
        /// <param name="block">The block.</param>
        /// <returns></returns>
        public static bool SetGestureConfig( IntPtr hwnd, uint id, uint want, uint block )
        {
            if ( _pSetGestureConfig == null )
                return false;

            var configs = new[] { new GestureConfig( id, want, block )  };
            return _pSetGestureConfig( hwnd, 0, 1, configs, (uint)Marshal.SizeOf( typeof( GestureConfig ) ) );
        }

        /// <summary>
        /// Gets the gesture info.
        /// </summary>
        /// <param name="gestureInfoHandle">The gesture info handle.</param>
        /// <param name="gestureInfo">The gesture info.</param>
        /// <returns></returns>
        public static bool GetGestureInfo( IntPtr gestureInfoHandle, out GestureInfo gestureInfo )
        {
            gestureInfo = new GestureInfo();

            if ( _pGetGestureInfoPtr == null )
                return false;

            gestureInfo.size = Marshal.SizeOf( gestureInfo );
            return _pGetGestureInfoPtr( gestureInfoHandle, ref gestureInfo );
        }

        /// <summary>
        /// Closes the gesture info handle.
        /// </summary>
        /// <param name="gestureInfoHandle">The gesture info handle.</param>
        /// <returns></returns>
        public static bool CloseGestureInfoHandle( IntPtr gestureInfoHandle )
        {
            if ( _pCloseGestureInfoHandle == null )
                return false;

            return _pCloseGestureInfoHandle( gestureInfoHandle );
        }

        #endregion
    }

    /// <summary>
    /// Gesture configuration flags
    /// </summary>
    internal static class GestureConfigurationFlag
    {
        public const int GC_ALLGESTURES = 0x00000001;
        public const int GC_ZOOM = 0x00000001;
        public const int GC_PAN = 0x00000001;
        public const int GC_PAN_WITH_SINGLE_FINGER_VERTICALLY = 0x00000002;
        public const int GC_PAN_WITH_SINGLE_FINGER_HORIZONTALLY = 0x00000004;
        public const int GC_PAN_WITH_GUTTER = 0x00000008;
        public const int GC_PAN_WITH_INERTIA = 0x00000010;
        public const int GC_ROTATE = 0x00000001;
        public const int GC_TWOFINGERTAP = 0x00000001;
        public const int GC_PRESSANDTAP = 0x00000001;
    }

    /// <summary>
    /// Gesture flags - GestureInfo.flags
    /// </summary>
    [Flags]
    public enum GestureFlags
    {
        /// <summary>
        /// GF_BEGIN
        /// </summary>
        Begin = 0x1,
        /// <summary>
        /// GF_INERTIA
        /// </summary>
        Inertia = 0x2,
        /// <summary>
        /// GF_END
        /// </summary>
        End = 0x4
    }

    /// <summary>
    /// Gesture IDs - GestureInfo.id
    /// </summary>
    public enum GestureId
    {
        /// <summary>
        /// GID_BEGIN
        /// </summary>
        Begin = 1,
        /// <summary>
        /// GID_END
        /// </summary>
        End = 2,
        /// <summary>
        /// GID_ZOOM
        /// </summary>
        Zoom = 3,
        /// <summary>
        /// GID_PAN
        /// </summary>
        Pan = 4,
        /// <summary>
        /// GID_ROTATE
        /// </summary>
        Rotate = 5,
        /// <summary>
        /// GID_TWOFINGERTAP
        /// </summary>
        TwoFingerTap = 6,
        /// <summary>
        /// GID_PRESSANDTAP
        /// </summary>
        PressAndTap = 7
    }

    /// <summary>
    /// Window Messages
    /// </summary>
    internal static class WindowMessage
    {
        public const int WM_TOUCH = 0x0240;
        public const int WM_GESTURE = 0x0119;
        public const int WM_GESTURENOTIFY = 0x011A;
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct Points
    {
        public short x;
        public short y;
    }

    /// <summary>
    /// Gesture configuration structure
    /// </summary>
    /// <remarks>
    /// Used in SetGestureConfig and GetGestureConfig
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/dd353231%28v=vs.85%29.aspx
    /// </remarks>
    [StructLayout( LayoutKind.Sequential )]
    public struct GestureConfig
    {
        public GestureConfig( uint id, uint want, uint block )
        {
            Id = id;
            Want = want;
            Block = block;
        }

        /// <summary>
        /// The identifier for the type of configuration that will have messages enabled or disabled.
        /// </summary>
        public uint Id;

        /// <summary>
        /// The messages to enable.
        /// </summary>
        public uint Want;

        /// <summary>
        /// The messages to disable.
        /// </summary>
        public uint Block;
    }

    /// <summary>
    /// Stores information about a gesture.
    /// </summary>
    /// <remarks>
    /// - Pass the HGESTUREINFO received in the WM_GESTURE message lParam into the
    ///   GetGestureInfo function to retrieve this information.
    /// - If cbExtraArgs is non-zero, pass the HGESTUREINFO received in the WM_GESTURE
    ///   message lParam into the GetGestureExtraArgs function to retrieve extended
    ///   argument information. 
    /// </remarks>
    [StructLayout( LayoutKind.Sequential )]
    public struct GestureInfo
    {
        /// <summary>
        /// The size of the structure, in bytes. The caller must set this.
        /// </summary>
        public int size;
        /// <summary>
        /// The state of the gesture. Contains GestureFlags.
        /// </summary>
        public int flags;
        /// <summary>
        /// The identifier of the gesture command. Contains GestureId.
        /// </summary>
        public int id;
        /// <summary>
        /// A handle to the window that is targeted by this gesture.
        /// </summary>
        public IntPtr hwnd;
        /// <summary>
        /// A Points structure containing the coordinates associated with the gesture. These coordinates are always relative to the origin of the screen.
        /// </summary>
        public Points location;
        /// <summary>
        /// An internally used identifier for the structure.
        /// </summary>
        public int instanceId;
        /// <summary>
        /// An internally used identifier for the sequence.
        /// </summary>
        public int sequenceId;
        /// <summary>
        /// A 64-bit unsigned integer that contains the arguments for gestures that fit into 8 bytes. 
        /// </summary>
        public Int64 arguments;
        /// <summary>
        /// The size, in bytes, of extra arguments that accompany this gesture.
        /// </summary>
        public int extraArguments;

        /// <summary>
        /// Gets a value indicating whether the <see cref="GestureFlags"/> Begin is set.
        /// </summary>
        public bool Begin
        {
            get { return ((GestureFlags)flags & GestureFlags.Begin) == GestureFlags.Begin; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="GestureFlags"/> End is set.
        /// </summary>
        public bool End
        {
            get { return ((GestureFlags)flags & GestureFlags.End) == GestureFlags.End; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="GestureFlags"/> Inertia is set
        /// </summary>
        public bool Inertia
        {
            get { return ((GestureFlags)flags & GestureFlags.Inertia) == GestureFlags.Inertia; }
        }
    }
}
