using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gilzoide.SqliteAsset
{
    public static class CsvReader
    {
        public enum SeparatorChar
        {
            Comma,
            Semicolon,
            Tabs,
        }

        public static IEnumerable<string> ParseStream(StreamReader stream, SeparatorChar separator = SeparatorChar.Comma, int maxFieldSize = int.MaxValue)
        {
            bool insideQuotes = false;
            var stringBuilder = new StringBuilder();
            while (true)
            {
                int c = stream.Read();
                switch (c)
                {
                    case '\r':
                        if (!insideQuotes && stream.Peek() == '\n')
                        {
                            stream.Read();
                            goto case '\n';
                        }
                        else
                        {
                            goto default;
                        }

                    case '\n':
                        if (!insideQuotes)
                        {
                            yield return stringBuilder.ToString();
                            stringBuilder.Clear();
                            yield return null;
                        }
                        else
                        {
                            goto default;
                        }
                        break;

                    case ',':
                        if (!insideQuotes && separator == SeparatorChar.Comma)
                        {
                            yield return stringBuilder.ToString();
                            stringBuilder.Clear();
                        }
                        else
                        {
                            goto default;
                        }
                        break;

                    case ';':
                        if (!insideQuotes && separator == SeparatorChar.Semicolon)
                        {
                            yield return stringBuilder.ToString();
                            stringBuilder.Clear();
                        }
                        else
                        {
                            goto default;
                        }
                        break;

                    case '\t':
                        if (!insideQuotes && separator == SeparatorChar.Tabs)
                        {
                            yield return stringBuilder.ToString();
                            stringBuilder.Clear();
                        }
                        else
                        {
                            goto default;
                        }
                        break;

                    case '"':
                        if (insideQuotes && stream.Peek() == '"')
                        {
                            stream.Read();
                            goto default;
                        }
                        else
                        {
                            insideQuotes = !insideQuotes;
                        }
                        break;

                    case < 0:
                        yield return stringBuilder.ToString();
                        yield return null;
                        yield break;

                    default:
                        if (stringBuilder.Length >= maxFieldSize)
                        {
                            throw new CsvException("Field size is greater than maximum allowed size.");
                        }
                        stringBuilder.Append((char) c);
                        break;
                }
            }
        }
    }
}
