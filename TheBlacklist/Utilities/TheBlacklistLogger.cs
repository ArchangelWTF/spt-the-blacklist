using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;
using TheBlacklist.Services;

namespace TheBlacklist.Utilities;

[Injectable]
public class TheBlacklistLogger(ISptLogger<TheBlacklistLogger> logger, ConfigService configService)
{
    public void Success(string data, Exception? ex = null)
    {
        logger.Success($"[The Blacklist] {data}", ex);
    }

    public void Error(string data, Exception? ex = null)
    {
        logger.Error($"[The Blacklist] {data}", ex);
    }

    public void Warning(string data, Exception? ex = null)
    {
        logger.Warning($"[The Blacklist] {data}", ex);
    }

    public void Info(string data, Exception? ex = null)
    {
        logger.Info($"[The Blacklist] {data}", ex);
    }

    public void Debug(string data, Exception? ex = null)
    {
        logger.Debug($"[The Blacklist] {data}", ex);
    }

    public void Critical(string data, Exception? ex = null)
    {
        logger.Critical($"[The Blacklist] {data}", ex);
    }

    public bool IsDebug()
    {
        if (ProgramStatics.DEBUG() || configService.TheBlacklistAdvancedConfig.EnableDebug)
        {
            return true;
        }

        return false;
    }
}
