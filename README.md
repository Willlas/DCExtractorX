# DCExtractorX

A preservation/reverse-engineering toolkit for unpacking and converting assets from
PS2-era Level-5 games, especially **Dark Cloud 1** and **Dark Cloud 2**.

This is a modernized migration of the original [DCExtractor](../DCExtractor) (WinForms,
.NET Framework 4.6.1) onto the newest .NET, with the proven format-parsing code carried
over and a headless CLI added alongside the (optional) GUI. See
[`docs/MIGRATION.md`](docs/MIGRATION.md) for the full rationale and phased plan, and
[`docs/known-issues.md`](docs/known-issues.md) for known limitations/HACKs preserved from
the original tool.

> **This tool assumes you own the original game disc.** It does not include, link to, or
> assist with piracy workflows, ISO downloads, BIOS redistribution, or copyrighted asset
> distribution. It is intended for private preservation, documentation, interoperability,
> and modding research only.

## Supported formats

| Category | Formats | Notes |
|---|---|---|
| Root archives | `.DAT` + `.HD2`/`.HD3` | `.HD2` = Dark Cloud 1, `.HD3` = Dark Cloud 2 |
| Pack/container | `.PAK`, `.CHR`, `.EFP`, `.IPK`, `.MPK`, `.PCP`, `.SKY`, `.SND` | All share one generic parser |
| Models | `.MDS` (+ sibling `.BBP`, `.WGT`, `.MOT`) | Auto-loaded when present next to the `.MDS` |
| Images | `.IMG`, `.TM2` | IM2 magic = Dark Cloud 1, IM3 magic = Dark Cloud 2 |

## Conversions

- `.MDS` → `.OBJ` (Wavefront)
- `.MDS` → `.SMD` (Studiomdl: skeleton, weights, bind pose)
- `.IMG` / `.TM2` → `.PNG`

**Known limitations** (unchanged from the original tool — see `docs/known-issues.md` for
full detail): not a repacker; some skybox/effect/UI models may fail to convert; `.MOT`
animation export is present but produces incorrect bind-pose rotations; SMD export has no
scale channel.

## Building

Requires the **.NET 10 SDK**.

```powershell
dotnet build DCExtractorX.slnx
dotnet test DCExtractorX.Tests/DCExtractorX.Tests.csproj
```

## Usage (CLI)

```powershell
dotnet run --project DCExtractorX.Cli -- --help

dotnet run --project DCExtractorX.Cli -- extract-dat DATA.DAT DATA.HD2 .\out
dotnet run --project DCExtractorX.Cli -- extract-pak-dir .\out
dotnet run --project DCExtractorX.Cli -- mds2smd-dir .\out --split
dotnet run --project DCExtractorX.Cli -- img2png-dir .\out
```

## Project layout

- `DCExtractorX.Core` — format parsers/exporters/types (`DC.IO`, `DC.Types`, `Custom.*`).
  No UI framework dependency.
- `DCExtractorX.Cli` — headless command-line front-end over Core.
- `DCExtractorX.Gui` — optional WinForms front-end over Core (Windows-only; not yet
  ported from the original — see `docs/MIGRATION.md` Phase 5).
- `DCExtractorX.Tests` — xUnit tests.
- `Specifications/` — reverse-engineered format documentation (reference only).

See [`CONTRIBUTING.md`](CONTRIBUTING.md) before making changes, especially around the
"minimal-diff" and "don't silently fix documented HACKs" conventions.

## License

GPL-3.0, consistent with the original DCExtractor.
