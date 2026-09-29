# FastColoredTextBox - Agent Instructions

## Build / Test / Pack

```bash
# Main library only (recommended)
dotnet build FastColoredTextBox/FastColoredTextBox.csproj -c Release

# Full solution
dotnet build FastColoredTextBox.sln -c Release

# Unit tests (xUnit, no UI required)
dotnet test FastColoredTextBox.Tests

# NuGet package
dotnet pack FastColoredTextBox/FastColoredTextBox.csproj -c Release -o ./nupkg
```

## Hard constraints / verified pitfalls

- Tester/TesterVB (the demo projects in the full solution) fail on .NET 9+ with `WFO1000`; build the library alone, or pass `-p:NoWarn=WFO1000` when the full solution is required.
- `FastColoredTextBox.Tests` disables test parallelization: `TextSource.CurrentTB` is static.
- Version lives only in `FastColoredTextBox/FastColoredTextBox.csproj`; keep `PackageReleaseNotes` updated for releases.
- CI (`.github/workflows/ci.yml`) builds all TFMs and runs tests on push/PR (windows-latest).
