using System;
using System.Data.SqlClient;
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
                            OknoNauczyciela oknoNauczyciela = new OknoNauczyciela(nauczycielId);
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