#load "setup.fsx"
open FsChess
open FsChess.Pgn

let fn = __SOURCE_DIRECTORY__ + "/data/pgn/molinari-bordais-1979.pgn"
let games = fn|>Games.ReadListFromFile
let first_game = games.Head
let white = first_game.WhitePlayer
let black = first_game.BlackPlayer
let mvs = first_game.MoveText|>Game.MovesStr
let result = first_game.Result 

