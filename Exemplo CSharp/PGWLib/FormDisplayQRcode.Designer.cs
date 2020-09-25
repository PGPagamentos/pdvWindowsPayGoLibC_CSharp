namespace PGWLib
{
    partial class FormDisplayQRcode
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
            this.lblMessage = new System.Windows.Forms.Label();
            this.pbQRcode = new System.Windows.Forms.PictureBox();
            this.tbQRcode = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbQRcode)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.Font = new System.Drawing.Font("Courier New", 24F, System.Drawing.FontStyle.Bold);
            this.lblMessage.Location = new System.Drawing.Point(47, 9);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(395, 144);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbQRcode
            // 
            this.pbQRcode.Location = new System.Drawing.Point(47, 158);
            this.pbQRcode.Name = "pbQRcode";
            this.pbQRcode.Size = new System.Drawing.Size(395, 395);
            this.pbQRcode.TabIndex = 1;
            this.pbQRcode.TabStop = false;
            // 
            // tbQRcode
            // 
            this.tbQRcode.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbQRcode.Font = new System.Drawing.Font("Courier New", 24F, System.Drawing.FontStyle.Bold);
            this.tbQRcode.Location = new System.Drawing.Point(47, 158);
            this.tbQRcode.Multiline = true;
            this.tbQRcode.Name = "tbQRcode";
            this.tbQRcode.ReadOnly = true;
            this.tbQRcode.Size = new System.Drawing.Size(395, 395);
            this.tbQRcode.TabIndex = 2;
            this.tbQRcode.TabStop = false;
            this.tbQRcode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbQRcode_KeyUp);
            // 
            // FormDisplayQRcode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(503, 612);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.pbQRcode);
            this.Controls.Add(this.tbQRcode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormDisplayQRcode";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormDisplayQRcode";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormDisplayQRcode_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pbQRcode)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.PictureBox pbQRcode;
        private System.Windows.Forms.TextBox tbQRcode;
    }
}