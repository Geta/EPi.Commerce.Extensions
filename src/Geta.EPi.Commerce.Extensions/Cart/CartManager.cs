using System;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Managers;

namespace Geta.EPi.Commerce.Extensions.Cart
{
    public class CartManager
    {
        /// <summary>
        /// Deletes the current basket instance from the database.
        /// </summary>
        public virtual void Delete(Mediachase.Commerce.Orders.Cart cart)
        {
            // Remove any reservations
            // Load existing usage Dto for the current order
            PromotionUsageDto usageDto = PromotionManager.GetPromotionUsageDto(0, Guid.Empty, cart.OrderGroupId);

            // Clear all old items first
            if (usageDto.PromotionUsage.Count > 0)
            {
                foreach (PromotionUsageDto.PromotionUsageRow row in usageDto.PromotionUsage)
                {
                    if (row.OrderGroupId == cart.OrderGroupId)
                    {
                        row.Delete();
                    }
                }
            }

            // Save the promotion usage
            PromotionManager.SavePromotionUsage(usageDto);

            cart.Delete();
        }
    }
}