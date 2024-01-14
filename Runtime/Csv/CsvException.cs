using System;

namespace Gilzoide.SqliteAsset
{
    public class CsvException : Exception
    {
        public CsvException(string message) : base(message) {}
    }
}
