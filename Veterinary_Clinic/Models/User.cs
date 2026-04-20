using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veterinary_Clinic.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public UserRole Role { get; set; }
    }
}
