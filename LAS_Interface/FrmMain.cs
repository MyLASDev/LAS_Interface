using Library;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;
using System.Data.SqlClient;
using System.Drawing;
using System.Xml.Linq;
using Google.Protobuf.WellKnownTypes;
using System.CodeDom;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.Remoting.Messaging;
using Ubiety.Dns.Core;
using MySqlX.XDevAPI.Common;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using LAS_Interface;

namespace LAS_Interface
{
    public partial class FrmMain : Form
    {
        string strconn = "server=r98du2bxwqkq3shg.cbetxkdyhwsb.us-east-1.rds.amazonaws.com;database=ahda1gtbqhb7pncg;uid=hktvkvjk6993txuk;pwd=ma46ffmhhxgl0zj6";
        string dirLog = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        //public AcculoadProcess AlcProcess = new AcculoadProcess();

        public SqlConnection cn;
        public SqlDataAdapter da;
        public AcculoadLib.AcculoadMember[] AclMember;
        public AcculoadProcess AcculoadProcess;
       // public AcculoadProcess.AcculoadMember[] AclProMember;
        public CLogfiles LogFile = new CLogfiles();

        IPEndPoint remoteEP;
        Socket socket;

        public string batch_no;
        public string SelectedCells;
        public string load_no;
        public int pPreset;

        private bool stpProcess = false;
        private int currentBatch;
        public static string[] delete = new string[5];
        public static string[] backup = new string[2];

        public FrmMain()
        {
            InitializeComponent();

        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            AcculoadProcess.frmmain = this;
            timer1.Start();
            RaiseEvents("---------------Application Start--------------");
            updatedgvLH();
            updatedgvLL();
            try
            {
                using (StreamReader readtext = new StreamReader(dirLog + "currentbatch.text", true))
                {
                    string readText = readtext.ReadLine();
                    currentBatch = Int32.Parse(readText);
                    backup[0] = readText;
                }
                File.Delete(dirLog + "currentbatch.text");
                string sql = @"SELECT TotalizerGV, CurrentPreset, LoadedGV, CurrentFlowrate FROM loadinglines where BatchNo = " + currentBatch;
               
                using (MySqlConnection connection = new MySqlConnection(strconn))
                {
                    MySqlCommand command = new MySqlCommand(sql, connection);
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    string Read = string.Empty;
                    while (reader.Read())
                    {
                        txtTotalizer.Text = reader.GetString("TotalizerGV");
                        txtPreset.Text = reader.GetString("CurrentPreset");
                        txtGV.Text = reader.GetString("LoadedGV");
                        txtFlowRate.Text = reader.GetString("CurrentFlowrate");
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                RaiseEventsErr(ex.ToString());
            }

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

        public void RaiseEvents(string pMsg)
        {
            string vMsg = DateTime.Now + ">[LAS InterFace]> " + pMsg;
            DisplayMessage("", vMsg);
            LogFile.WriteLog("AclAddr14", vMsg);
            //LogFile.WriteErrLog("AclAddr14", vMsg);
        }

        public void RaiseEventsErr(string pMsg)
        {
            string vMsg = DateTime.Now + ">[LAS InterFace]> " + pMsg;
            DisplayMessage("", vMsg);
            //LogFile.WriteLog("AclAddr14", vMsg);
            LogFile.WriteErrLog("AclAddr14", vMsg);
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
                //bool vCheck = ClientLib.getIsConnectAcl();
                if (value)
                {
                    RaiseEvents("Accuload connect successful");
                    try
                    {
                        if (DatabaseLib.IsServerConnected())
                        {
                            stpProcess = true;
                            AcculoadProcess = new AcculoadProcess(this);
                            AcculoadProcess.stpBatch = 1;
                            AcculoadProcess.StartThread();
                            Thread.Sleep(1000);
                            RaiseEvents("Database connect successful");
                            Thread.Sleep(1000);
                            RaiseEvents("Start read from accuload");
                            this.Invoke(new Action(() =>
                            {
                                btnConnectAcl.Enabled = false;
                            }));
                        
                        }
                        else
                        {
                            //MessageBox.Show("Database doesn't connect");
                            RaiseEvents("Database connect fail");
                        }

                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("Error = " + ex);
                        RaiseEventsErr(ex.ToString());
                        RaiseEvents("Database connect fail");
                    }

                }
                else
                {
                    RaiseEvents("Accuload connect lost");
                }

            }

        }

        private void updatedgvLH()
        {

            string sql = "select * from loadingheaders";
            DataTable dt = new DataTable();
            dt = DatabaseLib.Excute_DataAdapter(sql);
            dataGridView1.DataSource = dt;
        }

        private void updatedgvLL()
        {
            if (load_no != null)
            {
                string sql = @"SELECT BatchNo, LoadNo, status, Compartment, ProductName, Preset FROM loadinglines where LoadNo = " + load_no;
                //string sql = "SELECT BatchNo, LoadNo, status, Compartment, ProductName, Preset FROM loadinglines";
                DataTable dt = new DataTable();
                dt = DatabaseLib.Excute_DataAdapter(sql);
                dataGridView2.DataSource = dt;
            }
            else
            {
                string sql = @"SELECT BatchNo, LoadNo, status, Compartment, ProductName, Preset FROM loadinglines where LoadNo = " + dataGridView1.SelectedCells[0].Value.ToString(); ;
                //string sql = "SELECT BatchNo, LoadNo, status, Compartment, ProductName, Preset FROM loadinglines";
                DataTable dt = new DataTable();
                dt = DatabaseLib.Excute_DataAdapter(sql);
                dataGridView2.DataSource = dt;
            }
       
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show("คุณต้องการ start loading ใช่หรือไม่ ?", "Start Loading", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                if (stpProcess)
                {
                    if (SelectedCells != null)
                    {
                       
                        checkkBox(dataGridView2.SelectedCells[0].Value.ToString());
               
                        batch_no = dataGridView2.SelectedCells[0].Value.ToString();

                        //AcculoadProcess.GetLoadNo();
                        
                        string sql = @"SELECT Preset, status FROM loadinglines where BatchNo = " + batch_no;
                   
                        string result = string.Empty;
                        string result1 = string.Empty;

                        DataTable dt = DatabaseLib.Excute_DataAdapter(sql);

                        result = dt.Rows[0]["Preset"].ToString();
                        result1 = dt.Rows[0]["status"].ToString();
                     

                        AclMember = new AcculoadLib.AcculoadMember[1];
                  
                        bool isParsable = Int32.TryParse(result, out pPreset);

                        if (isParsable && result1!= "5")
                        {
                            string vCmd1 = AcculoadLib.AllocateBlendRecipes(14, 1);
                            ClientLib.SendData(vCmd1);

                            string vCmd2 = AcculoadLib.AuthorizeSetBatch(14, pPreset);
                            ClientLib.SendData(vCmd2);

                            PullEnquireStatus();

                            try
                            {
                                PullEnquireStatus();

                                if (AclMember[0].AclValueNew.EQ.A1b0_Authorized)
                                {
                                    string vCmd3 = AcculoadLib.RemoteStart(14);
                                    ClientLib.SendData(vCmd3);
                                } 
                                else
                                {
                                    RaiseEvents("Loading Unauthorized.");
                                }
                            }
                            catch (Exception ex)
                            {
                                RaiseEventsErr(ex.ToString());
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please select load no again");
                        }

                        PullEnquireStatus();
                    }
                    else
                    {
                        MessageBox.Show("Please Select Loadinglines");
                    }
                }
                else
                {
                    MessageBox.Show("Please Connect Accuload");
                }
            }
        }

        private void btnContinueBatch_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("คุณต้องการ Continue Batch ใช่หรือไม่ ?", "Continue Batch", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                if (ClientLib.IsConnectedAcl())
                {
                    string vCmd3 = AcculoadLib.RemoteStart(14);
                    ClientLib.SendData(vCmd3);
                    AcculoadProcess = new AcculoadProcess(this);
                    AcculoadProcess.stpBatch = 3;
                    PullEnquireStatus();
                    this.Invoke(new Action(() =>
                    {
                        btnContinueBatch.Enabled = false;
                        btnEndBatch.Enabled = false;
                    }));
                }
                else
                {
                    RaiseEvents("Connection Lost...");
                }

            }
        }

        private void btnEndTransaction_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("คุณต้องการ End Transaction ใช่หรือไม่ ?", "End Transaction", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                if (ClientLib.IsConnectedAcl())
                {
                    string vCmd = AcculoadLib.EndTransaction(14);
                    ClientLib.SendData(vCmd);
                    PullEnquireStatus();
                }
                else
                {
                    RaiseEvents("Connection Lost...");
                }
               
            }

        }

        public void PullEnquireStatus()
        {

            AclMember = new AcculoadLib.AcculoadMember[1];
            AclMember[0].AclValueNew = new AcculoadLib._AcculoadValue();
            string vCmd = AcculoadLib.RequestEnquireStatus(14);
            string vData = ClientLib.SendData(vCmd);
            AcculoadLib.DecodedEnquireStatus(ref AclMember[0].AclValueNew.EQ, vData);

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("คุณต้องการ Add Loading Order ใช่หรือไม่ ?", "Stop Loading", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                if (DatabaseLib.IsServerConnected())
                {
                    FrmLoading frmDO = new FrmLoading(this);
                    frmDO.ShowDialog();
                    //RaiseEvents("Add Delivery Order");
                    updatedgvLH();
                }
                else
                {
                    RaiseEvents("Database connect fail");
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("คุณต้องการ Edit Loading Order ใช่หรือไม่ ?", "Stop Loading", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                if (DatabaseLib.IsServerConnected())
                {
                    FrmLoading frmDO = new FrmLoading(this);
                    frmDO.frmActon = 2;
                    frmDO.ShowDialog();
                    //RaiseEvents("Edit Delivery Order");
                    updatedgvLH();
                    updatedgvLL();
                }
                else
                {
                    RaiseEvents("Database connect fail");
                }
            }
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("คุณต้องการ Delete Loading Order ใช่หรือไม่ ?", "Stop Loading", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                if (DatabaseLib.IsServerConnected())
                {
                    try
                    {
                        delheader();
                        delline();
                        delback();
                        updatedgvLH();
                        MessageBox.Show("successfully");
                        RaiseEvents("Delete Delivery Order");
                    }
                    catch (Exception ex)
                    {
                        RaiseEventsErr(ex.ToString());
                        RaiseEvents("Delete not successfully");
                    }
                }
                else
                {
                    RaiseEvents("Database connect fail");
                }
            }
        }

        private void delback()
        {
            using (MySqlConnection conn = new MySqlConnection(strconn))
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                for(int i = 0; i <= delete.Length; i++)
                {
                    if (delete[i] != null)
                    {
                        Console.WriteLine(delete[2]);

                        cmd.CommandText = "delete from backuplines where BatchNo = " + delete[i];
                        cmd.ExecuteNonQuery();
              
                    }
                    else
                    {
                        break;
                    }
                }
            }
        } 

        public void delline()
        { 
            string loadno = dataGridView1.SelectedCells[0].Value.ToString();

            string sql = @"SELECT BatchNo FROM loadinglines where LoadNo = " + loadno;
            DataTable dt = DatabaseLib.Excute_DataAdapter(sql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                delete[i] = dt.Rows[0]["BatchNo"].ToString();
            }

            using (MySqlConnection conn = new MySqlConnection(strconn))
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.Parameters.AddWithValue("@LoadNo", loadno);
                cmd.CommandText = "delete from loadinglines where LoadNo = @LoadNo";
                cmd.ExecuteNonQuery();
            }
        }

        private void delheader()
        {
            string loadno = dataGridView1.SelectedCells[0].Value.ToString();
            using (MySqlConnection conn = new MySqlConnection(strconn))
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.Parameters.AddWithValue("@LoadNo", loadno);
                cmd.CommandText = "delete from loadingheaders where LoadNo = @LoadNo";
                cmd.ExecuteNonQuery();
            }
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            load_no = dataGridView1.SelectedCells[0].Value.ToString();
            string sql2 = @"SELECT BatchNo, LoadNo, status, Compartment, ProductName, Preset FROM loadinglines where LoadNo = " + load_no;
            DataTable dt2 = new DataTable();
            dt2 = DatabaseLib.Excute_DataAdapter(sql2);
            dataGridView2.DataSource = dt2;

        }

        private void dataGridView2_MouseClick(object sender, MouseEventArgs e)
        {
            SelectedCells = dataGridView2.SelectedCells[0].ToString();
        }


        private void btnStop_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("คุณต้องการ stop loading ใช่หรือไม่ ?", "Stop Loading", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                if (ClientLib.IsConnectedAcl())
                {
                    string vCmd = AcculoadLib.RemoteStop(14);
                    ClientLib.SendData(vCmd);
                    AcculoadProcess = new AcculoadProcess(this);
                    AcculoadProcess.stpBatch = 2;
                    PullEnquireStatus();
                    this.Invoke(new Action(() =>
                    {
                        btnContinueBatch.Enabled = true;
                        btnEndBatch.Enabled = true;
                    }));
                }
                else
                {
                    RaiseEvents("Connection Lost...");
                }
            }
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            FrmStatus frmStatus = new FrmStatus();
            frmStatus.ShowDialog();
        }

        private void btnResetAlarm_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("คุณต้องการ Reset Alarm ใช่หรือไม่ ?", "Reset Alarm", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                if (ClientLib.IsConnectedAcl())
                {
                    string vCmd = AcculoadLib.ResetAlarm(14);
                    ClientLib.SendData(vCmd);
                    PullEnquireStatus();
                    this.Invoke(new Action(() =>
                    {
                        btnContinueBatch.Enabled = true;
                        btnEndBatch.Enabled = true;
                    }));
                }
                else
                {
                    RaiseEvents("Connection Lost...");
                }
            }
        }

        private void btnStopProcess_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("คุณต้องการ ยกเลิกการเชื่อมต่อ ใช่หรือไม่ ?", "Disconnect", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                
                AcculoadProcess = new AcculoadProcess(this);
                AcculoadProcess.thrShutdown = true;
                ClientLib.DisconnectAcl();
                Enable_Click();
                if (backup[1] == AcculoadProcess.Batch_no) 
                {
                    using (System.IO.StreamWriter pLogFile = new StreamWriter(dirLog + "currentbatch.text", true))
                    {
                        pLogFile.WriteLine(AcculoadProcess.Batch_no);
                        pLogFile.Dispose();
                    }
                    RaiseEvents("---------------Application Stop---------------");
                    this.Invoke(new Action(() =>
                    {
                        btnConnectAcl.Enabled = true;
                    }));
                }
            }
        }


        private void btnEndBatch_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("คุณต้องการ End Batch ใช่หรือไม่ ?", "End Batch", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                if (ClientLib.IsConnectedAcl())
                {
                    string vCmd = AcculoadLib.EndBatch(14);
                    ClientLib.SendData(vCmd);
                    AcculoadProcess = new AcculoadProcess(this);
                    AcculoadProcess.cnlBatch = 2;
                    PullEnquireStatus();
                    this.Invoke(new Action(() =>
                    {
                        btnContinueBatch.Enabled = false;
                        btnEndTransaction.Enabled = true;
                    }));
                }
                else
                {
                    RaiseEvents("Connection Lost...");
                }
            }
        }

//<<<<<<< HEAD
//=======
        public void Show_TextBox()
        {
            try
            {
            
                string sql = @"SELECT TotalizerGV, CurrentPreset, LoadedGV, CurrentFlowrate FROM backuplines where BatchNo = " + AcculoadProcess.Batch_no;
                DataTable dt = DatabaseLib.Excute_DataAdapter(sql);

                if (dt.Rows.Count > 0)
                {
                    txtFlowRate.Invoke((MethodInvoker)(() => txtFlowRate.Text = dt.Rows[0]["CurrentFlowrate"].ToString()));
                
                    txtGV.Invoke((MethodInvoker)(() => txtGV.Text = dt.Rows[0]["LoadedGV"].ToString()));
                 
                    txtPreset.Invoke((MethodInvoker)(() => txtPreset.Text = dt.Rows[0]["CurrentPreset"].ToString()));

                    txtTotalizer.Invoke((MethodInvoker)(() => txtTotalizer.Text = dt.Rows[0]["TotalizerGV"].ToString()));
                } 
                UPdate_loadinglines();
            }
            catch (Exception ex)
            {
                RaiseEventsErr(ex.ToString());
               // string vCmd = AcculoadLib.RemoteStop(14);
               // ClientLib.SendData(vCmd);
            }
        }

        public void UPdate_loadinglines()
        {
            try
            {
                string sql = @"SELECT TotalizerGV, TotalizerGST, TotalizerGSV, CurrentPreset, CurrentFlowrate, LoadedGV, LoadedGST, LoadedGSV FROM backuplines where BatchNo =  " + AcculoadProcess.Batch_no;
                DataTable dt = DatabaseLib.Excute_DataAdapter(sql);
                if (dt.Rows.Count > 0)
                {
                    string StrQuery = string.Format("update loadinglines set CurrentPreset = {1}, CurrentFlowrate = {2}, LoadedGV = {3}, LoadedGST = {4}, LoadedGSV = {5}, TotalizerGV = {6}, TotalizerGST = {7}, TotalizerGSV = {8} WHERE BatchNo = {0}",
                        AcculoadProcess.Batch_no, dt.Rows[0]["CurrentPreset"].ToString(), dt.Rows[0]["CurrentFlowrate"].ToString(), dt.Rows[0]["LoadedGV"].ToString(), dt.Rows[0]["LoadedGST"].ToString(),
                        dt.Rows[0]["LoadedGSV"].ToString(), dt.Rows[0]["TotalizerGV"].ToString(), dt.Rows[0]["TotalizerGST"].ToString(), dt.Rows[0]["TotalizerGSV"].ToString());

                    DatabaseLib.ExecuteSQL(StrQuery);
                }
            }
            catch (Exception ex)   
            {
                RaiseEventsErr(ex.ToString());
                string vCmd = AcculoadLib.RemoteStop(14);
                ClientLib.SendData(vCmd);
            }
        }

        public void Enable_Click()
        {
            this.Invoke(new Action(() =>
            {
                btnContinueBatch.Enabled = true;
                btnEndTransaction.Enabled = true;
                btnEndBatch.Enabled = true;
                btnStart.Enabled = true;
            }));
        }
        public void End_Click()
        {
            this.Invoke(new Action(() =>
            {
                btnEndTransaction.Enabled = true;
            }));
        }

        public void Disable_Click()
        {
            this.Invoke(new Action(() =>
            {
                btnContinueBatch.Enabled = false;
                btnEndTransaction.Enabled = false;
                btnEndBatch.Enabled = false;
                btnStart.Enabled = false;
            }));
        }

        public void checkkBox(string BatchNum)
        {
            string Checkk = DatabaseLib.ExecuteReader_bBatchNum(@"SELECT BatchNo FROM backuplines where BatchNo = " + BatchNum);
            if (Checkk == "")
            {
                string StrQuery = string.Format("INSERT INTO backuplines (BatchNo, CurrentPreset, CurrentFlowrate, LoadedGV, LoadedGST, LoadedGSV, TotalizerGV, TotalizerGST, TotalizerGSV) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}); ",
                BatchNum, "0", "0", "0", "0", "0", "0", "0", "0");
                DatabaseLib.ExecuteSQL(StrQuery);
            }

            AcculoadProcess = new AcculoadProcess(this);
            AcculoadProcess.Batch_no = BatchNum;
            backup[1] = AcculoadProcess.Batch_no;
        }

        private void FrmMain_Closed(object sender, FormClosedEventArgs e)
        {
            if (backup[1] == null) 
            {
                backup[1] = backup[0];
                using (System.IO.StreamWriter pLogFile = new StreamWriter(dirLog + "currentbatch.text", true))
                {
                    pLogFile.WriteLine(backup[1]);
                    pLogFile.Dispose();
                }
            }
        }
    }
}

