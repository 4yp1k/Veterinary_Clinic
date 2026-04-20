using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veterinary_Clinic.Models;

namespace Veterinary_Clinic.Data
{
    public static class Session
    {
        public static User CurrentUser { get; set; } = new User
        {
            Role = UserRole.Doctor
        };
    }
}
