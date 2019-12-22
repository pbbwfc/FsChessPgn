#load "setup.fsx"
open FsChessPgn

let pgn = @"D:\GitHub\FsChessPgn\FsChessPgn.Test\TestExamples\RealGames\chess-informant-sample.pgn"

let pgms = pgn|>Games.ReadFromFile