# Geta.EPi.Commerce.Extensions

![](http://tc.geta.no/app/rest/builds/buildType:(id:TeamFrederik_EPiCommerceExtensions_ExtensionsDebug)/statusIcon)

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

### LineItem

```
string url = lineItem.GetUrl();
string fullUrl = lineItem.GetFullUrl();
string thumbnail = lineItem.GetThumbnailUrl();
Money price = lineItem.PlacedPriceTotal();
Money discountPrice = lineItem.ExtendedPriceTotal();
Money placedPriceTotal = lineItem.ToMoney(lineItem.PlacedPrice * lineItem.Quantity);
```

### Package content

```csharp
IEnumerable<ContentReference> GetPackageEntries(this PackageContent packageContent);
IEnumerable<ContentReference> GetPackageEntries(this PackageContent packageContent, ILinksRepository linksRepository);
```

### Product content

```csharp
IEnumerable<ContentReference> GetVariations(this ProductContent productContent);
IEnumerable<ContentReference> GetVariations(this ProductContent productContent, ILinksRepository linksRepository);
```

### VariantContent

```
string url = variant.GetUrl();
```
