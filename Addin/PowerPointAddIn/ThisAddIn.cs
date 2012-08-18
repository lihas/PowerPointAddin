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

namespace PowerPointAddIn
{
    public partial class ThisAddIn
    {
        SlideShowWindow Wnglob;
        Socket clientSockglob;
        int flagIsFirst = 0;
        private const int listenPort = 11000;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            
            Application.SlideShowBegin += OnSlideShowBegin;

         

        }
        
        void OnSlideShowBegin(SlideShowWindow Wn) {
            flagIsFirst = flagIsFirst + 1;
            Wnglob = Wn;
            Thread t = new Thread(socFun);
            if (flagIsFirst == 1) //start the server only if this is the first time the slide-show is started -otherwise PowerPoint crashes
            {

                t.Start();
            }
            else { t.Abort();  }
            

            
        }

        void socFun(){
            


            bool done = false;
        UdpClient listener = new UdpClient(listenPort);
        IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);
        string received_data;
        byte[] receive_byte_array;

        try
        {
            while (!done)
            {
                receive_byte_array = listener.Receive(ref groupEP);
                received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                int v1 = int.Parse(received_data);
                switch (v1)
                {
                    case 1: Wnglob.View.GotoClick(Wnglob.View.GetClickIndex() + 1); break;
                    case 2: Wnglob.View.Previous(); break;
                    case 3: Wnglob.View.Next(); break;
                }
            }
        }
        catch (Exception e)
        {
            MessageBox.Show("exception In UDP socket!");
        }
        listener.Close();
            

        }
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
          clientSockglob.Close();
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
    }
}
