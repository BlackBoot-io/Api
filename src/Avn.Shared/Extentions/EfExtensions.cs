namespace Avn.Shared.Extentions;
public static class EfExtensions
{
    public static bool ToSaveChangeResult(this int result) => result >= 1;
}
