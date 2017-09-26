using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

namespace Geta.EPi.Commerce.Extensions
{
    public static class AssetContainerExtensions
    {
#pragma warning disable 649
        private static Injected<AssetUrlResolver> _assetUrlResolver;
        private static Injected<UrlResolver> _urlResolver;
        private static Injected<IContentLoader> _contentLoader;
#pragma warning restore 649

        public static string GetDefaultAsset<TContentMedia>(this IAssetContainer assetContainer) where TContentMedia : IContentMedia
        {
            var url = _assetUrlResolver.Service.GetAssetUrl<TContentMedia>(assetContainer);
            return Uri.TryCreate(url, UriKind.Absolute, out var uri)
                ? uri.PathAndQuery
                : url;
        }

        public static IList<string> GetAssets<TContentMedia>(this IAssetContainer assetContainer) where TContentMedia : IContentMedia
        {
            var assets = new List<string>();
            if (assetContainer.CommerceMediaCollection != null)
            {
                assets.AddRange(
                    assetContainer
                        .CommerceMediaCollection
                        .Where(x => ValidateCorrectType<TContentMedia>(x.AssetLink))
                        .Select(media => _urlResolver.Service.GetUrl(media.AssetLink)));
            }

            if (!assets.Any())
            {
                assets.Add(string.Empty);
            }

            return assets;
        }

        private static bool ValidateCorrectType<TContentMedia>(ContentReference contentLink) where TContentMedia : IContentMedia
        {
            if (typeof(TContentMedia) == typeof(IContentMedia))
            {
                return true;
            }

            return !ContentReference.IsNullOrEmpty(contentLink)
                && _contentLoader.Service.TryGet(contentLink, out TContentMedia _);
        }
    }
}