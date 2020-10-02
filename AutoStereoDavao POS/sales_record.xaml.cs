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
    /// Interaction logic for sales_record.xaml
    /// </summary>
    public partial class sales_record : UserControl
    {
        String get_salesID = "";
        String get_prodID = "";
        public sales_record()
        {
            InitializeComponent();
            show_sales();
        }
        public void show_sales()
        {
            try
            {
                string query = "select sales_id  ," +
                    "customer_name ," +
                    "contact , " +
                    "sales.prod_id as product_id," +
                    "date_ordered," +
                    "date_purchased," +
                    "sales.prod_price," +
                    "payment," +
                    "customer_type ," +
                    "concat(prod_brand,' ',prod_model,' ',prod_yearModel,' (',prod_size,' inches)') as prod_desc ," +
                    "prod_image " +
                    "from inventory,sales " +
                    "WHERE inventory.prod_id = sales.prod_id " +
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
                tbl_sales.ItemsSource = dTable.DefaultView;
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
            String value = search.Text.ToString().Trim();
            try
            {
                string query = "select distinct sales_id ," +
                    "customer_name ," +
                    "contact , " +
                    "sales.prod_id," +
                    "date_ordered," +
                    "date_purchased," +
                    "sales.prod_price," +
                    "payment," +
                    "customer_type ," +
                    "concat(prod_brand,' ',prod_model,' ',prod_yearModel,' (',prod_size,' inches)') as prod_desc ," +
                    "prod_image " +
                    "from sales inner join inventory " +
                    "WHERE " +
                    "(customer_name LIKE @search_value " +
                    "OR contact LIKE @search_value " +
                    "OR date_ordered LIKE @search_value " +
                    "OR date_purchased LIKE @search_value " +
                    "OR sales.prod_price LIKE @search_value " +
                    "OR payment LIKE @search_value " +
                    "OR concat(prod_brand,' ',prod_model,' ',prod_yearModel,' (',prod_size,' inches)') LIKE @search_value " +
                    "OR customer_type LIKE @search_value) " +
                    "AND sales.prod_id = inventory.prod_id " +
                    "GROUP BY sales_id " +
                    "order by date_ordered";



                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@search_value", "%" + value + "%");
                cmd.Prepare();

                DataTable dTable = new DataTable();

                MyAdapter.Fill(dTable);
                tbl_sales.ItemsSource = dTable.DefaultView;
                connect.Close();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void create_report(object sender, RoutedEventArgs e)
        {

            if (start_date.Text == "" || end_date.Text == "")
            {
                return;
            }
            DateTime start = DateTime.Parse(start_date.Text);
            DateTime end = DateTime.Parse(end_date.Text);

   
            Console.WriteLine(start.ToString("yyyy-MM-dd"));
         
            sales_recordViewer reports = new sales_recordViewer(start.ToString("yyyy-MM-dd"),end.ToString("yyyy-MM-dd"));
            reports.ShowDialog();
        }

        private void tbl_sales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

           
            DataRowView SelectedItem = tbl_sales.SelectedItem as DataRowView;
            get_salesID = SelectedItem.Row["sales_id"].ToString();
            get_prodID = SelectedItem.Row["product_id"].ToString();
        }

        private void deleteData()
        {

            string query = "delete from sales where sales_id = @sales_id ";
            String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            MySqlConnection connect = new MySqlConnection(con);
            connect.Open();
            MySqlCommand cmd = new MySqlCommand(query, connect);
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@sales_id", get_salesID);
            cmd.ExecuteNonQuery();


            //to reset selected value


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (get_salesID == "" )
            {
                MessageBox.Show("Please Select an Item");
                return;
            }
            try
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Delete Highlighted Item #"+get_salesID+ "?", "Sales Record", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    revert_product();
                    deleteData();          
                    show_sales();
                    get_salesID = "";
                    MessageBox.Show("Successfully Deleted Data!", "Sales Record", MessageBoxButton.OK, MessageBoxImage.Information);

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

        private void revert_product()
        {
            string query = "update  inventory set prod_quantity = (Select prod_quantity from inventory where prod_id = @prod_id)+1 where prod_id = @prod_id";
            String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            MySqlConnection connect = new MySqlConnection(con);
            connect.Open();
            MySqlCommand cmd = new MySqlCommand(query, connect);
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@sales_id", get_salesID);
            cmd.Parameters.AddWithValue("@prod_id", get_prodID);
            cmd.ExecuteNonQuery();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (get_salesID == "")
            {
                MessageBox.Show("Please Select an Item");
                return;
            }
            try
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Undo Highlighted Item #" + get_salesID + "?", "Sales Record", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {

                    string query = "insert into reservation (customer_name,contact,product_id,date_ordered,product_price,payment,customer_type) " +
                            "SELECT customer_name,contact,prod_id,date_ordered,prod_price,payment,customer_type from sales where sales_id = @sales_id  ;";

                    String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                    MySqlConnection connect = new MySqlConnection(con);
                    connect.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connect);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@sales_id", get_salesID);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Data has been reverted into Reservation Data!", "Sales Record", MessageBoxButton.OK, MessageBoxImage.Information);


                    //to reset selected value
                    revert_product();
                    deleteData();
                    show_sales();
                    get_salesID = "";
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
    }
}
