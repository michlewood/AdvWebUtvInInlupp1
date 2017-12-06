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
        //TODO: change how to add and remove addresses work.
        //TODO: make a new delete address relation that removes an address relation from a user.

        private readonly ILogger<CustomerController> _logger;
        private DatabaseContext databaseContext;
        private Models.CustomerRepository customerRepository;

        public CustomerController(DatabaseContext databaseContext, ILogger<CustomerController> logger)
        {
            this.databaseContext = databaseContext;
            customerRepository = new Models.CustomerRepository();

            _logger = logger;
        }

        [HttpPost, Route("addnewcustomer")]
        public IActionResult AddCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Add customer called - Failed");
                return BadRequest(ModelState);

            }
            else
            {
                customerRepository.AddCustomer(customer, databaseContext);

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
            return databaseContext.GetAllCustomers();
        }

        [HttpGet, Route("getusingid")]
        public IActionResult GetUsingId(int id)
        {
            var customer = customerRepository.GetCustomerById(id, databaseContext);

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
            var selectedId = int.Parse(pk);
            var customerToEdit = customerRepository.GetCustomerById(selectedId, databaseContext);

            string capitalizedPropertyName = CapitalizeFirstLetterWithoutTouchingTheRest(name);

            customerRepository.UpdateCustomer(customerToEdit, capitalizedPropertyName, value, databaseContext);

            databaseContext.SaveChanges();
            return Ok($"Updated {capitalizedPropertyName} of id: {customerToEdit.Id} to {value}");
        }

        [HttpPost, Route("deletecustomer")]
        public IActionResult DeleteCustomer(int id)
        {
            var customerToRemove = customerRepository.GetCustomerById(id, databaseContext);
            customerRepository.RemoveCustomer(customerToRemove, databaseContext);

            return Ok("Customer removed");
        }

        [HttpGet, Route("countcustomers")]
        public IActionResult CountCustomers()
        {
            return Ok(databaseContext.GetAllCustomers().Count());
        }

        [HttpGet, Route("seedcustomers")]
        public IActionResult SeedCustomers()
        {
            string dataLocation = "wwwroot/data.txt";

            customerRepository.SeedCustomers(dataLocation, databaseContext);

            return Ok("seeded database");
        }

        [HttpGet, Route("showcustomeraddresses")]
        public List<Address> ShowCustomerAddresses(int id)
        {
            return databaseContext.GetAllAddresses(id);
        }

        [HttpPost, Route("addnewaddress")]
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

        [HttpPost, Route("deleteaddress")]
        public IActionResult DeleteAddress(int custId, int addressId)
        {
            var addressToRemove = databaseContext.Addresses.Single(address => address.Id == addressId);
            var relationsToRemove = databaseContext.Relations.Where(relation => relation.AdressId == addressId);
            foreach (var relation in relationsToRemove)
            {
                databaseContext.Remove(relation);
            }
            databaseContext.Remove(addressToRemove);
            databaseContext.SaveChanges();

            return Ok("address and all relations removed");
        }
    }
}
