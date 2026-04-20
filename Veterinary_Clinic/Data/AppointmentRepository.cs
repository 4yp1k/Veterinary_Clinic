using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veterinary_Clinic.Models;

namespace Veterinary_Clinic.Data
{
    public class AppointmentRepository : IRepository<Appointment>
    {
        public void Add(Appointment app)
        {
            var conn = RepositoryHelper.GetConnection();
            conn.Open();

            var cmd = new SqlCommand(@"
        INSERT INTO Appointments (AnimalId, VeterinarianId, VisitDate, Complaint)
        VALUES (@a,@v,@d,@c)", conn);

            cmd.Parameters.AddWithValue("@a", app.Animal.Id);
            cmd.Parameters.AddWithValue("@v", app.Doctor.Id);
            cmd.Parameters.AddWithValue("@d", app.VisitDate);
            cmd.Parameters.AddWithValue("@c", app.Complaint);

            cmd.ExecuteNonQuery();
        }

        public void Update(Appointment app)
        {
            var conn = RepositoryHelper.GetConnection();
            conn.Open();

            var cmd = new SqlCommand(@"
        UPDATE Appointments
        SET AnimalId = @a,
            VeterinarianId = @v,
            VisitDate = @d,
            Complaint = @c
        WHERE Id = @id", conn);

            cmd.Parameters.AddWithValue("@a", app.Animal.Id);
            cmd.Parameters.AddWithValue("@v", app.Doctor.Id);
            cmd.Parameters.AddWithValue("@d", app.VisitDate);
            cmd.Parameters.AddWithValue("@c", app.Complaint);
            cmd.Parameters.AddWithValue("@id", app.Id);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            var conn = RepositoryHelper.GetConnection();
            conn.Open();

            var cmd = new SqlCommand("DELETE FROM Appointments WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }

        public List<Appointment> GetAll()
        {
            var list = new List<Appointment>();

            var conn = RepositoryHelper.GetConnection();
            conn.Open();

            var cmd = new SqlCommand(@"
        SELECT 
            ap.Id,
            ap.VisitDate,
            ap.Complaint,
            a.Id AS AnimalId,
            a.Name AS AnimalName,
            v.Id AS VetId,
            v.FullName AS VetName
        FROM Appointments ap
        JOIN Animals a ON ap.AnimalId = a.Id
        JOIN Veterinarians v ON ap.VeterinarianId = v.Id", conn);

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Appointment
                {
                    Id = (int)reader["Id"],
                    VisitDate = (DateTime)reader["VisitDate"],
                    Complaint = reader["Complaint"].ToString(),

                    Animal = new Animal
                    {
                        Id = (int)reader["AnimalId"],
                        Name = reader["AnimalName"].ToString()
                    },

                    Doctor = new Veterinarian
                    {
                        Id = (int)reader["VetId"],
                        FullName = reader["VetName"].ToString()
                    }
                });
            }

            return list;
        }
    }
}
