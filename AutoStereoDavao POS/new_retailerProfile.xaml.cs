using Microsoft.Win32;
using MySql.Data.MySqlClient;
using Renci.SshNet.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutoStereoDavao_POS
{
    /// <summary>
    /// Interaction logic for new_retailerProfile.xaml
    /// </summary>
    public partial class new_retailerProfile : Window
    {
        public new_retailerProfile()
        {
            InitializeComponent();
        }

        private void save_newProduct(object sender, RoutedEventArgs e)
        {
            save_profile();
        }
        private void save_profile()
        {
            FileStream fs;
            BinaryReader br;
            byte[] ImageData = new byte[0];      
            string FileName = "";

            if (retailer_imageText.Text != "")
            {
                FileName = retailer_imageText.Text;
                fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(fs);
                ImageData = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();
            }
            if( retailer_firstName.Text == "" || retailer_lastName.Text == "")
            {
                MessageBox.Show("Fill-up missing fields");
                return;
            }
            try
            {
                String contact = retailer_contact.Text;
                String first = retailer_firstName.Text;
                String last = retailer_lastName.Text;

                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Register Profile to the System?", "Save Profile Details", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {

                    string query = "insert into retailer_profile VALUES ('',@firstName,@lastName,@contact,@image)";
                    String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                    MySqlConnection connect = new MySqlConnection(con);
                    connect.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connect);
                    cmd.Prepare();

                    cmd.Parameters.AddWithValue("@firstName", first);
                    cmd.Parameters.AddWithValue("@lastName", last);
                    cmd.Parameters.AddWithValue("@contact", contact);                   
                    cmd.Parameters.AddWithValue("@image", ImageData);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Successfully Saved Data!", "Registration", MessageBoxButton.OK, MessageBoxImage.Information);
                    clear_values();
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        public void clear_values()
        {
            retailer_lastName.Text = "";
            retailer_firstName.Text = "";
            retailer_image.Source = null;
            retailer_imageText.Text = "";
            retailer_contact.Text = "";
 
        }

        private void open_image(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.JPG;)|*.png;*.jpeg;*.JPG;";
            if (openFileDialog.ShowDialog() == true)
            {
                String file = (openFileDialog.FileName).ToString();
                retailer_imageText.Text = file;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(file);
                bitmap.EndInit();
                retailer_image.Source = bitmap;
            }
        }
    }
}
