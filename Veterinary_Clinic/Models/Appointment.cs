using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veterinary_Clinic.Models;

namespace Veterinary_Clinic
{
    public class Appointment : IEntity
    {
        public int Id { get; set; }

        public Animal Animal { get; set; }
        public Veterinarian Doctor { get; set; }

        public DateTime VisitDate { get; set; }
        public string Complaint { get; set; }

        public string DoctorName => Doctor?.FullName;

        public string DisplayInfo =>
           $"{VisitDate:dd.MM.yyyy} - {Animal?.Name} - {Doctor?.FullName}";
    }
}
