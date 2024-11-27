using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;

namespace geletaDziennik
{
    public partial class AddGradeWindow : Window
    {
        private int _studentPesel;
        private int _teacherPesel;

        public AddGradeWindow(int studentPesel, int teacherPesel)
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
            string grade = GradeTextBox.Text;

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
                SELECT o.id, o.id_ucznia, o.id_przedmiotu, o.ocena, p.nazwa 
                FROM ocena o
                JOIN przedmiot p ON o.id_przedmiotu = p.id
                WHERE o.id_ucznia = @studentPesel";

            List<StudentGrade> studentGrades = new List<StudentGrade>();

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
                        studentGrades.Add(new StudentGrade
                        {
                            Id = reader.GetInt32(0),
                            UczenId = reader.GetInt32(1),
                            PrzedmiotId = reader.GetInt32(2),
                            Ocena = reader.GetDouble(3),
                            NazwaPrzedmiotu = reader.GetString(4)
                        });
                    }

                    reader.Close();
                }

                GradesDataGrid.ItemsSource = studentGrades;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading student grades: " + ex.Message);
            }
        }
    }
}