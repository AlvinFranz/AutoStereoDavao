using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for transaction.xaml
    /// </summary>
    public partial class transaction : UserControl
    {
        int product_id = 0;
        String reseller_name = "";
        String order_type = "";
        String contact = "";
        String name ="";
        String value = "";
        public transaction()
        {
            InitializeComponent();
          
            inventory();
            populate_reseller();
        }
        public string TextBoxValue
        {
            get { return customer_name.Text; }
            set { customer_name.Text = value; }
           
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.customer_contact.Text = "";
            this.customer_name.Text = "";
            name = "";
            contact = "";
            value = ((System.Windows.Controls.ComboBoxItem)client_type.SelectedItem).Content as string;
         
            if (value.Equals("Resellers"))
            {
                
                cb_reseller.Visibility = System.Windows.Visibility.Visible;
                customer_name.Visibility = System.Windows.Visibility.Hidden;
                customer_contact.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (value.Equals("Customer"))
            {
                
                cb_reseller.Visibility = System.Windows.Visibility.Hidden;
                customer_name.Visibility = System.Windows.Visibility.Visible;
                customer_contact.Visibility = System.Windows.Visibility.Visible;
            }

        }

        public void inventory()
        {
            try
            {
                string query = "select * from inventory where prod_quantity != 0 ";
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

        private void tbl_inventory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            prod_image.Source = null;
            DataRowView SelectedItem = tbl_inventory.SelectedItem as DataRowView;
       
            String brand = SelectedItem.Row["prod_brand"].ToString();
            String model = SelectedItem.Row["prod_model"].ToString();
            String year_model = SelectedItem.Row["prod_yearModel"].ToString();         
            String size = SelectedItem.Row["prod_size"].ToString();
            prod_id.Text = SelectedItem.Row["prod_id"].ToString();
            prod_desc.Text = brand + " " + model + " " + year_model + " (" + size + " inches)";
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

        private void search_TextChanged(object sender, TextChangedEventArgs e)
        {
            search_inventory();
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
                cmd.Parameters.AddWithValue("@search", "%" + search_value.Trim() + "%");
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
         
         private void order_details ()
        {
            if (value == "")
            {
                MessageBox.Show("Fill-up missing details", "Incomplete Details", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (value.Equals("Resellers"))
            {
                if (prod_id.Text == "" || reseller_name == "" )
                {
                    MessageBox.Show("Fill-up missing details", "Incomplete Details", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else
                {                           
                    product_id = int.Parse(prod_id.Text);
                    purchase_popup pop = new purchase_popup(product_id, reseller_name, "", "", order_type,"Reseller");
                   
                    pop.Show();
                }

            }
            else if (value.Equals("Customer"))
            {
                if (prod_id.Text == "" || customer_contact.Text == "" || customer_name.Text == "")
                {
                    MessageBox.Show("Fill-up missing details", "Incomplete Details", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else
                {                 
                    product_id = int.Parse(prod_id.Text);
                    name = customer_name.Text;
                    contact = customer_contact.Text;
                    purchase_popup pop = new purchase_popup(product_id, "", name, contact, order_type,"Customer");
                    
                    pop.Show();
                   
                }

            }
        }

        private void purchase_pop(object sender, RoutedEventArgs e)
        {
            order_type = "purchase";
            order_details();
         
           
        }
        private void reserve_pop(object sender, RoutedEventArgs e)
        {
            order_type = "reservation";
            order_details();
        }

        private void cb_reseller_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           if (reseller_name == "")
            {
                reseller_name = cb_reseller.SelectedItem.ToString();
            }
         


        }
        private void populate_reseller()
        {
            try
            {
                string query = "select concat(retailer_firstName,' ',retailer_lastName) as reseller_name  from retailer_profile";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@name", reseller_name);
                cmd.Prepare();
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cb_reseller.Items.Add((string)reader["reseller_name"]);
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }


    }
}
