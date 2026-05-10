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
            VetBox.ItemsSource = vetRepo.GetActive();

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

            if (IsTimeConflict(vet.Id, visit, Appointment?.Id))
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

        private bool IsTimeConflict(int vetId, DateTime newStart, int? excludeAppointmentId = null)
        {
            DateTime newEnd = newStart.AddMinutes(30);

            var appointRepo = new AppointmentRepository();
            var appointments = appointRepo.GetAll();

            foreach (var app in appointments)
            {
                if (excludeAppointmentId.HasValue && app.Id == excludeAppointmentId.Value)
                    continue;

                if (app.Doctor != null && app.Doctor.Id == vetId)
                {
                    if (newStart < app.VisitDate.AddMinutes(30) && newEnd > app.VisitDate)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
