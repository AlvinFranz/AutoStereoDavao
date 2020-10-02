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
    /// Interaction logic for track_orderTab.xaml
    /// </summary>
    public partial class track_orderTab : UserControl
    {
        string reference_number = "";
        public track_orderTab()
        {
            InitializeComponent();
            show_trackOrder();
        }

        public void show_trackOrder()
        {
            try
            {
                string query = "select * from track_order";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;

                cmd.Prepare();

                DataTable dTable = new DataTable();

                MyAdapter.Fill(dTable);
                tbl_track.ItemsSource = dTable.DefaultView;
                connect.Close();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }


        }

        private void tbl_track_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;


            DataRowView SelectedItem = tbl_track.SelectedItem as DataRowView;
            track_num.Text = SelectedItem.Row["track_number"].ToString();
            name.Text = SelectedItem.Row["prod_desc"].ToString();
            reference_number = SelectedItem.Row["ref_num"].ToString();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (track_num.Text == "" || name.Text == "")
            {
                MessageBox.Show("No Data Selected!", "Oops", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Receive Product? This will update Quantity from Inventory?", "Receive Product", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    string query = "update inventory set prod_quantity = (select prod_quantity from inventory WHERE " +
                       "concat(prod_brand,' ',prod_model,' ',prod_yearModel,' (',prod_size,' inches )') = @prod_desc)+1 WHERE" +
                       " concat(prod_brand,' ',prod_model,' ',prod_yearModel,' (',prod_size,' inches )') = @prod_desc ";


                    string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                    MySqlConnection connect = new MySqlConnection(con);
                    connect.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connect);
                    MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                    MyAdapter.SelectCommand = cmd;
                    cmd.Parameters.AddWithValue("@prod_desc", name.Text);
                    cmd.Prepare();

                    DataTable dTable = new DataTable();

                    MyAdapter.Fill(dTable);
                    tbl_track.ItemsSource = dTable.DefaultView;
                    connect.Close();
                    remove_trackOrder();
                    show_trackOrder();

                }
                else { return; }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        private void remove_trackOrder()
        {
            try
            {
                string query = "delete from track_order where ref_num = @ref_number";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@ref_number", reference_number);
                cmd.Prepare();

                DataTable dTable = new DataTable();

                MyAdapter.Fill(dTable);
                tbl_track.ItemsSource = dTable.DefaultView;
                connect.Close();
                reference_number = "";

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (track_num.Text == "" || name.Text == "")
            {
                MessageBox.Show("No Data Selected!", "Oops", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Remove this Data from the System? ?", "Remove Tracking Order", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                remove_trackOrder();
                show_trackOrder();
                MessageBox.Show("Tracking Number :" + track_num.Text + " \n Product: " + name.Text + " !", "Removed from Tracking Order", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else { return; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new_incomingProduct product = new new_incomingProduct();
            product.ShowDialog();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            show_trackOrder();
        }
    }
}
