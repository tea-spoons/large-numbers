# Change plan

> Draft. This file tracks what changed on the way to this repo and what I plan to change next. Edit freely.

## Origin

Originally developed at Bigpoint. Published here with Bigpoint's permission for research, education and other noncommercial use. Copyright (c) 2026 Bigpoint; see [LICENSE.md](LICENSE.md).

## Changes made before publishing

- Namespaces are now `TeaSpoons.*` and the package id is `com.tea-spoons.large-numbers` (assemblies renamed to match).
- Internal build, registry and tracker references were removed; the repo uses GitHub Actions (`CI` and `Release`) built on `unity-ci-kit`.
- Added `LICENSE.md` (PolyForm Noncommercial 1.0.0), an install section in the README, and package metadata (author, license and documentation URLs).

## Planned changes

- [x] Tag and publish `v0.7.2` with the Release workflow.
- [ ] Run this package's tests in CI with `unity-ci-kit` (needs a small test-project helper in the kit).
<!-- review-items:start -->
- [ ] **P0** Fix `inverseScaleFactor` (`1.0 / ScaleFactor`) and add a regression test on `FloatingDigits` for a nine-digit value.
- [ ] **P1** Add property tests: `Parse`/`ToString` round trip, total ordering, `(a + b) - b` within tolerance, overflow at 10^65,536, and `long.MinValue` negation.
- [ ] **P1** Add `TryFormat(Span<char>)` and `TryParse(ReadOnlySpan<char>)` to avoid allocations, with culture support, aligned with `number-formatting`.
- [ ] **P1** Declares `unity: 2022.3`, but only Unity 6000.3.8f1 was tested. Add a Unity version matrix to CI once package tests run there (see the `unity-ci-kit` plan), or raise the minimum.
- [ ] **P2** Write a "Choosing a number type" table in the README: `long`, `LargeInt`, BigDouble-style doubles and `BigInteger`, with range, precision, determinism and speed.
- [ ] **P2** Benchmark add/multiply/compare against BigDouble and publish the numbers.
- [ ] **P2** Add a `CHANGELOG.md`. Unity's package layout lists one next to `README.md`, and the `unity-ci-kit` validator warns without it.
- [ ] **P2** The README is only 33 lines. Add a short example for each public type.
<!-- review-items:end -->

<!-- review:start -->
## Review (September 2026)

Reviewed as a senior Unity engineer would: I read the code and compared the package with similar open-source projects (September 2026). Those projects are listed for ideas only. Nothing was copied from them, and their licenses are noted in case code is ever reused. Priorities: **P0** correctness bug or broken metadata, **P1** should be done soon, **P2** nice to have.

### Compared with

| Project | License | Worth noting |
|---|---|---|
| [Razenpok/BreakInfinity.cs](https://github.com/Razenpok/BreakInfinity.cs) | MIT | C# port of break_infinity.js: a `BigDouble` that replaces `double` up to about 1e(9e15), built for incremental games, "speed over accuracy". A single file to drop in; `ToString` with G, E and F formats. |

### Findings from reading the code

- **[Bug]** `private const double inverseScaleFactor = 1f / ScaleFactor;` (`LargeInt.cs`, line 23) is evaluated in single precision and then widened. `FloatingDigits` and `Divide` use it, so results carry a relative error of about 1e-8 in a struct that promises 9 significant digits. `1.0 / ScaleFactor` is exact enough.
- **[Naming]** `LargeInt` is a decimal floating value (a `long` mantissa with 9 significant digits times 10^`ushort`), not an exact integer. The doc comment says so, the name does not.
- **[Design note]** The mantissa is an integer, so results are identical on every platform. A `double` mantissa (as in BigDouble) is not. That is an advantage for server/client parity worth stating in the README. The exponent is unsigned, so fractions below 1 cannot be represented.
- **[Tests]** 350 test lines for 730 lines of code; no round-trip or ordering property tests.
<!-- review:end -->

## Notes and ideas

_Add your own here._
