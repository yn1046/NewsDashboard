using dbLib;
using Microsoft.Win32;
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

namespace DBD
{
    /// <summary>
    /// Логика взаимодействия для AddNewsWindow.xaml
    /// </summary>
    public partial class AddNewsWindow : Window
    {
        public MainWindow MyMainWindow { get; set; }
        
        public string ImageUri { get; set; }

        public AddNewsWindow(MainWindow mainWin)
        {
            MyMainWindow = mainWin;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ImageBrush myBrush = new ImageBrush();
            myBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/bg.png", UriKind.Absolute));
            Background = myBrush;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Title = "Выбор изображения";
            openDialog.Filter = "Все поддерживаемые типы|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";

            if (openDialog.ShowDialog() == true)
            {
                ImageUri = openDialog.FileName;
                fileNameLabel.Content = ImageUri;
            }
            else
            {
                ImageUri = string.Empty;
                fileNameLabel.Content = "Не выбран...";
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(titleBox.Text)) throw new Exception("Заголовок не может быть пустым.");
                if (string.IsNullOrEmpty(ImageUri)) throw new Exception("Выберите изображение.");
                if (string.IsNullOrEmpty(textBox.Text)) throw new Exception("Текст не может быть пустым.");
                if (titleBox.Text.Length > 50) throw new Exception("Длина заголовка не может превышать 50 символов.");
                if (ImageUri.Length > 200) throw new Exception("Длина пути к файлу не может превышать 200 символов.");
                if (textBox.Text.Length > 4000) throw new Exception("Длина текста не может превышать 4000 символов.");

                var category = categoryBox.SelectedItem as ComboBoxItem;
                if (category == null)
                {
                    category = new ComboBoxItem();
                    category.Content = string.Empty;
                }

                var post = new Post()
                {
                    Title = titleBox.Text,
                    ImagePath = ImageUri,
                    Text = textBox.Text,
                    Category = category.Content.ToString(),
                    PostDate = DateTime.Now
                };

                using (var db = new MyContext())
                {
                    if (db.Posts.Any(p => p.Title.Equals(post.Title))) throw new Exception("Пост с таким названием уже существует.");
                    db.Posts.Add(post);
                    db.SaveChanges();
                }

                MyMainWindow.LoadNews();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK);
                return;
            }
        }
    }
}
