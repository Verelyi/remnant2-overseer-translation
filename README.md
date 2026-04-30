# Remnant 2 Overseer — Translation (RU)

Russian translation patch for [RemnantOverseer](https://github.com/Angelore/remnant-two-overseer) covering item names, location/zone names, and item acquisition notes.

Based on [`lib.remnant2.analyzer`](https://github.com/AndrewSav/lib.remnant2.analyzer) **0.0.43** and RemnantOverseer as of the **November 2024** game update.

---

## Repository contents

### Part 1 — Item name patch

| File | Purpose |
|------|---------|
| `lib.remnant2.analyzer.db.json` | Patched database — each item `Name` field contains `"English / Russian"` |
| `en.Remnant2.json` | Official EN localization extracted from `Remnant2.locres` via FModel |
| `ru.Remnant2.json` | Official RU localization extracted from `Remnant2.locres` via FModel |

### Part 2 — Location/zone name translation

| File | Target path in project | Purpose |
|------|------------------------|---------|
| `LocationTranslations.cs` | `RemnantOverseer/Utilities/LocationTranslations.cs` | Static EN→RU dictionary for zone and location names (new file) |
| `Location.cs` | `RemnantOverseer/Models/Location.cs` | Adds `LocalizedName` and `DisplayName` properties |
| `Zone.cs` | `RemnantOverseer/Models/Zone.cs` | Adds `LocalizedName` and `DisplayName` properties |
| `DatasetMapper.cs` | `RemnantOverseer/Utilities/DatasetMapper.cs` | Calls `LocationTranslations.Get()` during zone/location mapping |
| `WorldView.axaml` | `RemnantOverseer/Views/WorldView.axaml` | Binds zone/location labels to `DisplayName`; info icon replaced with clickable button (see Part 3) |
| `CHANGES.md` | — | Exact diff for manual application |

### Part 3 — Info icon → Google Translate

| File | Target path in project | Purpose |
|------|------------------------|---------|
| `WorldView.axaml` | `RemnantOverseer/Views/WorldView.axaml` | `PathIcon` for item `Note` field replaced with a `Button` |
| `WorldView.axaml.cs` | `RemnantOverseer/Views/WorldView.axaml.cs` | `NoteTranslateButton_Click` handler |

---

## Part 1 — Item names (db.json patch)

`lib.remnant2.analyzer.db.json` is embedded as a managed resource inside `lib.remnant2.analyzer.dll`.

1. Extract `lib.remnant2.analyzer.dll` from `RemnantOverseer.exe` (single-file bundle — open with 7-zip, extract `49.lib.remnant2.analyzer.dll`, rename accordingly).
2. In [dnSpy](https://github.com/dnSpy/dnSpy): `lib.remnant2.analyzer` → `Resources` → `lib.remnant2.analyzer.db.json` → **Import from File** → **File → Save Module**.
3. Copy the patched DLL to the NuGet cache:
   ```
   %USERPROFILE%\.nuget\packages\lib.remnant2.analyzer\0.0.43\lib\net8.0\lib.remnant2.analyzer.dll
   ```
   > Version path (`0.0.43`) must match `PackageReference` in `RemnantOverseer.csproj`.
4. Rebuild:
   ```
   dotnet publish -c Release -r win-x64 --self-contained true
   ```

### ⚠️ Only `Name` is safe to translate

The following fields are used by `lib.remnant2.analyzer` for exact-match lookups against the save file. Translating them produces `"Sequence contains no matching element"` at runtime.

| Field | Role |
|-------|------|
| `DropReference` | LootGroup key |
| `EventLocation` | Location key |
| `SpawnReference` | SpawnEntry key |
| `ProfileId` | UE asset path |
| `World` | World identifier |
| `Type` | Item type |
| `DropType` | Drop type |
| `Id` | Unique item key |

Correct entry example:
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

## Part 2 — Location and zone names

Location/zone names are resolved from the save file at runtime and are absent from `db.json`. Translation is implemented via a static dictionary resolved at the source-code level.

Copy files from this repo to their target paths and rebuild. See `CHANGES.md` for the exact diff.

---

## Part 3 — Info icon → Google Translate

The `(i)` icon next to each item opens a tooltip showing the `Note` field (item acquisition hint). This modification replaces the static `PathIcon` with a `Button` that opens Google Translate (EN→RU) in the default browser with the note text pre-filled.

**`WorldView.axaml`** — see `CHANGES.md` § 6 for the exact replacement block.

**`WorldView.axaml.cs`** — adds the handler:
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

Required `using` (already present in the file from this repo):
```csharp
using RemnantOverseer.Models;
using System.Diagnostics;
```

---

## Translation source

Item names were matched using the official game localization files extracted from `pakchunk0-Windows.pak` via [FModel](https://fmodel.app/):

```
Remnant2/Content/Localization/Remnant2/{en,ru}/Remnant2.locres
```

Both files were exported as JSON (`en.Remnant2.json`, `ru.Remnant2.json`) and are included in this repo. Each entry shares a UUID key across languages.

Patching script workflow:
1. For each item in `db.json`, look up `Name` in `en.Remnant2.json`
2. Retrieve the matching RU string from `ru.Remnant2.json` by UUID key
3. Write result back to `db.json` as `"English / Russian"`

Items with no match in the locres files (custom/handwritten entries) retain the original English name unchanged.
