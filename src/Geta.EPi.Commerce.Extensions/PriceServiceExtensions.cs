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
    public static class PriceServiceExtensions
    {
#pragma warning disable 649
        private static Injected<IContentRepository> _contentRepository;
#pragma warning restore 649

        /// <summary>
        /// Get's the MSRP price
        /// </summary>
        /// <param name="priceService"></param>
        /// <param name="contentLink"></param>
        /// <param name="marketId"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        public static IPriceValue LoadMsrp(this IPriceService priceService, ContentReference contentLink, MarketId marketId, Currency currency)
        {
            var contentRepository = ServiceLocator.Current.GetInstance <IContentRepository>();
            var entryContent = contentRepository.Get<EntryContentBase>(contentLink);
            var catalogKey = new CatalogKey(entryContent.Code);
            var priceFilter = new PriceFilter {Currencies = new List<Currency> {currency}};

            return priceService
                .GetPrices(marketId, DateTime.UtcNow, catalogKey, priceFilter)
                .FirstOrDefault(
                    p => p.CustomerPricing.PriceTypeId == CustomerPricing.PriceType.PriceGroup
                        && p.CustomerPricing.PriceCode == "MSRP");
        }

        /// <summary>
        /// Get the previous price. Example of usage: Was 1000 now 800.
        /// </summary>
        /// <param name="priceService"></param>
        /// <param name="contentLink"></param>
        /// <param name="marketId"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        public static IPriceValue GetPreviousPrice(this IPriceService priceService, ContentReference contentLink, MarketId marketId, Currency currency)
        {
            var entryContent = _contentRepository.Service.Get<EntryContentBase>(contentLink);
            var catalogKey = new CatalogKey(entryContent.Code);
            var prices = priceService.GetCatalogEntryPrices(catalogKey).ToList();
            var previousPrice = prices
                .FirstOrDefault(
                    p => p.MarketId == marketId
                        && p.UnitPrice.Currency == currency
                        && p.ValidUntil.HasValue
                        && p.ValidUntil.Value < DateTime.UtcNow);
            var currentPrice = prices
                .FirstOrDefault(
                    p => p.MarketId == marketId
                        && p.UnitPrice.Currency == currency
                        && p.ValidFrom < DateTime.UtcNow
                        && (!p.ValidUntil.HasValue || p.ValidUntil.Value > DateTime.UtcNow));

            return previousPrice?.UnitPrice.Amount < currentPrice?.UnitPrice.Amount
                ? previousPrice
                : currentPrice;
        }
    }
}