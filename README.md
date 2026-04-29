# Remnant 2 Overseer — Translation (RU)

Translation of item names and location names for [RemnantOverseer](https://github.com/Angelore/remnant-two-overseer) into Russian, using the official game localization file as the source.

Based on [`lib.remnant2.analyzer`](https://github.com/AndrewSav/lib.remnant2.analyzer) version **0.0.43** and RemnantOverseer as of **November 2024** (last game update).

---

## What's included

### Part 1 — Item name patch

| File | Purpose |
|------|---------|
| `lib.remnant2.analyzer.db.json` | Patched database — item `Name` fields contain `"English / Russian"` |

### Part 2 — Location/zone name translation (source code)

| File | Original path in project | Purpose |
|------|--------------------------|---------|
| `LocationTranslations.cs` | `RemnantOverseer/Utilities/LocationTranslations.cs` | Static EN→RU dictionary for zone and location names (new file) |
| `Location.cs` | `RemnantOverseer/Models/Location.cs` | Added `LocalizedName` and `DisplayName` properties |
| `Zone.cs` | `RemnantOverseer/Models/Zone.cs` | Added `LocalizedName` and `DisplayName` properties |
| `DatasetMapper.cs` | `RemnantOverseer/Utilities/DatasetMapper.cs` | Added `LocationTranslations.Get()` calls for zone and location mapping |
| `WorldView.axaml` | `RemnantOverseer/Views/WorldView.axaml` | Replaced `{Binding Name}` with `{Binding DisplayName}` in zone and location templates; info icon made clickable (see Part 3) |
| `CHANGES.md` | — | Describes the exact source code changes (for reference) |

### Part 3 — Clickable info icon with Google Translate

| File | Original path in project | Purpose |
|------|--------------------------|---------|
| `WorldView.axaml` | `RemnantOverseer/Views/WorldView.axaml` | `PathIcon` for item notes replaced with a clickable `Button` |
| `WorldView.axaml.cs` | `RemnantOverseer/Views/WorldView.axaml.cs` | Added `NoteTranslateButton_Click` handler |

---

## Part 1 — Item names (db.json patch)

`lib.remnant2.analyzer.db.json` is embedded as a managed resource inside `lib.remnant2.analyzer.dll`. The patching workflow:

1. **Extract the DLL** from `RemnantOverseer.exe` (it's a single-file publish bundle — use 7-zip), locate `49.lib.remnant2.analyzer.dll`, rename to `lib.remnant2.analyzer.dll`.
2. **Patch the resource** in [dnSpy](https://github.com/dnSpy/dnSpy): `lib.remnant2.analyzer` → `Resources` → right-click `lib.remnant2.analyzer.db.json` → **Import from File** → **File → Save Module**.
3. **Drop the patched DLL** into the NuGet cache so `dotnet publish` picks it up instead of the feed version:
   ```
   %USERPROFILE%\.nuget\packages\lib.remnant2.analyzer\0.0.43\lib\net8.0\lib.remnant2.analyzer.dll
   ```
4. **Rebuild:**
   ```
   dotnet publish -c Release -r win-x64 --self-contained true
   ```

> **Note:** the version path (`0.0.43`) must match the `PackageReference` in `RemnantOverseer.csproj`. Verify before copying.

### ⚠️ Critical: only `Name` is safe to translate

Several fields are used by [`lib.remnant2.analyzer`](https://github.com/AndrewSav/lib.remnant2.analyzer) for exact-match lookups against the save file. Translating them breaks matching and throws `"Sequence contains no matching element"` at runtime.

| Field | Used for |
|-------|----------|
| `DropReference` | LootGroup lookup |
| `EventLocation` | Location lookup |
| `SpawnReference` | SpawnEntry lookup |
| `ProfileId` | UE asset path |
| `World` | World identifier |
| `Type` | Item type classifier |
| `DropType` | Drop type classifier |
| `Id` | Unique item key |

Example of a correctly translated entry:
```json
{
  "Id": "Ring_CrimsonDreamstone",
  "Name": "Crimson Dreamstone / Багровый камень сновидицы",
  "ProfileId": "/Game/World_DLC1/Items/Trinkets/Rings/CrimsonDreamstone/Ring_CrimsonDreamstone.Ring_CrimsonDreamstone_C",
  "Type": "ring",
  "World": "World_DLC1",
  "DropType": "Event",
  "DropReference": "Quest_Injectable_BurningCity_DLC"
}
```

---

## Part 2 — Location and zone names (source code)

Location/zone names are read from the save file at runtime and are not present in `db.json`, so they require a separate approach: a static translation dictionary resolved at the source code level.

Copy the modified files from this repo into their respective paths in the project, then rebuild. See `CHANGES.md` for the exact line-by-line diff if you prefer to apply changes manually.

---

## Part 3 — Clickable info icon (Google Translate)

The `(i)` icon next to each item name shows a tooltip with the `Note` field — a handwritten hint by the original author about how to obtain the item. By default the icon is not interactive.

This modification replaces the `PathIcon` with a `Button` that opens Google Translate (EN→RU) in the default browser with the note text pre-filled.

**`WorldView.axaml`** — find (~line 322):
```xml
<PathIcon Data="{StaticResource RoundInfoIcon}" IsVisible="{Binding Description, Converter={x:Static StringConverters.IsNotNullOrEmpty}}" ToolTip.Tip="{Binding Description}" ToolTip.Placement="LeftEdgeAlignedTop"/>
```
Replace with:
```xml
<Button Classes="plain hint"
        IsVisible="{Binding Description, Converter={x:Static StringConverters.IsNotNullOrEmpty}}"
        ToolTip.Placement="LeftEdgeAlignedTop"
        Click="NoteTranslateButton_Click"
        Padding="0" Margin="5 0">
  <PathIcon Data="{StaticResource RoundInfoIcon}"/>
  <ToolTip.Tip>
    <StackPanel Orientation="Vertical" MaxWidth="350">
      <TextBlock Text="{Binding Description}" TextWrapping="Wrap"/>
      <TextBlock FontSize="11" Opacity="0.6" Margin="0 6 0 0" TextDecorations="{x:Null}">🌐 Click to translate in Google</TextBlock>
    </StackPanel>
  </ToolTip.Tip>
</Button>
```

**`WorldView.axaml.cs`** — add the handler to the `WorldView` class:
```csharp
private void NoteTranslateButton_Click(object? sender, RoutedEventArgs e)
{
    if (sender is Button btn && btn.DataContext is Item item && !string.IsNullOrEmpty(item.Description))
    {
        var text = Uri.EscapeDataString(item.Description);
        var url = $"https://translate.google.com/?sl=en&tl=ru&text={text}&op=translate";
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
}
```

Required `using` directives (already present in the file from this repo):
```csharp
using RemnantOverseer.Models;
using System.Diagnostics;
```

---

## Translation source

Item and location names are translated using the official Russian localization file extracted from the game via [FModel](https://fmodel.app/):

```
Remnant2\Remnant2\Content\Paks\pakchunk0-Windows.pak
  └── Remnant2/Content/Localization/Remnant2/ru/Remnant2.locres
```

Location names have some manual corrections for accuracy where the official translation was unclear.
