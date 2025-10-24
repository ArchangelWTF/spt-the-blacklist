using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace TheBlacklist;

public static class ExtensionMethods
{
    public static bool IsBulletOrShotgunShell(this TemplateItem item)
    {
        var itemProperties = item.Properties;

        if (itemProperties is null)
        {
            return false;
        }

        return itemProperties.AmmoType == "bullet" || itemProperties.AmmoType == "buckshot";
    }
}
