using System;
using System.Diagnostics.Contracts;
using Soenneker.Enums.UnitOfTime;

namespace Soenneker.Extensions.DateTimeOffsets.Years;

/// <summary>
/// Provides extension methods for <see cref="DateTimeOffset"/> that operate on year boundaries,
/// including helpers that compute year starts/ends in a specified time zone while returning UTC instants.
/// </summary>
public static class DateTimeOffsetsYearsExtension
{
    /// <summary>
    /// Returns the start of the year containing <paramref name="dateTimeOffset"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The value to adjust.</param>
    /// <returns>
    /// A <see cref="DateTimeOffset"/> representing the first moment of the year containing <paramref name="dateTimeOffset"/>.
    /// </returns>
    /// <remarks>
    /// No time zone conversion is performed and the offset is preserved.
    /// </remarks>
    [Pure]
    public static DateTimeOffset ToStartOfYear(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.Trim(UnitOfTime.Year);

    /// <summary>
    /// Returns the end of the year containing <paramref name="dateTimeOffset"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The value to adjust.</param>
    /// <returns>
    /// A <see cref="DateTimeOffset"/> representing the last tick of the year containing <paramref name="dateTimeOffset"/>.
    /// </returns>
    /// <remarks>
    /// Computed as one tick before the start of the next year. No time zone conversion is performed and the offset is preserved.
    /// </remarks>
    [Pure]
    public static DateTimeOffset ToEndOfYear(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToStartOfYear()
                      .AddYears(1)
                      .AddTicks(-1);

    /// <summary>
    /// Returns the start of the next year relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The value to adjust.</param>
    /// <returns>A <see cref="DateTimeOffset"/> representing the first moment of the next year.</returns>
    /// <remarks>No time zone conversion is performed and the offset is preserved.</remarks>
    [Pure]
    public static DateTimeOffset ToStartOfNextYear(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToStartOfYear()
                      .AddYears(1);

    /// <summary>
    /// Returns the start of the previous year relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The value to adjust.</param>
    /// <returns>A <see cref="DateTimeOffset"/> representing the first moment of the previous year.</returns>
    /// <remarks>No time zone conversion is performed and the offset is preserved.</remarks>
    [Pure]
    public static DateTimeOffset ToStartOfPreviousYear(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToStartOfYear()
                      .AddYears(-1);

    /// <summary>
    /// Returns the end of the previous year relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The value to adjust.</param>
    /// <returns>A <see cref="DateTimeOffset"/> representing the last tick of the previous year.</returns>
    /// <remarks>
    /// Computed as one tick before the start of the current year. No time zone conversion is performed and the offset is preserved.
    /// </remarks>
    [Pure]
    public static DateTimeOffset ToEndOfPreviousYear(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToStartOfYear()
                      .AddTicks(-1);

    /// <summary>
    /// Returns the end of the next year relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The value to adjust.</param>
    /// <returns>A <see cref="DateTimeOffset"/> representing the last tick of the next year.</returns>
    /// <remarks>
    /// Computed as one tick before the start of the year after next. No time zone conversion is performed and the offset is preserved.
    /// </remarks>
    [Pure]
    public static DateTimeOffset ToEndOfNextYear(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToStartOfYear()
                      .AddYears(2)
                      .AddTicks(-1);

    /// <summary>
    /// Computes the start of the year in <paramref name="tz"/> that contains the instant <paramref name="utcInstant"/>,
    /// returning the result as a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="utcInstant">
    /// An instant in time. It is normalized to UTC before conversion and treated as an instant (not a local wall time).
    /// </param>
    /// <param name="tz">The time zone whose local calendar rules determine year boundaries.</param>
    /// <returns>
    /// A UTC <see cref="DateTimeOffset"/> representing the start of the year in <paramref name="tz"/> that contains <paramref name="utcInstant"/>.
    /// </returns>
    /// <remarks>
    /// This computes the boundary as a local wall time (00:00 on Jan 1) and maps it to UTC using the time zone's rules
    /// at that wall time (DST-safe).
    /// </remarks>
    [Pure]
    public static DateTimeOffset ToStartOfTzYear(this DateTimeOffset utcInstant, TimeZoneInfo tz)
    {
        DateTimeOffset utc = utcInstant.ToUniversalTime();
        DateTimeOffset local = TimeZoneInfo.ConvertTime(utc, tz);

        DateTime localStart = new(local.Year, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        DateTime utcStart = TimeZoneInfo.ConvertTimeToUtc(localStart, tz);
        return new DateTimeOffset(utcStart, TimeSpan.Zero);
    }

    /// <summary>
    /// Computes the end of the year in <paramref name="tz"/> that contains the instant <paramref name="utcInstant"/>,
    /// returning the result as a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="utcInstant">
    /// An instant in time. It is normalized to UTC before conversion and treated as an instant (not a local wall time).
    /// </param>
    /// <param name="tz">The time zone whose local calendar rules determine year boundaries.</param>
    /// <returns>A UTC <see cref="DateTimeOffset"/> representing the last tick of the year in <paramref name="tz"/>.</returns>
    /// <remarks>
    /// Computed as one tick before the start of the next year in <paramref name="tz"/> (DST-safe).
    /// </remarks>
    [Pure]
    public static DateTimeOffset ToEndOfTzYear(this DateTimeOffset utcInstant, TimeZoneInfo tz) =>
        utcInstant.ToStartOfTzYear(tz)
                  .AddYears(1)
                  .AddTicks(-1);

    /// <summary>
    /// Computes the start of the previous year in <paramref name="tz"/> relative to the instant <paramref name="utcInstant"/>,
    /// returning the result as a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="utcInstant">
    /// An instant in time. It is normalized to UTC before conversion and treated as an instant (not a local wall time).
    /// </param>
    /// <param name="tz">The time zone whose local calendar rules determine year boundaries.</param>
    /// <returns>A UTC <see cref="DateTimeOffset"/> representing the start of the previous year in <paramref name="tz"/>.</returns>
    [Pure]
    public static DateTimeOffset ToStartOfPreviousTzYear(this DateTimeOffset utcInstant, TimeZoneInfo tz) =>
        utcInstant.ToStartOfTzYear(tz)
                  .AddYears(-1);

    /// <summary>
    /// Computes the end of the previous year in <paramref name="tz"/> relative to the instant <paramref name="utcInstant"/>,
    /// returning the result as a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="utcInstant">
    /// An instant in time. It is normalized to UTC before conversion and treated as an instant (not a local wall time).
    /// </param>
    /// <param name="tz">The time zone whose local calendar rules determine year boundaries.</param>
    /// <returns>A UTC <see cref="DateTimeOffset"/> representing the last tick of the previous year in <paramref name="tz"/>.</returns>
    /// <remarks>
    /// Computed as one tick before the start of the current year in <paramref name="tz"/> (DST-safe).
    /// </remarks>
    [Pure]
    public static DateTimeOffset ToEndOfPreviousTzYear(this DateTimeOffset utcInstant, TimeZoneInfo tz) =>
        utcInstant.ToStartOfTzYear(tz)
                  .AddTicks(-1);

    /// <summary>
    /// Computes the start of the next year in <paramref name="tz"/> relative to the instant <paramref name="utcInstant"/>,
    /// returning the result as a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="utcInstant">
    /// An instant in time. It is normalized to UTC before conversion and treated as an instant (not a local wall time).
    /// </param>
    /// <param name="tz">The time zone whose local calendar rules determine year boundaries.</param>
    /// <returns>A UTC <see cref="DateTimeOffset"/> representing the start of the next year in <paramref name="tz"/>.</returns>
    [Pure]
    public static DateTimeOffset ToStartOfNextTzYear(this DateTimeOffset utcInstant, TimeZoneInfo tz) =>
        utcInstant.ToStartOfTzYear(tz)
                  .AddYears(1);

    /// <summary>
    /// Computes the end of the next year in <paramref name="tz"/> relative to the instant <paramref name="utcInstant"/>,
    /// returning the result as a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="utcInstant">
    /// An instant in time. It is normalized to UTC before conversion and treated as an instant (not a local wall time).
    /// </param>
    /// <param name="tz">The time zone whose local calendar rules determine year boundaries.</param>
    /// <returns>A UTC <see cref="DateTimeOffset"/> representing the last tick of the next year in <paramref name="tz"/>.</returns>
    /// <remarks>
    /// Computed as one tick before the start of the year after next in <paramref name="tz"/> (DST-safe).
    /// </remarks>
    [Pure]
    public static DateTimeOffset ToEndOfNextTzYear(this DateTimeOffset utcInstant, TimeZoneInfo tz) =>
        utcInstant.ToStartOfTzYear(tz)
                  .AddYears(2)
                  .AddTicks(-1);
}