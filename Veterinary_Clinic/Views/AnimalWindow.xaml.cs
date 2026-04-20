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
using System.Windows.Shapes;
using Veterinary_Clinic.Data;

namespace Veterinary_Clinic
{
    public partial class AnimalWindow : Window
    {
        public Animal Animal { get; private set; }
        private OwnerRepository ownerRepo = new OwnerRepository();

        public AnimalWindow(Animal animal = null)
        {
            InitializeComponent();

            OwnerBox.ItemsSource = ownerRepo.GetAll();

            if (animal != null)
            {
                Animal = animal;
                NameBox.Text = animal.Name;
                SpeciesBox.Text = animal.Species;
                AgeBox.Text = animal.Age.ToString();
                OwnerBox.SelectedValue = animal.Owner?.Id;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите имя животного");
                return;
            }

            if (string.IsNullOrWhiteSpace(SpeciesBox.Text))
            {
                MessageBox.Show("Укажите вид животного");
                return;
            }

            if (AgeBox.SelectedDate == null)
            {
                MessageBox.Show("Введите возраст");
                return;
            }

            if (OwnerBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите владельца");
                return;
            }

            if (Animal == null)
                Animal = new Animal();

            Animal.Name = NameBox.Text.Trim();
            Animal.Species = SpeciesBox.Text.Trim();
            Animal.Age = (DateTime)AgeBox.SelectedDate;
            Animal.Owner = (Owner)OwnerBox.SelectedItem;

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