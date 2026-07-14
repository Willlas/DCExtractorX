# Known Issues & Intentional Quirks

This document consolidates the known limitations, HACKs, and quirks that were carried over
verbatim from the original DCExtractor during the migration to DCExtractorX. **None of these
are fixed by the migration** — the goal of the migration was to modernize the *shell*
(build, hosting, logging, tests) without silently changing parsing/export *behavior*.
Any fix to these should be a separate, reviewed, test-covered change (see `CONTRIBUTING.md`).

## Animation export (broken)

- `DCExtractorX.Core/Source/DC/IO/Input/MDS/BBP.cs` — bind-pose rotations are documented by
  the original author as producing incorrect results ("fishy"), despite extensive testing.
- `DCExtractorX.Core/Source/DC/IO/Output/SMD.cs` — animation rotation output is wrong despite
  verified quaternion math; root cause suspected to be the BBP bind pose data.
- `DCExtractorX.Core/Source/DC/IO/Output/SMD.cs` — HACK: breaks out of a channel's keyframe
  loop early when Dark Cloud 1 files have variable keyframe counts across channels for the
  same bone (Dark Cloud 2 aligns them; Dark Cloud 1 does not).
- **Status:** exported animation data should be treated as unreliable. `--anim` in the CLI
  is off by default and prints a warning in `--help`.

## Texture conversion

- `DCExtractorX.Core/Source/Data/Swizzle.cs` (`UnSwizzle`) — a small number of Dark Cloud 2
  images are not powers of two (e.g. a reported 256x255 image). The unswizzle algorithm
  gracefully bails out instead of throwing `IndexOutOfRangeException`, at the cost of losing
  the final row of pixel data. Locked in by `SwizzleTests.UnSwizzle_NonPowerOfTwoDimensions_DoesNotThrow`.
- `DCExtractorX.Core/Source/Data/ColorHelpers.cs` (`FromBufferRGBA32Bit`) — alpha values in
  32-bit textures are doubled (`× 2`, clamped to 255) to compensate for what appears to be a
  half-intensity alpha encoding used across virtually all Dark Cloud 2 32-bit textures. Locked
  in by `ColorHelpersTests.FromBufferRGBA32Bit_AppliesAlphaTimesTwoHack`.
- `DCExtractorX.Core/Source/DC/IO/Input/IMG/TM2/TIM2Picture.cs` — palettes may be linear or
  interleaved (`CLUTFormat & 0x80`); interleaved palettes are unpacked with a block-based
  algorithm.

## Model conversion

- `DCExtractorX.Core/Source/DC/IO/Output/WavefrontOBJ.cs` — only `GL_TRIANGLES`,
  `GL_TRIANGLE_STRIP`, and `DC2_COLLISION_TRIANGLES` primitive styles are supported;
  `GL_TRIANGLE_FAN`, `GL_QUADS`, `GL_QUAD_STRIP`, and `GL_POLYGON` are not implemented.
- `DCExtractorX.Core/Source/DC/IO/Output/SMD.cs` and `.../MDS/MDS.cs` — index-bounds capping
  HACK: some models have index data outside the bounds of the normal/UV arrays; these are
  capped/skipped rather than allowed to throw.
- `DCExtractorX.Core/Source/DC/IO/Output/SMD.cs` — the SMD format has no scale channel, so
  scale data present in `.mds` files is not preserved on export.

## Archive/pack extraction

- `DCExtractorX.Core/Source/DC/IO/Input/PAK/PAK.cs` — a single known file in Dark Cloud 2
  (`s55`) has a double header, causing it to be written without an extension; if a
  same-named directory already exists at that path, extraction of that one file is skipped
  with a warning (via `Custom.Diagnostics.Log.Current.Warn`) rather than crashing.

## Migration-only findings (new)

- `DCExtractorX.Core/Source/Math/Matrix4x4.cs` (`Zero()`) — **pre-existing bug**, confirmed
  by code and locked in by `Matrix4x4Tests.Zero_OnlyClearsFirstElement_PreExistingBug`: the
  loop body is `m[0] = 0.0f;` instead of `m[i] = 0.0f;`, so only the first element is
  actually cleared. `Zero()`/`isZero` do not appear to be used on any load/export hot path
  today, so this was **not fixed** during migration; flagged here for a deliberate, reviewed
  decision in a later phase.
- `System.Index` (introduced in modern .NET/C#) collides with the format's own
  `DC.Types.Index` struct now that the project targets `net10.0-windows`. Resolved via a
  `using Index = DC.Types.Index;` alias in `MDS.cs` and `WavefrontOBJ.cs` — no behavior
  change, purely a naming disambiguation required by the newer compiler/BCL.
