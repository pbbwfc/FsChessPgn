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
         TargetSquare:Square 
         TargetFile:File option
         Piece: PieceType option
         OriginSquare:Square
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

    let pMoveCreate(mt,tgp,tgs,tgf,pc) = pMoveCreateOrig(mt,tgp,tgs,tgf,pc,OUTOFBOUNDS,None,None)

    let pMoveCreateCastle(mt) = pMoveCreateOrig(mt,None,OUTOFBOUNDS,None,None,OUTOFBOUNDS,None,None)

    type MoveTextEntry =
        |MovePairEntry of int option * pMove * pMove
        |HalfMoveEntry of int option * bool * pMove
        |CommentEntry of string
        |GameEndEntry of GameResult
        |NAGEntry of int
        |RAVEntry of MoveTextEntry list
    
    type pGameInfo = {Name:string;Value:string}

    type pGame =
        {
            Event : string
            Site : string
            Year : int option
            Month : int option
            Day : int option
            Round :string
            WhitePlayer : string
            BlackPlayer : string
            Result : GameResult
            BoardSetup : Fen option
            AdditionalInfo : pGameInfo list
            Tags : Map<string,string>
            MoveText : MoveTextEntry list
        }

    let pGameEMP =
        {
            Event = ""
            Site = ""
            Year = None
            Month = None
            Day = None
            Round = ""
            WhitePlayer = ""
            BlackPlayer = ""
            Result = GameResult.Open
            BoardSetup = None
            AdditionalInfo = []
            Tags = Map.empty
            MoveText = []
        }