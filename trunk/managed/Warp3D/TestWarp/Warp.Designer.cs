namespace TestWarp
{
	partial class Warp
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.Load_Scene = new System.Windows.Forms.Button();
            this.button_Flare1 = new System.Windows.Forms.Button();
            this.button_Stones = new System.Windows.Forms.Button();
            this.button_Bounce = new System.Windows.Forms.Button();
            this.button_Studio = new System.Windows.Forms.Button();
            this.button_Physics = new System.Windows.Forms.Button();
            this.button_Body = new System.Windows.Forms.Button();
            this.button_Sphere = new System.Windows.Forms.Button();
            this.warp3D1 = new Warp3D.Warp3D();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Load_Scene
            // 
            this.Load_Scene.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
            this.Load_Scene.Location = new System.Drawing.Point( 12, 503 );
            this.Load_Scene.Name = "Load_Scene";
            this.Load_Scene.Size = new System.Drawing.Size( 75, 23 );
            this.Load_Scene.TabIndex = 1;
            this.Load_Scene.Text = "Chair";
            this.Load_Scene.Click += new System.EventHandler( this.Load_Scene_Click );
            // 
            // button_Flare1
            // 
            this.button_Flare1.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
            this.button_Flare1.Location = new System.Drawing.Point( 93, 503 );
            this.button_Flare1.Name = "button_Flare1";
            this.button_Flare1.Size = new System.Drawing.Size( 75, 23 );
            this.button_Flare1.TabIndex = 2;
            this.button_Flare1.Text = "Flare";
            this.button_Flare1.Click += new System.EventHandler( this.button_Flare1_Click );
            // 
            // button_Stones
            // 
            this.button_Stones.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
            this.button_Stones.Location = new System.Drawing.Point( 174, 503 );
            this.button_Stones.Name = "button_Stones";
            this.button_Stones.Size = new System.Drawing.Size( 75, 23 );
            this.button_Stones.TabIndex = 3;
            this.button_Stones.Text = "Stones";
            this.button_Stones.Click += new System.EventHandler( this.button_Stones_Click );
            // 
            // button_Bounce
            // 
            this.button_Bounce.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
            this.button_Bounce.Location = new System.Drawing.Point( 255, 503 );
            this.button_Bounce.Name = "button_Bounce";
            this.button_Bounce.Size = new System.Drawing.Size( 75, 23 );
            this.button_Bounce.TabIndex = 4;
            this.button_Bounce.Text = "Objects";
            this.button_Bounce.Click += new System.EventHandler( this.button_Bounce_Click );
            // 
            // button_Studio
            // 
            this.button_Studio.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
            this.button_Studio.Location = new System.Drawing.Point( 336, 503 );
            this.button_Studio.Name = "button_Studio";
            this.button_Studio.Size = new System.Drawing.Size( 75, 23 );
            this.button_Studio.TabIndex = 5;
            this.button_Studio.Text = "3Ds";
            this.button_Studio.Click += new System.EventHandler( this.button_Studio_Click );
            // 
            // button_Physics
            // 
            this.button_Physics.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
            this.button_Physics.Location = new System.Drawing.Point( 417, 503 );
            this.button_Physics.Name = "button_Physics";
            this.button_Physics.Size = new System.Drawing.Size( 75, 23 );
            this.button_Physics.TabIndex = 6;
            this.button_Physics.Text = "Physics";
            this.button_Physics.Click += new System.EventHandler( this.button_Physics_Click );
            // 
            // button_Body
            // 
            this.button_Body.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.button_Body.Enabled = false;
            this.button_Body.Location = new System.Drawing.Point( 450, 12 );
            this.button_Body.Name = "button_Body";
            this.button_Body.Size = new System.Drawing.Size( 42, 23 );
            this.button_Body.TabIndex = 10;
            this.button_Body.Text = "cube";
            this.button_Body.Click += new System.EventHandler( this.button_Body_Click );
            // 
            // button_Sphere
            // 
            this.button_Sphere.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.button_Sphere.Enabled = false;
            this.button_Sphere.Location = new System.Drawing.Point( 450, 41 );
            this.button_Sphere.Name = "button_Sphere";
            this.button_Sphere.Size = new System.Drawing.Size( 42, 23 );
            this.button_Sphere.TabIndex = 11;
            this.button_Sphere.Text = "sphere";
            this.button_Sphere.Click += new System.EventHandler( this.button_Sphere_Click );
            // 
            // warp3D1
            // 
            this.warp3D1.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.warp3D1.Location = new System.Drawing.Point( 12, 12 );
            this.warp3D1.Name = "warp3D1";
            this.warp3D1.Size = new System.Drawing.Size( 429, 462 );
            this.warp3D1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 12, 542 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 351, 13 );
            this.label1.TabIndex = 12;
            this.label1.Text = "Use the mouse to rotate the scene. Hold down control to zoom in and out.";
            // 
            // Warp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 503, 564 );
            this.Controls.Add( this.label1 );
            this.Controls.Add( this.button_Sphere );
            this.Controls.Add( this.button_Body );
            this.Controls.Add( this.button_Physics );
            this.Controls.Add( this.button_Studio );
            this.Controls.Add( this.button_Bounce );
            this.Controls.Add( this.button_Stones );
            this.Controls.Add( this.button_Flare1 );
            this.Controls.Add( this.Load_Scene );
            this.Controls.Add( this.warp3D1 );
            this.Name = "Warp";
            this.Text = "Warp Demos";
            this.Load += new System.EventHandler( this.Form1_Load );
            this.ResumeLayout( false );
            this.PerformLayout();

		}

		#endregion

		private Warp3D.Warp3D warp3D1;
		private System.Windows.Forms.Button Load_Scene;
        private System.Windows.Forms.Button button_Flare1;
        private System.Windows.Forms.Button button_Stones;
        private System.Windows.Forms.Button button_Bounce;
        private System.Windows.Forms.Button button_Studio;
        private System.Windows.Forms.Button button_Physics;
		private System.Windows.Forms.Button button_Body;
		private System.Windows.Forms.Button button_Sphere;
        private System.Windows.Forms.Label label1;


	}
}

