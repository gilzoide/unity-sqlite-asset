using System.IO;
using SQLite;
#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif
using UnityEngine;

namespace Gilzoide.SqliteAsset.Editor
{
    [ScriptedImporter(0, null, new[] { "csv" })]
    public class SqliteAssetCsvImporter : ScriptedImporter
    {
        [Header("SQLite asset options")]
        [Tooltip("Name of the table that will be created for holding the CSV data inside the database.")]
        [SerializeField] private string _tableName = "data";

        [Tooltip("Flags controlling how the SQLite connection should be opened. 'ReadWrite' and 'Create' flags will be ignored, since SQLite assets are read-only.")]
        [SerializeField] internal SQLiteOpenFlags _openFlags = SQLiteOpenFlags.ReadOnly;

        [Tooltip("Whether to store DateTime properties as ticks (true) or strings (false).")]
        [SerializeField] internal bool _storeDateTimeAsTicks = true;


        [Header("CSV options")]
        [Tooltip("Which separator character will be used when parsing the CSV file.")]
        [SerializeField] private CsvReader.SeparatorChar _csvSeparator = CsvReader.SeparatorChar.Comma;

        [Tooltip("If true, the original CSV file will also be imported as a TextAsset")]
        [SerializeField] private bool _importCsvTextAsset = false;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            byte[] bytes;
            using (var tempDb = new SQLiteConnection(""))
            using (var file = File.OpenRead(assetPath))
            using (var stream = new StreamReader(file))
            {
                tempDb.ImportCsvToTable(_tableName, stream, _csvSeparator);
                bytes = tempDb.Serialize();
            }

            SqliteAsset asset = ScriptableObject.CreateInstance<SqliteAsset>();
            asset.Bytes = bytes;
            asset.OpenFlags = _openFlags;
            asset.StoreDateTimeAsTicks = _storeDateTimeAsTicks;
            ctx.AddObjectToAsset("sqlite", asset);
            ctx.SetMainObject(asset);

            if (_importCsvTextAsset)
            {
                var textAsset = new TextAsset(File.ReadAllText(assetPath))
                {
                    name = $"{Path.GetFileNameWithoutExtension(assetPath)}",
                };
                ctx.AddObjectToAsset("text", textAsset);
            }
        }
    }
}
