using Refacto.DotNet.Controllers.Dtos.Product;

namespace Refacto.DotNet.Controllers.Services
{
    public interface IOrderService
    {
        /// <summary>
        /// Process order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>processed order</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        Task<ProcessOrderResponse> ProcessOrder(long orderId);
    }
}
