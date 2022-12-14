using System;
using Library;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Globalization;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using static Library.AcculoadLib;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.CodeDom;
using System.Security.Cryptography;
using Ubiety.Dns.Core.Records.NotUsed;

namespace LAS_Interface
{
    public class AcculoadProcess 
    {
        const byte NUL = 0;
        const byte STX = 2;
        const byte ETX = 3;
        const byte ESC = 27;
        const byte EM = 25;
        const byte SP = 32;
        const byte PAD = 127;

        int STX_Position;
        int ETX_Position;
        int DATA_Position;
        int readCount;

        Thread thrAclProcess;
        Thread thrAclRead;
        public static FrmMain frmmain;

        public struct AcculoadMember
        {

            //public AcculoadLib AclLib;
            public AcculoadLib._AcculoadValue AclValueNew;
            public AcculoadLib._AcculoadValue AclValueOld;

            public string MeterNo;

            public bool IsConnect;
            public Int64 BatchID;
            public string BatchName;
            public Int64 MeterID;
            public string MeterName;
            public int RecipeNo;
            public string IslandNo;
            public string IslandName;
            public int BayNo;
            public string BayName;
            public int IndexComport;   //select comp_id1 or 2
            public string ProductName;
            public double[] CompID;
            public DateTime LastReadTime;
            public bool DiagComp;

            public string MsgSend;
            public string MsgRecv;
            public int IntervalUpdate;
            public DateTime LastUpdated;

            public double StartTotalizerGV;
            public double StartTotalizerGST;
            public double StartTotalizerGSV;
            public double StartTotalizerMass;

            public int OldLoadActive;
            public _BatchMode NewBatchModeActive;
            public _BatchMode OldBatchModeActive;
            public _BatchStepProcess BatchStepProcess;
            public _BatchStepProcess LastBatchStepProcess;

            public int LastBatching;
            public int AuthorizeBayNo;
            public double AuthorizeLoadNo;
            public double AuthorizeLineNo;
            public int AuthorizeLoadCount;
            public double AuthorizeTopupNo;
            public int EndBatch;
            public int EndTransaction;
            public bool StopBatch;
            public bool EndLoad;
            public bool CancelLoad;
            public int ResetAlarmBatch;
            public int LockIsland;
            public int LockBay;
            public int LockBatch;
            public bool BatchScan;
            public bool OldBatchScan;
            public int LoadActive;
            public double LoadNo;
            public double BatchNo;
            public int LoadCount;
            public int CompTotal;
            public double Preset;
            public double Density15C;
            public double Density30C;
            public double VCF30C;
            public int WriteVCF30;
            public double TopupPreset;
            public int OldBayNo;
            public double TopupNo;
            public double ActiveTopupNo;
            public int KeyCompNo;
            public string KeyTopupNo;
            public int KeyTopupCompNo;
            public int HostCommandResult;
            public string Shipment;
            public string CompList;
            public DateTime TimeEndLoad;
            public DateTime TimeoutStartBatch;
            public DateTime TimeGetTopupNo;
            public DateTime TimeUpdateLoading;
            public int RET_CHECK;
            public string RET_MSG;

        }

        private enum _StepReadData
        {
            ReadMeterTotalizer
        ,
            ReadRecipeTotalizer
        ,
            ReadBatchCurrentValue
        }

        public enum _BatchMode
        {
            Disable = 0,
            OffLine,
            Initail,
            Auto,
            Topup
        }

        public enum _BatchStepProcess : int
        {
            bspNone = 0,
            bspBCActive,
            bspGetTruckPin,
            bspGetCompartment,
            bspAutoPreset,
            bspPreset,
            bspStart,
            bspLoading,
            bspStop,
            bspFirstEnd,
            bspChangeBayEnd,

            topupActive,
            topupGetTopupNo,
            topupGetCompartment,
            topupPreset,
            topupStart,
            topupLoading,
            topupStop,
            topupFirstEnd,
            topupChangeBayEnd
        }

        private const int AclAddress = 14;
        private const int MeterNo = 14;
        private const int ProductNo = 1;
        private int stepThreadReadData = 0;
        private int batchStatus = 0;
        private bool eCheck = false;
        private bool tstDone = false;
        private bool prepare = false;
        private bool OnLoad = false;
        private bool responseStatus = false;
        private string msgSend;
        private string records;
        private DateTime responseTime;

        bool Reset = true;
        bool thrRunning;
        bool firstStart;
        bool readComplete = false;

        public AcculoadMember[] AclMember;
        public AcculoadLib.AcculoadMember[] AclMember_Lib;

        public static int stpBatch = 0;
        public static int cnlBatch = 0;
        public static bool thrShutdown;
        public static bool Pass = false;
        public static string Batch_no;
       
        _StepReadData stepReadData;
        FrmMain frmMain;

        public AcculoadProcess(FrmMain frmMain)
        {
            this.frmMain = frmMain;
        }

        public void StartThread()
        {
            try
            {

               // frmMain.RaiseEvents("Start read from accuload");
                
                thrRunning = true;
                thrShutdown = false;

                thrAclProcess = new Thread(this.LoadingProcess);
                thrAclProcess.Start();

                thrAclProcess = new Thread(this.StartThreadReadData);
                thrAclProcess.Start();

            }
            catch (Exception exp)
            {
                thrRunning = false;
                frmMain.RaiseEventsErr(exp.ToString());
            }
        }

        public void StopThread()
        {
            thrShutdown = true;

        }

        private void StartThreadReadData()
        {
            AclMember_Lib = new AcculoadLib.AcculoadMember[1];
            AclMember_Lib[0].AclValueNew = new AcculoadLib._AcculoadValue();
            AclMember_Lib[0].AclValueNew.MeterValue = new AcculoadLib._MeterValue[1];
            stepReadData = _StepReadData.ReadBatchCurrentValue;
            while (thrRunning)
            {
                
                Thread.Sleep(300);

                if (thrShutdown)
                {
                    break;
                }
                switch (stepReadData)
                {
                    case _StepReadData.ReadBatchCurrentValue:
                        {
                            ReadBatchCurrentValue();
                            ReadBatchDeliveryValue();
                        }

                        stepReadData = _StepReadData.ReadMeterTotalizer;
                        break;

                    case _StepReadData.ReadMeterTotalizer:
                        {
                            ReadMeterTotalizer();
                        }

                        stepReadData = _StepReadData.ReadBatchCurrentValue;
                        break;
                }
          
                UpdateBatchValue();
                ClientLib.IsConnectedAcl();
            }
        }

        public void ReadBatchCurrentValue()
        {
            string vCmd = "";
            string vRecv = "";
            string vCheck = "";
            double d;

            try
            {
                
                int i = 0, j = 0;

                vCmd = AcculoadLib.RequestCurrentFlowRate(AclAddress);
                vRecv = TxRxAccuload(AclAddress, vCmd);
                vCheck = AclAddress.ToString("D2") + "RQ";

                if (vRecv != null)
                {
                    try
                    {
                        if (vRecv.IndexOf(vCheck) >= 0)
                        {

                            bool b = double.TryParse(vRecv.Substring(vCheck.Length + 2, vRecv.IndexOf(char.ConvertFromUtf32(ETX)) - vCheck.Length - 2).Trim(), out d);
                            AclMember_Lib[0].AclValueNew.MeterValue[j].CurrentFlowrate = d;
                        }
                    }
                    catch (Exception exp)
                    {
                        frmMain.RaiseEvents(exp.ToString());
                    }
                }

                vCmd = AcculoadLib.RequestPreset(AclAddress);
                vRecv = TxRxAccuload(AclAddress, vCmd);
                vCheck = AclAddress.ToString("D2") + "RP";
                if (vRecv != null)
                {
                    try
                    {
                        if ((vRecv.IndexOf(vCheck) >= 0) && (vRecv.ToLower().Contains("not available") == false))
                        {

                            bool b = double.TryParse(vRecv.Substring(vCheck.Length + 2, vRecv.IndexOf(char.ConvertFromUtf32(ETX)) - vCheck.Length - 2).Trim(), out d);
                            AclMember_Lib[0].AclValueNew.MeterValue[j].CurrentPreset = d;
                        }
                    }
                    catch (Exception exp)
                    {
                        frmMain.RaiseEventsErr(exp.ToString());
                    }
                }
                
            }
            catch (Exception exp)
            {
                frmMain.RaiseEventsErr(exp.ToString());
            }
        }

        private void ReadBatchDeliveryValue()
        {
            string vCmd = "";
            string vRecv = "";
            string vCheck = "";
            double d;

            try
            {
         
                int i = 0, j = 0;

                vCmd = AcculoadLib.RequestDeliveryGV(AclAddress, ProductNo); //current volume 
                vRecv = TxRxAccuload(AclAddress, vCmd);
                vCheck = AclAddress.ToString("D2") + "DY GV Batch";
                if (vRecv != null)
                {
                    try
                    {
                        if ((vRecv.IndexOf(vCheck) >= 0) && (vRecv.ToLower().Contains("not available") == false))
                        {
                            bool b = double.TryParse(vRecv.Substring(vCheck.Length + 4, 16).Trim(), out d);
                            AclMember_Lib[0].AclValueNew.MeterValue[j].DeliveryGV = d;
                        }
                    }
                    catch (Exception exp)
                    {
                        frmMain.RaiseEvents(exp.ToString());
                    }
                }

                vCmd = AcculoadLib.RequestDeliveryGST(AclAddress, ProductNo);
                vRecv = TxRxAccuload(AclAddress, vCmd);
                vCheck = AclAddress.ToString("D2") + "DY GST Batch";
                if (vRecv != null)
                {
                    try
                    {
                        if ((vRecv.IndexOf(vCheck) >= 0) && (vRecv.ToLower().Contains("not available") == false))
                        {
                            string kk = vRecv.Substring(vCheck.Length + 4, 15).Trim();
                            bool b = double.TryParse(kk, out d);
                            AclMember_Lib[0].AclValueNew.MeterValue[j].DeliveryGST = d;

                        }
                    }
                    catch (Exception exp)
                    {
                        frmMain.RaiseEventsErr(exp.ToString());
                    }
                }

                vCmd = AcculoadLib.RequestDeliveryGSV(AclAddress, ProductNo);
                vRecv = TxRxAccuload(AclAddress, vCmd);
                vCheck = AclAddress.ToString("D2") + "DY GSV Batch";
                if (vRecv != null)
                {
                    try
                    {
                        if ((vRecv.IndexOf(vCheck) >= 0) && (vRecv.ToLower().Contains("not available") == false))
                        {
                            string kk = vRecv.Substring(vCheck.Length + 4, 15).Trim();
                            bool b = double.TryParse(kk, out d);
                            AclMember_Lib[0].AclValueNew.MeterValue[j].DeliveryGSV = d;
                        }
                    }
                    catch (Exception exp)
                    {
                        frmMain.RaiseEventsErr(exp.ToString()); 
                    }
                }
            }

            catch (Exception exp)
            {
                frmMain.RaiseEventsErr(exp.ToString()); 
            }
        }

        private void ReadMeterTotalizer()
        {

            string vCmd = string.Empty;
            string vRecv = "";
            string vCheck = string.Empty;
            double d;

            try
            {
             
                int i = 0, j = 0;

                vCmd = AcculoadLib.RequestMeterTotalizerGV(AclAddress, ProductNo); //show totalizer 
                vRecv = TxRxAccuload(AclAddress, vCmd);
                if (vRecv != null)
                {
                    if (vRecv.IndexOf("VT G") > 0)
                    {
                        AclMember_Lib[0].AclValueNew.MeterValue[j].TotalizerGV = Convert.ToInt64(vRecv.Substring(vRecv.IndexOf("VT G") + 8, 9).Trim());
                    }
                }

                vCmd = AcculoadLib.RequestMeterTotalizerGST(AclAddress, ProductNo);
                vRecv = TxRxAccuload(AclAddress, vCmd);
                if (vRecv != null)
                {
                    if (vRecv.IndexOf("VT N") > 0)
                    {
                        AclMember_Lib[0].AclValueNew.MeterValue[j].TotalizerGST = Convert.ToInt64(vRecv.Substring(vRecv.IndexOf("VT N") + 8, 9).Trim());

                    }
                }

                vCmd = AcculoadLib.RequestMeterTotalizerGSV(AclAddress, ProductNo);
                vRecv = TxRxAccuload(AclAddress, vCmd);
                if (vRecv != null)
                {
                    if (vRecv.IndexOf("VT P") > 0)
                    {
                        AclMember_Lib[0].AclValueNew.MeterValue[j].TotalizerGSV = Convert.ToInt64(vRecv.Substring(vRecv.IndexOf("VT P") + 8, 9).Trim());
                    }
                }
            }
            catch (Exception exp)
            {
                frmMain.RaiseEventsErr(exp.ToString());
            }

        }

        public void LoadingProcess()
        {

            try
            {
                AclMember = new AcculoadProcess.AcculoadMember[1];
                AclMember[0].BatchStepProcess = _BatchStepProcess.bspBCActive;
            
                while (thrRunning)
                {
                    if (ClientLib.getIsConnectAcl())
                    {
                        Thread.Sleep(500);

                        if (stpBatch == 3)
                        {
                            AclMember[0].StopBatch = false;
                        }
                        if (stpBatch == 2)
                        {
                            AclMember[0].StopBatch = true;
                        }
                        if (cnlBatch == 2)
                        {
                            AclMember[0].CancelLoad = true;
                        }

                        if (thrShutdown)
                        {
                            break;
                        }

                        switch (AclMember[0].BatchStepProcess)
                        {

                            case _BatchStepProcess.bspBCActive:
                                Loading_Active();
                                break;
                            case _BatchStepProcess.bspStart:
                                Loading_Start();
                                break;
                            case _BatchStepProcess.bspLoading:
                                Loading_Loading();
                                break;
                            case _BatchStepProcess.bspStop:
                                Loading_Stop();
                                break;
                            default:
                                break;
                        }
                        PullEnquireStatus();
                    }
                    else
                    {
                        frmMain.RaiseEvents("Connection Lost... ");
                        Thread.Sleep(1000);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                frmMain.RaiseEventsErr(ex.ToString());
            }

        }

        private void Loading_Active()
        {
            //PullEnquireStatus();

            Console.WriteLine("Loading Active");
            OnLoad = false;

            if (AclMember[0].AclValueNew.EQ.A1b0_Authorized && AclMember[0].AclValueNew.EQ.A16b0_PresetInProgress)
            {
                GetLoadNo();
                frmMain.Disable_Click();
                batchStatus = 1;
                string StrQuery = string.Format("update loadinglines set  status = {0}, UpdatedAt = CURRENT_TIMESTAMP WHERE BatchNo = {1}", batchStatus, Batch_no);
                DatabaseLib.ExecuteSQL(StrQuery);

                frmMain.RaiseEvents("Load No[" + AclMember[0].LoadNo + "]" + " Batch No[" + AclMember[0].BatchNo + "]" + " remote start batch.");
                Thread.Sleep(700);
                frmMain.RaiseEvents("Valve open delay.");

                AclMember[0].BatchStepProcess = _BatchStepProcess.bspStart;
                stepThreadReadData = stepThreadReadData + 1;
            }
        }

        private void Loading_Start()
        {
            string vCmd;

            GetLoadNo();
            //PullEnquireStatus();

            Console.WriteLine("Loading Start");
  
            if (AclMember[0].AclValueNew.EQ.A1b0_Authorized)
            {

                if (!AclMember[0].AclValueNew.EQ.A3b3_AlarmOn && AclMember[0].AclValueNew.EQ.A2b3_TransactionInProgress && AclMember[0].AclValueNew.EQ.A1b1_Flowing)
                {
                    batchStatus = 2;
                    string StrQuery = string.Format("update loadinglines set  status = {0}, UpdatedAt = CURRENT_TIMESTAMP WHERE BatchNo = {1}", batchStatus, Batch_no);
                    DatabaseLib.ExecuteSQL(StrQuery);

                    frmMain.RaiseEvents("Load No[" + AclMember[0].LoadNo + "]" + " Batch No[" + AclMember[0].BatchNo + "]" + " loading.");
                    
                    AclMember[0].BatchStepProcess = _BatchStepProcess.bspLoading;

                    OnLoad = true;

                }

                if (AclMember[0].AclValueNew.EQ.A3b3_AlarmOn)
                {
                    batchStatus = 3;
                    string StrQuery = string.Format("update loadinglines set  status = {0}, UpdatedAt = CURRENT_TIMESTAMP WHERE BatchNo = {1}", batchStatus, Batch_no);
                    DatabaseLib.ExecuteSQL(StrQuery);

                    frmMain.RaiseEvents("Load No[" + AclMember[0].LoadNo + "]" + " Batch No[" + AclMember[0].BatchNo + "]" + " alarm on[Zero Flow].");
                    Reset = false;

                    AclMember[0].BatchStepProcess = _BatchStepProcess.bspStop;
                }

                if (AclMember[0].StopBatch)
                {
                    batchStatus = 3;
                    string StrQuery = string.Format("update loadinglines set  status = {0}, UpdatedAt = CURRENT_TIMESTAMP WHERE BatchNo = {1}", batchStatus, Batch_no);
                    DatabaseLib.ExecuteSQL(StrQuery);

                    frmMain.RaiseEvents("Load No[" + AclMember[0].LoadNo + "]" + " Batch No[" + AclMember[0].BatchNo + "]" + " end batch by LAS.");

                    AclMember[0].BatchStepProcess = _BatchStepProcess.bspStop;
                    stpBatch = 1;
                }

            }
            else
            {
                frmMain.RaiseEvents("Load No[" + AclMember[0].LoadNo + "]" + " Batch No[" + AclMember[0].BatchNo + "]" + " stop load[unauthorize].");
                AclMember[0].BatchStepProcess = _BatchStepProcess.bspBCActive;

            }

        }

        private void Loading_Loading()
        {
            string vCmd;

            GetLoadNo();
            // PullEnquireStatus();

            Console.WriteLine("Loading Loading");
      
            if (AclMember[0].AclValueNew.EQ.A1b0_Authorized && AclMember[0].AclValueNew.EQ.A2b3_TransactionInProgress && !AclMember[0].AclValueNew.EQ.A2b1_BatchDone)
            {
              
                if (!AclMember[0].AclValueNew.EQ.A1b1_Flowing )
                {

                    if (AclMember[0].CancelLoad)
                    {
                        batchStatus = 3;
                        string StrQuery = string.Format("update loadinglines set  status = {0}, UpdatedAt = CURRENT_TIMESTAMP WHERE BatchNo = {1}", batchStatus, Batch_no);
                        DatabaseLib.ExecuteSQL(StrQuery);
                        Console.WriteLine("64");

                        frmMain.RaiseEvents("Load No[" + AclMember[0].LoadNo + "]" + " Batch No[" + AclMember[0].BatchNo + "]" + " stop load[permissive].");

                        AclMember[0].BatchStepProcess = _BatchStepProcess.bspStop;

                    }

                    if (AclMember[0].StopBatch)
                    {
                        batchStatus = 3;
                        string StrQuery = string.Format("update loadinglines set  status = {0}, UpdatedAt = CURRENT_TIMESTAMP WHERE BatchNo = {1}", batchStatus, Batch_no);
                        DatabaseLib.ExecuteSQL(StrQuery);
                        Console.WriteLine("63");

                        frmMain.RaiseEvents("Load No[" + AclMember[0].LoadNo + "]" + " Batch No[" + AclMember[0].BatchNo + "]" + " stop load by LAS.");

                        AclMember[0].BatchStepProcess = _BatchStepProcess.bspStop;
                        stpBatch = 1;

                    }

                    if (AclMember[0].AclValueNew.EQ.A3b3_AlarmOn)
                    {
                        frmMain.RaiseEvents("Load No[" + AclMember[0].LoadNo + "]" + " Batch No[" + AclMember[0].BatchNo + "]" + " stop load[alarm on].");

                        AclMember[0].BatchStepProcess = _BatchStepProcess.bspStop;

                        Console.WriteLine("62");
                        batchStatus = 3;
                        string StrQuery = string.Format("update loadinglines set  status = {0}, UpdatedAt = CURRENT_TIMESTAMP WHERE BatchNo = {1}", batchStatus, Batch_no);
                        DatabaseLib.ExecuteSQL(StrQuery);
                        Reset = false;
                    }

                    if (!AclMember[0].AclValueNew.EQ.A1b2_Release)
                    {
                        frmMain.RaiseEvents("Load No[" + AclMember[0].LoadNo + "]" + " Batch No[" + AclMember[0].BatchNo + "]" + " stop load[release].");

                        AclMember[0].BatchStepProcess = _BatchStepProcess.bspStop;

                        Console.WriteLine("61");
                        batchStatus = 3;
                        string StrQuery = string.Format("update loadinglines set  status = {0}, UpdatedAt = CURRENT_TIMESTAMP WHERE BatchNo = {1}", batchStatus, Batch_no);
                        DatabaseLib.ExecuteSQL(StrQuery);

                    }
                }
                else
                {
                    if (prepare)
                    {
                        frmMain.RaiseEvents("Load No[" + AclMember[0].LoadNo + "]" + " Batch No[" + AclMember[0].BatchNo + "]" + " continue loading.");
                        prepare = false;
                    }

                    if (AclMember[0].StopBatch)
                    {
                        batchStatus = 3;
                        string StrQuery = string.Format("update loadinglines set  status = {0}, UpdatedAt = CURRENT_TIMESTAMP WHERE BatchNo = {1}", batchStatus, Batch_no);
                        DatabaseLib.ExecuteSQL(StrQuery);

                        frmMain.RaiseEvents("Load No[" + AclMember[0].LoadNo + "]" + " Batch No[" + AclMember[0].BatchNo + "]" + " stop load by LAS.");

                        AclMember[0].BatchStepProcess = _BatchStepProcess.bspStop;
                        stpBatch = 1;

                    }

                }

            }

            if (AclMember[0].AclValueNew.EQ.A2b1_BatchDone)
            {
                batchStatus = 3;
                string StrQuery = string.Format("update loadinglines set  status = {0}, UpdatedAt = CURRENT_TIMESTAMP WHERE BatchNo = {1}", batchStatus, Batch_no);
                DatabaseLib.ExecuteSQL(StrQuery);
                tstDone = true;

                frmMain.RaiseEvents("Load No[" + AclMember[0].LoadNo + "]" + " Batch No[" + AclMember[0].BatchNo + "]" + " stop load[batch done].");

                AclMember[0].BatchStepProcess = _BatchStepProcess.bspStop;
                frmMain.End_Click();
            }

        }

        private void Loading_Stop()
        {
            string vCmd;

            GetLoadNo();
            // PullEnquireStatus();
           
            Console.WriteLine("Loading Stop");
            if (!AclMember[0].AclValueNew.EQ.A3b3_AlarmOn && !AclMember[0].AclValueNew.EQ.A2b1_BatchDone && !AclMember[0].AclValueNew.EQ.A2b2_TransactionDone && !Reset)
            {
                frmMain.RaiseEvents("Load No[" + AclMember[0].LoadNo + "]" + " Batch No[" + AclMember[0].BatchNo + "]" + "alarm reset");
                Reset = true;
            }
            
            if (!AclMember[0].AclValueNew.EQ.A3b3_AlarmOn && !AclMember[0].StopBatch && !OnLoad )
            {
                batchStatus = 2;
                string StrQuery = string.Format("update loadinglines set  status = {0}, UpdatedAt = CURRENT_TIMESTAMP WHERE BatchNo = {1}", batchStatus, Batch_no);
                DatabaseLib.ExecuteSQL(StrQuery);

                frmMain.RaiseEvents("Load No[" + AclMember[0].LoadNo + "]" + " Batch No[" + AclMember[0].BatchNo + "]" + " prepare start load");
                Console.WriteLine("Loading Stop1111");
                AclMember[0].BatchStepProcess = _BatchStepProcess.bspStart;
               // prepare = true;
            }

            if (!AclMember[0].StopBatch && !tstDone && !AclMember[0].AclValueNew.EQ.A3b3_AlarmOn && !AclMember[0].AclValueNew.EQ.A2b2_TransactionDone && !AclMember[0].CancelLoad && AclMember[0].AclValueNew.EQ.A1b2_Release && OnLoad )
            {
                batchStatus = 2;
                string StrQuery = string.Format("update loadinglines set  status = {0}, UpdatedAt = CURRENT_TIMESTAMP WHERE BatchNo = {1}", batchStatus, Batch_no);
                DatabaseLib.ExecuteSQL(StrQuery);

                frmMain.RaiseEvents("Load No[" + AclMember[0].LoadNo + "]" + " Batch No[" + AclMember[0].BatchNo + "]" + " prepare loading.");
                Console.WriteLine("Loading Stop22222");
                AclMember[0].BatchStepProcess = _BatchStepProcess.bspLoading;
                Console.WriteLine("6");
                prepare = true;
                
            }

            if (AclMember[0].AclValueNew.EQ.A2b1_BatchDone && AclMember[0].CancelLoad && AclMember[0].AclValueNew.EQ.A2b2_TransactionDone)
            {
                batchStatus = 4;
                string StrQuery = string.Format("update loadinglines set  status = {0},  UpdatedAt = CURRENT_TIMESTAMP WHERE BatchNo = {1}", batchStatus, Batch_no);
                DatabaseLib.ExecuteSQL(StrQuery);

                frmMain.RaiseEvents("Load No[" + AclMember[0].LoadNo + "]" + " Batch No[" + AclMember[0].BatchNo + "]" + " batch stop[cancel batch].");

                AclMember[0].BatchStepProcess = _BatchStepProcess.bspBCActive;
                AclMember[0].CancelLoad = false;
                AclMember[0].StopBatch = false;
                stpBatch = 1;
                cnlBatch = 1;
                frmMain.Enable_Click();
                
            }

            else if (AclMember[0].AclValueNew.EQ.A2b2_TransactionDone && !AclMember[0].CancelLoad)
            {
                batchStatus = 5;
                string StrQuery = string.Format("update loadinglines set  status = {0}, UpdatedAt = CURRENT_TIMESTAMP WHERE BatchNo = {1}", batchStatus, Batch_no);
                DatabaseLib.ExecuteSQL(StrQuery);

                frmMain.RaiseEvents("Load No[" + AclMember[0].LoadNo + "]" + " Batch No[" + AclMember[0].BatchNo + "]" + " batch done.");

                frmMain.RaiseEvents("Load No[" + AclMember[0].LoadNo + "]" + " Batch No[" + AclMember[0].BatchNo + "]" + " stop TOTGV = " + AclMember_Lib[0].AclValueNew.MeterValue[0].TotalizerGV +  
                                                                ", stop TOTGST = " + AclMember_Lib[0].AclValueNew.MeterValue[0].TotalizerGST +
                                                                ", stop TOTGSV = " + AclMember_Lib[0].AclValueNew.MeterValue[0].TotalizerGSV);
                frmMain.RaiseEvents("Load No[" + AclMember[0].LoadNo + "]" + " loaded GV = " + AclMember_Lib[0].AclValueNew.MeterValue[0].DeliveryGV +
                                                        ", loaded GST = " + AclMember_Lib[0].AclValueNew.MeterValue[0].DeliveryGST +
                                                        ", loaded GSV = " + AclMember_Lib[0].AclValueNew.MeterValue[0].DeliveryGSV);

                AclMember[0].BatchStepProcess = _BatchStepProcess.bspBCActive;
                frmMain.Enable_Click();
            }
        }

        public void PullEnquireStatus()
        {

            AclMember[0].AclValueNew = new AcculoadLib._AcculoadValue();
            string vCmd = AcculoadLib.RequestEnquireStatus(14);
            string vData = ClientLib.SendData(vCmd);
            AcculoadLib.DecodedEnquireStatus(ref AclMember[0].AclValueNew.EQ, vData);

        }

        public void GetLoadNo()
        {
            string sql = @"SELECT LoadNo FROM loadinglines where BatchNo = " + Batch_no;
            string result = string.Empty;
            DataTable dt = DatabaseLib.Excute_DataAdapter(sql);
            result = dt.Rows[0]["LoadNo"].ToString();
            AclMember[0].LoadNo = Convert.ToDouble(result);
            AclMember[0].BatchNo = Convert.ToDouble(Batch_no);    
        }

        public string TxRxAccuload(int p, string pCommand)
        {
            string vRecv = "";
            vRecv = ClientLib.SendData(pCommand);

            if (CheckMessageReceive(vRecv, p))
            {
                vRecv = vRecv.Substring(STX_Position, ETX_Position - STX_Position + 3);

            }

            return vRecv;
        }

        private bool CheckMessageReceive(string pRecv, int pAddress)
        {
            try
            {
                if (!CheckFirstBlockRecv(pRecv, pAddress))
                    return false;
                if (!CheckEndBlockRecv(pRecv))
                    return false;
            }
            catch (Exception ex)
            {
                frmMain.RaiseEventsErr(ex.ToString());
                return false;
            }
            return true;

        }

        private bool CheckFirstBlockRecv(string pRecv, int pAddrress)
        {
            bool bCheck = false;
            int CheckPos;
            pRecv.Trim();

            if (pRecv.Length >= 5)
            {
                STX_Position = pRecv.IndexOf(char.ConvertFromUtf32(STX));
                CheckPos = pRecv.IndexOf(char.ConvertFromUtf32(STX) + pAddrress.ToString("D2"));
                if (CheckPos >= 0)
                {
                    DATA_Position = CheckPos + 3;
                    bCheck = true;
                }
            }
            return bCheck;
        }

        private bool CheckEndBlockRecv(string pRecv)
        {
            bool bCheck = false;
            string vPad;

            pRecv.Trim();
            if (pRecv.Length >= 5)
            {
                ETX_Position = pRecv.IndexOf(char.ConvertFromUtf32(ETX));
                if (STX_Position > ETX_Position)
                {
                    ETX_Position = pRecv.IndexOf(char.ConvertFromUtf32(ETX), STX_Position);
                }
                vPad = pRecv.Substring(ETX_Position + 2, 1);
                if ((ETX_Position > 0) && (char.ConvertFromUtf32(PAD) == vPad))
                {
                    bCheck = true;
                }
            }
            return bCheck;
        }
        private void UpdateBatchValue()
        {
            int i = 0, j = 0;
           
            string StrQuery = string.Format("update backuplines set CurrentPreset = {1}, CurrentFlowrate = {2}, LoadedGV = {3}, LoadedGST = {4}, LoadedGSV = {5}, TotalizerGV = {6}, TotalizerGST = {7}, TotalizerGSV = {8} WHERE BatchNo = {0}", 
            Batch_no, AclMember_Lib[0].AclValueNew.MeterValue[j].CurrentPreset, AclMember_Lib[0].AclValueNew.MeterValue[j].CurrentFlowrate, AclMember_Lib[0].AclValueNew.MeterValue[j].DeliveryGV, AclMember_Lib[0].AclValueNew.MeterValue[j].DeliveryGST,
            AclMember_Lib[0].AclValueNew.MeterValue[j].DeliveryGSV, AclMember_Lib[0].AclValueNew.MeterValue[j].TotalizerGV, AclMember_Lib[0].AclValueNew.MeterValue[j].TotalizerGST, AclMember_Lib[0].AclValueNew.MeterValue[j].TotalizerGSV);
            DatabaseLib.ExecuteSQL(StrQuery);
           // Console.WriteLine(AclMember_Lib[0].AclValueNew.MeterValue[j].DeliveryGV);
            
            frmMain.Show_TextBox();
        }
    }
}
