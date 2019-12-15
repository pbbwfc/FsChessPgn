namespace FsChessPgn.Data

module Game =

    let MoveCount(mtel:MoveTextEntry list) =
        let mc(mte:MoveTextEntry) =
            match mte with
            |HalfMoveEntry(_) -> 1
            |_ -> 0
        mtel|>List.map mc|>List.reduce(+)
        
    let FullMoveCount(mtel:MoveTextEntry list) = MoveCount(mtel)/2

    let GetMoves(mtel:MoveTextEntry list) =
        let gm(mte:MoveTextEntry) =
            match mte with
            |HalfMoveEntry(_,_,mv) -> [mv]
            |_ -> []
        mtel|>List.map gm|>List.concat

