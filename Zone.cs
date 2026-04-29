using System.Collections.Generic;

namespace RemnantOverseer.Models;
public class Zone
{
    public string Name { get; set; } = string.Empty;
    public string? LocalizedName { get; set; }
    public string DisplayName => string.IsNullOrEmpty(LocalizedName) ? Name : $"{Name} / {LocalizedName}";
    public string Story { get; set; } = string.Empty;
    public List<Location> Locations { get; set; } = [];

    public Zone ShallowCopy()
    {
        return (Zone)MemberwiseClone();
    }
}
