using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;

namespace geletaDziennik
{
    public partial class OknoNauczyciela : Window
    {
        private int _teacherId;
        private bool _isDirector;

        public OknoNauczyciela(int teacherId, bool isDirector)
        {
            _teacherId = teacherId;
            _isDirector = isDirector;
            InitializeComponent();
            LoadTeacherData();
            LoadTeacherStudents();
        }

        private void LoadTeacherData()
        {
            string query = @"
                SELECT PESEL, imie, nazwisko, sala 
                FROM nauczyciel 
                WHERE PESEL = @teacherId";

            try
            {
                using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@teacherId", _teacherId);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        TeacherData teacherData = new TeacherData
                        {
                            Pesel = reader.GetInt32(0),
                            Imie = reader.GetString(1),
                            Nazwisko = reader.GetString(2),
                            Sala = reader.GetString(3)
                        };
                        TeacherInfoTextBlock.Text = $"PESEL: {teacherData.Pesel}, Imię: {teacherData.Imie}, Nazwisko: {teacherData.Nazwisko}, Sala: {teacherData.Sala}";
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading teacher data: " + ex.Message);
            }
        }

        private void LoadTeacherStudents()
        {
            string query;
            if (_isDirector)
            {
                Console.WriteLine("DYREKTOR");
                query = @"
                    SELECT u.PESEL, u.imie, u.nazwisko, u.klasa_id, u.punkty 
                    FROM uczen u";
            }
            else
            {
                query = @"
                    SELECT u.PESEL, u.imie, u.nazwisko, u.klasa_id, u.punkty 
                    FROM uczen u
                    JOIN klasa k ON u.klasa_id = k.id
                    WHERE k.wychowawca_id = @teacherId";
            }

            List<StudentData> teacherStudents = new List<StudentData>();

            try
            {
                using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    if (!_isDirector)
                    {
                        command.Parameters.AddWithValue("@teacherId", _teacherId);
                    }

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        teacherStudents.Add(new StudentData
                        {
                            Pesel = reader.GetInt32(0),
                            Imie = reader.GetString(1),
                            Nazwisko = reader.GetString(2),
                            KlasaId = reader.GetString(3),
                            Punkty = reader.GetInt32(4)
                        });
                    }

                    reader.Close();
                }

                TeacherStudentsDataGrid.ItemsSource = teacherStudents;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading teacher students: " + ex.Message);
            }
        }

        private void AddGradeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (TeacherStudentsDataGrid.SelectedItem is StudentData selectedStudent)
            {
                AddGradeWindow addGradeWindow = new AddGradeWindow(selectedStudent.Pesel, _teacherId);
                addGradeWindow.ShowDialog();
                LoadTeacherStudents();
            }
            else
            {
                MessageBox.Show("Proszę wybrać ucznia.");
            }
        }

        private void AddWarningMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (TeacherStudentsDataGrid.SelectedItem is StudentData selectedStudent)
            {
                AddWarningWindow addWarningWindow = new AddWarningWindow(selectedStudent.Pesel, _teacherId);
                addWarningWindow.ShowDialog();
                LoadTeacherStudents();
            }
            else
            {
                MessageBox.Show("Proszę wybrać ucznia.");
            }
        }

        private class StudentData
        {
            public int Pesel { get; set; }
            public string Imie { get; set; }
            public string Nazwisko { get; set; }
            public string KlasaId { get; set; }
            public int Punkty { get; set; }
        }
    }

    public class TeacherData
    {
        public int Pesel { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Sala { get; set; }
    }
}