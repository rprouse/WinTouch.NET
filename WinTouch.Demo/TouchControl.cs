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

using System.Diagnostics;
using System.Windows.Forms;

#endregion

namespace Alteridem.WinTouch.Demo
{
    public partial class TouchControl : UserControl
    {
        private TouchListener _touch;

        public TouchControl()
        {
            InitializeComponent();
            _touch = new TouchListener( this );
            _touch.Pan += OnPan;
            _touch.PressAndTap += OnPressAndTap;
            _touch.Rotate += OnRotate;
            _touch.TwoFingerTap += OnTwoFingerTap;
            _touch.Zoom += OnZoom;
        }

        void OnPan( object sender, PanEventArgs e )
        {
            string msg = string.Format( "Pan Loc:({0},{1}) InertiaVector:({2},{3})", e.Location.X, e.Location.Y,
                                       e.InertiaVector.X, e.InertiaVector.Y );
            Debug.WriteLine( msg );
        }

        void OnPressAndTap( object sender, PressAndTapEventArgs e )
        {
            string msg = string.Format( "PressAndTap Loc:({0},{1}) Distance:{2})", e.Location.X, e.Location.Y, e.Distance );
            Debug.WriteLine( msg );
        }

        void OnRotate( object sender, RotateEventArgs e )
        {
            string msg = string.Format( "Rotate Loc:({0},{1}) Angle:{2}", e.Location.X, e.Location.Y, e.Degrees );
            Debug.WriteLine( msg );
        }

        void OnTwoFingerTap( object sender, TwoFingerTapEventArgs e )
        {
            string msg = string.Format( "TwoFingerTap Loc:({0},{1}) Distance:{2}", e.Location.X, e.Location.Y, e.Distance );
            Debug.WriteLine( msg );
        }

        void OnZoom( object sender, ZoomEventArgs e )
        {
            string msg = string.Format( "Zoom Loc:({0},{1}) Distance:{2}", e.Location.X, e.Location.Y, e.Distance );
            Debug.WriteLine( msg );
        }
    }
}
