using System;
using System.Web;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.ServiceLocation;
using Geta.EPi.Commerce.Extensions.Entry;
using Geta.EPi.Commerce.Extensions.Order;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Orders;

namespace Geta.EPi.Commerce.Extensions.Cart
{
    public static class LineItemExtensions
    {
#pragma warning disable 649
        private static Injected<ReferenceConverter> _referenceConverter;
        private static Injected<IContentLoader> _contentLoader;
        private static Injected<ThumbnailUrlResolver> _thumbnailUrlResolver;
#pragma warning restore 649

        /// <summary>
        /// Gets url of the specified line item's asset. If the group name of asset is emtpy, the url of the first asset is returned.
        /// </summary>
        /// <param name="lineItem"></param>
        /// <param name="assetGroupName">Asset group name, can be empty</param>
        /// <returns>Content url of asset if such is found</returns>
        public static string GetAssetUrl(this LineItem lineItem, string assetGroupName = "")
        {
            var entry = CatalogContext.Current.GetCatalogEntry(lineItem.Code, 
                            new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.Assets));

            return entry.GetAssetUrl();
        }

        /// <summary>
        /// Calculates placed price with quantity
        /// </summary>
        /// <param name="lineItem">Line item for which calculate the price</param>
        /// <returns>Placed price for the quantity of line item</returns>
        public static decimal CalculatePlacedPrice(this LineItem lineItem)
        {
            return lineItem.PlacedPrice*lineItem.Quantity;
        }

        public static string GetUrl(this LineItem lineItem)
        {
            var variantLink = _referenceConverter.Service.GetContentLink(lineItem.Code);
            var variant = _contentLoader.Service.Get<VariationContent>(variantLink);
            return variant.GetUrl();
        }

        public static string GetFullUrl(this LineItem lineItem)
        {
            var rightUrl = lineItem.GetUrl();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            return new Uri(new Uri(baseUrl), rightUrl).ToString();
        }

        public static string GetThumbnailUrl(this LineItem lineItem)
        {
            return GetThumbnailUrl(lineItem.Code);
        }

        private static string GetThumbnailUrl(string code)
        {
            var content = _contentLoader.Service.Get<VariationContent>(_referenceConverter.Service.GetContentLink(code, CatalogContentType.CatalogEntry));

            return _thumbnailUrlResolver.Service.GetThumbnailUrl(content, "thumbnail");
        }

        public static Money ToMoney(this LineItem lineItem, decimal amount)
        {
            return lineItem.Parent.Parent.ToMoney(amount);
        }
    }
}