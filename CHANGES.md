# Source Code Changes — Location & Zone Name Translation

These changes add Russian translation support for zone and location names displayed in the WorldView.
All changes are in the `RemnantOverseer` project.

---

## 1. Add `LocationTranslations.cs`

Copy `LocationTranslations.cs` (from this repo) to:
```
RemnantOverseer/Utilities/LocationTranslations.cs
```

---

## 2. `Models/Location.cs`

After `public string Name { get; set; } = string.Empty;` add:

```csharp
public string? LocalizedName { get; set; }
public string DisplayName => string.IsNullOrEmpty(LocalizedName) ? Name : $"{Name} / {LocalizedName}";
```

---

## 3. `Models/Zone.cs`

After `public string Name { get; set; } = string.Empty;` add:

```csharp
public string? LocalizedName { get; set; }
public string DisplayName => string.IsNullOrEmpty(LocalizedName) ? Name : $"{Name} / {LocalizedName}";
```

---

## 4. `Utilities/DatasetMapper.cs`

Find (~line 106, zone mapping):
```csharp
Name = zone.Name,
```
Add after:
```csharp
LocalizedName = LocationTranslations.Get(zone.Name.Trim()),
```

Find (~line 119, location mapping):
```csharp
Name = location.Name,
```
Add after:
```csharp
LocalizedName = LocationTranslations.Get(location.Name.Trim()),
```

---

## 5. `Views/WorldView.axaml`

Replace **both** occurrences of:
```xml
Text="{Binding Name}"
```
(inside Zone and Location `DataTemplate`) with:
```xml
Text="{Binding DisplayName}"
```
