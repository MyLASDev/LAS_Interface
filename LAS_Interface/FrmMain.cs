using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using Library;
using MySql.Data.MySqlClient;

namespace LAS_Interface
{
    public partial class FrmMain : Form
    {
        string strconn = "server=192.168.3.50;database=las_database;uid=root;pwd=";
        IPEndPoint remoteEP;
        Socket socket;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            timer1.Start();
            RaiseEvents("Application Start");
            updatedatagridview();
        }
        private void SetStatusbar(string pMessage)        
        {
            toolStripStatus.Text = pMessage;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string text  = "[Date Time : " + DateTime.Now + "]";
            SetStatusbar(text);
        }
        private bool ConnectAcl()
        {
            try
            {
                string uri = "ktd-devth.ddns.net";
                string ipAddress = "192.168.1.114";
                var addresses = Dns.GetHostAddresses(uri);
                remoteEP = new IPEndPoint(addresses[0], 7734);
                //remoteEP = new IPEndPoint(IPAddress.Parse(ipAddress), 7734);
                // Create a TCP/IP socket.
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.ReceiveTimeout = 2000;
                socket.SendTimeout = 1000;
                socket.Connect(remoteEP);
                RaiseEvents("Connection Success");
                return true;
            }
            catch (Exception ex)
            {
                RaiseEvents("Connect Lost = "+ex.Message);
                return false;
            }


        }
        void RaiseEvents(string pMsg)
        {
            string vMsg = DateTime.Now + ">[LAS InterFace]> " + pMsg;
            DisplayMessage("",vMsg);
        }
        #region ListboxItem

        public void DisplayMessage(string pFileName, string pMsg)
        {
            if (this.lstMain.InvokeRequired)
            {
                // This is a worker thread so delegate the task.
                if (lstMain.Items.Count > 1000)
                {
                    //lstMain.Items.Clear();
                    this.Invoke((Action)(() => lstMain.Items.Clear()));
                }

                this.lstMain.Invoke(new DisplayMessageEventHandler(this.DisplayMessage), pFileName, pMsg);
            }
            else
            {
                // This is the UI thread so perform the task.
                if (pMsg != null)
                {
                    if (lstMain.Items.Count > 1000)
                    {
                        //lstMain.Items.Clear();
                        this.Invoke((Action)(() => lstMain.Items.Clear()));
                    }

                    this.lstMain.Items.Insert(0, pMsg);

                    //PLog.WriteLog(pFileName, iMsg);
                }
            }
        }

        private delegate void DisplayMessageEventHandler(string pFileName, string pMsg);

        public object AddListBox
        {
            set
            {
                AddListBoxItem(value);
            }
        }

        private delegate void AddListBoxItemEventHandler(object pItem);

        private void AddListBoxItem(object pItem)
        {
            if (this.lstMain.InvokeRequired)
            {
                // This is a worker thread so delegate the task.
                if (lstMain.Items.Count > 1000)
                {
                    //lstMain.Items.Clear();
                    this.Invoke((Action)(() => lstMain.Items.Clear()));
                }

                this.lstMain.Invoke(new AddListBoxItemEventHandler(this.AddListBoxItem), pItem);
            }
            else
            {
                // This is the UI thread so perform the task.
                if (pItem != null)
                {
                    if (lstMain.Items.Count > 1000)
                    {
                        //lstMain.Items.Clear();
                        this.Invoke((Action)(() => lstMain.Items.Clear()));
                    }

                    this.lstMain.Items.Insert(0, pItem);
                }
            }
        }

        #endregion

        private void btnConnectAcl_Click(object sender, EventArgs e)
        {
            bool value = ClientLib.ConnectAcl();
            if (value)
            {
                RaiseEvents("Connection Success");
            }
            else
            {
                RaiseEvents("Connection lost");
            }
                       
        }
        public void updatedatagridview()
        {
            MySqlConnection conn = new MySqlConnection(strconn);
            conn.Open();
            string viewdata = "select * from Las_data";
            MySqlDataAdapter adapter = new MySqlDataAdapter(viewdata, conn);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dgvDisplay.DataSource = dt;
            conn.Close();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            RaiseEvents("Start Loading");
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            RaiseEvents("End Transaction");
        }

        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
