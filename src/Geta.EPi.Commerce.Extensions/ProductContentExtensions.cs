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
        private static Injected<IRelationRepository> _relationRepository;
        #pragma warning restore 649

        /// <summary>
        /// Gets all variants/SKUs for a product
        /// </summary>
        /// <param name="productContent">The product</param>
        /// <param name="relationRepository">The relation repository</param>
        /// <returns>Collection of variation references</returns>
        public static IEnumerable<ContentReference> GetVariations(this ProductContent productContent, IRelationRepository relationRepository)
        {
            return relationRepository.GetChildren<ProductVariation>(productContent.ContentLink).Select(r => r.Child);
        }

        /// <summary>
        /// Gets all variants/SKUs for a product
        /// </summary>
        /// <param name="productContent">The product</param>
        /// <returns>Collection of variation references</returns>
        public static IEnumerable<ContentReference> GetVariations(this ProductContent productContent)
        {
            return productContent.GetVariations(_relationRepository.Service);
        }
    }
}