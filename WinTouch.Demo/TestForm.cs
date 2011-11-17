using System.Diagnostics;
using System.Windows.Forms;
using WinTouch;

namespace Alteridem.WinTouch.Demo
{
    public partial class TestForm : Form
    {
        private TouchListener _touch;

        public TestForm()
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
