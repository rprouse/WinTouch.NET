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
    public partial class GestureControl : UserControl
    {
        // Our touch listener
        private readonly GestureListener _gesture;

        // Size, location and rotation of the image
        private Point _location;
        private int _size;
        private double _rotation;

        // An image to draw
        private Image m_image;

        // Brushes
        private readonly Brush[] _backBrushes = { Brushes.White, Brushes.AntiqueWhite, Brushes.Bisque, Brushes.Wheat, Brushes.Bisque, Brushes.AntiqueWhite };
        private int _backBrush;

        public GestureControl()
        {
            InitializeComponent();

            SetStyle( ControlStyles.AllPaintingInWmPaint |
                      ControlStyles.UserPaint |
                      ControlStyles.ResizeRedraw |
                      ControlStyles.OptimizedDoubleBuffer, true );

            // Subscribe to all the touch events
            _gesture = new GestureListener( this );
            _gesture.Pan += OnPan;
            _gesture.PressAndTap += OnPressAndTap;
            _gesture.Rotate += OnRotate;
            _gesture.TwoFingerTap += OnTwoFingerTap;
            _gesture.Zoom += OnZoom;

            m_image = Properties.Resources.icon;
        }

        private void OnLoad( object sender, EventArgs e )
        {
            ResetImage();
        }

        private void ResetImage()
        {
            // Center the image on the form and reset its size
            _size = Math.Min( m_image.Width, m_image.Height ) / 2;
            _location = new Point( Width / 2, Height / 2 );
            _rotation = 0;
        }

        void OnPan( object sender, PanEventArgs e )
        {
            string msg = string.Format( "Pan Loc:({0},{1}) InertiaVector:({2},{3})", e.Location.X, e.Location.Y,
                                       e.InertiaVector.X, e.InertiaVector.Y );
            Debug.WriteLine( msg );

            if ( !e.Begin )
            {
                _location.Offset( e.PanOffset );

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
                if ( ++_backBrush >= _backBrushes.Length )
                {
                    _backBrush = 0;
                }
                Invalidate();
            }
        }

        void OnRotate( object sender, RotateEventArgs e )
        {
            string msg = string.Format( "Rotate Loc:({0},{1}) Angle:{2}", e.Location.X, e.Location.Y, e.TotalDegrees );
            Debug.WriteLine( msg );

            if ( !e.Begin && !e.End )
            {
                _rotation -= e.Degrees;
                Invalidate();
            }
        }

        void OnTwoFingerTap( object sender, TwoFingerTapEventArgs e )
        {
            string msg = string.Format( "TwoFingerTap Loc:({0},{1}) Distance:{2}", e.Location.X, e.Location.Y, e.Distance );
            Debug.WriteLine( msg );

            if ( e.Begin )
            {
                ResetImage();
                Invalidate();
            }
        }

        void OnZoom( object sender, ZoomEventArgs e )
        {
            string msg = string.Format( "Zoom Loc:({0},{1}) Distance:{2}", e.Location.X, e.Location.Y, e.Distance );
            Debug.WriteLine( msg );

            if ( !e.Begin )
            {
                _size = (int)(_size * e.PercentChange);
                Invalidate();
            }
        }

        protected override void OnPaint( PaintEventArgs e )
        {
            // Draw Background
            e.Graphics.FillRectangle( _backBrushes[_backBrush], 0, 0, Width, Height );

            // Draw Info
            string info = string.Format( "Pos:{0} Size:{1} Rotation:{2}", _location, _size, _rotation );
            e.Graphics.DrawString( info, SystemFonts.DefaultFont, Brushes.Black, 5, 5 );

            // Draw Square
            e.Graphics.TranslateTransform( _location.X, _location.Y );
            e.Graphics.RotateTransform( (float)_rotation );
            e.Graphics.TranslateTransform( -_location.X, -_location.Y );
            e.Graphics.DrawImage( m_image, _location.X - _size / 2, _location.Y - _size / 2, _size, _size );
        }
    }
}
