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

### LineItem

```
string url = lineItem.GetUrl();
string fullUrl = lineItem.GetFullUrl();
string thumbnail = lineItem.GetThumbnailUrl();
Money price = lineItem.PlacedPriceTotal();
Money discountPrice = lineItem.ExtendedPriceTotal();
Money placedPriceTotal = lineItem.ToMoney(lineItem.PlacedPrice * lineItem.Quantity);
```

### VariantContent

```
string url = variant.GetUrl();
```
