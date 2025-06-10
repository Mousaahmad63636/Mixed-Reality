using Microsoft.Data.SqlClient;
using PoultryPOS.Models;
using System.Data;

namespace PoultryPOS.Services
{
    public class DriverService
    {
        private readonly DatabaseService _dbService;

        public DriverService()
        {
            _dbService = new DatabaseService();
        }

        public List<Driver> GetAll()
        {
            var drivers = new List<Driver>();
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand("SELECT * FROM Drivers WHERE IsActive = 1", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                drivers.Add(new Driver
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    Phone = reader.IsDBNull("Phone") ? null : reader.GetString("Phone"),
                    IsActive = reader.GetBoolean("IsActive")
                });
            }

            return drivers;
        }

        public void Add(Driver driver)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand("INSERT INTO Drivers (Name, Phone) VALUES (@Name, @Phone)", connection);
            command.Parameters.AddWithValue("@Name", driver.Name);
            command.Parameters.AddWithValue("@Phone", driver.Phone ?? (object)DBNull.Value);

            command.ExecuteNonQuery();
        }

        public void Update(Driver driver)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand("UPDATE Drivers SET Name = @Name, Phone = @Phone WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", driver.Id);
            command.Parameters.AddWithValue("@Name", driver.Name);
            command.Parameters.AddWithValue("@Phone", driver.Phone ?? (object)DBNull.Value);

            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand("UPDATE Drivers SET IsActive = 0 WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }
    }
}