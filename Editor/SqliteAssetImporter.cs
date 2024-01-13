using System.IO;
using SQLite;
#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif
using UnityEngine;

namespace Gilzoide.SqliteNet.Editor
{
    [ScriptedImporter(0, "db")]
    public class SqliteAssetImporter : ScriptedImporter
    {
        [Tooltip("Flags controlling how the SQLite connection should be opened. 'ReadWrite' flag will be ignored, since SQLite assets are read-only.")]
        [SerializeField] internal SQLiteOpenFlags _openFlags = SQLiteOpenFlags.ReadOnly;

        [Tooltip("Whether to store DateTime properties as ticks (true) or strings (false).")]
        [SerializeField] internal bool _storeDateTimeAsTicks = true;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var asset = ScriptableObject.CreateInstance<SqliteAsset>();
            asset.OpenFlags = _openFlags;
            asset.StoreDateTimeAsTicks = _storeDateTimeAsTicks;
            asset.Bytes = File.ReadAllBytes(ctx.assetPath);
            ctx.AddObjectToAsset("main", asset);
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            if (_openFlags.HasFlag(SQLiteOpenFlags.ReadWrite))
            {
                Debug.LogWarning("SQLiteAsset does not support writing. Ignoring \"ReadWrite\" flag.", this);
                _openFlags &= ~SQLiteOpenFlags.ReadWrite;
            }
        }
#endif
    }
}
