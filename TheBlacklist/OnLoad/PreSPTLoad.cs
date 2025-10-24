using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using TheBlacklist.Services;

namespace TheBlacklist.OnLoad;

[Injectable(TypePriority = OnLoadOrder.PreSptModLoader + TheBlacklistLoadPriority.TheBlacklistPriorityOffset)]
public class PreSPTLoad(ConfigService configService) : IOnLoad
{
    public async Task OnLoad()
    {
        await configService.LoadAsync();
    }
}
