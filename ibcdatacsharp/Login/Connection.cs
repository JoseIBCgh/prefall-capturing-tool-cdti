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
using System.Data.Entity;

namespace sign_in_dotnet_wpf;
/// <summary>
/// Wrapper de la conexion con la base de datos
/// </summary>
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
    /// <summary>
    /// Inicializa la canexion con la base de datos (los parametros estan dentro de la funcion)
    /// </summary>
    public void Initialize()
    {
        server = "srv.ibc.bio";
        port = 32817;
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
    /// <summary>
    /// Abre la conexion.
    /// </summary>
    /// <returns>true si se ha abierto, falso si no</returns>
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
    /// <summary>
    /// Cierra la conexion.
    /// </summary>
    public void CloseConnection()
    {
        this._connection.Close();
    }
    /// <summary>
    /// Devuelve la conexion
    /// </summary>
    /// <returns>La conexion</returns>
    public MySqlConnection GetConnection()
    {
        return this._connection;
    }
}
