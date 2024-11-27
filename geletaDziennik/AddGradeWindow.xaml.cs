using System;
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
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding grade: " + ex.Message);
            }
        }
    }
}