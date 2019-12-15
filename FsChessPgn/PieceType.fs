namespace FsChessPgn.Data

module PieceType = 
    let LookupArrayLength = 7
    let ForPlayer (player : Player) (pt : PieceType) = (int (pt) ||| (int (player) <<< 3)) |> Pc
    

