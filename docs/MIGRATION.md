# Migration Plan: DCExtractor -> DCExtractorX

This document records the decision and phased plan behind this solution, for reference by
anyone (human or model) continuing the work.

## Decision

The original [DCExtractor](../../DCExtractor) is a WinForms-only, untested, .NET Framework
4.6.1 tool. Rather than rewriting it from scratch, **the proven parser/exporter/type code was
migrated verbatim into a new, GUI-free `DCExtractorX.Core` library**, targeting the newest
.NET (net10.0-windows), with only the *shell* (hosting, logging, settings, tests) rewritten.

Rationale: the reverse-engineered format knowledge in `DC.IO`/`DC.Types`/`Custom.Data`
(PS2 swizzle math, TIM2 palette unpacking, MDS/MOT/BBP/WGT binary layouts, and the
HACKs documented in `docs/known-issues.md`) is the hard-to-reproduce, valuable part of the
project. A from-scratch rewrite would have discarded that and required re-discovering the
same edge cases from real game data.

Constraints agreed with the project owner:
- Windows-only is acceptable (keep `System.Drawing` for PNG export; WinForms GUI optional).
- Newest .NET (net10.0-windows at time of writing).
- Hand-rolled CLI argument parsing (no external CLI framework dependency).
- xUnit for tests.

## What changed vs. the original repo

| Area | Change |
|---|---|
| Target framework | `.NET Framework 4.6.1` → `net10.0-windows` |
| GUI coupling in Core | `MessageBox.Show(...)` calls replaced with `Custom.Diagnostics.ILog` (`Log.Current`), defaulting to a silent `NullLog` |
| Progress reporting | `DCProgress` decoupled from the WinForms-thread-bound `DCBackgroundWorker`; now raises plain `Action<T>` events + a `Func<bool> CancelRequested` callback. Property names (`value`/`maximum`/`name`/`canceled`) are unchanged, so parser call sites needed no edits |
| Settings | The file-persisted, WinForms-driven `DCExtractor.Settings` was replaced with an in-memory `DC.Settings` holding the same 4 properties/defaults. Persistence/UI is a host concern (CLI/GUI), not Core's |
| `System.Drawing` | Added the `System.Drawing.Common` NuGet package (no longer part of the base framework on modern .NET) |
| `Index` ambiguity | `System.Index` (added in modern .NET/C#) collides with `DC.Types.Index`; resolved via a `using Index = DC.Types.Index;` alias in the two affected files |
| Everything else in `DC.*`/`Custom.*` | Copied verbatim, including all documented HACKs (see `docs/known-issues.md`) |

Not migrated (left for a later, explicitly reviewed phase):
- `FileHelpers.MoveFiles`/`DeleteFiles` and the "Extract and Convert" one-click operation
  (both depend on the WinForms `FileConflictDialog` for interactive conflict resolution).
- The WinForms `SettingsForm`/`SettingField` UI and `settings.txt` persistence.
- The `#if DEBUG` file/directory comparison and MDS/WGT metadata dump tools.

## Phased plan

- [x] **Phase 0** — Scaffold `DCExtractorX.sln`(x) + `Core`/`Cli`/`Gui`/`Tests` projects,
  `.editorconfig`, `.gitignore`, `README.md`, `CONTRIBUTING.md`, copied `Specifications/`.
- [x] **Phase 1** — Migrate `Core`: Data/Math/IO helpers, `ILog` seam, in-memory `Settings`,
  decoupled `DCProgress`, all DAT/PAK/IMG/TM2/MDS/BBP/MOT/WGT readers and
  OBJ/SMD/PNG exporters, `DC.Types`. Builds clean on `net10.0-windows`.
- [x] **Phase 2** — `DCExtractorX.Cli`: hand-rolled subcommands mapping the original 12
  operations (`extract-dat`, `extract-pak[-dir]`, `mds2obj[-dir]`, `mds2smd[-dir]`,
  `img2png[-dir]`, `tm22png[-dir]`). `extract-and-convert` intentionally **not** ported yet
  (see "Not migrated" above).
- [x] **Phase 3** — `DCExtractorX.Tests` (xUnit): unit tests for deterministic Core logic
  (`Swizzle`, `ColorHelpers`, `Matrix4x4`, `MathHelpers`, `PathHelpers`, `FileHelpers`,
  `DCProgress`). 36/36 passing. **Not yet covered:** end-to-end golden-file tests against
  real `.dat/.pak/.mds/.img/.tm2` samples, since those require assets from an owned game
  disc that aren't present in this environment.
- [ ] **Phase 4** — Output folder organization + validation/summary reports; batch mode that
  aggregates failures instead of interrupting per-file.
- [ ] **Phase 5** — `DCExtractorX.Gui`: port the WinForms shell (menus, dialogs, progress bar,
  Settings UI with persistence) over `DCExtractorX.Core`, wiring `Log.Current` to a
  MessageBox-based sink and `DCProgress` events to a real progress bar. Port
  `FileHelpers.MoveFiles`/`FileConflictDialog` and the "Extract and Convert" operation.
- [ ] **Phase 6** — Reliability fixes, each behind its own test: index-bounds/primitive-type
  handling, non-power-of-2 textures, alpha handling, explicit Dark Cloud 1 vs 2 detection,
  and a deliberate decision on the `Matrix4x4.Zero()` bug found during Phase 3.
- [ ] **Phase 7** — Animation status write-up + any experimental bind-pose fix attempt,
  gated behind an explicit opt-in flag, default off, validated by tests.

## Verification performed so far

- `dotnet build DCExtractorX.slnx` → succeeds, 0 errors (Core/Cli/Gui/Tests).
- `dotnet test DCExtractorX.Tests/DCExtractorX.Tests.csproj` → 36/36 passing.
- `dotnet run --project DCExtractorX.Cli -- --help` → prints usage as expected.
- End-to-end extraction/conversion against real game files has **not** been verified in this
  environment (no game assets available); this should be the first thing verified by
  whoever has the source discs, before relying on this build.
