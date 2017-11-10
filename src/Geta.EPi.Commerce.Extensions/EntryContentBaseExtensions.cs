using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace Geta.EPi.Commerce.Extensions
{
    public static class EntryContentBaseExtensions
    {
#pragma warning disable 649
        private static Injected<IRelationRepository> _relationRepository;
        private static Injected<IContentLoader> _contentLoader;
#pragma warning restore 649

        /// <summary>
        /// Get the parent packages
        /// </summary>
        /// <param name="entryContent">The entry content</param>
        /// <param name="relationRepository">The relation repository</param>
        /// <returns>Collection of package references</returns>
        public static IEnumerable<ContentReference> GetPackages(this EntryContentBase entryContent, IRelationRepository relationRepository)
        {
            return relationRepository.GetParents<PackageEntry>(entryContent.ContentLink).Select(r => r.Parent);
        }

        /// <summary>
        /// Get the parent packages
        /// </summary>
        /// <param name="entryContent">The entry content</param>
        /// <returns>Collection of package references</returns>
        public static IEnumerable<ContentReference> GetPackages(this EntryContentBase entryContent)
        {
            return entryContent.GetPackages(_relationRepository.Service);
        }

        /// <summary>
        /// Get the parent bundles
        /// </summary>
        /// <param name="entryContent">The entry content</param>
        /// <param name="relationRepository">The relation repository</param>
        /// <returns>Collection of bundle references</returns>
        public static IEnumerable<ContentReference> GetBundles(this EntryContentBase entryContent, IRelationRepository relationRepository)
        {
            return relationRepository.GetParents<BundleEntry>(entryContent.ContentLink).Select(r => r.Parent);
        }

        /// <summary>
        /// Get the parent bundles
        /// </summary>
        /// <param name="entryContent">The entry content</param>
        /// <returns>Collection of bundle references</returns>
        public static IEnumerable<ContentReference> GetBundles(this EntryContentBase entryContent)
        {
            return entryContent.GetBundles(_relationRepository.Service);
        }

        /// <summary>
        /// Get the parent categories
        /// </summary>
        /// <param name="entryContent">The entry content</param>
        /// <param name="relationRepository">The relation repository</param>
        /// <returns>Collection of category content references</returns>
        public static IEnumerable<ContentReference> GetParentCategories(this EntryContentBase entryContent, IRelationRepository relationRepository)
        {
            return relationRepository.GetParents<NodeRelation>(entryContent.ContentLink).Select(r => r.Parent);
        }

        /// <summary>
        /// Get the parent categories
        /// </summary>
        /// <param name="entryContent">The entry content</param>
        /// <returns>Collection of category content references</returns>
        public static IEnumerable<ContentReference> GetParentCategories(this EntryContentBase entryContent)
        {
            return entryContent.GetParentCategories(_relationRepository.Service);
        }

        /// <summary>
        /// Get ancestor categories.
        /// </summary>
        /// <param name="entryContent">The entry content.</param>
        /// <returns>Collection of category content.</returns>
        public static IEnumerable<NodeContent> GetAncestorCategories(
            this EntryContentBase entryContent)
        {
            return entryContent.GetAncestorCategories(_contentLoader.Service);
        }

        /// <summary>
        /// Get ancestor categories.
        /// </summary>
        /// <param name="entryContent">The entry content.</param>
        /// <param name="contentLoader">The content loader.</param>
        /// <returns>Collection of category content.</returns>
        public static IEnumerable<NodeContent> GetAncestorCategories(
            this EntryContentBase entryContent, IContentLoader contentLoader)
        {
            return entryContent.GetCategories()
                .SelectMany(parentLink =>
                    contentLoader.GetAncestors(parentLink)
                        .OfType<NodeContent>()
                        .Concat(new[] { contentLoader.Get<NodeContent>(parentLink) }))
                .Distinct(new NodeContentComparer());
        }

        private class NodeContentComparer : IEqualityComparer<NodeContent>
        {
            public bool Equals(NodeContent x, NodeContent y)
            {
                return x != null && y != null && x.Code == y.Code;
            }

            public int GetHashCode(NodeContent obj)
            {
                return obj.Code.GetHashCode();
            }
        }
    }
}