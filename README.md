WinTouch.NET
============

WinTouch.NET is a .NET library for handling Windows 7 Touch Gestures in a WinForms application.

It allows you to handle the following gestures,

 - Pan
 - Rotate
 - Zoom
 - Two Finger Tap
 - Press and Tap

For more information on Windows Gestures, see [Windows Touch Gestures Overview](http://msdn.microsoft.com/en-us/library/windows/desktop/dd940543%28v=vs.85%29.aspx) on MSDN.

The library has the following features,

 - Fails gracefully on operating systems older than Windows 7
 - Compiled against .NET 2.0 for maximum compatibility
 - Allows you to subscribe to just the events that you are interested in.

## Usage ##

1. Add a reference to `WinTouch.dll` to your project.
2. Create a `GestureListener` passing in the control you want to receive touch events for.
3. Optionally pass in a collection of `GestureConfig` objects to the `GestureListener` constructor to specify which gestures you want it to capture events for. By default, it captures `GestureConfigurationFlag.GC_ALLGESTURES`.
4. On the `GestureListener` object you created, subscribe to the gesture events that you are interested in. It supports `Pan`, `PressAndTap`, `Rotate`, `TwoFingerTap` and `Zoom`.


For a demo on how to use the library, see the Demo project on [GitHub](https://github.com/rprouse/WinTouch.NET). Here is an example,

```C#
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

        m_image = Properties.Resources.windows_logo;
    }

    private void OnLoad( object sender, EventArgs e )
    {
        ResetImage();
    }

    private void ResetImage()
    {
        // Center the image on the form and make it half the width
        // of the shortest side
        _size = Math.Min( Width, Height ) / 2;
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
```