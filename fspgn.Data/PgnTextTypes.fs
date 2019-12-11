namespace fspgn.Data

open System

module PgnTextTypes =

    type pMoveType =
        | Simple = 0
        | Capture = 1
        | CaptureEnPassant = 2
        | CastleKingSide = 3
        | CastleQueenSide = 4

    type pMoveAnnotation =
        |MindBlowing
        |Brilliant
        |Good
        |Interesting
        |Dubious
        |Mistake
        |Blunder
        |Abysmal
        |FascinatingButUnsound
        |Unclear
        |WithCompensation
        |EvenPosition
        |SlightAdvantageWhite
        |SlightAdvantageBlack
        |AdvantageWhite
        |AdvantageBlack
        |DecisiveAdvantageWhite
        |DecisiveAdvantageBlack
        |Space
        |Initiative
        |Development
        |Counterplay
        |Countering
        |Idea
        |TheoreticalNovelty
        |UnknownAnnotation

    type pMove = 
        {Mtype:pMoveType 
         TargetPiece:PieceType option
         TargetSquare:Position 
         TargetFile:File option
         Piece: PieceType option
         OriginSquare:Position
         OriginFile:File option
         OriginRank:Rank option
         PromotedPiece: PieceType option
         IsCheck:bool option
         IsDoubleCheck:bool option
         IsCheckMate:bool option
         Annotation:pMoveAnnotation option}

    let pMoveCreateOrig(mt,tgp,tgs,tgf,pc,ors,orf,orr) =
        {Mtype=mt 
         TargetPiece=tgp
         TargetSquare=tgs
         TargetFile=tgf
         Piece=pc
         OriginSquare=ors
         OriginFile=orf
         OriginRank=orr
         PromotedPiece=None
         IsCheck=None
         IsDoubleCheck=None
         IsCheckMate=None
         Annotation=None}

    let pMoveCreate(mt,tgp,tgs,tgf,pc) = pMoveCreateOrig(mt,tgp,tgs,tgf,pc,Position.OUTOFBOUNDS,None,None)

    let pMoveCreateCastle(mt) = pMoveCreateOrig(mt,None,Position.OUTOFBOUNDS,None,None,Position.OUTOFBOUNDS,None,None)

