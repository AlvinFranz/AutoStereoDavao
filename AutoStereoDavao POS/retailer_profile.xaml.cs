using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoStereoDavao_POS
{
    /// <summary>
    /// Interaction logic for retailer_profile.xaml
    /// </summary>
    public partial class retailer_profile : UserControl
    {
        public retailer_profile()
        {
            InitializeComponent();
            show_profile();
        }

        public void show_profile()
        {
            try
            {
                string query = "select * from retailer_profile";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;

                cmd.Prepare();

                DataTable dTable = new DataTable();

                MyAdapter.Fill(dTable);
                tbl_retailerProfile.ItemsSource = dTable.DefaultView;
                connect.Close();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            show_profile();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            new_retailerProfile profile = new new_retailerProfile();
            profile.ShowDialog();

        }

        private void search_TextChanged(object sender, TextChangedEventArgs e)
        {
            search_profile();
        }

        private void search_profile()
        {
            String value = search.Text;
            try 
            {              
            string query = "select * from retailer_profile WHERE " +
                    "retailer_id LIKE @search OR " +
                    "retailer_firstName LIKE @search OR " +
                    "retailer_lastName LIKE @search OR " +
                    "retailer_contact LIKE @search;";
            string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            MySqlConnection connect = new MySqlConnection(con);
            connect.Open();
            MySqlCommand cmd = new MySqlCommand(query, connect);
            MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
            MyAdapter.SelectCommand = cmd;
            cmd.Parameters.AddWithValue("@search", "%" + value + "%");
            cmd.Prepare();

            DataTable dTable = new DataTable();

            MyAdapter.Fill(dTable);
            tbl_retailerProfile.ItemsSource = dTable.DefaultView;
            connect.Close();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

}

        private void tbl_retailerProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            retail_image.Source = null;
            DataRowView SelectedItem = tbl_retailerProfile.SelectedItem as DataRowView;
            acc_id.Text = SelectedItem.Row["retailer_id"].ToString();
            firstname.Text = SelectedItem.Row["retailer_firstName"].ToString();
            surname.Text = SelectedItem.Row["retailer_lastName"].ToString();
            contact.Text = SelectedItem.Row["retailer_contact"].ToString();
            byte[] data = (byte[])SelectedItem.Row["retailer_image"]; // 0 is okay if you only selecting one column
                                                                  //And use:
            if (data != null && data.Length > 0)
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(data))
                {
                    var imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    imageSource.StreamSource = ms;
                    imageSource.CacheOption = BitmapCacheOption.OnLoad;
                    imageSource.EndInit();
                    retail_image.Source = imageSource;
                }
            }
            else
            {
                return;
            }
        }

        private void update_profile(object sender, RoutedEventArgs e)
        {
            if (acc_id.Text == "" )
            {
                return;
            }
            FileStream fs;
            BinaryReader br;
            byte[] ImageData = new byte[0];
            string FileName = "";

            if (imageText.Text != "")
            {
                FileName = imageText.Text;
                fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(fs);
                ImageData = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();
            }

            try
            {
                String retailer_id = acc_id.Text;
                String retail_contact = contact.Text;
                String first = firstname.Text;
                String last = surname.Text;

                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Update Profile Details?", "Save Profile Details", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    string query = "";
                    if(FileName == "") 
                    {
                        query = "update retailer_profile set retailer_firstName = @firstName , retailer_lastName = @lastName , retailer_contact = @contact  where retailer_id = @id;";
                    }
                    else
                    {
                        query = "update retailer_profile set retailer_firstName = @firstName , retailer_lastName = @lastName , retailer_contact = @contact , retailer_image = @image where retailer_id = @id;";
                    }
                    String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                    MySqlConnection connect = new MySqlConnection(con);
                    connect.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connect);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@id", retailer_id);
                    cmd.Parameters.AddWithValue("@firstName", first);
                    cmd.Parameters.AddWithValue("@lastName", last);
                    cmd.Parameters.AddWithValue("@contact", retail_contact);
                    cmd.Parameters.AddWithValue("@image", ImageData);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Successfully Updated Profile!", "Registration", MessageBoxButton.OK, MessageBoxImage.Information);
                    clear_values();
                    show_profile();
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
        
        private void clear_values()
        {
            acc_id.Text = "";
            contact.Text = "";
            firstname.Text = "";
            surname.Text = "";
            imageText.Text = "";
            retail_image.Source = null;
        }
        private void open_image(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.JPG;)|*.png;*.jpeg;*.JPG;";
            if (openFileDialog.ShowDialog() == true)
            {
                String file = (openFileDialog.FileName).ToString();
                imageText.Text = file;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(file);
                bitmap.EndInit();
                retail_image.Source = bitmap;
            }
        }
    }
}
