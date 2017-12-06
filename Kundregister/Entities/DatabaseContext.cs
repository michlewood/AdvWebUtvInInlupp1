using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kundregister.Entities
{
    public partial class DatabaseContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> context) : base(context)
        {

        }

        internal Customer GetCustomerById(int id)
        {
            return Customers.SingleOrDefault(customer => customer.Id == id);
        }
    }
}
