using RemnantOverseer.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace RemnantOverseer.Models;
public class Location
{
    public string Name { get; set; } = string.Empty;
    public string? LocalizedName { get; set; }
    public string DisplayName => string.IsNullOrEmpty(LocalizedName) ? Name : $"{Name} / {LocalizedName}";
    public List<Item> Items { get; set; } = [];
    public bool IsTraitBookPresent { get; set; }
    public bool IsSimulacrumPresent { get; set; }
    public bool IsTraitBookLooted { get; set; }
    public bool IsSimulacrumLooted { get; set; }
    public bool IsBloodmoon { get; set; }

    public bool IsRespawnLocation { get; set; }
    public RespawnPointType RespawnPointType { get; set; }
    public string RespawnPointName { get; set; } = string.Empty;
    public string? FormattedRespawnPointName
    {
        get
        {
            if (!IsRespawnLocation) return null;
            return RespawnPointType switch
            {
                // Extra spaces are a temporary workaround to https://github.com/AvaloniaUI/Avalonia/issues/17862, remove when fixed
                // It's not fixed yet, but I moved this text out of the tooltip. Removing spaces, keeping comment
                RespawnPointType.WorldStone => $"World Stone: {RespawnPointName}",
                RespawnPointType.Checkpoint => $"Checkpoint: {RespawnPointName}",
                RespawnPointType.ZoneTransition => GetFormattedZoneTransition(),
                _ => null
            };
        }
    }

    private string GetFormattedZoneTransition()
    {
        var split = RespawnPointName.Split('/');
        return $"Transition between {split[0]} and {split[1]}";
    }

    public bool IsGenesisLocation => Name.Equals("Withered Necropolis");
    public bool IsWard13Location => Name.Equals("Ward 13");

    // Trying this out. Should not be a big performance hit since it's just ~10 calls
    private string[] _possibleOracleSpawns = ["Morrow Parish", "Forsaken Quarter", "Ironborough", "Brocwithe Quarter"];
    public bool IsOracleLocation => _possibleOracleSpawns.Contains(Name) && Items.Any(i => i.OriginName.Equals("Oracle's Refuge", System.StringComparison.Ordinal));


    public Location ShallowCopy()
    {
        return (Location)MemberwiseClone();
    }
}
