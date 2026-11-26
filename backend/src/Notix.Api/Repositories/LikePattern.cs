namespace Notix.Api.Repositories;

// Builds a LIKE pattern from user input: %, _ and the escape character itself are matched
// literally. The pattern is always sent as a SQL parameter, never concatenated into SQL.
public static class LikePattern
{
    public const string EscapeCharacter = "\\";

    public static string Contains(string term) => $"%{Escape(term)}%";

    public static string Escape(string term) => term
        .Replace(EscapeCharacter, EscapeCharacter + EscapeCharacter)
        .Replace("%", EscapeCharacter + "%")
        .Replace("_", EscapeCharacter + "_");
}
