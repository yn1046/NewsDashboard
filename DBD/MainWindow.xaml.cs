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

        public void LoadNews()
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

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            var addNewsWindow = new AddNewsWindow(this);
            addNewsWindow.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var post = DashBoard.SelectedItem as Post;
                if (string.IsNullOrEmpty(post.Title)) throw new Exception("Необходимо выбрать новость.");

                using (var db = new MyContext())
                {
                    if (db.Favourites.Any(f => f.UserId == CurrentUser.Id && f.PostId == post.Id))
                        throw new Exception("Вы уже добавили эту новость в избранное.");

                    var favourite = new Favourite()
                    {
                        UserId = CurrentUser.Id,
                        PostId = post.Id
                    };

                    db.Favourites.Add(favourite);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK);
            }

        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            var selection = listView.SelectedItem as ListViewItem;
            if (selection.Content.Equals("Избранное"))
            {
                try
                {
                    using (var db = new MyContext())
                    {
                        var favourites = db.Favourites.Where(f => f.UserId == CurrentUser.Id);
                        var list = db.Posts.Where(p => favourites.Any(f => f.PostId == p.Id)).ToList();
                        NewsFeed = new ObservableCollection<Post>(list.OrderByDescending(g => g.PostDate).ToList());
                    }
                    DashBoard.ItemsSource = NewsFeed;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK);
                }
            }
            else LoadNews();
        }

        private void ListView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            var selection = listView.SelectedItem as ListViewItem;
            try
            {
                using (var db = new MyContext())
                {
                    var list = db.Posts.Where(p => p.Category.Equals(selection.Content.ToString())).ToList();
                    NewsFeed = new ObservableCollection<Post>(list.OrderByDescending(g => g.PostDate).ToList());
                }
                DashBoard.ItemsSource = NewsFeed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK);
            }

        }
    }
}
