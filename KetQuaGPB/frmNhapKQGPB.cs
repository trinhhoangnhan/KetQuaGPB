using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Net;
using System.Collections;
using System.Data.SqlClient;
using DataColection;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using DShowNET;
using DShowNET.Device;
using System.Collections;
using System.Drawing.Imaging;
using System.Xml;

namespace KetQuaGPB
{
    public partial class frmNhapKQGPB : DevExpress.XtraEditors.XtraForm, ISampleGrabberCB
    {
        public frmNhapKQGPB()
        {
            InitializeComponent();
        }

        public frmNhapKQGPB(MySqlConnection _Conn)
        {
            InitializeComponent();
            Conn = _Conn;
        }

        public frmNhapKQGPB( string strConn)
        {
            InitializeComponent();
            ConnStr = strConn;
        }


        /// <summary> sample callback, NOT USED. </summary>
        int ISampleGrabberCB.SampleCB(double SampleTime, IMediaSample pSample)
        {
            //Trace.WriteLine("!!CB: ISampleGrabberCB.SampleCB");
            return 0;
        }

        /// <summary> buffer callback, COULD BE FROM FOREIGN THREAD. </summary>
        int ISampleGrabberCB.BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            if (captured || (savedArray == null))
            {
                //Trace.WriteLine("!!CB: ISampleGrabberCB.BufferCB");
                return 0;
            }

            captured = true;
            bufferedSize = BufferLen;
            //Trace.WriteLine("!!CB: ISampleGrabberCB.BufferCB  !GRAB! size = " + BufferLen.ToString());
            if ((pBuffer != IntPtr.Zero) && (BufferLen > 1000) && (BufferLen <= savedArray.Length))
                Marshal.Copy(pBuffer, savedArray, 0, BufferLen);
            //else
            //Trace.WriteLine("    !!!GRAB! failed ");
            //this.BeginInvoke(new CaptureDone(this.OnCaptureDone));
            if (pHinh1 == 1)
            {
                this.BeginInvoke(new CaptureDone(this.OnCaptureDonePic1));
            }
            else if (pHinh2 == 1)
            {
                this.BeginInvoke(new CaptureDone(this.OnCaptureDonePic2));

            }
            return 0;
        }

        string ConnStr = "";
        Configuration Config = null;
        MySqlConnection Conn = null;
        DataTable dtCode = null;
        public static int gSoKyTuMaBN = 6;

        private ArrayList arrControlsKQ = new ArrayList();
        private ArrayList arrControlsKey = new ArrayList();
        private ArrayList arrControlsBN = new ArrayList();


        /// <summary> list of installed video devices. </summary>
        private ArrayList capDevices;
        /// <summary> control interface. </summary>
        private IMediaControl mediaCtrl;
        /// <summary> capture graph builder interface. </summary>
        private ICaptureGraphBuilder2 capGraph;
        private ISampleGrabber sampGrabber;

        /// <summary> base filter of the actually used video devices. </summary>
        private IBaseFilter capFilter;

        private System.Windows.Forms.ToolBarButton toolBarBtnTune;

        /// <summary> graph builder interface. </summary>
        private IGraphBuilder graphBuilder;

        /// <summary> structure describing the bitmap to grab. </summary>
        private VideoInfoHeader videoInfoHeader;

        private const int WM_GRAPHNOTIFY = 0x00008001;	// message from graph

        private const int WS_CHILD = 0x40000000;	// attributes for video window
        private const int WS_CLIPCHILDREN = 0x02000000;
        private const int WS_CLIPSIBLINGS = 0x04000000;
        /// <summary> event when callback has finished (ISampleGrabberCB.BufferCB). </summary>
        private delegate void CaptureDone();
        /// <summary> flag to detect first Form appearance </summary>
        private bool firstActive;

        /// <summary> buffer for bitmap data. </summary>
        private byte[] savedArray;
        private bool captured = true;
        private int bufferedSize;
        private int pHinh1 = 0, pHinh2 = 0;
#if DEBUG
        private int rotCookie = 0;

#endif
        /// <summary> start all the interfaces, graphs and preview window. </summary>
        bool StartupVideo(UCOMIMoniker mon)
        {
            int hr;
            try
            {
                if (!CreateCaptureDevice(mon))
                    return false;

                if (!GetInterfaces())
                    return false;

                if (!SetupGraph())
                    return false;

                if (!SetupVideoWindow())
                    return false;

#if DEBUG
                DsROT.AddGraphToRot(graphBuilder, out rotCookie);		// graphBuilder capGraph
#endif

                hr = mediaCtrl.Run();
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                bool hasTuner = DsUtils.ShowTunerPinDialog(capGraph, capFilter, this.Handle);
                //toolBarBtnTune.Enabled = hasTuner;

                return true;
            }
            catch (Exception ee)
            {
                MessageBox.Show(this, "Could not start video stream\r\n" + ee.Message, "DirectShow.NET", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
        }

        /// <summary> resize preview video window to fill client area. </summary>
        void ResizeVideoWindow()
        {
            if (videoWin != null)
            {
                Rectangle rc = videoPanel.ClientRectangle;
                videoWin.SetWindowPosition(0, 0, rc.Right, rc.Bottom);
            }
        }
        /// <summary> build the capture graph for grabber. </summary>
        bool SetupGraph()
        {
            int hr;
            try
            {
                hr = capGraph.SetFiltergraph(graphBuilder);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                hr = graphBuilder.AddFilter(capFilter, "Ds.NET Video Capture Device");
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                DsUtils.ShowCapPinDialog(capGraph, capFilter, this.Handle);

                AMMediaType media = new AMMediaType();
                media.majorType = MediaType.Video;
                media.subType = MediaSubType.RGB24;
                media.formatType = FormatType.VideoInfo;		// ???
                hr = sampGrabber.SetMediaType(media);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                hr = graphBuilder.AddFilter(baseGrabFlt, "Ds.NET Grabber");
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                Guid cat = PinCategory.Preview;
                Guid med = MediaType.Video;
                hr = capGraph.RenderStream(ref cat, ref med, capFilter, null, null); // baseGrabFlt 
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                cat = PinCategory.Capture;
                med = MediaType.Video;
                hr = capGraph.RenderStream(ref cat, ref med, capFilter, null, baseGrabFlt); // baseGrabFlt 
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                media = new AMMediaType();
                hr = sampGrabber.GetConnectedMediaType(media);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);
                if ((media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero))
                    throw new NotSupportedException("Unknown Grabber Media Format");

                videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
                Marshal.FreeCoTaskMem(media.formatPtr); media.formatPtr = IntPtr.Zero;

                hr = sampGrabber.SetBufferSamples(false);
                if (hr == 0)
                    hr = sampGrabber.SetOneShot(false);
                if (hr == 0)
                    hr = sampGrabber.SetCallback(null, 0);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                return true;
            }
            catch (Exception ee)
            {
                MessageBox.Show(this, "Could not setup graph\r\n" + ee.Message, "DirectShow.NET", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
        }

        /// <summary> make the video preview window to show in videoPanel. </summary>
        bool SetupVideoWindow()
        {
            int hr;
            try
            {
                // Set the video window to be a child of the main window
                hr = videoWin.put_Owner(videoPanel.Handle);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                // Set video window style
                hr = videoWin.put_WindowStyle(WS_CHILD | WS_CLIPCHILDREN);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                // Use helper function to position video window in client rect of owner window
                ResizeVideoWindow();

                // Make the video window visible, now that it is properly positioned
                hr = videoWin.put_Visible(DsHlp.OATRUE);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                hr = mediaEvt.SetNotifyWindow(this.Handle, WM_GRAPHNOTIFY, IntPtr.Zero);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);
                return true;
            }
            catch (Exception ee)
            {
                MessageBox.Show(this, "Could not setup video window\r\n" + ee.Message, "DirectShow.NET", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
        }

        /// <summary> create the user selected capture device. </summary>
        bool CreateCaptureDevice(UCOMIMoniker mon)
        {
            object capObj = null;
            try
            {
                Guid gbf = typeof(IBaseFilter).GUID;
                mon.BindToObject(null, null, ref gbf, out capObj);
                capFilter = (IBaseFilter)capObj; capObj = null;
                return true;
            }
            catch (Exception ee)
            {
                MessageBox.Show(this, "Could not create capture device\r\n" + ee.Message, "DirectShow.NET", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            finally
            {
                if (capObj != null)
                    Marshal.ReleaseComObject(capObj); capObj = null;
            }

        }

        /// <summary> video window interface. </summary>
        private IVideoWindow videoWin;

        /// <summary> event interface. </summary>
        private IMediaEventEx mediaEvt;

        /// <summary> grabber filter interface. </summary>
        private IBaseFilter baseGrabFlt;

        /// <summary> create the used COM components and get the interfaces. </summary>
        bool GetInterfaces()
        {
            Type comType = null;
            object comObj = null;
            try
            {
                comType = Type.GetTypeFromCLSID(Clsid.FilterGraph);
                if (comType == null)
                    throw new NotImplementedException(@"DirectShow FilterGraph not installed/registered!");
                comObj = Activator.CreateInstance(comType);
                graphBuilder = (IGraphBuilder)comObj; comObj = null;

                Guid clsid = Clsid.CaptureGraphBuilder2;
                Guid riid = typeof(ICaptureGraphBuilder2).GUID;
                comObj = DsBugWO.CreateDsInstance(ref clsid, ref riid);
                capGraph = (ICaptureGraphBuilder2)comObj; comObj = null;

                comType = Type.GetTypeFromCLSID(Clsid.SampleGrabber);
                if (comType == null)
                    throw new NotImplementedException(@"DirectShow SampleGrabber not installed/registered!");
                comObj = Activator.CreateInstance(comType);
                sampGrabber = (ISampleGrabber)comObj; comObj = null;

                mediaCtrl = (IMediaControl)graphBuilder;
                videoWin = (IVideoWindow)graphBuilder;
                mediaEvt = (IMediaEventEx)graphBuilder;
                baseGrabFlt = (IBaseFilter)sampGrabber;
                return true;
            }
            catch (Exception ee)
            {
                MessageBox.Show(this, "Could not get interfaces\r\n" + ee.Message
                                , "DirectShow.NET", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            finally
            {
                if (comObj != null)
                    Marshal.ReleaseComObject(comObj); comObj = null;
            }
        }

        public void InitCapDev()
        {
            if (!DsUtils.IsCorrectDirectXVersion())
            {
                MessageBox.Show(this, "DirectX 8.1 NOT installed!", "DirectShow.NET"
                                , MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.Close(); return;
            }

            if (!DsDev.GetDevicesOfCat(FilterCategory.VideoInputDevice, out capDevices))
            {
                MessageBox.Show(this, "No video capture devices found!", "DirectShow.NET"
                                , MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.Close(); return;
            }

            DsDevice dev = null;
            if (capDevices.Count == 1)
                dev = capDevices[0] as DsDevice;
            else
            {
                DeviceSelector selector = new DeviceSelector(capDevices);
                selector.ShowDialog(this);
                dev = selector.SelectedDevice;
            }

            if (dev == null)
            {
                this.Close(); 
                return;
            }
            //StartupVideo(dev.Mon);
            if (!StartupVideo(dev.Mon))
                this.Close();
        }

        private void frmNhapKQGPB_Load(object sender, EventArgs e)
        {
            Config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

            Conn = Uit.it_MySql.OpenConnect(ConnStr);

            dtCode = BUS.GetData.getKeyCode(Conn);
            DataTable dtCBP = BUS.GetData.getLoaiDV(Conn);
            comLoaiDV.Properties.DataSource = dtCBP;
            comLoaiDV.EditValue = 11;

            arrControlsKQ.Clear();
            arrControlsKey.Clear();
            arrControlsBN.Clear();
            AddControlsBN();


           
        }


        /// <summary> capture event, triggered by buffer callback. </summary>
        void OnCaptureDonePic1()
        {
            try
            {
                int hr;
                if (sampGrabber == null)
                    return;
                hr = sampGrabber.SetCallback(null, 0);

                int w = videoInfoHeader.BmiHeader.Width;
                int h = videoInfoHeader.BmiHeader.Height;
                if (((w & 0x03) != 0) || (w < 32) || (w > 4096) || (h < 32) || (h > 4096))
                    return;
                int stride = w * 3;

                GCHandle handle = GCHandle.Alloc(savedArray, GCHandleType.Pinned);
                int scan0 = (int)handle.AddrOfPinnedObject();
                scan0 += (h - 1) * stride;
                Bitmap b = new Bitmap(w, h, -stride, PixelFormat.Format24bppRgb, (IntPtr)scan0);
                handle.Free();
                savedArray = null;

                Image old = picHinh1.Image;
                picHinh1.Image = b;
                if (old != null)
                    old.Dispose();
                /*======================save image====================*/

                //LuuImage("\\x100_", 1);
                pHinh1 = 0;
                /*======================end save image====================*/
            }
            catch (Exception ee)
            {
                MessageBox.Show(this, "Could not grab picture\r\n" + ee.Message, "DirectShow.NET", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        /// <summary> capture event, triggered by buffer callback. </summary>
        void OnCaptureDonePic2()
        {
            try
            {
                int hr;
                if (sampGrabber == null)
                    return;
                hr = sampGrabber.SetCallback(null, 0);

                int w = videoInfoHeader.BmiHeader.Width;
                int h = videoInfoHeader.BmiHeader.Height;
                if (((w & 0x03) != 0) || (w < 32) || (w > 4096) || (h < 32) || (h > 4096))
                    return;
                int stride = w * 3;

                GCHandle handle = GCHandle.Alloc(savedArray, GCHandleType.Pinned);
                int scan0 = (int)handle.AddrOfPinnedObject();
                scan0 += (h - 1) * stride;
                Bitmap b = new Bitmap(w, h, -stride, PixelFormat.Format24bppRgb, (IntPtr)scan0);
                handle.Free();
                savedArray = null;

                Image old = picHinh2.Image;
                picHinh2.Image = b;
                if (old != null)
                    old.Dispose();
                /*======================save image====================*/

                //LuuImage("\\x100_", 1);
                pHinh1 = 0;
                /*======================end save image====================*/
            }
            catch (Exception ee)
            {
                MessageBox.Show(this, "Could not grab picture\r\n" + ee.Message, "DirectShow.NET", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void txtCode_EditValueChanged(object sender, EventArgs e)
        {           
            DataRow[] arrRow = null;
            lsvCode.Items.Clear();
            string key = txtCodeKQ.Text;

            if (dtCode != null)
            {
                arrRow = dtCode.Select("KeyCode LIKE '%"+key+"%'");
                for (int i = 0; i < arrRow.Length; i++)
                {
                    ListViewItem item = new ListViewItem(arrRow[i]["KeyCode"].ToString());
                   
                    item.SubItems.Add(arrRow[i]["ViThe"].ToString());
                    item.SubItems.Add(arrRow[i]["KetLuan"].ToString());

                    lsvCode.Items.Add(item);
                }                
            }
             lsvCode.Visible = true;
            lsvCode.Location = new System.Drawing.Point(0, 268);
            lsvCode.Size = new System.Drawing.Size(525, 105);
            lsvCode.Columns[0].Width = 50;
            lsvCode.Columns[1].Width = 350;
            lsvCode.Columns[2].Width = 350;
        }
        
        private void btnSave_Click(object sender, EventArgs e)
        {                      
            arrControlsKQ.Clear();
            AddControlsKQ();
            arrControlsKey.Clear();
            AddControlsKeyKQ();
            bool rskq = InsertOneKey("gpb_ketqua", arrControlsKQ);
            if (rskq)
            {
                Uit.it_Msg.Information("Lưu kết quả thành công!");
                int NewNum = Uit.it_Parse.ToInteger(txtIDGPB.Text.Substring(2, 5));
                NewNum++;
                int LoaiSo = Uit.it_Parse.ToInteger(txtLoaiKQ.Text);
                BUS.UpdateData.updSoMax(HangSo.MAPK,LoaiSo, NewNum, Conn);
            }
        } 

        /// <summary> do cleanup and release DirectShow. </summary>
        void CloseInterfaces()
        {
            int hr;
            try
            {
#if DEBUG
                if (rotCookie != 0)
                    DsROT.RemoveGraphFromRot(ref rotCookie);
#endif

                if (mediaCtrl != null)
                {
                    hr = mediaCtrl.Stop();
                    mediaCtrl = null;
                }

                if (mediaEvt != null)
                {
                    hr = mediaEvt.SetNotifyWindow(IntPtr.Zero, WM_GRAPHNOTIFY, IntPtr.Zero);
                    mediaEvt = null;
                }

                if (videoWin != null)
                {
                    hr = videoWin.put_Visible(DsHlp.OAFALSE);
                    hr = videoWin.put_Owner(IntPtr.Zero);
                    videoWin = null;
                }

                baseGrabFlt = null;
                if (sampGrabber != null)
                    Marshal.ReleaseComObject(sampGrabber); sampGrabber = null;

                if (capGraph != null)
                    Marshal.ReleaseComObject(capGraph); capGraph = null;

                if (graphBuilder != null)
                    Marshal.ReleaseComObject(graphBuilder); graphBuilder = null;

                if (capFilter != null)
                    Marshal.ReleaseComObject(capFilter); capFilter = null;

                if (capDevices != null)
                {
                    foreach (DsDevice d in capDevices)
                        d.Dispose();
                    capDevices = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("CH-" + ex.Message.ToString(), "Thông Báo");
            }
        }
        private void frmNhapKQGPB_FormClosing(object sender, FormClosingEventArgs e)
        {
            firstActive = false;
            CloseInterfaces();
            Conn.Close();
        }

        void LoadDataToTextBox(ListViewItem item)
        {
            if (item == null)
                return;
            txtCodeKQ.Text = item.SubItems[0].Text;
            txtViThe.Text = item.SubItems[1].Text;
            txtKetLuan.Text = item.SubItems[2].Text;
            lsvCode.Visible = false;
        }
        private void lsvCode_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem item = lsvCode.SelectedItems[0];
            LoadDataToTextBox(item);
        }

        private void lsvCode_DragEnter(object sender, DragEventArgs e)
        {
           
        }

        private void lsvCode_KeyUp(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {                
                case Keys.Escape:
                    txtCodeKQ.Focus();
                    lsvCode.Visible = false;
                    break;
                case Keys.Enter:
                    ListViewItem item = lsvCode.SelectedItems[0];
                    LoadDataToTextBox(item);
                    break;
                default:
                    break;
            }

        }

        private void frmNhapKQGPB_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:                    
                    lsvCode.Focus();
                    lsvCode.Items[lsvCode.Items.Count-1].Selected = true;                    
                    break;
                default:
                    break;
            }
        }

        private void txtCode_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:                    
                    lsvCode.Focus();
                    lsvCode.Items[lsvCode.Items.Count-1].Selected = true;                    
                    break;

                case Keys.Down:
                    lsvCode.Focus();
                    lsvCode.Items[0].Selected = true;
                    break;
                case Keys.Escape:
                    txtCodeKQ.Focus();
                    if (lsvCode.Visible == true) 
                        lsvCode.Visible = false;
                    break;
                default:
                    break;
            }
        }
        
        private void comLoaiDV_EditValueChanged(object sender, EventArgs e)
        {
            DataRowView rowView = (DataRowView)comLoaiDV.GetSelectedDataRow();
            DataRow SelcRow = rowView.Row;
            txtLoaiKQ.Text = "";
            txtTenLoaiKQ.Text = "";
            txtPrefix.Text = "";
            //txtNam.Text = "";
            if (SelcRow != null)
            {
                txtLoaiKQ.Text = SelcRow[0].ToString();
                txtTenLoaiKQ.Text = SelcRow[3].ToString();
                txtPrefix.Text = SelcRow[2].ToString();
                //txtNam.Text = SelcRow[4].ToString().Substring(2,2);

                btnNewIDGPB_Click(sender, e);
            }
        }

        private void txtPrefix_EditValueChanged(object sender, EventArgs e)
        {
            //TextEdit txt = (TextEdit)sender;

            //string preFix = txtSoTieuBan.Text.Substring(0, 1);
            //preFix = txt.Text;
            //txtIDGPB.Text = txt.Text;
        }

        private void txtNam_EditValueChanged(object sender, EventArgs e)
        {
            TextEdit txt = (TextEdit)sender;

            txtIDGPB.Text += txt.Text;
        }

        private void txtSTTTieuBan_EditValueChanged(object sender, EventArgs e)
        {
            if (txtIDGPB.Text.Length != 7)
                return;
            arrControlsKQ.Clear();
            AddControlsKQ();

            long Num = Uit.it_Parse.ToLong( txtIDGPB.Text);
            int LoaiKQ = Uit.it_Parse.ToInteger(txtLoaiKQ.Text);
            DataTable dtKQ = BUS.GetData.getKQByID(Num, LoaiKQ, Conn);
            long MaBN = 0;
            if (dtKQ != null)
            {
                if (dtKQ.Rows.Count > 0)
                {
                    SetDataToForm(arrControlsKQ, dtKQ.Rows[0]);
                    MaBN = Uit.it_Parse.ToLong(dtKQ.Rows[0]["MaBN"]);
                    txtMaBN.Text = MaBN.ToString();
                    
                }
                else
                {
                    SetDataToForm(arrControlsKQ, dtKQ.NewRow());
                }
                lsvCode.Visible = false;
            }

            SetTTBN(MaBN);
            
        }

        private void txtNam_Validating(object sender, CancelEventArgs e)
        {
            
        }

        void AddControlsBN()
        {
            this.arrControlsBN.Add(txtMaBN);   
            this.arrControlsBN.Add(txtHo);
            this.arrControlsBN.Add(txtTen);
            this.arrControlsBN.Add(txtNamSinh);
            this.arrControlsBN.Add(radGioiTinh);
            this.arrControlsBN.Add(txtDiaChi);
            this.arrControlsBN.Add(txtDienThoai);

        }
        void AddControlsKey()
        {
            this.arrControlsKey.Add(txtMaBN);          

        }

        void AddControlsKeyKQ()
        {
            this.arrControlsKey.Add(txtIDGPB);
            this.arrControlsKey.Add(txtLoaiKQ);

        }
        void AddControlsKQ()
        {
            this.arrControlsKQ.Add(txtIDGPB);
            this.arrControlsKQ.Add(txtKetLuan);
            this.arrControlsKQ.Add(txtViThe);
            this.arrControlsKQ.Add(txtCodeKQ);
            this.arrControlsKQ.Add(txtDaiThe);
            this.arrControlsKQ.Add(datNgayTra);
            this.arrControlsKQ.Add(txtBSDocQK);
            this.arrControlsKQ.Add(txtDVGuiMau);
            this.arrControlsKQ.Add(txtBSChiDinh);
            this.arrControlsKQ.Add(datNgayLM);
            this.arrControlsKQ.Add(txtChanDoan);
            this.arrControlsKQ.Add(txtLoaiKQ);                     
        }

        public bool UpdateOneKey(string pcTable, ArrayList arrControl)
        {
            DbSql ole = new DbSql(ConnStr);

            bool flag = false;
            if (string.IsNullOrEmpty(pcTable.Trim()) || (arrControl.Count == 0))
            {
                MessageBox.Show("Tham số truyền cho phương thức 'UpdateOneKey' kh\x00f4ng hợp lệ. \n Developer vui l\x00f2ng kiểm tra lại !", "Th\x00f4ng b\x00e1o", MessageBoxButtons.OK);
                return false;
            }
            string str = "";
            string str2 = "";
            string pcKeyColumn = "";
            string pcKeyValue = "";
            if (arrControlsKey[0].GetType().ToString().Trim() == "System.Windows.Forms.MaskedTextBox")
            {
                MaskedTextBox box = (MaskedTextBox)arrControlsKey[0];
                pcKeyColumn = box.Name.Substring(3);
                pcKeyValue = box.Text.Trim();
            }
            else if (arrControlsKey[0].GetType().ToString().Trim() == "DevExpress.XtraEditors.GridLookUpEdit")
            {
                GridLookUpEdit box2 = (GridLookUpEdit)arrControlsKey[0];
                pcKeyColumn = box2.Name.Substring(3);
                pcKeyValue = box2.EditValue.ToString();
            }
            else if (arrControlsKey[0].GetType().ToString().Trim() == "DevExpress.XtraEditors.TextEdit")
            {
                TextEdit box3 = (TextEdit)arrControlsKey[0];
                pcKeyColumn = box3.Name.Substring(3);
                pcKeyValue = box3.Text.Trim();
            }       
            
            if (CheckExistItem(pcTable, pcKeyColumn, pcKeyValue))
            {
                str = "UPDATE " + pcTable;
                for (int i = 1; i < arrControl.Count; i++)
                {
                    TextEdit box7;
                    PictureBox box10;
                    string str6;
                    switch (arrControl[i].GetType().ToString().Trim())
                    {
                        case "System.Windows.Forms.NumericUpDown":
                            {
                                NumericUpDown down = (NumericUpDown)arrControl[i];
                                str2 = down.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str2, down.Value);
                                goto Label_0614;
                            }
                        case "DevExpress.XtraEditors.MemoEdit":
                            {
                                MemoEdit box4 = (MemoEdit)arrControl[i];
                                str2 = box4.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str2, box4.Text);
                                goto Label_0614;
                            }
                        case "DevExpress.XtraEditors.GroupControl":
                            {
                                GroupControl box5 = (GroupControl)arrControl[i];
                                str2 = box5.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str2, box5.Tag.ToString().Trim());
                                goto Label_0614;
                            }
                        case "DevExpress.XtraEditors.DateEdit":
                            {
                                DateEdit picker = (DateEdit)arrControl[i];
                                str2 = picker.Name.Substring(3);
                                string dateValue = Uit.it_Parse.ToDateTime( picker.EditValue).ToString("yyyy-MM-dd");
                                ole.DBCmd.Parameters.AddWithValue("@" + str2, dateValue);
                                goto Label_0614;
                            }
                        case "DevExpress.XtraEditors.CheckEdit":
                            {
                                CheckEdit box6 = (CheckEdit)arrControl[i];
                                str2 = box6.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str2, box6.CheckState);
                                goto Label_0614;
                            }
                        case "DevExpress.XtraEditors.RadioGroup":
                            {
                                RadioGroup button = (RadioGroup)arrControl[i];
                                str2 = button.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str2, button.EditValue);
                                goto Label_0614;
                            }
                        case "DevExpress.XtraEditors.TextEdit":
                            box7 = (TextEdit)arrControl[i];
                            str2 = box7.Name.Substring(3).ToLower();
                            if (!str2.Contains("fileh"))
                            {
                                break;
                            }
                            ole.DBCmd.Parameters.AddWithValue("@" + str2, box7.Text.Trim());
                            goto Label_0614;

                        case "DevExpress.XtraEditors.GridLookUpEdit":
                            {
                                GridLookUpEdit box8 = (GridLookUpEdit)arrControl[i];
                                str2 = box8.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str2, box8.EditValue);
                                goto Label_0614;
                            }
                        case "System.Windows.Forms.MaskedTextBox":
                            {
                                MaskedTextBox box9 = (MaskedTextBox)arrControl[i];
                                str2 = box9.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str2, box9.Text.Trim());
                                goto Label_0614;
                            }
                        case "DevExpress.XtraEditors.PictureEdit":
                            {
                                box10 = (PictureBox)arrControl[i];
                                str2 = box10.Name.Substring(3);
                                if (!str2.Contains("file"))
                                {
                                    goto Label_05E6;
                                }
                                if (box10.Image == null)
                                {
                                    goto Label_05BE;
                                }
                                MemoryStream stream = new MemoryStream();
                                box10.Image.Save(stream, ImageFormat.Jpeg);
                                byte[] buffer = stream.ToArray();
                                ole.DBCmd.Parameters.AddWithValue("@" + str2, buffer);
                                goto Label_0614;
                            }
                        default:
                            goto Label_0614;
                    }
                    ole.DBCmd.Parameters.AddWithValue("@" + str2, box7.Text.Trim());
                    goto Label_0614;
                Label_05BE:
                    str6 = "";
                    ole.DBCmd.Parameters.AddWithValue("@" + str2, str6);
                    goto Label_0614;
                Label_05E6:
                    ole.DBCmd.Parameters.AddWithValue("@" + str2, box10.Tag.ToString().Trim());
                Label_0614:
                    str = str + (str.Contains("SET") ? (", " + str2 + " = @" + str2) : (" SET " + str2 + " = @" + str2));
                }
                string str8 = str;
                str = str8 + " WHERE " + pcKeyColumn + " = @" + pcKeyColumn;
                ole.DBCmd.CommandText = str;
                ole.DBCmd.Parameters.AddWithValue("@" + pcKeyColumn, pcKeyValue.Trim());
                try
                {
                    flag = ole.DBCmd.ExecuteNonQuery() > 0;
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
                ole.CloseConnect();
                return flag;
            }

            return InsertOneKey(pcTable, arrControl);
        }

        public bool InsertOneKey(string pcTable, ArrayList arrControl)
        {
            bool flag = false;
            if (string.IsNullOrEmpty(pcTable.Trim()) || (arrControl.Count == 0) || (arrControlsKey.Count == 0))
            {
                MessageBox.Show("Tham số truyền cho phương thức 'InsertOneKey' kh\x00f4ng hợp lệ. \n Developer vui l\x00f2ng kiểm tra lại !", "Th\x00f4ng b\x00e1o", MessageBoxButtons.OK);
                return false;
            }
            string str = "";
            string str2 = "";
            string str3 = "";
            string str4 = "";
            string pcKeyColumn = "";
            string pcKeyValue = "";

            if (arrControlsKey[0].GetType().ToString().Trim() == "System.Windows.Forms.MaskedTextBox")
            {
                MaskedTextBox box = (MaskedTextBox)arrControlsKey[0];
                pcKeyColumn = box.Name.Substring(3);
                pcKeyValue = box.Text.Trim();
            }
            else if (arrControlsKey[0].GetType().ToString().Trim() == "DevExpress.XtraEditors.GridLookUpEdit")
            {
                GridLookUpEdit box2 = (GridLookUpEdit)arrControlsKey[0];
                pcKeyColumn = box2.Name.Substring(3);
                pcKeyValue = box2.EditValue.ToString();
            }
            else if (arrControlsKey[0].GetType().ToString().Trim() == "DevExpress.XtraEditors.TextEdit")
            {
                TextEdit box3 = (TextEdit)arrControlsKey[0];
                pcKeyColumn = box3.Name.Substring(3);
                pcKeyValue = box3.Text.Trim();
            }
            else if (arrControlsKey[0].GetType().ToString().Trim() == "DevExpress.XtraEditors.MemoEdit")
            {
                MemoEdit box4 = (MemoEdit)arrControlsKey[0];
                pcKeyColumn = box4.Name.Substring(3);
                pcKeyValue = box4.EditValue.ToString();
            }
            else if (arrControlsKey[0].GetType().ToString().Trim() == "DevExpress.XtraEditors.GroupControl")
            {
                GroupBox box5 = (GroupBox)arrControlsKey[0];
                pcKeyColumn = box5.Name.Substring(3);
                pcKeyValue = box5.Tag.ToString().Trim();
            }
            
            DbSql ole = new DbSql(ConnStr);
            
            if (!CheckExistItem(pcTable, pcKeyColumn, pcKeyValue))
            {
                str2 = "INSERT INTO " + pcTable;
                str3 = "VALUES";
                for (int i = 0; i < arrControl.Count; i++)
                {
                    PictureBox box12;
                    switch (arrControl[i].GetType().ToString().Trim())
                    {
                        case "DevExpress.XtraEditors.SpinEdit":
                            {
                                SpinEdit down = (SpinEdit)arrControl[i];
                                str4 = down.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str4, down.Value);
                                goto Label_06A7;
                            }
                        case "DevExpress.XtraEditors.MemoEdit":
                            {
                                MemoEdit box6 = (MemoEdit)arrControl[i];
                                str4 = box6.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str4, box6.Text);
                                goto Label_06A7;
                            }
                        case "DevExpress.XtraEditors.DateEdit":
                            {
                                DateEdit picker = (DateEdit)arrControl[i];
                                str4 = picker.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str4, picker.EditValue);
                                goto Label_06A7;
                            }
                        case "DevExpress.XtraEditors.CheckEdit":
                            {
                                CheckEdit box7 = (CheckEdit)arrControl[i];
                                str4 = box7.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str4, box7.EditValue);
                                goto Label_06A7;
                            }
                        case "DevExpress.XtraEditors.RadioGroup":
                            {
                                RadioGroup button = (RadioGroup)arrControl[i];
                                str4 = button.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str4, button.EditValue);
                                goto Label_06A7;
                            }
                        case "DevExpress.XtraEditors.TextEdit":
                            {
                                TextEdit box8 = (TextEdit)arrControl[i];
                                str4 = box8.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str4, box8.Text.Trim());
                                goto Label_06A7;
                            }
                        case "DevExpress.XtraEditors.GridLookUpEdit":
                            {
                                GridLookUpEdit box9 = (GridLookUpEdit)arrControl[i];
                                str4 = box9.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str4, box9.EditValue);
                                goto Label_06A7;
                            }
                        case "DevExpress.XtraEditors.GroupControl":
                            {
                                GroupBox box10 = (GroupBox)arrControl[i];
                                str4 = box10.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str4, box10.Tag.ToString().Trim());
                                goto Label_06A7;
                            }

                        case "DevExpress.XtraEditors.PictureEdit":
                            {
                                box12 = (PictureBox)arrControl[i];
                                str4 = box12.Name.Substring(3);
                                if (!str4.Contains("file"))
                                {
                                    goto Label_0678;
                                }
                                if (box12.Image == null)
                                {
                                    break;
                                }
                                MemoryStream stream = new MemoryStream();
                                box12.Image.Save(stream, ImageFormat.Jpeg);
                                byte[] buffer = stream.ToArray();
                                ole.DBCmd.Parameters.AddWithValue("@" + str4, buffer);
                                goto Label_06A7;
                            }
                        default:
                            goto Label_06A7;
                    }
                    string str8 = "";
                    ole.DBCmd.Parameters.AddWithValue("@" + str4, str8);
                    goto Label_06A7;
                Label_0678:
                    ole.DBCmd.Parameters.AddWithValue("@" + str4, box12.Tag.ToString().Trim());
                Label_06A7:
                    str2 = str2 + (str2.Contains("(") ? (", " + str4) : (" (" + str4));
                    str3 = str3 + (str3.Contains("(") ? (", @" + str4) : (" (@" + str4));
                }
                str = str2 + ") " + str3 + ")";
                try
                {
                    ole.DBCmd.CommandText = str;
                    flag = ole.DBCmd.ExecuteNonQuery() > 0;
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
                ole.CloseConnect();
                return flag;
            }
            return UpdateOneKey(pcTable, arrControl);
        }

        public bool CheckExistItem(string pcTable, string pcKeyColumn, string pcKeyValue)
        {
            string sql = "SELECT " + pcKeyColumn + " FROM " + pcTable + " WHERE " + pcKeyColumn.Trim() + " = '" + pcKeyValue.Trim() + "'";
            return !string.IsNullOrEmpty(GetStringValue(sql).Trim());
        }

        public string GetStringValue(string stringQuery)
        {
            string str = "";
            

            object obj2 = Uit.it_MySql.ExecuteScalar(stringQuery, Conn);
            if (obj2 != null)
            {
                str = obj2.ToString().Trim();
            }
          
            return str;

        }

        public string Tangchuoi(string pcNumber, int SoKyTu)
        {
            pcNumber = string.IsNullOrEmpty(pcNumber) ? pcNumber.PadLeft(SoKyTu, '0') : pcNumber.Trim();
            string s = "";
            string str2 = "";
            string str3 = "0123456789";
            try
            {
                for (int i = pcNumber.Length - 1; i >= 0; i--)
                {
                    if (!str3.Contains(pcNumber.Substring(i, 1)))
                    {
                        break;
                    }
                    s = pcNumber.Substring(i, 1) + s;
                }
                if (s == "")
                {
                    s = "0";
                    return (pcNumber + 1);
                }
                str2 = (ulong.Parse(s) + ((ulong)1L)).ToString();
                str2 = pcNumber.Substring(0, pcNumber.Length - s.Length) + str2.PadLeft(s.Length, '0');
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            return str2;
        }
        long GetMaxNum(int MaPK, int LoaiSo )
        {
           
            long Num = BUS.GetData.getMaxNum(MaPK, LoaiSo, Conn);

          
            return Num;
        }

        void SetDataToForm(ArrayList arrControls, DataRow dr)
        {
            TextEdit box7;
            PictureBox box10;
            string str6;
            string NameControl = "";
            for (int i = 1; i < arrControls.Count; i++)
            {
                
                switch (arrControls[i].GetType().ToString().Trim())
                {
                    case "System.Windows.Forms.NumericUpDown":
                        {
                            NumericUpDown down = (NumericUpDown)arrControls[i];
                            NameControl = down.Name.Substring(3);
                            down.Value = Uit.it_Parse.ToInteger(dr[NameControl]);
                            break;
                        }
                    case "DevExpress.XtraEditors.MemoEdit":
                        {
                            MemoEdit box4 = (MemoEdit)arrControls[i];
                            NameControl = box4.Name.Substring(3);
                            box4.Text = dr[NameControl].ToString();
                            break;
                        }
                    case "DevExpress.XtraEditors.GroupControl":
                        {
                            GroupControl box5 = (GroupControl)arrControls[i];
                            NameControl = box5.Name.Substring(3);
                            box5.Text = dr[NameControl].ToString();
                            break;
                        }
                    case "DevExpress.XtraEditors.DateEdit":
                        {
                            DateEdit picker = (DateEdit)arrControls[i];
                            NameControl = picker.Name.Substring(3);
                            picker.EditValue = Uit.it_Parse.ToDateTime(dr[NameControl]);
                            break;
                        }
                    case "DevExpress.XtraEditors.CheckEdit":
                        {
                            CheckEdit box6 = (CheckEdit)arrControls[i];
                            NameControl = box6.Name.Substring(3);
                            box6.Text = dr[NameControl].ToString();
                            break;
                        }
                    case "DevExpress.XtraEditors.RadioGroup":
                        {
                            RadioGroup button = (RadioGroup)arrControls[i];
                            NameControl = button.Name.Substring(3);
                            button.EditValue = Uit.it_Parse.ToInteger(dr[NameControl]);
                            break;
                        }
                    case "DevExpress.XtraEditors.TextEdit":
                        {
                            box7 = (TextEdit)arrControls[i];
                            NameControl = box7.Name.Substring(3);
                            box7.Text = dr[NameControl].ToString();
                            break;
                        }
                    case "DevExpress.XtraEditors.GridLookUpEdit":
                        {
                            GridLookUpEdit box8 = (GridLookUpEdit)arrControls[i];
                            NameControl = box8.Name.Substring(3);
                            box8.EditValue = Uit.it_Parse.ToInteger(dr[NameControl]);
                            break;
                        }
                    case "System.Windows.Forms.MaskedTextBox":
                        {
                            MaskedTextBox box9 = (MaskedTextBox)arrControls[i];
                            NameControl = box9.Name.Substring(3);
                            box9.Text = Uit.it_Parse.ToString(dr[NameControl]);
                            break;
                        }
                    case "DevExpress.XtraEditors.PictureEdit":
                        {
                            //box10 = (PictureBox)arrControl[i];
                            //NameControl = box9.Name.Substring(3);

                            //MemoryStream stream = new MemoryStream();
                            //box10.Image.Save(stream, ImageFormat.Jpeg);
                            //byte[] buffer = stream.ToArray();
                            //ole.DBCmd.Parameters.AddWithValue("@" + str2, buffer);
                            //goto Label_0614;
                            break;
                        }
                    default:
                        break;
                }

            }
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            long Num = GetMaxNum(HangSo.MAPK, HangSo.SOHS);
            string temp = HangSo.MAPK.ToString() + Num.ToString();
            Num = Uit.it_Parse.ToLong(temp);
            txtMaBN.Text = Num.ToString();
            arrControlsBN.Clear();
            AddControlsBN();
            DataTable dtBN = BUS.GetData.getBNByMa(Num, Conn);
            if (dtBN != null)
            {
                if (dtBN.Rows.Count > 0)
                {
                    SetDataToForm(arrControlsBN, dtBN.Rows[0]);
                }
                else
                {
                    SetDataToForm(arrControlsBN, dtBN.NewRow());
                }
            }
           
        }

        private void btnNext_Click(object sender, EventArgs e)
        {           
            long NumMax = BUS.GetData.get_CurMaxNumBN(Conn);
            long Num = 0;
            long CurNum = Uit.it_Parse.ToLong(txtMaBN.Text);
            if (CurNum >= NumMax)
            {
                MessageBox.Show("Bạn đã chọn bệnh nhân có ID lớn nhất!");
                return;
            }
            if (CurNum == 0)
            {
                Num = BUS.GetData.get_CurMaxNumBN(Conn);
                txtMaBN.Text = Num.ToString();
                SetTTBN(Num);
                return;
            }
            if (txtMaBN.Text.Trim() == "")
            {
                Num = GetMaxNum(HangSo.MAPK, HangSo.SOHS);
                string temp = HangSo.MAPK.ToString() + Num.ToString();
                Num = Uit.it_Parse.ToLong(temp);
                txtMaBN.Text = Num.ToString();
            }
            else
            {
                Num = Uit.it_Parse.ToLong(txtMaBN.Text) + 1;
                txtMaBN.Text = Num.ToString();
                
            }
            SetTTBN(Num);
            
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            
            long NumMin = BUS.GetData.get_CurMinNumBN(Conn);
            long Num = 0;
            long CurNum = Uit.it_Parse.ToLong(txtMaBN.Text);
            
            if (CurNum == 0)
            {
                Num = BUS.GetData.get_CurMinNumBN(Conn);
                txtMaBN.Text = Num.ToString();
                SetTTBN(Num);
                return;
            }
            if (CurNum <= NumMin)
            {
                MessageBox.Show("Bạn đã chọn bệnh nhân có ID nhỏ nhất!");
                return;
            } 
            if (txtMaBN.Text.Trim() == "")
            {
                Num = GetMaxNum(HangSo.MAPK, HangSo.SOHS);
                string temp = HangSo.MAPK.ToString() + Num.ToString();
                Num = Uit.it_Parse.ToLong(temp);
                txtMaBN.Text = Num.ToString();
            }
            else
            {
                Num = Uit.it_Parse.ToLong(txtMaBN.Text) - 1;
                txtMaBN.Text = Num.ToString();
                SetTTBN(Num);
            }            
            
        }

        long setNewIDGPB()
        {
            int LoaiKQ = Uit.it_Parse.ToInteger(txtLoaiKQ.Text);
            long Num = GetMaxNum(HangSo.MAPK, LoaiKQ);
            Num++;
            txtIDGPB.Text = Num.ToString();    
            return Num;
        }

        private void btnNewIDGPB_Click(object sender, EventArgs e)
        {
            long Num = setNewIDGPB();
            SetTTBN(0);

        }

        private void btnPrevIDGPB_Click(object sender, EventArgs e)
        {
           
            long NumMin = BUS.GetData.get_MinIDGPB(Conn);
            long Num = 0;
            long CurNum = Uit.it_Parse.ToLong(txtIDGPB.Text);
            int LoaiKQ = Uit.it_Parse.ToInteger(txtLoaiKQ.Text);
            if (CurNum <= NumMin)
            {
                MessageBox.Show("Bạn đã chọn bệnh nhân có ID nhỏ nhất!");
                return;
            }
            if (txtIDGPB.Text.Trim() == "")
            {
                Num = GetMaxNum(HangSo.MAPK, LoaiKQ);
                txtMaBN.Text = "";

            }
            else
            {
                Num = Uit.it_Parse.ToLong(txtIDGPB.Text) - 1;
                txtMaBN.Text = "";
                txtIDGPB.Text = Num.ToString();
                
            }
        }

        private void btnNextIDGPB_Click(object sender, EventArgs e)
        {
            int LoaiKQ = Uit.it_Parse.ToInteger(txtLoaiKQ.Text);
            long NumMax = BUS.GetData.get_MaxIDGPB(LoaiKQ, Conn);
            long Num = 0;
            long CurNum = Uit.it_Parse.ToLong(txtIDGPB.Text);
           
            if (CurNum > NumMax)
            {
                MessageBox.Show("Bạn đã chọn bệnh nhân có ID lớn nhất!");
                return;
            }
            if (txtIDGPB.Text.Trim() == "")
            {
                Num = GetMaxNum(HangSo.MAPK, LoaiKQ);
                txtMaBN.Text = Num.ToString();
            }
            else
            {
                Num = Uit.it_Parse.ToLong(txtIDGPB.Text) + 1;
                txtIDGPB.Text = Num.ToString();
            }
            
        }

        void SetTTBN(long Num)
        {
            arrControlsBN.Clear();
            AddControlsBN();
            DataTable dtBN = BUS.GetData.getBNByMa(Num, Conn);
            if (dtBN != null)
            {
                if (dtBN.Rows.Count > 0)
                {
                    SetDataToForm(arrControlsBN, dtBN.Rows[0]);
                    DataTable dtKQ = BUS.GetData.getKQByMaBN(Num, Conn);//neu co ket qua thi hien thi
                    if (dtKQ.Rows.Count > 0)
                    {
                        txtIDGPB.Text = dtKQ.Rows[0]["IDGPB"].ToString();
                    }
                    else
                    {
                        setNewIDGPB();
                    }
                }
                else
                {
                    SetDataToForm(arrControlsBN, dtBN.NewRow());
                }
            }
        }
        private void txtMaBN_EditValueChanged(object sender, EventArgs e)
        {
            if (txtMaBN.Text.Length != 10)
                return;

            
            long Num = Uit.it_Parse.ToLong(txtMaBN.Text);
            
        }

        private void btnComOn_Click(object sender, EventArgs e)
        {
            /***++++++++++++++++++++++++++++++++++++++++++++++*/
            if (firstActive)
                return;
            firstActive = true;
            InitCapDev();
            /***++++++++++++++++++++++++++++++++++++++++++++++*/
        }

        private void btnChonHinh1_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "C:\\";
            openFileDialog1.Filter = "image files (*.jpg)|*.jpg|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {                            
                            Bitmap bm = new Bitmap(myStream, false);
                            picHinh1.Image = bm;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "C:\\";
            openFileDialog1.Filter = "image files (*.jpg)|*.jpg|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            Bitmap bm = new Bitmap(myStream, false);
                            picHinh2.Image = bm;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void picHinh2_Click(object sender, EventArgs e)
        {
            try
            {
                /*=====================================*/
                int hr;
                
                if (savedArray == null)
                {
                    int size = videoInfoHeader.BmiHeader.ImageSize;
                    if ((size < 1000) || (size > 16000000))
                        return;
                    savedArray = new byte[size + 64000];
                }

                Image old = picHinh2.Image;
                picHinh2.Image = null;
                if (old != null)
                    old.Dispose();
                captured = false;
                hr = sampGrabber.SetCallback(this, 1);
                pHinh2 = 1;
                /*=====================================*/
                SaveImage(2);
            }
            catch (Exception ex)
            {
                MessageBox.Show("CH" + ex.Message.ToString(), "Thông Báo");
                return;
            }
        }

        private void picHinh1_Click(object sender, EventArgs e)
        {
            btnChupHinh1_Click(sender, e);
        }

        private void btnChupHinh1_Click(object sender, EventArgs e)
        {
            try
            {
                /*=====================================*/
                int hr;
                if (savedArray == null)
                {
                    int size = videoInfoHeader.BmiHeader.ImageSize;
                    if ((size < 1000) || (size > 16000000))
                        return;
                    savedArray = new byte[size + 64000];
                }

                Image old = picHinh1.Image;
                picHinh1.Image = null;
                if (old != null)
                    old.Dispose();
                captured = false;
                hr = sampGrabber.SetCallback(this, 1);
                pHinh1 = 1;
                /*=====================================*/
                SaveImage(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("CH" + ex.Message.ToString(), "Thông Báo");
                return;
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void SaveImage(int PicNum)
        {
            string pathStrbF = ".\\PictureTemp\\";
            //save image to temp directory
            if (PicNum == 1)
            {
                if (picHinh1.Image != null)
                {
                    picHinh1.Image.Save(pathStrbF, ImageFormat.Jpeg);
                    //ChinhDoSang(pathStrbF, pathStr);
                    //LoadHinh(picHinh1, pathStr); ;
                }
            }
            if (PicNum == 2)
            {
                if (picHinh2.Image != null)
                {
                    picHinh2.Image.Save(pathStrbF, ImageFormat.Jpeg);
                    //ChinhDoSang(pathStrbF, pathStr);
                    //pic400.Image = null;
                   // LoadHinh(pic400, pathStr);
                }
            }

            //if (File.Exists(pathStr))
            //{                
            //    /// if not exists directory then create directory
            //    if (!Directory.Exists(ThuMucLuu))
            //    {
            //        Directory.CreateDirectory(ThuMucLuu);
            //    }

            //    ///delete if exists
            //    if (File.Exists(pathHinh1Ser))
            //    {
            //        File.Delete(pathHinh1Ser);
            //    }

            //    //copy hinh to server
            //    File.Copy(pathStr, pathHinh1Ser);

            //    ///delete file local
            //    File.Delete(pathStr);
            //}
        }

        private void btnXemKQ_Click(object sender, EventArgs e)
        {
            DataTable dtKQ = new DataTable();
            long IDGPB = Uit.it_Parse.ToLong(txtIDGPB.Text);
            int LoaiKQ = Uit.it_Parse.ToInteger(txtLoaiKQ.Text);
            dtKQ = BUS.GetData.getKQByID(IDGPB, LoaiKQ, Conn);

            if (dtKQ.Rows.Count > 0)
            {
                rptPhieuKQ rptKQ = new rptPhieuKQ();
                rptKQ.txtSoTieuBan.Text = txtPrefix.Text + dtKQ.Rows[0]["IDGPB"].ToString();
                rptKQ.txthoten.Text = txtHo.Text + " " + txtTen.Text;
                rptKQ.txttuoi.Text = txtNamSinh.Text;
                rptKQ.txtGioiTinh.Text = "";
                rptKQ.txtDiaChi.Text = txtDiaChi.Text;
                rptKQ.txtDienThoai.Text = txtDienThoai.Text;
                rptKQ.txtchatBP.Text = dtKQ.Rows[0]["ChatBP"].ToString();
                rptKQ.txtbsthuchien.Text = ComBSDocKQ.Text;
                rptKQ.txtBSCD.Text = ComBSCD.Text;

                rptKQ.txtDaiThe.Text = dtKQ.Rows[0]["DaiThe"].ToString();
                rptKQ.txtViThe.Text = dtKQ.Rows[0]["ViThe"].ToString();
                rptKQ.txtKetLuan.Text = dtKQ.Rows[0]["KetLuan"].ToString().ToUpper();


                rptKQ.piclan1.Image = picHinh1.Image;
                rptKQ.piclan2.Image = picHinh2.Image;
                rptKQ.txtngaygioin.Text = DateTime.Now.ToString("dd/MM/yyyy, HH:mm:ss");
                rptKQ.txtbsthuchien.Text = ComBSDocKQ.Text;
                DateTime cdate = DateTime.Today;
                cdate = DateTime.Parse(dtKQ.Rows[0]["CDate"].ToString());
                rptKQ.txtngaythang.Text = "Ngày " + cdate.Day.ToString("00") +
                                  " Tháng " + cdate.Month.ToString("00") +
                                  " Năm " + cdate.Year.ToString("0000");
                rptKQ.ShowPreview();
            }
        }

        private void btnInKQ_Click(object sender, EventArgs e)
        {

        }

        private void comboBoxEdit2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}