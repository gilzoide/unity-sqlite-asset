using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using SQLite;
using UnityEngine;

namespace Gilzoide.SqliteAsset
{
    public static class SQLiteConnectionExtensions
    {
        public const int SQLITE_SERIALIZE_NOCOPY = 0x001;  /* Do no memory allocations */

        public const int SQLITE_DESERIALIZE_FREEONCLOSE = 1;  /* Call sqlite3_free() on close */
        public const int SQLITE_DESERIALIZE_RESIZEABLE  = 2;  /* Resize using sqlite3_realloc64() */
        public const int SQLITE_DESERIALIZE_READONLY    = 4;  /* Database is read-only */

        [DllImport(SQLite3.LibraryPath)]
        private static extern IntPtr sqlite3_serialize(
            IntPtr db,  /* The database connection */
            [MarshalAs(UnmanagedType.LPStr)] string zSchema,  /* Which DB to serialize. ex: "main", "temp", ... */
            out long piSize,  /* Write size of the DB here, if not NULL */
            int mFlags  /* Zero or more SQLITE_SERIALIZE_* flags */
        );

        [DllImport(SQLite3.LibraryPath)]
        private static extern SQLite3.Result sqlite3_deserialize(
            IntPtr db,  /* The database connection */
            [MarshalAs(UnmanagedType.LPStr)] string zSchema,  /* Which DB to reopen with the deserialization */
            byte[] pData,  /* The serialized database content */
            long szDb,  /* Number bytes in the deserialization */
            long szBuf,  /* Total size of buffer pData[] */
            int mFlags  /* Zero or more SQLITE_DESERIALIZE_* flags */
        );

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

        public static SqliteAsset SerializeToAsset(this SQLiteConnection db, SQLiteOpenFlags openFlags = SQLiteOpenFlags.ReadOnly, bool storeDateTimeAsTicks = true, string streamingAssetsPath = null)
        {
            var asset = ScriptableObject.CreateInstance<SqliteAsset>();
            asset.Bytes = db.Serialize();
            asset.OpenFlags = openFlags;
            asset.StoreDateTimeAsTicks = storeDateTimeAsTicks;
            asset.StreamingAssetsPath = streamingAssetsPath;
            return asset;
        }

        public static SQLiteConnection Deserialize(this SQLiteConnection db, byte[] buffer, string schema = null, int flags = SQLITE_DESERIALIZE_READONLY)
        {
            return Deserialize(db, buffer, buffer.Length, schema, flags);
        }

        public static SQLiteConnection Deserialize(this SQLiteConnection db, byte[] buffer, int usedSize, string schema = null, int flags = SQLITE_DESERIALIZE_READONLY)
        {
            SQLite3.Result result = sqlite3_deserialize(db.Handle, schema, buffer, usedSize, buffer.LongLength, flags);
            if (result != SQLite3.Result.OK)
            {
                throw SQLiteException.New(result, SQLite3.GetErrmsg(db.Handle));
            }
            return db;
        }
    }
}
