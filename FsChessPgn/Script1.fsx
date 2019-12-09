#r@"bin\Debug\netcoreapp3.0\FsChessPgn.dll"

open PgnLib

let pgn = @"D:\GitHub\PgnTools\data\GordonWScott.pgn"

let gms = pgn|>PGN.ReadFromFile

