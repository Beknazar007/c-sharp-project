using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace PersonalAccountantApp
{
    public partial class Form1 : Form
    {
        NpgsqlConnection conn;
        String tableInit = "CREATE TABLE IF NOT EXISTS expenses(id serial, name varchar(255), date varchar(255), amount bigint);";

        public Form1()
        {
            InitializeComponent();
            if (conn == null)
            {
                conn = new NpgsqlConnection("Server=localhost;Port=5432;Database=project;User Id=postgres;Password=1234");
                conn.Open();
            }
            initTable();
            refreshTable();
        }

        private void initTable()
        {
            NpgsqlCommand command = conn.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = tableInit;
            command.ExecuteNonQuery();
        }

        private void refreshTable()
        {
            String readAll = "SELECT * FROM expenses;";
            NpgsqlCommand command = conn.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = readAll;
            NpgsqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                DataTable dt = new DataTable();
                dt.Load(reader);
                dataGridView1.DataSource = dt;
            }
            reader.Close();

            String readTotalAmount = "SELECT SUM(amount) FROM expenses;";
            command.CommandText = readTotalAmount;
            String total = "";
            try {
                total = command.ExecuteScalar().ToString().Trim();
            } catch (Exception ex) {
            }
            if (String.Equals("", total))
            {
                total = "0 soms";
            }
            else 
            {
                total += " soms";
            }
                
            label3.Text = "Total amount: " + total;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String name = textBox1.Text.Trim();
            int amount;
            try {
                amount = Convert.ToInt32(textBox2.Text.Trim());
                if (amount == null) {
                    throw new Exception();
                }
                String addExp = String.Format("INSERT INTO expenses (name, date, amount) values ('{0}', '{1}', {2})",
                    name, DateTime.Now.ToString(), amount);
                NpgsqlCommand command = conn.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = addExp;
                command.ExecuteNonQuery();
                refreshTable();
            } catch (Exception ex){
                MessageBox.Show("Given amount is invalid.", "Input error!");
            }
            
        }
    }
}
