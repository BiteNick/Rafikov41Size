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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Rafikov41Size
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        DispatcherTimer timer;
        public AuthPage()
        {
            InitializeComponent();
        }

        private void enterBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = loginTB.Text;
            string password = passwordTB.Text;
            if (login == "" || password == "")
            {
                MessageBox.Show("Есть пустые поля");
                return;
            }

            User user = Rafikov41Entities.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);
            if (user != null)
            {
                Manager.MainFrame.Navigate(new ProductPage(user));
                loginTB.Text = "";
                passwordTB.Text = "";
            }
            else
            {
                enterBtn.IsEnabled = false;
                MessageBox.Show("Введены неверные данные");

                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(10);
                timer.Tick += TimerTick;
                timer.Start();
            }
        }

        void TimerTick(object sender, EventArgs a)
        {
            enterBtn.IsEnabled = true;
            timer.Stop();
        }

        private void guestBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new ProductPage(null));
            loginTB.Text = "";
            passwordTB.Text = "";
        }
    }
}
