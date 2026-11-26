namespace Notix.Api.Repositories.Common;

public static class LikePattern
{
    public const string EscapeCharacter = "\\";

    public static string Contains(string term) => $"%{Escape(term)}%";

    public static string Escape(string term) => term
        .Replace(EscapeCharacter, EscapeCharacter + EscapeCharacter)
        .Replace("%", EscapeCharacter + "%")
        .Replace("_", EscapeCharacter + "_");
}
