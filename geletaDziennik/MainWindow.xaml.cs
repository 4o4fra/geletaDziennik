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
            bool isStudent = uczen.IsChecked == true;
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
                            throw new NotImplementedException();
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