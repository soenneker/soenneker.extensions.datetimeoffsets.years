using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Soenneker.Enums.UnitOfTime;

namespace Soenneker.Extensions.DateTimeOffsets.Years;

/// <summary>
/// Extension methods for <see cref="DateTimeOffset"/> that operate on year boundaries.
/// </summary>
/// <remarks>
/// <para>
/// The “non-TZ” methods (e.g., <see cref="ToStartOfYear(DateTimeOffset)"/>) preserve the input offset and do not perform any time-zone conversion.
/// </para>
/// <para>
/// The “TZ” methods (e.g., <see cref="ToStartOfTzYear(DateTimeOffset, TimeZoneInfo)"/>) treat the input as an instant in time,
/// compute the year boundary using the calendar rules of the provided <see cref="TimeZoneInfo"/>, and return the boundary as a UTC instant
/// (<see cref="DateTimeOffset.Offset"/> = <see cref="TimeSpan.Zero"/>).
/// </para>
/// <para>
/// End-of-* methods are defined as “one tick before the start of the next boundary” (tick = 100ns).
/// </para>
/// </remarks>
public static class DateTimeOffsetsYearsExtension
{
    private const long _oneTick = 1;

    /// <summary>
    /// Returns the start of the year containing <paramref name="dateTimeOffset"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The value to adjust.</param>
    /// <returns>
    /// A <see cref="DateTimeOffset"/> representing 00:00:00.0000000 on January 1 of the same year (same offset).
    /// </returns>
    /// <remarks>
    /// No time-zone conversion is performed; the offset is preserved.
    /// This delegates to <c>Trim(UnitOfTime.Year)</c>, so the exact trimming semantics depend on your <c>Trim</c> implementation.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToStartOfYear(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.Trim(UnitOfTime.Year);

    /// <summary>
    /// Returns the end of the year containing <paramref name="dateTimeOffset"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The value to adjust.</param>
    /// <returns>
    /// A <see cref="DateTimeOffset"/> representing the last tick of the same year (same offset).
    /// </returns>
    /// <remarks>
    /// Defined as one tick before the start of the next year.
    /// No time-zone conversion is performed; the offset is preserved.
    /// If the computation would overflow, <see cref="DateTimeOffset.MaxValue"/> is returned.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToEndOfYear(this DateTimeOffset dateTimeOffset)
    {
        DateTimeOffset start = dateTimeOffset.ToStartOfYear();
        DateTimeOffset nextStart = SafeAddYears(start, 1, DateTimeOffset.MaxValue);
        return SafeAddTicksPreserveOffset(nextStart, -_oneTick, DateTimeOffset.MaxValue);
    }

    /// <summary>
    /// Returns the start of the next year relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The value to adjust.</param>
    /// <returns>A <see cref="DateTimeOffset"/> representing the start of the next year (same offset).</returns>
    /// <remarks>
    /// No time-zone conversion is performed; the offset is preserved.
    /// If the computation would overflow, <see cref="DateTimeOffset.MaxValue"/> is returned.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToStartOfNextYear(this DateTimeOffset dateTimeOffset)
    {
        DateTimeOffset start = dateTimeOffset.ToStartOfYear();
        return SafeAddYears(start, 1, DateTimeOffset.MaxValue);
    }

    /// <summary>
    /// Returns the start of the previous year relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The value to adjust.</param>
    /// <returns>A <see cref="DateTimeOffset"/> representing the start of the previous year (same offset).</returns>
    /// <remarks>
    /// No time-zone conversion is performed; the offset is preserved.
    /// If the computation would underflow, <see cref="DateTimeOffset.MinValue"/> is returned.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToStartOfPreviousYear(this DateTimeOffset dateTimeOffset)
    {
        DateTimeOffset start = dateTimeOffset.ToStartOfYear();
        return SafeAddYears(start, -1, DateTimeOffset.MinValue);
    }

    /// <summary>
    /// Returns the end of the previous year relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The value to adjust.</param>
    /// <returns>A <see cref="DateTimeOffset"/> representing the last tick of the previous year (same offset).</returns>
    /// <remarks>
    /// Defined as one tick before the start of the current year.
    /// No time-zone conversion is performed; the offset is preserved.
    /// If the computation would underflow, <see cref="DateTimeOffset.MinValue"/> is returned.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToEndOfPreviousYear(this DateTimeOffset dateTimeOffset)
    {
        DateTimeOffset start = dateTimeOffset.ToStartOfYear();
        return SafeAddTicksPreserveOffset(start, -_oneTick, DateTimeOffset.MinValue);
    }

    /// <summary>
    /// Returns the end of the next year relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The value to adjust.</param>
    /// <returns>A <see cref="DateTimeOffset"/> representing the last tick of the next year (same offset).</returns>
    /// <remarks>
    /// Defined as one tick before the start of the year after next.
    /// No time-zone conversion is performed; the offset is preserved.
    /// If the computation would overflow, <see cref="DateTimeOffset.MaxValue"/> is returned.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToEndOfNextYear(this DateTimeOffset dateTimeOffset)
    {
        DateTimeOffset start = dateTimeOffset.ToStartOfYear();
        DateTimeOffset afterNextStart = SafeAddYears(start, 2, DateTimeOffset.MaxValue);
        return SafeAddTicksPreserveOffset(afterNextStart, -_oneTick, DateTimeOffset.MaxValue);
    }

    /// <summary>
    /// Computes the start of the year in <paramref name="tz"/> that contains the instant <paramref name="utcInstant"/>,
    /// returning the boundary as a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="utcInstant">
    /// The instant to locate within <paramref name="tz"/>'s calendar.
    /// The value is normalized via <see cref="DateTimeOffset.ToUniversalTime()"/>; it does not need to already be UTC.
    /// </param>
    /// <param name="tz">The time zone whose local calendar rules determine year boundaries.</param>
    /// <returns>
    /// A UTC <see cref="DateTimeOffset"/> (offset = 00:00) representing 00:00 on January 1 in <paramref name="tz"/> for the located local year.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="tz"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// The boundary is formed as a local wall time (<c>00:00</c> on Jan 1) and mapped to UTC using <paramref name="tz"/>'s rules.
    /// <para>
    /// For custom/edge time zones where the local boundary is invalid (gap) or ambiguous (overlap),
    /// this method:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Advances to the first valid local minute if the boundary is invalid.</description></item>
    /// <item><description>Chooses the earlier occurrence if the boundary is ambiguous.</description></item>
    /// </list>
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToStartOfTzYear(this DateTimeOffset utcInstant, TimeZoneInfo tz)
    {
        if (tz is null)
            throw new ArgumentNullException(nameof(tz));

        DateTimeOffset utc = utcInstant.ToUniversalTime();
        DateTimeOffset local = TimeZoneInfo.ConvertTime(utc, tz);

        // Local wall-time boundary (00:00 Jan 1 of the local year)
        var localStart = new DateTime(local.Year, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        DateTime utcStart = ConvertLocalBoundaryToUtc(localStart, tz);
        return new DateTimeOffset(utcStart, TimeSpan.Zero);
    }

    /// <summary>
    /// Computes the end of the year in <paramref name="tz"/> that contains <paramref name="utcInstant"/>,
    /// returning the boundary as a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="utcInstant">The instant to locate; normalized to UTC and treated as an instant.</param>
    /// <param name="tz">The time zone whose local calendar rules determine year boundaries.</param>
    /// <returns>A UTC <see cref="DateTimeOffset"/> representing the last tick of that local year in <paramref name="tz"/>.</returns>
    /// <remarks>
    /// Defined as one tick before the start of the next local year in <paramref name="tz"/>.
    /// If the computation would overflow, <see cref="DateTimeOffset.MaxValue"/> is returned.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToEndOfTzYear(this DateTimeOffset utcInstant, TimeZoneInfo tz)
    {
        DateTimeOffset start = utcInstant.ToStartOfTzYear(tz);
        DateTimeOffset next = SafeAddYears(start, 1, DateTimeOffset.MaxValue);
        return SafeAddTicksPreserveOffset(next, -_oneTick, DateTimeOffset.MaxValue);
    }

    /// <summary>
    /// Computes the start of the previous year in <paramref name="tz"/> relative to <paramref name="utcInstant"/>,
    /// returning the boundary as a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="utcInstant">The instant to locate; normalized to UTC and treated as an instant.</param>
    /// <param name="tz">The time zone whose local calendar rules determine year boundaries.</param>
    /// <returns>A UTC <see cref="DateTimeOffset"/> representing the start of the previous local year in <paramref name="tz"/>.</returns>
    /// <remarks>If the computation would underflow, <see cref="DateTimeOffset.MinValue"/> is returned.</remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToStartOfPreviousTzYear(this DateTimeOffset utcInstant, TimeZoneInfo tz)
    {
        DateTimeOffset start = utcInstant.ToStartOfTzYear(tz);
        return SafeAddYears(start, -1, DateTimeOffset.MinValue);
    }

    /// <summary>
    /// Computes the end of the previous year in <paramref name="tz"/> relative to <paramref name="utcInstant"/>,
    /// returning the boundary as a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="utcInstant">The instant to locate; normalized to UTC and treated as an instant.</param>
    /// <param name="tz">The time zone whose local calendar rules determine year boundaries.</param>
    /// <returns>A UTC <see cref="DateTimeOffset"/> representing the last tick of the previous local year in <paramref name="tz"/>.</returns>
    /// <remarks>
    /// Defined as one tick before the start of the current local year in <paramref name="tz"/>.
    /// If the computation would underflow, <see cref="DateTimeOffset.MinValue"/> is returned.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToEndOfPreviousTzYear(this DateTimeOffset utcInstant, TimeZoneInfo tz)
    {
        DateTimeOffset start = utcInstant.ToStartOfTzYear(tz);
        return SafeAddTicksPreserveOffset(start, -_oneTick, DateTimeOffset.MinValue);
    }

    /// <summary>
    /// Computes the start of the next year in <paramref name="tz"/> relative to <paramref name="utcInstant"/>,
    /// returning the boundary as a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="utcInstant">The instant to locate; normalized to UTC and treated as an instant.</param>
    /// <param name="tz">The time zone whose local calendar rules determine year boundaries.</param>
    /// <returns>A UTC <see cref="DateTimeOffset"/> representing the start of the next local year in <paramref name="tz"/>.</returns>
    /// <remarks>If the computation would overflow, <see cref="DateTimeOffset.MaxValue"/> is returned.</remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToStartOfNextTzYear(this DateTimeOffset utcInstant, TimeZoneInfo tz)
    {
        DateTimeOffset start = utcInstant.ToStartOfTzYear(tz);
        return SafeAddYears(start, 1, DateTimeOffset.MaxValue);
    }

    /// <summary>
    /// Computes the end of the next year in <paramref name="tz"/> relative to <paramref name="utcInstant"/>,
    /// returning the boundary as a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="utcInstant">The instant to locate; normalized to UTC and treated as an instant.</param>
    /// <param name="tz">The time zone whose local calendar rules determine year boundaries.</param>
    /// <returns>A UTC <see cref="DateTimeOffset"/> representing the last tick of the next local year in <paramref name="tz"/>.</returns>
    /// <remarks>
    /// Defined as one tick before the start of the year after next in <paramref name="tz"/>.
    /// If the computation would overflow, <see cref="DateTimeOffset.MaxValue"/> is returned.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToEndOfNextTzYear(this DateTimeOffset utcInstant, TimeZoneInfo tz)
    {
        DateTimeOffset start = utcInstant.ToStartOfTzYear(tz);
        DateTimeOffset afterNext = SafeAddYears(start, 2, DateTimeOffset.MaxValue);
        return SafeAddTicksPreserveOffset(afterNext, -_oneTick, DateTimeOffset.MaxValue);
    }

    /// <summary>
    /// Adds years with a fallback for overflow/underflow.
    /// </summary>
    /// <remarks>
    /// <see cref="DateTimeOffset.AddYears(int)"/> performs calendar-correct math (leap days, etc.),
    /// so we keep it and only pay exception cost in extremely rare boundary cases.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static DateTimeOffset SafeAddYears(DateTimeOffset value, int years, DateTimeOffset fallback)
    {
        try
        {
            return value.AddYears(years);
        }
        catch (ArgumentOutOfRangeException)
        {
            return fallback;
        }
    }

    /// <summary>
    /// Adds ticks while preserving the original offset, using a fast bounds check to avoid exceptions in the common case.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static DateTimeOffset SafeAddTicksPreserveOffset(DateTimeOffset value, long ticks, DateTimeOffset fallback)
    {
        // Fast-path under/overflow checks using UTC ticks (range is defined by UTC representability).
        long utcTicks = value.UtcTicks;
        long minUtc = DateTimeOffset.MinValue.UtcTicks;
        long maxUtc = DateTimeOffset.MaxValue.UtcTicks;

        if (ticks < 0)
        {
            long delta = -ticks;
            if ((ulong)(utcTicks - minUtc) < (ulong)delta)
                return fallback;
        }
        else if (ticks > 0)
        {
            if ((ulong)(maxUtc - utcTicks) < (ulong)ticks)
                return fallback;
        }

        // Preserve offset: construct from local ticks + original offset.
        // This stays exception-free after the UTC bounds checks above.
        return new DateTimeOffset(value.Ticks + ticks, value.Offset);
    }

    /// <summary>
    /// Converts a local (unspecified-kind) wall-time boundary to UTC for <paramref name="tz"/>, handling invalid/ambiguous times.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static DateTime ConvertLocalBoundaryToUtc(DateTime localBoundary, TimeZoneInfo tz)
    {
        // Extremely rare for a year boundary, but possible for custom zones:
        // If invalid (gap), move forward to first valid minute.
        if (tz.IsInvalidTime(localBoundary))
        {
            DateTime probe = localBoundary;

            // Upper bound search: 24h in 1-minute steps. Executes only on the invalid-time path.
            for (int i = 0; i < 24 * 60; i++)
            {
                probe = probe.AddMinutes(1);
                if (!tz.IsInvalidTime(probe))
                {
                    localBoundary = probe;
                    break;
                }
            }
        }

        // If ambiguous (overlap), choose the earlier occurrence.
        if (tz.IsAmbiguousTime(localBoundary))
        {
            TimeSpan[] offsets = tz.GetAmbiguousTimeOffsets(localBoundary);

            // Earlier occurrence corresponds to the larger offset (e.g., -04:00 is earlier than -05:00).
            TimeSpan chosen;
            if (offsets.Length == 2)
                chosen = offsets[0] >= offsets[1] ? offsets[0] : offsets[1];
            else if (offsets.Length == 1)
                chosen = offsets[0];
            else
                chosen = tz.GetUtcOffset(localBoundary);

            return new DateTimeOffset(localBoundary, chosen).UtcDateTime;
        }

        // Non-ambiguous, valid time: map via offset.
        TimeSpan offset = tz.GetUtcOffset(localBoundary);
        return new DateTimeOffset(localBoundary, offset).UtcDateTime;
    }
}