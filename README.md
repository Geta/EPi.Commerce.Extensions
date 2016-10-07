# Geta.EPi.Commerce.Extensions

![](http://tc.geta.no/app/rest/builds/buildType:(id:TeamFrederik_EPiCommerceExtensions_ExtensionsDebug)/statusIcon)
[![Platform](https://img.shields.io/badge/Platform-.NET 4.5.2-blue.svg?style=flat)](https://msdn.microsoft.com/en-us/library/w0x726c2%28v=vs.110%29.aspx)
[![Platform](https://img.shields.io/badge/EPiServer%20Commerce-%208.11.2-orange.svg?style=flat)](http://world.episerver.com/cms/)

```
Install-Package Geta.EPi.Commerce.Extensions
```

## Examples

### Asset

```
string image = variant.GetDefaultAsset<IContentImage>();
IEnumerable<string> images = variation.GetAssets<IContentImage>();
```

### Bundle content

```csharp
IEnumerable<ContentReference> GetBundleEntries(this BundleContent bundleContent);
IEnumerable<ContentReference> GetBundleEntries(this BundleContent bundleContent, ILinksRepository linksRepository);
```

### Entry content

```csharp
IEnumerable<ContentReference> GetPackages(this EntryContentBase entryContent);
IEnumerable<ContentReference> GetPackages(this EntryContentBase entryContent, ILinksRepository linksRepository);

IEnumerable<ContentReference> GetBundles(this EntryContentBase entryContent);
IEnumerable<ContentReference> GetBundles(this EntryContentBase entryContent, ILinksRepository linksRepository);

IEnumerable<ContentReference> GetParentCategories(this EntryContentBase entryContent);
IEnumerable<ContentReference> GetParentCategories(this EntryContentBase entryContent, ILinksRepository linksRepository);
```

### LineItem

```
string url = lineItem.GetUrl();
string fullUrl = lineItem.GetFullUrl();
string thumbnail = lineItem.GetThumbnailUrl();
Money price = lineItem.PlacedPriceTotal();
Money discountPrice = lineItem.ExtendedPriceTotal();
Money placedPriceTotal = lineItem.ToMoney(lineItem.PlacedPrice * lineItem.Quantity);
```

### Node content

```csharp
IEnumerable<ContentReference> GetParentCategories(this NodeContent nodeContent);
IEnumerable<ContentReference> GetParentCategories(this NodeContent nodeContent, ILinksRepository linksRepository)
```

### Package content

```csharp
IEnumerable<ContentReference> GetPackageEntries(this PackageContent packageContent);
IEnumerable<ContentReference> GetPackageEntries(this PackageContent packageContent, ILinksRepository linksRepository);
```

### Price detail service

[MSRP](https://en.wikipedia.org/wiki/List_price) (list price/manufacturer's suggested retail price)

```csharp
SaveMsrp(this IPriceDetailService priceDetailService, ContentReference contentLink, MarketId marketId, Currency currency, decimal amount);
```

### Price service

[MSRP](https://en.wikipedia.org/wiki/List_price) (list price/manufacturer's suggested retail price)

```csharp
IPriceValue LoadMsrp(this IPriceService priceService, ContentReference contentLink, MarketId marketId, Currency currency);

// Example usage: Was 1000 now only 800
IPriceValue GetPreviousPrice(this IPriceService priceService, ContentReference contentLink, MarketId marketId, Currency currency);
```

### Product content

```csharp
IEnumerable<ContentReference> GetVariations(this ProductContent productContent);
IEnumerable<ContentReference> GetVariations(this ProductContent productContent, ILinksRepository linksRepository);
```

### VariantContent

```
string url = variant.GetUrl();
IEnumerable<ContentReference> GetProducts(this VariationContent variationContent, ILinksRepository linksRepository);
IEnumerable<ContentReference> GetProducts(this VariationContent variationContent);
```
