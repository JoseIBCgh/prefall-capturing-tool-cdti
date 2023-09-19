using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using sign_in_dotnet_wpf;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ibcdatacsharp.UI.Pacientes
{
    public static class RemoteTransactions
    {
        private static int NUM_TEST = 24;
        public static void SubirTest(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        string firstLine = reader.ReadLine();

                        dynamic jsonData = JsonConvert.DeserializeObject(firstLine);

                        int id = jsonData.id;

                        for (int i = 0; i < 5; i++)
                        {
                            reader.ReadLine();
                        }

                        Connection connection = new Connection();
                        connection.OpenConnection();
                        using (var transaction = connection.GetConnection().BeginTransaction())
                        {
                            try
                            {

                                string sql = "SELECT num_test FROM test WHERE id_paciente = @id_paciente ORDER BY num_test DESC LIMIT 1;";

                                using (MySqlCommand command = new MySqlCommand(sql, connection.GetConnection()))
                                {
                                    command.Parameters.AddWithValue("@id_paciente", id); // Replace patientId with the actual patient's ID.

                                    object result = command.ExecuteScalar();
                                    int nextNumTest;

                                    if (result != null)
                                    {
                                        int lastNumTest = Convert.ToInt32(result);
                                        nextNumTest = lastNumTest + 1;
                                    }
                                    else
                                    {
                                        nextNumTest = 0;
                                    }
                                    sql = "INSERT INTO test (num_test, id_paciente, id_centro, date, model, probabilidad_caida, data) VALUES (@num_test ,@id_paciente, @id_centro, @date, NULL, NULL, NULL);";

                                    using (MySqlCommand insertCommand = new MySqlCommand(sql, connection.GetConnection()))
                                    {
                                        insertCommand.Parameters.AddWithValue("@num_test", nextNumTest);
                                        insertCommand.Parameters.AddWithValue("@id_paciente", id);
                                        insertCommand.Parameters.AddWithValue("@id_centro", 1);
                                        DateTime date = ExtractDate(Path.GetFileNameWithoutExtension(path));
                                        insertCommand.Parameters.AddWithValue("@date", date);
                                        // Execute the INSERT statement within the transaction
                                        insertCommand.ExecuteNonQuery();
                                    }
                                    string line;
                                    string tableName = "test_unit";
                                    int item = 0;
                                    StringBuilder bulkInsertSql = new StringBuilder();
                                    bulkInsertSql.Append($"INSERT INTO {tableName} (num_test, id_paciente, item, time, acc_x, acc_y, acc_z, gyr_x, gyr_y, gyr_z, mag_x, mag_y, mag_z, lacc_x, lacc_y, lacc_z, quat_x, quat_y, quat_z, quat_w) VALUES ");

                                    while ((line = reader.ReadLine()) != null)
                                    {
                                        string[] data = line.Split(' ');

                                        // Append values for each row
                                        bulkInsertSql.Append($"({nextNumTest}, {id}, {data[2]}, '{data[1]}', '{data[3]}', '{data[4]}', '{data[5]}', '{data[6]}', '{data[7]}', '{data[8]}', '{data[9]}', '{data[10]}', '{data[11]}', '{data[12]}', '{data[13]}', '{data[14]}', '{data[15]}', '{data[16]}', '{data[17]}', '{data[18]}'),");
                                        item++;
                                    }

                                    // Remove the trailing comma
                                    bulkInsertSql.Length -= 1;

                                    using (MySqlCommand bulkInsertCommand = new MySqlCommand(bulkInsertSql.ToString(), connection.GetConnection()))
                                    {
                                        // Execute the bulk insert
                                        bulkInsertCommand.ExecuteNonQuery();
                                    }

                                    // Commit the transaction
                                    transaction.Commit();
                                }
                            }
                            catch (Exception ex)
                            {
                                // Handle exceptions and possibly rollback the transaction
                                transaction.Rollback();
                                Console.WriteLine("An error occurred: " + ex.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
        public static DateTime ExtractDate(string filename)
        {
            string format = "yyyyMMdd-HH-mm-ss-fff";
            return DateTime.ParseExact(filename, format, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }
    }
}
