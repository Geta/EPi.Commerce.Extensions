using System;
using Mediachase.Commerce;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Security;

namespace Geta.EPi.Commerce.Extensions.Cart
{
    public class CartFactory
    {
        private readonly ICurrentMarket _currentMarket;

        public CartFactory(ICurrentMarket currentMarket)
        {
            _currentMarket = currentMarket;
        }

        /// <summary>
        ///     Returns cart by given cart name for the current user. Creates new one if it does not already exist.
        /// </summary>
        /// <param name="cartName">Name of the cart. If empty, the default cart name is used.</param>
        /// <returns></returns>
        public virtual Mediachase.Commerce.Orders.Cart GetOrCreate(string cartName)
        {
            cartName = string.IsNullOrWhiteSpace(cartName) ? Mediachase.Commerce.Orders.Cart.DefaultName : cartName;

            var marketId = _currentMarket.GetCurrentMarket().MarketId;
            var userId = SecurityContext.Current.CurrentContactId;

            var cart = OrderContext.Current.GetCart(cartName, userId, marketId);

            if (string.IsNullOrEmpty(cart.CustomerName) ||
                cart.CustomerName.Equals(SecurityContext.Anonymous, StringComparison.OrdinalIgnoreCase))
            {
                cart.CustomerName = SecurityContext.Current.CurrentUserName;
            }

            return cart;
        }
    }
}