# Contributing to DCExtractorX

DCExtractorX is a preservation/reverse-engineering tool for PS2-era Level-5 game assets
(Dark Cloud 1/2). It assumes you own the original game disc; this project does not
distribute, and will not accept contributions related to, copyrighted game assets, ISOs,
BIOS files, or piracy workflows of any kind.

## Scope

This is a migration/modernization of the original [DCExtractor](https://github.com/) WinForms
tool into a modern, testable, headless-capable solution. See `docs/MIGRATION.md` for the
rationale and phased plan.

Guiding principles for changes:
- **Minimal-diff.** Prefer the smallest change that accomplishes the goal.
- **The parser/exporter logic in `DCExtractorX.Core` is the valuable, hard-won asset.**
  Do not "clean up" or restructure format-parsing code without a specific, reviewed reason.
  Known quirks/HACKs are intentionally preserved and documented in `docs/known-issues.md`;
  do not silently "fix" them in an unrelated change.
- **Tests before behavior changes.** If you're changing parsing/export behavior (not just
  adding a new host or diagnostics), add a test in `DCExtractorX.Tests` that captures the
  behavior before and after your change.
- **No GUI framework dependency in Core.** `DCExtractorX.Core` must not reference
  `System.Windows.Forms`. Diagnostics go through `Custom.Diagnostics.ILog`
  (`Custom.Diagnostics.Log.Current`), not `MessageBox`.

## Building

Requires the .NET 10 SDK.

```powershell
dotnet build DCExtractorX.slnx
dotnet test DCExtractorX.Tests/DCExtractorX.Tests.csproj
```

## Project layout

- `DCExtractorX.Core` - format parsers/exporters/types, no UI dependency.
- `DCExtractorX.Cli` - headless command-line front-end over Core.
- `DCExtractorX.Gui` - optional WinForms front-end over Core (Windows-only).
- `DCExtractorX.Tests` - xUnit tests.
- `Specifications/` - reverse-engineered format documentation (reference only).
