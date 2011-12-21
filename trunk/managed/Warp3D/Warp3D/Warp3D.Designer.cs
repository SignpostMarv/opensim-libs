namespace Warp3D
{
	partial class Warp3D
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.SuspendLayout();
            // 
            // Warp3D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "Warp3D";
            this.Size = new System.Drawing.Size( 172, 186 );
            this.Load += new System.EventHandler( this.Warp3D_Load );
            this.KeyUp += new System.Windows.Forms.KeyEventHandler( this.Warp3D_KeyUp );
            this.KeyDown += new System.Windows.Forms.KeyEventHandler( this.Warp3D_KeyDown );
            this.ResumeLayout( false );

		}

		#endregion
	}
}
