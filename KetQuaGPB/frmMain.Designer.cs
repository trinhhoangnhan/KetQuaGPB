namespace KetQuaGPB
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.btnSetting = new DevExpress.XtraBars.BarButtonItem();
            this.btnLogin = new DevExpress.XtraBars.BarButtonItem();
            this.btnNhapDSDoc = new DevExpress.XtraBars.BarButtonItem();
            this.btnBSChiDinh = new DevExpress.XtraBars.BarButtonItem();
            this.btnDSBenhPham = new DevExpress.XtraBars.BarButtonItem();
            this.btnDVGuiMau = new DevExpress.XtraBars.BarButtonItem();
            this.btnVietTac = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.btnNhapKQGPB = new DevExpress.XtraBars.BarButtonItem();
            this.btnNhapBN = new DevExpress.XtraBars.BarButtonItem();
            this.btnTimBN = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.bibCauHinh = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup5 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribBaoCao = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.xtraTabbedMdiManager1 = new DevExpress.XtraTabbedMdi.XtraTabbedMdiManager(this.components);
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabbedMdiManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.ApplicationButtonText = null;
            // 
            // 
            // 
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.ExpandCollapseItem.Name = "";
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.btnSetting,
            this.btnLogin,
            this.btnNhapDSDoc,
            this.btnBSChiDinh,
            this.btnDSBenhPham,
            this.btnDVGuiMau,
            this.btnVietTac,
            this.barButtonItem1,
            this.btnNhapKQGPB,
            this.btnNhapBN,
            this.btnTimBN,
            this.barButtonItem2});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 14;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1,
            this.bibCauHinh,
            this.ribBaoCao});
            this.ribbonControl1.Size = new System.Drawing.Size(984, 145);
            // 
            // btnSetting
            // 
            this.btnSetting.Caption = "Cấu hình hệ thống";
            this.btnSetting.Id = 1;
            this.btnSetting.Name = "btnSetting";
            this.btnSetting.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnSetting_ItemClick);
            // 
            // btnLogin
            // 
            this.btnLogin.Caption = "Đăng nhập";
            this.btnLogin.Id = 2;
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnLogin_ItemClick);
            // 
            // btnNhapDSDoc
            // 
            this.btnNhapDSDoc.Caption = "Bác sĩ đọc kết quả";
            this.btnNhapDSDoc.Id = 3;
            this.btnNhapDSDoc.Name = "btnNhapDSDoc";
            // 
            // btnBSChiDinh
            // 
            this.btnBSChiDinh.Caption = "Bác sĩ chỉ đinh";
            this.btnBSChiDinh.Id = 4;
            this.btnBSChiDinh.Name = "btnBSChiDinh";
            // 
            // btnDSBenhPham
            // 
            this.btnDSBenhPham.Caption = "DS bệnh phẩm";
            this.btnDSBenhPham.Id = 6;
            this.btnDSBenhPham.Name = "btnDSBenhPham";
            // 
            // btnDVGuiMau
            // 
            this.btnDVGuiMau.Caption = "Đơn vị gửi mẫu";
            this.btnDVGuiMau.Id = 7;
            this.btnDVGuiMau.Name = "btnDVGuiMau";
            // 
            // btnVietTac
            // 
            this.btnVietTac.Caption = "Viết tắc";
            this.btnVietTac.Id = 8;
            this.btnVietTac.Name = "btnVietTac";
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "Thống kê số lượt";
            this.barButtonItem1.Id = 9;
            this.barButtonItem1.Name = "barButtonItem1";
            // 
            // btnNhapKQGPB
            // 
            this.btnNhapKQGPB.Caption = "Nhập kết quả GPB";
            this.btnNhapKQGPB.Id = 10;
            this.btnNhapKQGPB.Name = "btnNhapKQGPB";
            this.btnNhapKQGPB.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnNhapKQGPB_ItemClick);
            // 
            // btnNhapBN
            // 
            this.btnNhapBN.Caption = "Nhập bệnh nhân";
            this.btnNhapBN.Id = 11;
            this.btnNhapBN.Name = "btnNhapBN";
            this.btnNhapBN.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnNhapBN_ItemClick);
            // 
            // btnTimBN
            // 
            this.btnTimBN.Caption = "Tìm bệnh nhân";
            this.btnTimBN.Id = 12;
            this.btnTimBN.Name = "btnTimBN";
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "Hệ thống";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.btnSetting);
            this.ribbonPageGroup1.ItemLinks.Add(this.btnLogin);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            // 
            // bibCauHinh
            // 
            this.bibCauHinh.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup2,
            this.ribbonPageGroup4,
            this.ribbonPageGroup5});
            this.bibCauHinh.Name = "bibCauHinh";
            this.bibCauHinh.Text = "Chức năng";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.btnNhapDSDoc);
            this.ribbonPageGroup2.ItemLinks.Add(this.btnBSChiDinh);
            this.ribbonPageGroup2.ItemLinks.Add(this.btnDSBenhPham);
            this.ribbonPageGroup2.ItemLinks.Add(this.btnDVGuiMau);
            this.ribbonPageGroup2.ItemLinks.Add(this.btnVietTac);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "Danh mục";
            // 
            // ribbonPageGroup4
            // 
            this.ribbonPageGroup4.ItemLinks.Add(this.btnNhapKQGPB);
            this.ribbonPageGroup4.ItemLinks.Add(this.barButtonItem2);
            this.ribbonPageGroup4.Name = "ribbonPageGroup4";
            this.ribbonPageGroup4.Text = "Nhập kết quả";
            // 
            // ribbonPageGroup5
            // 
            this.ribbonPageGroup5.ItemLinks.Add(this.btnNhapBN);
            this.ribbonPageGroup5.ItemLinks.Add(this.btnTimBN);
            this.ribbonPageGroup5.Name = "ribbonPageGroup5";
            this.ribbonPageGroup5.Text = "Bệnh nhân";
            // 
            // ribBaoCao
            // 
            this.ribBaoCao.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup3});
            this.ribBaoCao.Name = "ribBaoCao";
            this.ribBaoCao.Text = "Báo cáo";
            // 
            // ribbonPageGroup3
            // 
            this.ribbonPageGroup3.ItemLinks.Add(this.barButtonItem1);
            this.ribbonPageGroup3.Name = "ribbonPageGroup3";
            // 
            // xtraTabbedMdiManager1
            // 
            this.xtraTabbedMdiManager1.AppearancePage.PageClient.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.xtraTabbedMdiManager1.AppearancePage.PageClient.Options.UseBackColor = true;
            this.xtraTabbedMdiManager1.MdiParent = this;
            // 
            // barButtonItem2
            // 
            this.barButtonItem2.Caption = "barButtonItem2";
            this.barButtonItem2.Id = 13;
            this.barButtonItem2.Name = "barButtonItem2";
            this.barButtonItem2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem2_ItemClick);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 721);
            this.Controls.Add(this.ribbonControl1);
            this.IsMdiContainer = true;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nhập kết quả GPB";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabbedMdiManager1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.BarButtonItem btnSetting;
        private DevExpress.XtraBars.BarButtonItem btnLogin;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonPage bibCauHinh;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribBaoCao;
        private DevExpress.XtraBars.BarButtonItem btnNhapDSDoc;
        private DevExpress.XtraBars.BarButtonItem btnBSChiDinh;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.BarButtonItem btnDSBenhPham;
        private DevExpress.XtraBars.BarButtonItem btnDVGuiMau;
        private DevExpress.XtraBars.BarButtonItem btnVietTac;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
        private DevExpress.XtraBars.BarButtonItem btnNhapKQGPB;
        private DevExpress.XtraTabbedMdi.XtraTabbedMdiManager xtraTabbedMdiManager1;
        private DevExpress.XtraBars.BarButtonItem btnNhapBN;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup5;
        private DevExpress.XtraBars.BarButtonItem btnTimBN;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;

    }
}