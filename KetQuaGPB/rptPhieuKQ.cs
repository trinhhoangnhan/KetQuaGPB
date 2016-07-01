using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace KetQuaGPB
{
    public partial class rptPhieuKQ : DevExpress.XtraReports.UI.XtraReport
    {
        public rptPhieuKQ()
        {
            InitializeComponent();
        }
        
        private void txtketluan2_PrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            //if (e.PageIndex > 0)
            //{
            //    xrTable4.Visible = true;
            //}
            //else
            //{
            //    xrTable4.Visible = false;
            //}
            
        }

        private void txtketqualan_PrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            //XRLabel lbl = (XRLabel)sender;
            //int i = e.PageIndex;
            //if (e.PageIndex > 0)
            //{
            //    xrTable4.Visible = true;
            //    this.xrTable4.LocationFloat = new DevExpress.Utils.PointFloat(3.000005F, 0F);
            //    this.piclan1.LocationFloat = new DevExpress.Utils.PointFloat(50.60675F, 36.6F);
            //    this.piclan2.LocationFloat = new DevExpress.Utils.PointFloat(369.5F, 36.6F);
            //}
            //else
            //    xrTable4.Visible = false;

        }


    }
}
