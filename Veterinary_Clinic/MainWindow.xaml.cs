using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Veterinary_Clinic.Data;
using Veterinary_Clinic.Models;
using Veterinary_Clinic.Views;

namespace Veterinary_Clinic
{
    public partial class MainWindow : Window
    {
        private bool isAdminMode = false;
        private AnimalRepository animalRepo = new AnimalRepository();
        private OwnerRepository ownerRepo = new OwnerRepository();
        private VeterinarianRepository vetRepo = new VeterinarianRepository();
        private AppointmentRepository appointRepo = new AppointmentRepository();
        private TreatmentRepository treatRepo = new TreatmentRepository();

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                ApplyPermissions();
                RefreshGrids();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void ApplyPermissions()
        {
            if (Session.CurrentUser.Role != UserRole.Admin)
            {
                VetsTab.Visibility = Visibility.Collapsed;
                DeleteAnimalButton.Visibility = Visibility.Collapsed;
                DeleteOwnerButton.Visibility = Visibility.Collapsed;
                DeleteAppointButton.Visibility = Visibility.Collapsed;
                DeleteTreatButton.Visibility = Visibility.Collapsed;
                RevenueVetTab.Visibility = Visibility.Collapsed;
                tabControl.SelectedIndex = 3;
            }
            else
            {
                VetsTab.Visibility = Visibility.Visible;
                DeleteAnimalButton.Visibility = Visibility.Visible;
                DeleteOwnerButton.Visibility = Visibility.Visible;
                DeleteAppointButton.Visibility = Visibility.Visible;
                DeleteTreatButton.Visibility = Visibility.Visible;
                RevenueVetTab.Visibility = Visibility.Visible;
            }
        }

        private void RefreshGrids()
        {
            AnimalsGrid.ItemsSource = animalRepo.GetAll();
            OwnersGrid.ItemsSource = ownerRepo.GetAll();
            VetsGrid.ItemsSource = vetRepo.GetActive();
            AppointmentsGrid.ItemsSource = appointRepo.GetAll();
            TreatmentsGrid.ItemsSource = treatRepo.GetAll();
        }

        private void AddAnimal_Click(object sender, RoutedEventArgs e)
        {
            var window = new AnimalWindow();

            if (window.ShowDialog() == true)
            {
                animalRepo.Add(window.Animal);
                RefreshGrids();
            }
        }

        private void EditAnimal_Click(object sender, RoutedEventArgs e)
        {
            var selected = AnimalsGrid.SelectedItem as Animal;
            if (selected == null) return;

            var window = new AnimalWindow(selected);

            if (window.ShowDialog() == true)
            {
                animalRepo.Update(selected);
                RefreshGrids();
            }
        }

        private void DeleteAnimal_Click(object sender, RoutedEventArgs e)
        {
            var selected = AnimalsGrid.SelectedItem as Animal;
            if (selected == null) return;

            animalRepo.Delete(selected.Id);
            RefreshGrids();
        }

        private void AddOwner_Click(object sender, RoutedEventArgs e)
        {
            var window = new OwnerWindow();

            if (window.ShowDialog() == true)
            {
                ownerRepo.Add(window.Owner);
                RefreshGrids();
            }
        }

        private void EditOwner_Click(object sender, RoutedEventArgs e)
        {
            var selected = OwnersGrid.SelectedItem as Owner;
            if (selected == null) return;

            var window = new OwnerWindow(selected);

            if (window.ShowDialog() == true)
            {
                ownerRepo.Update(window.Owner);
                RefreshGrids();
            }
        }

        private void DeleteOwner_Click(object sender, RoutedEventArgs e)
        {
            var selected = OwnersGrid.SelectedItem as Owner;
            if (selected == null) return;

            ownerRepo.Delete(selected.Id);
            RefreshGrids();
        }

        private void AddVet_Click(object sender, RoutedEventArgs e)
        {
            var window = new VeterinarianWindow();

            if (window.ShowDialog() == true)
            {
                vetRepo.Add(window.Vet);
                RefreshGrids();
            }
        }

        private void EditVet_Click(object sender, RoutedEventArgs e)
        {
            var selected = VetsGrid.SelectedItem as Veterinarian;
            if (selected == null) return;

            var window = new VeterinarianWindow(selected);

            if (window.ShowDialog() == true)
            {
                vetRepo.Update(window.Vet);
                RefreshGrids();
            }
        }

        private void DeleteVet_Click(object sender, RoutedEventArgs e)
        {
            var selected = VetsGrid.SelectedItem as Veterinarian;
            if (selected == null) return;

            vetRepo.Delete(selected.Id);
            RefreshGrids();
        }

        private void AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            var window = new AppointmentWindow();

            if (window.ShowDialog() == true)
            {
                appointRepo.Add(window.Appointment);
                RefreshGrids();
            }
        }

        private void EditAppointment_Click(object sender, RoutedEventArgs e)
        {
            var selected = AppointmentsGrid.SelectedItem as Appointment;
            if (selected == null) return;

            var window = new AppointmentWindow(selected);

            window.DatePickerBox.IsEnabled = false;

            if (window.ShowDialog() == true)
            {
                appointRepo.Update(window.Appointment);
                RefreshGrids();
            }
        }

        private void DeleteAppointment_Click(object sender, RoutedEventArgs e)
        {
            var selected = AppointmentsGrid.SelectedItem as Appointment;
            if (selected == null) return;

            appointRepo.Delete(selected.Id);
            RefreshGrids();
        }

        private void AddTreatment_Click(object sender, RoutedEventArgs e)
        {
            var window = new TreatmentWindow();

            if (window.ShowDialog() == true)
            {
                treatRepo.Add(window.Treatment);
                RefreshGrids();
            }
        }

        private void EditTreatment_Click(object sender, RoutedEventArgs e)
        {
            var selected = TreatmentsGrid.SelectedItem as Treatment;
            if (selected == null) return;

            var window = new TreatmentWindow(selected);

            if (window.ShowDialog() == true)
            {
                treatRepo.Update(window.Treatment);
                RefreshGrids();
            }
        }

        private void DeleteTreatment_Click(object sender, RoutedEventArgs e)
        {
            var selected = TreatmentsGrid.SelectedItem as Treatment;
            if (selected == null) return;

            treatRepo.Delete(selected.Id);
            RefreshGrids();
        }

        private void SearchBoxAn_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = SearchBoxAn.Text.ToLower().Trim();

            AnimalsGrid.ItemsSource = animalRepo.GetAll()
                .Where(a =>
                    (a.Name ?? "").ToLower().Contains(text) ||
                    (a.Species ?? "").ToLower().Contains(text) ||
                    a.Age.ToString().Contains(text) ||
                    (a.Owner?.FullName ?? "").ToLower().Contains(text))
                .ToList();
        }

        private void SearchBoxOw_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = SearchBoxOw.Text.ToLower().Trim();

            OwnersGrid.ItemsSource = ownerRepo.GetAll()
                .Where(o =>
                    (o.FullName ?? "").ToLower().Contains(text) ||
                    (o.Phone ?? "").ToLower().Contains(text) ||
                    (o.Address ?? "").ToLower().Contains(text))
                .ToList();
        }

        private void SearchBoxVt_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = SearchBoxVt.Text.ToLower().Trim();

            VetsGrid.ItemsSource = vetRepo.GetAll()
                .Where(v =>
                    (v.FullName ?? "").ToLower().Contains(text) ||
                    (v.Specialty ?? "").ToLower().Contains(text))
                .ToList();
        }

        private void SearchBoxAp_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = SearchBoxAp.Text.ToLower().Trim();

            AppointmentsGrid.ItemsSource = appointRepo.GetAll()
                .Where(a =>
                    (a.Complaint ?? "").ToLower().Contains(text) ||
                    a.VisitDate.ToString("dd.MM.yyyy HH:mm").Contains(text) ||
                    (a.Animal?.Name ?? "").ToLower().Contains(text) ||
                    (a.Doctor?.FullName ?? "").ToLower().Contains(text))
                .ToList();
        }

        private void WatermarkTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = SearchBoxTr.Text.ToLower().Trim();

            TreatmentsGrid.ItemsSource = treatRepo.GetAll()
                .Where(t =>
                    (t.Diagnosis ?? "").ToLower().Contains(text) ||
                    (t.TreatmentPlan ?? "").ToLower().Contains(text) ||
                    t.Cost.ToString().Contains(text) ||
                    t.DateCreated.ToString("dd.MM.yyyy").Contains(text) ||
                    (t.Appointment?.Animal?.Name ?? "").ToLower().Contains(text) ||
                    (t.Appointment?.Doctor?.FullName ?? "").ToLower().Contains(text))
                .ToList();
        }

        private void AdminMode_Click(object sender, RoutedEventArgs e)
        {
            if (!isAdminMode)
            {
                var login = new LoginWindow();
                bool? result = login.ShowDialog();

                if (result == true && Session.CurrentUser.Role == UserRole.Admin)
                {
                    isAdminMode = true;

                    ApplyPermissions();

                    AdminButton.Content = "👉 Выйти из админа";

                    MessageBox.Show("Админ режим включен");
                }
            }
            else
            {
                isAdminMode = false;

                Session.CurrentUser.Role = UserRole.Doctor;

                ApplyPermissions();

                AdminButton.Content = "👤 Войти как админ";

                MessageBox.Show("Админ режим выключен");
            }
        }

        private void LoadRevenueByVets_Click(object sender, RoutedEventArgs e)
        {
            var treatmentRepo = new TreatmentRepository();
            var treatments = treatmentRepo.GetAll();

            if (treatments == null || treatments.Count == 0)
            {
                MessageBox.Show("Нет данных");
                return;
            }

            var grouped = treatments
                .GroupBy(t => new
                {
                    Day = t.DateCreated.Date,
                    VetName = t.Appointment?.Doctor?.FullName ?? "Неизвестно"
                })
                .Select(g => new RevenueVetItem
                {
                    Day = g.Key.Day,
                    VetName = g.Key.VetName,
                    Total = g.Sum(t => (decimal)t.Cost)
                })
                .OrderBy(x => x.Day)
                .ToList();

            if (grouped.Count == 0)
            {
                MessageBox.Show("Нет данных для отчёта");
                return;
            }

            RevenueVetGrid.ItemsSource = grouped;
            decimal totalSum = grouped.Sum(x => x.Total);
            TotalText.Text = $"Общая выручка: {totalSum} ₽";
        }
    }
}