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
    /// Interaction logic for reseller_record.xaml
    /// </summary>
    public partial class reseller_record : UserControl
    {
        public reseller_record()
        {
            InitializeComponent();
            show_profileRecords();
        }

        private void show_profileRecords()
        {
            try 
            {

                string query = "select retailer_image as reseller_image , " +
                             "  retailer_id as profile_id,  " +
                             "  concat(retailer_firstname, ' ', retailer_lastname) as reseller_name, " +
                             " count(*) as qty_items, " +
                             " sum(payment) as amount, " +
                             "  max(date_purchased) as date_purchased " +
                             "  from sales, retailer_profile where sales.customer_name = concat(retailer_firstname, ' ', retailer_lastname) group by customer_name order by count(*) desc; ";

            string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            MySqlConnection connect = new MySqlConnection(con);
            connect.Open();
            MySqlCommand cmd = new MySqlCommand(query, connect);
            MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
            MyAdapter.SelectCommand = cmd;

            cmd.Prepare();

            DataTable dTable = new DataTable();

            MyAdapter.Fill(dTable);
            tbl_records.ItemsSource = dTable.DefaultView;
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
