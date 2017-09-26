using System.Collections.Generic;
using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace Geta.EPi.Commerce.Extensions
{
    public static class NodeContentExtensions
    {
#pragma warning disable 649
        private static Injected<IRelationRepository> _relationRepository;
#pragma warning restore 649

        /// <summary>
        /// Get the parent categories
        /// </summary>
        /// <param name="nodeContent">The node content</param>
        /// <param name="linksRepository">The link repository</param>
        /// <returns>Collection of category content references</returns>
        public static IEnumerable<ContentReference> GetParentCategories(this NodeContent nodeContent, IRelationRepository linksRepository)
        {
            return linksRepository.GetParents<NodeRelation>(nodeContent.ContentLink).Select(r => r.Parent);
        }

        /// <summary>
        /// Get the parent categories
        /// </summary>
        /// <param name="nodeContent">The node content</param>
        /// <returns>Collection of category content references</returns>
        public static IEnumerable<ContentReference> GetParentCategories(this NodeContent nodeContent)
        {
            return nodeContent.GetParentCategories(_relationRepository.Service);
        }
    }
}