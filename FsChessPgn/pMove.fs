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
            if pmv.Piece.IsNone then MoveEmpty
            else
                match pmv.Piece.Value with
                |PieceType.Pawn ->
                    let mvs = 
                        bd|>MoveGenerate.PawnMoves
                        |>List.filter(fun mv -> pmv.TargetSquare=(mv|>Move.To))
                    if mvs.Length=1 then mvs.Head
                    else
                        MoveEmpty
                |_ -> MoveEmpty
        
        
        
        {
            PreBrd = bd
            Mv = mv
            PostBrd = bd|>Board.MoveApply mv
        }