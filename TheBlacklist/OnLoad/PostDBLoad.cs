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
    private TemplateItem? _baselineBullet;

    private int _blacklistedItemsUpdatedCount = 0;
    private int _nonBlacklistedItemsUpdatedCount = 0;
    private int _ammoPricesUpdatedCount = 0;

    public Task OnLoad()
    {
        var itemTable = databaseServer.GetTables().Templates.Items;
        var pricesTable = databaseServer.GetTables().Templates.Prices;

        _baselineBullet = itemTable[configService.TheBlacklistAdvancedConfig.BaselineBulletId];

        UpdateRagfairConfig();
        UpdateGlobals();

        foreach (var handbookItem in databaseServer.GetTables().Templates.Handbook.Items)
        {
            logger.Info(handbookItem.Id);

            var item = itemTable[handbookItem.Id];

            //Todo: causes exception
            var originalPrice = pricesTable[item.Id];

            var customItemConfig = configService.TheBlacklistConfig.CustomItemConfigs.FirstOrDefault(conf =>
            conf switch
            {
                CustomItemConfig itemConfig => itemConfig.ItemId == item.Id,
                CustomParentConfig parentConfig => parentConfig.ParentId == item.Parent,
                _ => false
            });

            if (customItemConfig is not null && UpdateItemUsingCustomItemConfig(customItemConfig, item, pricesTable, originalPrice))
            {
                continue;
            }

            var itemProps = item.Properties;

            if(item.IsBulletOrShotgunShell())
            {
                //Todo: Implement
            }

            if(itemProps?.CanSellOnRagfair ?? false)
            {
                //Todo: Implement
            }
        }

        //Todo: Implement logs

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

    private bool UpdateItemUsingCustomItemConfig(CustomItemConfigBase customItemConfig, TemplateItem item, Dictionary<MongoId, double> prices, double originalPrice)
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
            prices[item.Id] = customItemConfig.FleaPriceOverride.Value;

            if(logger.IsDebug())
            {
                logger.Debug($"Updated {item.Id} - {item.Name} flea price from {originalPrice} to {prices[item.Id]} (price override).");
            }

            if (item.Properties?.CanSellOnRagfair ?? false)
            {
                _nonBlacklistedItemsUpdatedCount++;
            }

            return true;
        }

        return false;
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
