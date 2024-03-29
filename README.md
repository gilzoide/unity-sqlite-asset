# SQLite Asset
[![openupm](https://img.shields.io/npm/v/com.gilzoide.sqlite-asset?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.gilzoide.sqlite-asset/)

Read-only [SQLite](https://sqlite.org/) database assets for Unity.

Automatically imports ".sqlite", ".sqlite2" and ".sqlite3" files as SQLite database assets.


## Features
- [SqliteAsset](Runtime/SqliteAsset.cs): read-only SQLite database Unity assets.
  + Files with the extensions ".sqlite", ".sqlite2" and ".sqlite3" will be imported as SQLite database assets.
  + Use the `CreateConnection()` method for connecting to the database provided by the asset.
    Make sure to `Dispose()` of any connections you create.
  + SQLite assets may be loaded from Streaming Assets folder or from memory, depending on the value of their "Streaming Assets Path" property.
  + **Warning**: Android and WebGL platforms don't support loading SQLite databases from Streaming Assets and will always load them in memory.
- `SQLiteConnection.SerializeToAsset` extension method for serializing a database to an instance of `SqliteAsset`.


## Dependencies
- [SQLite-net](https://github.com/gilzoide/unity-sqlite-net): library for managing SQLite databases


## Optional packages
- [SQLite Asset - CSV](https://github.com/gilzoide/unity-sqlite-asset-csv): easily import ".csv" files as read-only SQLite database assets


## How to install
Either:
- Use the [openupm registry](https://openupm.com/) and install this package using the [openupm-cli](https://github.com/openupm/openupm-cli):
  ```
  openupm add com.gilzoide.sqlite-asset
  ```
- Install using the [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html) with the following URL:
  ```
  https://github.com/gilzoide/unity-sqlite-asset.git
  ```
- Clone this repository or download a snapshot of it directly inside your project's `Assets` or `Packages` folder.


## Credits
- Database icons from [Solar Icons Set](https://www.figma.com/community/file/1166831539721848736/solar-icons-set), licensed under [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/)