using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Pricing;

namespace Geta.EPi.Commerce.Extensions
{
    public static class PriceDetailServiceExtensions
    {
        private static Injected<IContentRepository> ContentRepository { get; set; }

        /// <summary>
        /// Saves the MSRP price
        /// </summary>
        /// <param name="priceDetailService"></param>
        /// <param name="contentLink"></param>
        /// <param name="marketId"></param>
        /// <param name="currency"></param>
        /// <param name="amount"></param>
        public static void SaveMsrp(this IPriceDetailService priceDetailService, ContentReference contentLink, MarketId marketId, Currency currency, decimal amount)
        {
            int totalCount;
            var priceFilter = new PriceFilter {Currencies = new List<Currency>() {currency}};
            var msrp = (PriceDetailValue)priceDetailService.List(contentLink, marketId, priceFilter, 0, int.MaxValue, out totalCount).FirstOrDefault(p => p.UnitPrice.Currency == currency && p.CustomerPricing.PriceTypeId == CustomerPricing.PriceType.PriceGroup && p.CustomerPricing.PriceCode == "MSRP");

            if (msrp != null)
            {
                msrp.UnitPrice = new Money(amount, currency);
            }
            else
            {
                msrp = new PriceDetailValue();
                var entryContent = ContentRepository.Service.Get<EntryContentBase>(contentLink);
                msrp.CatalogKey = new CatalogKey(new Guid(entryContent.ApplicationId), entryContent.Code);
                msrp.CustomerPricing = new CustomerPricing(CustomerPricing.PriceType.PriceGroup, "MSRP");
                msrp.UnitPrice = new Money(amount, currency);
                msrp.MarketId = marketId;
                msrp.MinQuantity = 0;
                msrp.ValidFrom = entryContent.StartPublish.Value;
            }

            priceDetailService.Save(msrp);
        }
    }
}