using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veterinary_Clinic.Models;

namespace Veterinary_Clinic.Data
{
    public class OwnerRepository : IRepository<Owner>
    {
        public void Add(Owner owner)
        {
            var conn = RepositoryHelper.GetConnection();
            conn.Open();

            var cmd = new SqlCommand(
                "INSERT INTO Owners (FullName, Phone, Address) VALUES (@name, @phone, @address)", conn);

            cmd.Parameters.AddWithValue("@name", owner.FullName);
            cmd.Parameters.AddWithValue("@phone", owner.Phone);
            cmd.Parameters.AddWithValue("@address", owner.Address);

            cmd.ExecuteNonQuery();
        }

        public void Update(Owner owner)
        {
            var conn = RepositoryHelper.GetConnection();
            conn.Open();

            var cmd = new SqlCommand(@"
        UPDATE Owners
        SET FullName = @n,
            Phone = @p,
            Address = @a
        WHERE Id = @id", conn);

            cmd.Parameters.AddWithValue("@n", owner.FullName);
            cmd.Parameters.AddWithValue("@p", owner.Phone);
            cmd.Parameters.AddWithValue("@a", owner.Address);
            cmd.Parameters.AddWithValue("@id", owner.Id);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            var conn = RepositoryHelper.GetConnection();
            conn.Open();

            var cmd = new SqlCommand("DELETE FROM Owners WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }

        public List<Owner> GetAll()
        {
            var list = new List<Owner>();

            var conn = RepositoryHelper.GetConnection();
            conn.Open();

            var cmd = new SqlCommand("SELECT * FROM Owners", conn);
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Owner
                {
                    Id = (int)reader["Id"],
                    FullName = reader["FullName"].ToString(),
                    Phone = reader["Phone"].ToString(),
                    Address = reader["Address"].ToString()
                });
            }

            return list;
        }
    }
}
