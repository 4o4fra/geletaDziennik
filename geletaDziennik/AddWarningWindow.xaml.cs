using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;

namespace geletaDziennik
{
    public partial class AddWarningWindow : Window
    {
        private int _studentPesel;
        private int _teacherPesel;

        public AddWarningWindow(int studentPesel, int teacherPesel)
        {
            _studentPesel = studentPesel;
            _teacherPesel = teacherPesel;
            InitializeComponent();
            LoadStudentGrades();
        }

        private int GetSubjectId(int teacherPesel)
        {
            string query = @"
                SELECT Id
                FROM przedmiot
                WHERE nauczyciel_id = @teacherPesel";

            try
            {
                using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@teacherPesel", teacherPesel);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }
                    return -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting subject id: " + ex.Message);
                return -1;
            }
        }

        private void SubmitPointsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(WarningTextBox.Text, out int points))
            {
                MessageBox.Show("Proszę wpisać poprawną liczbę punktów.");
                return;
            }

            string query = @"
        UPDATE uczen
        SET punkty = punkty + @points
        WHERE PESEL = @studentPesel";

            try
            {
                using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@points", points);
                    command.Parameters.AddWithValue("@studentPesel", _studentPesel);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Punkty zostały zaktualizowane.");
                LoadStudentGrades();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating points: " + ex.Message);
            }
        }

        private void LoadStudentGrades()
        {
            string query = @"
                SELECT PESEL, imie, nazwisko, punkty 
                FROM uczen
                WHERE PESEL = @studentPesel";

            List<StudentWarnings> studentWarnings = new List<StudentWarnings>();

            try
            {
                using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@studentPesel", _studentPesel);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        studentWarnings.Add(new StudentWarnings
                        {
                            Pesel = reader.GetInt32(0),
                            Imie = reader.GetString(1),
                            Nazwisko = reader.GetString(2),
                            Punkty = reader.GetInt32(3)
                        });
                    }

                    reader.Close();
                }

                WarningsDataGrid.ItemsSource = studentWarnings;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading student grades: " + ex.Message);
            }
        }
        private class StudentWarnings
        {
            public int Pesel { get; set; }
            public string Imie { get; set; }
            public string Nazwisko { get; set; }
            public int Punkty { get; set; }
        }
    }
}