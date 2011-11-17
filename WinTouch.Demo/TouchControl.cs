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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Alteridem.WinTouch.Demo
{
    public partial class TouchControl : UserControl
    {
        // Our touch listener
        private readonly TouchListener _touch;

        // Size, location and rotation of the square
        private Point _location;
        private int _size;
        private float _rotation;

        // Saved state
        private Point _lastPanPoint;
        private float _lastRotation;
        private long _lastZoom;

        // Brushes
        private readonly Brush[] _backBrushes = { Brushes.White, Brushes.AntiqueWhite, Brushes.Bisque, Brushes.Wheat, Brushes.Bisque, Brushes.AntiqueWhite };
        private readonly Brush[] _foreBrushes = { Brushes.Blue, Brushes.DodgerBlue, Brushes.CornflowerBlue, Brushes.LightSteelBlue, Brushes.CornflowerBlue, Brushes.DodgerBlue };
        private int _backBrush;
        private int _foreBrush;

        public TouchControl()
        {
            InitializeComponent();

            SetStyle( ControlStyles.AllPaintingInWmPaint |
                      ControlStyles.UserPaint |
                      ControlStyles.ResizeRedraw |
                      ControlStyles.OptimizedDoubleBuffer, true );

            // Subscribe to all the touch events
            _touch = new TouchListener( this );
            _touch.Pan += OnPan;
            _touch.PressAndTap += OnPressAndTap;
            _touch.Rotate += OnRotate;
            _touch.TwoFingerTap += OnTwoFingerTap;
            _touch.Zoom += OnZoom;
        }

        private void OnLoad( object sender, EventArgs e )
        {
            _size = Math.Min( Width, Height ) / 2;
            _location = new Point( Width / 2, Height / 2 );
            _rotation = 0;
        }

        void OnPan( object sender, PanEventArgs e )
        {
            string msg = string.Format( "Pan Loc:({0},{1}) InertiaVector:({2},{3})", e.Location.X, e.Location.Y,
                                       e.InertiaVector.X, e.InertiaVector.Y );
            Debug.WriteLine( msg );

            if ( e.Begin )
            {
                _lastPanPoint = e.Location;
            }
            else
            {
                var offset = new Point( e.Location.X - _lastPanPoint.X, e.Location.Y - _lastPanPoint.Y );
                _location.Offset( offset );
                _lastPanPoint = e.Location;

                // Make sure it doesn't leave the screen
                if ( _location.X < 0 )
                    _location.X = 0;
                else if ( _location.X > Width )
                    _location.X = Width;

                if ( _location.Y < 0 )
                    _location.Y = 0;
                else if ( _location.Y > Height )
                    _location.Y = Height;

                Invalidate();
            }
        }

        void OnPressAndTap( object sender, PressAndTapEventArgs e )
        {
            string msg = string.Format( "PressAndTap Loc:({0},{1}) Distance:{2})", e.Location.X, e.Location.Y, e.Distance );
            Debug.WriteLine( msg );

            if ( e.Begin )
            {
                if (++_backBrush >= _backBrushes.Length)
                {
                    _backBrush = 0;
                }
                Invalidate();
            }
        }

        void OnRotate( object sender, RotateEventArgs e )
        {
            string msg = string.Format( "Rotate Loc:({0},{1}) Angle:{2}", e.Location.X, e.Location.Y, e.Degrees );
            Debug.WriteLine( msg );

            if ( e.End )
            {
                _lastRotation = 0;
            }
            else if ( !e.Begin )
            {
                float change = _lastRotation - (float)e.Degrees;
                _rotation += change;
                _lastRotation = (float)e.Degrees;
                Invalidate();
            }
        }

        void OnTwoFingerTap( object sender, TwoFingerTapEventArgs e )
        {
            string msg = string.Format( "TwoFingerTap Loc:({0},{1}) Distance:{2}", e.Location.X, e.Location.Y, e.Distance );
            Debug.WriteLine( msg );

            if ( e.Begin )
            {
                if (++_foreBrush >= _foreBrushes.Length)
                {
                    _foreBrush = 0;
                }
                Invalidate();
            }
        }

        void OnZoom( object sender, ZoomEventArgs e )
        {
            string msg = string.Format( "Zoom Loc:({0},{1}) Distance:{2}", e.Location.X, e.Location.Y, e.Distance );
            Debug.WriteLine( msg );

            if ( e.Begin )
            {
                _lastZoom = e.Distance;
            }
            else
            {
                float percent = (float)e.Distance / _lastZoom;
                _size = (int)(_size * percent);
                _lastZoom = e.Distance;
                Invalidate();
            }
        }

        protected override void OnPaint( PaintEventArgs e )
        {
            // Draw Background
            e.Graphics.FillRectangle( _backBrushes[_backBrush], 0, 0, Width, Height );

            // Draw Info
            string info = string.Format("Pos:{0} Size:{1} Rotation:{2}", _location, _size, _rotation);
            e.Graphics.DrawString( info, SystemFonts.DefaultFont, Brushes.Black, 5, 5 );

            // Draw Square
            e.Graphics.TranslateTransform( _location.X, _location.Y );
            e.Graphics.RotateTransform( _rotation );
            e.Graphics.TranslateTransform( -_location.X, -_location.Y );
            e.Graphics.FillRectangle( _foreBrushes[_foreBrush], _location.X - _size / 2, _location.Y - _size / 2, _size, _size );
        }
    }
}
