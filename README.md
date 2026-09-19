# Large Numbers
Support for creating numbers with plenty of digits.

### Range
`LargeInt` uses an unsigned 16-bit integer as the decimal exponent, allowing for numbers with up to 65,536 digits.

### Significant Digits
`LargeInt` currently has *9 significant digits*.

## Usage
- Create and use `LargeInt` instances for your large numbers.
- `LargeInt` are immutable. To serialize them, use the `SerializableLargeInt` wrapper.

## Installation

In Unity: **Window > Package Manager > + > Add package from git URL**, then enter:

```
https://github.com/tea-spoons/large-numbers.git
```

Pin a release by appending a tag, for example `#v0.7.2`.

## License

Copyright (c) 2026 Bigpoint. Authored by Muhammad Tarek Abdou.

Available for research, education and other noncommercial use under the [PolyForm Noncommercial 1.0.0](LICENSE.md)
license. Commercial use is not permitted.
