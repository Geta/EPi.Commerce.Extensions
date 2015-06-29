using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Geta.EPi.Commerce.Extensions.Order;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Managers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Website.Helpers;

namespace Geta.EPi.Commerce.Extensions.Cart
{
    public static class CartExtensions
    {
        /// <summary>
        /// Gets the cart promotions based on promotion type or All.
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="promoKey">The promotion group key.</param>
        /// <returns></returns>
        public static IEnumerable<string> GetCartPromotions(this Mediachase.Commerce.Orders.Cart cart, PromotionGroup.PromotionGroupKey? promoKey)
        {
            IEnumerable<Discount> discounts;
            OrderForm[] allOrderForms = cart.OrderForms.ToArray();
            IEnumerable<Shipment> allShipments;
            IEnumerable<LineItem> allLineItems;

            switch (promoKey)
            {
                case PromotionGroup.PromotionGroupKey.Order:
                    discounts = allOrderForms.SelectMany(x => x.Discounts.Cast<Discount>());
                    break;
                case PromotionGroup.PromotionGroupKey.Shipping:
                    allShipments = allOrderForms.SelectMany(x => x.Shipments.ToArray());
                    discounts = allShipments.SelectMany(x => x.Discounts.Cast<Discount>());
                    break;
                case PromotionGroup.PromotionGroupKey.Entry:
                    allLineItems = allOrderForms.SelectMany(x => x.LineItems.ToArray());
                    discounts = allLineItems.SelectMany(x => x.Discounts.Cast<Discount>());
                    break;
                default:
                    // Get all discounts
                    allShipments = allOrderForms.SelectMany(x => x.Shipments.ToArray());
                    allLineItems = allOrderForms.SelectMany(x => x.LineItems.ToArray());
                    discounts = allOrderForms.SelectMany(x => x.Discounts.Cast<Discount>());
                    discounts = discounts.Concat(allShipments.SelectMany(x => x.Discounts.Cast<Discount>()));
                    discounts = discounts.Concat(allLineItems.SelectMany(x => x.Discounts.Cast<Discount>()));
                    break;
            }

            //Grouping discounts by promotion type
            var groupingDiscounts = discounts.GroupBy(x => x.DiscountId);

            PromotionDto dto = PromotionManager.GetPromotionDto();
            foreach (var groupDiscount in groupingDiscounts)
            {
                var promotionId = groupDiscount.Key;
                PromotionDto.PromotionRow promotion = dto.Promotion.SingleOrDefault(x => x.PromotionId == promotionId);
                if (promotion != null)
                {
                    var retVal = GetPromotionDisplayName(promotion, Thread.CurrentThread.CurrentCulture.Name);
                    if (string.IsNullOrEmpty(retVal))
                    {
                        retVal = promotion.Name;
                    }
                    yield return retVal;
                }
            }
        }

        private static string GetPromotionDisplayName(PromotionDto.PromotionRow row, string languageCode)
        {
            PromotionDto.PromotionLanguageRow[] langRows = row.GetPromotionLanguageRows();
            if (langRows != null && langRows.Length > 0)
            {
                foreach (PromotionDto.PromotionLanguageRow lang in langRows)
                {
                    if (lang.LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase))
                    {
                        return lang.DisplayName;
                    }
                }
            }

            return row.Name;
        }

        /// <summary>
        /// Creates order manager class instance
        /// </summary>
        /// <param name="cart">The cart for order manager</param>
        /// <returns></returns>
        public static OrderManager GetOrderManager(this Mediachase.Commerce.Orders.Cart cart)
        {
            return new OrderManager(cart);
        }

        /// <summary>
        /// Returns all line items of the cart regardless of count of order forms in it
        /// </summary>
        /// <param name="cart">Cart that contains line items</param>
        /// <returns>All line items of the given cart</returns>
        public static IReadOnlyCollection<LineItem> GetAllLineItems(this Mediachase.Commerce.Orders.Cart cart)
        {
            return cart.OrderForms.Any() ? cart.OrderForms.First().LineItems.ToList() : new List<LineItem>();
        }

        /// <summary>
        /// Returns the first line item that matches the code
        /// </summary>
        /// <param name="cart">Cart that contains line items</param>
        /// <param name="code">The catalog entry code</param>
        /// <returns>Line item matching the code of the given cart</returns>
        public static LineItem GetLineItem(this Mediachase.Commerce.Orders.Cart cart, string code)
        {
            return cart.GetAllLineItems().FirstOrDefault(x => x.Code == code);
        }

        /// <summary>
        /// Returns new CartHelper instance for given cart
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        public static CartHelper GetCartHelper(this Mediachase.Commerce.Orders.Cart cart)
        {
            return new CartHelper(cart);
        }

        /// <summary>
        /// Runs "CartValidate" workflow with option to save cart and add validation error messages to given param list
        /// </summary>
        /// <param name="cart">Cart which needs to validate</param>
        /// <param name="warnings">List that gets validation messages added, can be null</param>
        /// <param name="updateCart">If true, cart is saved</param>
        /// <param name="deleteIfEmpty">If true and cart is empty and parameter updateCart is true, cart is deleted and saved</param>
        /// <returns>Cart</returns>
        public static Mediachase.Commerce.Orders.Cart RunValidateWorkflow(this Mediachase.Commerce.Orders.Cart cart,
            ICollection<string> warnings = null,
            bool updateCart = false,
            bool deleteIfEmpty = false)
        {
            return cart.RunWorkflow(CartWorkflow.CartValidate, warnings, updateCart, deleteIfEmpty);
        }

        /// <summary>
        /// Runs requires workflow with option to save cart and add workflow error messages to given param list
        /// </summary>
        /// <param name="cart">Cart upon which to run the worklfow</param>
        /// <param name="warnings">List that gets validation messages added, can be null</param>
        /// <param name="updateCart">If true, cart is saved</param>
        /// <param name="deleteIfEmpty">If true and cart is empty and parameter updateCart is true, cart is deleted and saved</param>
        /// <returns>Cart</returns>
        public static Mediachase.Commerce.Orders.Cart RunWorkflow(this Mediachase.Commerce.Orders.Cart cart,
            CartWorkflow workflow,
            ICollection<string> warnings = null,
            bool updateCart = false,
            bool deleteIfEmpty = false)
        {
            if (cart.GetCartHelper().IsEmpty)
            {
                if (deleteIfEmpty && updateCart)
                {
                    cart.Delete();
                    cart.AcceptChanges();
                }

                return cart;
            }

            var results = cart.RunWorkflow(Enum.GetName(typeof(CartWorkflow), workflow));
            var workflowWarnings = OrderGroupWorkflowManager.GetWarningsFromWorkflowResult(results);

            if (warnings != null)
            {
                foreach (var warning in workflowWarnings)
                {
                    warnings.Add(warning);
                }
            }

            if (updateCart)
            {
                cart.AcceptChanges();
            }

            return cart;
        }
    }
}