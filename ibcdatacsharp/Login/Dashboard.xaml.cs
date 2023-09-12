using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using ibcdatacsharp.Login;
using ibcdatacsharp.UI;
using MySql.Data.MySqlClient;

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
        sql = "SELECT username, password from users where username='" + this.username + "'";
        if (this.connection.OpenConnection() == true)
        {
            var command = new MySqlCommand(sql, this.connection.GetConnection());
            object a = command.ExecuteScalar();

            if (a != null)
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
                MessageBox.Show("Error login");
                
            }
        }


    }
}
