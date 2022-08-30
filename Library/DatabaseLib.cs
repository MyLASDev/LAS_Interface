using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Library
{
    public class DatabaseLib
    {
        static string strconn = "server=r98du2bxwqkq3shg.cbetxkdyhwsb.us-east-1.rds.amazonaws.com ;database=ahda1gtbqhb7pncg;uid=hktvkvjk6993txuk;pwd=ma46ffmhhxgl0zj6";
        public static bool ExecuteSQL(string pStrSQL)
        {
            bool vCheck = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(strconn))
                {
                    MySqlCommand command = new MySqlCommand(pStrSQL, connection);
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                    vCheck = true;
                }
            }
            catch (Exception)
            {
                vCheck = false;
            }


            return vCheck;
        }
    }
}
