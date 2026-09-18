namespace FastColoredTextBoxNS.Text
{
    /// <summary>
    /// Language
    /// </summary>
    public enum Language
    {
        Custom,
        CSharp,
        VB,
        HTML,
        XML,
        SQL,
        PHP,
        JS,
        Lua,
        JSON,
        Markdown
    }

    public static class LanguageDetector
    {
        /// <summary>
        /// Converts a string like "lua" or "csharp" to a Language
        /// </summary>
        public static Language StringToLanguage(string language)
        {
            return language.Trim().ToLowerInvariant() switch
            {
                "lua" => Language.Lua,
                "html" => Language.HTML,
                "xml" => Language.XML,
                "sql" => Language.SQL,
                "vb" => Language.VB,
                "cs" => Language.CSharp,
                "csharp" => Language.CSharp,
                "java" => Language.CSharp,
                "js" => Language.JS,
                "json" => Language.JSON,
                "php" => Language.PHP,
                "markdown" => Language.Markdown,
                "md" => Language.Markdown,
                _ => Language.Custom,
            };
        }
    }
}
