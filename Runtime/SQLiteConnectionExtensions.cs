using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using SQLite;

namespace Gilzoide.SqliteAsset
{
    public static class SQLiteConnectionExtensions
    {
        [DllImport(SQLite3.LibraryPath)]
        private static extern IntPtr sqlite3_serialize(IntPtr db, [MarshalAs(UnmanagedType.LPStr)] string zSchema, out long piSize, int mFlags);

        [DllImport(SQLite3.LibraryPath)]
        private static extern void sqlite3_free(IntPtr extensionFun);

        public static void ImportCsvToTable(this SQLiteConnection db, string tableName, StreamReader csvStream, CsvReader.SeparatorChar separator = CsvReader.SeparatorChar.Comma, int maxFieldSize = int.MaxValue)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }
            if (csvStream == null)
            {
                throw new ArgumentNullException(nameof(csvStream));
            }

            var columns = new List<string>();
            bool parsingHeader = true;
            db.RunInTransaction(() =>
            {
                foreach (string field in CsvReader.ParseStream(csvStream, separator, maxFieldSize))
                {
                    if (field == null)  // newline
                    {
                        string joinedColumns = string.Join(", ", columns);
                        if (parsingHeader)
                        {
                            db.Execute($"CREATE TABLE {tableName} ({joinedColumns})");
                            parsingHeader = false;
                        }
                        else
                        {
                            db.Execute($"INSERT INTO {tableName} VALUES ({joinedColumns})");
                        }
                        columns.Clear();
                    }
                    else
                    {
                        if (parsingHeader && string.IsNullOrWhiteSpace(field))
                        {
                            throw new CsvException("Header cannot have empty column name.");
                        }

                        columns.Add(SQLiteConnection.Quote(field));
                    }
                }
            });
        }

        public static byte[] Serialize(this SQLiteConnection db, string schema = null)
        {
            IntPtr bytes = sqlite3_serialize(db.Handle, schema, out long size, 0);
            try
            {
                var value = new byte[size];
                Marshal.Copy(bytes, value, 0, (int) size);
                return value;
            }
            finally
            {
                sqlite3_free(bytes);
            }
        }
    }
}
