using System;
using System.Runtime.InteropServices;
using SQLite;

namespace Gilzoide.SqliteNet
{
    public class SQLiteConnectionMemory : SQLiteConnection
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        public const string LibraryPath = "__Internal";
#else
        public const string LibraryPath = "sqlite3memvfs";
#endif

        [DllImport(LibraryPath, EntryPoint = "sqlite3_memvfs_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern SQLite3.Result sqlite3_memvfs_init(IntPtr _, out IntPtr errorMessage, IntPtr sqliteApi);

        static SQLiteConnectionMemory()
        {
            SQLite3.Result result = sqlite3_memvfs_init(IntPtr.Zero, out _, IntPtr.Zero);
            if (result != SQLite3.Result.OK)
            {
                throw SQLiteException.New(result, "Failed initializing memvfs");
            }
        }

        private GCHandle _memoryHandle;

        public SQLiteConnectionMemory(byte[] bytes, bool storeDateTimeAsTicks = true)
            : this(bytes, SQLiteOpenFlags.ReadOnly, storeDateTimeAsTicks)
        {
        }

        public SQLiteConnectionMemory(byte[] bytes, SQLiteOpenFlags openFlags, bool storeDateTimeAsTicks = true)
            : this(bytes, bytes.Length, openFlags, storeDateTimeAsTicks)
        {
        }

        public SQLiteConnectionMemory(byte[] bytes, int currentSize, SQLiteOpenFlags openFlags, bool storeDateTimeAsTicks = true)
            : this(GCHandle.Alloc(bytes, GCHandleType.Pinned), currentSize, bytes.Length, openFlags, storeDateTimeAsTicks)
        {
        }

        public SQLiteConnectionMemory(GCHandle memoryHandle, int size, int maxSize, SQLiteOpenFlags openFlags, bool storeDateTimeAsTicks = true)
            : this(memoryHandle.AddrOfPinnedObject(), size, maxSize, openFlags, storeDateTimeAsTicks)
        {
            _memoryHandle = memoryHandle;
        }

        public SQLiteConnectionMemory(IntPtr memory, int size, int maxSize, SQLiteOpenFlags openFlags, bool storeDateTimeAsTicks = true)
            : base($"file:?ptr=0x{memory:X}&sz={size}&maxsz={maxSize}&vfs=memvfs", openFlags, storeDateTimeAsTicks)
        {
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_memoryHandle.IsAllocated)
            {
                _memoryHandle.Free();
            }
        }
    }
}
