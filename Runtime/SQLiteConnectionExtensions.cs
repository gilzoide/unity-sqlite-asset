using SQLite;
using UnityEngine;

namespace Gilzoide.SqliteAsset
{
    public static class SQLiteConnectionExtensions
    {
        public static SqliteAsset SerializeToAsset(this SQLiteConnection db, SQLiteOpenFlags openFlags = SQLiteOpenFlags.ReadOnly, bool storeDateTimeAsTicks = true, string streamingAssetsPath = null)
        {
            var asset = ScriptableObject.CreateInstance<SqliteAsset>();
            asset.Bytes = db.Serialize();
            asset.OpenFlags = openFlags;
            asset.StoreDateTimeAsTicks = storeDateTimeAsTicks;
            asset.StreamingAssetsPath = streamingAssetsPath;
            return asset;
        }
    }
}
