using System.Collections.Generic;
using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace Geta.EPi.Commerce.Extensions
{
    public static class EntryContentBaseExtensions
    {
#pragma warning disable 649
        private static Injected<ILinksRepository> _linksRepository;
#pragma warning restore 649

        /// <summary>
        /// Get the parent packages
        /// </summary>
        /// <param name="entryContent">The entry content</param>
        /// <param name="linksRepository">The link repository</param>
        /// <returns>Collection of package references</returns>
        public static IEnumerable<ContentReference> GetPackages(this EntryContentBase entryContent, ILinksRepository linksRepository)
        {
            return linksRepository.GetRelationsByTarget<PackageEntry>(entryContent.ContentLink).Select(r => r.Source);
        }

        /// <summary>
        /// Get the parent packages
        /// </summary>
        /// <param name="entryContent">The entry content</param>
        /// <returns>Collection of package references</returns>
        public static IEnumerable<ContentReference> GetPackages(this EntryContentBase entryContent)
        {
            return entryContent.GetPackages(_linksRepository.Service);
        }

        /// <summary>
        /// Get the parent bundles
        /// </summary>
        /// <param name="entryContent">The entry content</param>
        /// <param name="linksRepository">The link repository</param>
        /// <returns>Collection of bundle references</returns>
        public static IEnumerable<ContentReference> GetBundles(this EntryContentBase entryContent, ILinksRepository linksRepository)
        {
            return linksRepository.GetRelationsByTarget<BundleEntry>(entryContent.ContentLink).Select(r => r.Source);
        }

        /// <summary>
        /// Get the parent bundles
        /// </summary>
        /// <param name="entryContent">The entry content</param>
        /// <returns>Collection of bundle references</returns>
        public static IEnumerable<ContentReference> GetBundles(this EntryContentBase entryContent)
        {
            return entryContent.GetBundles(_linksRepository.Service);
        }

        /// <summary>
        /// Get the parent categories
        /// </summary>
        /// <param name="entryContent">The entry content</param>
        /// <param name="linksRepository">The link repository</param>
        /// <returns>Collection of category content references</returns>
        public static IEnumerable<ContentReference> GetParentCategories(this EntryContentBase entryContent, ILinksRepository linksRepository)
        {
            return linksRepository.GetRelationsBySource<NodeRelation>(entryContent.ContentLink).Select(r => r.Target);
        }

        /// <summary>
        /// Get the parent categories
        /// </summary>
        /// <param name="entryContent">The entry content</param>
        /// <returns>Collection of category content references</returns>
        public static IEnumerable<ContentReference> GetParentCategories(this EntryContentBase entryContent)
        {
            return entryContent.GetParentCategories(_linksRepository.Service);
        }
    }
}