using System;
using System.Web;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Mediachase.Commerce.Catalog;

namespace Geta.EPi.Commerce.Extensions
{
    public static class LineItemExtensions
    {
#pragma warning disable 649
        private static Injected<UrlResolver> _urlResolver;
        private static Injected<ReferenceConverter> _referenceConverter;
        private static Injected<IContentLoader> _contentLoader;
        private static Injected<ThumbnailUrlResolver> _thumbnailUrlResolver;
#pragma warning restore 649

        public static string GetUrl(this ILineItem lineItem)
        {
            var link = _referenceConverter.Service.GetContentLink(lineItem.Code);
            if (link == null || link == ContentReference.EmptyReference)
            {
                return string.Empty;
            }

            var entry = _contentLoader.Service.Get<EntryContentBase>(link);
            var variant = entry as VariationContent;

            return variant?.GetUrl() ?? _urlResolver.Service.GetUrl(entry.ContentLink) ?? string.Empty;
        }

        public static string GetFullUrl(this ILineItem lineItem)
        {
            var rightUrl = lineItem.GetUrl();
            if (string.IsNullOrEmpty(rightUrl)) return string.Empty;
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            return new Uri(new Uri(baseUrl), rightUrl).ToString();
        }

        public static string GetThumbnailUrl(this ILineItem lineItem)
        {
            return GetThumbnailUrl(lineItem.Code);
        }

        private static string GetThumbnailUrl(string code)
        {
            var link = _referenceConverter.Service.GetContentLink(code, CatalogContentType.CatalogEntry); 
            if (link == null || link == ContentReference.EmptyReference)
            {
                return string.Empty;
            }
            var content = _contentLoader.Service.Get<EntryContentBase>(link);
            if (content == null) return string.Empty;
            return _thumbnailUrlResolver.Service.GetThumbnailUrl(content, "thumbnail");
        }
    }
}