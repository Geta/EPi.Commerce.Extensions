using Mediachase.Commerce.Orders;

namespace Geta.EPi.Commerce.Extensions.Order
{
    public class OrderManager
    {
        private readonly Mediachase.Commerce.Orders.Cart _cart;

        public OrderManager(Mediachase.Commerce.Orders.Cart cart)
        {
            this._cart = cart;
        }

        /// <summary>
        /// Gets the named OrderForm.
        /// </summary>
        /// <param name="orderFormName">The name of the OrderForm object to retrieve.</param>
        /// <returns>The named OrderForm.</returns>
        public virtual OrderForm GetOrCreateOrderForm(string orderFormName = "")
        {
            if (string.IsNullOrEmpty(orderFormName))
            {
                orderFormName = _cart.Name;  // TODO use _cart.DefaultName, but it is static...
            }

            OrderForm orderForm = this._cart.OrderForms[orderFormName];
            if (orderForm == null)
            {
                orderForm = new OrderForm {Name = orderFormName};
                this._cart.OrderForms.Add(orderForm);
            }

            return orderForm;
        }   
    }
}