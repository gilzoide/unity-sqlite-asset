EMCC ?= emcc
ANDROID_NDK_ROOT ?=

ifeq ($(DEBUG),1)
	CFLAGS += -O0 -g -DDEBUG
else
	CFLAGS += -O3 -DNDEBUG
endif

BUILD_DIRS = \
	lib/windows/x86_64 lib/windows/x86 \
	lib/linux/x86_64 lib/linux/x86 \
	lib/macos lib/ios lib/tvos lib/visionos \
	lib/android/arm64 lib/android/arm32 lib/android/x86 lib/android/x86_64 \
	lib/webgl

MEMVFS_SRC = vfs~/memvfs.c
CFLAGS += -Isqlite-amalgamation-3440200~

$(BUILD_DIRS):
	mkdir -p $@

# Windows
lib/windows/%/sqlite3memvfs.dll: LINKFLAGS += -shared
lib/windows/%/sqlite3memvfs.dll: $(MEMVFS_SRC) | lib/windows/%
	$(CC) -o $@ $^ $(CFLAGS) $(LINKFLAGS)

# Linux
lib/linux/%/libsqlite3memvfs.so: CFLAGS += -fPIC
lib/linux/%/libsqlite3memvfs.so: LINKFLAGS += -shared
lib/linux/%/libsqlite3memvfs.so: $(MEMVFS_SRC) | lib/linux/%
	$(CC) -o $@ $^ $(CFLAGS) $(LINKFLAGS)

# macOS
lib/macos/libsqlite3memvfs.dylib: CFLAGS += -arch arm64 -arch x86_64 -isysroot $(shell xcrun --show-sdk-path --sdk macosx)
lib/macos/libsqlite3memvfs.dylib: LINKFLAGS += -shared
lib/macos/libsqlite3memvfs.dylib: $(MEMVFS_SRC) | lib/macos
	$(CC) -o $@ $^ $(CFLAGS) $(LINKFLAGS)

# iOS
lib/ios/libsqlite3memvfs.dylib: CFLAGS += -arch arm64 -isysroot $(shell xcrun --show-sdk-path --sdk iphoneos)
lib/ios/libsqlite3memvfs.dylib: LINKFLAGS += -shared
lib/ios/libsqlite3memvfs.dylib: $(MEMVFS_SRC) | lib/ios
	$(CC) -o $@ $^ $(CFLAGS) $(LINKFLAGS)

# tvOS
lib/tvos/libsqlite3memvfs.dylib: CFLAGS += -arch arm64 -isysroot $(shell xcrun --show-sdk-path --sdk appletvos)
lib/tvos/libsqlite3memvfs.dylib: LINKFLAGS += -shared
lib/tvos/libsqlite3memvfs.dylib: $(MEMVFS_SRC) | lib/tvos
	$(CC) -o $@ $^ $(CFLAGS) $(LINKFLAGS)

# visionOS
lib/visionos/libsqlite3memvfs.dylib: CFLAGS += -arch arm64 -isysroot $(shell xcrun --show-sdk-path --sdk xros)
lib/visionos/libsqlite3memvfs.dylib: LINKFLAGS += -shared
lib/visionos/libsqlite3memvfs.dylib: $(MEMVFS_SRC) | lib/visionos
	$(CC) -o $@ $^ $(CFLAGS) $(LINKFLAGS)

# Android
lib/android/%/libsqlite3memvfs.so: CFLAGS += -fPIC
lib/android/%/libsqlite3memvfs.so: LINKFLAGS += -shared
lib/android/%/libsqlite3memvfs.so: $(MEMVFS_SRC) | lib/android/%
	$(CC) -o $@ $^ $(CFLAGS) $(LINKFLAGS)

lib/android/arm64/libsqlite3memvfs.so: CC = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/aarch64-linux-android21-clang)
lib/android/arm32/libsqlite3memvfs.so: CC = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/armv7a-linux-androideabi19-clang)
lib/android/x86_64/libsqlite3memvfs.so: CC = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/x86_64-linux-android21-clang)
lib/android/x86/libsqlite3memvfs.so: CC = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/i686-linux-android19-clang)

# WebGL
lib/webgl/libsqlite3memvfs.bc: $(MEMVFS_SRC) | lib/webgl
	$(EMCC) -o $@ $< $(CFLAGS) -c -emit-llvm


# Targets
windows-x86_64: lib/windows/x86_64/sqlite3memvfs.dll
windows-x86: lib/windows/x86/sqlite3memvfs.dll

windows-mingw-x86_64: CC = x86_64-w64-mingw32-gcc
windows-mingw-x86_64: LINKFLAGS += -static-libgcc
windows-mingw-x86_64: lib/windows/x86_64/sqlite3memvfs.dll

windows-mingw-x86: CC = i686-w64-mingw32-gcc
windows-mingw-x86: LINKFLAGS += -static-libgcc
windows-mingw-x86: lib/windows/x86/sqlite3memvfs.dll

linux-x86_64: lib/linux/x86_64/libsqlite3memvfs.so

macos-universal: lib/macos/libsqlite3memvfs.dylib
ios-universal: lib/ios/libsqlite3memvfs.dylib
tvos-universal: lib/tvos/libsqlite3memvfs.dylib
visionos-universal: lib/visionos/libsqlite3memvfs.dylib

android-arm64: lib/android/arm64/libsqlite3memvfs.so
android-arm32: lib/android/arm32/libsqlite3memvfs.so
android-x86_64: lib/android/x86_64/libsqlite3memvfs.so
android-x86: lib/android/x86/libsqlite3memvfs.so

webgl: lib/webgl/libsqlite3memvfs.bc

all-android: android-arm64 android-arm32 android-x86_64 android-x86
all-apple: macos-universal ios-universal tvos-universal visionos-universal
all-linux: linux-x86_64
all-webgl: webgl
all-windows: windows-x86_64 windows-x86
all-windows-mingw: windows-mingw-x86_64 windows-mingw-x86
