using System;
using System.Drawing;

namespace WinTouch
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
            Flags = (GestureFlags)info.flags;
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

        protected GestureFlags Flags { get; set; }

        /// <summary>
        /// Gets the location of the gesture in Screen (not client) coordinates.
        /// </summary>
        public Point Location { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="GestureEventArgs"/> is beginning.
        /// </summary>
        public bool Begin
        {
            get { return (Flags & GestureFlags.Begin) == GestureFlags.Begin; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="GestureEventArgs"/> is ending
        /// </summary>
        public bool End
        {
            get { return (Flags & GestureFlags.End) == GestureFlags.End; }
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
        internal PanEventArgs( GestureInfo info )
            : base( info )
        {
            int hiword = HiDWord( info.arguments );
            InertiaVector = new Point( LoWord( hiword ), HiWord( hiword ) );
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="GestureEventArgs"/> has triggered inertia.
        /// </summary>
        public bool Inertia
        {
            get { return (Flags & GestureFlags.Inertia) == GestureFlags.Inertia; }
        }

        public Point InertiaVector { get; private set; }
    }

    #endregion

    #region ZoomEventArgs Class

    /// <summary>
    /// Event for the Zoom gesture
    /// </summary>
    public class ZoomEventArgs : GestureEventArgs
    {
        internal ZoomEventArgs( GestureInfo info )
            : base( info )
        {
            Distance = info.arguments;
        }

        /// <summary>
        /// Gets the distance between the two points as they are being zoomed.
        /// </summary>
        public long Distance { get; private set; }
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
        internal RotateEventArgs( GestureInfo info )
            : base( info )
        {
            int loword = LoDWord( info.arguments );
            Angle = RotateAngleFromArgument( loword );
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
        /// Gets the angle of rotation in Radians
        /// </summary>
        public double Angle { get; private set; }

        /// <summary>
        /// Gets the angle of rotation in Degrees
        /// </summary>
        public double Degrees
        {
            get { return Angle * 180.0 / Math.PI; }
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