using System;
using System.Runtime.InteropServices;
using SQLite;
using Unity.Collections.LowLevel.Unsafe;

namespace Gilzoide.SqliteAsset
{
    public unsafe class SQLiteConnectionMemory : SQLiteConnection
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        public const string MemVfsLibraryPath = "__Internal";
#else
        public const string MemVfsLibraryPath = "gilzoide-sqlite-asset";
#endif

        [DllImport(SQLite3.LibraryPath)]
        private static extern SQLite3.Result sqlite3_auto_extension(IntPtr extensionFun);

        [DllImport(SQLite3.LibraryPath)]
        private static extern int sqlite3_cancel_auto_extension(IntPtr extensionFun);

        [DllImport(MemVfsLibraryPath)]
        private static extern IntPtr sqlite3_memvfs_get_init();

        static SQLiteConnectionMemory()
        {
            // Workaround to init "memvfs" extension using automatic extensions.
            // This way we can rely on "DllImport" finding the right library
            // instead of specifying relative paths for each platform.
            IntPtr memVfsEntrypoint = sqlite3_memvfs_get_init();
            SQLite3.Result result = sqlite3_auto_extension(memVfsEntrypoint);
            if (result != SQLite3.Result.OK)
            {
                throw SQLiteException.New(result, $"Error registering \"memvfs\" autoinit: {result}");
            }
            // Load "memvfs" using auto extension initialization
            new SQLiteConnection(":memory:").Dispose();
            if (sqlite3_cancel_auto_extension(memVfsEntrypoint) == 0)
            {
                throw SQLiteException.New(result, "Error unregistering \"memvfs\" autoinit");
            }
        }

        private ulong _memoryGcHandle;

        public SQLiteConnectionMemory(byte[] bytes, bool storeDateTimeAsTicks = true)
            : this(bytes, SQLiteOpenFlags.ReadOnly, storeDateTimeAsTicks)
        {
        }

        public SQLiteConnectionMemory(byte[] bytes, SQLiteOpenFlags openFlags, bool storeDateTimeAsTicks = true)
            : this(bytes, bytes.Length, openFlags, storeDateTimeAsTicks)
        {
        }

        public SQLiteConnectionMemory(byte[] bytes, int currentSize, SQLiteOpenFlags openFlags, bool storeDateTimeAsTicks = true)
            : this((IntPtr) UnsafeUtility.PinGCArrayAndGetDataAddress(bytes, out ulong memoryGcHandle), currentSize, bytes.Length, openFlags, storeDateTimeAsTicks)
        {
            _memoryGcHandle = memoryGcHandle;
        }

        private SQLiteConnectionMemory(IntPtr memory, int size, int maxSize, SQLiteOpenFlags openFlags, bool storeDateTimeAsTicks = true)
            : base(new SQLiteConnectionString($"file:name?ptr=0x{(ulong)memory:X}&sz={size}&max={maxSize}", openFlags, storeDateTimeAsTicks, vfsName: "memvfs"))
        {
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_memoryGcHandle != 0)
            {
                UnsafeUtility.ReleaseGCObject(_memoryGcHandle);
                _memoryGcHandle = 0;
            }
        }
    }
}
