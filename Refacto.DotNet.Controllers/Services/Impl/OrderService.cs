using Microsoft.EntityFrameworkCore;
using Refacto.DotNet.Controllers.Database.Context;
using Refacto.DotNet.Controllers.Dtos.Product;
using Refacto.DotNet.Controllers.Entities;

namespace Refacto.DotNet.Controllers.Services.Impl
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _ctx;
        private readonly IProductService _ps;

        public OrderService(AppDbContext ctx, IProductService ps)
        {
            _ctx = ctx;
            _ps = ps;
        }

        /// <summary>
        /// Process order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>processed order</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<ProcessOrderResponse> ProcessOrder(long orderId)
        {
            Order? order = await _ctx.Orders
                .Include(o => o.Items)
                .SingleOrDefaultAsync(o => o.Id == orderId)
                ?? throw new KeyNotFoundException($"The order is not found using the id {orderId}");

            if (order.Items is null || order.Items is { Count: 0 })
            {
                throw new Exception($"The order does not contain any products");
            }

            foreach (Product p in order.Items)
            {
                ProcessProduct(p);
            }
            await _ctx.SaveChangesAsync();

            return new ProcessOrderResponse(order.Id);
        }

        /// <summary>
        /// Process product by type
        /// </summary>
        /// <param name="p"></param>
        /// <exception cref="Exception"></exception>
        private void ProcessProduct(Product p)
        {
            switch (p.Type)
            {
                case "NORMAL":
                    ProcessNormalProduct(p);
                    break;

                case "SEASONAL":
                    ProcessPeriodicalProduct(p, p.SeasonStartDate, p.SeasonEndDate, true);
                    break;

                case "EXPIRABLE":
                    ProcessExpiredProduct(p);
                    break;

                case "FLASHSALE":
                    ProcessPeriodicalProduct(p, p.FlashsaleStartDate, p.FlashsaleEndDate, false);
                    break;

                default:
                    throw new Exception($"Le type de produit {p.Type} n'est pas géré");
            }
        }

        /// <summary>
        /// Process availability
        /// </summary>
        /// <param name="p"></param>
        /// <returns>True if product is available, False if not</returns>
        private static bool ProcessAvailability(Product p)
        {
            if (p.Available > 0)
            {
                p.Available -= 1;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Process normal product
        /// </summary>
        /// <param name="p"></param>
        private void ProcessNormalProduct(Product p)
        {
            if (!ProcessAvailability(p))
            {
                if (p.LeadTime > 0)
                {
                    _ps.NotifyDelay(p);
                }
            }
        }

        /// <summary>
        /// Process periodical products (seasonal, flashsale)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="sendNotif"></param>
        private void ProcessPeriodicalProduct(
            Product p, 
            DateTime? startDate, 
            DateTime? endDate, 
            bool sendNotif)
        {
            if (DateTime.Now.Date <= startDate && 
                DateTime.Now.Date >= endDate &&
                !ProcessAvailability(p) &&
                sendNotif)
            {
                _ps.HandleSeasonalProduct(p);
            }
        }

        /// <summary>
        /// Process expired product
        /// </summary>
        /// <param name="p"></param>
        private void ProcessExpiredProduct(Product p)
        {
            if (p.ExpiryDate <= DateTime.Now.Date && !ProcessAvailability(p))
            {
                _ps.HandleExpiredProduct(p);
            }
        }
    }
}
