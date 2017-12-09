using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Kundregister.Entities;
using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Kundregister.Models;

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

            _logger.LogInformation("GetAllCustomers called");

            return listOfCustomers;
        }

        [HttpPost]
        public IActionResult AddCustomer(AddCustomerVM addCustomerVM)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("AddCustomer called - Failed");
                return BadRequest(ModelState);
            }
            else
            {
                var newCustomer = new Customer
                {
                    FirstName = addCustomerVM.FirstName,
                    LastName = addCustomerVM.LastName,
                    Email = addCustomerVM.Email,
                    Gender = addCustomerVM.Gender,
                    Age = addCustomerVM.Age,
                    DateCreated = addCustomerVM.DateCreated
                };
                customerRepository.AddCustomer(newCustomer);

                _logger.LogInformation("AddCustomer called - Success");

                return Ok(newCustomer.Id);
            }
        }

        [HttpGet, Route("{id}")]
        public IActionResult GetCustomerById(int id)
        {
            var customer = customerRepository.GetCustomerById(id);

            if (customer != null)
            {
                _logger.LogInformation("GetCustomerById called - Success");
                return Ok(customer);
            }

            else
            {
                _logger.LogInformation("GetCustomerById called - Failed");
                return NotFound($"A customer with the Id {id} was not found");
            }
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
                _logger.LogInformation("EditCustomer called - Success");
                return Ok($"Updated {capitalizedPropertyName} of id: {customerToEdit.Id} to {value}");
            }

            else
            {
                _logger.LogInformation("EditCustomer called - Failed");
                return BadRequest("incorrect value");
            }
        }

        [HttpDelete, Route("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            var customerToRemove = customerRepository.GetCustomerById(id);
            customerRepository.RemoveCustomer(customerToRemove);

            _logger.LogInformation("DeleteCustomer called - Success");
            return Ok("Customer removed");
        }

        [HttpGet, Route("count")]
        public IActionResult CountCustomers()
        {
            _logger.LogInformation("CountCustomers called - Success");
            return Ok(customerRepository.CountCustomers());
        }

        [HttpGet, Route("seed")]
        public IActionResult SeedCustomers()
        {
            string dataLocation = "wwwroot/data.txt";

            customerRepository.SeedCustomers(dataLocation);

            _logger.LogInformation("SeedCustomers called - Success");
            return Ok("seeded database");
        }

        [HttpGet, Route("{id}/address")]
        public IEnumerable<Address> GetCustomerAddresses(int id)
        {
            _logger.LogInformation("GetCustomerAddresses called - Success");
            return customerRepository.GetCustomerAddresses(id);
        }

        [HttpPost, Route("{custId}/address/{addressId}")]
        public IActionResult AddRelationsBetweenCustomerAndAddress(AddRelationVM addRelationVM)
        {
            if (ModelState.IsValid)
            {
                if (databaseContext.Relations.Any(relation => relation.CustId == addRelationVM.CustId && relation.AdressId == addRelationVM.AddressId))
                {
                    _logger.LogInformation("AddRelationsBetweenCustomerAndAddress called - Failed");
                    return BadRequest("Relation already exists");
                }
                else
                {
                    customerRepository.GetCustomerById(addRelationVM.CustId).DateEdited = DateTime.Now;
                    var newRelation = new CustomerToAddressRelations
                    {
                        CustId = addRelationVM.CustId,
                        AdressId = addRelationVM.AddressId
                    };
                    databaseContext.Add(newRelation);
                    databaseContext.SaveChanges();
                    _logger.LogInformation("AddRelationsBetweenCustomerAndAddress called - Success");
                    return Ok("Relations Created");
                }
            }
            else
            {
                return BadRequest("Invalid Input");
            }
        }

        [HttpDelete, Route("{custId}/address/{addressId}")]
        public IActionResult RemoveRelationsBetweenCustomerAndAddress(int custId, int addressId)
        {
            var relationsToRemove = databaseContext.Relations.Where(relation => relation.AdressId == addressId && relation.CustId == custId);
            foreach (var relation in relationsToRemove)
            {
                databaseContext.Remove(relation);
            }
            customerRepository.GetCustomerById(custId).DateEdited = DateTime.Now;
            databaseContext.SaveChanges();
            _logger.LogInformation("RemoveRelationsBetweenCustomerAndAddress called - Success");
            return Ok("Relations removed");
        }

        public string CapitalizeFirstLetterWithoutTouchingTheRest(string input)
        {
            var output = char.ToUpper(input[0]) + input.Substring(1);
            return output;
        }
    }
}
