using Library;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Xml.Linq;
using Google.Protobuf.WellKnownTypes;

namespace LAS_Interface
{
    public partial class FrmMain : Form
    {
        string strconn = "server=r98du2bxwqkq3shg.cbetxkdyhwsb.us-east-1.rds.amazonaws.com;database=ahda1gtbqhb7pncg;uid=hktvkvjk6993txuk;pwd=ma46ffmhhxgl0zj6";
        IPEndPoint remoteEP;
        Socket socket;
        public AcculoadLib.AcculoadMember[] AclMember;
        public string load_no;
        public string batch_no;
        public int pPreset;


        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            timer1.Start();
            RaiseEvents("Application Start");
            updatedgvLH();
           
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
            string sql = "select * from loadingheaders";
            DataTable dt = new DataTable();
            dt = DatabaseLib.Excute_DataAdapter(sql);
            dataGridView1.DataSource = dt;

        }

        public void updatedgvLL()
        {
            string sql = "select * from LoadingLine";
            DataTable dt = new DataTable();
            dt = DatabaseLib.Excute_DataAdapter(sql);
            dataGridView2.DataSource = dt;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            batch_no = dataGridView2.SelectedCells[0].Value.ToString();

            string sql = @"SELECT BatchNo, LoadNo, Compartment, ProductName, Preset, LoadindVolume, CreatedAt, UpdatedAt FROM loadinglines where BatchNo = " + batch_no;
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
                RaiseEvents("----------------------------------------------------Start Load----------------------------------------------------");
                CurrentStatus();

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
            CurrentStatus();

            /*string vCmd = AcculoadLib.EndBatch(14);
            ClientLib.SendData(vCmd);
            RaiseEvents("----------------------------------------------------End Transaction----------------------------------------------------");
            CurrentStatus();*/

            /*string vCmd = AcculoadLib.ResetAlarm(14);
            ClientLib.SendData(vCmd);
            RaiseEvents("----------------------------------------------------Reset Alarm----------------------------------------------------");
            CurrentStatus();*/

            /*string vCmd = AcculoadLib.EndBatch(14);
            ClientLib.SendData(vCmd);
            RaiseEvents("----------------------------------------------------End Batch----------------------------------------------------");
            CurrentStatus();*/

            /*string vCmd = AcculoadLib.RemoteStop(14);
            ClientLib.SendData(vCmd);
            RaiseEvents("----------------------------------------------------Stop Load----------------------------------------------------");
            CurrentStatus();*/
        }
            
        private void btnEnd_Click(object sender, EventArgs e)
        {
            string vCmd = AcculoadLib.EndTransaction(14);
            ClientLib.SendData(vCmd);
            RaiseEvents("----------------------------------------------------End Transaction----------------------------------------------------");
        }

       /* private void btnEQ_Click(object sender, EventArgs e)
        {
            PullEnquireStatus();
            RaiseEvents("Transaction Done = " + AclMember[0].AclValueNew.EQ.A2b2_TransactionDone.ToString());
        }*/
        public void PullEnquireStatus()
        {

            AclMember = new AcculoadLib.AcculoadMember[1];
            AclMember[0].AclValueNew = new AcculoadLib._AcculoadValue();
            string vCmd = AcculoadLib.RequestEnquireStatus(14);
            string vData = ClientLib.SendData(vCmd);
            AcculoadLib.DecodedEnquireStatus(ref AclMember[0].AclValueNew.EQ, vData);

        }

        public void CurrentStatus()
        {
            
            PullEnquireStatus();

            if (AclMember[0].AclValueNew.EQ.A1b3_ProgramMode)
            {
                RaiseEvents("ProgramMode = " + AclMember[0].AclValueNew.EQ.A1b3_ProgramMode);
            }
            if (AclMember[0].AclValueNew.EQ.A1b2_Release)
            {
                RaiseEvents("Release = " + AclMember[0].AclValueNew.EQ.A1b2_Release);
            }
            if (AclMember[0].AclValueNew.EQ.A1b1_Flowing)
            {
                RaiseEvents("Flowing = " + AclMember[0].AclValueNew.EQ.A1b1_Flowing);
            }
            if (AclMember[0].AclValueNew.EQ.A1b0_Authorized)
            {
                RaiseEvents("Authorized = " + AclMember[0].AclValueNew.EQ.A1b0_Authorized);
            }
            if (AclMember[0].AclValueNew.EQ.A2b3_TransactionInProgress)
            {
                RaiseEvents("TransactionInProgress = " + AclMember[0].AclValueNew.EQ.A2b3_TransactionInProgress);
            }
            if (AclMember[0].AclValueNew.EQ.A2b2_TransactionDone)
            {
                RaiseEvents("TransactionDone = " + AclMember[0].AclValueNew.EQ.A2b2_TransactionDone);
            }
            if (AclMember[0].AclValueNew.EQ.A2b1_BatchDone)
            {
                RaiseEvents("BatchDone = " + AclMember[0].AclValueNew.EQ.A2b1_BatchDone);
            }
            if (AclMember[0].AclValueNew.EQ.A3b3_AlarmOn)
            {
                RaiseEvents("AlarmOn = " + AclMember[0].AclValueNew.EQ.A3b3_AlarmOn);
            }
            if (AclMember[0].AclValueNew.EQ.A3b2_StandbyTransactionExist)
            {
                RaiseEvents("StandbyTransactionExist = " + AclMember[0].AclValueNew.EQ.A3b2_StandbyTransactionExist);
            }
            if (AclMember[0].AclValueNew.EQ.A3b1_StorageFull)
            {
                RaiseEvents("StorageFull = " + AclMember[0].AclValueNew.EQ.A3b1_StorageFull);
            }
            if (AclMember[0].AclValueNew.EQ.A3b0_InStandbyMode)
            {
                RaiseEvents("InStandbyMode = " + AclMember[0].AclValueNew.EQ.A3b0_InStandbyMode);
            }
            if (AclMember[0].AclValueNew.EQ.A4b3_ProgramValueChange)
            {
                RaiseEvents("ProgramValueChange = " + AclMember[0].AclValueNew.EQ.A4b3_ProgramValueChange);
            }
            if (AclMember[0].AclValueNew.EQ.A4b2_DelayPrompt)
            {
                RaiseEvents("DelayPrompt = " + AclMember[0].AclValueNew.EQ.A4b2_DelayPrompt);
            }
            if (AclMember[0].AclValueNew.EQ.A4b1_DisplayMessageTimeout)
            {
                RaiseEvents("DisplayMessageTimeout = " + AclMember[0].AclValueNew.EQ.A4b1_DisplayMessageTimeout);
            }
            if (AclMember[0].AclValueNew.EQ.A4b0_PowerFailOccurred)
            {
                RaiseEvents("PowerFailOccurred = " + AclMember[0].AclValueNew.EQ.A4b0_PowerFailOccurred);
            }
            if (AclMember[0].AclValueNew.EQ.A5b3_CheckingEntries)
            {
                RaiseEvents("CheckingEntries = " + AclMember[0].AclValueNew.EQ.A5b3_CheckingEntries);
            }
            if (AclMember[0].AclValueNew.EQ.A5b1_Input2)
            {
                RaiseEvents("Input2 = " + AclMember[0].AclValueNew.EQ.A5b1_Input2);
            }
            if (AclMember[0].AclValueNew.EQ.A16b3_PrintingInProgress)
            {
                RaiseEvents("PrintingInProgress = " + AclMember[0].AclValueNew.EQ.A16b3_PrintingInProgress);
            }
            if (AclMember[0].AclValueNew.EQ.A16b2_PermissiveDelay)
            {
                RaiseEvents("PermissiveDelay = " + AclMember[0].AclValueNew.EQ.A16b2_PermissiveDelay);
            }
            if (AclMember[0].AclValueNew.EQ.A16b1_CardDataPresent)
            {
                RaiseEvents("CardDataPresent = " + AclMember[0].AclValueNew.EQ.A16b1_CardDataPresent);
            }
            if (AclMember[0].AclValueNew.EQ.A16b0_PresetInProgress)
            {
                RaiseEvents("PresetInProgress = " + AclMember[0].AclValueNew.EQ.A16b0_PresetInProgress);
            }
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error");
            }

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
            string sql2 = @"SELECT BatchNo, LoadNo, Compartment, ProductName, Preset
                              FROM loadinglines where LoadNo = " + load_no;
           

           /* if ()
            {
                

            }*/
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

        private void button2_Click(object sender, EventArgs e)
        {
            const byte STX = 2;
            Console.WriteLine(char.ConvertFromUtf32(STX));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string vCmd = AcculoadLib.EndBatch(14);
            ClientLib.SendData(vCmd);
            RaiseEvents("----------------------------------------------------End Transaction----------------------------------------------------");
            CurrentStatus();
        }

   

    

        

    
    }
}
