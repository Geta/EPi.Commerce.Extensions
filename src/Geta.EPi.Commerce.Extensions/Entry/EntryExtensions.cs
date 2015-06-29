using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Inventory;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Marketing.Objects;
using Mediachase.Commerce.Pricing;
using Mediachase.Commerce.Security;
using Mediachase.Commerce.Website.Helpers;

namespace Geta.EPi.Commerce.Extensions.Entry
{
    public static class EntryExtensions
    {
        /// <summary>
        /// Gets the sale price.
        /// </summary>
        /// <param name="entry">The entry used to fetch prices.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="market">The market.</param>
        /// <param name="currency">The currency.</param>
        /// <returns></returns>
        public static Price GetSalePrice(this Mediachase.Commerce.Catalog.Objects.Entry entry, decimal quantity, IMarket market, Currency currency)
        {
            var customerPricing = new List<CustomerPricing>();
            customerPricing.Add(CustomerPricing.AllCustomers);

            MembershipUser currentUser = SecurityContext.Current.CurrentUser;
            if (currentUser != null)
            {
                if (!string.IsNullOrEmpty(currentUser.UserName))
                {
                    customerPricing.Add(new CustomerPricing(CustomerPricing.PriceType.UserName, currentUser.UserName));
                }

                CustomerContact currentUserContact = CustomerContext.Current.GetContactForUser(currentUser);
                if (currentUserContact != null && !string.IsNullOrEmpty(currentUserContact.EffectiveCustomerGroup))
                {
                    customerPricing.Add(new CustomerPricing(CustomerPricing.PriceType.PriceGroup, currentUserContact.EffectiveCustomerGroup));
                }
            }

            IPriceService priceService = ServiceLocator.Current.GetInstance<IPriceService>();
            PriceFilter filter = new PriceFilter
            {
                Quantity = quantity,
                Currencies = new Currency[] { currency },
                CustomerPricing = customerPricing
            };

            // return less price value
            IPriceValue priceValue = priceService.GetPrices(market.MarketId, FrameworkContext.Current.CurrentDateTime, new CatalogKey(entry), filter)
                .OrderBy(pv => pv.UnitPrice)
                .FirstOrDefault();

            if (priceValue != null)
            {
                return new Price(priceValue.UnitPrice);
            }

            return null;
        }

        /// <summary>
        /// Gets the sale price. The current culture info currency code will be used.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="market">Market for filtering.</param>
        /// <returns>Price</returns>
        public static Price GetSalePrice(this Mediachase.Commerce.Catalog.Objects.Entry entry, decimal quantity, IMarket market)
        {
            return GetSalePrice(entry, quantity, market, market.DefaultCurrency);
        }

        /// <summary>
        /// Gets the sale price. The current culture info currency code will be used.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns>Price</returns>
        /// 
        public static Price GetSalePrice(this Mediachase.Commerce.Catalog.Objects.Entry entry, decimal quantity)
        {
            var currentMarketService = ServiceLocator.Current.GetInstance<ICurrentMarket>();
            return GetSalePrice(entry, quantity, currentMarketService.GetCurrentMarket());
        }

        /// <summary>
        /// Gets the discount price.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        public static Price GetDiscountPrice(this Mediachase.Commerce.Catalog.Objects.Entry entry)
        {
            return GetDiscountPrice(entry, string.Empty, string.Empty);
        }

        /// <summary>
        /// Gets the discount price.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <returns></returns>
        public static Price GetDiscountPrice(this Mediachase.Commerce.Catalog.Objects.Entry entry, string catalogName)
        {
            return GetDiscountPrice(entry, catalogName, string.Empty);
        }

        /// <summary>
        /// Gets the discounted price of the catalog entry.
        /// </summary>
        /// <param name="entry">The catalog entry.</param>
        /// <param name="catalogName">Name of the catalog to filter the discounts for. If null, all catalogs containing the entry will be used.</param>
        /// <param name="catalogNodeCode">The catalog node code to filter the discounts for. If null, all catalog nodes containing the entry will be used.</param>
        /// <param name="market">The market to fetch tiered base pricing for.</param>
        /// <returns>The discounted price of the catalog entry.</returns>
        /// <remarks>Uses market.DefaultCurrency for the currency.</remarks>
        public static Price GetDiscountPrice(this Mediachase.Commerce.Catalog.Objects.Entry entry, string catalogName, string catalogNodeCode, IMarket market)
        {
            return GetDiscountPrice(entry, catalogName, catalogNodeCode, market, market.DefaultCurrency);
        }

        /// <summary>
        /// Gets the discounted price of the catalog entry.
        /// </summary>
        /// <param name="entry">The catalog entry.</param>
        /// <param name="catalogName">Name of the catalog to filter the discounts for. If null, all catalogs containing the entry will be used.</param>
        /// <param name="catalogNodeCode">The catalog node code to filter the discounts for. If null, all catalog nodes containing the entry will be used.</param>
        /// <param name="market">The market to fetch tiered base pricing for.</param>
        /// <param name="currency">The currency to fetch prices in.</param>
        /// <returns>The discounted price of the catalog entry.</returns>
        public static Price GetDiscountPrice(this Mediachase.Commerce.Catalog.Objects.Entry entry, string catalogName, string catalogNodeCode, IMarket market, Currency currency)
        {
            if (entry == null)
            {
                throw new NullReferenceException("entry can't be null");
            }

            decimal minQuantity = 1;

            // get min quantity attribute
            if (entry.ItemAttributes != null)
            {
                minQuantity = entry.ItemAttributes.MinQuantity;
            }

            // we can't pass qauntity of 0, so make it default to 1
            if (minQuantity <= 0)
            {
                minQuantity = 1;
            }

            // Get sale price for the current user
            Price price = StoreHelper.GetSalePrice(entry, minQuantity, market, currency);
            if (price == null)
            {
                return null;
            }

            string catalogNodes = string.Empty;
            string catalogs = string.Empty;
            // Now cycle through all the catalog nodes where this entry is present filtering by specified catalog and node code
            // The nodes are only populated when Full or Nodes response group is specified.
            if (entry.Nodes != null && entry.Nodes.CatalogNode != null && entry.Nodes.CatalogNode.Length > 0)
            {
                foreach (CatalogNode node in entry.Nodes.CatalogNode)
                {
                    string entryCatalogName = CatalogContext.Current.GetCatalogDto(node.CatalogId).Catalog[0].Name;

                    // Skip filtered catalogs
                    if (!string.IsNullOrEmpty(catalogName) && !entryCatalogName.Equals(catalogName))
                    {
                        continue;
                    }

                    // Skip filtered catalogs nodes
                    if (!string.IsNullOrEmpty(catalogNodeCode) && !node.ID.Equals(catalogNodeCode, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(catalogs))
                    {
                        catalogs = entryCatalogName;
                    }
                    else
                    {
                        catalogs += ";" + entryCatalogName;
                    }

                    if (string.IsNullOrEmpty(catalogNodes))
                    {
                        catalogNodes = node.ID;
                    }
                    else
                    {
                        catalogNodes += ";" + node.ID;
                    }
                }
            }

            if (string.IsNullOrEmpty(catalogs))
            {
                catalogs = catalogName;
            }

            if (string.IsNullOrEmpty(catalogNodes))
            {
                catalogNodes = catalogNodeCode;
            }

            var filter = new PromotionFilter
            {
                IgnoreConditions = false,
                IgnorePolicy = false,
                IgnoreSegments = false,
                IncludeCoupons = false
            };

            // Create new entry
            // TPB: catalogNodes is determined by the front end. GetParentNodes(entry)
            var result = new PromotionEntry(catalogs, catalogNodes, entry.ID, price.Money.Amount);
            var promotionEntryPopulateService = (IPromotionEntryPopulate)MarketingContext.Current.PromotionEntryPopulateFunctionClassInfo.CreateInstance();
            promotionEntryPopulateService.Populate(result, entry, market.MarketId, currency);

            var sourceSet = new PromotionEntriesSet();
            sourceSet.Entries.Add(result);

            return GetDiscountPrice(filter, price, sourceSet, sourceSet);
        }

        /// <summary>
        /// Gets the discount price by evaluating the discount rules and taking into account segments customer belongs to.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="catalogName">Name of the catalog to filter the discounts for. If null, all catalogs entry belongs to will be applied.</param>
        /// <param name="catalogNodeCode">The catalog node code to filter the discounts for. If null, all catalog nodes entry belongs to will be applied.</param>
        /// <returns></returns>
        public static Price GetDiscountPrice(this Mediachase.Commerce.Catalog.Objects.Entry entry, string catalogName, string catalogNodeCode)
        {
            var currentMarketService = ServiceLocator.Current.GetInstance<ICurrentMarket>();

            return GetDiscountPrice(entry, catalogName, catalogNodeCode, currentMarketService.GetCurrentMarket());
        }

        /// <summary>
        /// Gets the discount price.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="salePrice">The sale price.</param>
        /// <param name="sourceSet">The source set.</param>
        /// <param name="targetSet">The target set.</param>
        /// <returns></returns>
        private static Price GetDiscountPrice(PromotionFilter filter, Price salePrice, PromotionEntriesSet sourceSet, PromotionEntriesSet targetSet)
        {
            // Create new promotion helper, which will initialize PromotionContext object for us and setup context dictionary
            var helper = new PromotionHelper();

            // Only target entries
            helper.PromotionContext.TargetGroup = PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Entry).Key;

            // Configure promotion context
            helper.PromotionContext.SourceEntriesSet = sourceSet;
            helper.PromotionContext.TargetEntriesSet = targetSet;

            // Execute the promotions and filter out basic collection of promotions, we need to execute with cache disabled, so we get latest info from the database
            helper.Eval(filter);

            // Check the count, and get new price
            if (helper.PromotionContext.PromotionResult.PromotionRecords.Count > 0)
            {
                return ObjectHelper.CreatePrice(new Money(salePrice.Money.Amount - GetDiscountPrice(helper.PromotionContext.PromotionResult), salePrice.Money.Currency));
            }
            
            return salePrice;
        }

        /// <summary>
        /// Gets the discount price.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        private static decimal GetDiscountPrice(PromotionResult result)
        {
            decimal discountAmount = 0;
            foreach (PromotionItemRecord record in result.PromotionRecords)
            {
                discountAmount += GetDiscountAmount(record, record.PromotionReward);
            }

            return discountAmount;
        }

        /// <summary>
        /// Gets the discount amount for one entry only.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="reward">The reward.</param>
        /// <returns></returns>
        private static decimal GetDiscountAmount(PromotionItemRecord record, PromotionReward reward)
        {
            decimal discountAmount = 0;
            if (reward.RewardType == PromotionRewardType.EachAffectedEntry || reward.RewardType == PromotionRewardType.AllAffectedEntries)
            {
                if (reward.AmountType == PromotionRewardAmountType.Percentage)
                {
                    discountAmount = record.AffectedEntriesSet.TotalCost * reward.AmountOff / 100;
                }
                else // need to split discount between all items
                {
                    discountAmount += reward.AmountOff; // since we assume only one entry in affected items
                }
            }

            return Math.Round(discountAmount, 2);
        }

        /// <summary>
        /// Determines whether entry is in stock.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>
        /// 	<c>true</c> if entry is in stock otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInStock(this Mediachase.Commerce.Catalog.Objects.Entry entry)
        {
            if (entry == null)
            {
                return false;
            }

            if (entry.WarehouseInventories == null)
            {
                return false;
            }

            IWarehouseInventory sumInventory = SumInventories(entry.WarehouseInventories.WarehouseInventory);

            // If we don't account inventory return true always
            if (sumInventory.InventoryStatus != InventoryTrackingStatus.Enabled)
            {
                return true;
            }

            return entry.GetItemsInStock() > 0;
        }

        /// <summary>
        /// Gets the items in stock.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        public static decimal GetItemsInStock(this Mediachase.Commerce.Catalog.Objects.Entry entry)
        {
            if (entry == null)
            {
                return 0;
            }

            if (entry.WarehouseInventories == null)
            {
                return 0;
            }

            IWarehouseInventory sumInventory = SumInventories(entry.WarehouseInventories.WarehouseInventory);
            return sumInventory.InStockQuantity - sumInventory.ReservedQuantity;
        }

        /// <summary>
        /// Determines whether [is available for backorder] [the specified entry].
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>
        /// 	<c>true</c> if [is available for backorder] [the specified entry]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAvailableForBackorder(this Mediachase.Commerce.Catalog.Objects.Entry entry)
        {
            if (entry == null)
            {
                return false;
            }

            if (entry.WarehouseInventories == null)
            {
                return false;
            }

            IWarehouseInventory sumInventory = SumInventories(entry.WarehouseInventories.WarehouseInventory);

            if (sumInventory.AllowBackorder && sumInventory.BackorderQuantity > 0 && sumInventory.BackorderAvailabilityDate <= DateTime.UtcNow)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether [is available for preorder] [the specified entry].
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>
        /// 	<c>true</c> if [is available for preorder] [the specified entry]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAvailableForPreorder(this Mediachase.Commerce.Catalog.Objects.Entry entry)
        {
            if (entry == null)
            {
                return false;
            }

            if (entry.WarehouseInventories == null)
            {
                return false;
            }

            IWarehouseInventory sumInventory = SumInventories(entry.WarehouseInventories.WarehouseInventory);

            if (sumInventory.AllowPreorder && sumInventory.PreorderQuantity > 0 && sumInventory.PreorderAvailabilityDate <= DateTime.UtcNow)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets all item in stock.
        /// </summary>
        /// <param name="inventories"> The WarehouseInventory.</param>
        /// <returns></returns>
        public static IWarehouseInventory SumInventories(IEnumerable<IWarehouseInventory> inventories)
        {
            var result = new WarehouseInventory
            {
                InStockQuantity = 0,
                ReservedQuantity = 0,
                ReorderMinQuantity = 0,
                PreorderQuantity = 0,
                BackorderQuantity = 0,
                AllowBackorder = false,
                AllowPreorder = false,
                PreorderAvailabilityDate = DateTime.MaxValue,
                BackorderAvailabilityDate = DateTime.MaxValue
            };

            var warehouseRepository = ServiceLocator.Current.GetInstance<IWarehouseRepository>();

            foreach (IWarehouseInventory inventory in inventories)
            {
                if (warehouseRepository.Get(inventory.WarehouseCode).IsActive)
                {
                    // Sum up quantity fields
                    result.BackorderQuantity += inventory.BackorderQuantity;
                    result.InStockQuantity += inventory.InStockQuantity;
                    result.PreorderQuantity += inventory.PreorderQuantity;
                    result.ReorderMinQuantity += inventory.ReorderMinQuantity;
                    result.ReservedQuantity += inventory.ReservedQuantity;

                    // Check flags that should be global when aggregating warehouse inventories
                    result.AllowBackorder = inventory.AllowBackorder ? inventory.AllowBackorder : result.AllowBackorder;
                    result.AllowPreorder = inventory.AllowPreorder ? inventory.AllowPreorder : result.AllowPreorder;

                    result.BackorderAvailabilityDate = GetAvailabilityDate(result.BackorderAvailabilityDate, inventory.BackorderAvailabilityDate);
                    result.PreorderAvailabilityDate = GetAvailabilityDate(result.PreorderAvailabilityDate, inventory.PreorderAvailabilityDate);
                }
            }

            return result;
        }

        private static DateTime? GetAvailabilityDate(DateTime? resultDate, DateTime? originalDate)
        {
            if (resultDate.HasValue && originalDate.HasValue)
            {
                return DateTime.Compare(resultDate.Value, originalDate.Value) < 0 ? resultDate : originalDate;
            }

            return resultDate;
        }

        /// <summary>
        /// Gets the discount price by current market.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        public static Price GetBasePrice(this Mediachase.Commerce.Catalog.Objects.Entry entry)
        {
            return GetBasePrice(entry, 1);
        }
        /// <summary>
        /// Gets the current price by current market with min quantity.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="minQuantity">The min quantity.</param>
        /// <returns></returns>
        public static Price GetBasePrice(this Mediachase.Commerce.Catalog.Objects.Entry entry, int minQuantity)
        {
            var currentMarketService = ServiceLocator.Current.GetInstance<ICurrentMarket>();

            MarketId currentMarketId = currentMarketService.GetCurrentMarket().MarketId;

            var priceValue = entry.PriceValues.PriceValue.Where(p => p.MinQuantity <= minQuantity && p.MarketId.Equals(currentMarketId) && IsActivePrice(p))
                .OrderByDescending(p => p.MinQuantity).FirstOrDefault();
            return priceValue == null ? null : new Price(priceValue.UnitPrice);
        }

        /// <summary>
        /// Determines whether price is active is not.
        /// </summary>
        /// <param name="price">The price.</param>
        /// <returns>
        /// 	<c>true</c> if price is active otherwise, <c>false</c>.
        /// </returns>
        private static bool IsActivePrice(PriceValue price)
        {
            return price.ValidFrom < DateTime.Now && price.ValidUntil > DateTime.Now;
        }


        /// <summary>
        /// Returns url of the first or specified group asset item
        /// </summary>
        /// <param name="entry">Catalog entry</param>
        /// <param name="assetGroupName">Group name of the asset, can be empty</param>
        /// <returns>Url of the asset, if any found</returns>
        public static string GetAssetUrl(this Mediachase.Commerce.Catalog.Objects.Entry entry, string assetGroupName = "")
        {
            if (entry == null || entry.Assets == null || !entry.Assets.Any()) return string.Empty;

            ItemAsset asset;

            if (!string.IsNullOrEmpty(assetGroupName) && entry.Assets.Any(x => x.GroupName == assetGroupName))
            {
                asset = entry.Assets.First(x => x.GroupName == assetGroupName);
            }
            else
            {
                asset = entry.Assets.First();
            }

            var contentReference = PermanentLinkUtility.FindContentReference(new Guid(asset.AssetKey));
            var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
            return urlResolver.GetUrl(contentReference);
        }
    }
}