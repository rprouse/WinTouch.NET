namespace Alteridem.WinTouch.Demo
{
   partial class TestForm
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose( bool disposing )
      {
         if ( disposing && (components != null) )
         {
            components.Dispose();
         }
         base.Dispose( disposing );
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
            this._touchControl = new Alteridem.WinTouch.Demo.TouchControl();
            this.SuspendLayout();
            // 
            // _touchControl
            // 
            this._touchControl.BackColor = System.Drawing.SystemColors.Window;
            this._touchControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._touchControl.Location = new System.Drawing.Point(0, 0);
            this._touchControl.Name = "_touchControl";
            this._touchControl.Size = new System.Drawing.Size(969, 740);
            this._touchControl.TabIndex = 0;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(969, 740);
            this.Controls.Add(this._touchControl);
            this.Name = "TestForm";
            this.Text = "Test Form";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

      }

      #endregion

      private TouchControl _touchControl;

   }
}

