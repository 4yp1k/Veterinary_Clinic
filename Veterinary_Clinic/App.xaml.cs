using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Veterinary_Clinic.Data;
using Veterinary_Clinic.Models;
using Veterinary_Clinic.Views;

namespace Veterinary_Clinic
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ExcelPackage.License.SetNonCommercialOrganization("Каширин И.Г.");

            var userRepo = new UserRepository();
            var users = userRepo.GetAll();
            if (users.Count == 0)
            {
                userRepo.Add(new User
                {
                    Username = "admin",
                    Password = "admin",
                    Role = UserRole.Admin
                });
            }
        }
    }
}
