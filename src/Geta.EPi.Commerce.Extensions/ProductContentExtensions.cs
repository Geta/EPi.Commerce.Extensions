using System.Collections.Generic;
using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace Geta.EPi.Commerce.Extensions
{
    public static class ProductContentExtensions
    {
        #pragma warning disable 649
        private static Injected<ILinksRepository> _linksRepository;
        #pragma warning restore 649

        /// <summary>
        /// Gets all variants/SKUs for a product
        /// </summary>
        /// <param name="productContent">The product</param>
        /// <param name="linksRepository">The links repository</param>
        /// <returns></returns>
        public static IEnumerable<ContentReference> GetVariations(this ProductContent productContent, ILinksRepository linksRepository)
        {
            return linksRepository.GetRelationsBySource<ProductVariation>(productContent.ContentLink).Select(r => r.Source);
        }

        /// <summary>
        /// Gets all variants/SKUs for a product
        /// </summary>
        /// <param name="productContent">The product</param>
        /// <returns></returns>
        public static IEnumerable<ContentReference> GetVariations(this ProductContent productContent)
        {
            return _linksRepository.Service.GetRelationsBySource<ProductVariation>(productContent.ContentLink).Select(r => r.Source);
        }
    }
}