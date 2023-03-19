using InventoryTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace InventoryTracking.Controllers
{
    
    [RoutePrefix("api/inventory")]
    public class InventoryController : ApiController
    {
       
        private static List<InventoryItem> inventory = new List<InventoryItem>
        {
            new InventoryItem { Name = "Apples", Quantity = 3, CreatedOn = "2020-01-01" },
            new InventoryItem { Name = "Oranges", Quantity = 7, CreatedOn = "2020-02-01" },
            new InventoryItem { Name = "Pomegranates", Quantity = 55, CreatedOn = "2020-01-10" }
        };


        // GET api/inventory
       // [Route("api/inventory")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(inventory);
        }

        // GET api/inventory/Apples
        [Route("{itemName}")]
        [HttpGet]
        public IHttpActionResult Get(string itemName)
        {
            // Check if item exists in inventory
            InventoryItem item = inventory.Find(i => i.Name.ToLower() == itemName.ToLower());
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }


        [HttpPost]     
        public async Task<IHttpActionResult> AddInventoryItem([FromBody] List<InventoryItem> items)
        {
            await Task.Delay(1000);

            // Check if item already exists in inventory
            foreach (InventoryItem item in items)
            {
                InventoryItem existingItem = inventory.FirstOrDefault(i => i.Name == item.Name);

                if (existingItem != null)
                {
                    // Item already exists, return 409 Conflict response
                    return Conflict();
                }
                else
                {
                    // Item does not exist, add to inventory and return 201 Created response
                    item.CreatedOn = DateTime.Now.Date.ToString();
                    inventory.Add(item);
                    
                }
            }
            return Created(Request.RequestUri, items);
        }


        // PUT /inventory/{itemName}
        [HttpPut]
        [Route("{itemName}")]
        public IHttpActionResult UpdateInventoryItem(string itemName, [FromBody] InventoryItem updatedItem)
        {
            var item = inventory.FirstOrDefault(i => i.Name == itemName);
            if (item == null)
            {
                // Create a new item if not found
                inventory.Add(updatedItem);
                return Created(Request.RequestUri + updatedItem.Name, updatedItem);
            }
            else
            {
                // Update the existing item
                item.Quantity = updatedItem.Quantity;
                item.CreatedOn = updatedItem.CreatedOn;
                return Ok(item);
            }
        }

        // Define a route for the delete operation
        [HttpDelete]
        [Route("{itemName}")]
        public  async Task<IHttpActionResult> DeleteItem(string itemName)
        {

            await Task.Delay(1000);
            // Lookup the inventory item by its ID
            InventoryItem itemToDelete =  inventory.FirstOrDefault(i => i.Name == itemName);
            if (itemToDelete == null)
            {
                // Return a 404 Not Found response if the item doesn't exist
                return NotFound();
            }

            // Delete the item from the repository
            inventory.Remove(itemToDelete);

            // Return a 204 No Content response to indicate success
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpGet]
        [Route("name/{itemName}")]
        public InventoryItem GetInventoryByName(string itemName)
        {
            return inventory.Find(i => i.Name == itemName);
        }

        // GET /highest-quantity
        [Route("highest-quantity")]
        public IHttpActionResult GetHighestQuantity()
        {
            var item = inventory.OrderByDescending(i => i.Quantity).FirstOrDefault();
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [Route("lowest-quantity")]
        public IHttpActionResult GetLowestQuantity()
        {
            var item = inventory.OrderBy(i => i.Quantity).FirstOrDefault();
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [Route("oldest-item")]
        public IHttpActionResult GetInventoryByOldestItem()
        {
             var item = inventory.OrderBy(i => i.CreatedOn).FirstOrDefault();
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }


        // GET /newest-item
        [Route("newest-item")]
        public IHttpActionResult GetInventoryByNewestItem()
        {
            var item =  inventory.OrderByDescending(i => i.CreatedOn).FirstOrDefault();
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpGet]
        [Route("search/{keyword}")]
        public IHttpActionResult SearchInventory(string keyword)
        {
            var items = inventory.Where(item => item.Name.Contains(keyword)).ToList();
            if (items == null)
            {
                return NotFound();
            }
            return Ok(items);
        }

        [HttpGet]
        [Route("sort/{attribute}")]
        public IHttpActionResult SortInventory(string attribute)
        {
            switch (attribute)
            {
                case "name":
                    return Ok(inventory.OrderBy(item => item.Name));
                case "quantity":
                    return Ok(inventory.OrderBy(item => item.Quantity));
                case "createdOn":
                    return Ok(inventory.OrderBy(item => item.CreatedOn));
                default:
                    return Ok(inventory);
            }
        }
    }
}
