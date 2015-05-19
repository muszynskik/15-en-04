namespace SE2Client
{
	partial class Main_Form
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
				components.Dispose( );
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent( )
		{
            this.IP_TextBox = new System.Windows.Forms.TextBox();
            this.Connect_Button = new System.Windows.Forms.Button();
            this.SendRequest_Button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // IP_TextBox
            // 
            this.IP_TextBox.Location = new System.Drawing.Point(12, 12);
            this.IP_TextBox.Name = "IP_TextBox";
            this.IP_TextBox.Size = new System.Drawing.Size(135, 20);
            this.IP_TextBox.TabIndex = 0;
            this.IP_TextBox.Text = "192.168.143.13";
            // 
            // Connect_Button
            // 
            this.Connect_Button.Location = new System.Drawing.Point(153, 10);
            this.Connect_Button.Name = "Connect_Button";
            this.Connect_Button.Size = new System.Drawing.Size(75, 23);
            this.Connect_Button.TabIndex = 1;
            this.Connect_Button.Text = "Connect";
            this.Connect_Button.UseVisualStyleBackColor = true;
            this.Connect_Button.Click += new System.EventHandler(this.Connect_Button_Click);
            // 
            // SendRequest_Button
            // 
            this.SendRequest_Button.Location = new System.Drawing.Point(12, 226);
            this.SendRequest_Button.Name = "SendRequest_Button";
            this.SendRequest_Button.Size = new System.Drawing.Size(216, 23);
            this.SendRequest_Button.TabIndex = 2;
            this.SendRequest_Button.Text = "Send request";
            this.SendRequest_Button.UseVisualStyleBackColor = true;
            this.SendRequest_Button.Click += new System.EventHandler(this.SendRequest_Button_Click);
            // 
            // Main_Form
            // 
            this.AcceptButton = this.Connect_Button;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(239, 261);
            this.Controls.Add(this.SendRequest_Button);
            this.Controls.Add(this.Connect_Button);
            this.Controls.Add(this.IP_TextBox);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Main_Form";
            this.Text = "Client";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox IP_TextBox;
		private System.Windows.Forms.Button Connect_Button;
		private System.Windows.Forms.Button SendRequest_Button;
	}
}

