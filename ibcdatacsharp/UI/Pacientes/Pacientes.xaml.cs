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
using Microsoft.Win32;

namespace ibcdatacsharp.UI.Pacientes
{
    /// <summary>
    /// Lógica de interacción para Pacientes.xaml
    /// </summary>
    public partial class Pacientes : Page
    {
        private Connection connection;
        public ObservableCollection<Medico> medico
        {
            get;
            set;
        }
        public ObservableCollection<CentroRoot> centro
        {
            get;
            set;
        }
        public Pacientes()
        {
            InitializeComponent();
            connection = new Connection();
            DataContext = this;
            this.medico = new ObservableCollection<Medico>();
            this.centro = new ObservableCollection<CentroRoot>();
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
                    Medico medico = new Medico(LoginInfo.nombre);
                    sql = "SELECT c.nombreFiscal, GROUP_CONCAT(u_paciente.username) AS pacientes, GROUP_CONCAT(u_paciente.id) AS ids " +
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
                            string ids = reader.GetString("ids");
                            Centro centro = new Centro(nombreFiscal);
                            string[] pacientesArray = pacientes.Split(',');
                            string[] idsArray = ids.Split(',');
                            for (int i = 0; i < pacientesArray.Count(); i++)
                            {
                                string p = pacientesArray[i];
                                string id = idsArray[i];
                                Paciente paciente = new Paciente(p.Trim(), int.Parse(id.Trim()));
                                centro.Pacientes.Add(paciente);
                            }
                            medico.Centros.Add(centro);
                        }
                    }
                    reader.Close();
                    this.medico.Add(medico);
                    treeMedico.Visibility = Visibility.Visible;
                    treeAuxiliar.Visibility = Visibility.Collapsed;
                }
                else if((string)a == "auxiliar")
                {
                    Auxiliar auxiliar = new Auxiliar(LoginInfo.nombre);
                    sql = "SELECT centros.nombreFiscal " +
                        "FROM centros " +
                        "INNER JOIN users ON centros.id = users.id_centro " +
                        "WHERE users.username = '" + LoginInfo.nombre + "';";
                    command = new MySqlCommand(sql, this.connection.GetConnection());
                    a = command.ExecuteScalar();
                    CentroRoot centro = new CentroRoot((string)a);
                    sql = "SELECT users.username, users.id " +
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
                        if (!reader.IsDBNull(0) && !reader.IsDBNull(1))
                        {
                            Paciente paciente = new Paciente(reader.GetString(0), reader.GetInt32(1));
                            auxiliar.Pacientes.Add(paciente);
                        }
                    }
                    reader.Close();
                    centro.Auxiliar.Add(auxiliar);
                    this.centro.Add(centro);
                    treeAuxiliar.Visibility = Visibility.Visible;
                    treeMedico.Visibility = Visibility.Collapsed;
                }
            }
            ((MainWindow)Application.Current.MainWindow).fileSaver.filesAdded += (s, id, files) =>
            {
                Paciente? paciente = Find(id);
                if(paciente != null)
                {
                    foreach (var file in files)
                    {
                        Test test = new Test(file);
                        paciente.Tests.Add(test);
                    }
                    MessageRecorded message = new MessageRecorded(paciente.Nombre, id, files);
                    message.Show();
                }
                else
                {
                    MessageBox.Show("No se ha encontrado al paciente");
                }
            };
        }
        private Paciente? Find(int id)
        {
            if (this.medico.Count > 0)
            {
                Medico medico = this.medico[0];
                foreach (Centro centro in medico.Centros)
                {
                    foreach (Paciente paciente in centro.Pacientes)
                    {
                        if(paciente.Id == id)
                        {
                            return paciente;
                        }
                    }
                }
            }
            if(this.centro.Count > 0)
            {
                CentroRoot centroRoot = this.centro[0];
                Auxiliar auxiliar = centroRoot.Auxiliar[0];
                foreach(Paciente paciente in auxiliar.Pacientes)
                {
                    if(paciente.Id == id)
                    {
                        return paciente;
                    }
                }
            }
            return null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = Config.INITIAL_PATH;

            openFileDialog.Filter = "Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                RemoteTransactions.SubirTest(selectedFilePath);
                MessageBox.Show("Test subido");
            }

        }
    }
}
