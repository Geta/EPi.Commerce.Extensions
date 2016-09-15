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
        private static Injected<ILinksRepository> _linksRepository;
        #pragma warning restore 649

        /// <summary>
        /// Gets package entries for a package
        /// </summary>
        /// <param name="packageContent">The package content</param>
        /// <param name="linksRepository">The link repository</param>
        /// <returns>Collection of package entry references</returns>
        public static IEnumerable<ContentReference> GetPackageEntries(this PackageContent packageContent, ILinksRepository linksRepository)
        {
            return linksRepository.GetRelationsBySource<PackageEntry>(packageContent.ContentLink).Select(r => r.Source);
        }

        /// <summary>
        /// Gets package entries for a package
        /// </summary>
        /// <param name="packageContent">The package content</param>
        /// <returns>Collection of package entry references</returns>
        public static IEnumerable<ContentReference> GetPackageEntries(this PackageContent packageContent)
        {
            return _linksRepository.Service.GetRelationsBySource<PackageEntry>(packageContent.ContentLink).Select(r => r.Source);
        }
    }
}