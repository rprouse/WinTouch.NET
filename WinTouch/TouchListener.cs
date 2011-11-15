using System;
using System.Windows.Forms;

namespace WinTouch
{
   [System.Security.Permissions.PermissionSet( System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust" )]
   public class TouchListener : NativeWindow
   {        // Constant value was found in the "windows.h" header file.
      private const int WM_ACTIVATEAPP = 0x001C;

      public TouchListener( Form parent )
      {
         parent.HandleCreated += OnHandleCreated;
         parent.HandleDestroyed += OnHandleDestroyed;
      }

      void OnHandleCreated( object sender, EventArgs e )
      {            
         // Window is now created, assign handle to NativeWindow.
         var form = sender as Form;
         if ( form != null )
         {
            AssignHandle(form.Handle);
         }
      }

      void OnHandleDestroyed( object sender, EventArgs e )
      {            
         // Window was destroyed, release hook.
         ReleaseHandle();
      }

      [System.Security.Permissions.PermissionSet( System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust" )]
      protected override void WndProc( ref Message m )
      {
         // Listen for operating system messages
         switch ( m.Msg )
         {
            case WM_ACTIVATEAPP:

               // Notify the form that this message was received.
               // Application is activated or deactivated, 
               // based upon the WParam parameter.
               System.Diagnostics.Debug.WriteLine( "Application Active = " + ((int)m.WParam != 0).ToString() );

               break;
         }
         base.WndProc( ref m );
      }
   }
}
