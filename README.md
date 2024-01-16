# SQLite Asset
Read-only [SQLite](https://sqlite.org/) database assets for Unity.

Automatically imports ".db", ".sqlite", ".sqlite2" and ".sqlite3" files as SQLite database assets.
Optionally, you can import ".csv" files as SQLite database assets.


## Features
- [SqliteAsset](Runtime/SqliteAsset.cs): read-only SQLite database Unity assets.
  + Files with the extensions ".db", ".sqlite", ".sqlite2" and ".sqlite3" will be loaded as SQLite database assets.
  + Use the `CreateConnection()` method for connecting to the database provided by the asset.
    Make sure to `Dispose()` of any connections you create.
  + CSV files may be optionally imported as SQLite database assets, just change the importer to `Gilzoide.SqliteAsset.Editor.SqliteAssetCsvImporter` in the Inspector.
  + SQLite assets may be loaded from Streaming Assets folder or from memory, depending on the value of the  property.
  + Loading databases from Streaming Assets is not supported in Android and WebGL platforms.
- `SQLiteConnection.Serialize` extension method for serializing a database to `byte[]` (Reference: [SQLite Serialization](https://www.sqlite.org/c3ref/serialize.html)).
- `SQLiteConnection.SerializeToAsset` extension method for serializing a database to an instance of `SqliteAsset`.
- `SQLiteConnection.Deserialize` extension method for deserializing memory (`byte[]`) into a SQLiteConnection (Reference: [SQLite Deserialization](https://www.sqlite.org/c3ref/deserialize.html)).
- `SQLiteConnection.ImportCsvToTable` extension method for importing a CSV data stream as a new table inside the database.


## Dependencies
- [SQLite-net](https://github.com/gilzoide/unity-sqlite-net): library for managing SQLite databases


## How to install
Either:
- Install using the [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html) with the following URL:
  ```
  https://github.com/gilzoide/unity-sqlite-asset.git
  ```
- Clone this repository or download a snapshot of it directly inside your project's `Assets` or `Packages` folder.


## Credits
- Database icons from [Solar Icons Set](https://www.figma.com/community/file/1166831539721848736/solar-icons-set), licensed under [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/)