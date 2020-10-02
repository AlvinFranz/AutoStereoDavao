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
using System.Windows.Threading;

namespace AutoStereoDavao_POS
{
    /// <summary>
    /// Interaction logic for load_screen.xaml
    /// </summary>
    public partial class load_screen : Window
    {
        DispatcherTimer dt = new DispatcherTimer();
      
        public load_screen()
        {
            InitializeComponent();

            dt.Tick += new EventHandler(Dt_Tick);
            dt.Interval = new TimeSpan(0, 0, 5);
            dt.Start();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
          
        }

        private void Dt_Tick(object sender, EventArgs e)
        {          
                MainWindow main = new MainWindow();
                main.Show();
                dt.Stop();
                this.Close();
            
        }
      
     
    }
}
