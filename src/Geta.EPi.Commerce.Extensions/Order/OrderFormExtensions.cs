using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.ServiceLocation;
using Geta.EPi.Commerce.Extensions.Entry;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Inventory;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Website.Helpers;
using Mediachase.MetaDataPlus.Configurator;

namespace Geta.EPi.Commerce.Extensions.Order
{
    public static class OrderFormExtensions
    {
        /// <summary>
        /// Adds the entry. Line item's qty will be increased by 1.
        /// </summary>
        /// <param name="orderForm"></param>
        /// <param name="entry">The entry.</param>
        public static OrderForm AddEntry(this OrderForm orderForm, Mediachase.Commerce.Catalog.Objects.Entry entry)
        {
            return AddEntry(orderForm, entry, 1, false);
        }

        /// <summary>
        /// Adds the entry. Line item's qty will be increased by 1.
        /// </summary>
        /// <param name="orderForm"></param>
        /// <param name="entry">The entry.</param>
        /// <param name="helpersToRemove"></param>
        public static OrderForm AddEntry(this OrderForm orderForm, Mediachase.Commerce.Catalog.Objects.Entry entry, params CartHelper[] helpersToRemove)
        {
            return AddEntry(orderForm, entry, 1, false, helpersToRemove);
        }

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <param name="orderForm"></param>
        /// <param name="entry">The entry.</param>
        /// <param name="fixedQuantity">If true, lineitem's qty will be set to <paramref name="fixedQuantity"/> value. Otherwise, <paramref name="fixedQuantity"/> will be added to the current line item's qty value.</param>
        public static OrderForm AddEntry(this OrderForm orderForm, Mediachase.Commerce.Catalog.Objects.Entry entry, bool fixedQuantity)
        {
            return AddEntry(orderForm, entry, 1, fixedQuantity);
        }

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <param name="orderForm"></param>
        /// <param name="entry">The entry.</param>
        /// <param name="fixedQuantity">If true, lineitem's qty will be set to <paramref name="fixedQuantity"/> value. Otherwise, <paramref name="fixedQuantity"/> will be added to the current line item's qty value.</param>
        /// <param name="helpersToRemove"></param>
        public static OrderForm AddEntry(this OrderForm orderForm, Mediachase.Commerce.Catalog.Objects.Entry entry, bool fixedQuantity, params CartHelper[] helpersToRemove)
        {
            return AddEntry(orderForm, entry, 1, fixedQuantity, helpersToRemove);
        }

        /// <summary>
        /// Adds the entry with default warehouse code
        /// </summary>
        /// <param name="orderForm"></param>
        /// <param name="entry">The entry.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="fixedQuantity">If true, lineitem's qty will be set to <paramref name="quantity"/> value. Otherwise, <paramref name="quantity"/> will be added to the current line item's qty value.</param>
        /// <param name="helpersToRemove">CartHelper(s) from which the item needs to be removed simultaneously with adding it to the current CartHelper.</param>
        public static OrderForm AddEntry(this OrderForm orderForm, Mediachase.Commerce.Catalog.Objects.Entry entry, decimal quantity, bool fixedQuantity, params CartHelper[] helpersToRemove)
        {
            return AddEntry(orderForm, entry, quantity, fixedQuantity, String.Empty, helpersToRemove);
        }

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <param name="orderForm"></param>
        /// <param name="entry">The entry.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="fixedQuantity">If true, lineitem's qty will be set to <paramref name="quantity"/> value. Otherwise, <paramref name="quantity"/> will be added to the current line item's qty value.</param>
        /// <param name="warehouseCode">The warehouse code</param>
        /// <param name="helpersToRemove">CartHelper(s) from which the item needs to be removed simultaneously with adding it to the current CartHelper.</param>        
        public static OrderForm AddEntry(this OrderForm orderForm, Mediachase.Commerce.Catalog.Objects.Entry entry, decimal quantity, bool fixedQuantity, string warehouseCode, params CartHelper[] helpersToRemove)
        {
            // Add line items
            LineItem lineItem = CreateLineItem(entry, quantity, warehouseCode);

            // check if items already exist
            bool found = false;
            foreach (LineItem item in orderForm.LineItems)
            {
                if (item.CatalogEntryId == lineItem.CatalogEntryId)
                {
                    item.Quantity = fixedQuantity ? lineItem.Quantity : item.Quantity + lineItem.Quantity;
                    item.ExtendedPrice = lineItem.ListPrice;
                    item.WarehouseCode = warehouseCode;
                    item.InventoryStatus = lineItem.InventoryStatus;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                orderForm.LineItems.Add(lineItem);
            }

            // remove entry from other helpers if needed
            if (helpersToRemove != null && helpersToRemove.Length > 0)
            {
                foreach (CartHelper ch in helpersToRemove)
                {
                    // if entry is in the helper, remove it
                    LineItem li = ch.LineItems.FirstOrDefault<LineItem>(item => item.CatalogEntryId == entry.ID);
                    if (li != null)
                    {
                        li.Delete();

                        // If helper is empty, remove it from the database
                        if (ch.IsEmpty)
                            ch.Delete();

                        // save changes
                        ch.Cart.AcceptChanges();
                    }
                }
            }

            return orderForm;
        }

        /// <summary>
        /// Gets the total number of items in the basket.
        /// </summary>
        /// <returns>the total number of items in the basket.</returns>
        public static decimal GetTotalItemCount(this IEnumerable<OrderForm> orderForms)
        {
            decimal totalItemCount = 0;
            foreach (OrderForm orderForm in orderForms)
            {
                foreach (LineItem lineItem in orderForm.LineItems)
                {
                    totalItemCount += lineItem.Quantity;
                }
            }

            return totalItemCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderForm"></param>
        /// <param name="entryId"></param>
        /// <returns></returns>
        public static OrderForm DeleteEntryById(this OrderForm orderForm, int entryId)
        {
            var item = orderForm.LineItems.FindItem(entryId);
            if (item != null)
            {
                orderForm.LineItems.Remove(item);
            }

            return orderForm;
        }

        /// <summary>
        /// Create the lineitem
        /// </summary>
        /// <param name="entry">The entry</param>
        /// <param name="quantity">The quantity</param>
        /// <param name="warehouseCode">The warehouse code</param>
        /// <returns></returns>
        private static LineItem CreateLineItem(Mediachase.Commerce.Catalog.Objects.Entry entry, decimal quantity, string warehouseCode)
        {
            var lineItem = new LineItem();

            // If entry has a parent, add parents name
            if (entry.ParentEntry != null)
            {
                lineItem.DisplayName = string.Format("{0}: {1}", entry.ParentEntry.Name, entry.Name);
                lineItem.ParentCatalogEntryId = entry.ParentEntry.ID;
            }
            else
            {
                lineItem.DisplayName = entry.Name;
                lineItem.ParentCatalogEntryId = string.Empty;
            }

            var currentMarket = ServiceLocator.Current.GetInstance<ICurrentMarket>().GetCurrentMarket();
            var listPrice = entry.GetSalePrice(quantity, currentMarket, SiteContext.Current.Currency);

            lineItem.CatalogEntryId = entry.ID;
            lineItem.MaxQuantity = entry.ItemAttributes.MaxQuantity;
            lineItem.MinQuantity = entry.ItemAttributes.MinQuantity;
            lineItem.Quantity = quantity;

            if (string.IsNullOrEmpty(warehouseCode))
            {
                lineItem.WarehouseCode = string.Empty; // was "entry.ItemAttributes.WarehouseCode;" but questionable
                lineItem.InventoryStatus = (int)entry.InventoryStatus;
            }
            else
            {
                lineItem.WarehouseCode = warehouseCode;
                lineItem.InventoryStatus = (int)GetInventoryStatus(entry, warehouseCode);
            }

            if (listPrice != null)
            {
                lineItem.ListPrice = listPrice.Money.Amount;
                lineItem.PlacedPrice = listPrice.Money.Amount;
                lineItem.ExtendedPrice = listPrice.Money.Amount;
            }
            else
            {
                lineItem.ListPrice = lineItem.PlacedPrice;
            }

            //Populate LineItem description field from the Entry object, if it exists in entry metaclass
            //Entry description metafield is also configurable so it could map another field to the description field . This fixes issue #718
            var mc = MetaClass.Load(CatalogContext.MetaDataContext, entry.MetaClassId);
            foreach (var mf in mc.MetaFields)
            {
                if (mf.Name.Equals("Description", StringComparison.InvariantCultureIgnoreCase) && entry.ItemAttributes["Description"] != null)
                {
                    lineItem.Description = entry.ItemAttributes["Description"].ToString();
                }
            }

            return lineItem;
        }

        private static InventoryTrackingStatus GetInventoryStatus(Mediachase.Commerce.Catalog.Objects.Entry entry, string warehouseCode)
        {
            var inventoryStatus = entry.InventoryStatus;
            var warehouse = ServiceLocator.Current.GetInstance<IWarehouseRepository>().Get(warehouseCode);
            if (warehouse != null)
            {
                var inventory = ServiceLocator.Current.GetInstance<IWarehouseInventoryService>().Get(new CatalogKey(entry), warehouse);
                if (inventory != null)
                {
                    inventoryStatus = inventory.InventoryStatus;
                }
            }

            return inventoryStatus;
        }

        /// <summary>
        /// Helper method to return cart to which the orderform belongs
        /// </summary>
        /// <param name="orderForm">Order form</param>
        /// <returns>Parent cart</returns>
        public static Mediachase.Commerce.Orders.Cart GetParentCart(this OrderForm orderForm)
        {
            return orderForm.Parent as Mediachase.Commerce.Orders.Cart;
        }
    }
}