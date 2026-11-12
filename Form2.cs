using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Casual_Instructor_Hours_Tracker
{
    public partial class Form2 : Form
    {
        private string dbPath="Data Source=C:\\rdb lab\\SQLite\\KatiaEBP.db"; // Ensure it has a proper extension
        public Form2()
        {
            InitializeComponent();
        }
        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            string inputPassword = txtpassword.Text.Trim();
            string inputusername = txtusername.Text.Trim();

            if (string.IsNullOrEmpty(inputPassword) || string.IsNullOrEmpty(inputusername))
            {
                MessageBox.Show("Please enter username and password.");
                return;
            }

            string hashedInput = HashPassword(inputPassword);

            using (var con = new SQLiteConnection(dbPath))
            {
                con.Open();

                string query = "SELECT passwordhash FROM Users WHERE username = @username LIMIT 1";
                using (var cmd = new SQLiteCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@username", inputusername);

                    var result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        string storedHash = result.ToString().Trim();

                        if (hashedInput == storedHash)
                        {
                            this.Hide();
                            Form1 mainForm = new Form1();
                            mainForm.ShowDialog();
                            this.Close();
                            MessageBox.Show("Login successfully.");
                        }
                        else
                        {
                            MessageBox.Show("Wrong password. Please try again.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Username not found.");
                    }
                }
            }
        }

        
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
