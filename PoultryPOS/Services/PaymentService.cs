using Microsoft.Data.SqlClient;
using PoultryPOS.Models;
using System.Data;

namespace PoultryPOS.Services
{
    public class PaymentService
    {
        private readonly DatabaseService _dbService;

        public PaymentService()
        {
            _dbService = new DatabaseService();
        }

        public void Add(Payment payment)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand(@"
                INSERT INTO Payments (CustomerId, Amount, PaymentDate, Notes) 
                VALUES (@CustomerId, @Amount, @PaymentDate, @Notes)", connection);

            command.Parameters.AddWithValue("@CustomerId", payment.CustomerId);
            command.Parameters.AddWithValue("@Amount", payment.Amount);
            command.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
            command.Parameters.AddWithValue("@Notes", payment.Notes ?? (object)DBNull.Value);

            command.ExecuteNonQuery();
        }

        public void Update(Payment payment)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand(@"
                UPDATE Payments 
                SET CustomerId = @CustomerId, Amount = @Amount, PaymentDate = @PaymentDate, Notes = @Notes 
                WHERE Id = @Id", connection);

            command.Parameters.AddWithValue("@Id", payment.Id);
            command.Parameters.AddWithValue("@CustomerId", payment.CustomerId);
            command.Parameters.AddWithValue("@Amount", payment.Amount);
            command.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
            command.Parameters.AddWithValue("@Notes", payment.Notes ?? (object)DBNull.Value);

            command.ExecuteNonQuery();
        }

        public Payment GetById(int id)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand(@"
                SELECT p.*, c.Name as CustomerName
                FROM Payments p
                JOIN Customers c ON p.CustomerId = c.Id
                WHERE p.Id = @Id", connection);

            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Payment
                {
                    Id = reader.GetInt32("Id"),
                    CustomerId = reader.GetInt32("CustomerId"),
                    Amount = reader.GetDecimal("Amount"),
                    PaymentDate = reader.GetDateTime("PaymentDate"),
                    Notes = reader.IsDBNull("Notes") ? null : reader.GetString("Notes"),
                    CustomerName = reader.GetString("CustomerName")
                };
            }

            return null;
        }

        public List<Payment> GetAll()
        {
            var payments = new List<Payment>();
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand(@"
                SELECT p.*, c.Name as CustomerName
                FROM Payments p
                JOIN Customers c ON p.CustomerId = c.Id
                ORDER BY p.PaymentDate DESC", connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                payments.Add(new Payment
                {
                    Id = reader.GetInt32("Id"),
                    CustomerId = reader.GetInt32("CustomerId"),
                    Amount = reader.GetDecimal("Amount"),
                    PaymentDate = reader.GetDateTime("PaymentDate"),
                    Notes = reader.IsDBNull("Notes") ? null : reader.GetString("Notes"),
                    CustomerName = reader.GetString("CustomerName")
                });
            }

            return payments;
        }

        public List<Payment> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            var payments = new List<Payment>();
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand(@"
                SELECT p.*, c.Name as CustomerName
                FROM Payments p
                JOIN Customers c ON p.CustomerId = c.Id
                WHERE p.PaymentDate >= @FromDate AND p.PaymentDate <= @ToDate
                ORDER BY p.PaymentDate DESC", connection);

            command.Parameters.AddWithValue("@FromDate", fromDate.Date);
            command.Parameters.AddWithValue("@ToDate", toDate.Date.AddDays(1).AddSeconds(-1));

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                payments.Add(new Payment
                {
                    Id = reader.GetInt32("Id"),
                    CustomerId = reader.GetInt32("CustomerId"),
                    Amount = reader.GetDecimal("Amount"),
                    PaymentDate = reader.GetDateTime("PaymentDate"),
                    Notes = reader.IsDBNull("Notes") ? null : reader.GetString("Notes"),
                    CustomerName = reader.GetString("CustomerName")
                });
            }

            return payments;
        }

        public List<Payment> GetByCustomer(int customerId)
        {
            var payments = new List<Payment>();
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand(@"
                SELECT p.*, c.Name as CustomerName
                FROM Payments p
                JOIN Customers c ON p.CustomerId = c.Id
                WHERE p.CustomerId = @CustomerId
                ORDER BY p.PaymentDate DESC", connection);

            command.Parameters.AddWithValue("@CustomerId", customerId);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                payments.Add(new Payment
                {
                    Id = reader.GetInt32("Id"),
                    CustomerId = reader.GetInt32("CustomerId"),
                    Amount = reader.GetDecimal("Amount"),
                    PaymentDate = reader.GetDateTime("PaymentDate"),
                    Notes = reader.IsDBNull("Notes") ? null : reader.GetString("Notes"),
                    CustomerName = reader.GetString("CustomerName")
                });
            }

            return payments;
        }

        public void Delete(int id)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand("DELETE FROM Payments WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }
    }
}