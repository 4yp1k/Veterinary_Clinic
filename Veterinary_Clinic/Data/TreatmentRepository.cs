using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veterinary_Clinic.Models;

namespace Veterinary_Clinic.Data
{
    public class TreatmentRepository : IRepository<Treatment>
    {
        public void Add(Treatment treatment)
        {
            var conn = RepositoryHelper.GetConnection();
            conn.Open();

            var cmd = new SqlCommand(@"
                INSERT INTO Treatments 
                    (AppointmentId, Diagnosis,TreatmentPlan, DateCreated, Cost)
                VALUES 
                    (@AppointmentId, @Diagnosis, @TreatmentPlan, @DateCreated, @Cost);
                ", conn);

            cmd.Parameters.AddWithValue("@AppointmentId", treatment.Appointment.Id);
            cmd.Parameters.AddWithValue("@Diagnosis", treatment.Diagnosis ?? "");
            cmd.Parameters.AddWithValue("@TreatmentPlan", treatment.TreatmentPlan ?? "");
            cmd.Parameters.AddWithValue("@DateCreated", treatment.DateCreated);
            cmd.Parameters.AddWithValue("@Cost", treatment.Cost);

            cmd.ExecuteNonQuery();
        }

        public void Update(Treatment treatment)
        {
            var conn = RepositoryHelper.GetConnection();
            conn.Open();

            var cmd = new SqlCommand(@"
                UPDATE Treatments
                SET 
                    AppointmentId = @AppointmentId,
                    Diagnosis = @Diagnosis,
                    TreatmentPlan = @TreatmentPlan,
                    Cost = @Cost
                WHERE Id = @Id;
                ", conn);

            cmd.Parameters.AddWithValue("@Id", treatment.Id);
            cmd.Parameters.AddWithValue("@AppointmentId", treatment.Appointment.Id);
            cmd.Parameters.AddWithValue("@Diagnosis", treatment.Diagnosis ?? "");
            cmd.Parameters.AddWithValue("@TreatmentPlan", treatment.TreatmentPlan ?? "");
            cmd.Parameters.AddWithValue("@DateCreated", treatment.DateCreated);
            cmd.Parameters.AddWithValue("@Cost", treatment.Cost);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            var conn = RepositoryHelper.GetConnection();
            conn.Open();

            var cmd = new SqlCommand(@"
                DELETE FROM Treatments WHERE Id = @Id;", conn);

            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();
        }

        public List<Treatment> GetAll()
        {
            var list = new List<Treatment>();

            var conn = RepositoryHelper.GetConnection();
            conn.Open();

            var cmd = new SqlCommand(@"
                SELECT 
                    t.Id,
                    t.Diagnosis,
                    t.TreatmentPlan,
                    t.DateCreated,
                    t.Cost,

                    ap.Id AS AppointmentId,
                    ap.VisitDate,

                    a.Id AS AnimalId,
                    a.Name AS AnimalName,

                    v.Id AS VetId,
                    v.FullName AS VetName

                    FROM Treatments t
                    JOIN Appointments ap ON t.AppointmentId = ap.Id
                    JOIN Animals a ON ap.AnimalId = a.Id
                    JOIN Veterinarians v ON ap.VeterinarianId = v.Id
                    ", conn);

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Treatment
                {
                    Id = (int)reader["Id"],
                    Diagnosis = reader["Diagnosis"].ToString(),
                    TreatmentPlan = reader["TreatmentPlan"].ToString(),
                    DateCreated = (DateTime)reader["DateCreated"],
                    Cost = (int)(reader["Cost"] == DBNull.Value ? 0 : (decimal)reader["Cost"]),

                    Appointment = new Appointment
                    {
                        Id = (int)reader["AppointmentId"],
                        VisitDate = (DateTime)reader["VisitDate"],

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
                    }
                });
            }

            return list;
        }
    }
}
