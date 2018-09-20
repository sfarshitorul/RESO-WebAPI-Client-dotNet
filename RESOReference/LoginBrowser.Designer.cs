namespace RESOReference
{
    partial class LoginBrowser
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
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.lbl_Password = new System.Windows.Forms.Label();
            this.edit_Password = new System.Windows.Forms.TextBox();
            this.lbl_UserName = new System.Windows.Forms.Label();
            this.edit_UserName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser1.Location = new System.Drawing.Point(2, 84);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(548, 387);
            this.webBrowser1.TabIndex = 0;
            this.webBrowser1.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.webBrowser1_Navigated);
            this.webBrowser1.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowser1_Navigating);
            // 
            // lbl_Password
            // 
            this.lbl_Password.AutoSize = true;
            this.lbl_Password.Location = new System.Drawing.Point(55, 41);
            this.lbl_Password.Name = "lbl_Password";
            this.lbl_Password.Size = new System.Drawing.Size(56, 13);
            this.lbl_Password.TabIndex = 128;
            this.lbl_Password.Text = "Password:";
            // 
            // edit_Password
            // 
            this.edit_Password.Location = new System.Drawing.Point(117, 39);
            this.edit_Password.Name = "edit_Password";
            this.edit_Password.Size = new System.Drawing.Size(379, 20);
            this.edit_Password.TabIndex = 126;
            // 
            // lbl_UserName
            // 
            this.lbl_UserName.AutoSize = true;
            this.lbl_UserName.Location = new System.Drawing.Point(10, 19);
            this.lbl_UserName.Name = "lbl_UserName";
            this.lbl_UserName.Size = new System.Drawing.Size(99, 13);
            this.lbl_UserName.TabIndex = 127;
            this.lbl_UserName.Text = "            User Name:";
            // 
            // edit_UserName
            // 
            this.edit_UserName.Location = new System.Drawing.Point(117, 12);
            this.edit_UserName.Name = "edit_UserName";
            this.edit_UserName.Size = new System.Drawing.Size(380, 20);
            this.edit_UserName.TabIndex = 125;
            // 
            // LoginBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 483);
            this.Controls.Add(this.lbl_Password);
            this.Controls.Add(this.edit_Password);
            this.Controls.Add(this.lbl_UserName);
            this.Controls.Add(this.edit_UserName);
            this.Controls.Add(this.webBrowser1);
            this.Name = "LoginBrowser";
            this.Text = "LoginBrowser";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Label lbl_Password;
        private System.Windows.Forms.TextBox edit_Password;
        private System.Windows.Forms.Label lbl_UserName;
        private System.Windows.Forms.TextBox edit_UserName;
    }
}