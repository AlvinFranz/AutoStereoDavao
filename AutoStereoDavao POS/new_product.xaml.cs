using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Data;
using System.CodeDom;
using System.IO;

namespace AutoStereoDavao_POS
{
    /// <summary>
    /// Interaction logic for new_product.xaml
    /// </summary>
    public partial class new_product : Window
    {
        public new_product()
        {
            InitializeComponent();

        }

        private void open_image(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.JPG;)|*.png;*.jpeg;*.JPG;";
            if (openFileDialog.ShowDialog() == true)
            {
                String file = (openFileDialog.FileName).ToString();
                prod_imageText.Text =  file;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(file);
                bitmap.EndInit();
                prod_image.Source = bitmap;
            }
        }

        private void save_newProduct(object sender, RoutedEventArgs e)
        {
            FileStream fs;
            BinaryReader br;
            byte[] ImageData = new byte[0];
            int quantity = 0;
            string FileName = "";

            if (prod_qty.Text != "" )
            {  
                quantity = int.Parse(prod_qty.Text);
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
            int size = int.Parse(prod_size.Text);
            String yearModel = prod_yearModel.Text;        
            Double price = Double.Parse(prod_price.Text);
            Double capital = Double.Parse(prod_capital.Text);

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Register product to the system?", "Save Product Details", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
           
                string query = "insert into inventory VALUES ('',@prod_brand,@prod_model,@prod_yearModel,@prod_size,@prod_price,@prod_capital,@prod_quantity,@prod_image)";
                String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                cmd.Prepare();
         
                cmd.Parameters.AddWithValue("@prod_brand", brand);
                cmd.Parameters.AddWithValue("@prod_model", model);
                cmd.Parameters.AddWithValue("@prod_yearModel", yearModel);
                cmd.Parameters.AddWithValue("@prod_size", size);
                cmd.Parameters.AddWithValue("@prod_capital", capital);
                cmd.Parameters.AddWithValue("@prod_price", price);
                cmd.Parameters.AddWithValue("@prod_quantity", quantity);
                cmd.Parameters.AddWithValue("@prod_image", ImageData);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Successfully Saved Data!","Registration", MessageBoxButton.OK, MessageBoxImage.Information);
                clear_values();
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
        public void clear_values()
        {
            prod_brand.Text = "";
            prod_model.Text = "";
            prod_size.Text = "";
            prod_yearModel.Text = "";
            prod_price.Text = "";
            prod_capital.Text = "";
            prod_imageText.Text = "";
            prod_qty.Text = "";
            prod_image.Source = null;

        }

        private void prod_yearModel_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
