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
#pragma warning disable 649
        private static Injected<IContentRepository> _contentRepository;
#pragma warning restore 649

        /// <summary>
        /// Saves the MSRP price
        /// </summary>
        /// <param name="priceDetailService"></param>
        /// <param name="contentLink"></param>
        /// <param name="marketId"></param>
        /// <param name="currency"></param>
        /// <param name="amount"></param>
        public static void SaveMsrp(
            this IPriceDetailService priceDetailService, ContentReference contentLink, MarketId marketId, Currency currency, decimal amount)
        {
            var priceFilter = new PriceFilter {Currencies = new List<Currency> {currency}};
            var msrp = (PriceDetailValue)priceDetailService
                .List(contentLink, marketId, priceFilter, 0, int.MaxValue, out _)
                .FirstOrDefault(
                    p => p.UnitPrice.Currency == currency
                        && p.CustomerPricing.PriceTypeId == CustomerPricing.PriceType.PriceGroup
                        && p.CustomerPricing.PriceCode == "MSRP");

            if (msrp != null)
            {
                msrp.UnitPrice = new Money(amount, currency);
            }
            else
            {
                msrp = new PriceDetailValue();
                var entryContent = _contentRepository.Service.Get<EntryContentBase>(contentLink);
                msrp.CatalogKey = new CatalogKey(entryContent.Code);
                msrp.CustomerPricing = new CustomerPricing(CustomerPricing.PriceType.PriceGroup, "MSRP");
                msrp.UnitPrice = new Money(amount, currency);
                msrp.MarketId = marketId;
                msrp.MinQuantity = 0;
                msrp.ValidFrom = entryContent.StartPublish ?? DateTime.MinValue;
            }

            priceDetailService.Save(msrp);
        }
    }
}