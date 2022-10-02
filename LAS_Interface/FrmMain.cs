using Library;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Drawing;

namespace LAS_Interface
{
    public partial class FrmMain : Form
    {
        string strconn = "server=r98du2bxwqkq3shg.cbetxkdyhwsb.us-east-1.rds.amazonaws.com;database=ahda1gtbqhb7pncg;uid=hktvkvjk6993txuk;pwd=ma46ffmhhxgl0zj6";
        IPEndPoint remoteEP;
        Socket socket;
        public AcculoadLib.AcculoadMember[] AclMember;
        public string load_no;
        public SqlConnection cn;
        public SqlDataAdapter da;
        //private Rectangle button1OriginalRectangle;
        //private Rectangle button2OriginalRectangle;
        //private Rectangle button3OriginalRectangle;
        //private Rectangle button4OriginalRectangle;
        //private Rectangle button5OriginalRectangle;
        //private Rectangle splitOriginalRectangle;
        //private Rectangle originalFormSize;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            //originalFormSize = new Rectangle (this.Location , this.Size);
            //button1OriginalRectangle = new Rectangle(btnEnd.Location, btnEnd.Size );
            //button2OriginalRectangle = new Rectangle(btnStart.Location, btnStart.Size );
            //button3OriginalRectangle = new Rectangle(btnConnectAcl.Location, btnConnectAcl.Size );
            //button4OriginalRectangle = new Rectangle(btnStop.Location, btnStop.Size );
            //button5OriginalRectangle = new Rectangle(btnResetAlarm.Location, btnResetAlarm.Size );
            //splitOriginalRectangle = new Rectangle(splitContainer2.Location.X, splitContainer2.Location.Y, splitContainer2.Width, splitContainer2.Height);
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
            string text = "[Date Time : " + DateTime.Now + "]";
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
                RaiseEvents("Connect Lost = " + ex.Message);
                return false;
            }


        }
        void RaiseEvents(string pMsg)
        {
            string vMsg = DateTime.Now + ">[LAS InterFace]> " + pMsg;
            DisplayMessage("", vMsg);
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
            DialogResult result = MessageBox.Show("คุณต้องการ connect Accuload ใช่หรือไม่ ?", "Connect Accuload", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
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
           
        }
        public void updatedgvLH()
        {
            
            string sql = "select * from loadingheaders";
            DataTable dt = new DataTable();
            dt = DatabaseLib.Excute_DataAdapter(sql);
            dataGridView1.DataSource = dt;

        }

        //public void updatedgvLL()
        //{
        //    string sql = "select * from LoadingLine";
        //    DataTable dt = new DataTable();
        //    dt = DatabaseLib.Excute_DataAdapter(sql);
        //    dataGridView2.DataSource = dt;
        //}

        private void btnStart_Click(object sender, EventArgs e)
        {
            string status = dataGridView2.SelectedCells[4].Value.ToString();
            int s = System.Convert.ToInt32(status);
            if ( s == 1)
            {
                DialogResult result = MessageBox.Show("คุณต้องการ start loading ใช่หรือไม่ ?", "Start Loading", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    AclMember = new AcculoadLib.AcculoadMember[1];
                    AclMember[0].AclValueNew = new AcculoadLib._AcculoadValue();

                    string vCmd = AcculoadLib.RequestEnquireStatus(14);
                    string vData = ClientLib.SendData(vCmd);

                    RaiseEvents("Start Loading");
                }
            }
            
            
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("คุณต้องการ End Transaction ใช่หรือไม่ ?", "End Transaction", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                RaiseEvents("End Transaction");
            }
            
        }

        //private void btnEQ_Click(object sender, EventArgs e)
        //{
        //    AclMember = new AcculoadLib.AcculoadMember[1];
        //    AclMember[0].AclValueNew = new AcculoadLib._AcculoadValue();

        //    string vCmd = AcculoadLib.RequestEnquireStatus(14);
        //    string vData = ClientLib.SendData(vCmd);
        //    AcculoadLib.DecodedEnquireStatus(ref AclMember[0].AclValueNew.EQ, vData);

        //    RaiseEvents("Transaction Done = " + AclMember[0].AclValueNew.EQ.A2b2_TransactionDone.ToString());
        //}

        private void btnAdd_Click(object sender, EventArgs e)
        {
           FrmLoading frmDO = new FrmLoading(this);
           frmDO.ShowDialog();
           RaiseEvents("Add Delivery Order");
           updatedgvLH();
            //bool vCheck = DatabaseLib.ExecuteSQL(StrQuery);
            //if (vCheck == true)
            //{

            //}
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            FrmLoading frmDO = new FrmLoading(this);
            frmDO.frmActon = 2;
            frmDO.ShowDialog();
            RaiseEvents("Edit Delivery Order");
            updatedgvLH();
            
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            //string loadno = dataGridView1.SelectedCells[0].Value.ToString();
            //MySqlConnection conn = new MySqlConnection(strconn);
            //conn.Open();
            //MySqlCommand cmd = conn.CreateCommand();
            //cmd.Parameters.AddWithValue("@LoadNo", loadno);
            //cmd.CommandText = "delete from loadingheaders where LoadNo = @LoadNo";
            try
            {
                delheader();
                delline();
                updatedgvLH();
                MessageBox.Show("successfully");
                RaiseEvents("Delete Delivery Order");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error");
                RaiseEvents("Delete not successfully");
            }
            //if (cmd.ExecuteNonQuery() > 0)
            //    MessageBox.Show("successfully");
            //else
            //    MessageBox.Show("error");
            //conn.Close();

        }

        public void delline()
        {
            string loadno = dataGridView1.SelectedCells[0].Value.ToString();
            MySqlConnection conn = new MySqlConnection(strconn);
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.Parameters.AddWithValue("@LoadNo", loadno);
            cmd.CommandText = "delete from loadinglines where LoadNo = @LoadNo";
            cmd.ExecuteNonQuery();
        }

        public void delheader()
        {
            string loadno = dataGridView1.SelectedCells[0].Value.ToString();
            MySqlConnection conn = new MySqlConnection(strconn);
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.Parameters.AddWithValue("@LoadNo", loadno);
            cmd.CommandText = "delete from loadingheaders where LoadNo = @LoadNo";
            cmd.ExecuteNonQuery();
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            load_no = dataGridView1.SelectedCells[0].Value.ToString();
            string sql2 = @"SELECT  LoadNo, Compartment, ProductName, Preset, Status
                              FROM loadinglines where LoadNo = " + load_no;
            DataTable dt2 = new DataTable();
            dt2 = DatabaseLib.Excute_DataAdapter(sql2);
            dataGridView2.DataSource = dt2;

            /*for (int i = 0; i < dt2.Rows.Count; i++)
              {
                  Console.WriteLine(i);
                  dataGridView2.Rows[i].Cells[2].Value = dt2.Rows[i]["ProductName"].ToString();
                  dataGridView2.Rows[i].Cells[3].Value = dt2.Rows[i]["Preset"].ToString();          
              }*/
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("คุณต้องการ stop loading ใช่หรือไม่ ?", "Stop Loading", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                RaiseEvents("Stop Loading");
            }
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            FrmStatus frmStatus = new FrmStatus();
            frmStatus.ShowDialog();
        }



        //private void resizeControl(Rectangle r, Control c)
        //{
        //    float xratio = (float)(this.ClientRectangle.Width) / (float)(originalFormSize.Width);
        //    float yratio = (float)(this.ClientRectangle.Height) / (float)(originalFormSize.Height);
        //    int newx = (int)(r.Location.X * xratio);
        //    int newy = (int)(r.Location.Y * yratio);
        //    int newWidth = (int)(r.Width * xratio);
        //    int newHeight = (int)(r.Height * yratio);
        //    c.Location = new Point(newx, newy);
        //    c.Size = new Size(newWidth, newHeight);
        //}
        //private void FrmMain_Resize(object sender, EventArgs e)
        //{
        //    ResizeChildren();
        //}

        //private void ResizeChildren()
        //{
        //    resizeControl(button1OriginalRectangle, btnEnd);
        //    resizeControl(button5OriginalRectangle, btnResetAlarm);
        //    resizeControl(button2OriginalRectangle, btnStart);
        //    resizeControl(button4OriginalRectangle, btnStop);
        //    resizeControl(button3OriginalRectangle, btnConnectAcl);
        //    //resizeControl(splitOriginalRectangle, splitContainer2);
        //}

    }
}
