using System;
using System.Drawing;

namespace WinTouch
{
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

      protected static int LoWord( IntPtr lParam )
      {
         return (int)(lParam.ToInt64() & 0xFFFF);
      }

      protected static int HiWord( IntPtr lParam )
      {
         return (int)(lParam.ToInt64() >> 16);
      }

      protected GestureFlags Flags { get; set; }

      /// <summary>
      /// Gets the location of the gesture in Screen (not client) coordinates.
      /// </summary>
      public Point Location { get; private set; }

      /// <summary>
      /// Gets a value indicating whether this <see cref="GestureEventArgs"/> is beginning.
      /// </summary>
      public bool Begin { get { return ( Flags & GestureFlags.Begin ) == GestureFlags.Begin; }}

      /// <summary>
      /// Gets a value indicating whether this <see cref="GestureEventArgs"/> is ending
      /// </summary>
      public bool End { get { return (Flags & GestureFlags.End) == GestureFlags.End; } }

      /// <summary>
      /// Gets or sets a value indicating whether the window message was handled. Set this to false if you don't handle the message.
      /// </summary>
      public bool Handled { get; set; }
   }

   /// <summary>
   /// Event for the Pan gesture
   /// </summary>
   public class PanEventArgs : GestureEventArgs
   {
      internal PanEventArgs( GestureInfo info )
         : base( info )
      {
         int hiword = (int)(info.arguments >> 32);
         int x = hiword >> 16;
         int y = hiword & 0x00FF;
         InertiaVector = new Point( x, y );
      }

      /// <summary>
      /// Gets a value indicating whether this <see cref="GestureEventArgs"/> has triggered inertia.
      /// </summary>
      public bool Inertia { get { return (Flags & GestureFlags.Inertia) == GestureFlags.Inertia; } }

      public Point InertiaVector { get; private set; }
   }

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

   /// <summary>
   /// Event for the Press and Tap gesture
   /// </summary>
   public class PressAndTapEventArgs : GestureEventArgs
   {
      internal PressAndTapEventArgs( GestureInfo info )
         : base( info )
      {
         Distance = info.arguments;
      }

      /// <summary>
      /// Gets the distance between the two points.
      /// </summary>
      public long Distance { get; private set; }
   }

   /// <summary>
   /// Event for the Rotate gesture
   /// </summary>
   public class RotateEventArgs : GestureEventArgs
   {
      internal RotateEventArgs( GestureInfo info, IntPtr lParam )
         : base( info )
      {
         int loword = LoWord( lParam );
         Angle = RotateAngleFromArgument( loword );
      }

      /// <summary>
      /// Gesture argument helper that converts a rotation angle to an argument.
      /// </summary>
      /// <param name="angle">The angle to convert. Should be a double in the range of -2pi to +2pi.</param>
      /// <returns></returns>
      private static int RotateAngleToArgument( double angle )
      {
         return (int)((((angle) + 2.0 * Math.PI) / (4.0 * Math.PI)) * 65535.0);
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
         get { return Angle * (180.0 * Math.PI); }
      }
   }

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
}