using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAS_Interface
{
    public class CLogfiles
    {

        string dirLog = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Log\\";//Directory.GetCurrentDirectory() + "\\Log\\"
        
        public void CreateFolderLog()
        {
            if (!Directory.Exists(dirLog))
            {
                Directory.CreateDirectory(dirLog);
            }
        } 

        public void WriteLog(string pFileName, string pMsg)
        {
            

            string vFlieName = "Log" + pFileName + "_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
            string vHeaderText = "<" + pFileName + "><";

            CreateFolderLog();
            using (System.IO.StreamWriter pLogFile = new StreamWriter(dirLog + vFlieName, true))
            {
                pLogFile.WriteLine(vHeaderText + pMsg);
                pLogFile.Dispose();
            }
        }

        public void WriteErrLog(string pMsg)
        {
            string vFlieName = "Err_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
            string vHeaderText = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "> ";

            CreateFolderLog();
            using (System.IO.StreamWriter pLogFile = new StreamWriter(dirLog + vFlieName, true))
            {
                pLogFile.WriteLine(vHeaderText + pMsg);
                pLogFile.Dispose();
            }
           
        }

    }
}
