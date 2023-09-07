using AvalonDock.Themes;
using ibcdatacsharp.Login;
using ibcdatacsharp.UI.Pacientes.Models;
using MySql.Data.MySqlClient;
using sign_in_dotnet_wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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

namespace ibcdatacsharp.UI.Pacientes
{
    /// <summary>
    /// Lógica de interacción para Pacientes.xaml
    /// </summary>
    public partial class Pacientes : Page
    {
        private Connection connection;
        public ObservableCollection<User> user
        {
            get;
            set;
        }
        public Pacientes()
        {
            InitializeComponent();
            connection = new Connection();
            DataContext = this;
            this.user = new ObservableCollection<User>();
            User user = new User(LoginInfo.nombre);
            string sql = "SELECT roles.name " +
                "FROM roles " +
                "INNER JOIN roles_users ON roles.id = roles_users.role_id " +
                "INNER JOIN users ON roles_users.user_id = users.id " +
                "WHERE users.username = '" + LoginInfo.nombre + "';";
            if (this.connection.OpenConnection() == true)
            {
                var command = new MySqlCommand(sql, this.connection.GetConnection());
                object a = command.ExecuteScalar();
                if((string)a == "medico")
                {
                    sql = "SELECT c.nombreFiscal, GROUP_CONCAT(u_paciente.username) AS pacientes " +
    "FROM centros c " +
    "LEFT JOIN users u_paciente ON c.id = u_paciente.id_centro " +
    "LEFT JOIN pacientes_asociados pa ON u_paciente.id = pa.id_paciente " +
    "LEFT JOIN users u_doctor ON pa.id_medico = u_doctor.id " +
    "WHERE u_doctor.username = @DoctorUsername " +
    "GROUP BY c.nombreFiscal";
                    command = new MySqlCommand(sql, this.connection.GetConnection());
                    command.Parameters.AddWithValue("@DoctorUsername", LoginInfo.nombre);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Trace.WriteLine("row");
                        if (!reader.IsDBNull(0))
                        {
                            string nombreFiscal = reader.GetString("nombreFiscal");
                            string pacientes = reader.GetString("pacientes");
                            Centro centro = new Centro(nombreFiscal);
                            string[] pacientesArray = pacientes.Split(',');
                            foreach (string p in pacientesArray)
                            {
                                Paciente paciente = new Paciente(p.Trim());
                                centro.Pacientes.Add(paciente);
                            }
                            user.Centros.Add(centro);
                        }
                    }
                    reader.Close();
                }
                else if((string)a == "auxiliar")
                {
                    sql = "SELECT centros.nombreFiscal " +
                        "FROM centros " +
                        "INNER JOIN users ON centros.id = users.id_centro " +
                        "WHERE users.username = '" + LoginInfo.nombre + "';";
                    command = new MySqlCommand(sql, this.connection.GetConnection());
                    a = command.ExecuteScalar();
                    Centro centro = new Centro((string)a);
                    sql = "SELECT users.username " +
                        "FROM users " +
                        "INNER JOIN centros ON users.id_centro = centros.id " +
                        "INNER JOIN roles_users ON users.id = roles_users.user_id " +
                        "INNER JOIN roles ON roles_users.role_id = roles.id " +
                        "WHERE centros.nombreFiscal = @CentroNombre " +
                        "AND roles.name = @RoleName;";
                    command = new MySqlCommand(sql, this.connection.GetConnection());
                    command.Parameters.AddWithValue("@CentroNombre", (string)a);
                    command.Parameters.AddWithValue("@RoleName", "paciente");
                    MySqlDataReader reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            Paciente paciente = new Paciente(reader.GetString(0));
                            centro.Pacientes.Add(paciente);
                        }
                    }
                    reader.Close();
                    user.Centros.Add(centro);
                }
            }
            this.user.Add(user);
        }
    }
}
