using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

namespace Geta.EPi.Commerce.Extensions
{
    public static class AssetContainerExtensions
    {
#pragma warning disable 649
        private static Injected<UrlResolver> _urlResolver;
        private static Injected<IContentLoader> _contentLoader;
#pragma warning restore 649

        private const string EpiserversDefaultGroupName = "default";

        /// <summary>
        /// Get the first asset of Episerver's default group.
        /// </summary>
        /// <typeparam name="TContentMedia">Type of media.</typeparam>
        /// <param name="assetContainer">Asset container.</param>
        /// <returns>Url of the found asset or empty string if asset not found.</returns>
        public static string GetDefaultAsset<TContentMedia>(this IAssetContainer assetContainer)
            where TContentMedia : IContentMedia
        {
            return assetContainer.GetDefaultAssets<TContentMedia>().FirstOrDefault() ?? string.Empty;
        }

        /// <summary>
        /// Get assets of Episerver's default group.
        /// </summary>
        /// <typeparam name="TContentMedia">Type of media.</typeparam>
        /// <param name="assetContainer">Asset container.</param>
        /// <returns>The list of found assets URLs.</returns>
        public static IList<string> GetDefaultAssets<TContentMedia>(this IAssetContainer assetContainer)
            where TContentMedia : IContentMedia
        {
            return assetContainer.GetAssets<TContentMedia>(new[] { EpiserversDefaultGroupName });
        }

        /// <summary>
        /// Get the first asset of the group.
        /// </summary>
        /// <typeparam name="TContentMedia">Type of media.</typeparam>
        /// <param name="assetContainer">Asset container.</param>
        /// <param name="groupName">Asset group name.</param>
        /// <returns>Url of the found asset or empty string if asset not found.</returns>
        public static string GetAsset<TContentMedia>(this IAssetContainer assetContainer, string groupName)
            where TContentMedia : IContentMedia
        {
            return assetContainer.GetAssets<TContentMedia>(groupName).FirstOrDefault() ?? string.Empty;
        }

        /// <summary>
        /// Get assets of the group.
        /// </summary>
        /// <typeparam name="TContentMedia">Type of media.</typeparam>
        /// <param name="assetContainer">Asset container.</param>
        /// <param name="groupName">Asset group name.</param>
        /// <returns>The list of found assets URLs.</returns>
        public static IList<string> GetAssets<TContentMedia>(this IAssetContainer assetContainer, string groupName)
            where TContentMedia : IContentMedia
        {
            if (groupName == null) throw new ArgumentNullException(nameof(groupName));
            return assetContainer.GetAssets<TContentMedia>(new[] { groupName });
        }

        /// <summary>
        /// Get assets of groups.
        /// </summary>
        /// <typeparam name="TContentMedia">Type of media.</typeparam>
        /// <param name="assetContainer">Asset container.</param>
        /// <param name="groupNames">Asset group names. If no groups provided, will return all assets.</param>
        /// <returns>The list of found assets URLs.</returns>
        public static IList<string> GetAssets<TContentMedia>(this IAssetContainer assetContainer, string[] groupNames)
            where TContentMedia : IContentMedia
        {
            if (groupNames == null) throw new ArgumentNullException(nameof(groupNames));
            if (assetContainer.CommerceMediaCollection == null) return new List<string>();

            IEnumerable<CommerceMedia> media = assetContainer.CommerceMediaCollection;

            if (groupNames.Length > 0)
            {
                media = media.Where(
                    x => groupNames.Any(
                        groupName => x.GroupName.Equals(groupName, StringComparison.InvariantCultureIgnoreCase)));
            }

            return media
                    .Where(x => ValidateCorrectType<TContentMedia>(x.AssetLink))
                    .Select(m => _urlResolver.Service.GetUrl(m.AssetLink))
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();
        }

        /// <summary>
        /// Get all assets.
        /// </summary>
        /// <typeparam name="TContentMedia">Type of media.</typeparam>
        /// <param name="assetContainer">Asset container.</param>
        /// <returns>The list of found assets URLs.</returns>
        public static IList<string> GetAssets<TContentMedia>(this IAssetContainer assetContainer)
            where TContentMedia : IContentMedia
        {
            return assetContainer.GetAssets<TContentMedia>(new string[0]);
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