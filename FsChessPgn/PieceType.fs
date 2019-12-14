namespace fspgn.Data

module PieceType = 
    let LookupArrayLength = 7
    let AllPieceTypes = 
        [| PieceType.Pawn; PieceType.Knight; PieceType.Bishop; PieceType.Rook; PieceType.Queen; PieceType.King |]
    let ForPlayer (player : Player) (pt : PieceType) = (int (pt) ||| (int (player) <<< 3)) |> Pc
    
    let MaximumMoves pct = 
        match pct with
        | PieceType.EMPTY -> 0
        | PieceType.Pawn -> 4
        | PieceType.Knight -> 8
        | PieceType.Bishop -> 13
        | PieceType.Rook -> 14
        | PieceType.Queen -> 27
        | PieceType.King -> 8
        | _ -> failwith "invalid Piece Type"
