using dbLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace DBD
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public User CurrentUser { get; set; }
        public ObservableCollection<Post> NewsFeed { get; set; }

        public MainWindow(User user)
        {
            CurrentUser = user;
            InitializeComponent();
            LoadNews();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ImageBrush myBrush = new ImageBrush();
            myBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/bg.png", UriKind.Absolute));
            Background = myBrush;
        }

        private void LoadNews()
        {
            try
            {
                using (var db = new MyContext())
                {
                    var list = db.Posts.ToList();
                    NewsFeed = new ObservableCollection<Post>(list.OrderByDescending(g => g.PostDate).ToList());
                }
                DashBoard.ItemsSource = NewsFeed;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK);
            }
        }

        private void Button_Loaded(object sender, RoutedEventArgs e)
        {
            addButton.Visibility = CurrentUser.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
