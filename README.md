# Geta.EPi.Commerce.Extensions

![](http://tc.geta.no/app/rest/builds/buildType:(id:TeamFrederik_EPiCommerceExtensions_EPiCommerceExtensionsBuildAndPublishNuGetPack)/statusIcon)
[![Platform](https://img.shields.io/badge/Platform-.NET%204.6.1-blue.svg?style=flat)](https://msdn.microsoft.com/en-us/library/w0x726c2%28v=vs.110%29.aspx)
[![Platform](https://img.shields.io/badge/Episerver%20Commerce-%2011-orange.svg?style=flat)](http://world.episerver.com/commerce/)

## Description

Helpers and extension methods for Episerver Commerce.

```
Install-Package Geta.EPi.Commerce.Extensions
```

## Features

#### Extension methods for:
* **EPiServer.Commerce.Catalog.ContentTypes.IAssetContainer** - *get assets of Episerver's default group*
* **EPiServer.Commerce.Catalog.ContentTypes.BundleContent** - *gets all bundle entries for a bundle*
* **EPiServer.Commerce.Catalog.ContentTypes.EntryContentBase** - *gets the parent packages, bundles and categories*
* **EPiServer.Commerce.Order.ILineItem** - *gets Url, FullUrl and Thumbnailurl of an EPiServer.Commerce.Catalog.ContentTypes.EntryContentBase*
* **EPiServer.Commerce.Catalog.ContentTypes** - *gets the parent categories*
* **EPiServer.Commerce.Order.IOrderGroup** - *converts decimal to Mediachase.Commerce.Money*
* **EPiServer.Commerce.Catalog.ContentTypes.PackageContent** - *gets package entries for a package*
* **Mediachase.Commerce.Pricing.IPriceDetailService** - *saves the MSRP price*
* **Mediachase.Commerce.Pricing.IPriceService** - *gets the previous price and load the Msrp.*
* **EPiServer.Commerce.Catalog.ContentTypes.ProductContent** - *gets all variants/SKUs for a product*
* **EPiServer.Commerce.Catalog.ContentTypes.VariationContent** - *get the url and parent products*


## Examples

### Asset

```csharp
string image = variant.GetDefaultAsset<IContentImage>();
IEnumerable<string> images = variation.GetAssets<IContentImage>();
```

### Bundle content

```csharp
IEnumerable<ContentReference> GetBundleEntries(this BundleContent bundleContent);
IEnumerable<ContentReference> GetBundleEntries(this BundleContent bundleContent, IRelationRepository relationRepository);
```

### Entry content

```csharp
IEnumerable<ContentReference> GetPackages(this EntryContentBase entryContent);
IEnumerable<ContentReference> GetPackages(this EntryContentBase entryContent, IRelationRepository relationRepository);

IEnumerable<ContentReference> GetBundles(this EntryContentBase entryContent);
IEnumerable<ContentReference> GetBundles(this EntryContentBase entryContent, IRelationRepository relationRepository);

IEnumerable<ContentReference> GetParentCategories(this EntryContentBase entryContent);
IEnumerable<ContentReference> GetParentCategories(this EntryContentBase entryContent, IRelationRepository relationRepository);
```

### LineItem

```csharp
string url = lineItem.GetUrl();
string fullUrl = lineItem.GetFullUrl();
string thumbnail = lineItem.GetThumbnailUrl();
```

## Cart

```csharp
void AddValidationIssues(this Dictionary<ILineItem, List<ValidationIssue>> issues, ILineItem lineItem, ValidationIssue issue);
bool HasItemBeenRemoved(this Dictionary<ILineItem, List<ValidationIssue>> issuesPerLineItem, ILineItem lineItem);
```

### Node content

```csharp
IEnumerable<ContentReference> GetParentCategories(this NodeContent nodeContent);
IEnumerable<ContentReference> GetParentCategories(this NodeContent nodeContent, IRelationRepository relationRepository)
```

### Package content

```csharp
IEnumerable<ContentReference> GetPackageEntries(this PackageContent packageContent);
IEnumerable<ContentReference> GetPackageEntries(this PackageContent packageContent, IRelationRepository relationRepository);
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
IEnumerable<ContentReference> GetVariations(this ProductContent productContent, IRelationRepository relationRepository);
```

### VariantContent

```csharp
string url = variant.GetUrl();
IEnumerable<ContentReference> GetProducts(this VariationContent variationContent, IRelationRepository relationRepository);
IEnumerable<ContentReference> GetProducts(this VariationContent variationContent);
```

## Package maintainer 
https://github.com/frederikvig
