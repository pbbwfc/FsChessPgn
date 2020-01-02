#load "setup.fsx"
open FsChess.Pgn

let pgn = __SOURCE_DIRECTORY__ + "/data/pgn/kasparov-deep-blue-1997.pgn"

//loads all games
let gml = Games.ReadListFromFile(pgn)
let count = gml.Length

//does not load any game yet
let gms = Games.ReadSeqFromFile(pgn)

//Just loads first game
let first_game = gms|>Seq.head
let event = first_game.Event


