using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PowerPointAddIn
{
    public partial class ThisAddIn
    {
        #region global declarations
        int flagIsFirst = 0;
        bool flagShowRunning = false;//stores the state of the slide show, ie if its running or not
        private const int listenPort = 11000;
        UdpClient listener_glob;
        SlideShowWindow Wn_glob; //global var is used since i wasn't able to pass Wn as reference to a thread
        /// <summary>
        /// stores info about the device properties such as resolution, etc.
        /// </summary>
        class destination {
            public static int xRes=480, yRes=800; //by default configured for m dell xcd-35
        }


        private static Mutex mut = new Mutex();
        #endregion

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            
            Application.SlideShowBegin += OnSlideShowBegin;
            
            Application.SlideShowEnd += new EApplication_SlideShowEndEventHandler(OnSlideShowEnd);

         

        }

       
        
        void OnSlideShowBegin(SlideShowWindow Wn) {
            flagIsFirst = flagIsFirst + 1;
            flagShowRunning = true;
            Wn_glob = Wn;
            Thread t = new Thread(socFun);
            if (flagIsFirst == 1) //start the server only if this is the first time the slide-show is started -otherwise PowerPoint crashes
            {

                t.Start();
            }
        }

        void OnSlideShowEnd(Presentation Pres)
        {

            flagShowRunning = false;
        }

        void socFun()
        {


            bool done = false;
            UdpClient listener = new UdpClient(listenPort);
            listener_glob = listener;
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);
            string received_data;
            byte[] receive_byte_array;

            while (!done)
            {
                try
                {
                    receive_byte_array = listener.Receive(ref groupEP);
                    received_data = Encoding.ASCII.GetString(receive_byte_array, 0, 1);
                    int v1 = int.Parse(received_data);
                    if (flagShowRunning)
                    { // perform action only if slide show is running
                                                
                        //send bitmap of powepoint to android
                        //sendToAndroid(1, Wn_glob.HWND); //type=1 represents drawing                        


                        switch (v1)
                        {
                            case 1: Wn_glob.View.GotoClick(Wn_glob.View.GetClickIndex() + 1); break;
                            case 2: Wn_glob.View.Previous(); break;
                            case 3: Wn_glob.View.Next(); break;
                        }

                    }
                    else
                    {
                        //send message to android that the slide show is not running
                    }
                }
                catch (Exception e)
                {
                    // MessageBox.Show("Exception in thread ,error: " + e.Message);
                }
            }


            listener.Close();
        }

        /// <summary>
        /// use 'ONLY' this function to send data to Android as threading might cause erros while sending data
        /// type signifies the type data to send
        /// type 1: send bitmap
        /// type 2:info
        /// type 3:error
        /// </summary>
        /// <param name="type"></param>
        private void sendToAndroid(int type, object obj)
        {
            mut.WaitOne();
            switch(type){
                case 1:
                    {//send bitmap                        
                        Image img=getWindowImage(obj);     
#region non-working code for saving image with codec
                       // ImageCodecInfo[] codecs=ImageCodecInfo.GetImageEncoders();
                       // ImageCodecInfo ici=null;
                       // foreach(ImageCodecInfo codec in codecs)
                       // {                     
                       //  if(codec.MimeType=="image/jpeg")
                       //                     ici=codec;
                        
                       // }
                       // EncoderParameters eps = new EncoderParameters(1);
                        
                       // eps.Param[0] = new EncoderParameter( System.Drawing.Imaging.Encoder.Quality, 100 );
                        //// img.Save("d:\\abcd.jpg", ici, eps);
#endregion

                        img.Save("d:\\abcd.jpg", ImageFormat.Jpeg);                        
                        byte[] file_byte_array=File.ReadAllBytes("d:\\abcd.jpg");
                        //the first packet will contain type and size of the following packet
                        //listener_glob.Send(Encoding.ASCII.GetBytes(file_byte_array.GetLength(0).ToString()), file_byte_array.GetLength(0).ToString().Length);
                        listener_glob.Send(file_byte_array, file_byte_array.GetLength(0));
                        

                    } break;
            
            
            }
            mut.ReleaseMutex();
            
        }

        private Image getWindowImage(object obj) //get image from handle stored in obj
        {
           
            int hwnd = (int)obj;
            
            IntPtr hdcSrc = User32.GetWindowDC(hwnd);
            
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect((IntPtr)hwnd, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            //IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width/2, height/2);  //since for displayng on mobile device complte res. isn't necsary
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            //GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);
            GDI32.StretchBlt(hdcDest, 0, 0, width/2, height/2, hdcSrc, 0, 0, width, height, GDI32.SRCCOPY); //since for displayng on mobile device complte res. isn't necsary            
            GDI32.SelectObject(hdcDest, hOld); //not sure why is this required again
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC((IntPtr)hwnd, hdcSrc);
            Image img = Image.FromHbitmap(hBitmap);
            GDI32.DeleteObject(hBitmap);
            return img;
        }
            

       


       
        
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            listener_glob.Close();
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion

        /// <summary>
        /// from http://www.developerfusion.com/code/4630/capture-a-screen-shot/
        /// </summary>
        private class GDI32
        {

            public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                int nWidth, int nHeight, IntPtr hObjectSource,
                int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, 
                int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hdcSrc, int nXOriginSrc, 
                int nYOriginSrc, int nWidthSrc, int nHeightSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }

        /// <summary>
        /// Helper class containing User32 API functions
        /// from http://www.developerfusion.com/code/4630/capture-a-screen-shot/
        /// </summary>
        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            //[DllImport("user32.dll")] //original code
            //public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(int hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
        }
    }
    
}
