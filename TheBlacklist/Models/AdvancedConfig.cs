using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;

namespace TheBlacklist.Models;

public record AdvancedConfig
{
    /// <summary>
    /// Represents the M856A1 bullet which is a blacklist round that is good to balance the price of other ammos around.
    /// </summary>
    [JsonPropertyName("baselineBulletId")]
    public MongoId BaselineBulletId { get; set; } = "59e6906286f7746c9f75e847";

    /// <summary>
    /// The M856A1 will be 1000 roubles on the flea, better blacklisted rounds will be more expensive proportionally and worse rounds will be cheaper.
    /// </summary>
    [JsonPropertyName("baselineBulletPrice")]
    public int BaselineBulletPrice { get; set; } = 1000;

    /// <summary>
    /// Reduces the weighting of the bullet damage against their flea price. So a blacklisted low pen, high dmg round won't be as expensive as better pen rounds.
    /// </summary>
    [JsonPropertyName("bulletDamageMultiplierRedutionFactor")]
    public double BulletDamageMultiplierReductionFactor { get; set; } = 0.7;

    /// <summary>
    /// When enabled, creates more logging, mostly for Platinum to debug things.
    /// </summary>
    [JsonPropertyName("enableDebug")]
    public bool EnableDebug { get; set; } = false;

    /// <summary>
    /// Items that are blacklisted in these categories will remain blacklisted.
    /// </summary>
    [JsonPropertyName("excludedCategories")]
    public List<MongoId> ExcludedCategories { get; set; } =
    [
        "5b5f78b786f77447ed5636af", // Money
        "5b47574386f77428ca22b345", // Special equipment
        "5b47574386f77428ca22b33c"  // Ammo boxes
    ];

    /// <summary>
    /// By default, SPT flea prices will use a trader's price for an item if the trader's price is higher.
    /// Custom item configs from this mod won't apply so you might need to disable this if you have some items set really cheap.
    /// </summary>
    [JsonPropertyName("useTraderPriceForOffersIfHigher")]
    public bool UseTraderPriceForOffersIfHigher { get; set; } = true;

    /// <summary>
    /// When an item doesn't have a flea price because it was blacklisted by default,
    /// multiply the handbook price by this number to get the new flea price. Default: 3.
    /// </summary>
    [JsonPropertyName("handbookPriceMultiplier")]
    public int HandbookPriceMultiplier { get; set; } = 3;

    /// <summary>
    /// Overrides ragfairConfig.runIntervalValues.outOfRaid if config.enableFasterSales is true. Default: 3 seconds.
    /// </summary>
    [JsonPropertyName("runIntervalSecondsOverride")]
    public int RunIntervalSecondsOverride { get; set; } = 3;

    /// <summary>
    /// Sale times when slower sales is enabled.
    /// </summary>
    [JsonPropertyName("slowerSalesTime")]
    public MinMax<double> SlowerSalesTime { get; set; } = new MinMax<double> { Min = 5, Max = 10 };

    /// <summary>
    /// When config.enableScarceOffers is true, use these values to limit the number of offers on the flea.
    /// </summary>
    [JsonPropertyName("offerItemCountOverride")]
    public MinMax<int> OfferItemCountOverride { get; set; } = new MinMax<int> { Min = 2, Max = 4 };

    /// <summary>
    /// When config.enableScarceOffers is true, use these values to limit the quantity of a stackable item (like ammo) per offer on the flea.
    /// </summary>
    [JsonPropertyName("stackablePercentOverride")]
    public MinMax<double> StackablePercentOverride { get; set; } = new MinMax<double> { Min = 20, Max = 200 };

    /// <summary>
    /// When config.enableScarceOffers is true, use these values to limit the quantity of a non-stackable item (most items) per offer on the flea.
    /// </summary>
    [JsonPropertyName("nonStackableCountOverride")]
    public MinMax<int> NonStackableCountOverride { get; set; } = new MinMax<int> { Min = 1, Max = 2 };

    /// <summary>
    /// Adds the specified amount of offers for your current flea market rating if config.addExtraOfferSlot is enabled.
    /// </summary>
    [JsonPropertyName("extraOfferSlotsToAdd")]
    public int ExtraOfferSlotsToAdd { get; set; } = 1;
}
