using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Servers;
using TheBlacklist.Models;
using TheBlacklist.Services;
using TheBlacklist.Utilities;

namespace TheBlacklist.OnLoad;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + TheBlacklistLoadPriority.TheBlacklistPriorityOffset)]
public class PostDBLoad(ConfigService configService, DatabaseServer databaseServer, ConfigServer configServer, TheBlacklistLogger logger) : IOnLoad
{
    private readonly RagfairConfig _ragfairConfig = configServer.GetConfig<RagfairConfig>();
    private Dictionary<MongoId, TemplateItem> _itemsTable = [];
    private Dictionary<MongoId, double> _pricesTable = [];
    private TemplateItem? _baselineBullet;

    private int _blacklistedItemsUpdatedCount = 0;
    private int _nonBlacklistedItemsUpdatedCount = 0;
    private int _ammoPricesUpdatedCount = 0;

    public Task OnLoad()
    {
        // Initialize various database tables here, so we can use them throughout this class
        _itemsTable = databaseServer.GetTables().Templates.Items;
        _pricesTable = databaseServer.GetTables().Templates.Prices;
        _baselineBullet = _itemsTable[configService.TheBlacklistAdvancedConfig.BaselineBulletId];

        UpdateRagfairConfig();
        UpdateGlobals();

        foreach (var handbookItem in databaseServer.GetTables().Templates.Handbook.Items)
        {
            var item = _itemsTable[handbookItem.Id];

            var originalPrice = 0d;

            if (_pricesTable.TryGetValue(item.Id, out var pricesValue))
            {
                originalPrice = pricesValue;
            }

            var customItemConfig = configService.TheBlacklistConfig.CustomItemConfigs.FirstOrDefault(conf =>
                 conf switch
                 {
                     CustomItemConfig itemConfig => itemConfig.ItemId == item.Id,
                     CustomParentConfig parentConfig => parentConfig.ParentId == item.Parent,
                     _ => false
                 });

            if (customItemConfig is not null && UpdateItemUsingCustomItemConfig(customItemConfig, item, originalPrice))
            {
                continue;
            }

            var itemProps = item.Properties;

            if (itemProps is null)
            {
                logger.Warning($"Item properties on item {item.Id} - {item.Name} are null!");
                continue;
            }

            if (item.IsBulletOrShotgunShell())
            {
                UpdateAmmoPrice(item);
            }

            if (!itemProps.CanSellOnRagfair ?? false)
            {
                // Some blacklisted items are hard to balance or just shouldn't be allowed so we will keep them blacklisted.
                if (configService.TheBlacklistAdvancedConfig.ExcludedCategories.Contains(handbookItem.ParentId))
                {
                    _ragfairConfig.Dynamic.Blacklist.Custom.Add(item.Id);

                    if (logger.IsDebug())
                    {
                        logger.Debug($"Ignoring item {item.Id} - {item.Name} because we are excluding handbook category {handbookItem.ParentId}.");
                    }

                    continue;
                }

                var itemPrice = GetUpdatedPrice(handbookItem, item);

                if (itemPrice is null || itemPrice == 0)
                {
                    if (logger.IsDebug())
                    {
                        logger.Debug($"There are no flea prices for {item.Id} - {item.Name}!");
                    }

                    continue;
                }

                if (customItemConfig?.PriceMultiplier is not null)
                {
                    itemPrice *= customItemConfig.PriceMultiplier;
                }

                _pricesTable[item.Id] = itemPrice.Value;

                if (logger.IsDebug())
                {
                    logger.Debug($"Updated {item.Id} - {item.Name} flea price from {originalPrice} to {itemPrice}.");
                }

                itemProps.CanSellOnRagfair = true;

                _blacklistedItemsUpdatedCount++;
            }
        }

        logger.Success($"Success! Found {_blacklistedItemsUpdatedCount} blacklisted & {_nonBlacklistedItemsUpdatedCount} non-blacklisted items to update.");

        if (configService.TheBlacklistConfig.UseBalancedPricingForAllAmmo)
        {
            logger.Success($"config.useBalancedPricingForAllAmmo is enabled! Updated {_ammoPricesUpdatedCount} ammo prices.");
        }

        return Task.CompletedTask;
    }

    public void UpdateRagfairConfig()
    {
        _ragfairConfig.Dynamic.Blacklist.EnableBsgList = !configService.TheBlacklistConfig.DisableBsgBlacklist;
        _ragfairConfig.Dynamic.UseTraderPriceForOffersIfHigher = !configService.TheBlacklistAdvancedConfig.UseTraderPriceForOffersIfHigher;

        if (!configService.TheBlacklistConfig.EnableSlowerSales && configService.TheBlacklistConfig.EnableFasterSales)
        {
            _ragfairConfig.RunIntervalValues.OutOfRaid = configService.TheBlacklistAdvancedConfig.RunIntervalSecondsOverride;
        }

        if (configService.TheBlacklistConfig.EnableSlowerSales)
        {
            _ragfairConfig.Sell.Time = configService.TheBlacklistAdvancedConfig.SlowerSalesTime;
        }

        if (configService.TheBlacklistConfig.EnableScarceOffers)
        {
            SetScarceOffers();
        }
    }

    public void SetScarceOffers()
    {
        foreach (var offerItemCount in _ragfairConfig.Dynamic.OfferItemCount)
        {
            offerItemCount.Value.Min = configService.TheBlacklistAdvancedConfig.OfferItemCountOverride.Min;
            offerItemCount.Value.Max = configService.TheBlacklistAdvancedConfig.OfferItemCountOverride.Max;
        }

        _ragfairConfig.Dynamic.StackablePercent = configService.TheBlacklistAdvancedConfig.StackablePercentOverride;
        _ragfairConfig.Dynamic.NonStackableCount = configService.TheBlacklistAdvancedConfig.NonStackableCountOverride;

        _ragfairConfig.Dynamic.Barter.ChancePercent = 0;
        _ragfairConfig.Dynamic.Pack.ChancePercent = 0;
    }

    private bool UpdateItemUsingCustomItemConfig(CustomItemConfigBase customItemConfig, TemplateItem item, double originalPrice)
    {
        if (customItemConfig.Blacklisted ?? false)
        {
            if (logger.IsDebug())
            {
                logger.Debug($"Blacklisted item {item.Id} - {item.Name} due to its customItemConfig.");
            }

            _ragfairConfig.Dynamic.Blacklist.Custom.Add(item.Id);

            if (item.Properties?.CanSellOnRagfair ?? false)
            {
                _nonBlacklistedItemsUpdatedCount++;
            }

            return true;
        }

        if (customItemConfig.FleaPriceOverride is not null)
        {
            _pricesTable[item.Id] = customItemConfig.FleaPriceOverride.Value;

            if (logger.IsDebug())
            {
                logger.Debug($"Updated {item.Id} - {item.Name} flea price from {originalPrice} to {_pricesTable[item.Id]} (price override).");
            }

            if (item.Properties?.CanSellOnRagfair ?? false)
            {
                _nonBlacklistedItemsUpdatedCount++;
            }

            return true;
        }

        return false;
    }

    private void UpdateAmmoPrice(TemplateItem item)
    {
        var itemProperties = item.Properties;
        var canSellOnRagfair = itemProperties?.CanSellOnRagfair ?? false;

        // We don't care about this standard ammo item if we haven't enabled useBalancedPricingForAllAmmo
        if (canSellOnRagfair && !configService.TheBlacklistConfig.UseBalancedPricingForAllAmmo)
        {
            return;
        }

        var newAmmoPrice = GetUpdatedAmmoPrice(item);

        if (newAmmoPrice is not null)
        {
            _pricesTable[item.Id] = newAmmoPrice.Value;

            if (canSellOnRagfair)
            {
                _blacklistedItemsUpdatedCount++;
            }
            else
            {
                _nonBlacklistedItemsUpdatedCount++;
            }

            _ammoPricesUpdatedCount++;
        }
        else
        {
            logger.Warning($"Could not update ammo price on {item.Id} - {item.Name}");
        }
    }

    private double? GetUpdatedAmmoPrice(TemplateItem item)
    {
        if (_baselineBullet is null || _baselineBullet.Properties is null)
        {
            return null;
        }

        var baselinePen = _baselineBullet.Properties.PenetrationPower;
        var baselineDamage = _baselineBullet.Properties.Damage;

        var basePenetrationMultiplier = item.Properties?.PenetrationPower / baselinePen;
        var baseDamageMultiplier = item.Properties?.Damage / baselineDamage;

        if (basePenetrationMultiplier is null || baseDamageMultiplier is null)
        {
            return null;
        }

        double? penetrationMultiplier;

        // We are checking for > 0.99 because we want the baseline bullet (mult of 1) to be close to its baseline price.
        if (basePenetrationMultiplier > 0.99)
        {
            // A good gradient to make higher power rounds more expensive
            penetrationMultiplier = 3 * basePenetrationMultiplier - 2;
        }
        else
        {
            // The baseline ammo is mid tier with a reasonable 1000 rouble each. Ammo weaker than this tend to be pretty crap so we'll make it much cheaper
            var newMultiplier = basePenetrationMultiplier * 0.7;
            penetrationMultiplier = newMultiplier < 0.1 ? 0.1 : newMultiplier;
        }

        // Reduces the effect of the damage multiplier so high DMG rounds aren't super expensive.
        // Eg. let baseDamageMultiplier = 2 & bulletDamageMultiplierRedutionFactor = 0.7. Instead of a 2x price when a bullet is 2x damage, we instead get:
        // 2 + (1 - 2) * 0.7 = 2 - 0.7 = 1.3x the price.
        var damageMultiplier = baseDamageMultiplier + (1 - baseDamageMultiplier) * configService.TheBlacklistAdvancedConfig.BulletDamageMultiplierReductionFactor;

        return configService.TheBlacklistAdvancedConfig.BaselineBulletPrice * penetrationMultiplier * damageMultiplier * configService.TheBlacklistConfig.BlacklistedAmmoAdditionalPriceMultiplier;
    }

    private double? GetUpdatedPrice(HandbookItem handbookItem, TemplateItem templateItem)
    {
        if (!_pricesTable.TryGetValue(templateItem.Id, out double priceTableValue))
        {
            return handbookItem.Price * configService.TheBlacklistAdvancedConfig.HandbookPriceMultiplier;
        }


        return priceTableValue * configService.TheBlacklistConfig.BlacklistedItemPriceMultiplier;
    }

    public void UpdateGlobals()
    {
        if (configService.TheBlacklistConfig.AddExtraOfferSlot)
        {
            var ragfairGlobals = databaseServer.GetTables().Globals.Configuration.RagFair;

            foreach (var maxActiveOfferCount in ragfairGlobals.MaxActiveOfferCount)
            {
                maxActiveOfferCount.Count += configService.TheBlacklistAdvancedConfig.ExtraOfferSlotsToAdd;
            }
        }
    }
}
