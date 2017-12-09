using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kundregister.Entities;

namespace Kundregister.Models
{
    public class CustomerRepository : ICustomerRepository
    {
        private DatabaseContext databaseContext;

        public CustomerRepository(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public void AddCustomer(Customer newCustomer)
        {
            newCustomer.DateCreated = DateTime.Now;
            databaseContext.Add(newCustomer);
            databaseContext.SaveChanges();
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return databaseContext.Customers;
        }

        public Customer GetCustomerById(int idOfCustomer)
        {
            var listOfCustomers = GetAllCustomers();
            return databaseContext.GetCustomerById(idOfCustomer);
        }

        public void RemoveCustomer(Customer customerToRemove)
        {
            databaseContext.Remove(customerToRemove);
            databaseContext.SaveChanges();
        }

        public void SeedCustomers(string fileLocation)
        {
            var customers = GetCustomersFromTextFile(fileLocation);

            databaseContext.Customers.RemoveRange(databaseContext.Customers);
            databaseContext.Addresses.RemoveRange(databaseContext.Addresses);
            databaseContext.Relations.RemoveRange(databaseContext.Relations);
            databaseContext.AddRange(customers);

            databaseContext.SaveChanges();
        }

        private IEnumerable<Customer> GetCustomersFromTextFile(string fileLocation)
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

        public bool UpdateCustomer(Customer customerToEdit, string nameOfThePropertyToUpdateTheValueOf, string newValue)
        {
            customerToEdit.DateEdited = DateTime.Now;

            var property = customerToEdit.GetType().GetProperties()
                .SingleOrDefault(prop => prop.Name.Equals(nameOfThePropertyToUpdateTheValueOf));

            bool worked = true;

            if (property.PropertyType == typeof(int))
            {
                worked = int.TryParse(newValue, out int parsedValue);
                if(worked) property.SetValue(customerToEdit, parsedValue);
            }
            else property.SetValue(customerToEdit, newValue);

            return worked;
        }

        public bool UpdateAddress(Address addressToEdit, string nameOfThePropertyToUpdateTheValueOf, string newValue)
        {
            var property = addressToEdit.GetType().GetProperties()
                .SingleOrDefault(prop => prop.Name.Equals(nameOfThePropertyToUpdateTheValueOf));

            bool worked = true;

            if (property.PropertyType == typeof(int))
            {
                worked = int.TryParse(newValue, out int parsedValue);
                if (worked) property.SetValue(addressToEdit, parsedValue);
            }
            else property.SetValue(addressToEdit, newValue);

            databaseContext.SaveChanges();
            return worked;
        }

        public int CountCustomers()
        {
            return databaseContext.Customers.Count();
        }

        public IEnumerable<Address> GetCustomerAddresses(int id)
        {
            return databaseContext.GetAllAddressesForGivenCustomerId(id);
        }
    }
}
