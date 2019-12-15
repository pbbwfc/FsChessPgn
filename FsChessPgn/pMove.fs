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

