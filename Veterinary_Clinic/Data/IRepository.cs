using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veterinary_Clinic.Models
{
    public interface IRepository<T> where T : IEntity
    {
        List<T> GetAll();
        void Add(T item);
        void Update(T item);
        void Delete(int id);
    }
}