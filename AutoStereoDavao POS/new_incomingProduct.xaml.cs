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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutoStereoDavao_POS
{
    /// <summary>
    /// Interaction logic for new_incomingProduct.xaml
    /// </summary>
    public partial class new_incomingProduct : Window
    {
        String prod_desc = "";
        public new_incomingProduct()
        {
            InitializeComponent();
            populate_products();
        }

        private void populate_products()
        {
            try
            {
                string query = "select concat(prod_brand,' ',prod_model,' ',prod_yearModel,' (',prod_size,' inches )') as product from inventory";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
              
                cmd.Prepare();
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cb_products.Items.Add((string)reader["product"]);
                }

            }
                
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void save_newProduct(object sender, RoutedEventArgs e)
        {
            if (track_num.Text == "" || prod_desc == "")
            {
                MessageBox.Show("Incomplete details");
                return;
            }       
            try
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Save details to the system?", "Incoming Products", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {

                    string query = "insert into track_order values ('',@track_number,@prod_desc) ";
                    String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                    MySqlConnection connect = new MySqlConnection(con);
                    connect.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connect);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@track_number", track_num.Text);
                    cmd.Parameters.AddWithValue("@prod_desc", prod_desc);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Successfully Saved Data!", "Track Products", MessageBoxButton.OK, MessageBoxImage.Information);
               

                    //to reset selected value
                    cb_products.SelectedIndex = 0;
                    prod_desc = "";
              
              
                    
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
        private void cb_reseller_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            prod_desc = cb_products.SelectedItem.ToString();
        }

    }
}
