namespace Avn.Shared.Utilities;

public static class RandomStringGenerator
{
    private static readonly Random random = new();

    public static string Generate(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
