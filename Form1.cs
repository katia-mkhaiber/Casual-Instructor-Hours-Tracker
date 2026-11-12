using System;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Casual_Instructor_Hours_Tracker.Properties;
using iTextSharp.text;


namespace Casual_Instructor_Hours_Tracker
{
    public partial class Form1 : Form
    {
        private string dbFileName = "C:\\rdb lab\\SQLite\\KatiaEBP.db"; // Ensure it has a proper extension
        private string dbPath;

        public Form1()
        {
            InitializeComponent();
            numericUpDown1.Maximum = 12;

            // Initialize the connection string before using it
            dbPath = $"Data Source={Path.Combine(Application.StartupPath, dbFileName)}";

            InitializeDatabase();
            LoadEntries();

        }

        private void InitializeDatabase()
        {
            string fullPath = Path.Combine(Application.StartupPath, dbFileName);
            if (!File.Exists(fullPath))
            {
                SQLiteConnection.CreateFile(fullPath);

                using (var con = new SQLiteConnection(dbPath))
                {
                    con.Open();
                    string tableCmd = "CREATE TABLE Hours (Name TEXT, Date TEXT, Hour INTEGER)";
                    using (var cmd = new SQLiteCommand(tableCmd, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            lblTotalHours.Text = "Total Hours: 0";
        }

        private void LoadEntries()
        {
            dataGridView1.Rows.Clear();
            int totalHours = 0;
            string selectedName = comboBox1.Text.Trim();

            if (string.IsNullOrWhiteSpace(selectedName))
                return;

            using (var con = new SQLiteConnection(dbPath))
            {
                con.Open();
                string selectCmd = "SELECT Date, Hour FROM Hours WHERE Name = @Name";
                using (var cmd = new SQLiteCommand(selectCmd, con))
                {
                    cmd.Parameters.AddWithValue("@Name", selectedName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string date = reader["Date"].ToString();
                            int hours = Convert.ToInt32(reader["Hour"]);

                            dataGridView1.Rows.Add(date, hours);
                            totalHours += hours;
                        }
                    }
                }
            }

            lblTotalHours.Text = $"Total Hours: {totalHours}";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            lblTotalHours.Text = "Total Hours: 0";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEntries();
        }

        // Optional transparent panel for design
        public class TransparentPanel : Panel
        {
            public TransparentPanel()
            {
                this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
                this.BackColor = Color.Transparent;
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                // Do nothing – keeps it transparent
            }
        }
        private void btnOpenForm2_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show(); // Opens Form2
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string name = comboBox1.Text.Trim();
            DateTime date = dateTimePicker1.Value.Date;
            int hour = (int)numericUpDown1.Value;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please select instructor name.");
                return;
            }

            if (hour < 0 || hour > 12)
            {
                MessageBox.Show("Hours must be between 0 and 12.");
                return;
            }

            using (var con = new SQLiteConnection(dbPath))
            {
                con.Open();
                string insertCmd = "INSERT INTO Hours (Name, Date, Hour) VALUES (@Name, @Date, @Hour)";
                using (var cmd = new SQLiteCommand(insertCmd, con))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Date", date.ToShortDateString());
                    cmd.Parameters.AddWithValue("@Hour", hour);
                    cmd.ExecuteNonQuery();
                }
            }
            LoadEntries();
        }

        private void btnupdate_Click_1(object sender, EventArgs e)
        {
            string name = comboBox1.Text.Trim();
            DateTime date = dateTimePicker1.Value.Date;
            int hour = (int)numericUpDown1.Value;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter instructor name.");
                return;
            }
            if (hour < 0 || hour > 12)
            {
                MessageBox.Show("Hour must be between 0 and 12.");
                return;
            }

            using (var con = new SQLiteConnection(dbPath))
            {
                con.Open();
                // Update hour for record matching name and date
                string updateCmd = "UPDATE Hours SET Hour = @Hour WHERE Name = @Name AND Date = @Date";
                using (var cmd = new SQLiteCommand(updateCmd, con))
                {
                    cmd.Parameters.AddWithValue("@Hour", hour);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Date", date.ToShortDateString());
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        MessageBox.Show("No matching record found to update.");
                    }
                    else
                    {
                        MessageBox.Show("Entry updated successfully.");
                    }
                }
            }

            LoadEntries();
        }

        private void btndelete_Click_1(object sender, EventArgs e)
        {
            string name = comboBox1.Text.Trim();
            DateTime date = dateTimePicker1.Value.Date;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter name and date.");
                return;
            }
            var confirmResult = MessageBox.Show($"Are you sure to delete the entry for {name} on {date.ToShortDateString()}?",
                                     "Confirm Delete",
                                     MessageBoxButtons.YesNo);
            if (confirmResult != DialogResult.Yes)
                return;

            using (var con = new SQLiteConnection(dbPath))
            {
                con.Open();
                // Delete record matching name and date
                string deleteCmd = "DELETE FROM Hours WHERE Name = @Name AND Date = @Date";
                using (var cmd = new SQLiteCommand(deleteCmd, con))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Date", date.ToShortDateString());
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        MessageBox.Show("No matching record found to delete.");
                    }
                    else
                    {
                        MessageBox.Show("Entry deleted successfully.");
                    }
                }
            }

            LoadEntries();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(lblTotalHours.Text, "Generate Term Report");

        }
    }
  }



