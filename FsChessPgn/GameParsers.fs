[<AutoOpen>]
module FsChessPgn.PgnParsers.Game

open FParsec
open FsChessPgn.Data.PgnTextTypes

let setTag(igame : pGame, tag : PgnTag) =
    let game = {igame with Tags = igame.Tags.Add(tag.Name, tag.Value)}
    match tag.Name with
    | "Event" -> {game with Event = tag.Value}
    | "Site" -> {game with Site = tag.Value}
    | "Date" -> {game with Year = (tag :?> PgnDateTag).Year; Month = (tag :?> PgnDateTag).Month; Day = (tag :?> PgnDateTag).Day}
    | "Round" -> {game with Round = tag.Value}
    | "White" -> {game with WhitePlayer = tag.Value}
    | "Black" -> {game with BlackPlayer = tag.Value}
    | "Result" -> {game with Result = (tag :?> PgnResultTag).Result}
    | "FEN" -> {game with BoardSetup = (tag :?> FenTag).Setup|>Some}
    | _ ->
        {game with AdditionalInfo= game.AdditionalInfo @ [{Name=tag.Name; Value=tag.Value}]}

let makeGame (tagList : PgnTag list, moveTextList : MoveTextEntry list) =
    let mutable game = pGameEMP
    tagList |> List.iter(fun tag -> game <- setTag(game, tag)) 
    {game with MoveText = moveTextList}

let pGame =
    ws >>. pTagList .>> ws .>>.  pMoveSeries .>> (ws <|> eof)
    |>>  makeGame
    <!!> ("pGame", 5)

let pDatabase =
    sepEndBy pGame ws .>> eof
    //|>> fun games ->
    //        let db = new Database()
    //        db.Games.AddRange(games)
    //        db

