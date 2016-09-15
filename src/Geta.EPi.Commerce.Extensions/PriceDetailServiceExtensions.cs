using System;
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
            var priceService = ServiceLocator.Current.GetInstance<IPriceService>();
            var msrp = (PriceDetailValue)priceService.LoadMsrp(contentLink, marketId, currency);
    
            if (msrp != null)
            {
                msrp.UnitPrice = new Money(amount, currency);
            }
            else
            {
                msrp = new PriceDetailValue();
                var contentRepository = ServiceLocator.Current.GetInstance <IContentRepository>();
                var entryContent = contentRepository.Get<EntryContentBase>(contentLink);
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