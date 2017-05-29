using dbLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Логика взаимодействия для LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        public ICommand EnterCommand { get; set; }

        public LogInWindow()
        {
            EnterCommand = new DelegateCommand(DoEnter);
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DoEnter();
        }

        private void DoEnter()
        {
            try
            {
                if (loginBox.Text.Equals(string.Empty) || passBox.Password.Equals(string.Empty))
                    throw new Exception("Заполните все поля.");
                using (var db = new MyContext())
                {
                    if (!db.Users.Any(u => u.Login.Equals(loginBox.Text)))
                        throw new Exception("Пользователя с таким логином не существует.");
                    else
                    {
                        var user = db.Users.FirstOrDefault(u => u.Login.Equals(loginBox.Text));
                        var password = GetHashedPassword(user.Salt);
                        if (!user.Password.Equals(password))
                            throw new Exception("Неверный пароль.");
                        else
                        {
                            var main = new MainWindow(user);
                            main.Show();
                        }
                    }
                }
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK);
                return;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var registration = new RegistrationWindow();
            registration.Show();
        }

        private string GetHashedPassword(string salt)
        {
            var saltBytes = Encoding.UTF8.GetBytes(salt);
            var passwordBytes = Encoding.UTF8.GetBytes(passBox.Password);

            var hmacMD5 = new HMACMD5(saltBytes);
            var saltedPasswordHash = hmacMD5.ComputeHash(passwordBytes);

            var result = string.Empty;
            foreach (var b in saltedPasswordHash)
            {
                result += b.ToString("x2");
            }
            return result;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ImageBrush myBrush = new ImageBrush();
            myBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/bg.png", UriKind.Absolute));
            Background = myBrush;
        }
    }
}
