using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for resevation.xaml
    /// </summary>
    public partial class reservation : UserControl
    {

        int prod_id = 0;
        //string type_customer = "";
        //string customer_name = "";
        //string customer_contact = "";
        //string date_of_order = "";
        //double item_price = 0;
      
        
        public reservation()
        {
            InitializeComponent();
            show_reservation();
        }

        private void tbl_reservation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

         
            DataRowView SelectedItem = tbl_reservation.SelectedItem as DataRowView;
            reserve_id.Text = SelectedItem.Row["ref_id"].ToString();
            customer_type.Text = SelectedItem.Row["customer_type"].ToString();
            name.Text = SelectedItem.Row["customer_name"].ToString();
            contact.Text = SelectedItem.Row["contact"].ToString();
            product.Text = SelectedItem.Row["prod_desc"].ToString();
            price.Text = SelectedItem.Row["product_price"].ToString();
         
            selling_price.Text = SelectedItem.Row["payment"].ToString();
            prod_id = int.Parse(SelectedItem.Row["product_id"].ToString());

            //DateTime get_date = DateTime.Parse(SelectedItem.Row["date_ordered"].ToString());
            String convert = SelectedItem.Row["date_ordered"].ToString();        
            DateTime get_date = DateTime.ParseExact(convert,"MM/dd/yyyy",null);
            date_order.Text = get_date.ToString("yyyy-MM-dd");
          
        }

        private void show_reservation()
        {
            try
            {
                string query = "select reserve_id as ref_id ," +
                    "customer_name ," +
                    "contact , " +
                    "product_id," +
                    "date_ordered," +
                    "product_price," +
                    "payment," +
                    "customer_type ," +
                    "concat(prod_brand,' ',prod_model,' ',prod_yearModel,' (',prod_size,' inches)') as prod_desc ," +
                    "prod_image " +
                    "from inventory,reservation " +
                    "WHERE inventory.prod_id = reservation.product_id " +
                    "order by date_ordered";
                    
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;

                cmd.Prepare();

                DataTable dTable = new DataTable();

                MyAdapter.Fill(dTable);
                tbl_reservation.ItemsSource = dTable.DefaultView;
                connect.Close();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

        }

        private void search_TextChanged(object sender, TextChangedEventArgs e)
        {
            String value = search.Text.ToString(); 
            try
            {
                string query = "select reserve_id as ref_id ," +
                    "customer_name ," +
                    "contact , " +
                    "product_id," +
                    "date_ordered," +
                    "product_price," +
                    "payment," +
                    "customer_type ," +
                    "concat(prod_brand,' ',prod_model,' ',prod_yearModel,' (',prod_size,' inches)') as prod_desc ," +
                    "prod_image " +
                    "from reservation inner join inventory " +
                    "WHERE " +                
                    "(customer_name LIKE @search_value " +
                    "OR contact LIKE @search_value " +
                    "OR date_ordered LIKE @search_value " +
                    "OR product_price LIKE @search_value " +
                    "OR payment LIKE @search_value " +
                    "OR customer_type LIKE @search_value) " +
                    "AND inventory.prod_id = reservation.product_id " +
                    "GROUP BY reserve_id " +
                    "order by date_ordered"; 



                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@search_value", "%"+ value + "%");
                cmd.Prepare();

                DataTable dTable = new DataTable();

                MyAdapter.Fill(dTable);
                tbl_reservation.ItemsSource = dTable.DefaultView;
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

            double labor = 0;
            if (labor_cost.Text != "")
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

            String dateNow = DateTime.Now.ToString("yyyy-MM-dd");
            try
            {
                
                int reference = int.Parse(reserve_id.Text);
                Double pay = Double.Parse(selling_price.Text);

                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Confirm Order?", "Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {

                    string query = "insert into sales VALUES ('',@prod_id,@customer_name,@contact,@date_ordered,@date_purchased,@prod_price,(select prod_capital from inventory where prod_id = @prod_id),@payment,@labor_cost,@customer_type)";
                    String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                    MySqlConnection connect = new MySqlConnection(con);
                    connect.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connect);
                    cmd.Prepare();

                    cmd.Parameters.AddWithValue("@prod_id", prod_id);
                    cmd.Parameters.AddWithValue("@customer_name", name.Text);
                    cmd.Parameters.AddWithValue("@contact", contact.Text);
                    cmd.Parameters.AddWithValue("@date_ordered", date_order.Text);
                    cmd.Parameters.AddWithValue("@date_purchased", dateNow);
                    cmd.Parameters.AddWithValue("@prod_price", price.Text);
                    cmd.Parameters.AddWithValue("@payment", pay);
                    cmd.Parameters.AddWithValue("@labor_cost", labor);
                    cmd.Parameters.AddWithValue("@customer_type", customer_type.Text);

                    cmd.ExecuteNonQuery();                   
                    MessageBox.Show("Successfully Saved Data!", "Sales Record", MessageBoxButton.OK, MessageBoxImage.Information);
                    Console.WriteLine(prod_id);
                   
                    remove_reservation();
                    show_reservation();
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
        private void clear_values()
        {
            reserve_id.Text = "";
            customer_type.Text = "";
            name.Text = "";
            contact.Text = "";
            product.Text = "";
            price.Text = "";
            date_order.Text = "";
            selling_price.Text = ""; ;
            prod_id = 0;
            labor_cost.Text = "";
        }

        private void delete_reservation(object sender, RoutedEventArgs e)
        {
            if (reserve_id.Text == "")
            {
                MessageBox.Show("Select an order");
                return;
            }
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Cancel Reservation?", "Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                string query = "delete from reservation where reserve_id = @reserve_id";
                String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                cmd.Prepare();

                cmd.Parameters.AddWithValue("@reserve_id", reserve_id.Text);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Order has been cancelled! \n" +
                    "Product Quantity has been rerolled ", "Order Cancellation", MessageBoxButton.OK, MessageBoxImage.Information);
                show_reservation();
                reroll_quantity();
                clear_values();
            }
            else
            {
                return;
            }
        }

        private void remove_reservation()
        {
         
                string query = "delete from reservation where reserve_id = @reserve_id";
                String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                cmd.Prepare();

                cmd.Parameters.AddWithValue("@reserve_id", reserve_id.Text);

                cmd.ExecuteNonQuery();
    
                show_reservation();
                clear_values();
        
        }

        private void reroll_quantity()
        {

                string query = "update  inventory set prod_quantity = (Select prod_quantity from inventory where prod_id = @prod_id)+1 where prod_id = @prod_id";
                String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                cmd.Prepare();

                cmd.Parameters.AddWithValue("@prod_id", prod_id);
                Console.WriteLine(prod_id);
                cmd.ExecuteNonQuery();             
                show_reservation();
                clear_values();
            }
        
        }


    

}
