using Kundregister.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kundregister.Models
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetAllCustomers();
        Customer GetCustomerById(int id);
        void AddCustomer(Customer newCustomer);
        void RemoveCustomer(Customer customerToRemove);
        bool UpdateCustomer(int id, string nameOfPropertyToUpdateValue, string newValue);
        void SeedCustomers(string dataLocation);
        int CountCustomers();

        IEnumerable<Address> GetCustomerAddresses(int id);

        CustomerToAddressRelations GetRelationByCustIdAndAddressId(int custId, int addressId);
        void AddRelationsBetweenCustomerAndAddress(CustomerToAddressRelations newRelation);


    }
}
