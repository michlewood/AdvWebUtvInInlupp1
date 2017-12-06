using Kundregister.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kundregister.Models
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetAllCustomers(DatabaseContext databaseContext);
        Customer GetCustomerById(int id, DatabaseContext databaseContext);
        void AddCustomer(Customer newCustomer, DatabaseContext databaseContext);
        void RemoveCustomer(Customer customerToRemove, DatabaseContext databaseContext);
        void UpdateCustomer(Customer CustomerToEdit, string nameOfPropertyToUpdateValue, string newValue, DatabaseContext databaseContext);
        void SeedCustomers(string dataLocation, DatabaseContext databaseContext);
    }
}
