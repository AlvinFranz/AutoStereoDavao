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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutoStereoDavao_POS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        public MainWindow()
        {
            InitializeComponent();
            //dashboard dash = new dashboard();
            //stack.Children.Clear();
            //stack.Children.Add(dash);
        }
        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            //image_logo.Visibility = Visibility.Visible; 
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
            //image_logo.Visibility = Visibility.Collapsed;
        }

        private void inventory_tab(object sender, System.EventArgs e)
        {

            Inventory inventory = new Inventory();
            stack.Children.Clear();
            stack.Children.Add(inventory);

        }

        

        private void Purchasing_Selected(object sender, RoutedEventArgs e)
        {
            transaction purchase = new transaction();
            stack.Children.Clear();
            stack.Children.Add(purchase);
        }

        private void Reservation_Selected(object sender, RoutedEventArgs e)
        {
            reservation reserve = new reservation();
            stack.Children.Clear();
            stack.Children.Add(reserve);
        }

        private void report_Selected(object sender, RoutedEventArgs e)
        {
            sales_record sales = new sales_record();
            stack.Children.Clear();
            stack.Children.Add(sales);
        }

        private void trackOrder_Selected(object sender, RoutedEventArgs e)
        {
            track_orderTab track = new track_orderTab();
            stack.Children.Clear();
            stack.Children.Add(track);
        }
        private void ranking_Selected(object sender, RoutedEventArgs e)
        {
            reseller_record rank = new reseller_record();
            stack.Children.Clear();
            stack.Children.Add(rank);
        }

        private void resellers_Selected(object sender, RoutedEventArgs e)
        {
            retailer_profile reseller = new retailer_profile();
            stack.Children.Clear();
            stack.Children.Add(reseller);
        }

        private void expenses_Selected(object sender, RoutedEventArgs e)
        {
            track_expenseTab expense = new track_expenseTab();
            stack.Children.Clear();
            stack.Children.Add(expense);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            database data = new database();
            data.ShowDialog();
        }

        private void Dashboard_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void dashboard_tab(object sender, RoutedEventArgs e)
        {
            dashboard dash = new dashboard();
            stack.Children.Clear();
            stack.Children.Add(dash);
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
