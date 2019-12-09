[<AutoOpen>]
module fspgn.PgnParsers.BasicChess

open System
open FParsec
open fspgn.Data



let fileSymbol = [ 'a' .. 'h'] |> List.map (fun x -> x.ToString())
let findFile (a: string) = a|>Convert.ToChar|>File.Parse

let rankSymbol = [1 .. 8] |> List.map (fun x -> x.ToString())
let findRank (a: string) = a|>Convert.ToChar|>Rank.Parse

//NOTE: we allow S (ger. "Springer") for knight was used traditionally and is around in older PGNs
//NOTE: 'b' is not allowed here as it is reserved for the b file
let pPiece =
        (pchar 'p' >>% PieceType.Pawn)
    <|> (pchar 'P' >>% PieceType.Pawn)
    <|> (pchar 'N' >>% PieceType.Knight)
    <|> (pchar 'n' >>% PieceType.Knight)
    <|> (pchar 'S' >>% PieceType.Knight)
    <|> (pchar 's' >>% PieceType.Knight)
    <|> (pchar 'B' >>% PieceType.Bishop)
    <|> (pchar 'R' >>% PieceType.Rook)
    <|> (pchar 'r' >>% PieceType.Rook)
    <|> (pchar 'Q' >>% PieceType.Queen)
    <|> (pchar 'q' >>% PieceType.Queen)
    <|> (pchar 'K' >>% PieceType.King)
    <|> (pchar 'k' >>% PieceType.King)
    <?> "Piece (N, B, R, Q, K, P, n, r, q, k, p)"

let pFile =
    pList(strCI, fileSymbol) |>> findFile
    <?> "File letter (A..H)"

let pRank =
    pList(strCI, rankSymbol) |>> findRank
    <?> "Rank (1..8)"


let apply p = run (pPiece >>. pFile >>. pRank) p