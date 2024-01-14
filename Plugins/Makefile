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
lib/windows/%/gilzoide-sqlite-asset.dll: LINKFLAGS += -shared
lib/windows/%/gilzoide-sqlite-asset.dll: $(MEMVFS_SRC) | lib/windows/%
	$(CC) -o $@ $^ $(CFLAGS) $(LINKFLAGS)

# Linux
lib/linux/%/gilzoide-sqlite-asset.so: CFLAGS += -fPIC
lib/linux/%/gilzoide-sqlite-asset.so: LINKFLAGS += -shared
lib/linux/%/gilzoide-sqlite-asset.so: $(MEMVFS_SRC) | lib/linux/%
	$(CC) -o $@ $^ $(CFLAGS) $(LINKFLAGS)

# macOS
lib/macos/gilzoide-sqlite-asset.dylib: CFLAGS += -arch arm64 -arch x86_64 -isysroot $(shell xcrun --show-sdk-path --sdk macosx)
lib/macos/gilzoide-sqlite-asset.dylib: LINKFLAGS += -shared
lib/macos/gilzoide-sqlite-asset.dylib: $(MEMVFS_SRC) | lib/macos
	$(CC) -o $@ $^ $(CFLAGS) $(LINKFLAGS)

# iOS
lib/ios/gilzoide-sqlite-asset.dylib: CFLAGS += -arch arm64 -isysroot $(shell xcrun --show-sdk-path --sdk iphoneos)
lib/ios/gilzoide-sqlite-asset.dylib: LINKFLAGS += -shared
lib/ios/gilzoide-sqlite-asset.dylib: $(MEMVFS_SRC) | lib/ios
	$(CC) -o $@ $^ $(CFLAGS) $(LINKFLAGS)

# tvOS
lib/tvos/gilzoide-sqlite-asset.dylib: CFLAGS += -arch arm64 -isysroot $(shell xcrun --show-sdk-path --sdk appletvos)
lib/tvos/gilzoide-sqlite-asset.dylib: LINKFLAGS += -shared
lib/tvos/gilzoide-sqlite-asset.dylib: $(MEMVFS_SRC) | lib/tvos
	$(CC) -o $@ $^ $(CFLAGS) $(LINKFLAGS)

# visionOS
lib/visionos/gilzoide-sqlite-asset.dylib: CFLAGS += -arch arm64 -isysroot $(shell xcrun --show-sdk-path --sdk xros)
lib/visionos/gilzoide-sqlite-asset.dylib: LINKFLAGS += -shared
lib/visionos/gilzoide-sqlite-asset.dylib: $(MEMVFS_SRC) | lib/visionos
	$(CC) -o $@ $^ $(CFLAGS) $(LINKFLAGS)

# Android
lib/android/%/gilzoide-sqlite-asset.so: CFLAGS += -fPIC
lib/android/%/gilzoide-sqlite-asset.so: LINKFLAGS += -shared
lib/android/%/gilzoide-sqlite-asset.so: $(MEMVFS_SRC) | lib/android/%
	$(CC) -o $@ $^ $(CFLAGS) $(LINKFLAGS)

lib/android/arm64/gilzoide-sqlite-asset.so: CC = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/aarch64-linux-android21-clang)
lib/android/arm32/gilzoide-sqlite-asset.so: CC = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/armv7a-linux-androideabi19-clang)
lib/android/x86_64/gilzoide-sqlite-asset.so: CC = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/x86_64-linux-android21-clang)
lib/android/x86/gilzoide-sqlite-asset.so: CC = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/i686-linux-android19-clang)

# WebGL
lib/webgl/gilzoide-sqlite-asset.bc: $(MEMVFS_SRC) | lib/webgl
	$(EMCC) -o $@ $< $(CFLAGS) -c -emit-llvm


# Targets
windows-x86_64: lib/windows/x86_64/gilzoide-sqlite-asset.dll
windows-x86: lib/windows/x86/gilzoide-sqlite-asset.dll

windows-mingw-x86_64: CC = x86_64-w64-mingw32-gcc
windows-mingw-x86_64: LINKFLAGS += -static-libgcc
windows-mingw-x86_64: lib/windows/x86_64/gilzoide-sqlite-asset.dll

windows-mingw-x86: CC = i686-w64-mingw32-gcc
windows-mingw-x86: LINKFLAGS += -static-libgcc
windows-mingw-x86: lib/windows/x86/gilzoide-sqlite-asset.dll

linux-x86_64: lib/linux/x86_64/gilzoide-sqlite-asset.so

macos-universal: lib/macos/gilzoide-sqlite-asset.dylib
ios-universal: lib/ios/gilzoide-sqlite-asset.dylib
tvos-universal: lib/tvos/gilzoide-sqlite-asset.dylib
visionos-universal: lib/visionos/gilzoide-sqlite-asset.dylib

android-arm64: lib/android/arm64/gilzoide-sqlite-asset.so
android-arm32: lib/android/arm32/gilzoide-sqlite-asset.so
android-x86_64: lib/android/x86_64/gilzoide-sqlite-asset.so
android-x86: lib/android/x86/gilzoide-sqlite-asset.so

webgl: lib/webgl/gilzoide-sqlite-asset.bc

all-android: android-arm64 android-arm32 android-x86_64 android-x86
all-apple: macos-universal ios-universal tvos-universal visionos-universal
all-linux: linux-x86_64
all-webgl: webgl
all-windows: windows-x86_64 windows-x86
all-windows-mingw: windows-mingw-x86_64 windows-mingw-x86
