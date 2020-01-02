#load "setup.fsx"
open FsChess.Pgn

let pgn = __SOURCE_DIRECTORY__ + "/data/pgn/kasparov-deep-blue-1997.pgn"

//loads all games
let gml = Games.ReadListFromFile(pgn)

let gml2 = gml.[0..1]
let pgn2 = "c:/temp/test.pgn"

gml2|>Games.WriteFile pgn2


