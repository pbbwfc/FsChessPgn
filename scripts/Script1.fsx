#r"FSChessPgn.dll"
open FsChessPgn.NET

let pgn = @"D:\GitHub\FsChessPgn\FsChessPgn.Test\TestExamples\RealGames\chess-informant-sample.pgn"

let pgms = pgn|>PgnReader.ReadFromFile