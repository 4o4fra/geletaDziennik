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

        private void SubmitGradeButton_Click(object sender, RoutedEventArgs e)
        {
            string grade = WarningTextBox.Text;

            if (string.IsNullOrEmpty(grade))
            {
                MessageBox.Show("Ocena jest wymagana.");
                return;
            }

            string query = @"
                INSERT INTO ocena (id_ucznia, id_przedmiotu, ocena)
                VALUES (@studentPesel, @przedmiotId, @grade)";

            try
            {
                using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@studentPesel", _studentPesel);
                    command.Parameters.AddWithValue("@przedmiotId", GetSubjectId(_teacherPesel));
                    command.Parameters.AddWithValue("@grade", grade);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Ocena została dodana.");
                LoadStudentGrades();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding grade: " + ex.Message);
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