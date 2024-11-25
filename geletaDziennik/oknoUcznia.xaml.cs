using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;

namespace geletaDziennik
{
    public partial class OknoUcznia : Window
    {
        private int _studentId;

        public OknoUcznia(int studentId)
        {
            _studentId = studentId;
            InitializeComponent();
            LoadStudentData();
            LoadStudentGrades();
        }

        private void LoadStudentData()
        {
            string query = @"
                SELECT pesel, imie, nazwisko, klasa_id, punkty 
                FROM uczen 
                WHERE pesel = @studentId";

            try
            {
                using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@studentId", _studentId);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    
                    if (reader.Read())
                    {
                        StudentData studentData     = new StudentData
                        {
                            Pesel = reader.GetInt32(0),
                            Imie = reader.GetString(1),
                            Nazwisko = reader.GetString(2),
                            KlasaId = reader.GetString(3),
                            Punkty = reader.GetInt32(4)
                        };
                        StudentInfoTextBlock.Text = $"PESEL: {studentData.Pesel}, Imię: {studentData.Imie}, Nazwisko: {studentData.Nazwisko}, Klasa ID: {studentData.KlasaId}, Punkty: {studentData.Punkty}";
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading student data: " + ex.Message);
            }
        }

        private void LoadStudentGrades()
        {
            string query = @"
                SELECT o.id, o.id_ucznia, o.id_przedmiotu, o.ocena, p.nazwa 
                FROM ocena o
                JOIN przedmiot p ON o.id_przedmiotu = p.id
                WHERE o.id_ucznia = @studentId";

            List<StudentGrade> studentGrades = new List<StudentGrade>();

            try
            {
                using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@studentId", _studentId);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        studentGrades.Add(new StudentGrade
                        {
                            Id = reader.GetInt32(0),
                            UczenId = reader.GetInt32(1),
                            PrzedmiotId = reader.GetInt32(2),
                            Ocena = reader.GetInt32(3),
                            NazwaPrzedmiotu = reader.GetString(4)
                        });
                    }

                    reader.Close();
                }

                StudentGradesDataGrid.ItemsSource = studentGrades;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading student grades: " + ex.Message);
            }
        }
    }

    public class StudentGrade
    {
        public int Id { get; set; }
        public int UczenId { get; set; }
        public int PrzedmiotId { get; set; }
        public int Ocena { get; set; }
        public string NazwaPrzedmiotu { get; set; }
    }

    public class StudentData
    {
        public int Pesel { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Haslo { get; set; }
        public string KlasaId { get; set; }
        public int Punkty { get; set; }
    }
}