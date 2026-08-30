[![](https://img.shields.io/nuget/v/soenneker.extensions.datetimeoffsets.years.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.datetimeoffsets.years/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.datetimeoffsets.years/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.datetimeoffsets.years/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.datetimeoffsets.years.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.datetimeoffsets.years/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.datetimeoffsets.years/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.datetimeoffsets.years/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.DateTimeOffsets.Years

Year-boundary extensions for `DateTimeOffset`, including boundaries defined by a supplied time zone.

## Installation

```bash
dotnet add package Soenneker.Extensions.DateTimeOffsets.Years
```

## Offset-preserving boundaries

Use the ordinary methods when the value's displayed year and existing offset are the intended frame of reference. No time-zone conversion occurs.

```csharp
using Soenneker.Extensions.DateTimeOffsets.Years;

var value = new DateTimeOffset(2024, 6, 15, 12, 30, 0, TimeSpan.FromHours(-5));

DateTimeOffset start = value.ToStartOfYear();
// 2024-01-01 00:00:00 -05:00

DateTimeOffset end = value.ToEndOfYear();
// 2024-12-31 23:59:59.9999999 -05:00
```

Previous and next boundaries are available through `ToStartOfPreviousYear()`, `ToEndOfPreviousYear()`, `ToStartOfNextYear()`, and `ToEndOfNextYear()`.

## Time-zone year boundaries

Use the `Tz` methods when the input represents an instant and the supplied time zone determines its local year. Results are UTC `DateTimeOffset` values with a zero offset.

```csharp
TimeZoneInfo eastern = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
var instant = new DateTimeOffset(2024, 6, 15, 12, 30, 0, TimeSpan.Zero);

DateTimeOffset startUtc = instant.ToStartOfTzYear(eastern);
// 2024-01-01 05:00:00 +00:00

DateTimeOffset nextStartUtc = instant.ToStartOfNextTzYear(eastern);
// 2025-01-01 05:00:00 +00:00
```

The time-zone variants are `ToStartOfTzYear()`, `ToEndOfTzYear()`, `ToStartOfPreviousTzYear()`, `ToEndOfPreviousTzYear()`, `ToStartOfNextTzYear()`, and `ToEndOfNextTzYear()`.

End methods are inclusive and return one tick before the following year begins. Every time-zone boundary is calculated from that local January 1, so historical offset and dateline changes are respected. A skipped local midnight resolves to the first valid local time, and an ambiguous midnight resolves to its earlier UTC occurrence.

Operations that would pass the `DateTimeOffset` range return `DateTimeOffset.MinValue` or `DateTimeOffset.MaxValue` instead of throwing.
