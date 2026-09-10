# FastColoredTextBox - Agent Instructions

## Project Structure
- **FastColoredTextBox/** - Main library (C# WinForms UserControl)
- **Tester/** - C# demo/test application
- **TesterVB/** - VB.NET demo/test application
- Solution: `FastColoredTextBox.sln`

## Build Commands
```bash
# Build main library only (recommended)
dotnet build FastColoredTextBox/FastColoredTextBox.csproj -c Release

# Build full solution (test projects have errors on .NET 9+)
dotnet build FastColoredTextBox.sln -c Release
```

## Key Project Settings (FastColoredTextBox.csproj)
- **Target Frameworks**: net7.0-windows7.0;net8.0-windows7.0;net9.0-windows7.0;net10.0-windows7.0
- **LangVersion**: 13
- **AllowUnsafeBlocks**: true
- **Signed Assembly**: Yes (FCTB_key.snk)
- **Package ID**: Vax-FCTB (NuGet)
- **Output**: Post-build copies to `Binary/` folder

## Development Notes
- Main control: `FastColoredTextBox.cs` (~314 KB)
- Key components: AutocompleteMenu, DocumentMap, Ruler, Find/Replace/GoTo forms
- CJK/WordWrap improvements in recent versions
- Test projects target older frameworks (.NET 8/9) and have WFO1000 errors on .NET 9+
- No formal test suite - manual testing via Tester apps

## Recent Optimizations (v2.17.0.206)
- Horizontal scroll text overlapping line number area fully fixed (CJK re-evaluation, non-uniform width support)
- Default Padding 2px (matches WinForms TextBox)
- Localization support (EN/ZH) for Find/Replace/GoTo/Hotkeys forms
- Horizontal scroll text overlapping line numbers fixed
- Undo/Redo core logic refactored (position accuracy, cross-line support)
- Column selection mode IndexOutOfRangeException fixed
- Label z-order fixed (no longer overlaps controls)
- Chinese labels shortened to match English width
- Redo shortcut changed to Ctrl+Y (standard Windows convention)
- .NET 10.0 support added

## NuGet Packaging
```bash
dotnet pack FastColoredTextBox/FastColoredTextBox.csproj -c Release -o ./nupkg
```
Package config in csproj: icon.png, README.md, license.txt included.

## Git/Release
- Version in csproj: `<Version>2.17.0.206</Version>`
- Update `PackageReleaseNotes` in csproj for releases
- Main branch: `master`