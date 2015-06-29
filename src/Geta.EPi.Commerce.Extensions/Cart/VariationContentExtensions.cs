using System.Linq;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

namespace Geta.EPi.Commerce.Extensions.Cart
{
    public static class VariationContentExtensions
    {
#pragma warning disable 649
        private static Injected<UrlResolver> _urlResolver;
        private static Injected<ILinksRepository> _linksRepository;
#pragma warning restore 649

        public static string GetUrl(this VariationContent variant)
        {
            var productLink = variant.GetParentProducts(_linksRepository.Service).FirstOrDefault();
            if (productLink == null)
            {
                return string.Empty;
            }
            var urlBuilder = new UrlBuilder(_urlResolver.Service.GetUrl(productLink));

            if (variant.Code != null)
            {
                urlBuilder.QueryCollection.Add("variationId", variant.Code);
            }
            return urlBuilder.ToString();
        }
    }
}