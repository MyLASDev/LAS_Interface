using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LAS_Interface
{
    public partial class FrmPreset : Form
    {
        
        FrmMain FrmMain;

        string strconn = "server=localhost;database=las_database;uid=root;pwd=";
        public FrmPreset(FrmMain frmMain)
        {
            InitializeComponent();
            this.FrmMain = frmMain;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(strconn);
            conn.Open();
            MySqlCommand command = conn.CreateCommand();
            command.Parameters.AddWithValue("Preset(Litre)", txtPreset.Text);
            command.CommandText = "insert into Las_data (Preset(Litre)) values (@Preset(Litre))";
            conn.Close();
            this.Close();
        }
    }
}
