namespace FsChessPgn.Data

open System

type MvHstry = 
    { Move : Move
      Enpassant : Square
      Castle : CstlFlgs
      FiftyCount : int
      MovesSinceNull : int
      //Zobrist : int64
      //ZobristPawn : int64
      //ZobristMaterial : int64
      Checkers : Bitboard }

type Brd = 
    { PieceAt : Piece []
      WtPcCnt : int []
      BkPcCnt : int []
      WtKingPos : Square
      BkKingPos : Square
      PieceTypes : Bitboard []
      WtPrBds : Bitboard
      BkPrBds : Bitboard
      PieceLocationsAll : Bitboard
      Checkers : Bitboard
      WhosTurn : Player
      CastleRights : CstlFlgs
      EnPassant : Square
      Fiftymove : int
      Fullmove : int
      //ZobristBoard : int64
      //ZobPawn : int64
      //ZobMaterial : int64
      HistL : MvHstry list
      MovesSinceNull : int
      //PcSqEvaluator : PcsqPcPs
      //PcSq : PhsdScr 
      }

module Board = 
    let Cf0 = CasFlg 0
    let From(move : Move) :Square = (int(move) &&& 0x3F)
    let To(move : Move) :Square = (int(move) >>> 6 &&& 0x3F)
    let MovingPiece(move : Move) = (int(move) >>> 12 &&& 0xF) |> Pc
    let IsW(move : Move) = move|>MovingPiece|>int<9
    let MovingPieceType(move : Move) = (int(move) >>> 12 &&& 0x7) |> PcTp
    let MovingPlayer(move : Move) = (int(move) >>> 15 &&& 0x1) |> Plyr
    let IsCapture(move : Move) = (int(move) >>> 16 &&& 0xF) <> 0
    let CapturedPiece(move : Move) = (int(move) >>> 16 &&& 0xF) |> Pc
    let CapturedPieceType(move : Move) = (int(move) >>> 16 &&& 0x7) |> PcTp
    let IsPromotion(move : Move) = (int (move) >>> 20 &&& 0x7) <> 0
    let PromoteType(move : Move) = (int (move) >>> 20 &&& 0x7) |> PcTp
    
    let Promote(move : Move) = 
        if move
           |> PromoteType
           = PieceType.EMPTY then Piece.EMPTY
        else (move |> PromoteType)|>PieceType.ForPlayer(move|>MovingPlayer)
    
    let IsEnPassant(move : Move) = 
        move|>MovingPieceType = PieceType.Pawn && not (move|>IsCapture) 
                                && (move|>From|>Square.ToFile) <> (move|>To|>Square.ToFile)
    let IsCastle(move : Move) = 
        move|>MovingPieceType = PieceType.King && Math.Abs(int (move|>From) - int (move|>To)) = 2
    let IsPawnDoubleJump(move : Move) = 
        move|>MovingPieceType = PieceType.Pawn && Math.Abs(int (move|>From) - int (move|>To)) = 16
    
    //let EndgameMateKingPcSq loseKing (winKing : Square) = 
    //    let getsqs pos = 
    //        let d1 = pos|>Square.DistanceToNoDiag(D4)
    //        let d2 = pos|>Square.DistanceToNoDiag(D5)
    //        let d3 = pos|>Square.DistanceToNoDiag(E4)
    //        let d4 = pos|>Square.DistanceToNoDiag(E5)
    //        let minDist = [ d1; d2; d3; d4 ] |> List.min
    //        minDist * 50
    //    let kPcSq = SQUARES|>Array.map getsqs
    //    PhasedScore.Create 0 (kPcSq.[int (loseKing)] - ((winKing|>Square.DistanceTo(loseKing)) * 25))
    
    //let UseEndGamePcSq (board : Brd) (winPlayer : Player) = 
    //    let losePlayer = winPlayer|>Player.PlayerOther
    //    let losect = if losePlayer=Player.White then board.WtPcCnt else board.BkPcCnt
    //    let winct = if winPlayer=Player.White then board.WtPcCnt else board.BkPcCnt

    //    if losect.[int (PieceType.Pawn)] = 0 
    //       && losect.[int (PieceType.Queen)] = 0 
    //       && losect.[int (PieceType.Rook)] = 0 
    //       && (losect.[int (PieceType.Bishop)] 
    //           + losect.[int (PieceType.Knight)] <= 1) then 
    //        if winct.[int (PieceType.Queen)] > 0 
    //           || winct.[int (PieceType.Rook)] > 0 
    //           || winct.[int (PieceType.Bishop)] 
    //              + winct.[int (PieceType.Knight)] >= 2 then 
    //            let loseKing = if losePlayer=Player.White then board.WtKingPos else board.BkKingPos
    //            let winKing = if winPlayer=Player.White then board.WtKingPos else board.BkKingPos
    //            let newPcSq = EndgameMateKingPcSq loseKing winKing
    //            true, newPcSq
    //        else false, PhasedScore.Create 0 0
    //    else false, PhasedScore.Create 0 0
    
    let MhSet move enpassant castle fifty sinceNull (*zob zobPawn zobMaterial*) checkers = 
        { Move = move
          Enpassant = enpassant
          Castle = castle
          FiftyCount = fifty
          MovesSinceNull = sinceNull
          //Zobrist = zob
          //ZobristPawn = zobPawn
          //ZobristMaterial = zobMaterial
          Checkers = checkers }
    
    let MhEmp = MhSet MoveEmpty OUTOFBOUNDS CstlFlgs.All 0 0 (*0L 0L 0L*) Bitboard.Empty
    
    let Create() = 
        { PieceAt = Array.create 65 Piece.EMPTY
          WtPcCnt = Array.create 7 0
          BkPcCnt = Array.create 7 0
          WtKingPos = OUTOFBOUNDS
          BkKingPos = OUTOFBOUNDS
          PieceTypes = Array.create PieceType.LookupArrayLength Bitboard.Empty
          WtPrBds = Bitboard.Empty
          BkPrBds = Bitboard.Empty
          PieceLocationsAll = Bitboard.Empty
          Checkers = Bitboard.Empty
          WhosTurn = Player.White
          CastleRights = Cf0
          EnPassant = OUTOFBOUNDS
          Fiftymove = 0
          Fullmove = 0
          //ZobristBoard = 0L
          //ZobPawn = 0L
          //ZobMaterial = 0L
          HistL = []
          MovesSinceNull = 100
          //PcSqEvaluator = PcSqEvaluator.Emp()
          //PcSq = PhasedScore.Create 0 0 
          }
    
    //let BoardZob(bd : Brd) = 
    //    let zpc = 
    //        let getpcpos p =
    //            let pc = bd.PieceAt.[int (p)]
    //            if pc = Piece.EMPTY then 0L else Zobrist.PiecePosition pc p
    //        SQUARES
    //        |>Array.map getpcpos
    //        |>Array.reduce(^^^)
    //    let ztrn = if bd.WhosTurn = Player.Black then Zobrist.PlayerKey else 0L
    //    let zcs1 = if (bd.CastleRights &&& CstlFlgs.WhiteShort) <> Cf0 then Zobrist.CastleWS else 0L
    //    let zcs2 = if (bd.CastleRights &&& CstlFlgs.WhiteLong) <> Cf0 then Zobrist.CastleWL else 0L
    //    let zcs3 = if (bd.CastleRights &&& CstlFlgs.BlackShort) <> Cf0 then Zobrist.CastleBS else 0L
    //    let zcs4 = if (bd.CastleRights &&& CstlFlgs.BlackLong) <> Cf0 then Zobrist.CastleBL else 0L
    //    let zep = if bd.EnPassant|>Square.IsInBounds then Zobrist.Enpassant(bd.EnPassant) else 0L
    //    zpc ^^^ ztrn ^^^ zcs1 ^^^ zcs2 ^^^ zcs3 ^^^ zcs4 ^^^ zep
    
    //let BoardZobPawn(bd : Brd) = 
    //    let getpcpos p =
    //        let pc = bd.PieceAt.[int (p)]
    //        if pc = Piece.WPawn || pc = Piece.BPawn then Zobrist.PiecePosition pc p else 0L
    //    SQUARES
    //    |>Array.map getpcpos
    //    |>Array.reduce(^^^)
    
    //let BoardZobMaterial(bd : Brd) = 
    //    let getz pr pt =
    //        let ct = if pr=Player.White then bd.WtPcCnt.[int(pt)] else bd.BkPcCnt.[int(pt)]
    //        if ct>0 then
    //            [0 .. ct - 1]
    //            |>List.map(fun i -> Zobrist.Material (pt|>PieceType.ForPlayer(pr)) i)
    //            |>List.reduce(^^^)
    //        else 0L
    //    Player.AllPlayers
    //    |>Array.map(fun pr -> PieceType.AllPieceTypes|>Array.map(getz pr)|>Array.reduce(^^^))
    //    |>Array.reduce(^^^)
    
    let PieceMove (mfrom : Square) mto (bd : Brd) = 
        let piece = bd.PieceAt.[int (mfrom)]
        let player = piece|>Piece.PieceToPlayer
        let pieceType = piece|>Piece.ToPieceType
        let pieceat = bd.PieceAt|>Array.mapi(fun i p -> if i=int(mto) then piece elif i = int(mfrom) then Piece.EMPTY else p)
        //let zobristBoard = bd.ZobristBoard ^^^ (Zobrist.PiecePosition piece mfrom) ^^^ (Zobrist.PiecePosition piece mto)
        //let pcSq = PcSqEvaluator.PcSqValuesRemove piece mfrom bd.PcSqEvaluator bd.PcSq
        //let pcSq = PcSqEvaluator.PcSqValuesAdd piece mto bd.PcSqEvaluator pcSq
        let posBits = (mfrom |> Square.ToBitboard) ||| (mto |> Square.ToBitboard)
        let piecetypes = bd.PieceTypes|>Array.mapi(fun i p -> if i=int(pieceType) then p ^^^ posBits else p)
        let wtprbds = if player=Player.White then bd.WtPrBds ^^^ posBits else bd.WtPrBds
        let bkprbds = if player=Player.Black then bd.BkPrBds ^^^ posBits else bd.BkPrBds
        let pieceLocationsAll = bd.PieceLocationsAll ^^^ posBits
        //let zobPawn = 
        //    if pieceType = PieceType.Pawn then 
        //        bd.ZobPawn ^^^ (Zobrist.PiecePosition piece mfrom) ^^^ (Zobrist.PiecePosition piece mto)
        //    else bd.ZobPawn
        let wtkingpos = if pieceType = PieceType.King && player=Player.White then mto else bd.WtKingPos
        let bkkingpos = if pieceType = PieceType.King && player=Player.Black then mto else bd.BkKingPos
        { bd with PieceAt = pieceat
                  //ZobristBoard = zobristBoard
                  //PcSq = pcSq
                  PieceTypes = piecetypes
                  WtPrBds = wtprbds
                  BkPrBds = bkprbds
                  PieceLocationsAll = pieceLocationsAll
                  //ZobPawn = zobPawn
                  WtKingPos = wtkingpos
                  BkKingPos = bkkingpos }
 
    let PieceAdd pos (piece : Piece) (bd : Brd) = 
        let player = piece |> Piece.PieceToPlayer
        let pieceType = piece |> Piece.ToPieceType
        
        let pieceat = 
            bd.PieceAt |> Array.mapi (fun i p -> 
                              if i = int (pos) then piece
                              else p)
        
        //let zobristboard = bd.ZobristBoard ^^^ (Zobrist.PiecePosition piece pos)
        
        let countOthers = 
            if player = Player.White then bd.WtPcCnt.[int (pieceType)]
            else bd.BkPcCnt.[int (pieceType)]
        
        //let zobmaterial = bd.ZobMaterial ^^^ (Zobrist.Material piece countOthers)
        let wtpccnt = 
            bd.WtPcCnt |> Array.mapi (fun i p -> 
                              if player = Player.White && i = int (pieceType) then countOthers + 1
                              else p)
        
        let bkpccnt = 
            bd.BkPcCnt |> Array.mapi (fun i p -> 
                              if player = Player.Black && i = int (pieceType) then countOthers + 1
                              else p)
        
        //let pcSq = PcSqEvaluator.PcSqValuesAdd piece pos bd.PcSqEvaluator bd.PcSq
        let posBits = pos |> Square.ToBitboard
        
        let piecetypes = 
            bd.PieceTypes |> Array.mapi (fun i p -> 
                                 if i = int (piece |> Piece.ToPieceType) then p ||| posBits
                                 else p)
        
        let piecelocationsall = bd.PieceLocationsAll ||| posBits
        let wtprbds = if (piece |> Piece.PieceToPlayer)=Player.White then bd.WtPrBds ||| posBits else bd.WtPrBds
        let bkprbds = if (piece |> Piece.PieceToPlayer)=Player.Black then bd.BkPrBds ||| posBits else bd.BkPrBds
        //let zobpawn = 
        //    if pieceType = PieceType.Pawn then bd.ZobPawn ^^^ (Zobrist.PiecePosition piece pos)
        //    else bd.ZobPawn
        let wtkingpos = if pieceType = PieceType.King && player=Player.White then pos else bd.WtKingPos
        let bkkingpos = if pieceType = PieceType.King && player=Player.Black then pos else bd.BkKingPos
        { bd with PieceAt = pieceat
                  //ZobristBoard = zobristboard
                  //ZobMaterial = zobmaterial
                  WtPcCnt = wtpccnt
                  BkPcCnt = bkpccnt
                  //PcSq = pcSq
                  PieceTypes = piecetypes
                  PieceLocationsAll = piecelocationsall
                  WtPrBds = wtprbds
                  BkPrBds = bkprbds
                  //ZobPawn = zobpawn
                  WtKingPos = wtkingpos
                  BkKingPos = bkkingpos }

    let PieceRemove (pos : Square) (bd : Brd) = 
        let piece = bd.PieceAt.[int (pos)]
        let player = piece |> Piece.PieceToPlayer
        let pieceType = piece |> Piece.ToPieceType
        
        let pieceat = 
            bd.PieceAt |> Array.mapi (fun i p -> 
                              if i = int (pos) then Piece.EMPTY
                              else p)
        
        //let zobbd = bd.ZobristBoard ^^^ (Zobrist.PiecePosition piece pos)
        let countOthers = 
            if player = Player.White then bd.WtPcCnt.[int (pieceType)] - 1
            else bd.BkPcCnt.[int (pieceType)] - 1
        //let zobmat = bd.ZobMaterial ^^^ (Zobrist.Material piece countOthers)
        
        let wtpccnt = 
            bd.WtPcCnt |> Array.mapi (fun i p -> 
                              if player = Player.White && i = int (pieceType) then countOthers
                              else p)
        
        let bkpccnt = 
            bd.BkPcCnt |> Array.mapi (fun i p -> 
                              if player = Player.Black && i = int (pieceType) then countOthers
                              else p)
        
        //let pcSq = PcSqEvaluator.PcSqValuesRemove piece pos bd.PcSqEvaluator bd.PcSq
        let notPosBits = ~~~(pos |> Square.ToBitboard)
        
        let piecetypes = 
            bd.PieceTypes |> Array.mapi (fun i p -> 
                                 if i = int (pieceType) then p &&& notPosBits
                                 else p)
        let wtprbds = if player=Player.White then bd.WtPrBds &&& notPosBits else bd.WtPrBds
        let bkprbds = if player=Player.Black then bd.BkPrBds &&& notPosBits else bd.BkPrBds
        let piecelocationsall = bd.PieceLocationsAll &&& notPosBits
        //let zobpawn = 
        //    if pieceType = PieceType.Pawn then bd.ZobPawn ^^^ (Zobrist.PiecePosition piece pos)
        //    else bd.ZobPawn
        { bd with PieceAt = pieceat
                  //ZobristBoard = zobbd
                  //ZobMaterial = zobmat
                  WtPcCnt = wtpccnt
                  BkPcCnt = bkpccnt
                  //PcSq = pcSq
                  PieceTypes = piecetypes
                  WtPrBds = wtprbds
                  BkPrBds = bkprbds
                  PieceLocationsAll = piecelocationsall
                  //ZobPawn = zobpawn 
                  }
    
    let PieceChange pos newPiece (bd : Brd) = 
        bd
        |> PieceRemove(pos)
        |> PieceAdd pos newPiece
    
    let RookSliders(bd : Brd) = bd.PieceTypes.[int (PieceType.Rook)] ||| bd.PieceTypes.[int (PieceType.Queen)]
    let BishopSliders(bd : Brd) = bd.PieceTypes.[int (PieceType.Bishop)] ||| bd.PieceTypes.[int (PieceType.Queen)]
    
    
    let AttacksTo (mto : Square) (bd : Brd) = 
        (Attacks.KnightAttacks(mto) &&& bd.PieceTypes.[int (PieceType.Knight)]) 
        ||| ((Attacks.RookAttacks mto bd.PieceLocationsAll) 
             &&& (bd.PieceTypes.[int (PieceType.Queen)] ||| bd.PieceTypes.[int (PieceType.Rook)])) 
        ||| ((Attacks.BishopAttacks mto bd.PieceLocationsAll) 
             &&& (bd.PieceTypes.[int (PieceType.Queen)] ||| bd.PieceTypes.[int (PieceType.Bishop)])) 
        ||| (Attacks.KingAttacks(mto) &&& (bd.PieceTypes.[int (PieceType.King)])) 
        ||| ((Attacks.PawnAttacks mto Player.Black) &&& bd.WtPrBds
             &&& bd.PieceTypes.[int (PieceType.Pawn)]) 
        ||| ((Attacks.PawnAttacks mto Player.White) &&& bd.BkPrBds 
             &&& bd.PieceTypes.[int (PieceType.Pawn)])
    let AttacksTo2 (mto : Square) (by : Player) (bd : Brd) = bd
                                                               |> AttacksTo(mto)
                                                               &&& (if by=Player.White then bd.WtPrBds else bd.BkPrBds)
    let PositionAttacked (mto : Square) (by : Player) (bd : Brd) = bd
                                                                     |> AttacksTo2 mto by
                                                                     <> Bitboard.Empty
    
    let MoveApply (move : Move) (bd : Brd) = 
        let mfrom = move|>From
        let mto = move|>To
        let piece = move|>MovingPiece
        let capture = move|>CapturedPiece

        let mhl = (MhSet move bd.EnPassant bd.CastleRights bd.Fiftymove bd.MovesSinceNull (* bd.ZobristBoard bd.ZobPawn bd.ZobMaterial *) bd.Checkers)::bd.HistL
        let bd = 
            { bd with HistL = mhl
                      MovesSinceNull = bd.MovesSinceNull + 1 }
        
        let bd = 
            if capture <> Piece.EMPTY then bd |> PieceRemove(mto)
            else bd
        
        let bd = bd |> PieceMove mfrom mto
        
        let bd = 
            if move |> IsPromotion then bd |> PieceChange mto (move |> Promote)
            else bd
        
        let bd = 
            if move |> IsCastle then 
                if piece = Piece.WKing && mfrom = E1 && mto = G1 then 
                    bd |> PieceMove H1 F1
                elif piece = Piece.WKing && mfrom = E1 && mto = C1 then 
                    bd |> PieceMove A1 D1
                elif piece = Piece.BKing && mfrom = E8 && mto = G8 then 
                    bd |> PieceMove H8 F8
                else bd |> PieceMove A8 D8
            else bd
        
        let bd = 
            if bd.CastleRights <> Cf0 then 
                if (bd.CastleRights &&& CstlFlgs.WhiteShort) <> Cf0 
                   && (piece = Piece.WKing || mfrom = H1) then 
                    { bd with CastleRights = bd.CastleRights &&& ~~~CstlFlgs.WhiteShort
                              //ZobristBoard = bd.ZobristBoard ^^^ Zobrist.CastleWS 
                              }
                elif (bd.CastleRights &&& CstlFlgs.WhiteLong) <> Cf0 
                     && (piece = Piece.WKing || mfrom = A1) then 
                    { bd with CastleRights = bd.CastleRights &&& ~~~CstlFlgs.WhiteLong
                              //ZobristBoard = bd.ZobristBoard ^^^ Zobrist.CastleWL 
                              }
                elif (bd.CastleRights &&& CstlFlgs.BlackShort) <> Cf0
                     && (piece = Piece.BKing || mfrom = H8) then 
                    { bd with CastleRights = bd.CastleRights &&& ~~~CstlFlgs.BlackShort
                              //ZobristBoard = bd.ZobristBoard ^^^ Zobrist.CastleBS 
                              }
                elif (bd.CastleRights &&& CstlFlgs.BlackLong) <> Cf0
                     && (piece = Piece.BKing || mfrom = A8) then 
                    { bd with CastleRights = bd.CastleRights &&& ~~~CstlFlgs.BlackLong
                              //ZobristBoard = bd.ZobristBoard ^^^ Zobrist.CastleBL 
                              }
                else bd
            else bd
        
        let bd = 
            if move |> IsEnPassant then 
                bd |> PieceRemove((mto|>Square.ToFile)|>File.ToPosition(move|>MovingPlayer|>Player.MyRank(Rank5)))
            else bd
        
        let bd = 
            if bd.EnPassant|>Square.IsInBounds then 
                { bd with //ZobristBoard = bd.ZobristBoard ^^^ Zobrist.Enpassant(bd.EnPassant)
                          EnPassant = OUTOFBOUNDS }
            else bd
        
        let bd = 
            if move |> IsPawnDoubleJump then 
                let ep = mfrom|>Square.PositionInDirectionUnsafe(move|>MovingPlayer|>Player.MyNorth)
                { bd with EnPassant = ep
                          //ZobristBoard = bd.ZobristBoard ^^^ Zobrist.Enpassant(ep) 
                          }
            else bd
        
        let bd = 
            if bd.WhosTurn = Player.Black then { bd with Fullmove = bd.Fullmove + 1 }
            else bd
        
        let bd = 
            if piece <> Piece.WPawn && piece <> Piece.BPawn && capture = Piece.EMPTY then 
                { bd with Fiftymove = bd.Fiftymove + 1 }
            else { bd with Fiftymove = 0 }
        
        let bd = 
            { bd with WhosTurn = bd.WhosTurn|>Player.PlayerOther
                      //ZobristBoard = bd.ZobristBoard ^^^ Zobrist.PlayerKey 
                      }
        { bd with Checkers = bd
                             |> AttacksTo(if bd.WhosTurn=Player.White then bd.WtKingPos else bd.BkKingPos)
                             &&& (if (bd.WhosTurn|>Player.PlayerOther)=Player.White then bd.WtPrBds else bd.BkPrBds) }
    
    let MoveUndo(bd : Brd) = 
        let movehist = bd.HistL.Head
        let bd = { bd with HistL = bd.HistL.Tail }
        let moveUndoing = movehist.Move
        
        let bd = 
            if moveUndoing |> IsPromotion then bd |> PieceChange (moveUndoing|>To) (moveUndoing|>MovingPiece)
            else bd
        
        let bd = bd |> PieceMove (moveUndoing|>To) (moveUndoing|>From)
        
        let bd = 
            if moveUndoing|>IsCapture then bd |> PieceAdd (moveUndoing|>To) (moveUndoing|>CapturedPiece)
            else bd
        
        let bd = 
            if moveUndoing |> IsCastle then 
                let movingPlayer = moveUndoing|>MovingPlayer
                let mto = moveUndoing|>To
                if moveUndoing|>MovingPlayer = Player.White && (moveUndoing|>To) = G1 then 
                    bd |> PieceMove F1 H1
                elif moveUndoing|>MovingPlayer= Player.White && (moveUndoing|>To) = C1 then 
                    bd |> PieceMove D1 A1
                elif moveUndoing|>MovingPlayer = Player.Black && (moveUndoing|>To) = G8 then 
                    bd |> PieceMove F8 H8
                else bd |> PieceMove D8 A8
            else bd
        
        let bd = 
            if moveUndoing |> IsEnPassant then 
                let tofile = moveUndoing|>To|>Square.ToFile
                let enpassantRank = moveUndoing|>MovingPlayer|>Player.MyRank(Rank5)
                bd 
                |> PieceAdd (tofile|>File.ToPosition(enpassantRank)) 
                       (PieceType.Pawn|>PieceType.ForPlayer(moveUndoing|>MovingPlayer|>Player.PlayerOther))
            else bd
        
        { bd with CastleRights = movehist.Castle
                  EnPassant = movehist.Enpassant
                  Fiftymove = movehist.FiftyCount
                  MovesSinceNull = movehist.MovesSinceNull
                  Fullmove = 
                      if bd.WhosTurn = Player.White then bd.Fullmove - 1
                      else bd.Fullmove
                  WhosTurn = bd.WhosTurn|>Player.PlayerOther
                  //ZobristBoard = movehist.Zobrist
                  //ZobPawn = movehist.ZobristPawn
                  //ZobMaterial = movehist.ZobristMaterial
                  Checkers = movehist.Checkers }
    
    let IsChk(bd : Brd) = bd.Checkers <> Bitboard.Empty
    
    let IsCheck (kingplayer : Player) (bd : Brd) = 
        let kingpos = if kingplayer=Player.White then bd.WtKingPos else bd.BkKingPos
        bd |> PositionAttacked kingpos (kingplayer|>Player.PlayerOther)
    
    let PieceInDirection (from : Square) (dir : Direction) (bd : Brd) = 
        let rec getpospc dist (pos : Square) pc = 
            if not (pos|>Square.IsInBounds) then pc, pos
            else 
                let npc = bd.PieceAt.[int (pos)]
                if npc <> Piece.EMPTY then npc, pos
                elif dir |> Direction.IsDirectionKnight then npc, pos
                else 
                    let npos = pos|>Square.PositionInDirection(dir)
                    getpospc (dist + 1) npos npc
        getpospc 1 (from|>Square.PositionInDirection(dir)) Piece.EMPTY
    
    //let PositionRepetitionCount(bd : Brd) = 
    //    //let currzob = bd.ZobristBoard
        
    //    let rec getct (mhl:MvHstry list) repcount = 
    //        let movehist = mhl.Head
            
    //        let nrc = 
    //            if movehist.Zobrist = currzob then repcount + 1
    //            else repcount
    //        if nrc >= 3 then nrc
    //        elif movehist.Move|>IsCapture then nrc
    //        elif movehist.Move|>MovingPieceType = PieceType.Pawn then nrc
    //        elif List.isEmpty mhl.Tail then nrc
    //        else getct mhl.Tail nrc
    //    if List.isEmpty bd.HistL then 1
    //    else getct bd.HistL 1
    
    //let IsDrawByRepetition(bd : Brd) = bd
    //                                   |> PositionRepetitionCount
    //                                   >= 3
    //let IsDrawBy50MoveRule(bd : Brd) = bd.Fiftymove >= 100
    
    let initPieceAtArray (bd : Brd) = 
        { bd with PieceAt = bd.PieceAt |> Array.map (fun p -> Piece.EMPTY)
                  WtPcCnt = bd.WtPcCnt|> Array.map (fun p -> 0)
                  BkPcCnt = bd.BkPcCnt|> Array.map (fun p -> 0)
                  WtKingPos = OUTOFBOUNDS
                  BkKingPos = OUTOFBOUNDS
                  PieceTypes = bd.PieceTypes |> Array.map (fun p -> Bitboard.Empty)
                  WtPrBds = Bitboard.Empty
                  BkPrBds = Bitboard.Empty
                  PieceLocationsAll = Bitboard.Empty
                  //PcSq = PhasedScore.Create 0 0 
                  }
    
    let LastTwoMovesNull(bd : Brd) = 
        bd.MovesSinceNull = 0 && bd.HistL.Length >= 2 && bd.HistL.Tail.Head.Move = MoveEmpty
    
    let MoveNullUndo(bd : Brd) = 
        let movehist = bd.HistL.Head
        { bd with HistL = bd.HistL.Tail
                  CastleRights = movehist.Castle
                  EnPassant = movehist.Enpassant
                  Fiftymove = movehist.FiftyCount
                  MovesSinceNull = movehist.MovesSinceNull
                  WhosTurn = bd.WhosTurn|>Player.PlayerOther
                  //ZobristBoard = movehist.Zobrist
                  //ZobPawn = movehist.ZobristPawn
                  //ZobMaterial = movehist.ZobristMaterial
                  Checkers = movehist.Checkers }
    
    let MoveNullApply(bd : Brd) = 
        let mhl = (MhSet MoveEmpty bd.EnPassant bd.CastleRights bd.Fiftymove bd.MovesSinceNull (*bd.ZobristBoard bd.ZobPawn bd.ZobMaterial*) bd.Checkers)::bd.HistL
        let bd = 
            { bd with HistL = mhl
                      MovesSinceNull = 0 }
        
        let bd = 
            if bd.EnPassant|>Square.IsInBounds then 
                { bd with //ZobristBoard = bd.ZobristBoard ^^^ Zobrist.Enpassant(bd.EnPassant)
                          EnPassant = (OUTOFBOUNDS) }
            else bd
        
        { bd with WhosTurn = bd.WhosTurn|>Player.PlayerOther
                  //ZobristBoard = bd.ZobristBoard ^^^ Zobrist.PlayerKey
                  Checkers = bd
                             |> AttacksTo(if bd.WhosTurn=Player.White then bd.WtKingPos else bd.BkKingPos)
                             &&& (if (bd.WhosTurn|>Player.PlayerOther)=Player.White then bd.WtPrBds else bd.BkPrBds) }
    
    let HistMove (movesAgo : int) (bd : Brd) = (List.item (movesAgo-1) bd.HistL).Move
    
    let HistoryMoves(bd : Brd) = bd.HistL|>List.map(fun h -> h.Move)|>List.rev
    
    //let ZobristPrevious(bd : Brd) = bd.HistL.Head.Zobrist

    let FENCurrent (fen : Fen) (bd : Brd) = 
        let rec rempc posl ibd = 
            if List.isEmpty posl then ibd
            else 
                let pos = posl.Head
                let pc = bd.PieceAt.[int (pos)]
                if pc = Piece.EMPTY then rempc posl.Tail ibd
                else rempc posl.Tail (ibd |> PieceRemove pos)
        
        let bd = rempc (SQUARES|>List.ofArray) bd
        let bd = { bd with HistL = [] }
        let bd = bd |> initPieceAtArray
        
        let rec addpc posl ibd = 
            if List.isEmpty posl then ibd
            else 
                let pos = posl.Head
                let pc = fen.Pieceat.[int (pos)]
                if pc = Piece.EMPTY then addpc posl.Tail ibd
                else addpc posl.Tail (ibd |> PieceAdd pos pc)
        
        let bd = addpc (SQUARES|>List.ofArray) bd
        { bd with CastleRights = 
                      Cf0 ||| (if fen.CastleWS then CstlFlgs.WhiteShort
                               else Cf0) ||| (if fen.CastleWL then CstlFlgs.WhiteLong
                                              else Cf0) ||| (if fen.CastleBS then CstlFlgs.BlackShort
                                                             else Cf0)
                      ||| (if fen.CastleBL then CstlFlgs.BlackLong
                           else Cf0)
                  WhosTurn = fen.Whosturn
                  EnPassant = fen.Enpassant
                  Fiftymove = fen.Fiftymove
                  Fullmove = fen.Fullmove
                  //ZobristBoard = BoardZob(bd)
                  //ZobPawn = BoardZobPawn(bd)
                  //ZobMaterial = BoardZobMaterial(bd)
                  Checkers = bd
                             |> AttacksTo(if bd.WhosTurn=Player.White then bd.WtKingPos else bd.BkKingPos)
                             &&& (if (bd.WhosTurn|>Player.PlayerOther)=Player.White then bd.WtPrBds else bd.BkPrBds) }

    let Create2 fen = 
        let bd = Create()
        
        //let bd = { bd with PcSqEvaluator = pcSqEvaluator }
        let bd = bd |> initPieceAtArray
        let bd = bd |> FENCurrent fen
        bd
    
    let Create3 fen prevMoves pcSqEvaluator = 
        let rec getbd mvl ibd = 
            if List.isEmpty mvl then ibd
            else getbd mvl.Tail (ibd |> MoveApply mvl.Head)
        getbd prevMoves (Create2 fen)


