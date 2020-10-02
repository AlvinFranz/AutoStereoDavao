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
    /// Interaction logic for expenses.xaml
    /// </summary>
    public partial class expenses : Window
    {
   
        public expenses()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (description.Text == "" || amount.Text == "")
            {
                MessageBox.Show("Incomplete details");
                return;
            }
            try
            {
                try { double.Parse(amount.Text); } catch (Exception ex) { MessageBox.Show(ex.Message);  return;  }

                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Save details to the system?", "Expense Details", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {

                    string query = "insert into expenses values ('',@description,@amount,@date) ";
                    String con = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                    MySqlConnection connect = new MySqlConnection(con);
                    connect.Open(); 
                    MySqlCommand cmd = new MySqlCommand(query, connect);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@description", description.Text);
                    cmd.Parameters.AddWithValue("@amount", amount.Text);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd") );
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Successfully Saved Data!", "Expense Details", MessageBoxButton.OK, MessageBoxImage.Information);

                    //to reset selected value
                    description.Text = "";
                    amount.Text = "";

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
