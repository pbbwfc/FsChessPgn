﻿[<AutoOpen>]
module FsChessPgn.PgnParsers.MoveSeries

open FParsec
open FsChessPgn.Data
open FsChessPgn.Data.PgnTextTypes

let pPeriods =
    (str ".." .>> manyChars (pchar '.') >>% true) //two or more dots => Continued move pair
    <|> (str "…" >>% true) // => Continued move pair
    <|> (pchar '.' >>% false) // => Non-Continued move pair (move start)

let pMoveNumberIndicator =
    attempt(pint32 .>> ws .>>. pPeriods |>> fun (num, contd) -> (Some(num), contd))
    <|> preturn (None, false)
    <!> "pMoveNumberIndicator"
    <?> "Move number indicator (e.g. 5. or 13...)"

let pSplitMoveTextEntry =
    pMoveNumberIndicator .>> ws .>>. pMove
    |>> fun ((moveNum, contd), move) -> HalfMoveEntry(moveNum, contd, move)
    <!!> ("pSplitMoveTextEntry", 3)

let pCommentary =
    between (str "{") (str "}") (many (noneOf "}"))
    <|> between (str ";") newline (many (noneOf "\n")) //to end of line comment
    |>> charList2String
    |>> fun text -> CommentEntry(text)
    <!!> ("pCommentary", 3)
    <?> "Comment ( {...} or ;... )"

let pOneHalf = str "1/2" <|> str "½"
let pDraw = pOneHalf .>> ws .>> str "-" .>> ws .>> pOneHalf |>> fun _ -> GameResult.Draw
let pWhiteWin = str "1" .>> ws .>> str "-" .>> ws .>> str "0"  |>> fun _ -> GameResult.WhiteWins
let pBlackWin = str "0" .>> ws .>> str "-" .>> ws .>> str "1"  |>> fun _ -> GameResult.BlackWins
let pEndOpen = str "*"  |>> fun _ -> GameResult.Open

let pEndOfGame =
    pDraw <|> pWhiteWin <|> pBlackWin <|> pEndOpen |>> fun endType -> GameEndEntry(endType) 
    <!!> ("pEndOfGame", 3)
    <?> "Game termination marker (1/2-1/2 or 1-0 or 0-1 or *)"

let pNAG =
    pchar '$' >>. pint32 |>> fun code -> NAGEntry(code) 
    <?> "NAG ($<num> e.g. $6 or $32)"
    <!!> ("pNAG", 3)

let pMoveSeries, pMoveSeriesImpl = createParserForwardedToRef()

let pRAV =
    pchar '(' .>> ws >>. pMoveSeries .>> ws .>> pchar ')'
    |>> fun moveSeries ->
            RAVEntry(moveSeries)
    <?> "RAV e.g. \"(6. Bd3)\""
    <!!> ("pRAV", 4)

let pMoveSeriesEntry=
     pCommentary
    <|> pNAG
    <|> pRAV
    <|> attempt(pSplitMoveTextEntry)
    <|> attempt(pEndOfGame)
    <!!> ("pMoveSeriesEntry", 4)

do pMoveSeriesImpl := (
    sepEndBy1 pMoveSeriesEntry ws
    <!!> ("pMoveSeries", 5)
    )
