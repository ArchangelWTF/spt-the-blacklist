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
    /// Turning this on allows only found in raid items to be sold to the flea. Default: false
    /// </summary>
    [JsonPropertyName("enableFIRFleaSelling")]
    public bool EnableFIRFleaSelling { get; set; } = false;

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
            /// .338 AP round, the best round in the game so it's quite expensive. This also demonstrates the use of the priceMultiplier property. You can lower this if you think it's too expensive.
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
            /// 6.8x51mm FMJ, a bit expensive at 5k a round because of the novelty
            ItemId = new("6529302b8c26af6326029fb7"),
            FleaPriceOverride = 2000
        },
        new CustomItemConfig
        {
            /// 6.8x51mm Hybrid, it's around 2k using my ammo price balancing but we'll make it a bit more expensive due to its novelty.
            ItemId = new("6529243824cbe3c74a05e5c1"),
            FleaPriceOverride = 2800
        },
        new CustomItemConfig
        {
            /// Regular Weapon Case, I think it's too expensive at the 5 mil mark by default
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
            /// Dogtag USEC (Prestige 1)
            ItemId = new("6764207f2fa5e32733055c4a"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Dogtag BEAR (Prestige 1)
            ItemId = new("675dc9d37ae1a8792107ca96"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Dogtag USEC (Prestige 2)
            ItemId = new("6764202ae307804338014c1a"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Dogtag BEAR (Prestige 2)
            ItemId = new("675dcb0545b1a2d108011b2b"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Dogtag USEC (Prestige 3)
            ItemId = new("68418091b5b0c9e4c60f0e7a"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Dogtag BEAR (Prestige 3)
            ItemId = new("684180bc51bf8645f7067bc8"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Dogtag BEAR (Prestige 4)
            ItemId = new("684181208d035f60230f63f9"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Dogtag USEC (Prestige 4)
            ItemId = new("684180ee9b6d80d840042e8a"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Armband (Prestige 1)
            ItemId = new("67614b542eb91250020f2b86"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Armband (Prestige 2)
            ItemId = new("67614b6b47c71ea3d40256d7"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Armband (Prestige 3)
            ItemId = new("6841b2506c1fcc41ed0db319"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Armband (Prestige 4)
            ItemId = new("6841b3463fcc417de40a6768"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Poster (Prestige 1)
            ItemId = new("6759bb94b8913ff13e049669"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Poster (Prestige 2)
            ItemId = new("6759e07e4ff23436160d7fed"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Poster (Prestige 3)
            ItemId = new("6841b179322db20d190b4b99"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Poster (Prestige 4)
            ItemId = new("6841b1ff51bf8645f7067bca"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Origins figure (Prestige 3)
            ItemId = new("6841b6b674a3c16f5e03d653"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Labyrinth secret extract item
            ItemId = new("67e183377c6c2011970f3149"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Labyrinth key (Has no price)
            ItemId = new("679baa2c61f588ae2b062a24"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Labyrinth key (Has no price)
            ItemId = new("679baa4f59b8961f370dd683"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Labyrinth key (Has no price)
            ItemId = new("679baa5a59b8961f370dd685"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Labyrinth key (Has no price)
            ItemId = new("679baa9091966fe40408f149"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Labyrinth key (Has no price)
            ItemId = new("679baace4e9ca6b3d80586b2"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Labyrinth key (Has no price)
            ItemId = new("679baae891966fe40408f14c"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Labyrinth key (Has no price)
            ItemId = new("679bab714e9ca6b3d80586b4"),
            Blacklisted = true
        },
        new CustomItemConfig
        {
            /// Labyrinth key (Has no price)
            ItemId = new("679bac1d61f588ae2b062a26"),
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
