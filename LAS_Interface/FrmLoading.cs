using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library;
using Org.BouncyCastle.Utilities.Collections;
using System.Security.Cryptography.X509Certificates;

namespace LAS_Interface
{
    public partial class FrmLoading : Form
    {
        FrmMain frmMain;
        string strconn = "server=r98du2bxwqkq3shg.cbetxkdyhwsb.us-east-1.rds.amazonaws.com ;database=ahda1gtbqhb7pncg;uid=hktvkvjk6993txuk;pwd=ma46ffmhhxgl0zj6";
        public int frmActon = 1;
        public string load_no;

        public FrmLoading(FrmMain frmMain)
        {
            InitializeComponent();
            this.frmMain = frmMain;
        }

        public void add()
        {
            try
            {
                string tu_number1 = txt_หัว.Text.Trim();
                string tu_number2 = txt_พ่วง.Text.Trim();
                string driver_name = txt_คนขับ.Text.Trim();
                long loadno;
                string strSQL = string.Format("INSERT INTO loadingheaders (TuNumber1, TuNumber2, DriverName) VALUES('{0}', '{1}', '{2}'); ", tu_number1, tu_number2, driver_name);

                if (DatabaseLib.Execute_NonQueryResId(strSQL, out loadno))
                {
<<<<<<< HEAD
                    if (loadno > 0)
                    {
                        addLine(loadno);
                        MessageBox.Show("successfully");

                        this.Close();
                    }

                }
                else
                {
                    MessageBox.Show("error");
=======
                    addLine(loadno) ;
                    MessageBox.Show("successfully");
>>>>>>> a3f3b76d3e96581de7de81e1b14740a63c7c3119
                    this.Close();
                }
                
            }
            catch (Exception ex)
            {
                
            }
            

        }

        public void addLine(long pLoadNo)
        {

            MySqlConnection conn = new MySqlConnection(strconn);
            string tu_number1 = txt_หัว.Text.Trim();
            string tu_number2 = txt_พ่วง.Text.Trim();
            string driver_name = txt_คนขับ.Text.Trim();
            string StrQuery = string.Empty;

            for (int i = 0; i < dgvLL.Rows.Count; i++)
            {
                string compartment = dgvLL.Rows[i].Cells["compartment"].Value.ToString();
                string productName = dgvLL.Rows[i].Cells["product"].Value.ToString();
                string preset = dgvLL.Rows[i].Cells["preset"].Value.ToString();
                StrQuery = string.Format (@"INSERT INTO loadinglines (LoadNo, Compartment, ProductName, Preset) VALUES({0}, {1}, '{2}', {3});",pLoadNo,compartment,productName,preset);
                bool vCheck= DatabaseLib.ExecuteSQL(StrQuery);

            }

        }

        public void updateLine()
        {

            MySqlConnection conn = new MySqlConnection(strconn);
            string tu_number1 = txt_หัว.Text.Trim();
            string tu_number2 = txt_พ่วง.Text.Trim();
            string driver_name = txt_คนขับ.Text.Trim();
            string StrQuery = string.Empty;

            for (int i = 0; i < dgvLL.Rows.Count; i++)
            {
                string compartment = dgvLL.Rows[i].Cells["compartment"].Value.ToString();
                string productName = dgvLL.Rows[i].Cells["product"].Value.ToString();
                string preset = dgvLL.Rows[i].Cells["preset"].Value.ToString();
                StrQuery = string.Format("update loadinglines set Compartment = {0}, ProductName = '{1}', Preset = {2}, UpdatedAt = CURRENT_TIMESTAMP WHERE LoadNo = {3} ;", compartment, productName, preset, load_no );
                bool vCheck = DatabaseLib.ExecuteSQL(StrQuery);
            }

        }
        public void edit()
        {
            
            string tu_number1 = txt_หัว.Text.Trim();
            string tu_number2 = txt_พ่วง.Text.Trim();
            string driver_name = txt_คนขับ.Text.Trim();
<<<<<<< HEAD
=======
            long loadno;
            string strSQL = string.Format ("UPDATE loadingheaders SET TuNumber1 = '{0}', TuNumber2 = '{1}', DriverName =  '{2}', UpdatedAt = CURRENT_TIMESTAMP WHERE LoadNo = loadno ", tu_number1, tu_number2, driver_name);
>>>>>>> a3f3b76d3e96581de7de81e1b14740a63c7c3119
          
            string strSQL = string.Format("UPDATE loadingheaders SET TuNumber1 = '{0}', TuNumber2 = '{1}', DriverName = '{2}', UpdatedAt = CURRENT_TIMESTAMP WHERE LoadNo = {3} ;", tu_number1, tu_number2, driver_name, load_no ) ;
          
            if (DatabaseLib.ExecuteSQL (strSQL ))
            {
                    updateLine();
                    MessageBox.Show("successfully");
                    this.Close();

            }
            else
            {
                MessageBox.Show("error");
                this.Close();
            }


            //MySqlConnection conn = new MySqlConnection(strconn);
            //conn.Open();
            //MySqlCommand command = conn.CreateCommand();
            //command.Parameters.AddWithValue("@TuNumber1", txt_หัว.Text);
            //command.Parameters.AddWithValue("@TuNumber2", txt_พ่วง.Text);
            //command.Parameters.AddWithValue("@DriverName", txt_คนขับ.Text);
            //command.Parameters.AddWithValue("@LoadNo", frmMain.dataGridView1.SelectedCells[0].Value.ToString());
            //command.CommandText = "update `loadingheaders` set `TuNumber1`= @TuNumber1,`TuNumber2`= @TuNumber2,`DriverName`= @DriverName, UpdatedAt = current_timestamp() where `loadingheaders`.`LoadNo` = @LoadNo";
            //if (command.ExecuteNonQuery() > 0)
            //    MessageBox.Show("successfully");
            //else
            //    MessageBox.Show("error");
            //conn.Close();
        }

        private void txt_ช่อง_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string txtNum = txt_ช่อง.Text;
                int num = 0;
                if (!string.IsNullOrEmpty(txtNum))
                {
                    num = Convert.ToInt16(txt_ช่อง.Text);
                }

                dgvLL.Rows.Clear();
                if (num > 5)
                {
                    MessageBox.Show("จำนวนช่องเติมสูงสุด 5 ช่อง", "การแจ้งเตือน",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                    txt_ช่อง.Text = "5";
                }

                for (int i = 0; i < num; i++)
                {

                    int iRowNo = i + 1;
                    string[] row1 = new string[] { iRowNo.ToString(), "", "" };
                    dgvLL.Rows.Add(row1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
            }


        }

        private void FrmLoading_Load(object sender, EventArgs e)
        {
            if (frmActon == 2)
            {
                load_no = frmMain.dataGridView1.SelectedCells[0].Value.ToString();
                txt_หัว.Text = frmMain.dataGridView1.SelectedCells[1].Value.ToString();
                txt_พ่วง.Text = frmMain.dataGridView1.SelectedCells[2].Value.ToString();
                txt_คนขับ.Text = frmMain.dataGridView1.SelectedCells[3].Value.ToString();

                string sql = @"SELECT BatchNo, LoadNo, Compartment, ProductName, Preset, LoadindVolume, CreatedAt, UpdatedAt
                              FROM loadinglines where LoadNo = "+load_no;
                DataTable dt = new DataTable();
                dt =DatabaseLib.Excute_DataAdapter(sql);
                dgvLL.Rows.Clear();
                txt_ช่อง.Text = dt.Rows.Count.ToString();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dgvLL.Rows[i].Cells["product"].Value = dt.Rows[i]["ProductName"].ToString();
                    dgvLL.Rows[i].Cells["preset"].Value = dt.Rows[i]["Preset"].ToString();
                }
                
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("คุณต้องการบันทึกข้อมูลหรือไม่ ?","การบันทึกข้อมูล", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                if (Recievetext())
                {
                    if (frmActon == 1)
                    {
                        add();
                    }
                    else if (frmActon == 2)
                    {
                        edit();
                    }
                }
            }

        }

        private  bool Recievetext()
        {
            //Check loading header
            string tu_number1 = txt_หัว.Text;
            string driver_name = txt_คนขับ.Text;
            string numrow = txt_ช่อง.Text;
            if (string.IsNullOrEmpty(tu_number1))
            {
                MessageBox.Show("กรุณากรอก ทะเบียนหัวลาก", "การแจ้งเตือนข้อมูล", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(driver_name))
            {
                MessageBox.Show("กรุณากรอก ชื่อพนักงานขับรถ", "การแจ้งเตือนข้อมูล", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            //Check number of row
            if (string.IsNullOrEmpty(numrow))
            {
                MessageBox.Show("กรุณากรอก จำนวนช่องจ่าย", "การแจ้งเตือนข้อมูล", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            //Check loading line
            for (int i = 0; i < dgvLL.Rows.Count; i++)
            {
              int compartment = i + 1;
              string product=  dgvLL.Rows[i].Cells["product"].Value.ToString();
              string preset = dgvLL.Rows[i].Cells["preset"].Value.ToString();
                if (string.IsNullOrEmpty(product))
                {
                    MessageBox.Show("ช่องเติมที่ "+compartment + " กรุณากรอก ผลิตภัณฑ์", "การแจ้งเตือนข้อมูล", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (string.IsNullOrEmpty(preset))
                {
                    MessageBox.Show("ช่องเติมที่ " + compartment + "กรุณากรอก ปริมาณการสั่งเติม", "การแจ้งเตือนข้อมูล", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true;   
        }

        private void txt_ช่อง_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        
        public static DataRow ToDataRow(DataGridViewRow Value)
        {
            try
            {
                DataRowView dv = (DataRowView)Value.DataBoundItem;
                return dv.Row;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
