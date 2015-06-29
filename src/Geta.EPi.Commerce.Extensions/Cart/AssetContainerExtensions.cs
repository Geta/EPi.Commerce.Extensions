using System.Collections.Generic;
using System.Linq;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

namespace Geta.EPi.Commerce.Extensions.Cart
{
    public static class AssetContainerExtensions
    {
#pragma warning disable 649
        private static Injected<AssetUrlResolver> _assetUrlResolver;
        private static Injected<UrlResolver> _urlResolver;
#pragma warning restore 649

        public static string GetDefaultAsset(this IAssetContainer assetContainer)
        {
            return _assetUrlResolver.Service.GetAssetUrl(assetContainer);
        }

        public static IEnumerable<string> GetAssets(this IAssetContainer assetContainer)
        {
            var assets = new List<string>();
            if (assetContainer.CommerceMediaCollection != null)
            {
                assets.AddRange(assetContainer.CommerceMediaCollection.Select(media => _urlResolver.Service.GetUrl(media.AssetLink)));
            }
            if (!assets.Any())
            {
                assets.Add(string.Empty);
            }
            return assets;
        }
    }
}