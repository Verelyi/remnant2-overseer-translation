# Remnant 2 Overseer — Translation

Translation of item names and location names for [RemnantOverseer](https://github.com/Angelore/remnant-two-overseer) into Russian, using the official `ru.Remnant2.json` game localization file as the source.

Based on `lib.remnant2.analyzer` version **0.0.43** and RemnantOverseer as of **November 2024** (last game update).

---

## What's included

| File | Purpose |
|------|---------|
| `lib.remnant2.analyzer.db.json` | Patched database — item `Name` fields contain `"English / Russian"` |
| `LocationTranslations.cs` | Static EN→RU dictionary for zone and location names |
| `CHANGES.md` | Source code changes needed in RemnantOverseer to display translated location names |

---

## Part 1 — Item names (db.json patch)

The item database is embedded as a resource inside `lib.remnant2.analyzer.dll`. To apply the translation:

1. Open `RemnantOverseer.exe` with 7-zip and extract `49.lib.remnant2.analyzer.dll` → rename to `lib.remnant2.analyzer.dll`
2. Open it in [dnSpy](https://github.com/dnSpy/dnSpy)
3. Expand `lib.remnant2.analyzer` → `Resources` → `lib.remnant2.analyzer.db.json`
4. Right-click → **Import from File** → select `lib.remnant2.analyzer.db.json` from this repo
5. **File → Save Module** → save as patched DLL
6. Copy the patched DLL into NuGet cache:
   ```
   %USERPROFILE%\.nuget\packages\lib.remnant2.analyzer\0.0.43\lib\net8.0\
   ```
7. Rebuild RemnantOverseer: `dotnet publish -c Release -r win-x64 --self-contained true`

### ⚠️ Critical rule: only translate the `Name` field

`lib.remnant2.analyzer` uses several other fields for **internal matching** against the save file. Translating them causes a crash (`"Sequence contains no matching element"`).

| Field | Must stay in English |
|-------|---------------------|
| `DropReference` | Used to look up LootGroup by exact match |
| `EventLocation` | Used to look up Location by exact match |
| `SpawnReference` | Used to look up SpawnEntry by exact match |
| `ProfileId` | In-game asset path |
| `World` | World identifier (`World_DLC1`, `World_Fae`, etc.) |
| `Type` | Item type (`ring`, `amulet`, `weapon`, etc.) |
| `DropType` | Drop type (`Event`, `Location`, `Crafting`, etc.) |
| `Id` | Unique item identifier |

**Only `Name` is safe to translate.**

Example:
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

Location/zone names come from the save file at runtime, not from `db.json`, so they need a separate approach: a static translation dictionary applied at the source code level.

See `LocationTranslations.cs` and `CHANGES.md` for the full implementation.

---

## Translation source

Item names are translated using the official **`ru.Remnant2.json`** localization file included with the Russian version of the game. Location names are translated from the same source with some manual corrections for accuracy.
