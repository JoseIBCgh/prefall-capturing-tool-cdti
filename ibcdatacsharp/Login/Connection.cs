using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.EntityFramework;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Diagnostics.Eventing.Reader;

namespace sign_in_dotnet_wpf;
public class Connection
{
    private MySqlConnection _connection;
    private MySqlConnectionStringBuilder _connectionStringBuilder;
    private string server;
    private uint port;
    private string user;
    private string pass;
    private string db;


    public Connection()
    {
        Initialize();
    }

    public void Initialize()
    {
        server = "srv.ibc.bio";
        port = 32772;
        user = "root";
        pass = "root";
        db = "prefall";

        _connectionStringBuilder = new MySqlConnectionStringBuilder();
        _connectionStringBuilder.Server = server;
        _connectionStringBuilder.Port = port;
        _connectionStringBuilder.Database = db;
        _connectionStringBuilder.UserID = user;
        _connectionStringBuilder.Password = pass;

        _connection = new MySqlConnection(_connectionStringBuilder.ToString());

       

    }

    public bool OpenConnection()
    {
        try
        {
            if (_connection.State == System.Data.ConnectionState.Closed)
            {
                _connection.Open();

            }
            System.Windows.MessageBox.Show($"Connection done");
            return true;

        }
        catch (MySqlException e)
        {
            switch (e.Number)
            {
                case 0: System.Windows.MessageBox.Show("Error" + e); break;
                case 1045: System.Windows.MessageBox.Show($"Error: {e.Message}"); break;
            }
            return false;
        }
    }

    public void CloseConnection()
    {
        this._connection.Close();
    }

    public MySqlConnection GetConnection()
    {
        return this._connection;
    }
}
