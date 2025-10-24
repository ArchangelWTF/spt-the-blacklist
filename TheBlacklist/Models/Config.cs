using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;

namespace TheBlacklist.Models;

public record Config
{
    /// <summary>
    /// Main feature of the mod, set to true to disable blacklist, false to keep it enabled.
    /// Other parts of the mod still apply. Default: true.
    /// </summary>
    [JsonPropertyName("disableBsgBlacklist")]
    public bool DisableBsgBlacklist { get; set; } = true;

    /// <summary>
    /// Make cheap offers sell faster. Default: true.
    /// </summary>
    [JsonPropertyName("enableFasterSales")]
    public bool EnableFasterSales { get; set; } = true;

    /// <summary>
    /// Makes your sales go through slower. Encourages bulk selling. Overwrites `enableFasterSales` if true. Default: false.
    /// </summary>
    [JsonPropertyName("enableSlowerSales")]
    public bool EnableSlowerSales { get; set; } = false;

    /// <summary>
    /// Reduces number of offers and quantities per offer to make flea market experience more hardcore. Default: false.
    /// </summary>
    [JsonPropertyName("enableScarceOffers")]
    public bool EnableScarceOffers { get; set; } = false;

    /// <summary>
    /// Adds an extra offer for your current flea market rating bracket. Default: false.
    /// </summary>
    [JsonPropertyName("addExtraOfferSlot")]
    public bool AddExtraOfferSlot { get; set; } = false;

    /// <summary>
    /// Balances flea prices of all ammo types. Useful if ammo prices are unbalanced. Default: false.
    /// </summary>
    [JsonPropertyName("useBalancedPricingForAllAmmo")]
    public bool UseBalancedPricingForAllAmmo { get; set; } = false;

    /// <summary>
    /// Multiplies the price of all blacklisted items that are now available. Default: 1.
    /// </summary>
    [JsonPropertyName("blacklistedItemPriceMultiplier")]
    public double BlacklistedItemPriceMultiplier { get; set; } = 1.0;

    /// <summary>
    /// Multiplies the price of all blacklisted ammo in addition to `blacklistedItemPriceMultiplier`. Default: 1.
    /// </summary>
    [JsonPropertyName("blacklistedAmmoAdditionalPriceMultiplier")]
    public double BlacklistedAmmoAdditionalPriceMultiplier { get; set; } = 1.0;

    /// <summary>
    /// Manually add flea prices to items, set multipliers or blacklist items.
    /// </summary>
    [JsonPropertyName("customItemConfigs")]
    public List<CustomItemConfigBase> CustomItemConfigs { get; set; } =
    [
        new CustomParentConfig
            {
                /// Headwear parent id. Affects all items under this parent. ie. Team Wendy Exfil, Airframe etc.
                ParentId = new("5a341c4086f77401f2541505"),
                PriceMultiplier = 12.5
            },
            new CustomItemConfig
            {
                /// .338 AP round, expensive. Demonstrates priceMultiplier.
                ItemId = new("5fc382a9d724d907e2077dab"),
                PriceMultiplier = 1.2
            },
            new CustomItemConfig
            {
                /// .338 FMJ round
                ItemId = new("5fc275cf85fd526b824a571a"),
                PriceMultiplier = 1.3
            },
            new CustomItemConfig
            {
                /// 6.8x51mm FMJ, a bit expensive due to novelty
                ItemId = new("6529302b8c26af6326029fb7"),
                FleaPriceOverride = 2000
            },
            new CustomItemConfig
            {
                /// 6.8x51mm Hybrid, slightly more expensive than base price
                ItemId = new("6529243824cbe3c74a05e5c1"),
                FleaPriceOverride = 2800
            },
            new CustomItemConfig
            {
                /// Regular Weapon Case, too expensive by default
                ItemId = new("59fb023c86f7746d0d4b423c"),
                FleaPriceOverride = 1500000
            },
            new CustomItemConfig
            {
                /// Dogtag BEAR
                ItemId = new("59f32bb586f774757e1e8442"),
                Blacklisted = true
            },
            new CustomItemConfig
            {
                /// Dogtag BEAR (EOD)
                ItemId = new("6662e9aca7e0b43baa3d5f74"),
                Blacklisted = true
            },
            new CustomItemConfig
            {
                /// Dogtag BEAR (Unhinged edition)
                ItemId = new("6662e9cda7e0b43baa3d5f76"),
                Blacklisted = true
            },
            new CustomItemConfig
            {
                /// Dogtag USEC
                ItemId = new("59f32c3b86f77472a31742f0"),
                Blacklisted = true
            },
            new CustomItemConfig
            {
                /// Dogtag USEC (EOD)
                ItemId = new("6662e9f37fa79a6d83730fa0"),
                Blacklisted = true
            },
            new CustomItemConfig
            {
                /// Dogtag USEC (Unhinged edition)
                ItemId = new("6662ea05f6259762c56f3189"),
                Blacklisted = true
            }
    ];
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(CustomItemConfig), "item")]
[JsonDerivedType(typeof(CustomParentConfig), "parent")]
public record CustomItemConfigBase
{
    [JsonPropertyName("priceMultiplier")]
    public double? PriceMultiplier { get; set; }

    [JsonPropertyName("fleaPriceOverride")]
    public double? FleaPriceOverride { get; set; }

    [JsonPropertyName("blacklisted")]
    public bool? Blacklisted { get; set; }
}

public record CustomParentConfig : CustomItemConfigBase
{
    [JsonPropertyName("parentId")]
    public required MongoId ParentId { get; init; }
}

public record CustomItemConfig : CustomItemConfigBase
{
    [JsonPropertyName("itemId")]
    public required MongoId ItemId { get; init; }
}
