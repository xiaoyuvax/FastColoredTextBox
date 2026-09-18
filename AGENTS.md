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
- Test projects target older frameworks (.NET 8/9) and have WFO1000 errors on .NET 9+ (suppress with `-p:NoWarn=WFO1000`)
- Unit tests: `FastColoredTextBox.Tests/` (xUnit, no UI required) - run with `dotnet test FastColoredTextBox.Tests`

## Recent Optimizations (v2.17.0.207)
- Native Markdown syntax highlighting (Language.Markdown): fenced code blocks, headings, inline code, links/images, blockquotes, lists, HR
- Markdown regex fixes: fenced blocks now match across lines (Singleline), headings/inline code/blockquotes no longer span lines, images no longer double-styled as links
- Highlighting performance: RegexCompiledOption now always Compiled, regex cache in TextSelectionRange.GetRanges, precompiled auto-indent regexes
- Stability: GDI brush disposal for Markdown styles, Ruler Dispose (event unsubscribe), FileTextSource.SaveToFile failure-safe temp file handling, ReadExactly in EncodingDetector, UTF-7 BOM no longer returned
- Assembly version fixed: GenerateAssemblyInfo re-enabled (DLL version was 0.0.0.0)
- Repo hygiene: stale AnalysisReport.sarif / .NET Framework app.config removed

## NuGet Packaging
```bash
dotnet pack FastColoredTextBox/FastColoredTextBox.csproj -c Release -o ./nupkg
```
Package config in csproj: icon.png, README.md, license.txt included.

## Git/Release
- Version in csproj: `<Version>2.17.0.207</Version>`
- Update `PackageReleaseNotes` in csproj for releases
- Main branch: `master`