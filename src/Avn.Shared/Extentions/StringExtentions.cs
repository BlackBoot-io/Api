namespace Avn.Shared.Extentions;

public static class StringExtentions
{
    public static bool HasValue(this string value, bool ignoreWhiteSpace = true)
        => ignoreWhiteSpace ? !string.IsNullOrWhiteSpace(value) : !string.IsNullOrEmpty(value);
}