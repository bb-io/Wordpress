using Apps.Wordpress.Constants;

namespace Apps.Wordpress.Extensions;

public static class DateExtensions
{
    public static string GetPastHoursDate(this int? hours)
    {
        if (hours is null)
            return null;

        var minusHours = (-1 * hours).Value;
        return DateTime.UtcNow.AddHours(minusHours).ToString(Formats.ISO8601);
    }
}