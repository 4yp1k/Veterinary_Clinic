using System;
using System.Collections.Generic;
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

namespace Veterinary_Clinic
{
    public partial class TreatmentWindow : Window
    {
        public Treatment Treatment { get; set; }
        private AppointmentRepository appointRepo = new AppointmentRepository();

        public TreatmentWindow(Treatment t = null)
        {
            InitializeComponent();

            AppointmentComboBox.ItemsSource = appointRepo.GetAll();

            if (t != null)
            {
                Treatment = t;

                AppointmentComboBox.SelectedValue = t.Appointment?.Id;
                DiagnosisBox.Text = t.Diagnosis;
                TreatmentBox.Text = t.TreatmentPlan;
                CostBox.Text = t.Cost.ToString();
            }
            else
            {
                Treatment = new Treatment();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (AppointmentComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите приём");
                return;
            }

            if (string.IsNullOrWhiteSpace(DiagnosisBox.Text))
            {
                MessageBox.Show("Введите диагноз");
                return;
            }

            if (string.IsNullOrWhiteSpace(TreatmentBox.Text))
            {
                MessageBox.Show("Введите план лечения");
                return;
            }

            if (Treatment == null)
                Treatment = new Treatment();

            Treatment.Appointment = new Appointment
            {
                Id = (int)AppointmentComboBox.SelectedValue
            };

            Treatment.Diagnosis = DiagnosisBox.Text.Trim();
            Treatment.TreatmentPlan = TreatmentBox.Text.Trim();
            Treatment.Cost = int.Parse(CostBox.Text);
            Treatment.DateCreated = DateTime.Now;

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OnlyNumbers(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }
    }
}