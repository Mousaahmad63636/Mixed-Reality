using Microsoft.Data.SqlClient;
using PoultryPOS.Models;
using System.Data;

namespace PoultryPOS.Services
{
    public class TruckService
    {
        private readonly DatabaseService _dbService;
        private readonly TruckLoadingSessionService _loadingSessionService;

        public TruckService()
        {
            _dbService = new DatabaseService();
            _loadingSessionService = new TruckLoadingSessionService();
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

            var getCurrentLoadCommand = new SqlCommand("SELECT CurrentLoad FROM Trucks WHERE Id = @Id", connection);
            getCurrentLoadCommand.Parameters.AddWithValue("@Id", truckId);
            var currentLoad = (int)getCurrentLoadCommand.ExecuteScalar();

            var newLoad = currentLoad - cagesUsed;

            var command = new SqlCommand("UPDATE Trucks SET CurrentLoad = @NewLoad WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", truckId);
            command.Parameters.AddWithValue("@NewLoad", Math.Max(0, newLoad));

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

        public void UpdateTruckFromSale(int truckId, int cagesUsed, decimal weightUsed)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var getCurrentDataCommand = new SqlCommand("SELECT CurrentLoad, NetWeight FROM Trucks WHERE Id = @Id", connection);
            getCurrentDataCommand.Parameters.AddWithValue("@Id", truckId);

            using var reader = getCurrentDataCommand.ExecuteReader();
            int currentLoad = 0;
            decimal currentWeight = 0;

            if (reader.Read())
            {
                currentLoad = reader.GetInt32("CurrentLoad");
                currentWeight = reader.GetDecimal("NetWeight");
            }
            reader.Close();

            var newLoad = Math.Max(0, currentLoad - cagesUsed);
            var newWeight = currentWeight - weightUsed;

            var updateCommand = new SqlCommand("UPDATE Trucks SET CurrentLoad = @NewLoad, NetWeight = @NewWeight WHERE Id = @Id", connection);
            updateCommand.Parameters.AddWithValue("@Id", truckId);
            updateCommand.Parameters.AddWithValue("@NewLoad", newLoad);
            updateCommand.Parameters.AddWithValue("@NewWeight", newWeight);
            updateCommand.ExecuteNonQuery();

            if (newLoad <= 0)
            {
                CheckAndCompleteLoadingSession(truckId, newWeight);
            }
        }

        private void CheckAndCompleteLoadingSession(int truckId, decimal finalWeight)
        {
            var activeSession = _loadingSessionService.GetActiveSessionByTruckId(truckId);
            if (activeSession != null)
            {
                _loadingSessionService.CompleteLoadingSession(truckId, finalWeight);
            }
        }

        public void StartLoadingSession(int truckId, int initialCages, decimal initialWeight)
        {
            var existingSession = _loadingSessionService.GetActiveSessionByTruckId(truckId);
            if (existingSession != null)
            {
                throw new InvalidOperationException($"الشاحنة لديها جلسة تحميل نشطة بالفعل. يجب إنهاء الجلسة الحالية أولاً.");
            }

            using var connection = _dbService.GetConnection();
            connection.Open();

            var updateTruckCommand = new SqlCommand("UPDATE Trucks SET CurrentLoad = @CurrentLoad, NetWeight = @NetWeight WHERE Id = @Id", connection);
            updateTruckCommand.Parameters.AddWithValue("@Id", truckId);
            updateTruckCommand.Parameters.AddWithValue("@CurrentLoad", initialCages);
            updateTruckCommand.Parameters.AddWithValue("@NetWeight", initialWeight);
            updateTruckCommand.ExecuteNonQuery();

            var session = new TruckLoadingSession
            {
                TruckId = truckId,
                LoadDate = DateTime.Now,
                InitialCages = initialCages,
                InitialWeight = initialWeight,
                IsCompleted = false
            };

            _loadingSessionService.StartLoadingSession(session);
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