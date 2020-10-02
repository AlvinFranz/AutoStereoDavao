
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AutoStereoDavao_POS
{
    /// <summary>
    /// Interaction logic for Inventory.xaml
    /// </summary>
    public partial class Inventory : UserControl 
    {
        public Inventory()
        {
            InitializeComponent();
            show_inventory();
        }

        private void new_product(object sender, System.EventArgs e)
        {
            new_product new_prod = new new_product();
            new_prod.ShowDialog();
        }

  

        private void tbl_inventory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            prod_image.Source = null;
            DataRowView SelectedItem = tbl_inventory.SelectedItem as DataRowView;
            prod_id.Text = SelectedItem.Row["prod_id"].ToString();
            prod_brand.Text = SelectedItem.Row["prod_brand"].ToString();
            prod_model.Text = SelectedItem.Row["prod_model"].ToString();
            prod_yearModel.Text = SelectedItem.Row["prod_yearModel"].ToString();
            prod_capital.Text = SelectedItem.Row["prod_capital"].ToString();
            prod_size.Text = SelectedItem.Row["prod_size"].ToString();
            prod_quantity.Text = SelectedItem.Row["prod_quantity"].ToString();
            prod_price.Text = SelectedItem.Row["prod_price"].ToString();

            byte[] data = (byte[])SelectedItem.Row["prod_image"]; // 0 is okay if you only selecting one column
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
                    prod_image.Source = imageSource;
                }
            } 
            else
            {
                return;
            }
        }

        private void edit_product(object sender, RoutedEventArgs e)
        {
            FileStream fs;
            BinaryReader br;
            byte[] ImageData = new byte[0];
            int quantity = 0;
            string FileName = "";

            if (prod_quantity.Text != "")
            {
                try { quantity = int.Parse(prod_quantity.Text); } catch (Exception E) { MessageBox.Show(E.Message); return;  }
            }

            if (prod_imageText.Text != "")
            {
                FileName = prod_imageText.Text;
                fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(fs);
                ImageData = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();
            }

            try
            {       
                String brand = prod_brand.Text;
                String model = prod_model.Text;
                int id = int.Parse(prod_id.Text);
                int size = int.Parse(prod_size.Text);
                int yearModel = int.Parse(prod_yearModel.Text);
                Double price = Double.Parse(prod_price.Text);
                Double capital = Double.Parse(prod_capital.Text);

                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Update Product Details?", "Save Product Details", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    string query = "";
                    if(FileName == "")
                    {
                        query = "update inventory set prod_brand = @prod_brand, prod_model = @prod_model, prod_yearModel = @prod_yearModel," +
                        " prod_size = @prod_size, prod_capital = @prod_capital, prod_price = @prod_price, prod_quantity = @prod_quantity " +
                        " where prod_id = @prod_id";
                    } else
                    {
                        query = "update inventory set prod_brand = @prod_brand, prod_model = @prod_model, prod_yearModel = @prod_yearModel," +
                       " prod_size = @prod_size, prod_capital = @prod_capital, prod_price = @prod_price, prod_quantity = @prod_quantity, " +
                       "prod_image = @prod_image where prod_id = @prod_id";
                    }
       
                String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@prod_id", id);
                cmd.Parameters.AddWithValue("@prod_brand", brand);
                cmd.Parameters.AddWithValue("@prod_model", model);
                cmd.Parameters.AddWithValue("@prod_yearModel", yearModel);
                cmd.Parameters.AddWithValue("@prod_size", size);
                cmd.Parameters.AddWithValue("@prod_capital", capital);
                cmd.Parameters.AddWithValue("@prod_price", price);
                cmd.Parameters.AddWithValue("@prod_quantity", quantity);
                cmd.Parameters.AddWithValue("@prod_image", ImageData);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Successfully Updated Data!", "Registration", MessageBoxButton.OK, MessageBoxImage.Information);
                clear_values();
                show_inventory();
                }
                    else
                {
                    return;
                }
             }
            catch (Exception ex)
            {
                    MessageBox.Show(ex.Message,"Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
            }
        }

        public void show_inventory()
        {
          
            try
            {
                string query = "select * from inventory";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;

                cmd.Prepare();

                DataTable dTable = new DataTable();

                MyAdapter.Fill(dTable);
                tbl_inventory.ItemsSource = dTable.DefaultView;
                connect.Close();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        public void clear_values()
         {
            prod_id.Text = "";
            prod_brand.Text = "";
            prod_model.Text = "";
            prod_size.Text = "";
            prod_yearModel.Text = "";
            prod_price.Text = "";
            prod_capital.Text = "";
            prod_imageText.Text = "";
            prod_quantity.Text = "";
            prod_image.Source = null;

         }
        private void open_image(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.JPG;)|*.png;*.jpeg;*.JPG;";
            if (openFileDialog.ShowDialog() == true)
            {
                String file = (openFileDialog.FileName).ToString();
                prod_imageText.Text = file;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(file);
                bitmap.EndInit();
                prod_image.Source = bitmap;
            }
        }

        public void search_inventory()
        {
            String search_value = search.Text;
            try
            {
                Console.WriteLine(search_value);
                string query = "select * from inventory WHERE " +
                    "prod_id LIKE @search OR " +
                    "prod_brand LIKE  @search OR " +
                    "prod_model LIKE  @search OR " +
                    "prod_yearModel LIKE  @search OR " +
                    "prod_capital LIKE  @search OR " +
                    "prod_size LIKE  @search OR " +
                    "prod_quantity LIKE  @search ";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@search", "%" +search_value.Trim() + "%");
                cmd.Prepare();

                DataTable dTable = new DataTable();

                MyAdapter.Fill(dTable);
                tbl_inventory.ItemsSource = dTable.DefaultView;
                connect.Close();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            search_inventory();
        }

        private void refresh(object sender, RoutedEventArgs e)
        {
            show_inventory();
        }
    }
}
