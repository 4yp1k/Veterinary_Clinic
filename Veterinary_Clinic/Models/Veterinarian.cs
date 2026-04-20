using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veterinary_Clinic.Models;

namespace Veterinary_Clinic
{
    public class Veterinarian : IEntity
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Specialty { get; set; }
        public override string ToString() => FullName;
    }
}
