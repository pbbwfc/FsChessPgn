namespace FsChessPgn.Data

module Piece = 
    let LookupArrayLength = 15
    let AllPieces = 
        [| Piece.WPawn; Piece.WKnight; Piece.WBishop; Piece.WRook; Piece.WQueen; Piece.WKing; Piece.BPawn; Piece.BKnight; 
           Piece.BBishop; Piece.BRook; Piece.BQueen; Piece.BKing |]
    let AllPieces2 = 
        [| Piece.EMPTY; Piece.WPawn; Piece.WKnight; Piece.WBishop; Piece.WRook; Piece.WQueen; Piece.WKing; Piece.EMPTY;
           Piece.EMPTY; Piece.BPawn; Piece.BKnight; Piece.BBishop; Piece.BRook; Piece.BQueen; Piece.BKing |]
    
    let Parse(c : char) = 
        match c with
        | 'P' -> Piece.WPawn
        | 'N' -> Piece.WKnight
        | 'B' -> Piece.WBishop
        | 'R' -> Piece.WRook
        | 'Q' -> Piece.WQueen
        | 'K' -> Piece.WKing
        | 'p' -> Piece.BPawn
        | 'n' -> Piece.BKnight
        | 'b' -> Piece.BBishop
        | 'r' -> Piece.BRook
        | 'q' -> Piece.BQueen
        | 'k' -> Piece.BKing
        | _ -> failwith (c.ToString() + " is not a valid piece")
    
    let PieceToString(piece : Piece) = 
        match piece with
        | Piece.WPawn -> "P"
        | Piece.WKnight -> "N"
        | Piece.WBishop -> "B"
        | Piece.WRook -> "R"
        | Piece.WQueen -> "Q"
        | Piece.WKing -> "K"
        | Piece.BPawn -> "p"
        | Piece.BKnight -> "n"
        | Piece.BBishop -> "b"
        | Piece.BRook -> "r"
        | Piece.BQueen -> "q"
        | Piece.BKing -> "k"
        | _ -> failwith ("not a valid piece")
    
    let PieceValBasic(piece : Piece) = 
        match piece with
        | Piece.WPawn | Piece.BPawn -> 100
        | Piece.WKnight | Piece.BKnight -> 300
        | Piece.WBishop | Piece.BBishop -> 300
        | Piece.WRook | Piece.BRook -> 500
        | Piece.WQueen | Piece.BQueen -> 900
        | Piece.WKing | Piece.BKing -> 10000
        | _ -> 0
    
    let ToPieceType(piece : Piece) = (int (piece) &&& 7) |> PcTp
    let ParseAsPiece (player : Player) (c : char) = (Parse(c) |> ToPieceType)|>PieceType.ForPlayer(player)
    
    let ToOppositePlayer(piece : Piece) = 
        if piece = Piece.EMPTY then Piece.EMPTY
        else (int (piece) ^^^ 8) |> Pc
    
    let PieceToPlayer(piece : Piece) = (int (piece) >>> 3) |> Plyr
    
    let PieceIsSliderRook(piece : Piece) = 
        match piece with
        | Piece.WRook | Piece.WQueen | Piece.BRook | Piece.BQueen -> true
        | _ -> false
