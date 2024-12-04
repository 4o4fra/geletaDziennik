using System;
using System.Data.SqlClient;
using System.Windows;

namespace geletaDziennik
{
    public partial class AddStudentWindow : Window
    {
        public AddStudentWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string pesel = PeselTextBox.Text;
            string imie = ImieTextBox.Text;
            string nazwisko = NazwiskoTextBox.Text;
            string klasaId = KlasaIdTextBox.Text;

            string query = @"
                INSERT INTO uczen (PESEL, imie, nazwisko, klasa_id, haslo, punkty)
                VALUES (@Pesel, @Imie, @Nazwisko, @KlasaId, @Haslo, @Punkty)";

            try
            {
                using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Pesel", pesel);
                    command.Parameters.AddWithValue("@Imie", imie);
                    command.Parameters.AddWithValue("@Nazwisko", nazwisko);
                    command.Parameters.AddWithValue("@KlasaId", klasaId);
                    command.Parameters.AddWithValue("@Haslo", "123");
                    command.Parameters.AddWithValue("@Punkty", 250);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Uczeń został dodany pomyślnie.");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas dodawania ucznia: " + ex.Message);
            }
        }
    }
}