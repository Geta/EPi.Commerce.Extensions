using System.Collections.Generic;
using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace Geta.EPi.Commerce.Extensions
{
    public static class BundleContentExtensions
    {
        #pragma warning disable 649
        private static Injected<ILinksRepository> _linksRepository;
        #pragma warning restore 649

        /// <summary>
        /// Gets all bundle entries for a bundle
        /// </summary>
        /// <param name="bundleContent">The bundle content to use</param>
        /// <param name="linksRepository">The link repository</param>
        /// <returns>Collection of bundle entry references</returns>
        public static IEnumerable<ContentReference> GetBundleEntries(this BundleContent bundleContent, ILinksRepository linksRepository)
        {
            return linksRepository.GetRelationsBySource<BundleEntry>(bundleContent.ContentLink).Select(r => r.Source);
        }

        /// <summary>
        /// Gets all bundle entries for a bundle
        /// </summary>
        /// <param name="bundleContent">The bundle content to use</param>
        /// <returns>Collection of bundle entry references</returns>
        public static IEnumerable<ContentReference> GetBundleEntries(this BundleContent bundleContent)
        {
            return _linksRepository.Service.GetRelationsBySource<BundleEntry>(bundleContent.ContentLink).Select(r => r.Source);
        }
    }
}