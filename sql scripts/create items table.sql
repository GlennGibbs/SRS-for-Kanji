-- Script Date: 30/07/2020 14:07  - ErikEJ.SqlCeScripting version 3.5.2.86
CREATE TABLE [Items] (
  [kanji] TEXT NOT NULL
, [repition] INTEGER DEFAULT (0) NOT NULL
, [easiness] REAL DEFAULT (0) NOT NULL
, [interval] INTEGER DEFAULT (0) NOT NULL
, CONSTRAINT [PK_Items] PRIMARY KEY ([kanji])
);
