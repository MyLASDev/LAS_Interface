using Library;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace LAS_Interface
{
    public partial class FrmMain : Form
    {
        string strconn = "server=r98du2bxwqkq3shg.cbetxkdyhwsb.us-east-1.rds.amazonaws.com;database=ahda1gtbqhb7pncg;uid=hktvkvjk6993txuk;pwd=ma46ffmhhxgl0zj6";
        IPEndPoint remoteEP;
        Socket socket;
        public AcculoadLib.AcculoadMember[] AclMember;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            timer1.Start();
            RaiseEvents("Application Start");
            updatedgvLH();
            //updatedgvLL();
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
            bool vCheck = ClientLib.getIsConnectAcl();
            if (value)
            {
                RaiseEvents("Connection Success");
            }
            else
            {
                RaiseEvents("Connection lost");
            }
                       
        }
        public void updatedgvLH()
        {
            MySqlConnection conn = new MySqlConnection(strconn);
            conn.Open();
            string viewdata = "select * from loadingheaders";
            MySqlDataAdapter adapter = new MySqlDataAdapter(viewdata, conn);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
            conn.Close();
        }

        public void updatedgvLL()
        {
            MySqlConnection conn = new MySqlConnection(strconn);
            conn.Open();
            string viewdata = "select * from LoadingLine";
            MySqlDataAdapter adapter = new MySqlDataAdapter(viewdata, conn);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dataGridView2.DataSource = dt;
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

        private void btnEQ_Click(object sender, EventArgs e)
        {
            AclMember = new AcculoadLib.AcculoadMember[1];
            AclMember[0].AclValueNew = new AcculoadLib._AcculoadValue();

            string vCmd = AcculoadLib.RequestEnquireStatus(14);
            string vData = ClientLib.SendData(vCmd);
            AcculoadLib.DecodedEnquireStatus(ref AclMember[0].AclValueNew.EQ, vData);

            RaiseEvents("Transaction Done = " + AclMember[0].AclValueNew.EQ.A2b2_TransactionDone.ToString());
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FrmLoading frmDO = new FrmLoading(this);
            frmDO.ShowDialog();
            RaiseEvents("Add Delivery Order");
            updatedgvLH();
            //updatedgvLL();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            FrmLoading frmDO = new FrmLoading(this);
            frmDO.frmActon = 2;
            frmDO.ShowDialog();
            RaiseEvents("Edit Delivery Order");
            updatedgvLH();
            //updatedgvLL();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            string loadno = dataGridView1.SelectedCells[0].Value.ToString();
            MySqlConnection conn = new MySqlConnection(strconn);
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.Parameters.AddWithValue("@LoadNo", loadno);
            cmd.CommandText = "delete from loadingheaders where LoadNo = @LoadNo";
            if (cmd.ExecuteNonQuery() > 0)
                MessageBox.Show("successfully");
            else
                MessageBox.Show("error");
            conn.Close();
            updatedgvLH();
        }

    }
}
