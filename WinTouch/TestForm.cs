using System.Windows.Forms;

namespace WinTouch
{
   public partial class TestForm : Form
   {
      private TouchListener _touch;

      public TestForm()
      {
         InitializeComponent();
         _touch = new TouchListener( this );
      }
   }
}
