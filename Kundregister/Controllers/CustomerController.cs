using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Kundregister.Entities;
using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Kundregister.Controllers
{
    [Route("api/customers")]
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;
        private DatabaseContext databaseContext;
        private Models.CustomerRepository cr;

        public CustomerController(DatabaseContext databaseContext, ILogger<CustomerController> logger)
        {
            this.databaseContext = databaseContext;
            cr = new Models.CustomerRepository();

            _logger = logger;
        }

        [HttpPost]
        public IActionResult Add(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Add customer called - Failed");
                return BadRequest(ModelState);

            }
            else
            {
                cr.AddCustomer(customer, databaseContext);

                _logger.LogInformation("Add customer called - Success");

                return Ok(customer.Id);
            }
        }

        [HttpGet, Route("getallcustomers")]
        public List<Customer> GetAllCustomers()
        {
            var listOfCustomers = GetCustomerList();

            _logger.LogInformation("Get all called");

            return listOfCustomers;
        }

        public List<Customer> GetCustomerList()
        {
            return databaseContext.GetAllCostumers();
        }

        [HttpGet, Route("getusingid")]
        public IActionResult GetUsingId(int id)
        {
            var customer = cr.GetCustomerById(id, databaseContext);

            if (customer != null)
            {
                return Ok($"{customer.Id}. {customer.FirstName} {customer.LastName} - {customer.Gender} - {customer.Email} - {customer.Age} years old");
            }

            else return NotFound($"A customer with the Id {id} was not found");
        }
        public string CapitalizeFirstLetterWithoutTouchingTheRest(string input)
        {
            var output = char.ToUpper(input[0]) + input.Substring(1);
            return output;
        }

        [HttpPost, Route("editcustomer")]
        public IActionResult EditCustomer(string name, string pk, string value)
        {
            bool ifSomethingFails = false;

            var selectedId = int.Parse(pk);
            var customerToEdit = cr.GetCustomerById(selectedId, databaseContext);

            string capitalizedPropertyName = CapitalizeFirstLetterWithoutTouchingTheRest(name);

            cr.UpdateCustomer(customerToEdit, capitalizedPropertyName, value, databaseContext);

            if (!ifSomethingFails)
            {
                databaseContext.SaveChanges();
                return Ok($"Updated {capitalizedPropertyName} of id: {customerToEdit.Id} to {value}");
            }
            else
            {
                return BadRequest("I have failed you.");
            }
        }

        [HttpPost, Route("deletecustomer")]
        public IActionResult DeleteCustomer(int id)
        {
            var customerToRemove = cr.GetCustomerById(id, databaseContext);
            cr.RemoveCustomer(customerToRemove, databaseContext);

            return Ok("Customer removed");
        }

        [HttpGet, Route("countcustomers")]
        public IActionResult CountCustomers()
        {
            return Ok(databaseContext.GetAllCostumers().Count());
        }

        [HttpGet, Route("seedcustomers")]
        public IActionResult SeedCustomers()
        {
            string dataLocation = "wwwroot/data.txt";

            cr.SeedCustomers(dataLocation, databaseContext);

            return Ok("seeded database");
        }

        [HttpGet, Route("showcustomeraddresses")]
        public List<Address> ShowCustomerAddresses(int id)
        {
            return databaseContext.GetAllAddresses(id);
        }

        [HttpGet, Route("addnewaddress")]
        public IActionResult AddNewAddress(int id)
        {
            Address newAddress = new Address
            {
                Area = "a",
                PostalCode = 11111,
                StreetName = "AAA",
                StreetNumber = 111
            };
            databaseContext.Add(newAddress);
            databaseContext.SaveChanges();

            int newAddressId = databaseContext.Addresses.Single(address => address == newAddress).Id;

            CustomerToAddressRelations relation = new CustomerToAddressRelations
            {
                AdressId = newAddressId,
                CustId = id
            };
            databaseContext.Add(relation);
            databaseContext.SaveChanges();

            return Ok("new address added");
        }

        [HttpPost, Route("deleteaddressrelation")]
        public IActionResult DeleteAddressRelation(int custId, int addressId)
        {
            var addressToRemove = databaseContext.Addresses.Single(address => address.Id == addressId);
            var relationsToRemove = databaseContext.Relations.Where(relation => relation.AdressId == addressId);
            foreach (var relation in relationsToRemove)
            {
                databaseContext.Remove(relation);
            }
            databaseContext.Remove(addressToRemove);
            databaseContext.SaveChanges();

            return Ok("address removed");
        }
    }
}
