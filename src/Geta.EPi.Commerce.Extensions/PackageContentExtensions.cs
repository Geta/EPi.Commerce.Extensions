using System.Collections.Generic;
using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace Geta.EPi.Commerce.Extensions
{
    public static class PackageContentExtensions
    {
        #pragma warning disable 649
        private static Injected<IRelationRepository> _relationRepository;
        #pragma warning restore 649

        /// <summary>
        /// Gets package entries for a package
        /// </summary>
        /// <param name="packageContent">The package content</param>
        /// <param name="relationRepository">The relation repository</param>
        /// <returns>Collection of package entry references</returns>
        public static IEnumerable<ContentReference> GetPackageEntries(this PackageContent packageContent, IRelationRepository relationRepository)
        {
            return relationRepository.GetChildren<PackageEntry>(packageContent.ContentLink).Select(r => r.Child);
        }

        /// <summary>
        /// Gets package entries for a package
        /// </summary>
        /// <param name="packageContent">The package content</param>
        /// <returns>Collection of package entry references</returns>
        public static IEnumerable<ContentReference> GetPackageEntries(this PackageContent packageContent)
        {
            return packageContent.GetPackageEntries(_relationRepository.Service);
        }
    }
}