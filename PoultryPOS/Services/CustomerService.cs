using Microsoft.Data.SqlClient;
using PoultryPOS.Models;
using System.Data;

namespace PoultryPOS.Services
{
    public class CustomerService
    {
        private readonly DatabaseService _dbService;
        private readonly ChangeDetectionService _changeDetection;

        public CustomerService()
        {
            _dbService = new DatabaseService();
            _changeDetection = new ChangeDetectionService();
        }

        public List<Customer> GetAll()
        {
            var customers = new List<Customer>();
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand("SELECT * FROM Customers WHERE IsActive = 1", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                customers.Add(new Customer
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    Phone = reader.IsDBNull("Phone") ? null : reader.GetString("Phone"),
                    Balance = reader.GetDecimal("Balance"),
                    IsActive = reader.GetBoolean("IsActive")
                });
            }

            return customers;
        }

        public void Add(Customer customer)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand(@"
                INSERT INTO Customers (Name, Phone, Balance) 
                VALUES (@Name, @Phone, @Balance);
                SELECT SCOPE_IDENTITY();", connection);

            command.Parameters.AddWithValue("@Name", customer.Name);
            command.Parameters.AddWithValue("@Phone", customer.Phone ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Balance", customer.Balance);

            var newId = Convert.ToInt32(command.ExecuteScalar());

            var changeData = new Dictionary<string, object?>
            {
                ["Id"] = newId,
                ["Name"] = customer.Name,
                ["Phone"] = customer.Phone,
                ["Balance"] = customer.Balance,
                ["IsActive"] = true
            };

            _changeDetection.RecordChange("Customers", newId, "INSERT", changeData);
        }

        public void Update(Customer customer)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand("UPDATE Customers SET Name = @Name, Phone = @Phone, Balance = @Balance WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", customer.Id);
            command.Parameters.AddWithValue("@Name", customer.Name);
            command.Parameters.AddWithValue("@Phone", customer.Phone ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Balance", customer.Balance);

            command.ExecuteNonQuery();

            var changeData = new Dictionary<string, object?>
            {
                ["Id"] = customer.Id,
                ["Name"] = customer.Name,
                ["Phone"] = customer.Phone,
                ["Balance"] = customer.Balance,
                ["IsActive"] = customer.IsActive
            };

            _changeDetection.RecordChange("Customers", customer.Id, "UPDATE", changeData);
        }

        public void UpdateBalance(int customerId, decimal newBalance)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand("UPDATE Customers SET Balance = @Balance WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", customerId);
            command.Parameters.AddWithValue("@Balance", newBalance);

            command.ExecuteNonQuery();

            var changeData = new Dictionary<string, object?>
            {
                ["Id"] = customerId,
                ["Balance"] = newBalance
            };

            _changeDetection.RecordChange("Customers", customerId, "UPDATE_BALANCE", changeData);
        }

        public void Delete(int id)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand("UPDATE Customers SET IsActive = 0 WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();

            var changeData = new Dictionary<string, object?>
            {
                ["Id"] = id,
                ["IsActive"] = false
            };

            _changeDetection.RecordChange("Customers", id, "DELETE", changeData);
        }

        public Customer GetById(int id)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand("SELECT * FROM Customers WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Customer
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    Phone = reader.IsDBNull("Phone") ? null : reader.GetString("Phone"),
                    Balance = reader.GetDecimal("Balance"),
                    IsActive = reader.GetBoolean("IsActive")
                };
            }

            return null;
        }
    }
}