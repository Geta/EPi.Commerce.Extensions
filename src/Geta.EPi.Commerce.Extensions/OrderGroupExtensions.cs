using EPiServer.Commerce.Order;
using Mediachase.Commerce;

namespace Geta.EPi.Commerce.Extensions
{
    public static class OrderGroupExtensions
    {
        public static Money ToMoney(this IOrderGroup orderGroup, decimal amount)
        {
            return new Money(amount, orderGroup.Currency);
        }
    }
}