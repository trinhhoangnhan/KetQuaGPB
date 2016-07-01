namespace KetQuaGPB
{
    partial class frmLogin
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
            this.txtUserName = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtPassword = new DevExpress.XtraEditors.TextEdit();
            this.btnLogin = new DevExpress.XtraEditors.SimpleButton();
            this.lblMessage = new DevExpress.XtraEditors.LabelControl();
            this.lblMessUserName = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // txtUserName
            // 
            this.txtUserName.EnterMoveNextControl = true;
            this.txtUserName.Location = new System.Drawing.Point(98, 24);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Properties.AutoHeight = false;
            this.txtUserName.Size = new System.Drawing.Size(177, 27);
            this.txtUserName.TabIndex = 0;
            this.txtUserName.Validated += new System.EventHandler(this.txtUserName_Validated);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(18, 27);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(76, 13);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "Tên đăng nhập:";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(162, 125);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 33);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Thoát";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(46, 63);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(48, 13);
            this.labelControl2.TabIndex = 4;
            this.labelControl2.Text = "Mật khẩu:";
            // 
            // txtPassword
            // 
            this.txtPassword.EnterMoveNextControl = true;
            this.txtPassword.Location = new System.Drawing.Point(98, 60);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Properties.AutoHeight = false;
            this.txtPassword.Properties.PasswordChar = '*';
            this.txtPassword.Properties.UseSystemPasswordChar = true;
            this.txtPassword.Properties.ValidateOnEnterKey = true;
            this.txtPassword.Size = new System.Drawing.Size(177, 27);
            this.txtPassword.TabIndex = 1;
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(46, 125);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 33);
            this.btnLogin.TabIndex = 2;
            this.btnLogin.Text = "Đăng nhập";
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Italic);
            this.lblMessage.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblMessage.Location = new System.Drawing.Point(18, 93);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(78, 14);
            this.lblMessage.TabIndex = 6;
            this.lblMessage.Text = "nnnnnnnnnnn";
            // 
            // lblMessUserName
            // 
            this.lblMessUserName.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Italic);
            this.lblMessUserName.Appearance.ForeColor = System.Drawing.Color.Red;
            this.lblMessUserName.Location = new System.Drawing.Point(98, 41);
            this.lblMessUserName.Name = "lblMessUserName";
            this.lblMessUserName.Size = new System.Drawing.Size(0, 14);
            this.lblMessUserName.TabIndex = 7;
            // 
            // frmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(287, 187);
            this.Controls.Add(this.lblMessUserName);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.txtUserName);
            this.DoubleBuffered = true;
            this.Name = "frmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đăng nhập";
            this.Load += new System.EventHandler(this.frmLogin_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmLogin_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit txtUserName;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txtPassword;
        private DevExpress.XtraEditors.SimpleButton btnLogin;
        private DevExpress.XtraEditors.LabelControl lblMessage;
        private DevExpress.XtraEditors.LabelControl lblMessUserName;
    }
}