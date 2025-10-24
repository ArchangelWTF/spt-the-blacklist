using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Utils;
using TheBlacklist.Models;

namespace TheBlacklist.Services;

[Injectable(InjectionType.Singleton)]
public class ConfigService(ModHelper modHelper, JsonUtil jsonUtil)
{
    public Config TheBlacklistConfig { get; private set; } = default!;
    public AdvancedConfig TheBlacklistAdvancedConfig { get; private set; } = default!;

    public string GetModPath()
    {
        return modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
    }

    public string GetConfigPath()
    {
        return Path.Combine(GetModPath(), "Config", "config.jsonc");
    }

    public string GetAdvancedConfigPath()
    {
        return Path.Combine(GetModPath(), "Config", "advancedConfig.jsonc");
    }

    public async Task LoadAsync()
    {
        await LoadConfigAsync();
        await LoadAdvancedConfigAsync();
    }

    private async Task LoadConfigAsync()
    {
        Config? loadedConfig = await jsonUtil.DeserializeFromFileAsync<Config>(GetConfigPath());

        if (loadedConfig is not null)
        {
            TheBlacklistConfig = loadedConfig;
        }
        else
        {
            throw new Exception("Could not deserialize config.jsonc!");
        }
    }

    private async Task LoadAdvancedConfigAsync()
    {
        AdvancedConfig? loadedConfig = await jsonUtil.DeserializeFromFileAsync<AdvancedConfig>(GetAdvancedConfigPath());

        if (loadedConfig is not null)
        {
            TheBlacklistAdvancedConfig = loadedConfig;
        }
        else
        {
            throw new Exception("Could not deserialize advancedConfig.jsonc!");
        }
    }
}
