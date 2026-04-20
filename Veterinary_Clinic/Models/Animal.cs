using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veterinary_Clinic.Models;

namespace Veterinary_Clinic
{
    public class Animal : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public DateTime Age { get; set; }

        public Owner Owner { get; set; }

        public string OwnerName => Owner?.FullName;

        public override string ToString() => Name;
    }
}