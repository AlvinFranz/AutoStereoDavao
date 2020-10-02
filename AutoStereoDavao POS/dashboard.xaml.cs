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
    /// Interaction logic for dashboard.xaml
    /// </summary>
    public partial class dashboard : UserControl
    {
        public dashboard()
        {
            InitializeComponent();
            show_sales();
            show_net();
            show_trend();
            paramaters();
        }
        private void show_sales()
        {
            String firstDate;
            String secondDate;

            DateTime getSunday = DateTime.Now.AddDays(-1);

            DayOfWeek day = DateTime.Now.DayOfWeek;
            if (day == DayOfWeek.Sunday)
            {
                firstDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                secondDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
            {
                while (getSunday.DayOfWeek != DayOfWeek.Sunday)
                {
                    getSunday = getSunday.AddDays(-1);
                }
                secondDate = getSunday.ToString("yyyy-MM-dd");
                firstDate = getSunday.AddDays(-7).ToString("yyyy-MM-dd");

                this.weekly_report.Text = "Weekly Sales Report ( " +DateTime.Parse(firstDate).ToLongDateString() + " - " + DateTime.Parse(secondDate).ToLongDateString() + " )";
            }
          

            try
            {
                string query = "select sales_id  ," +
                         "customer_name ," +
                         "contact , " +
                         "sales.prod_id," +
                         "date_ordered," +
                         "date_purchased," +
                         "sales.prod_price," +
                         "sales.prod_capital," +
                         "sales.labor_cost," +
                         "payment," +
                         "(select count(sales.sales_id) from sales where date_purchased Between @firstDate AND @secondDate ) as qty_sold," +
                         "(select sum(payment) from sales WHERE date_purchased Between @firstDate AND @secondDate ) as sales_amount," +
                         "(select sum(sales.prod_capital) from sales WHERE date_purchased Between @firstDate AND @secondDate ) as capital," +
                         "(select sum(((payment - sales.prod_capital)-labor_cost)) from sales WHERE date_purchased Between @firstDate AND @secondDate ) as net, " +
                         "(select sum(labor_cost) from sales WHERE date_purchased Between @firstDate AND @secondDate) as labor, " +
                         "customer_type ," +
                         "concat(prod_brand,' ',prod_model,' ',prod_yearModel,' (',prod_size,' inches)') as prod_desc ," +
                         "prod_image " +
                         "from inventory,sales " +
                         "WHERE inventory.prod_id = sales.prod_id " +
                         "AND date_purchased Between @firstDate AND @secondDate " +
                         "order by date_purchased desc";

                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@firstDate", firstDate);
                cmd.Parameters.AddWithValue("@secondDate", secondDate);
                cmd.Prepare();

                MySqlDataReader readers = cmd.ExecuteReader();

                report_qtySold.Text = "0";
                report_gross.Text = "\u20B1 0.00";
                report_capital.Text = "\u20B1 0.00";
                report_labor.Text = "\u20B1 0.00";
                report_net.Text = "\u20B1 0.00";

                if (readers.Read())
                {
                    try { report_qtySold.Text = (readers["qty_sold"].ToString()); } catch (Exception e) { MessageBox.Show(e.Message); return; }
                    try { report_gross.Text = "\u20B1 " + String.Format("{0:0,0.00}", Double.Parse(readers["sales_amount"].ToString())); } catch (Exception e) { MessageBox.Show(e.Message); return; }
                    try { report_capital.Text = "\u20B1 " + String.Format("{0:0,0.00}", Double.Parse(readers["capital"].ToString())); } catch (Exception e) { MessageBox.Show(e.Message); return; }
                    try { report_labor.Text = "\u20B1 " + String.Format("{0:0,0.00}", Double.Parse(readers["labor"].ToString())); } catch (Exception e) { MessageBox.Show(e.Message); return; }
                    try { report_net.Text = "\u20B1 " + String.Format("{0:0,0.00}", Double.Parse(readers["net"].ToString())); } catch (Exception e) { MessageBox.Show(e.Message); return; }
                }


                readers.Close();



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

        private void show_net()
        {
            DateTime date = DateTime.Now;
            String firstDateOfMonth = new DateTime(date.Year, date.Month, 1).ToString("yyyy-MM-dd");
            String currentDate = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                string query = "select sales_id  ," +                                      
                         "sum((payment - prod_capital)-labor_cost) as net " +
                         "from sales " +
                         "WHERE date_purchased Between @firstDate AND @secondDate " +
                         "order by date_purchased desc";

                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@firstDate", firstDateOfMonth);
                cmd.Parameters.AddWithValue("@secondDate", currentDate);
                cmd.Prepare();
                MySqlDataReader readers = cmd.ExecuteReader();
                if (readers.Read())
                {
                    net.Text = "\u20B1 "+ String.Format("{0:0,0.00}", Double.Parse(readers["net"].ToString()));
                }
                else
                {
                    net.Text = "\u20B1 0.00";
                }
                readers.Close();
                connect.Close();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }


        }

        private void show_trend()
        {
            DateTime date = DateTime.Now;
            String firstDateOfMonth = new DateTime(date.Year, date.Month, 1).ToString("yyyy-MM-dd");
            String currentDate = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                string query = "select count(sales.prod_id) as total_qty, " +
                    "concat(prod_brand,' ',prod_model,' ',prod_yearModel,' (',prod_size,' inches)') as prod_desc " +
                    "from sales,inventory " +
                    "WHERE sales.prod_id = inventory.prod_id " +
                    "group by sales.prod_id " +
                    "order by count(sales.prod_id) " +
                    "desc limit 1;";

                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@firstDate", firstDateOfMonth);
                cmd.Parameters.AddWithValue("@secondDate", currentDate);
                cmd.Prepare();
                MySqlDataReader readers = cmd.ExecuteReader();
                if (readers.Read())
                {
                    trend.Text = (readers["prod_desc"].ToString())+" (Total: "+ (readers["total_qty"].ToString())+ " )";
                }
                readers.Close();
                connect.Close();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }


        }
        private void paramaters()
        {
            try
            {
                string query = "select (select sum(prod_quantity) from inventory) as qty_product, " +
                    "sum(payment) as receivable from reservation;";

                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Prepare();
                MySqlDataReader readers = cmd.ExecuteReader();

                if (readers.Read())
                {
                    receivable.Text = "\u20B1 "+ String.Format("{0:0,0.00}", Double.Parse(readers["receivable"].ToString()));
                    qty_product.Text = "Stocks left: "+(readers["qty_product"].ToString());
                }
                readers.Close();
                connect.Close();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

    }
}
