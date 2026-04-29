# Source Code Changes

Exact diffs for manual application. All paths are relative to the `RemnantOverseer` project root.

---

## 1. Add `Utilities/LocationTranslations.cs`

New file. Copy `LocationTranslations.cs` from this repo to:
```
RemnantOverseer/Utilities/LocationTranslations.cs
```

---

## 2. `Models/Location.cs`

Insert after `public string Name { get; set; } = string.Empty;`:

```csharp
public string? LocalizedName { get; set; }
public string DisplayName => string.IsNullOrEmpty(LocalizedName) ? Name : $"{Name} / {LocalizedName}";
```

---

## 3. `Models/Zone.cs`

Insert after `public string Name { get; set; } = string.Empty;`:

```csharp
public string? LocalizedName { get; set; }
public string DisplayName => string.IsNullOrEmpty(LocalizedName) ? Name : $"{Name} / {LocalizedName}";
```

---

## 4. `Utilities/DatasetMapper.cs`

~line 106 (zone mapping) — insert after `Name = zone.Name,`:
```csharp
LocalizedName = LocationTranslations.Get(zone.Name.Trim()),
```

~line 119 (location mapping) — insert after `Name = location.Name,`:
```csharp
LocalizedName = LocationTranslations.Get(location.Name.Trim()),
```

---

## 5. `Views/WorldView.axaml` — location/zone display

Replace both occurrences of:
```xml
Text="{Binding Name}"
```
(inside Zone and Location `DataTemplate`) with:
```xml
Text="{Binding DisplayName}"
```

---

## 6. `Views/WorldView.axaml` — item note button

~line 322 — replace:
```xml
<PathIcon Data="{StaticResource RoundInfoIcon}" IsVisible="{Binding Description, Converter={x:Static StringConverters.IsNotNullOrEmpty}}" ToolTip.Tip="{Binding Description}" ToolTip.Placement="LeftEdgeAlignedTop"/>
```
with:
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

---

## 7. `Views/WorldView.axaml.cs` — handler

Add to the `WorldView` class (file from this repo already includes this):
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

Required `using` directives (already present in `WorldView.axaml.cs` from this repo):
```csharp
using RemnantOverseer.Models;
using System.Diagnostics;
```
