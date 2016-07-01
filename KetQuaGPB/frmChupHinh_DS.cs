using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using BUS;
using System.Runtime.InteropServices;
using DShowNET;
using DShowNET.Device;
using System.Collections;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace KetQuaGPB
{
    public partial class frmChupHinh_DS : DevExpress.XtraEditors.XtraForm, ISampleGrabberCB
    {
        public frmChupHinh_DS()
        {
            InitializeComponent();
        }
        public frmChupHinh_DS(string _sotieuban, int _isCl)
        {
            InitializeComponent();
            sotieuban = _sotieuban;
           
            isCl = _isCl;
        }


        private string pathTemp = "";
        public string sThuMucLuu = "";
        public string sThuGoc = "";
        private Bitmap bmHinh1 = null;
        private Bitmap bmHinh2 = null;
        public string sotieuban = "";
        public string DVuCode = "";
        public string sophieu = "";
        public int isCl = 0;
        private int flag = 0;
        private int[] isImg = new int[4]{0,0,0,0} ;
        public delegate void DelegeChupH(object sender, Object data);
        public event DelegeChupH thoatEven;

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
                this.Close(); return;
            }

            if (!StartupVideo(dev.Mon))
                this.Close();
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
            if (p100 == 1)
            {
                this.BeginInvoke(new CaptureDone(this.OnCaptureDone));               
            }
             else if (p400 == 1)
            {
                this.BeginInvoke(new CaptureDone(this.OnCaptureDonePic400));
              
            }
            return 0;
        }

        private void frmChupHinh_Load(object sender, EventArgs e)
        {
            try
            {

                //load lai hinh neu da chup roi                
                txtsohs.Text = "";
                txtsobn.Text = "";
                txtSoTieuBan.Text = sotieuban;

                txtdiachi.Text = "";
                txtGioi.Text = "";
                txtHoTen.Text = "";
                txttendv.Text = "";
                txtTuoi.Text = "";

                pathTemp = Application.StartupPath + "\\PictureTemp";
                sThuGoc = sThuMucLuu + "\\";
                if (!Directory.Exists(pathTemp))
                {
                    Directory.CreateDirectory(pathTemp);
                }
                if (isCl == 1)
                {
                    txtSoTieuBan.Enabled = false;
                    btnNext.Visible = false;
                    btnPrevious.Visible = false;
                    btngo.Visible = false;
                    btngo_Click(sender, e);
                }
                else
                {
                    txtSoTieuBan.Enabled = true;
                }              

                /***++++++++++++++++++++++++++++++++++++++++++++++*/
                if (firstActive)
                    return;
                firstActive = true;
                InitCapDev();
                /***++++++++++++++++++++++++++++++++++++++++++++++*/
            }
            catch (Exception ex)
            {
                MessageBox.Show("CH" + ex.Message.ToString(), "Thông Báo");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="urlTemp">url tren may local</param>
        /// <param name="urlSer">url tren may server</param>
        public void ReloadImage(string urlTemp, string urlSer)
        {
            ////load hinh x100
            //string urlLocal1 = "";
            //string urlServ1 = "";
            //string[] words = txtsobn.Text.Split(new string[] { "*" }, StringSplitOptions.None);
            //string sobn = "";

            //if (words.Length > 1)
            //    sobn = words[0] + "_" + words[1];
            //else
            //    sobn = txtsobn.Text;
            //DataTable dtPL = GetThongTinSearchBUS.SearchPhanLoaiDV(DVuCode);
            //int NhomPL = 0;
            //if (dtPL.Rows.Count > 0)
            //    NhomPL = int.Parse(dtPL.Rows[0]["IDLoai"].ToString());
            ////if (DVuCode == "GE56" && isCl == 1)
            //if (NhomPL == 1 && isCl == 1)
            //{
            //    urlLocal1 = urlTemp + "x100_" + txtSoTieuBan.Text + "_" + isCl.ToString() + ".jpg";
            //    urlServ1 = urlSer + "x100_" + txtSoTieuBan.Text + "_" + isCl.ToString() + ".jpg";
            //}
            //else
            //{
            //    urlLocal1 = urlTemp + "x100_" + txtSoTieuBan.Text + ".jpg";
            //    urlServ1 = urlSer + "x100_" + txtSoTieuBan.Text + ".jpg";
            //}

            //if (File.Exists(urlLocal1))
            //{
            //    LoadHinh(pic100, urlLocal1);
            //}
            //else if (File.Exists(urlServ1))
            //{
            //    LoadHinh(pic100, urlServ1);
            //}
            //else
            //{
            //    ClearHinh(1);
            //    if (pic100.Name == "pic100")
            //    {
            //        bmHinh1 = null;
            //        pic100.Image = null;
            //    }
            //}

            ////load hinh x400
            //string urlLocal2 = "";
            //string urlServ2 = "";

            ////if (DVuCode == "GE56" && isCl == 1)
            //if (NhomPL == 1 && isCl == 1)
            //{
            //    urlLocal2 = urlTemp + "\\x400_" + txtSoTieuBan.Text + "_" + isCl.ToString() + ".jpg";
            //    urlServ2 = urlSer + "\\x400_" + txtSoTieuBan.Text + "_" + isCl.ToString() + ".jpg";
            //}
            //else
            //{
            //    urlLocal2 = urlTemp + "x400_" + txtSoTieuBan.Text + ".jpg";
            //    urlServ2 = urlSer + "\\x400_" + txtSoTieuBan.Text + ".jpg";
            //}
            //if (File.Exists(urlLocal2))//neu co hinh o local thi load o local 
            //{
            //    LoadHinh(pic400, urlLocal2);
            //}
            //else if (File.Exists(urlServ2))//neu co hinh o server thi load o local
            //{
            //    LoadHinh(pic400, urlServ2);
            //}
            //else
            //{
            //    ClearHinh(2);
            //    if (pic400.Name == "pic400")
            //    {
            //        bmHinh2 = null;
            //        pic400.Image = null;
            //    }
            //}
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            try
            {               
                ClearHinh(0);
                if (isCl == 1)
                {
                    if (thoatEven != null)
                    {
                        thoatEven(this, isImg);
                    }
                }
                //this.Close();                
            }
            catch (Exception ex)
            {
                MessageBox.Show("CH-" + ex.Message.ToString(), "Thông Báo");
            }
        }

        /// <summary>
        /// clear hinh tren picbox
        /// </summary>
        /// <param name="NumHinh">1: hinh 1; 2: hinh; 0: all</param>
        private void ClearHinh(int NumHinh)
        {
            if (NumHinh == 1)
            {
                if (bmHinh1 != null)
                {
                    bmHinh1.Dispose();
                    bmHinh1 = null;
                    pic100.Image = null;
                    pic100.Refresh();
                }
            }
            else if (NumHinh == 2)
            {
                if (bmHinh2 != null)
                {
                    bmHinh2.Dispose();
                    bmHinh2 = null;
                    pic400.Image = null;
                    pic400.Refresh();
                }
            }
            else if (NumHinh == 0)
            {
                if (bmHinh1 != null)
                {
                    bmHinh1.Dispose();
                    bmHinh1 = null;
                    pic100.Image = null;
                    pic100.Refresh();
                }
                if (bmHinh2 != null)
                {
                    bmHinh2.Dispose();
                    bmHinh2 = null;
                    pic400.Image = null;
                    pic400.Refresh();
                }
            }
        }

        private void LuuImage(string PreName, int imgTimes)
        {
           
                //string[] words = txtsobn.Text.Split(new string[] { "*" }, StringSplitOptions.None);
                //string sobn = "";
                //if (words.Length > 1)
                //    sobn = words[0] + "_" + words[1];
                //else
                //    sobn = txtsobn.Text;

                //string ThuMucLuu = sThuMucLuu;
               
                //string pathHinh1Ser = "";
                //string pathStr = "";
                //string pathStrbF = "";//Hinh truoc chinh sua
             
                //DataTable dtPL = GetThongTinSearchBUS.SearchPhanLoaiDV(DVuCode);
                //int NhomPL = 0;
                //if (dtPL.Rows.Count > 0)
                //    NhomPL = int.Parse(dtPL.Rows[0]["IDLoai"].ToString());

                //if (NhomPL == 1 && isCl == 1)
                //{
                //    pathStr = pathTemp + PreName + sotieuban + "_" + isCl.ToString() + ".jpg";
                //    pathStrbF = pathTemp + PreName + sotieuban + "bf_" + isCl.ToString() + ".jpg";
                //}
                //else
                //{
                //    pathStr = pathTemp + PreName + sotieuban + ".jpg";
                //    pathStrbF = pathTemp + PreName + sotieuban + "bf_" + ".jpg";
                //}

                //pathHinh1Ser = ThuMucLuu + PreName + sotieuban + ".jpg";
               
                ////save image to temp directory
                //if (imgTimes == 1)
                //{
                //    if (pic100.Image != null)
                //    {
                //        pic100.Image.Save(pathStrbF, ImageFormat.Jpeg);
                //        ChinhDoSang(pathStrbF, pathStr);
                //        LoadHinh(pic100, pathStr); ;
                //    }
                //}
                //if (imgTimes == 2)
                //{
                //    if (pic400.Image != null)
                //    {
                //        pic400.Image.Save(pathStrbF, ImageFormat.Jpeg);
                //        ChinhDoSang(pathStrbF, pathStr);
                //        pic400.Image = null;
                //        LoadHinh(pic400, pathStr);
                //    }
                //}

                //if (File.Exists(pathStr))
                //{
                //    flag = 1;
                //    isImg[0] = 1;
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

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                ClearHinh(1);
                LuuImage(  "\\x100_",1);
                ClearHinh(2);
                LuuImage("\\x400_",2);               
            }
            catch (Exception ex)
            {
                MessageBox.Show("CH-" + ex.Message.ToString(), "Thông Báo");
                return;
            }
            finally
            {             
                flag = 1;
                if (thoatEven != null)
                {
                    thoatEven(this, isImg);
                }
            }
        }

        private void xoaHinh(string path, PictureEdit edit)
        {           
            if (File.Exists(path))
            {
                if (edit.Name == "pic100")
                {
                   
                    if (bmHinh1 != null)
                    {                       
                        bmHinh1.Dispose();
                        pic100.Image = null;
                    }
                    File.Delete(path);
                }
                if (edit.Name == "pic400")
                {                   
                    if (bmHinh2 != null)
                    {
                        bmHinh2.Dispose();
                        pic400.Image = null;
                    }
                    File.Delete(path);
                }   
                
            }           
        }

        private void LoadHinh(PictureBox edit, string pathStr)
        {
            if (File.Exists(pathStr))
            {
                if (edit.Name == "pic100")
                {
                    Stream stream = File.Open(pathStr, FileMode.Open,
                    FileAccess.Read, FileShare.Delete);
                    bmHinh1 = (Bitmap)Bitmap.FromStream(stream);
                    stream.Close();
                    edit.Image = bmHinh1;
                }

                if (edit.Name == "pic400")
                {
                    Stream stream = File.Open(pathStr, FileMode.Open,
                    FileAccess.Read, FileShare.Delete);
                    bmHinh2 = (Bitmap)Bitmap.FromStream(stream);
                    stream.Close();
                    edit.Image = bmHinh2;
                }
            }
            
        }

        private void ChinhDoSang(string pathStrbf,string pathStr)
        {
            //Bitmap m_Bitmap ;
            //float value = int.Parse(txtdosang.Text)*10;
            ////doc file
            //Stream stream = File.Open(pathStrbf, FileMode.Open,
            //    FileAccess.ReadWrite, FileShare.Delete);
            //m_Bitmap = (Bitmap)Bitmap.FromStream(stream);
            //stream.Close(); 

            //BitmapFilter.Brightness(m_Bitmap, value);//tang do sang 
            //BitmapFilter.Contrast(m_Bitmap, (sbyte)value);//tang do net

            /////crop image
            //Rectangle cropRect = new Rectangle(8, 3, m_Bitmap.Width - 20, m_Bitmap.Height - 4);
            //        Bitmap target = new Bitmap(m_Bitmap);            
            //target = (Bitmap)cropImage(m_Bitmap, cropRect);            
            
            //target.Save(pathStr, ImageFormat.Jpeg);//save file moi chup thanh file moi 
            //File.Delete(pathStrbf);//xoa file goc di

        }

        /// <summary>
        /// crop image to new size
        /// </summary>
        /// <param name="img">bitmap to crop</param>
        /// <param name="cropArea"> rectangle crop</param>
        /// <returns></returns>
        private static Image cropImage(Bitmap img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }

        private void btnChupHinh_Click(object sender, EventArgs e)
        {            
            try
            { 
                /*=====================================*/
                int hr;
                p100 = 1;
                if (savedArray == null)
                {
                    int size = videoInfoHeader.BmiHeader.ImageSize;
                    if ((size < 1000) || (size > 16000000))
                        return;
                    savedArray = new byte[size + 64000];
                }

                Image old = pic100.Image;
                pic100.Image = null;
                if (old != null)
                    old.Dispose();               
                captured = false;
                hr = sampGrabber.SetCallback(this, 1);
                /*=====================================*/               
            }
            catch(Exception ex)
            {
                MessageBox.Show("CH" + ex.Message.ToString(), "Thông Báo");                
                return;
            }
        }
        /// <summary> buffer for bitmap data. </summary>
        private byte[] savedArray;
        private bool captured = true;
        private int bufferedSize;
        private int p100 = 0, p400 = 0;
       

        private void btnChupHinhX400_Click(object sender, EventArgs e)
        {                        
            try
            {
                /*=====================================*/
                int hr;
                p400 = 1;
                if (savedArray == null)
                {
                    int size = videoInfoHeader.BmiHeader.ImageSize;
                    if ((size < 1000) || (size > 16000000))
                        return;
                    savedArray = new byte[size + 64000];
                }

                Image old = pic400.Image;
                pic400.Image = null;
                if (old != null)
                    old.Dispose();
               
                captured = false;
                hr = sampGrabber.SetCallback(this, 1);
                /*=====================================*/
            }
            catch (Exception ex)
            {
                MessageBox.Show("CH" + ex.Message.ToString(), "Thông Báo");
                return;
            }
        }

        /// <summary> capture event, triggered by buffer callback. </summary>
        void OnCaptureDone()
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

                Image old = pic100.Image;
                pic100.Image = b;
                if (old != null)
                    old.Dispose();   
                /*======================save image====================*/
                
                LuuImage( "\\x100_",1);
                p100 = 0;
                /*======================end save image====================*/
            }
            catch (Exception ee)
            {
                MessageBox.Show(this, "Could not grab picture\r\n" + ee.Message, "DirectShow.NET", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
        
        void OnCaptureDonePic400()
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
                Image old = pic400.Image;
                pic400.Image = b;
                if (old != null)
                    old.Dispose();   
                /*======================save image====================*/               
                LuuImage( "\\x400_",2);
                p400 = 0;
                /*======================end save image====================*/
            }
            catch (Exception ee)
            {
                MessageBox.Show(this, "Could not grab picture\r\n" + ee.Message, "DirectShow.NET", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
        private void frmChupHinh_FormClosing(object sender, FormClosingEventArgs e)
        {
            ClearHinh(0);
            this.Hide();
            CloseInterfaces();
            this.Close();
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

        private void frmChupHinh_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {      
                if (e.KeyCode == Keys.F9)
                {
                    //p100 = 1;
                    btnChupHinh_Click(sender, e);
                }
                else if (e.KeyCode == Keys.F10)
                {
                    //p400 = 1;
                    btnChupHinhX400_Click(sender, e);
                }
                else if (e.KeyCode == Keys.F11)
                {
                    btnLuu_Click(sender, e);
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    btnThoat_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("CH-" + ex.Message.ToString(), "Thông Báo");
            }
        }
        /// <summary>
        /// Lay chuoi tu phai sang theo leng truyen vao
        /// </summary>
        /// <param name="value">chuoi can cat</param>
        /// <param name="length">chieu dai can cat</param>
        /// <returns>chuoi can cat</returns>
        public string Right(string value, int length)
        {
            if (value == "")
                return "";
            else
                return value.Substring(value.Length - length);
        }

        /// <summary>
        /// hàm lấy chuỗi từ trái sang
        /// </summary>
        /// <param name="value">giá trị của chuỗi cần cắt</param>
        /// <param name="length">chiều dài cảu chuỗi con</param>
        /// <returns>cuỗi cần lấy</returns>
        public string Left(string value, int length)
        {
            if (value == "")
                return "";
            else
                return value.Substring(0, length);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            //int numCur = 0, numNext = 0;
            //string pre = "";
            //if (txtSoTieuBan.Text == "")
            //    return;
            //numCur = int.Parse(Right(txtSoTieuBan.Text, hensu.LengTeBao));
            //if (numCur >= 1)
            //    btnPrevious.Enabled = true;
            //numNext = numCur + 1;
            //if (isCl == 1)//dang chup hinh cho cat lanh
            //{
            //    pre = Left(txtSoTieuBan.Text, 5);

            //    txtSoTieuBan.Text = pre + numNext.ToString("0000");
            //}
            //else// dang chup hinh cho gpb
            //{
            //    pre = Left(txtSoTieuBan.Text, 4);
            //    txtSoTieuBan.Text = pre + numNext.ToString("00000");
            //}
            //btngo_Click(sender, e);
        }
        private void CreateDir(DateTime NgNhan)
        {
            string goc = sThuGoc;
            DateTime dt;
            if (!NgNhan.Equals(""))
                dt = NgNhan;
            else
                dt = DateTime.Today;
            string namthang = dt.Year.ToString() + Uit.it_String.Right("0" + dt.Month.ToString(), 2);
            if (!Directory.Exists(goc + "\\" + namthang.Substring(0, 4)))
            {
                Directory.CreateDirectory(goc + "\\" + namthang.Substring(0, 4));
            }
            if (!Directory.Exists(goc + "\\" + namthang.Substring(0, 4) + "\\" + namthang.Substring(2, 4)))
            {
                Directory.CreateDirectory(goc + "\\" + namthang.Substring(0, 4) + "\\" + namthang.Substring(2, 4));
            }
            this.sThuMucLuu = goc + "\\" + namthang.Substring(0, 4) + "\\" + namthang.Substring(2, 4)+"\\";
        }

        private void btngo_Click(object sender, EventArgs e)
        {
            string SoTieuBan = txtSoTieuBan.Text;
            if (SoTieuBan.Equals(""))
                return;
            
            //DataTable dt = GetThongTinSearchBUS.SearchDaXacNhanBySoTB(SoTieuBan);
            //if (dt != null)
            //{
            //    if (dt.Rows.Count > 0)
            //    {
            //        btnChupHinh.Enabled = true;
            //        btnChupHinhX400.Enabled = true;
            //        btnLuu.Enabled = true;

            //        DateTime NgayNhan = DateTime.Parse(dt.Rows[0]["ngaynhan"].ToString());
            //        CreateDir(NgayNhan);

            //        txtHoTen.Text = dt.Rows[0]["HoTen"].ToString();
            //        txtTuoi.Text = dt.Rows[0]["namsinh"].ToString()
            //                     + "(" + dt.Rows[0]["tuoi"].ToString() + " Tuổi)";
            //        txtGioi.Text = dt.Rows[0]["gioi"].ToString();
            //        txtdiachi.Text = dt.Rows[0]["DiaChi"].ToString();
            //        txttendv.Text = dt.Rows[0]["tendvu"].ToString();
            //        txtsohs.Text = dt.Rows[0]["SoHS"].ToString();
            //        txtsobn.Text = dt.Rows[0]["SoBN"].ToString();
            //        sotieuban = txtSoTieuBan.Text;
                                     
            //        ReloadImage(pathTemp, sThuMucLuu);
            //        //tao folder tam tren local de luu hinh
            //        string[] words = txtsobn.Text.Split(new string[] { "*" }, StringSplitOptions.None); 
            //         string stb = "";
            //         if (words.Length > 1)                        
            //             stb = words[0] + "_" + words[1];                        
            //         else
            //             stb = txtsobn.Text;
            //         pathTemp = Application.StartupPath + "\\PictureTemp" ;
                   
            //        if (!Directory.Exists(pathTemp))
            //        {
            //            Directory.CreateDirectory(pathTemp);
            //        }
            //    }
            //    else
            //    {
            //        txtHoTen.Text = "";
            //        txtTuoi.Text = "";
            //        txtGioi.Text = "";
            //        txtdiachi.Text = "";
            //        txttendv.Text = "";
            //        txtsohs.Text = "";
            //        txtsobn.Text = "";
            //    }
            //}
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            //int numCur = 0, numPrev = 0;
            //string pre = "";
            //if (txtSoTieuBan.Text == "")
            //    return;
            //numCur = int.Parse(Right(txtSoTieuBan.Text, hensu.LengTeBao));
            //if (numCur <= 2)
            //    btnPrevious.Enabled = false;
            //numPrev = numCur - 1;
            //if (isCl == 1)//dang chup hinh cho cat lanh
            //{
            //    pre = Left(txtSoTieuBan.Text, 5);
            //    txtSoTieuBan.Text = pre + numPrev.ToString("0000");
            //}
            //else// dang chup hinh cho gpb
            //{
            //    pre = Left(txtSoTieuBan.Text, 4);
            //    txtSoTieuBan.Text = pre + numPrev.ToString("00000");
            //}
            //btngo_Click(sender, e);
        }

        private void txtSoTieuBan_Validated(object sender, EventArgs e)
        {
            btngo_Click(sender, e);
        }

        private void txtSoTieuBan_Validating(object sender, CancelEventArgs e)
        {
            txtSoTieuBan.Text = txtSoTieuBan.Text.ToUpper();
        }

        private void pic100_Click(object sender, EventArgs e)
        {
            if (btnChupHinh.Enabled == true)
            {
                //p100 = 1;
                btnChupHinh_Click(sender, e);
            }
        }

        private void pic400_Click(object sender, EventArgs e)
        {
            if (btnChupHinhX400.Enabled == true)
            {
                //p400 = 1;
                btnChupHinhX400_Click(sender, e);
            }
        }

        private void frmChupHinh_DS_Activated(object sender, EventArgs e)
        {
            if (firstActive)
                return;
            firstActive = true;
            /***++++++++++++++++++++++++++++++++++++++++++++++*/
            InitCapDev();
            /***++++++++++++++++++++++++++++++++++++++++++++++*/
        }
    }
}