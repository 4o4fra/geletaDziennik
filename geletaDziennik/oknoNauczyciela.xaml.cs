using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;

namespace geletaDziennik
{
    public partial class OknoNauczyciela : Window
    {
        private int _teacherId;

        public OknoNauczyciela(int teacherId)
        {
            _teacherId = teacherId;
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
            string query = @"
                SELECT u.PESEL, u.imie, u.nazwisko, u.klasa_id, u.punkty 
                FROM uczen u
                JOIN klasa k ON u.klasa_id = k.id
                WHERE k.wychowawca_id = @teacherId";

            List<StudentData> teacherStudents = new List<StudentData>();

            try
            {
                using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@teacherId", _teacherId);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        teacherStudents.Add(new StudentData
                        {
                            Pesel = reader.GetInt32(0),
                            Imie = reader.GetString(1),
                            Nazwisko = reader.GetString(2),
                            KlasaId = reader.GetString(3).Trim(),
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
    }

    public class TeacherData
    {
        public int Pesel { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Sala { get; set; }
    }
}