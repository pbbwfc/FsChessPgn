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
            |HalfMoveEntry(_,_,mv,_) -> [mv]
            |_ -> []
        mtel|>List.map gm|>List.concat

    //TODO
    let SetpMoves(gm:Game) =
        let rec setpmv (pmvl:MoveTextEntry list) bd opmvl =
            if pmvl|>List.isEmpty then opmvl|>List.rev
            else
                let mte = pmvl.Head
                match mte with
                |HalfMoveEntry(mn,ic,mv,_) -> 
                    let amv = mv|>pMove.ToaMove bd
                    let nmte = HalfMoveEntry(mn,ic,mv,Some(amv))
                    setpmv pmvl.Tail amv.PostBrd (nmte::opmvl)
                |_ -> setpmv pmvl.Tail bd (mte::opmvl)
        
        let ibd = if gm.BoardSetup.IsSome then gm.BoardSetup.Value|>Board.FromFEN else Board.Start
        let nmt = setpmv gm.MoveText ibd []
        {gm with MoveText=nmt}

