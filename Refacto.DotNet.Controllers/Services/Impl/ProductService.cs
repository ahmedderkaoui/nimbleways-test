using Refacto.DotNet.Controllers.Entities;

namespace Refacto.DotNet.Controllers.Services.Impl
{
    public class ProductService : IProductService
    {
        private readonly INotificationService _ns;

        public ProductService(INotificationService ns)
        {
            _ns = ns;
        }

        /// <summary>
        /// Notify the delay of product
        /// </summary>
        /// <param name="p"></param>
        public void NotifyDelay(Product p)
        {
            _ns.SendDelayNotification(p.LeadTime, p.Name);
        }

        /// <summary>
        /// Handle seasonal product notification
        /// </summary>
        /// <param name="p"></param>
        public void HandleSeasonalProduct(Product p)
        {
            if (DateTime.Now.AddDays(p.LeadTime) > p.SeasonEndDate ||
                p.SeasonStartDate > DateTime.Now)
            {
                _ns.SendOutOfStockNotification(p.Name);
            }
            else
            {
                NotifyDelay(p);
            }
        }

        /// <summary>
        /// Handle expired product notification
        /// </summary>
        /// <param name="p"></param>
        public void HandleExpiredProduct(Product p)
        {
            if (p.ExpiryDate.HasValue)
            {
                _ns.SendExpirationNotification(p.Name, p.ExpiryDate.Value);
            }
        }
    }
}
