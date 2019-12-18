namespace FsChessPgn.Data

module pMove =

    let CreateAll(mt,tgp,tgs,tgf,pc,ors,orf,orr,pp,ic,id,im,an) =
        {Mtype=mt 
         TargetPiece=tgp
         TargetSquare=tgs
         TargetFile=tgf
         Piece=pc
         OriginSquare=ors
         OriginFile=orf
         OriginRank=orr
         PromotedPiece=pp
         IsCheck=ic
         IsDoubleCheck=id
         IsCheckMate=im
         Annotation=an}

    let CreateOrig(mt,tgp,tgs,tgf,pc,ors,orf,orr) = CreateAll(mt,tgp,tgs,tgf,pc,ors,orf,orr,None,false,false,false,None)

    let Create(mt,tgp,tgs,tgf,pc) = CreateOrig(mt,tgp,tgs,tgf,pc,OUTOFBOUNDS,None,None)

    let CreateCastle(mt) = CreateOrig(mt,None,OUTOFBOUNDS,None,None,OUTOFBOUNDS,None,None)

    let ToaMove (bd:Brd) (pmv:pMove) =
    //TODO:need to fix this
        let mv = 
            if pmv.Mtype=MoveType.CastleKingSide then
                let mvs = 
                    bd|>MoveGenerate.CastleMoves
                    |>List.filter(fun mv -> FileG=(mv|>Move.To|>Square.ToFile))
                if mvs.Length=1 then mvs.Head
                else
                    failwith "kc"
            elif pmv.Mtype=MoveType.CastleQueenSide then
                let mvs = 
                    bd|>MoveGenerate.CastleMoves
                    |>List.filter(fun mv -> FileC=(mv|>Move.To|>Square.ToFile))
                if mvs.Length=1 then mvs.Head
                else
                    failwith "qc"
            elif pmv.Piece.IsNone then failwith "none"
            else
                match pmv.Piece.Value with
                |PieceType.Pawn ->
                    let mvs = 
                        bd|>MoveGenerate.PawnMoves
                        |>List.filter(fun mv -> pmv.TargetSquare=(mv|>Move.To))
                    if mvs.Length=1 then mvs.Head
                    else
                        failwith ("p " + (pmv|>PgnWrite.MoveStr))
                |PieceType.Knight ->
                    let mvs = 
                        bd|>MoveGenerate.KnightMoves
                        |>List.filter(fun mv -> pmv.TargetSquare=(mv|>Move.To))
                    if mvs.Length=1 then mvs.Head
                    elif pmv.OriginFile.IsSome then
                        let mvs1=mvs|>List.filter(fun mv -> pmv.OriginFile.Value=(mv|>Move.From|>Square.ToFile))
                        if mvs1.Length=1 then mvs1.Head else failwith "nf"
                    elif pmv.OriginRank.IsSome then
                        let mvs1=mvs|>List.filter(fun mv -> pmv.OriginRank.Value=(mv|>Move.From|>Square.ToRank))
                        if mvs1.Length=1 then mvs1.Head else failwith "nr"
                    else
                        failwith "n"
                |PieceType.Bishop ->
                    let mvs = 
                        bd|>MoveGenerate.BishopMoves
                        |>List.filter(fun mv -> pmv.TargetSquare=(mv|>Move.To))
                    if mvs.Length=1 then mvs.Head
                    elif pmv.OriginFile.IsSome then
                        let mvs1=mvs|>List.filter(fun mv -> pmv.OriginFile.Value=(mv|>Move.From|>Square.ToFile))
                        if mvs1.Length=1 then mvs1.Head else failwith "bf"
                    elif pmv.OriginRank.IsSome then
                        let mvs1=mvs|>List.filter(fun mv -> pmv.OriginRank.Value=(mv|>Move.From|>Square.ToRank))
                        if mvs1.Length=1 then mvs1.Head else failwith "br"
                    else
                        failwith "b"
                |PieceType.Rook ->
                    let mvs = 
                        bd|>MoveGenerate.RookMoves
                        |>List.filter(fun mv -> pmv.TargetSquare=(mv|>Move.To))
                    if mvs.Length=1 then mvs.Head
                    elif pmv.OriginFile.IsSome then
                        let mvs1=mvs|>List.filter(fun mv -> pmv.OriginFile.Value=(mv|>Move.From|>Square.ToFile))
                        if mvs1.Length=1 then mvs1.Head else failwith "rf"
                    elif pmv.OriginRank.IsSome then
                        let mvs1=mvs|>List.filter(fun mv -> pmv.OriginRank.Value=(mv|>Move.From|>Square.ToRank))
                        if mvs1.Length=1 then mvs1.Head else failwith "rr"
                    else
                        failwith "r"
                |PieceType.Queen ->
                    let mvs = 
                        bd|>MoveGenerate.QueenMoves
                        |>List.filter(fun mv -> pmv.TargetSquare=(mv|>Move.To))
                    if mvs.Length=1 then mvs.Head
                    elif pmv.OriginFile.IsSome then
                        let mvs1=mvs|>List.filter(fun mv -> pmv.OriginFile.Value=(mv|>Move.From|>Square.ToFile))
                        if mvs1.Length=1 then mvs1.Head else failwith "qf"
                    elif pmv.OriginRank.IsSome then
                        let mvs1=mvs|>List.filter(fun mv -> pmv.OriginRank.Value=(mv|>Move.From|>Square.ToRank))
                        if mvs1.Length=1 then mvs1.Head else failwith "qr"
                    else
                        failwith "q"
                |PieceType.King ->
                    let mvs = 
                        bd|>MoveGenerate.KingMoves
                        |>List.filter(fun mv -> pmv.TargetSquare=(mv|>Move.To))
                    if mvs.Length=1 then mvs.Head
                    else
                        failwith "k"
                |_ -> failwith "all"
        {
            PreBrd = bd
            Mv = mv
            PostBrd = bd|>Board.MoveApply mv
        }