using System;
using System.Data.SqlClient;
using System.Runtime.Remoting.Messaging;
using System.Windows;

namespace geletaDziennik
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            ConnectToDatabase();
            InitializeComponent();
        }

        private void ConnectToDatabase()
        {
            string query = "SELECT COUNT(*) FROM klasa";

            try
            {
                using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine("Połączono z bazą danych");

                    SqlCommand command = new SqlCommand(query, connection);
                    int result = (int)command.ExecuteScalar();

                    Console.WriteLine("Liczba klas: " + result.ToString());
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool isStudent;
            try
            {
                using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT COUNT(PESEL) FROM uczen WHERE pesel = '" + pesel.Text + "'", connection);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        if (reader.GetInt32(0) == 0)
                        {
                            Console.WriteLine("Logowanie jako nauczyciel");
                            isStudent = false;
                        }
                        else
                        {
                            Console.WriteLine("Logowanie jako uczen");
                            isStudent = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Logowanie jako nauczyciel");
                        isStudent = false;
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return;
            }
            string query = "SELECT PESEL FROM " + (isStudent ? "uczen" : "nauczyciel") + " WHERE pesel = '" + pesel.Text + "' AND haslo = '" + password.Password + "'";
            bool isDirector = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM nauczyciel WHERE PESEL = " + pesel.Text + " AND dyrektor = 1", connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        isDirector = reader.GetInt32(0) == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    

                    if (reader.Read())
                    {
                        Console.WriteLine("Zalogowano");

                        if (!isStudent)
                        {
                            int nauczycielId = reader.GetInt32(0);
                            OknoNauczyciela oknoNauczyciela = new OknoNauczyciela(nauczycielId, isDirector);
                            oknoNauczyciela.Show();
                        }
                        else
                        {
                            int studentId = reader.GetInt32(0);
                            OknoUcznia oknoUcznia = new OknoUcznia(studentId);
                            oknoUcznia.Show();
                        }
                        Close();
                    }
                    else
                    {
                        Console.WriteLine("Niepoprawne dane logowania");
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
    }
}