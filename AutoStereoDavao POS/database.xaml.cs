using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutoStereoDavao_POS
{
    /// <summary>
    /// Interaction logic for database.xaml
    /// </summary>
    public partial class database : Window
    {
        public database()
        {
            InitializeComponent();
        }

        private void save_newProduct(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.DefaultExt = "sql";

            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                try
                {
                    string constring = "server=localhost;user=root;pwd=;database=AutoStereoDavao;SslMode=none;";

                    using (MySqlConnection conn = new MySqlConnection(constring))
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            using (MySqlBackup mb = new MySqlBackup(cmd))
                            {
                                cmd.Connection = conn;
                                conn.Open();
                                mb.ExportToFile(saveFileDialog1.FileName);
                                conn.Close();
                            }
                        }
                    }

                }
                catch (Exception E) { System.Windows.MessageBox.Show(E.Message); return; }
                System.Windows.MessageBox.Show("Back-up Data Successfully created!");

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
         
           try
                {
                    restore();
                }
                catch (Exception E) 
            
           { 
                    System.Windows.MessageBox.Show(E.Message);
                    String connectionString = "Data Source =localhost;  username=root;password=;";
                    MySqlConnection connect = new MySqlConnection(connectionString);
                    MySqlCommand command = new MySqlCommand("CREATE DATABASE AutoStereoDavao;", connect);
                    connect.Open();
                    command.ExecuteNonQuery();
                    connect.Close();
                    restore();
                   
           }
              
            
        }
        private void restore()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                String file = (openFileDialog.FileName).ToString();

                string constring = "server=localhost;user=root;pwd=;database=AutoStereoDavao;SslMode=none;";

                using (MySqlConnection conn = new MySqlConnection(constring))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            cmd.Connection = conn;
                            conn.Open();
                            mb.ExportInfo.MaxSqlLength = 1024 * 1024; // 1MB
                            mb.ImportFromFile(file);
                            conn.Close();
                        }
                    }
                }
                System.Windows.MessageBox.Show("Data Successfully Restored");
            }

        }
    }
}
