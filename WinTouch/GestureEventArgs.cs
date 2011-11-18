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
using System.Drawing;

#endregion

namespace Alteridem.WinTouch
{
    #region GestureEventArgs Class

    /// <summary>
    /// Base class for all the gesture based events
    /// </summary>
    public abstract class GestureEventArgs : EventArgs
    {
        internal GestureEventArgs( GestureInfo info )
        {
            Location = new Point( info.location.x, info.location.y );
            Info = info;
            Handled = true;
        }

        #region Helper Methods

        protected static int LoDWord( IntPtr lParam )
        {
            return LoDWord( lParam.ToInt64() );
        }

        protected static int HiDWord( IntPtr lParam )
        {
            return HiDWord( lParam.ToInt64() );
        }

        protected static int LoDWord( long l )
        {
            return (int)(l & 0xFFFFFFFF);
        }

        protected static int HiDWord( long l )
        {
            return (int)((l >> 32) & 0xFFFFFFFF);
        }

        protected static short LoWord( int i )
        {
            return (short)(i & 0xFFFF);
        }

        protected static short HiWord( int i )
        {
            return (short)((i >> 16) & 0xFFFF);
        }

        #endregion

        #region Properties

        protected GestureInfo Info { get; set; }

        /// <summary>
        /// Gets the location of the gesture in Screen (not client) coordinates.
        /// </summary>
        public Point Location { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="GestureEventArgs"/> is beginning.
        /// </summary>
        public bool Begin
        {
            get { return Info.Begin; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="GestureEventArgs"/> is ending
        /// </summary>
        public bool End
        {
            get { return Info.End; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the window message was handled. Set this to false if you don't handle the message.
        /// </summary>
        public bool Handled { get; set; }

        #endregion
    }

    #endregion

    #region PanEventArgs Class

    /// <summary>
    /// Event for the Pan gesture
    /// </summary>
    public class PanEventArgs : GestureEventArgs
    {
        internal PanEventArgs( GestureInfo info, Point lastPanPoint )
            : base( info )
        {
            int hiword = HiDWord( info.arguments );
            InertiaVector = new Point( LoWord( hiword ), HiWord( hiword ) );
            PanOffset = new Point( Location.X - lastPanPoint.X, Location.Y - lastPanPoint.Y );
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="GestureEventArgs"/> has triggered inertia.
        /// </summary>
        public bool Inertia
        {
            get { return Info.Inertia; }
        }

        public Point InertiaVector { get; private set; }

        /// <summary>
        /// Gets the pan offset since the last pan message.
        /// </summary>
        public Point PanOffset { get; private set; }
    }

    #endregion

    #region ZoomEventArgs Class

    /// <summary>
    /// Event for the Zoom gesture
    /// </summary>
    public class ZoomEventArgs : GestureEventArgs
    {
        internal ZoomEventArgs( GestureInfo info, long lastZoomDistance )
            : base( info )
        {
            Distance = info.arguments;
            PercentChange = (double)Distance / lastZoomDistance;
        }

        /// <summary>
        /// Gets the distance between the two points as they are being zoomed.
        /// </summary>
        public long Distance { get; private set; }

        /// <summary>
        /// Gets the percent changed since the last zoom message
        /// </summary>
        public double PercentChange { get; private set; }
    }

    #endregion

    #region PressAndTapEventArgs Class

    /// <summary>
    /// Event for the Press and Tap gesture
    /// </summary>
    public class PressAndTapEventArgs : GestureEventArgs
    {
        internal PressAndTapEventArgs( GestureInfo info )
            : base( info )
        {
            int pointsStruct = LoDWord( info.arguments );
            Distance = new Point( LoWord( pointsStruct ), HiWord( pointsStruct ) );
        }

        /// <summary>
        /// Gets the distance between the two points.
        /// </summary>
        public Point Distance { get; private set; }
    }

    #endregion

    #region RotateEventArgs Class

    /// <summary>
    /// Event for the Rotate gesture
    /// </summary>
    public class RotateEventArgs : GestureEventArgs
    {
        internal RotateEventArgs( GestureInfo info, double lastRotation )
            : base( info )
        {
            int loword = LoDWord( info.arguments );
            TotalAngle = RotateAngleFromArgument( loword );
            Angle = TotalAngle - lastRotation;
            string msg = string.Format("Total:{0} Angle:{1} Last:{2}", TotalAngle, Angle, lastRotation );
            System.Diagnostics.Debug.WriteLine( msg );
        }

        /// <summary>
        /// Gesture argument helper that converts an argument to a rotation angle.
        /// </summary>
        /// <param name="arg">The argument to convert. Should be an unsigned 16-bit value.</param>
        /// <returns></returns>
        private static double RotateAngleFromArgument( int arg )
        {
            return ((arg / 65535.0) * 4.0 * Math.PI) - 2.0 * Math.PI;
        }

        /// <summary>
        /// Gets the angle of rotation in Radians since the beginning of the gesture
        /// </summary>
        public double TotalAngle { get; private set; }

        /// <summary>
        /// Gets the angle of rotation in Degrees since the beginning of the gesture
        /// </summary>
        public double TotalDegrees
        {
            get { return RadiandsToDegrees( TotalAngle ); }
        }

        /// <summary>
        /// Gets the angle of rotation in Radians since the last rotation message
        /// </summary>
        public double Angle { get; private set; }

        /// <summary>
        /// Gets the angle of rotation in Degrees since the last rotation message
        /// </summary>
        public double Degrees
        {
            get { return RadiandsToDegrees( Angle ); }
        }

        private double RadiandsToDegrees( double radians )
        {
            return radians * 180.0 / Math.PI;
        }
    }

    #endregion

    #region TwoFingerTapEventArgs Class

    /// <summary>
    /// Base class for the Two Finger Tap gesture
    /// </summary>
    public class TwoFingerTapEventArgs : GestureEventArgs
    {
        internal TwoFingerTapEventArgs( GestureInfo info )
            : base( info )
        {
            Distance = info.arguments;
        }

        /// <summary>
        /// Gets the distance between the two points.
        /// </summary>
        public long Distance { get; private set; }
    }

    #endregion
}