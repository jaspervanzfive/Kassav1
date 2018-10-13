using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Input;
using CompleetKassa.Views.Modules.Login;

namespace CompleetKassa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //LoginView page = new LoginView();

            //FirstPage.Content = page;
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Right || e.Key == Key.Left || e.Key == Key.LeftAlt || e.Key == Key.RightAlt)
            {
                e.Handled = true;
            }
          

        }



        #region Login Events

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoginPage.Visibility = Visibility.Hidden;
        }

        private void PasswordForgotten_Event(object sender, MouseButtonEventArgs e)
        {
            EmailBorder.Visibility = Visibility.Visible;
        }

        private void BacktoLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            EmailBorder.Visibility = Visibility.Hidden;
            SecretQuestionPage.Visibility = Visibility.Hidden;
        }
        #endregion

        private void RevealPassEvent(object sender, MouseButtonEventArgs e)
        {
            RevealPasswordPage.Visibility = Visibility.Visible;
        }

        private void RevealPassPage(object sender, MouseButtonEventArgs e)
        {
            SecretQuestionPage.Visibility = Visibility.Visible;
        }

        private void Login_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LoginPage.Visibility = Visibility.Hidden;
        }
    }
}
