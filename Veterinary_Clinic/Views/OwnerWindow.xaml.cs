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
    public partial class OwnerWindow : Window
    {
        public Owner Owner { get; set; }

        public OwnerWindow(Owner owner = null)
        {
            InitializeComponent();

            if (owner != null)
            {
                Owner = owner;

                FullNameBox.Text = owner.FullName;
                PhoneBox.Text = owner.Phone;
                AddressBox.Text = owner.Address;
            }
            else
            {
                Owner = new Owner();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameBox.Text))
            {
                MessageBox.Show("Введите ФИО владельца");
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneBox.Text))
            {
                MessageBox.Show("Введите номер телефона");
                return;
            }

            if (string.IsNullOrWhiteSpace(AddressBox.Text))
            {
                MessageBox.Show("Введите адрес");
                return;
            }

            if (Owner == null)
                Owner = new Owner();

            Owner.FullName = FullNameBox.Text;
            Owner.Phone = PhoneBox.Text;
            Owner.Address = AddressBox.Text;

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
