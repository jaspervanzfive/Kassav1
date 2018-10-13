using System.Windows.Controls;
using System.Windows;
namespace CompleetKassa.Views
{
    /// <summary>
    /// Interaction logic for ShoesView.xaml
    /// </summary>
    public partial class SalesView : UserControl
    {
        public SalesView()
        {
            InitializeComponent();
		}

        private void UserControl_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
         
        }

        private void Numpad_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (NumpadPage.Visibility == Visibility.Hidden)
                NumpadPage.Visibility = Visibility.Visible;
            else
                NumpadPage.Visibility = Visibility.Hidden;
        }

        private void Quantity_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (fold.Visibility == Visibility.Collapsed)
                fold.Visibility = Visibility.Visible;
            else
                fold.Visibility = Visibility.Collapsed;

        }
    }
}
