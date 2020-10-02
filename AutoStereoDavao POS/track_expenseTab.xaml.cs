using MySql.Data.MySqlClient;
using Renci.SshNet.Messages;
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
    /// Interaction logic for track_expenseTab.xaml
    /// </summary>
    public partial class track_expenseTab : UserControl
    {
        public track_expenseTab()
        {
            InitializeComponent();
            show_expense();
        }

        private void show_expense()
        {
            try
            {
                string query = "select * from expenses";
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                MySqlConnection connect = new MySqlConnection(con);
                connect.Open();
                MySqlCommand cmd = new MySqlCommand(query, connect);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = cmd;

                cmd.Prepare();

                DataTable dTable = new DataTable();

                MyAdapter.Fill(dTable);
                tbl_expense.ItemsSource = dTable.DefaultView;
                connect.Close();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }


        }

        private void tbl_expense_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;
       
            DataRowView SelectedItem = tbl_expense.SelectedItem as DataRowView;
            ref_num.Text = SelectedItem.Row["ref_id"].ToString();
            description.Text = SelectedItem.Row["description"].ToString();
            amount.Text = SelectedItem.Row["amount"].ToString();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            expenses expense = new expenses();
            expense.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(description.Text ==  "" || amount.Text == "" )
            {
            
                MessageBox.Show("Incomplete Data");
                return;

            }
            try { double.Parse(amount.Text); } catch (Exception ex) { MessageBox.Show(ex.Message); return; }

            try
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Update Highlighted Item #" + ref_num.Text + "?", "Expense Record", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    string query = "update expenses set descriptipn = @description, amount = @amount where ref_id = @ref_id";
                    String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                    MySqlConnection connect = new MySqlConnection(con);
                    connect.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connect);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@amount", amount.Text);
                    cmd.Parameters.AddWithValue("@description", description.Text);
                    cmd.Parameters.AddWithValue("@ref_id", ref_num.Text);
                    cmd.ExecuteNonQuery();
                    show_expense();

                    MessageBox.Show("Data has been updated!", "Expense Record", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else 
                { 
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (ref_num.Text == "")
            {

                MessageBox.Show("Please highlight an Item");
                return;

            }
          
            try
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Delete Highlighted Item #" + ref_num.Text + "?", "Expense Record", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    string query = "Delete from expenses where ref_id = @ref_id";
                    String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                    MySqlConnection connect = new MySqlConnection(con);
                    connect.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connect);
                    cmd.Prepare();
                    
                    cmd.Parameters.AddWithValue("@ref_id", ref_num.Text);
                    cmd.ExecuteNonQuery();
                    show_expense();

                    MessageBox.Show("Data has been deleted!", "Expense Record", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            show_expense();
        }
    }
    
}
