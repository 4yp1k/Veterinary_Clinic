using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veterinary_Clinic.Models;

namespace Veterinary_Clinic
{
    public class Treatment : IEntity
    {
        public int Id { get; set; }
        public Appointment Appointment { get; set; }
        public string Diagnosis { get; set; }
        public string TreatmentPlan { get; set; }
        public DateTime DateCreated { get; set; }
        public int Cost { get; set; }
        public string AnimalName => Appointment?.Animal?.Name;
        public string VetName => Appointment?.Doctor?.FullName;
    }
}
