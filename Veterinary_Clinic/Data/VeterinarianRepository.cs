using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veterinary_Clinic.Models;

namespace Veterinary_Clinic.Data
{
    public class VeterinarianRepository : IRepository<Veterinarian>
    {
        public void Add(Veterinarian vet)
        {
            var conn = RepositoryHelper.GetConnection();
            conn.Open();

            var cmd = new SqlCommand(
                "INSERT INTO Veterinarians (FullName, Specialty) VALUES (@n,@s)", conn);

            cmd.Parameters.AddWithValue("@n", vet.FullName);
            cmd.Parameters.AddWithValue("@s", vet.Specialty);

            cmd.ExecuteNonQuery();
        }

        public void Update(Veterinarian vet)
        {
            var conn = RepositoryHelper.GetConnection();
            conn.Open();

            var cmd = new SqlCommand(@"
        UPDATE Veterinarians
        SET FullName = @n,
            Specialty = @s
        WHERE Id = @id", conn);

            cmd.Parameters.AddWithValue("@n", vet.FullName);
            cmd.Parameters.AddWithValue("@s", vet.Specialty);
            cmd.Parameters.AddWithValue("@id", vet.Id);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            var conn = RepositoryHelper.GetConnection();
            conn.Open();

            var cmd = new SqlCommand("DELETE FROM Veterinarians WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }

        public List<Veterinarian> GetAll()
        {
            var list = new List<Veterinarian>();

             var conn = RepositoryHelper.GetConnection();
            conn.Open();

            var cmd = new SqlCommand("SELECT * FROM Veterinarians", conn);
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Veterinarian
                {
                    Id = (int)reader["Id"],
                    FullName = reader["FullName"].ToString(),
                    Specialty = reader["Specialty"].ToString()
                });
            }

            return list;
        }
    }
}
