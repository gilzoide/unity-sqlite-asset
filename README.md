# SQLite Asset
Read-only [SQLite](https://sqlite.org/) database assets for Unity.

Provides a ScriptedImporter for SQLite database assets with the ".db" extension.


## Features
- [SqliteAsset](Runtime/SqliteAsset.cs): read-only SQLite database Unity assets.
  + Files with the ".db" extension will be loaded as `SqliteAsset`s.
  + Use the `CreateConnection()` method for connecting to the database provided by the asset.
    Make sure to `Dispose()` of any connections you create.
  + CSV files may be optionally imported as SQLite database assets, just change the importer to `Gilzoide.SqliteAsset.Editor.SqliteAssetCsvImporter` in the Inspector.
- [SQLiteConnectionMemory](Runtime/SQLiteConnectionMemory.cs): custom `SQLiteConnection` subclass that loads a SQLite database from memory (`byte[]`) instead of from file path.
- `SQLiteConnection.Serialize` extension method for serializing a database to `byte[]`.
- `SQLiteConnection.ImportCsvToTable` extension method for importing a CSV data stream to a new table inside the database.
- Supported platforms: Windows, macOS, iOS, tvOS, visionOS, Android and WebGL


## Dependencies
- [SQLite-net](https://github.com/gilzoide/unity-sqlite-net): library for managing SQLite databases


## How to install
Either:
- Install using the [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html) with the following URL:
  ```
  https://github.com/gilzoide/unity-sqlite-net.git#1.0.0-preview2
  ```
- Clone this repository or download a snapshot of it directly inside your project's `Assets` or `Packages` folder.