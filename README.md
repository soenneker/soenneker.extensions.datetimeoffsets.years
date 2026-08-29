[![](https://img.shields.io/nuget/v/soenneker.extensions.datetimeoffsets.years.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.datetimeoffsets.years/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.datetimeoffsets.years/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.datetimeoffsets.years/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.datetimeoffsets.years.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.datetimeoffsets.years/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.datetimeoffsets.years/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.datetimeoffsets.years/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.DateTimeOffsets.Years
Helpful extension methods surrounding DateTimeOffsets relating to years.

## Installation

```bash
dotnet add package Soenneker.Extensions.DateTimeOffsets.Years
```

## Quick start

```csharp
using Soenneker.Extensions.DateTimeOffsets.Years;

DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
var result = dateTimeOffset.ToStartOfYear();
```

## Common operations

- `ToStartOfYear()` - Returns the start of the year containing `dateTimeOffset`.
- `ToEndOfYear()` - Returns the end of the year containing `dateTimeOffset`.
- `ToStartOfNextYear()` - Returns the start of the next year relative to `dateTimeOffset`.
- `ToStartOfPreviousYear()` - Returns the start of the previous year relative to `dateTimeOffset`.
- `ToEndOfPreviousYear()` - Returns the end of the previous year relative to `dateTimeOffset`.
- `ToEndOfNextYear()` - Returns the end of the next year relative to `dateTimeOffset`.
- `ToStartOfTzYear()` - Computes the start of the year in `tz` that contains the instant `utcInstant`, returning the boundary as a UTC `DateTimeOffset`. The boundary is formed as a local wall time (`00:00` on Jan 1) and mapped to UTC using `tz`'s rules.
- `ToEndOfTzYear()` - Computes the end of the year in `tz` that contains `utcInstant`, returning the boundary as a UTC `DateTimeOffset`.
- `ToStartOfPreviousTzYear()` - Computes the start of the previous year in `tz` relative to `utcInstant`, returning the boundary as a UTC `DateTimeOffset`.
- `ToEndOfPreviousTzYear()` - Computes the end of the previous year in `tz` relative to `utcInstant`, returning the boundary as a UTC `DateTimeOffset`.
- `ToStartOfNextTzYear()` - Computes the start of the next year in `tz` relative to `utcInstant`, returning the boundary as a UTC `DateTimeOffset`.
- `ToEndOfNextTzYear()` - Computes the end of the next year in `tz` relative to `utcInstant`, returning the boundary as a UTC `DateTimeOffset`.
