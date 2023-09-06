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
using System.Windows.Navigation;
using System.Windows.Shapes;
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

        sql = "SELECT username, password from users where username='" + this.username + "' AND " + "password='" + this.password +"'";

        if (this.connection.OpenConnection() == true)
        {
            var command = new MySqlCommand(sql, this.connection.GetConnection());
            object a = command.ExecuteScalar();

            if (a != null)
            {
                MessageBox.Show("login successful!");

                MainWindow mw = new MainWindow();
                this.Close();
                mw.Show();

            }
            else
            {
                MessageBox.Show("Error login");
                
            }
        }


    }
}
