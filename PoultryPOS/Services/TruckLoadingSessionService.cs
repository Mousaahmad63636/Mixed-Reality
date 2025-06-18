using Microsoft.Data.SqlClient;
using PoultryPOS.Models;
using System.Data;

namespace PoultryPOS.Services
{
    public class TruckLoadingSessionService
    {
        private readonly DatabaseService _dbService;

        public TruckLoadingSessionService()
        {
            _dbService = new DatabaseService();
        }

        public void StartLoadingSession(TruckLoadingSession session)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand(@"
                INSERT INTO TruckLoadingSessions (TruckId, LoadDate, InitialCages, InitialWeight, IsCompleted) 
                VALUES (@TruckId, @LoadDate, @InitialCages, @InitialWeight, 0)", connection);

            command.Parameters.AddWithValue("@TruckId", session.TruckId);
            command.Parameters.AddWithValue("@LoadDate", session.LoadDate);
            command.Parameters.AddWithValue("@InitialCages", session.InitialCages);
            command.Parameters.AddWithValue("@InitialWeight", session.InitialWeight);

            command.ExecuteNonQuery();
        }

        public void CompleteLoadingSession(int truckId, decimal finalWeight)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var activeSession = GetActiveSessionByTruckId(truckId);
            if (activeSession == null) return;

            var weightVariance = finalWeight;

            var command = new SqlCommand(@"
                UPDATE TruckLoadingSessions 
                SET CompletionDate = @CompletionDate, FinalWeight = @FinalWeight, 
                    WeightVariance = @WeightVariance, IsCompleted = 1 
                WHERE Id = @Id", connection);

            command.Parameters.AddWithValue("@Id", activeSession.Id);
            command.Parameters.AddWithValue("@CompletionDate", DateTime.Now);
            command.Parameters.AddWithValue("@FinalWeight", finalWeight);
            command.Parameters.AddWithValue("@WeightVariance", weightVariance);

            command.ExecuteNonQuery();
        }

        public TruckLoadingSession GetActiveSessionByTruckId(int truckId)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand(@"
                SELECT tls.*, t.Name as TruckName
                FROM TruckLoadingSessions tls
                JOIN Trucks t ON tls.TruckId = t.Id
                WHERE tls.TruckId = @TruckId AND tls.IsCompleted = 0", connection);

            command.Parameters.AddWithValue("@TruckId", truckId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new TruckLoadingSession
                {
                    Id = reader.GetInt32("Id"),
                    TruckId = reader.GetInt32("TruckId"),
                    LoadDate = reader.GetDateTime("LoadDate"),
                    InitialCages = reader.GetInt32("InitialCages"),
                    InitialWeight = reader.GetDecimal("InitialWeight"),
                    CompletionDate = reader.IsDBNull("CompletionDate") ? null : reader.GetDateTime("CompletionDate"),
                    FinalWeight = reader.IsDBNull("FinalWeight") ? null : reader.GetDecimal("FinalWeight"),
                    WeightVariance = reader.IsDBNull("WeightVariance") ? null : reader.GetDecimal("WeightVariance"),
                    IsCompleted = reader.GetBoolean("IsCompleted"),
                    TruckName = reader.GetString("TruckName")
                };
            }

            return null;
        }

        public List<TruckLoadingSession> GetAllSessions()
        {
            var sessions = new List<TruckLoadingSession>();
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand(@"
                SELECT tls.*, t.Name as TruckName
                FROM TruckLoadingSessions tls
                JOIN Trucks t ON tls.TruckId = t.Id
                ORDER BY tls.LoadDate DESC", connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                sessions.Add(new TruckLoadingSession
                {
                    Id = reader.GetInt32("Id"),
                    TruckId = reader.GetInt32("TruckId"),
                    LoadDate = reader.GetDateTime("LoadDate"),
                    InitialCages = reader.GetInt32("InitialCages"),
                    InitialWeight = reader.GetDecimal("InitialWeight"),
                    CompletionDate = reader.IsDBNull("CompletionDate") ? null : reader.GetDateTime("CompletionDate"),
                    FinalWeight = reader.IsDBNull("FinalWeight") ? null : reader.GetDecimal("FinalWeight"),
                    WeightVariance = reader.IsDBNull("WeightVariance") ? null : reader.GetDecimal("WeightVariance"),
                    IsCompleted = reader.GetBoolean("IsCompleted"),
                    TruckName = reader.GetString("TruckName")
                });
            }

            return sessions;
        }

        public List<TruckLoadingSession> GetSessionsByDateRange(DateTime fromDate, DateTime toDate)
        {
            var sessions = new List<TruckLoadingSession>();
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand(@"
                SELECT tls.*, t.Name as TruckName
                FROM TruckLoadingSessions tls
                JOIN Trucks t ON tls.TruckId = t.Id
                WHERE tls.LoadDate >= @FromDate AND tls.LoadDate <= @ToDate
                ORDER BY tls.LoadDate DESC", connection);

            command.Parameters.AddWithValue("@FromDate", fromDate.Date);
            command.Parameters.AddWithValue("@ToDate", toDate.Date.AddDays(1).AddSeconds(-1));

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                sessions.Add(new TruckLoadingSession
                {
                    Id = reader.GetInt32("Id"),
                    TruckId = reader.GetInt32("TruckId"),
                    LoadDate = reader.GetDateTime("LoadDate"),
                    InitialCages = reader.GetInt32("InitialCages"),
                    InitialWeight = reader.GetDecimal("InitialWeight"),
                    CompletionDate = reader.IsDBNull("CompletionDate") ? null : reader.GetDateTime("CompletionDate"),
                    FinalWeight = reader.IsDBNull("FinalWeight") ? null : reader.GetDecimal("FinalWeight"),
                    WeightVariance = reader.IsDBNull("WeightVariance") ? null : reader.GetDecimal("WeightVariance"),
                    IsCompleted = reader.GetBoolean("IsCompleted"),
                    TruckName = reader.GetString("TruckName")
                });
            }

            return sessions;
        }

        public List<TruckLoadingSession> GetActiveSessions()
        {
            var sessions = new List<TruckLoadingSession>();
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand(@"
                SELECT tls.*, t.Name as TruckName
                FROM TruckLoadingSessions tls
                JOIN Trucks t ON tls.TruckId = t.Id
                WHERE tls.IsCompleted = 0
                ORDER BY tls.LoadDate DESC", connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                sessions.Add(new TruckLoadingSession
                {
                    Id = reader.GetInt32("Id"),
                    TruckId = reader.GetInt32("TruckId"),
                    LoadDate = reader.GetDateTime("LoadDate"),
                    InitialCages = reader.GetInt32("InitialCages"),
                    InitialWeight = reader.GetDecimal("InitialWeight"),
                    CompletionDate = reader.IsDBNull("CompletionDate") ? null : reader.GetDateTime("CompletionDate"),
                    FinalWeight = reader.IsDBNull("FinalWeight") ? null : reader.GetDecimal("FinalWeight"),
                    WeightVariance = reader.IsDBNull("WeightVariance") ? null : reader.GetDecimal("WeightVariance"),
                    IsCompleted = reader.GetBoolean("IsCompleted"),
                    TruckName = reader.GetString("TruckName")
                });
            }

            return sessions;
        }

        public decimal GetTotalVarianceByDateRange(DateTime fromDate, DateTime toDate)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand(@"
                SELECT ISNULL(SUM(WeightVariance), 0) 
                FROM TruckLoadingSessions 
                WHERE IsCompleted = 1 AND LoadDate >= @FromDate AND LoadDate <= @ToDate", connection);

            command.Parameters.AddWithValue("@FromDate", fromDate.Date);
            command.Parameters.AddWithValue("@ToDate", toDate.Date.AddDays(1).AddSeconds(-1));

            return (decimal)command.ExecuteScalar();
        }

        public void DeleteSession(int sessionId)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand("DELETE FROM TruckLoadingSessions WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", sessionId);

            command.ExecuteNonQuery();
        }
    }
}