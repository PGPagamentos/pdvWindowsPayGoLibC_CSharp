namespace PGWLib
{
    partial class FormMenu
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
            this.LblHeader = new System.Windows.Forms.Label();
            this.LstMenu = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // LblHeader
            // 
            this.LblHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.LblHeader.Font = new System.Drawing.Font("Verdana", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblHeader.Location = new System.Drawing.Point(0, 0);
            this.LblHeader.Name = "LblHeader";
            this.LblHeader.Size = new System.Drawing.Size(603, 38);
            this.LblHeader.TabIndex = 0;
            this.LblHeader.Text = "MENU PRINCIPAL";
            this.LblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LstMenu
            // 
            this.LstMenu.Font = new System.Drawing.Font("Verdana", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LstMenu.FormattingEnabled = true;
            this.LstMenu.ItemHeight = 38;
            this.LstMenu.Location = new System.Drawing.Point(12, 59);
            this.LstMenu.Name = "LstMenu";
            this.LstMenu.Size = new System.Drawing.Size(579, 346);
            this.LstMenu.TabIndex = 1;
            this.LstMenu.MouseClick += new System.Windows.Forms.MouseEventHandler(this.LstMenu_MouseClick);
            this.LstMenu.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyPressed);
            // 
            // FormMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(603, 440);
            this.Controls.Add(this.LstMenu);
            this.Controls.Add(this.LblHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMenu";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SELECIONE UMA OPÇÃO";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormMenu_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyPressed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LblHeader;
        private System.Windows.Forms.ListBox LstMenu;
    }
}