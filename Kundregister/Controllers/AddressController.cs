using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kundregister.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Kundregister.Controllers
{
    [Route("api/address")]
    public class AddressController : Controller
    {
        private readonly ILogger<AddressController> _logger;
        private DatabaseContext databaseContext;
        private Models.CustomerRepository customerRepository;

        public AddressController(DatabaseContext databaseContext, ILogger<AddressController> logger)
        {
            this.databaseContext = databaseContext;
            customerRepository = new Models.CustomerRepository(databaseContext);

            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Address> GetAllAddresses()
        {
            var listOfAddresses = databaseContext.Addresses;

            _logger.LogInformation("GetAllAddresses called");

            return listOfAddresses;
        }

        [HttpPost]
        public IActionResult AddAddress(Address address)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogInformation("AddAddress called - Failed");
                return BadRequest(ModelState);
            }
            else
            {
                databaseContext.Add(address);
                databaseContext.SaveChanges();

                _logger.LogInformation("AddAddress called - Success");

                return Ok(address.Id);
            }
        }

        [HttpPost, Route("{pk}")]
        public IActionResult EditAddress(string name, string pk, string value)
        {
            var addressId = int.Parse(pk);
            var capitalizedPropertyName = CapitalizeFirstLetterWithoutTouchingTheRest(name);
            var addressToEdit = databaseContext.Addresses.SingleOrDefault(address => address.Id == addressId);

            bool worked = customerRepository.UpdateAddress(addressToEdit, capitalizedPropertyName, value);

            if (worked)
            {
                databaseContext.SaveChanges();
                _logger.LogInformation("EditAddress called - Success");
                return Ok($"Updated {capitalizedPropertyName} of id: {addressToEdit.Id} to {value}");
            }

            else
            {
                _logger.LogInformation("EditAddress called - Failed");
                return BadRequest("incorrect value");
            }
        }

        [HttpDelete, Route("{addressId}")]
        public IActionResult RemoveAddress(int addressId)
        {
            RemoveRelationsByAddressId(addressId);
            var addressToRemove = databaseContext.Addresses.Single(address => address.Id == addressId);
            databaseContext.Remove(addressToRemove);
            databaseContext.SaveChanges();

            _logger.LogInformation("RemoveAddress called - Success");
            return Ok("address and all relations removed");
        }

        [HttpDelete, Route("address/{addressId}/relations")]
        private IActionResult RemoveRelationsByAddressId(int addressId)
        {
            var relationsToRemove = databaseContext.Relations.Where(relation => relation.AdressId == addressId);
            foreach (var relation in relationsToRemove)
            {
                databaseContext.Remove(relation);
            }

            _logger.LogInformation("RemoveRelationsByAddressId called - Success");
            return Ok("Relations removed");
        }

        public string CapitalizeFirstLetterWithoutTouchingTheRest(string input)
        {
            var output = char.ToUpper(input[0]) + input.Substring(1);
            return output;
        }
    }
}
