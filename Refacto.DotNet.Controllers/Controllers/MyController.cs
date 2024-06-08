using Microsoft.AspNetCore.Mvc;
using Refacto.DotNet.Controllers.Dtos.Product;
using Refacto.DotNet.Controllers.Services;

namespace Refacto.DotNet.Controllers.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _os;

        public OrdersController(IOrderService os)
        {
            _os = os;
        }

        /// <summary>
        /// Process order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>processed order</returns>
        [HttpPost("{orderId}/processOrder")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ProcessOrderResponse>> ProcessOrder(long orderId)
        {
            return Ok(await _os.ProcessOrder(orderId));
        }
    }
}
