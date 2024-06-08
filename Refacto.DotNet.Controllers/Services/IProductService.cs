using Refacto.DotNet.Controllers.Entities;

namespace Refacto.DotNet.Controllers.Services
{
    public interface IProductService
    {
        /// <summary>
        /// Notify the delay of product
        /// </summary>
        /// <param name="p"></param>
        void NotifyDelay(Product p);

        /// <summary>
        /// Handle seasonal product notification
        /// </summary>
        /// <param name="p"></param>
        void HandleSeasonalProduct(Product p);

        /// <summary>
        /// Handle expired product notification
        /// </summary>
        /// <param name="p"></param>
        void HandleExpiredProduct(Product p);
    }
}
