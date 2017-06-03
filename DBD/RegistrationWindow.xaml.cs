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
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (loginBox.Text.Equals(string.Empty) || passBox.Password.Equals(string.Empty) || confirmBox.Password.Equals(string.Empty))
                    throw new Exception("Заполните все поля.");
                if (loginBox.Text.Length > 50 || passBox.Password.Length > 50 || confirmBox.Password.Length > 50)
                    throw new Exception("Длина полей не может превышать 50 символов.");
                if (!passBox.Password.Equals(confirmBox.Password))
                    throw new Exception("Пароли должны совпадать.");
                using (var db = new MyContext())
                {
                    var existing = db.Users.Where(u => u.Login.Equals(loginBox.Text));
                    if (existing.Count() > 0) throw new Exception("Пользователь с таким именем уже существует.");
                    else
                    {
                        var salt = GetUniqueKey();

                        var md5Password = GetHashedPassword(salt);

                        var user = new User()
                        {
                            Login = loginBox.Text,
                            Password = md5Password,
                            Salt = salt,
                            IsAdmin = false
                        };

                        db.Users.Add(user);
                        db.SaveChanges();
                        var str = $"Пользователь {user.Login} успешно добавлен.";
                        MessageBox.Show(str, "Результат", MessageBoxButton.OK);                        
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
            Close();
        }

        private string GetUniqueKey()
        {
            char[] chars = new char[62];
            chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[15];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(15);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
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
