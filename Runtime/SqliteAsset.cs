using System;
using SQLite;
using UnityEngine;

namespace Gilzoide.SqliteAsset
{
    public class SqliteAsset : ScriptableObject
    {
        [Tooltip("Flags controlling how the SQLite connection should be opened. 'ReadWrite' and 'Create' flags will be ignored, since SQLite assets are read-only.")]
        [SerializeField] internal SQLiteOpenFlags _openFlags = SQLiteOpenFlags.ReadOnly;

        [Tooltip("Whether to store DateTime properties as ticks (true) or strings (false).")]
        [SerializeField] internal bool _storeDateTimeAsTicks = true;

        [SerializeField, HideInInspector] internal byte[] _bytes;

        /// <summary>
        /// Flags controlling how the SQLite connection should be opened.
        /// </summary>
        /// <remarks>
        /// 'ReadWrite' flag will be ignored, since SQLite assets are read-only.
        /// </remarks>
        public SQLiteOpenFlags OpenFlags
        {
            get => _openFlags;
            set => _openFlags = value & ~(SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        }

        /// <summary>
        /// Whether to store DateTime properties as ticks (true) or strings (false).
        /// </summary>
        /// <seealso cref="SQLiteConnectionString.StoreDateTimeAsTicks"/>
        public bool StoreDateTimeAsTicks
        {
            get => _storeDateTimeAsTicks;
            set => _storeDateTimeAsTicks = value;
        }

        /// <summary>
        /// Bytes that compose the SQLite database file.
        /// </summary>
        public byte[] Bytes
        {
            get => _bytes;
            set => _bytes = value;
        }

        /// <summary>
        /// Creates a new connection to the read-only SQLite database represented by this asset.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">If <see cref="Bytes"/> is null.</exception>
        public SQLiteConnectionMemory CreateConnection()
        {
            if (_bytes == null)
            {
                throw new NullReferenceException(nameof(Bytes));
            }

            return new SQLiteConnectionMemory(_bytes, _openFlags, _storeDateTimeAsTicks);
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            if (_openFlags.HasFlag(SQLiteOpenFlags.ReadWrite))
            {
                Debug.LogWarning("SQLiteAsset does not support writing. Ignoring \"ReadWrite\" flag.", this);
                _openFlags &= ~SQLiteOpenFlags.ReadWrite;
            }
            if (_openFlags.HasFlag(SQLiteOpenFlags.Create))
            {
                Debug.LogWarning("SQLiteAsset does not support creating database. Ignoring \"Create\" flag.", this);
                _openFlags &= ~SQLiteOpenFlags.Create;
            }
        }
#endif
    }
}
