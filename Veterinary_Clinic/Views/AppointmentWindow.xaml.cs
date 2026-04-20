using LiveChartsCore.SkiaSharpView.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Veterinary_Clinic.Data;
using Veterinary_Clinic.Models;

namespace Veterinary_Clinic
{
    public partial class AppointmentWindow : Window
    {
        public Appointment Appointment { get; private set; }
        private AnimalRepository animalRepo = new AnimalRepository();
        private VeterinarianRepository vetRepo = new VeterinarianRepository();

        public AppointmentWindow(Appointment appt = null)
        {
            InitializeComponent();

            AnimalBox.ItemsSource = animalRepo.GetAll();
            VetBox.ItemsSource = vetRepo.GetAll();

            if (appt != null)
            {
                Appointment = appt;
                AnimalBox.SelectedValue = appt.Animal?.Id;
                VetBox.SelectedValue = appt.Doctor?.Id;
                DatePickerBox.Value = appt.VisitDate;
                ComplaintBox.Text = appt.Complaint;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var vet = VetBox.SelectedItem as Veterinarian;

            if (AnimalBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите животное");
                return;
            }

            if (VetBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите ветеринара");
                return;
            }

            if (DatePickerBox.Value == null)
            {
                MessageBox.Show("Укажите дату и время приёма");
                return;
            }

            if (string.IsNullOrWhiteSpace(ComplaintBox.Text))
            {
                MessageBox.Show("Введите жалобу");
                return;
            }

            DateTime visit = DatePickerBox.Value.Value;

            if (IsTimeConflict(vet.Id, visit))
            {
                MessageBox.Show("У врача уже есть приём в это время!");
                return;
            }

            if (Appointment == null)
                Appointment = new Appointment();

            Appointment.Animal = AnimalBox.SelectedItem as Animal;
            Appointment.Doctor = VetBox.SelectedItem as Veterinarian;
            Appointment.VisitDate = DatePickerBox.Value.Value;
            Appointment.Complaint = ComplaintBox.Text;

            DialogResult = true;
            Close();
        }

        private bool IsTimeConflict(int vetId, DateTime newStart)
        {
            DateTime newEnd = newStart.AddMinutes(30);

            var conn = RepositoryHelper.GetConnection();
            conn.Open();

            var cmd = new SqlCommand(@"
                SELECT COUNT(*)
                FROM Appointments
                WHERE VeterinarianId = @VetId
                AND (@NewStart < DATEADD(MINUTE, 30, VisitDate)
                AND @NewEnd > VisitDate)",
            conn);

            cmd.Parameters.AddWithValue("@VetId", vetId);
            cmd.Parameters.AddWithValue("@NewStart", newStart);
            cmd.Parameters.AddWithValue("@NewEnd", newEnd);

            int count = (int)cmd.ExecuteScalar();

            return count > 0;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
