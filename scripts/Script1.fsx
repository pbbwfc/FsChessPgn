//scripts don't work as FParsec is compiled under old code - need to wait for a Net Core 3 version!
#r "FsChessPgn.dll"
open FsChessPgn.NET

let pgn = @"D:\GitHub\FsChessPgn\FsChessPgn.Test\TestExamples\RealGames\chess-informant-sample.pgn"

let pgms = pgn|>PgnReader.ReadFromFile