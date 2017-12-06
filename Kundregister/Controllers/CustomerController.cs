using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Kundregister.Entities;
using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Logging;

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
            customerRepository = new Models.CustomerRepository(databaseContext);

            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Customer> GetAllCustomers()
        {
            var listOfCustomers = customerRepository.GetAllCustomers();

            _logger.LogInformation("Get all called");

            return listOfCustomers;
        }

        [HttpPost]
        public IActionResult AddCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("AddCustomer called - Failed");
                return BadRequest(ModelState);
            }
            else
            {
                customerRepository.AddCustomer(customer);

                _logger.LogInformation("AddCustomer called - Success");

                return Ok(customer.Id);
            }
        }

        [HttpGet, Route("{id}")]
        public IActionResult GetUsingId(int id)
        {
            var customer = customerRepository.GetCustomerById(id);

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

        [HttpPost, Route("{pk}")]
        public IActionResult EditCustomer(string name, string pk, string value)
        {
            var selectedId = int.Parse(pk);
            var customerToEdit = customerRepository.GetCustomerById(selectedId);

            string capitalizedPropertyName = CapitalizeFirstLetterWithoutTouchingTheRest(name);

            bool worked = customerRepository.UpdateCustomer(customerToEdit, capitalizedPropertyName, value);

            if (worked)
            {
                databaseContext.SaveChanges();
                return Ok($"Updated {capitalizedPropertyName} of id: {customerToEdit.Id} to {value}");
            }

            else return BadRequest("incorrect value");
        }

        [HttpDelete, Route("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            var customerToRemove = customerRepository.GetCustomerById(id);
            customerRepository.RemoveCustomer(customerToRemove);

            return Ok("Customer removed");
        }

        [HttpGet, Route("count")]
        public IActionResult CountCustomers()
        {
            return Ok(databaseContext.Customers.Count());
        }

        [HttpGet, Route("seed")]
        public IActionResult SeedCustomers()
        {
            string dataLocation = "wwwroot/data.txt";

            customerRepository.SeedCustomers(dataLocation);

            return Ok("seeded database");
        }

        [HttpGet, Route("{id}/address")]
        public IEnumerable<Address> ShowCustomerAddresses(int id)
        {
            return databaseContext.GetAllAddressesForGivenCustomerId(id);
        }

        [HttpPost, Route("{id}/address")]
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

        [HttpDelete, Route("{custId}/address/{addressId}")]
        public IActionResult RemoveAddress(int custId, int addressId)
        {
            RemoveRelationsWithAddressId(addressId);
            var addressToRemove = databaseContext.Addresses.Single(address => address.Id == addressId);
            databaseContext.Remove(addressToRemove);
            databaseContext.SaveChanges();

            return Ok("address and all relations removed");
        }

        [HttpDelete, Route("address/{addressId}")]
        private IActionResult RemoveRelationsWithAddressId(int addressId)
        {
            var relationsToRemove = databaseContext.Relations.Where(relation => relation.AdressId == addressId);
            foreach (var relation in relationsToRemove)
            {
                databaseContext.Remove(relation);
            }

            return Ok("Relations removed");
        }

        [HttpPost, Route("{custId}/address/{pk}")]
        public IActionResult EditAddress(string name, string pk, string value)
        {
            var addressId = int.Parse(pk);
            var capitalizedPropertyName = CapitalizeFirstLetterWithoutTouchingTheRest(name);
            var addressToEdit = databaseContext.Addresses.SingleOrDefault(address => address.Id == addressId);

            bool worked = customerRepository.UpdateAddress(addressToEdit, capitalizedPropertyName, value);

            if (worked)
            {
                databaseContext.SaveChanges();
                return Ok($"Updated {capitalizedPropertyName} of id: {addressToEdit.Id} to {value}");
            }

            else return BadRequest("incorrect value");
        }
    }
}
