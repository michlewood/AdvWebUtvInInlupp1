using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kundregister.Entities
{
    public partial class DatabaseContext : DbContext
    {
        public DbSet<Address> Addresses { get; set; }

        public DbSet<CustomerToAddressRelations> Relations { get; set; }

        internal List<Address> GetAllAddresses(int custId)
        {
            var databaseListofCustomerAddresses = new List<Address>();

            var relationsOfCustId = Relations.Where(address => address.CustId == custId);
            foreach (var relation in relationsOfCustId)
            {
                databaseListofCustomerAddresses.Add(Addresses.Single(address => address.Id == relation.AdressId));
            }

            return databaseListofCustomerAddresses;
        }

        internal Address GetAddressById(int id)
        {
            return Addresses.SingleOrDefault(address => address.Id == id);
        }
    }
}
