using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Net.Http;

namespace Library
{
    public class ClientLib
    {
        static TcpClient tcp;
        static Socket socket;
        static Stream stm;
        private static bool isConnectAcl; // field
        static int meterAddr =14;

        public string TimeSend;
        public string TimeRecv;

        public static bool IsConnectAcl   // property
        {
            get { return isConnectAcl; }   // get method
            set { isConnectAcl = value; }  // set method
        }
        public static bool getIsConnectAcl()
        {
            return IsConnectAcl;
        }
        public static int MeterAddress
        {
            get { return meterAddr; }  
            set { meterAddr = value; }  
        }

        public static int getMeterAddr()
        {
            return meterAddr;
        }
        public static bool ConnectAcl()
        {
            try
            {
                
                string uri = "ktd-devth.ddns.net";
                string ipAddress = "192.168.0.114";
               // var addresses = Dns.GetHostAddresses(uri);
                //remoteEP = new IPEndPoint(addresses[0], 7734);
                //remoteEP = new IPEndPoint(IPAddress.Parse(ipAddress), 7734);
                // Create a TCP/IP socket.
                //socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //socket.ReceiveTimeout = 2000;
                //socket.SendTimeout = 1000;

                tcp = new TcpClient(ipAddress, 7734);
                //tcp = new TcpClient(addresses[0].ToString(), 7734);
                //tc = new TcpClient("192.168.1.193", 7734);
                tcp.SendTimeout = 1000;
                stm = tcp.GetStream();
                IsConnectedAcl();
                return true;
                tcp.GetStream().Close();
                tcp.Close();

            }
            catch (Exception ex)
            {
                isConnectAcl = false;
                return false;
            }
        }


        public static bool IsConnectedAcl()
        {
           
            try
            {
                if ( tcp != null && tcp.Client != null &&  tcp.Connected)
                {
                    /* pear to the documentation on Poll:
                     * When passing SelectMode.SelectRead as a parameter to the Poll method it will return 
                     * -either- true if Socket.Listen(Int32) has been called and a connection is pending;
                     * -or- true if data is available for reading; 
                     * -or- true if the connection has been closed, reset, or terminated; 
                     * otherwise, returns false
                     */

                    // Detect if client disconnected
                     if ( tcp.Client.Poll(0, SelectMode.SelectRead))
                     {
                         byte[] buff = new byte[1];
                         if (tcp.Client.Receive(buff, SocketFlags.Peek) == 0)
                         {
                            // Client disconnected
                            isConnectAcl = false;
                            return false;
                            
                         }
                         else
                         {
                            isConnectAcl = true;
                            return true;
                         
                         }
                     }
                    isConnectAcl = true;
                    return true;
                    
                }
                else
                {
                    ConnectAcl();
                    isConnectAcl = false;
                    return false;
                    
                } 

            }
            catch
            {
                isConnectAcl = false;
                return false;
            }
            
        }

        public static void DisconnectAcl()
        {
            tcp.GetStream().Close();
            tcp.Close();
            
        }

        public static string SendData(string pMessage)
        {
            string vRecv = string.Empty;
            try
            {
                if (getIsConnectAcl())
                {
                    int byteCount = Encoding.ASCII.GetByteCount(pMessage);
                    //Console.WriteLine(byteCount);
                    byte[] sendData = new byte[byteCount];
                    sendData = Encoding.ASCII.GetBytes(pMessage);
                    //Console.WriteLine(sendData);
                    stm.Write(sendData, 0, byteCount);
                    vRecv = ReceiveData();
                    //Console.WriteLine(vRecv);
                }
            }
            catch (Exception ex)
            {
                vRecv = ex.Message;
            }

            return vRecv;
        }

        public static string ReceiveData()
        {
             string vRecv = string.Empty;
             byte[] mRecbyte = new byte[512];
             byte[] mSenbyte = new byte[512];
            if (getIsConnectAcl())
            {
                stm.Read(mRecbyte, 0, mRecbyte.Length);
                vRecv = ASCIIEncoding.UTF8.GetString(mRecbyte);
            }
            
            return vRecv;
        }
        

    }
   
}
