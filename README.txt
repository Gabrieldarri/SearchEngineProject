Version 2: 24-02-2026

This codebase is s PoC seachengine that consist of 4 programs and a class library.

The 4 programs are the indexer (also called a crawler), a search program (console), a API for the search logic,
and a webapp with a single search page. 

The indexer will crawl a folder (in depth) and create a reverse index
in a database. It will only index text files with .txt as extension. See the Config.cs for the folder
to be indexed.

The search program (see the ConsoleSearch project) offers a query-based search
in the reverse index. The same functionality is in the webapp with a single page for searching the
reverse index.

The class library Shared contains classes that are used by more than one of the programs. 

Support for using either Sqlite or Postgres as database engine. See SearchAPI.Logic.Paths for connectionstrings
to the databases. The specific databasenegine to use, is decided in SearchAPI.Controllers.SearchController.





