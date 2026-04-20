using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veterinary_Clinic.Models;

namespace Veterinary_Clinic.Data
{
    public class AnimalRepository : IRepository<Animal>
    {
        public void Add(Animal animal)
        {
            var conn = RepositoryHelper.GetConnection();
            conn.Open();

            var cmd = new SqlCommand(
                "INSERT INTO Animals (Name, Species, Age, OwnerId) VALUES (@n,@s,@a,@o)", conn);

            cmd.Parameters.AddWithValue("@n", animal.Name);
            cmd.Parameters.AddWithValue("@s", animal.Species);
            cmd.Parameters.AddWithValue("@a", animal.Age);
            cmd.Parameters.AddWithValue("@o", animal.Owner.Id);

            cmd.ExecuteNonQuery();
        }

        public void Update(Animal animal)
        {
            var conn = RepositoryHelper.GetConnection();
            conn.Open();

            var cmd = new SqlCommand(@"
        UPDATE Animals
        SET Name = @n,
            Species = @s,
            Age = @a,
            OwnerId = @o
        WHERE Id = @id", conn);

            cmd.Parameters.AddWithValue("@n", animal.Name);
            cmd.Parameters.AddWithValue("@s", animal.Species);
            cmd.Parameters.AddWithValue("@a", animal.Age);
            cmd.Parameters.AddWithValue("@o", animal.Owner.Id);
            cmd.Parameters.AddWithValue("@id", animal.Id);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            var conn = RepositoryHelper.GetConnection();
            conn.Open();

            var cmd = new SqlCommand("DELETE FROM Animals WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }

        public List<Animal> GetAll()
        {
            var list = new List<Animal>();

            var conn = RepositoryHelper.GetConnection();
            conn.Open();

            var cmd = new SqlCommand(@"
        SELECT a.*, o.FullName
        FROM Animals a
        JOIN Owners o ON a.OwnerId = o.Id", conn);

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Animal
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"].ToString(),
                    Species = reader["Species"].ToString(),
                    Age = (DateTime)reader["Age"],
                    Owner = new Owner
                    {
                        Id = (int)reader["OwnerId"],
                        FullName = reader["FullName"].ToString()
                    }
                });
            }

            return list;
        }
    }
}