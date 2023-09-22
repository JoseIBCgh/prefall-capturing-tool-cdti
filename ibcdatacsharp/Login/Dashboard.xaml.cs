using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ibcdatacsharp.Login;
using ibcdatacsharp.UI;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace sign_in_dotnet_wpf;
/// <summary>
/// Interaction logic for Dashboard.xaml
/// </summary>
public partial class Dashboard : Window
{
    private Connection connection;
    private string username, password, sql;

    public Dashboard()
    {
        InitializeComponent();
        connection = new Connection();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
       
        connection.OpenConnection();
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        connection.CloseConnection();
    }

    private void Button_Click_2(object sender, RoutedEventArgs e)
    {
        this.username = user.Text;
        this.password = pass.Password.ToString();

        //sql = "SELECT username, password from users where username='" + this.username + "' AND " + "password='" + this.password +"'";
        sql = "SELECT password from users where username='" + this.username + "'";
        if (this.connection.OpenConnection() == true)
        {
            var command = new MySqlCommand(sql, this.connection.GetConnection());
            object a = command.ExecuteScalar();

            if (a != null)
            {
                string password = a.ToString();
                if (CheckPassword(this.password, password))
                {
                    sql = "SELECT roles.name " +
                        "FROM roles " +
                        "INNER JOIN roles_users ON roles.id = roles_users.role_id " +
                        "INNER JOIN users ON roles_users.user_id = users.id " +
                        "WHERE users.username = '" + username + "';";
                    command = new MySqlCommand(sql, this.connection.GetConnection());
                    a = command.ExecuteScalar();
                    Trace.WriteLine((string)a);
                    if ((string)a == "paciente")
                    {
                        MessageBox.Show("Pacientes no pueden ingresar");
                    }
                    else
                    {
                        MessageBox.Show("login successful!");

                        LoginInfo.nombre = username;
                        MainWindow mw = new MainWindow();
                        this.Close();
                        mw.Show();
                    }
                }
                else
                {
                    MessageBox.Show("incorrect password");
                }
            }
            else
            {
                MessageBox.Show("Error login");
                
            }
        }


    }
    static bool CheckPassword(string password, string hashPassword)
    {
        string salt = "146585145368132386173505678016728509634";
        byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

        using (HMACSHA512 hmac = new HMACSHA512(saltBytes))
        {
            byte[] hashBytes = hmac.ComputeHash(passwordBytes);
            string hashedPassword = Convert.ToBase64String(hashBytes);

            return BCrypt.Net.BCrypt.Verify(hashedPassword, hashPassword);
        }
    }
}
