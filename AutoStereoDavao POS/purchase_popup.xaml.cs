using MaterialDesignThemes.Wpf;
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
    /// Interaction logic for purchase_popup.xaml
    /// </summary>
    public partial class purchase_popup : Window
    {
        int qty = 0;
        String type_order = "";
        String type_customer = "";
       
        public purchase_popup(int prod_id , String reseller_name, String customer_name, String customer_contact,String transact_type,String customer_type)
        {
            InitializeComponent();
            type_customer = customer_type;
            type_order = transact_type;
            this.order_type.Text = transact_type;
            this.product_id.Text = prod_id.ToString();
            customer_information(reseller_name,customer_name,customer_contact);
            product_data(prod_id);
        }
        private void product_data(int prod_id)
        {
            this.product_id.Text = prod_id.ToString();
            int size=0,  yearModel = 0;
            String brand ="" , model = "";
            double price = 0;
         
            try
            {
                string query = "select prod_brand, prod_model,prod_size,prod_quantity,prod_yearModel,prod_price,prod_image from inventory where prod_id = @prod_id";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@prod_id", prod_id);
                cmd.Prepare();
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    size = (int)reader["prod_size"];
                    yearModel= (int)reader["prod_yearModel"];
                    brand = (string)reader["prod_brand"];
                    model = (string)reader["prod_model"];
                    price = (double)reader["prod_price"];
                    qty = (int)reader["prod_quantity"];

                    byte[] data = (byte[])reader["prod_image"]; // 0 is okay if you only selecting one column
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

                    }
                }
              

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            this.item_desc.Text = brand + " " + model + " " + yearModel.ToString() + " (" + size.ToString() + " inches)";
            this.item_price.Text = price.ToString();

        }

        private void customer_information(String reseller_name , String customer_name,String customer_contact)
        {
            if(reseller_name == "" )
            {
                this.name.Text = customer_name;
                this.contact.Text = customer_contact;

            }
            else if (reseller_name != "")
               
            {
                try
                {
                    string query = "select retailer_contact as contact from retailer_profile  where CONCAT(retailer_firstName,' ',retailer_lastName) = @name";
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
                        this.contact.Text = (string)reader["contact"];
                    }

                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

                this.name.Text = reseller_name;
             
            }
        }

        private void insert_purchase()
        {
            double labor = 0;
            if(labor_cost.Text != "" )
            {
                try
                {
                     labor = double.Parse(this.labor_cost.Text);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
           
            try
            {
                double payment_check = double.Parse(this.offer.Text);
           
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            try
            {

                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Register sale to the system?", "Save Sales Details", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {

                    String dateNow = DateTime.Now.ToString("yyyy-MM-dd");

                    string query = "insert into sales VALUES ('',@prod_id,@customer_name,@contact,@date_now,@date_now,@product_price,(select prod_capital from inventory where prod_id = @prod_id),@payment,@labor_cost,@customer_type)";
                    String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                    MySqlConnection connect = new MySqlConnection(con);
                    connect.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connect);
                    cmd.Prepare();

                    cmd.Parameters.AddWithValue("@prod_id", int.Parse(this.product_id.Text));
                    cmd.Parameters.AddWithValue("@customer_name", this.name.Text.ToString());
                    cmd.Parameters.AddWithValue("@date_now", dateNow);
                    cmd.Parameters.AddWithValue("@customer_type", type_customer);
                    cmd.Parameters.AddWithValue("@contact", contact.Text);
                    cmd.Parameters.AddWithValue("@product_price", this.item_price.Text);
                    cmd.Parameters.AddWithValue("@payment", this.offer.Text);
                    cmd.Parameters.AddWithValue("@labor_cost", labor);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Successfully Saved Data!", "Reservation", MessageBoxButton.OK, MessageBoxImage.Information);
                    close_transaction();
                    update_productQuantity();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        private void insert_reservation()
        {
           try 
            { 
             double payment_check = double.Parse(this.offer.Text);
            } 
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message); 
                return; 
            }
            try 
            {
               
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Register reservation to the system?", "Save Reservation Details", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes) 
                { 

            String dateNow = DateTime.Now.ToString("yyyy-MM-dd");

            string query = "insert into reservation VALUES ('',@customer_name,@contact,@prod_id,@date_ordered,@product_price,@payment,@customer_type)";
            String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            MySqlConnection connect = new MySqlConnection(con);
            connect.Open();
            MySqlCommand cmd = new MySqlCommand(query, connect);
            cmd.Prepare();

            cmd.Parameters.AddWithValue("@prod_id", int.Parse(this.product_id.Text));
            cmd.Parameters.AddWithValue("@customer_name", this.name.Text.ToString()) ;
            cmd.Parameters.AddWithValue("@date_ordered", dateNow);
            cmd.Parameters.AddWithValue("@customer_type", type_customer);
            cmd.Parameters.AddWithValue("@contact", contact.Text);
            cmd.Parameters.AddWithValue("@product_price", this.item_price.Text);
            cmd.Parameters.AddWithValue("@payment", this.offer.Text);

            cmd.ExecuteNonQuery();
            MessageBox.Show("Successfully Saved Data!", "Reservation", MessageBoxButton.OK, MessageBoxImage.Information);
            close_transaction();
             update_productQuantity();
            this.Close(); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }


        private void update_productQuantity()
        {
            try
            {

            string query = "update inventory set prod_quantity = @prod_quantity where prod_id  = @prod_id";
            String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            MySqlConnection connect = new MySqlConnection(con);
            connect.Open();
            MySqlCommand cmd = new MySqlCommand(query, connect);
            cmd.Prepare();

            cmd.Parameters.AddWithValue("@prod_id", int.Parse(this.product_id.Text));
            cmd.Parameters.AddWithValue("@prod_quantity", qty-1);
            cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
            MessageBox.Show(ex.Message);
            return;
            }
          }

        private void close_transaction()
        {
            transaction purchase = new transaction();
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    (window as MainWindow).stack.Children.Clear();
                    (window as MainWindow).stack.Children.Add(purchase);
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            if (order_type.Text == "reservation")
            {
                insert_reservation();
            }
            else if (order_type.Text == "purchase")
            {
                insert_purchase();
            }
           
        }
        private void clear_values()
        {
            product_id.Text = "";
            item_desc.Text = "";
            item_price.Text = "";
            prod_image.Source = null;
            name.Text = "";
            contact.Text = "";
            order_type.Text = "";
            offer.Text = "";


        }

        private void offer_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }
    }
}
