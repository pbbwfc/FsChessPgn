[<AutoOpen>]
module FsChessPgn.PgnParsers.FenTagParser

open FParsec
open FsChessPgn.Data
open System.Linq

let pFenPieces =
        (pchar 'p' >>% [Piece.BPawn])
    <|> (pchar 'n' >>% [Piece.BKnight])
    <|> (pchar 'b' >>% [Piece.BBishop])
    <|> (pchar 'r' >>% [Piece.BRook])
    <|> (pchar 'q' >>% [Piece.BQueen])
    <|> (pchar 'k' >>% [Piece.BKing])
    <|> (pchar 'P' >>% [Piece.WPawn])
    <|> (pchar 'N' >>% [Piece.WKnight])
    <|> (pchar 'B' >>% [Piece.WBishop])
    <|> (pchar 'R' >>% [Piece.WRook])
    <|> (pchar 'Q' >>% [Piece.WQueen])
    <|> (pchar 'K' >>% [Piece.WKing])
    <|> (pint32 |>> fun n -> Enumerable.Repeat(Piece.EMPTY, n) |> List.ofSeq)

let check8elem (msg: string) (row: 'a list) : Parser<_, _> =
    fun stream ->
        match row.Length with
        | 8 -> Reply(row)
        | _  -> Reply(Error, messageError(msg))

let pFenRow =
    many pFenPieces |>> fun lists -> List.concat lists
    >>= check8elem "Invalid fen row lenght. Rows must be of length 8"


let checkBoardLenght (row: 'a list) : Parser<_, _> =
    fun stream ->
        match row.Length with
        | 8 -> Reply(row)
        | _  -> Reply(Error, messageError(sprintf "Invalid fen row lenght (%d). Rows must be of length 8"  row.Length ))

let pPiecePositions =
    sepEndBy1 pFenRow (pchar '/') >>= check8elem "Invalid fen row count. There must be 8 rows."
    |>> fun lists -> List.concat lists

let pFenCastlingInfo =
    attempt(pchar '-' >>% [ false; false; false; false] <!> "noCastling")
    <|> (
        (attempt(pchar 'K' >>% true <!> "king side white") <|> preturn false) .>>.
        (attempt(pchar 'Q' >>% true) <|> preturn false) .>>.
        (attempt(pchar 'k' >>% true) <|> preturn false) .>>.
        (attempt(pchar 'q' >>% true) <|> preturn false)
        |>> fun(((K, Q), k), q) -> [K; Q; k; q]
    )

let pFenEnPassantSquare =
    attempt(pchar '-' >>% OUTOFBOUNDS)
    <|> (pFile .>>. pRank |>> fun (f, r) -> Sq(f,r))

let pFenTagValue =
    pchar '"' >>. pPiecePositions .>> ws
    .>>. ((pchar 'w' >>% true) <|> (pchar 'b' >>% false)) .>> ws
    .>>. pFenCastlingInfo .>> ws
    .>>. pFenEnPassantSquare .>> ws
    .>>. pint32 .>> ws
    .>>. pint32 .>> ws
    .>> pchar '"'
    |>> fun (((((pieces, whiteMove), castlingInfo), enPassantSquare), halfMoves), fullMoves) ->
            let boardSetup= 
                { Pieceat = pieces
                  Whosturn = if whiteMove then Player.White else Player.Black
                  CastleWS = castlingInfo.[0]
                  CastleWL = castlingInfo.[1]
                  CastleBS = castlingInfo.[2]
                  CastleBL = castlingInfo.[3]
                  Enpassant = enPassantSquare
                  Fiftymove = halfMoves
                  Fullmove = fullMoves }

            FenTag("FEN", boardSetup) :> PgnTag;

    <!> "pFenTagValue"