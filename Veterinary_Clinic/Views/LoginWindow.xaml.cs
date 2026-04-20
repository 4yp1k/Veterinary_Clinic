using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using Veterinary_Clinic.Data;
using Veterinary_Clinic.Models;
using Xceed.Wpf.Toolkit;
using static System.Collections.Specialized.BitVector32;
using MessageBox = System.Windows.MessageBox;

namespace Veterinary_Clinic.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var conn = RepositoryHelper.GetConnection();
            conn.Open();

            var cmd = new SqlCommand(@"
        SELECT Id, Username, Role
        FROM Users
        WHERE Username=@u AND Password=@p", conn);

            cmd.Parameters.AddWithValue("@u", UsernameBox.Text);
            cmd.Parameters.AddWithValue("@p", PasswordBox.Password);

            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                var role = reader["Role"].ToString();

                if (role != "Admin")
                {
                    MessageBox.Show("Нет доступа администратора");
                    return;
                }

                Session.CurrentUser = new User
                {
                    Id = (int)reader["Id"],
                    Username = reader["Username"].ToString(),
                    Role = UserRole.Admin
                };

                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль");
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            WatermarkPasswordBox.Visibility = Visibility.Collapsed;
        }
    }
}
