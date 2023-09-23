using Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Inventory;
using Shared.SeedWord;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _service;

        public InventoryController(IInventoryService service)
        {
            _service = service;
        }
        
        [Route("items/{itemNo}", Name = "GetAllByItemNo")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InventoryEntryDto>), (int)HttpStatusCode.OK)] 
        public async Task<ActionResult<IEnumerable<InventoryEntryDto>>> GetAllByItemNo([Required] string itemNo)
        {
            var result = await _service.GetAllByItemNoAsync(itemNo);
            return Ok(result);
        }

        [Route("items/{itemNo}/paging", Name = "GetAllByItemNoPaging")]
        [HttpGet]
        [ProducesResponseType(typeof(PageList<InventoryEntryDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PageList<InventoryEntryDto>>> GetAllByItemNoPaging([Required] string itemNo, [FromQuery] GetInventoryPagingQuery query)
        {
            query.SetItemNo(itemNo);
            var result = await _service.GetAllByItemNoPagingAsync(query);
            return Ok(result);
        }

        [Route("{id}", Name = "GetInventoryById")]
        [HttpGet]
        [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<InventoryEntryDto>> GetById([Required] string id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("purchase/{itemNo}", Name = "PurchaseOrder")]
        [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<InventoryEntryDto>> PurchaseOrder([Required] string itemNo,
            [FromBody] PurchaseProductDto model)
        {
            model.SetItemNo(itemNo);
            var result = await _service.PurchaseItemAsync(itemNo, model);
            return Ok(result);
        }

        [HttpPost("sales/{itemNo}", Name = "SalesItem")]
        [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<InventoryEntryDto>> SalesItem([Required] string itemNo,
            [FromBody] SalesProductDto model)
        {
            model.SetItemNo(itemNo);
            var result = await _service.SalesItemAsync(itemNo, model);
            return Ok(result);
        }

        [HttpDelete("{id}", Name = "DeleteById")]
        [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<InventoryEntryDto>> PurchaseOrder([Required] string id)
        {
            var inventory = await _service.GetByIdAsync(id);
            if (inventory == null)
                return NotFound();


            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("sales/order-no/{orderNo}", Name = "SalesOrder")]
        [ProducesResponseType(typeof(CreatedOrderSuccessDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CreatedOrderSuccessDto>> SalesOrder([Required] string orderNo,
           [FromBody] SalesOrderDto model)
        {
            model.OrderNo = orderNo;
            var documentNo = await _service.SalesOrderAsync(model);
            var result = new CreatedOrderSuccessDto(documentNo);
            return Ok(result);
        }
    }
}
