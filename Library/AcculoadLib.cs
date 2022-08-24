using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Library
{
    public class AcculoadLib
    {
        const byte STX = 2;
        const byte ETX = 3;
        const byte PAD = 127;

        static int STX_Position;
        static int ETX_Position;
        static int DATA_Position;

       
        public  struct  _EnquireStatus
        {
            public string StringStatus;

            public bool A1b3_ProgramMode;
            public bool A1b2_Release;
            public bool A1b1_Flowing;
            public bool A1b0_Authorized;

            public bool A2b3_TransactionInProgress;
            public bool A2b2_TransactionDone;
            public bool A2b1_BatchDone;
            public bool A2b0_KeypadDataPending;

            public bool A3b3_AlarmOn;
            public bool A3b2_StandbyTransactionExist;
            public bool A3b1_StorageFull;
            public bool A3b0_InStandbyMode;

            public bool A4b3_ProgramValueChange;
            public bool A4b2_DelayPrompt;
            public bool A4b1_DisplayMessageTimeout;
            public bool A4b0_PowerFailOccurred;

            public bool A5b3_CheckingEntries;
            public bool A5b2_Input1;
            public bool A5b1_Input2;
            public bool A5b0_Input3;

            public bool A6b3_Input4;
            public bool A6b2_Input5;
            public bool A6b1_Input6;
            public bool A6b0_Input7;

            public bool A7b3_Input8;
            public bool A7b2_Input9;
            public bool A7b1_Input10;
            public bool A7b0_Input11;

            public bool A16b3_PrintingInProgress;
            public bool A16b2_PermissiveDelay;
            public bool A16b1_CardDataPresent;
            public bool A16b0_PresetInProgress;
        }
        public struct _AcculoadValue
        {
            //public _EnquireAlarmSystem EA_System;
            //public _EnquireAlarmArm EA_Arm;
            public _EnquireStatus EQ;
            public bool AlarmActive;
            public double AvgVCF;
            public double AvgKFactor;
            public double AvgMeterFactor;
            public double AvgTemp;

            public _MeterValue[] MeterValue;
            public _MeterValueCharacter[] MeterValueCharacter;

            public double[] AnalogValue;
            public DateTime BatchCurrentValueUpdatedDate;
            public DateTime BatchStatusUpdateDate;

        }
        public struct AcculoadMember
        {
            //public AcculoadLib AclLib;
            public AcculoadLib AclLib;
            public AcculoadLib._AcculoadValue AclValueNew;
            public AcculoadLib._AcculoadValue AclValueOld;
            //public AcculoadLib._AcculoadValueCharacter AclValueCharter;
            public string MeterNo;
            public int Address;
            ////public _BatchCmdFnc BatchCommand;
            ////public AcculoadLib._Transaction Transaction;
            ////public AcculoadLib._TransactionBatchValue TransactionBatchValue;
            ////public AcculoadLib._TransactionBatchValueCharacter TransactionBatchValueCharacter;

            public bool IsConnect;
            public Int64 BatchID;
            public string BatchName;
            public Int64 MeterID;
            public string MeterName;
            //public int ProductNo;
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
            //public _StepProcess StepProcess;
            //public _BCSTEPPROCESS mStepBay;
            public string MsgSend;
            public string MsgRecv;
            public int IntervalUpdate;
            public DateTime LastUpdated;

            public double StartTotalizerGV;
            public double StartTotalizerGST;
            public double StartTotalizerGSV;
            public double StartTotalizerMass;

            public int OldLoadActive;

            public int LastBatching;
            public int AuthorizeBayNo;
            //char 	BC_AuthorizeBayName[20];
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
            public double LineNo;
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
        public struct _MeterValueCharacter
        {
            public string MeterName;
            public int ProductNo;

            public string EAProductCharacter;
            public string EAMeterCharacter;

            public string CurrentTempCharacter;
            public string CurrentPressureCharacter;
            public string CurrentDensityCharacter;
            public string CurrentFlowrateCharacter;
            public string CurrentPresetCharacter;
            public string RefDensityCharacter;

            public string FlowrateGVCharacter;
            public string FlowrateGSVCharacter;
            public string FlowrateGSTCharacter;
            public string FlowrateMassCharacter;

            public string LoadAvgTempCharacter;
            public string LoadAvgPressureCharacter;
            public string LoadAvgDensityCharacter;
            public string LoadAvgMeterFactorCharacter;
            public string LoadAvgFlowrateCharacter;

            public string AvgVCFCharacter;
            public string AvgKFactorCharacter;
            public string AvgMeterCharacter;

            public string DeliveryIVCharacter;
            public string DeliveryGVCharacter;
            public string DeliveryGSVCharacter;
            public string DeliveryGSTCharacter;
            public string DeliveryGST30Character;
            public string DeliveryMASSCharacter;
            public string DeliveryMASSAirCharacter;

            public string TotalizerIVCharacter;
            public string TotalizerGVCharacter;
            public string TotalizerGSVCharacter;
            public string TotalizerGSTCharacter;
            public string TotalizerMASSCharacter;
        }

        public struct _MeterValue
        {
            //public string MeterNo;
            public string MeterName;
            public int ProductNo;
            public string BayNo;
            public string IslandNo;
            //public _EnquireAlarmProduct EA_Product;
            //public _EnquireAlarmMeter EA_Meter;

            public string CurrentTemp;
            public string CurrentPressure;
            public double CurrentDensity;
            public double CurrentFlowrate;
            public double CurrentPreset;
            public double RefDensity;

            public double LoadAvgTemp;
            public double LoadAvgPressure;
            public double LoadAvgDensity;
            public double LoadAvgMeterFactor;
            public double LoadAvgFlowrate;

            public double FlowrateGV;
            public double FlowrateGSV;
            public double FlowrateGST;
            public double FlowrateMass;

            public double DeliveryIV;
            public double DeliveryGV;
            public double DeliveryGSV;
            public double DeliveryGST;
            public double DeliveryGST30;
            public double DeliveryMASS;
            public double DeliveryMASSAir;

            public double TotalizerIV;
            public double TotalizerGV;
            public double TotalizerGSV;
            public double TotalizerGST;
            public double TotalizerMASS;
        }
        public static string RequestEnquireStatus(int pMeterAddr)
        {
            string result = BuildMessage(pMeterAddr, "EQ");
            return result;
        }

        private static string BuildMessage(int pMeterAddr, string pMessage)
        {

            string vLRC = CalLRC(pMeterAddr.ToString("D2") + pMessage + char.ConvertFromUtf32(ETX));
            string s = char.ConvertFromUtf32(STX) + pMeterAddr.ToString("D2") + pMessage + char.ConvertFromUtf32(ETX) + vLRC;

            return s;
        }
        private static string CalLRC(string pMsg)
        {
            byte[] b = Encoding.ASCII.GetBytes(pMsg);
            int LRC = 0;

            for (int i = 0; i < b.Length; i++)
            {
                LRC = LRC ^ b[i];
            }
            return char.ConvertFromUtf32(LRC);
        }
        public static bool DecodedEnquireStatus(ref _EnquireStatus pValue, string pDecodeData)
        {
            string vRecv = "";
            bool bRet = false;

            try
            {
                if (pDecodeData == null)
                    return bRet;

                bRet = CheckMessageReceive(pDecodeData);
                //vRecv = pDecodeData;
                vRecv = pDecodeData.Substring(DATA_Position, ETX_Position - DATA_Position);
                if (!bRet)
                {
                    pValue.StringStatus = "0000000000000000";
                    return bRet;
                }

                //if (vRecv.Length < 16)
                //    vRecv.PadRight(16, '0');
                pValue.StringStatus = vRecv;
                for (int vIndex = 0; vIndex < vRecv.Length; vIndex++)
                {
                    switch (vIndex)
                    {
                        case 0:
                            DecodeCharacterToStatus(Convert.ToChar(vRecv.Substring(vIndex, 1))
                                                    , ref pValue.A1b3_ProgramMode, ref pValue.A1b2_Release
                                                    , ref pValue.A1b1_Flowing, ref pValue.A1b0_Authorized);
                            break;
                        case 1:
                            DecodeCharacterToStatus(Convert.ToChar(vRecv.Substring(vIndex, 1))
                                                    , ref pValue.A2b3_TransactionInProgress, ref pValue.A2b2_TransactionDone
                                                    , ref pValue.A2b1_BatchDone, ref pValue.A2b0_KeypadDataPending);
                            break;
                        case 2:
                            DecodeCharacterToStatus(Convert.ToChar(vRecv.Substring(vIndex, 1))
                                                    , ref pValue.A3b3_AlarmOn, ref pValue.A3b2_StandbyTransactionExist
                                                    , ref pValue.A3b1_StorageFull, ref pValue.A3b0_InStandbyMode);
                            break;
                        case 3:
                            DecodeCharacterToStatus(Convert.ToChar(vRecv.Substring(vIndex, 1))
                                                    , ref pValue.A4b3_ProgramValueChange, ref pValue.A4b2_DelayPrompt
                                                    , ref pValue.A4b1_DisplayMessageTimeout, ref pValue.A4b0_PowerFailOccurred);
                            break;
                        case 4:
                            DecodeCharacterToStatus(Convert.ToChar(vRecv.Substring(vIndex, 1))
                                                    , ref pValue.A5b3_CheckingEntries, ref pValue.A5b2_Input1
                                                    , ref pValue.A5b1_Input2, ref pValue.A5b0_Input3);
                            break;
                        case 5:
                            DecodeCharacterToStatus(Convert.ToChar(vRecv.Substring(vIndex, 1))
                                                    , ref pValue.A6b3_Input4, ref pValue.A6b2_Input5
                                                    , ref pValue.A6b1_Input6, ref pValue.A6b0_Input7);
                            break;
                        case 6:
                            DecodeCharacterToStatus(Convert.ToChar(vRecv.Substring(vIndex, 1))
                                                    , ref pValue.A7b3_Input8, ref pValue.A7b2_Input9
                                                    , ref pValue.A7b1_Input10, ref pValue.A7b0_Input11);
                            break;
                        case 15:
                            DecodeCharacterToStatus(Convert.ToChar(vRecv.Substring(vIndex, 1))
                                                    , ref pValue.A16b3_PrintingInProgress, ref pValue.A16b2_PermissiveDelay
                                                    , ref pValue.A16b1_CardDataPresent, ref pValue.A16b0_PresetInProgress);
                            break;
                    }
                }
                //pValue = mAcculoadValue.EQ;
                return bRet;
            }
            catch (Exception exp)
            { return false; }
        }
        private  static void DecodeCharacterToStatus(char pValue, ref bool pBit3, ref bool pBit2, ref bool pBit1, ref bool pBit0)
        {
            switch (pValue)
            {
                case '0':
                    pBit0 = false;
                    pBit1 = false;
                    pBit2 = false;
                    pBit3 = false;
                    break;
                case '1':
                    pBit0 = true;
                    pBit1 = false;
                    pBit2 = false;
                    pBit3 = false;
                    break;
                case '2':
                    pBit0 = false;
                    pBit1 = true;
                    pBit2 = false;
                    pBit3 = false;
                    break;
                case '3':
                    pBit0 = true;
                    pBit1 = true;
                    pBit2 = false;
                    pBit3 = false;
                    break;
                case '4':
                    pBit0 = false;
                    pBit1 = false;
                    pBit2 = true;
                    pBit3 = false;
                    break;
                case '5':
                    pBit0 = true;
                    pBit1 = false;
                    pBit2 = true;
                    pBit3 = false;
                    break;
                case '6':
                    pBit0 = false;
                    pBit1 = true;
                    pBit2 = true;
                    pBit3 = false;
                    break;
                case '7':
                    pBit0 = true;
                    pBit1 = true;
                    pBit2 = true;
                    pBit3 = false;
                    break;
                case '8':
                    pBit0 = false;
                    pBit1 = false;
                    pBit2 = false;
                    pBit3 = true;
                    break;
                case '9':
                    pBit0 = true;
                    pBit1 = false;
                    pBit2 = false;
                    pBit3 = true;
                    break;
                case ':':
                    pBit0 = false;
                    pBit1 = true;
                    pBit2 = false;
                    pBit3 = true;
                    break;
                case ';':
                    pBit0 = true;
                    pBit1 = true;
                    pBit2 = false;
                    pBit3 = true;
                    break;
                case '<':
                    pBit0 = false;
                    pBit1 = false;
                    pBit2 = true;
                    pBit3 = true;
                    break;
                case '=':
                    pBit0 = true;
                    pBit1 = false;
                    pBit2 = false;
                    pBit3 = true;
                    break;
                case '>':
                    pBit0 = false;
                    pBit1 = true;
                    pBit2 = true;
                    pBit3 = true;
                    break;
                case '?':
                    pBit0 = true;
                    pBit1 = true;
                    pBit2 = true;
                    pBit3 = true;
                    break;
                default:
                    pBit0 = false;
                    pBit1 = false;
                    pBit2 = false;
                    pBit3 = false;
                    break;
            }
        }
        private static bool CheckMessageReceive(string pRecv)
        {
            if (!CheckFirstBlockRecv(pRecv))
                return false;
            if (!CheckEndBlockRecv(pRecv))
                return false;

            return true;
        }
        private static bool CheckFirstBlockRecv(string pRecv)
        {
            bool bCheck = false;
            int CheckPos;
            pRecv.Trim();

            if (pRecv.Length >= 5)
            {
                STX_Position = pRecv.IndexOf(char.ConvertFromUtf32(STX));
                CheckPos = pRecv.IndexOf(char.ConvertFromUtf32(STX) + ClientLib.getMeterAddr().ToString("D2"));
                if (CheckPos >= 0)
                {
                    DATA_Position = CheckPos + 3;
                    bCheck = true;
                }
            }
            return bCheck;
        }
        private static bool CheckEndBlockRecv(string pRecv)
        {
            bool bCheck = false;
            string vPad;

            pRecv.Trim();
            if (pRecv.Length >= 5)
            {
                ETX_Position = pRecv.IndexOf(char.ConvertFromUtf32(ETX));
                vPad = pRecv.Substring(ETX_Position + 2, 1);
                if ((ETX_Position > 0) && (char.ConvertFromUtf32(PAD) == vPad))
                {
                    bCheck = true;
                }
            }
            return bCheck;
        }

    }
}
