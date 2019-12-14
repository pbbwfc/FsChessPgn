[<AutoOpen>]
module FsChessPgn.PgnParsers.PgnTags

open System
open FsChessPgn.Data

type PgnTag(name: string, value: string) =
    member val Name = name with get, set
    member val Value = value with get, set

let formatDate (year : int option, month : int option, day : int option) =
    let yearStr =
        match year with
        | None -> "????"
        | _ -> year.Value.ToString("D4")

    let monthStr =
        match month with
        | None -> "??"
        | _ -> month.Value.ToString("D2");

    let dayStr =
        match day with
        | None -> "??"
        | _ -> day.Value.ToString("D2");

    String.Format("{0}-{1}-{2}", yearStr, monthStr, dayStr)

type PgnDateTag(name: string, year: int option, month: int option, day: int option) =
    inherit PgnTag(name, formatDate(year, month, day))

    member val Year = year with get, set
    member val Month = month with get, set
    member val Day = day with get, set


let formatResult(result: GameResult) =
    match result with
    | GameResult.WhiteWins -> "1 - 0"
    | GameResult.BlackWins -> "0 - 1"
    | GameResult.Draw  -> "1/2 - 1/2"
    | GameResult.Open  -> "*"
    | _ -> failwith "Enum GameResult is outside known cases"

type PgnResultTag(name: string, result: GameResult) =
    inherit PgnTag(name, formatResult(result))

    member val Result: GameResult = result with get, set

type FenTag(name: string, setup: Fen) =
    inherit PgnTag(name, setup.ToString())

    member val Setup: Fen = setup with get, set