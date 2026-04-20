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

namespace Veterinary_Clinic
{
    public partial class VeterinarianWindow : Window
    {
        public Veterinarian Vet { get; set; }

        public VeterinarianWindow(Veterinarian vet = null)
        {
            InitializeComponent();

            if (vet != null)
            {
                Vet = vet;

                FullNameBox.Text = vet.FullName;
                SpecialtyBox.Text = vet.Specialty;
            }
            else
            {
                Vet = new Veterinarian();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameBox.Text))
            {
                MessageBox.Show("Введите ФИО ветеринара");
                return;
            }

            if (string.IsNullOrWhiteSpace(SpecialtyBox.Text))
            {
                MessageBox.Show("Введите специализацию");
                return;
            }

            if (Vet == null)
                Vet = new Veterinarian();

            Vet.FullName = FullNameBox.Text.Trim();
            Vet.Specialty = SpecialtyBox.Text.Trim();

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
