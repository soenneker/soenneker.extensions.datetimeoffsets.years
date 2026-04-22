using AwesomeAssertions;
using System;
using Soenneker.Tests.Unit;

namespace Soenneker.Extensions.DateTimeOffsets.Years.Tests;

public sealed class DateTimeOffsetsYearsExtensionTests : UnitTest
{
    [Test]
    public void ToStartOfYear_preserves_offset()
    {
        var withOffset = new DateTimeOffset(2024, 6, 15, 12, 30, 0, TimeSpan.FromHours(-5));
        DateTimeOffset result = withOffset.ToStartOfYear();

        result.Year.Should().Be(2024);
        result.Month.Should().Be(1);
        result.Day.Should().Be(1);
        result.Hour.Should().Be(0);
        result.Minute.Should().Be(0);
        result.Second.Should().Be(0);
        result.Offset.Should().Be(TimeSpan.FromHours(-5));
    }

    [Test]
    public void ToEndOfYear_returns_last_tick_of_year_one_tick_before_next_year()
    {
        var midYear = new DateTimeOffset(2024, 7, 1, 0, 0, 0, TimeSpan.Zero);
        DateTimeOffset endOfYear = midYear.ToEndOfYear();
        DateTimeOffset startOfNextYear = midYear.ToStartOfNextYear();

        startOfNextYear.Ticks.Should().Be(endOfYear.Ticks + 1);
        endOfYear.Year.Should().Be(2024);
        endOfYear.Month.Should().Be(12);
        endOfYear.Day.Should().Be(31);
        endOfYear.Hour.Should().Be(23);
        endOfYear.Minute.Should().Be(59);
        endOfYear.Second.Should().Be(59);
        // End-of-year is one tick before start of next (already asserted above); sub-second precision depends on Trim
        endOfYear.Millisecond.Should().Be(999);
    }

    [Test]
    public void ToEndOfYear_leap_year_includes_feb_29()
    {
        var leapDay = new DateTimeOffset(2024, 2, 29, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset endOfYear = leapDay.ToEndOfYear();

        endOfYear.Year.Should().Be(2024);
        endOfYear.Month.Should().Be(12);
        endOfYear.Day.Should().Be(31);
    }

    [Test]
    public void ToStartOfPreviousYear_at_MinValue_returns_MinValue()
    {
        DateTimeOffset result = DateTimeOffset.MinValue.ToStartOfPreviousYear();
        result.Should().Be(DateTimeOffset.MinValue);
    }

    [Test]
    public void ToEndOfPreviousYear_at_MinValue_returns_MinValue()
    {
        DateTimeOffset result = DateTimeOffset.MinValue.ToEndOfPreviousYear();
        result.Should().Be(DateTimeOffset.MinValue);
    }

    [Test]
    public void ToEndOfPreviousYear_at_start_of_year_returns_last_tick_of_previous_year()
    {
        var startOf2024 = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        DateTimeOffset endOfPrev = startOf2024.ToEndOfPreviousYear();

        endOfPrev.Year.Should().Be(2023);
        endOfPrev.Month.Should().Be(12);
        endOfPrev.Day.Should().Be(31);
        endOfPrev.Hour.Should().Be(23);
        endOfPrev.Minute.Should().Be(59);
        endOfPrev.Second.Should().Be(59);
    }

    [Test]
    public void ToStartOfNextYear_at_MaxValue_returns_MaxValue()
    {
        DateTimeOffset result = DateTimeOffset.MaxValue.ToStartOfNextYear();
        result.Should().Be(DateTimeOffset.MaxValue);
    }

    [Test]
    public void ToEndOfYear_at_MaxValue_returns_last_tick_before_overflow()
    {
        // When next year overflows, implementation returns (nextStart - 1 tick); nextStart is MaxValue so result is MaxValue - 1 tick
        DateTimeOffset result = DateTimeOffset.MaxValue.ToEndOfYear();
        result.Ticks.Should().Be(DateTimeOffset.MaxValue.Ticks - 1);
        result.Offset.Should().Be(DateTimeOffset.MaxValue.Offset);
    }

    [Test]
    public void ToEndOfNextYear_at_MaxValue_returns_last_tick_before_overflow()
    {
        DateTimeOffset result = DateTimeOffset.MaxValue.ToEndOfNextYear();
        result.Ticks.Should().Be(DateTimeOffset.MaxValue.Ticks - 1);
    }

    [Test]
    public void ToStartOfNextYear_normal_returns_jan_1_next_year()
    {
        var midYear = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.FromHours(2));
        DateTimeOffset result = midYear.ToStartOfNextYear();

        result.Year.Should().Be(2025);
        result.Month.Should().Be(1);
        result.Day.Should().Be(1);
        result.Hour.Should().Be(0);
        result.Minute.Should().Be(0);
        result.Offset.Should().Be(TimeSpan.FromHours(2));
    }

    [Test]
    public void ToStartOfPreviousYear_normal_returns_jan_1_previous_year()
    {
        var midYear = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero);
        DateTimeOffset result = midYear.ToStartOfPreviousYear();

        result.Year.Should().Be(2023);
        result.Month.Should().Be(1);
        result.Day.Should().Be(1);
        result.Hour.Should().Be(0);
    }

    [Test]
    public void ToStartOfTzYear_null_tz_throws_ArgumentNullException()
    {
        var utc = new DateTimeOffset(2024, 6, 1, 0, 0, 0, TimeSpan.Zero);

        ArgumentNullException ex;

        try
        {
            utc.ToStartOfTzYear(null!);
            throw new Exception("Expected ArgumentNullException.");
        }
        catch (ArgumentNullException exception)
        {
            ex = exception;
        }

        ex.ParamName.Should().Be("tz");
    }

    [Test]
    public void ToStartOfTzYear_UTC_returns_jan_1_utc_for_that_year()
    {
        var utc = new DateTimeOffset(2024, 7, 15, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset result = utc.ToStartOfTzYear(TimeZoneInfo.Utc);

        result.Year.Should().Be(2024);
        result.Month.Should().Be(1);
        result.Day.Should().Be(1);
        result.Hour.Should().Be(0);
        result.Minute.Should().Be(0);
        result.Offset.Should().Be(TimeSpan.Zero);
    }

    [Test]
    public void ToStartOfTzYear_returns_UTC_offset_zero()
    {
        var localWithOffset = new DateTimeOffset(2024, 3, 1, 0, 0, 0, TimeSpan.FromHours(-6));
        DateTimeOffset result = localWithOffset.ToStartOfTzYear(TimeZoneInfo.Utc);

        result.Offset.Should().Be(TimeSpan.Zero);
    }

    [Test]
    public void ToEndOfTzYear_one_tick_before_start_of_next_tz_year()
    {
        var utc = new DateTimeOffset(2024, 7, 1, 0, 0, 0, TimeSpan.Zero);
        DateTimeOffset endOfTzYear = utc.ToEndOfTzYear(TimeZoneInfo.Utc);
        DateTimeOffset startOfNextTzYear = utc.ToStartOfNextTzYear(TimeZoneInfo.Utc);

        startOfNextTzYear.Ticks.Should().Be(endOfTzYear.Ticks + 1);
        endOfTzYear.Year.Should().Be(2024);
        endOfTzYear.Month.Should().Be(12);
        endOfTzYear.Day.Should().Be(31);
    }

    [Test]
    public void ToStartOfPreviousTzYear_at_MinValue_returns_MinValue()
    {
        // Only MinValue's "start of year" minus one year underflows
        DateTimeOffset result = DateTimeOffset.MinValue.ToStartOfPreviousTzYear(TimeZoneInfo.Utc);
        result.Should().Be(DateTimeOffset.MinValue);
    }

    [Test]
    public void ToStartOfNextTzYear_at_MaxValue_returns_MaxValue()
    {
        DateTimeOffset result = DateTimeOffset.MaxValue.ToStartOfNextTzYear(TimeZoneInfo.Utc);
        result.Should().Be(DateTimeOffset.MaxValue);
    }

    [Test]
    public void ToEndOfTzYear_at_MaxValue_returns_last_tick_before_overflow()
    {
        DateTimeOffset result = DateTimeOffset.MaxValue.ToEndOfTzYear(TimeZoneInfo.Utc);
        result.Ticks.Should().Be(DateTimeOffset.MaxValue.Ticks - 1);
    }

    [Test]
    public void ToEndOfPreviousTzYear_at_MinValue_returns_MinValue()
    {
        // Only when current TZ year start is MinValue does subtracting one tick underflow
        DateTimeOffset result = DateTimeOffset.MinValue.ToEndOfPreviousTzYear(TimeZoneInfo.Utc);
        result.Should().Be(DateTimeOffset.MinValue);
    }

    [Test]
    public void ToEndOfPreviousTzYear_at_start_of_year_two_returns_last_tick_of_year_one()
    {
        var startOfYearTwo = new DateTimeOffset(2, 1, 1, 0, 0, 0, TimeSpan.Zero);
        DateTimeOffset result = startOfYearTwo.ToEndOfPreviousTzYear(TimeZoneInfo.Utc);
        result.Year.Should().Be(1);
        result.Month.Should().Be(12);
        result.Day.Should().Be(31);
        result.Hour.Should().Be(23);
        result.Minute.Should().Be(59);
    }

    [Test]
    public void Dec_31_last_moment_ToEndOfYear_equals_that_instant()
    {
        // 23:59:59.999 = 9990000 ticks in fraction; add 9999 to get 23:59:59.9999999 (last tick of second)
        var lastTickOf2024 = new DateTimeOffset(2024, 12, 31, 23, 59, 59, 999, TimeSpan.Zero).AddTicks(9999);
        DateTimeOffset endOfYear = lastTickOf2024.ToEndOfYear();

        endOfYear.Ticks.Should().Be(lastTickOf2024.Ticks);
        endOfYear.Offset.Should().Be(lastTickOf2024.Offset);
    }
}
