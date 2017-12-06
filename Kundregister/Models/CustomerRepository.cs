using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kundregister.Entities;

namespace Kundregister.Models
{
    public class CustomerRepository : ICustomerRepository
    {
        public void AddCustomer(Customer newCustomer, DatabaseContext databaseContext)
        {
            newCustomer.DateCreated = DateTime.Now;
            databaseContext.Add(newCustomer);
            databaseContext.SaveChanges();
        }

        public IEnumerable<Customer> GetAllCustomers(DatabaseContext databaseContext)
        {
            return databaseContext.GetAllCustomers();
        }

        public Customer GetCustomerById(int idOfCustomer, DatabaseContext databaseContext)
        {
            var listOfCustomers = GetAllCustomers(databaseContext);
            return databaseContext.GetCustomerById(idOfCustomer);
        }

        public void RemoveCustomer(Customer customerToRemove, DatabaseContext databaseContext)
        {
            databaseContext.Remove(customerToRemove);
            databaseContext.SaveChanges();
        }

        public void SeedCustomers(string fileLocation, DatabaseContext databaseContext)
        {
            var customers = getCustomersFromTextFile(fileLocation);

            databaseContext.Customers.RemoveRange(databaseContext.Customers);
            databaseContext.Addresses.RemoveRange(databaseContext.Addresses);
            databaseContext.Relations.RemoveRange(databaseContext.Relations);
            databaseContext.AddRange(customers);

            databaseContext.SaveChanges();
        }

        private List<Customer> getCustomersFromTextFile(string fileLocation)
        {
            var dataSet = System.IO.File.ReadAllLines(fileLocation);

            var customers = new List<Customer>();

            foreach (var customer in dataSet)
            {
                string[] splitString = customer.Split(",");
                customers.Add(new Customer
                {
                    FirstName = splitString[1],
                    LastName = splitString[2],
                    Gender = splitString[3],
                    Email = splitString[4],
                    Age = int.Parse(splitString[5]),
                    DateCreated = DateTime.Now
                });
            }
            return customers;
        }

        public void UpdateCustomer(Customer CustomerToEdit, string nameOfThePropertyToUpdateTheValueOf, string newValue, DatabaseContext databaseContext)
        {
            CustomerToEdit.DateEdited = DateTime.Now;

            var property = CustomerToEdit.GetType().GetProperties()
                .SingleOrDefault(prop => prop.Name.Equals(nameOfThePropertyToUpdateTheValueOf));

            if (property.PropertyType == typeof(int)) property.SetValue(CustomerToEdit, int.Parse(newValue));
            else property.SetValue(CustomerToEdit, newValue);
        }
    }
}
