using System;
using System.Runtime.InteropServices;
using SQLite;
using Unity.Collections.LowLevel.Unsafe;

namespace Gilzoide.SqliteNet
{
    public unsafe class SQLiteConnectionMemory : SQLiteConnection
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        public const string MemVfsLibraryPath = "__Internal";
#else
        public const string MemVfsLibraryPath = "sqlite3memvfs";
#endif

        [DllImport(SQLite3.LibraryPath)]
        private static extern SQLite3.Result sqlite3_auto_extension(IntPtr extensionFun);

        [DllImport(MemVfsLibraryPath)]
        private static extern IntPtr sqlite3_memvfs_get_init();

        static SQLiteConnectionMemory()
        {
            SQLite3.Result result = sqlite3_auto_extension(sqlite3_memvfs_get_init());
            UnityEngine.Debug.Log(result);
            if (result != SQLite3.Result.OK)
            {
                throw SQLiteException.New(result, SQLite3.GetErrmsg(IntPtr.Zero));
            }
            new SQLiteConnection(":memory:").Dispose();
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
