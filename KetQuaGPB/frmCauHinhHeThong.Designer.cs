namespace KetQuaGPB
{
    partial class frmCauHinhHeThong
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
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtServer = new DevExpress.XtraEditors.TextEdit();
            this.txtDBName = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtUserName = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txtPass = new DevExpress.XtraEditors.TextEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnTestConnet = new DevExpress.XtraEditors.SimpleButton();
            this.lblRs = new DevExpress.XtraEditors.LabelControl();
            this.btnSaveConfig = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtServer.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDBName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPass.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.labelControl1.Location = new System.Drawing.Point(29, 25);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(43, 16);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Server:";
            // 
            // txtServer
            // 
            this.txtServer.EditValue = "localhost";
            this.txtServer.Location = new System.Drawing.Point(72, 24);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(368, 20);
            this.txtServer.TabIndex = 1;
            // 
            // txtDBName
            // 
            this.txtDBName.EditValue = "gpb";
            this.txtDBName.Location = new System.Drawing.Point(86, 50);
            this.txtDBName.Name = "txtDBName";
            this.txtDBName.Size = new System.Drawing.Size(354, 20);
            this.txtDBName.TabIndex = 3;
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.labelControl2.Location = new System.Drawing.Point(29, 51);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(57, 16);
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "DB Name:";
            // 
            // txtUserName
            // 
            this.txtUserName.EditValue = "gpb";
            this.txtUserName.Location = new System.Drawing.Point(93, 76);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(156, 20);
            this.txtUserName.TabIndex = 5;
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.labelControl3.Location = new System.Drawing.Point(29, 77);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(64, 16);
            this.labelControl3.TabIndex = 4;
            this.labelControl3.Text = "UserName:";
            // 
            // txtPass
            // 
            this.txtPass.EditValue = "123456";
            this.txtPass.Location = new System.Drawing.Point(339, 76);
            this.txtPass.Name = "txtPass";
            this.txtPass.Properties.PasswordChar = '*';
            this.txtPass.Size = new System.Drawing.Size(156, 20);
            this.txtPass.TabIndex = 7;
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.labelControl4.Location = new System.Drawing.Point(275, 77);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(62, 16);
            this.labelControl4.TabIndex = 6;
            this.labelControl4.Text = "PassWord:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(384, 134);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(98, 36);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnTestConnet
            // 
            this.btnTestConnet.Location = new System.Drawing.Point(29, 102);
            this.btnTestConnet.Name = "btnTestConnet";
            this.btnTestConnet.Size = new System.Drawing.Size(98, 24);
            this.btnTestConnet.TabIndex = 10;
            this.btnTestConnet.Text = "Test Connect";
            this.btnTestConnet.Click += new System.EventHandler(this.btnTestConnet_Click);
            // 
            // lblRs
            // 
            this.lblRs.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.lblRs.Location = new System.Drawing.Point(133, 106);
            this.lblRs.Name = "lblRs";
            this.lblRs.Size = new System.Drawing.Size(14, 16);
            this.lblRs.TabIndex = 11;
            this.lblRs.Text = "no";
            // 
            // btnSaveConfig
            // 
            this.btnSaveConfig.Location = new System.Drawing.Point(133, 134);
            this.btnSaveConfig.Name = "btnSaveConfig";
            this.btnSaveConfig.Size = new System.Drawing.Size(98, 36);
            this.btnSaveConfig.TabIndex = 9;
            this.btnSaveConfig.Text = "Save";
            this.btnSaveConfig.Click += new System.EventHandler(this.btnSaveConfig_Click);
            // 
            // frmCauHinhHeThong
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 194);
            this.Controls.Add(this.lblRs);
            this.Controls.Add(this.btnTestConnet);
            this.Controls.Add(this.btnSaveConfig);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtPass);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.txtDBName);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.txtServer);
            this.Controls.Add(this.labelControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCauHinhHeThong";
            this.Text = "frmCauHinhHeThong";
            this.Load += new System.EventHandler(this.frmCauHinhHeThong_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtServer.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDBName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPass.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtServer;
        private DevExpress.XtraEditors.TextEdit txtDBName;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txtUserName;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit txtPass;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnTestConnet;
        private DevExpress.XtraEditors.LabelControl lblRs;
        private DevExpress.XtraEditors.SimpleButton btnSaveConfig;
    }
}