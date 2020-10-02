using Microsoft.Reporting.WinForms;
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
using System.Windows.Shapes;

namespace AutoStereoDavao_POS
{
    /// <summary>
    /// Interaction logic for sales_recordViewer.xaml
    /// </summary>
    public partial class sales_recordViewer : Window
    {
        public sales_recordViewer(String date1, String date2)
        {
            InitializeComponent();
            getParameters(date1, date2);
            report(date1,date2);
        }

        private void report(String date1, String date2)
        {
            this.reportViewer1.LocalReport.ReportPath = @"..\..\report_sales.rdlc";
            this.reportViewer1.LocalReport.DataSources.Clear();

            DataTable dt = new DataTable();


            string query = "select sales_id  ," +
                        "customer_name ," +
                        "contact , " +
                        "sales.prod_id," +
                        "date_ordered," +
                        "date_purchased," +
                        "sales.prod_price," +
                        "payment," +
                        "(select count(sales.sales_id) from sales group by sales_id limit 1) as qty_sold," +
                        "payment as sales_amount," +
                        "sales.prod_capital as capital," +
                        "((payment - sales.prod_capital)-labor_cost) as net, " +
                        "labor_cost as labor, " +
                        "customer_type ," +
                        "concat(prod_brand,' ',prod_model,' ',prod_yearModel,' (',prod_size,' inches)') as prod_desc ," +
                        "prod_image " +
                        "from inventory,sales " +
                        "WHERE inventory.prod_id = sales.prod_id " +
                        "AND date_purchased Between @firstDate AND @secondDate " +
                        "order by date_ordered";



            String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            MySqlConnection connect = new MySqlConnection(con);
            connect.Open();
            MySqlCommand cmd = new MySqlCommand(query, connect);
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@firstDate", date1);
            cmd.Parameters.AddWithValue("@secondDate", date2);
            //cmd.ExecuteNonQuery();

            MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
            MyAdapter.SelectCommand = cmd;
            MyAdapter.Fill(dt);

            MySqlDataReader readers = cmd.ExecuteReader();

            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dt));

            this.reportViewer1.RefreshReport();
        }

        private void getParameters(String date_start, String date_end)
        {
          
            ReportParameter startDate = new ReportParameter();
            ReportParameter endDate = new ReportParameter();
            this.reportViewer1.LocalReport.ReportPath = @"..\..\report_sales.rdlc";
            this.reportViewer1.LocalReport.DataSources.Clear();

            startDate = new ReportParameter("startDate", date_start);
            endDate = new ReportParameter("endDate", date_end);
          
            this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { startDate, endDate });
            this.reportViewer1.RefreshReport();


        }
     
    }
}
