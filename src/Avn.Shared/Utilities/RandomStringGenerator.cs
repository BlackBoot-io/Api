using System.Security.Cryptography;

namespace Avn.Shared.Utilities;

public static class RandomStringGenerator
{
    private static readonly Random random = new();

#pragma warning disable CA1041 // Please Use Generate method to be secure
    [Obsolete]
#pragma warning restore CA1041 // Please Use Generate method to be secure
    public static string GenerateUnsecure(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static string Generate(int length)
    {
        var rng = RandomNumberGenerator.Create();
        byte[] rno = new byte[length];
        rng.GetBytes(rno);
        return Convert.ToBase64String(rno);
    }
}
