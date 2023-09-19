using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using sign_in_dotnet_wpf;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;

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
                                    int item = 0;
                                    while ((line = reader.ReadLine()) != null)
                                    {
                                        string[] data = line.Split(' ');
                                        string tableName = "test_unit";

                                        sql = $"INSERT INTO {tableName} (num_test, id_paciente, item, time, acc_x, acc_y, acc_z, gyr_x, gyr_y, gyr_z, mag_x, mag_y, mag_z, lacc_x, lacc_y, lacc_z, quat_x, quat_y, quat_z, quat_w) " +
                                                $"VALUES (@num_test, @id, @item, @time, @acc_x, @acc_y, @acc_z, @gyr_x, @gyr_y, @gyr_z, @mag_x, @mag_y, @mag_z, @lacc_x, @lacc_y, @lacc_z, @quat_x, @quat_y, @quat_z, @quat_w)";
                                        using (MySqlCommand insertRowcommand = new MySqlCommand(sql, connection.GetConnection()))
                                        {
                                            insertRowcommand.Parameters.AddWithValue("@num_test", nextNumTest);
                                            insertRowcommand.Parameters.AddWithValue("@id", id);
                                            insertRowcommand.Parameters.AddWithValue("@item", item);
                                            insertRowcommand.Parameters.AddWithValue("@time", data[1]);
                                            insertRowcommand.Parameters.AddWithValue("@acc_x", data[3]);
                                            insertRowcommand.Parameters.AddWithValue("@acc_y", data[4]);
                                            insertRowcommand.Parameters.AddWithValue("@acc_z", data[5]);
                                            insertRowcommand.Parameters.AddWithValue("@gyr_x", data[6]);
                                            insertRowcommand.Parameters.AddWithValue("@gyr_y", data[7]);
                                            insertRowcommand.Parameters.AddWithValue("@gyr_z", data[8]);
                                            insertRowcommand.Parameters.AddWithValue("@mag_x", data[9]);
                                            insertRowcommand.Parameters.AddWithValue("@mag_y", data[10]);
                                            insertRowcommand.Parameters.AddWithValue("@mag_z", data[11]);
                                            insertRowcommand.Parameters.AddWithValue("@lacc_x", data[12]);
                                            insertRowcommand.Parameters.AddWithValue("@lacc_y", data[13]);
                                            insertRowcommand.Parameters.AddWithValue("@lacc_z", data[14]);
                                            insertRowcommand.Parameters.AddWithValue("@quat_x", data[15]);
                                            insertRowcommand.Parameters.AddWithValue("@quat_y", data[16]);
                                            insertRowcommand.Parameters.AddWithValue("@quat_z", data[17]);
                                            insertRowcommand.Parameters.AddWithValue("@quat_w", data[18]);

                                            insertRowcommand.ExecuteNonQuery();
                                            item++;
                                        }
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
