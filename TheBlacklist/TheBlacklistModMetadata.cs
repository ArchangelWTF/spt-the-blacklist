using SPTarkov.Server.Core.Models.Spt.Mod;
using Range = SemanticVersioning.Range;
using Version = SemanticVersioning.Version;

namespace TheBlacklist;

public record TheBlacklistModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "com.platinum.theblacklist";
    public override string Name { get; init; } = "The Blacklist";
    public override string Author { get; init; } = "Platinum";
    public override List<string>? Contributors { get; init; } = [""];
    public override Version Version { get; init; } = new("3.0.1");
    public override Range SptVersion { get; init; } = new("~4.0");
    public override List<string>? Incompatibilities { get; init; } = [];
    public override Dictionary<string, Range>? ModDependencies { get; init; } = [];
    public override string? Url { get; init; } = "https://github.com/ArchangelWTF/spt-the-blacklist";
    public override bool? IsBundleMod { get; init; } = false;
    public override string License { get; init; } = "GNU GPLv3";
}
