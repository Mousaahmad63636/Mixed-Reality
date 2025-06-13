using Microsoft.Data.SqlClient;
using PoultryPOS.Models;
using System.Data;

namespace PoultryPOS.Services
{
    public class TruckService
    {
        private readonly DatabaseService _dbService;

        public TruckService()
        {
            _dbService = new DatabaseService();
        }

        public List<Truck> GetAll()
        {
            var trucks = new List<Truck>();
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand("SELECT * FROM Trucks WHERE IsActive = 1", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                trucks.Add(new Truck
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    CurrentLoad = reader.GetInt32("CurrentLoad"),
                    NetWeight = reader.GetDecimal("NetWeight"),
                    PlateNumber = reader.IsDBNull("PlateNumber") ? null : reader.GetString("PlateNumber"),
                    IsActive = reader.GetBoolean("IsActive")
                });
            }

            return trucks;
        }

        public void Add(Truck truck)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand("INSERT INTO Trucks (Name, CurrentLoad, NetWeight, PlateNumber) VALUES (@Name, @CurrentLoad, @NetWeight, @PlateNumber)", connection);
            command.Parameters.AddWithValue("@Name", truck.Name);
            command.Parameters.AddWithValue("@CurrentLoad", truck.CurrentLoad);
            command.Parameters.AddWithValue("@NetWeight", truck.NetWeight);
            command.Parameters.AddWithValue("@PlateNumber", truck.PlateNumber ?? (object)DBNull.Value);

            command.ExecuteNonQuery();
        }

        public void Update(Truck truck)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand("UPDATE Trucks SET Name = @Name, CurrentLoad = @CurrentLoad, NetWeight = @NetWeight, PlateNumber = @PlateNumber WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", truck.Id);
            command.Parameters.AddWithValue("@Name", truck.Name);
            command.Parameters.AddWithValue("@CurrentLoad", truck.CurrentLoad);
            command.Parameters.AddWithValue("@NetWeight", truck.NetWeight);
            command.Parameters.AddWithValue("@PlateNumber", truck.PlateNumber ?? (object)DBNull.Value);

            command.ExecuteNonQuery();
        }

        public void UpdateCurrentLoad(int truckId, int cagesUsed)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand("UPDATE Trucks SET CurrentLoad = CurrentLoad - @CagesUsed WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", truckId);
            command.Parameters.AddWithValue("@CagesUsed", cagesUsed);

            command.ExecuteNonQuery();
        }

        public void UpdateNetWeight(int truckId, decimal weightUsed)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand("UPDATE Trucks SET NetWeight = NetWeight - @WeightUsed WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", truckId);
            command.Parameters.AddWithValue("@WeightUsed", weightUsed);

            command.ExecuteNonQuery();
        }

        public Truck GetById(int id)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand("SELECT * FROM Trucks WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Truck
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    CurrentLoad = reader.GetInt32("CurrentLoad"),
                    NetWeight = reader.GetDecimal("NetWeight"),
                    PlateNumber = reader.IsDBNull("PlateNumber") ? null : reader.GetString("PlateNumber"),
                    IsActive = reader.GetBoolean("IsActive")
                };
            }
            return null;
        }

        public void Delete(int id)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand("UPDATE Trucks SET IsActive = 0 WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }
    }
}