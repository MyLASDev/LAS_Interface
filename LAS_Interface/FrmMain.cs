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
        public CLogfiles LogFile = new CLogfiles();

        IPEndPoint remoteEP;
        Socket socket;

        public string batch_no;
        public string SelectedCells;
        public string load_no;
        public int pPreset;

        private bool stpProcess = false;
        private int currentBatch;
        
        public FrmMain()  
        {
            InitializeComponent();
            
        }
       
        private void FrmMain_Load(object sender, EventArgs e)
        {
            AcculoadProcess.frmmain = this;
            timer1.Start();
            RaiseEvents("Application Start");
            updatedgvLH();
            txtFlowRate.Text = "0";
            txtPreset.Text = "0";
            txtGV.Text = "0";
            try
            {
                using (StreamReader readtext = new StreamReader(dirLog + "currentbatch.text", true))
                {
                    string readText = readtext.ReadLine();
                    currentBatch = Int32.Parse(readText);
                    
                }
                File.Delete(dirLog + "currentbatch.text");
                string sql = @"SELECT TotalizerGV FROM loadinglines where BatchNo = " + currentBatch; 
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
                       
                    }
                    reader.Close();
                } 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                    RaiseEvents("Connection Success");
                }
                else
                {
                    RaiseEvents("Connection lost");
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

        private void btnStart_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show("คุณต้องการ start loading ใช่หรือไม่ ?", "Start Loading", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes) 
            { 
                if (stpProcess)
                {
                    if (SelectedCells != null)
                    {
                        AcculoadProcess = new AcculoadProcess(this);
                        AcculoadProcess.Batch_no = dataGridView2.SelectedCells[0].Value.ToString();

                        using (System.IO.StreamWriter pLogFile = new StreamWriter(dirLog + "currentbatch.text", true))
                        {
                            pLogFile.WriteLine(AcculoadProcess.Batch_no);
                            pLogFile.Dispose();
                        }

                        batch_no = dataGridView2.SelectedCells[0].Value.ToString();

                        string sql = @"SELECT Preset FROM loadinglines where BatchNo = " + batch_no;
                        string result = string.Empty;
                        AclMember = new AcculoadLib.AcculoadMember[1];

                        result = DatabaseLib.ExecuteReader_pPreset(sql);
                        bool isParsable = Int32.TryParse(result, out pPreset);

                        if (isParsable)
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
                                    RaiseEvents("------------------------Start Load----------------------------");
                                }
                                else
                                {
                                    MessageBox.Show("Error");
                                }

                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Error");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Error");
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
                    MessageBox.Show("Please Start Process");
                }
            }
        }

        private void btnContinueBatch_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("คุณต้องการ Continue Batch ใช่หรือไม่ ?", "Continue Batch", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                string vCmd3 = AcculoadLib.RemoteStart(14);
                ClientLib.SendData(vCmd3);
                AcculoadProcess = new AcculoadProcess(this);
                AcculoadProcess.stpBatch = 3;
                PullEnquireStatus();
                RaiseEvents("------------------------Continue Batch--------------------------------");
            }
        }

        private void btnEndTransaction_Click(object sender, EventArgs e)
        {
           
            DialogResult result = MessageBox.Show("คุณต้องการ End Transaction ใช่หรือไม่ ?", "End Transaction", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                string vCmd = AcculoadLib.EndTransaction(14);
                ClientLib.SendData(vCmd);
                PullEnquireStatus();
                RaiseEvents("-----------------------End Transaction-----------------------------");           
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
           FrmLoading frmDO = new FrmLoading(this);
           frmDO.ShowDialog();
           RaiseEvents("Add Delivery Order");
           updatedgvLH();
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
                MessageBox.Show("Error = "+ ex);
                RaiseEvents("Delete not successfully");
            }

        }

        private void delline()
        {
            string loadno = dataGridView1.SelectedCells[0].Value.ToString();
            MySqlConnection conn = new MySqlConnection(strconn);
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.Parameters.AddWithValue("@LoadNo", loadno);
            cmd.CommandText = "delete from loadinglines where LoadNo = @LoadNo";
            cmd.ExecuteNonQuery();
        }

        private void delheader()
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
            string sql2 = @"SELECT BatchNo, LoadNo, status,Compartment, ProductName, Preset
                              FROM loadinglines where LoadNo = " + load_no;
           

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
                string vCmd = AcculoadLib.RemoteStop(14);
                ClientLib.SendData(vCmd);
                AcculoadProcess = new AcculoadProcess(this);
                AcculoadProcess.stpBatch = 2;
                PullEnquireStatus();
                RaiseEvents("--------------------------------Stop Load--------------------------");      
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
                string vCmd = AcculoadLib.ResetAlarm(14);
                ClientLib.SendData(vCmd);
                PullEnquireStatus();
                RaiseEvents("----------------------Reset Alarm-------------------------------");         
            } 
        }

        private void btnStartProcess_Click(object sender, EventArgs e)
        {
            try
            {
                stpProcess = true;
                AcculoadProcess = new AcculoadProcess(this);
                AcculoadProcess.stpBatch = 1;
                if (DatabaseLib.IsServerConnected())
                {
                    if (ClientLib.getIsConnectAcl())
                    {
                        AcculoadProcess.StartThread();
                    }
                    else
                    { 
                        MessageBox.Show("Accuload doesn't connect");
                    }
                }
                else
                {
                    MessageBox.Show("Database doesn't connect");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error = " + ex);
            }
        }

        private void btnStopProcess_Click(object sender, EventArgs e)
        {
            AcculoadProcess = new AcculoadProcess(this);
            AcculoadProcess.thrShutdown = true;
        }

        private void btnEndBatch_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("คุณต้องการ End Batch ใช่หรือไม่ ?", "End Batch", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                string vCmd = AcculoadLib.EndBatch(14);
                ClientLib.SendData(vCmd);
                AcculoadProcess = new AcculoadProcess(this);
                AcculoadProcess.cnlBatch = 2;
                PullEnquireStatus();
                RaiseEvents("---------------------------End Batch----------------------------");
            }   
        } 

        public void Show_TextBox()
        {
            try
            {
                string sql = @"SELECT TotalizerGV, CurrentPreset, LoadedGV, CurrentFlowrate FROM loadinglines where BatchNo = " + AcculoadProcess.Batch_no;
                string result = DatabaseLib.ExecuteReader_pPreset(sql);
               
                using (MySqlConnection connection = new MySqlConnection(strconn))
                {
                    AcculoadProcess = new AcculoadProcess(this);
                    MySqlCommand command = new MySqlCommand(sql, connection);
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    string Read = string.Empty;
                    while (reader.Read())
                    {
                        if (AcculoadProcess.stepFlowrate)
                        {
                            txtFlowRate.Invoke((MethodInvoker)(() => txtFlowRate.Text = reader.GetString("CurrentFlowrate")));
                        } 

                        if (AcculoadProcess.stepTotalizer)
                        {
                            txtTotalizer.Invoke((MethodInvoker)(() => txtTotalizer.Text = reader.GetString("TotalizerGV")));
                        }

                        if (AcculoadProcess.stepPreset)
                        {
                            txtPreset.Invoke((MethodInvoker)(() => txtPreset.Text = reader.GetString("CurrentPreset")));
                        }

                        if (AcculoadProcess.stepLoaded)
                        {
                            txtGV.Invoke((MethodInvoker)(() => txtGV.Text = reader.GetString("LoadedGV")));
                        } 
                 
                    }
                    reader.Close(); 
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
