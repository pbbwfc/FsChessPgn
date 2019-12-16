namespace FsChessPgn.Data

module Board = 
    
    let PieceMove (mfrom : Square) mto (bd : Brd) = 
        let piece = bd.PieceAt.[mfrom]
        let player = piece|>Piece.PieceToPlayer
        let pieceType = piece|>Piece.ToPieceType
        let pieceat = bd.PieceAt|>List.mapi(fun i p -> if i=int(mto) then piece elif i = int(mfrom) then Piece.EMPTY else p)
        let posBits = (mfrom |> Square.ToBitboard) ||| (mto |> Square.ToBitboard)
        let piecetypes = bd.PieceTypes|>List.mapi(fun i p -> if i=int(pieceType) then p ^^^ posBits else p)
        let wtprbds = if player=Player.White then bd.WtPrBds ^^^ posBits else bd.WtPrBds
        let bkprbds = if player=Player.Black then bd.BkPrBds ^^^ posBits else bd.BkPrBds
        let pieceLocationsAll = bd.PieceLocationsAll ^^^ posBits
        let wtkingpos = if pieceType = PieceType.King && player=Player.White then mto else bd.WtKingPos
        let bkkingpos = if pieceType = PieceType.King && player=Player.Black then mto else bd.BkKingPos
        { bd with PieceAt = pieceat
                  PieceTypes = piecetypes
                  WtPrBds = wtprbds
                  BkPrBds = bkprbds
                  PieceLocationsAll = pieceLocationsAll
                  WtKingPos = wtkingpos
                  BkKingPos = bkkingpos }
 
    let PieceAdd pos (piece : Piece) (bd : Brd) = 
        let player = piece |> Piece.PieceToPlayer
        let pieceType = piece |> Piece.ToPieceType
        
        let pieceat = 
            bd.PieceAt |> List.mapi (fun i p -> 
                              if i = int (pos) then piece
                              else p)
        
        let posBits = pos |> Square.ToBitboard
        
        let piecetypes = 
            bd.PieceTypes |> List.mapi (fun i p -> 
                                 if i = int (piece |> Piece.ToPieceType) then p ||| posBits
                                 else p)
        
        let piecelocationsall = bd.PieceLocationsAll ||| posBits
        let wtprbds = if (piece |> Piece.PieceToPlayer)=Player.White then bd.WtPrBds ||| posBits else bd.WtPrBds
        let bkprbds = if (piece |> Piece.PieceToPlayer)=Player.Black then bd.BkPrBds ||| posBits else bd.BkPrBds
        let wtkingpos = if pieceType = PieceType.King && player=Player.White then pos else bd.WtKingPos
        let bkkingpos = if pieceType = PieceType.King && player=Player.Black then pos else bd.BkKingPos
        { bd with PieceAt = pieceat
                  PieceTypes = piecetypes
                  PieceLocationsAll = piecelocationsall
                  WtPrBds = wtprbds
                  BkPrBds = bkprbds
                  WtKingPos = wtkingpos
                  BkKingPos = bkkingpos }

    let PieceRemove (pos : Square) (bd : Brd) = 
        let piece = bd.PieceAt.[int (pos)]
        let player = piece |> Piece.PieceToPlayer
        let pieceType = piece |> Piece.ToPieceType
        
        let pieceat = 
            bd.PieceAt |> List.mapi (fun i p -> 
                              if i = int (pos) then Piece.EMPTY
                              else p)
        let notPosBits = ~~~(pos |> Square.ToBitboard)
        
        let piecetypes = 
            bd.PieceTypes |> List.mapi (fun i p -> 
                                 if i = int (pieceType) then p &&& notPosBits
                                 else p)
        let wtprbds = if player=Player.White then bd.WtPrBds &&& notPosBits else bd.WtPrBds
        let bkprbds = if player=Player.Black then bd.BkPrBds &&& notPosBits else bd.BkPrBds
        let piecelocationsall = bd.PieceLocationsAll &&& notPosBits
        { bd with PieceAt = pieceat
                  PieceTypes = piecetypes
                  WtPrBds = wtprbds
                  BkPrBds = bkprbds
                  PieceLocationsAll = piecelocationsall
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
        let mfrom = move|>Move.From
        let mto = move|>Move.To
        let piece = move|>Move.MovingPiece
        let capture = move|>Move.CapturedPiece

        let bd = 
            { bd with MovesSinceNull = bd.MovesSinceNull + 1 }
        
        let bd = 
            if capture <> Piece.EMPTY then bd |> PieceRemove(mto)
            else bd
        
        let bd = bd |> PieceMove mfrom mto
        
        let bd = 
            if move |> Move.IsPromotion then bd |> PieceChange mto (move |> Move.Promote)
            else bd
        
        let bd = 
            if move |> Move.IsCastle then 
                if piece = Piece.WKing && mfrom = E1 && mto = G1 then 
                    bd |> PieceMove H1 F1
                elif piece = Piece.WKing && mfrom = E1 && mto = C1 then 
                    bd |> PieceMove A1 D1
                elif piece = Piece.BKing && mfrom = E8 && mto = G8 then 
                    bd |> PieceMove H8 F8
                else bd |> PieceMove A8 D8
            else bd
        
        let bd = 
            if bd.CastleRights <> CstlFlgs.EMPTY then 
                if (bd.CastleRights &&& CstlFlgs.WhiteShort) <> CstlFlgs.EMPTY 
                   && (piece = Piece.WKing || mfrom = H1) then 
                    { bd with CastleRights = bd.CastleRights &&& ~~~CstlFlgs.WhiteShort}
                elif (bd.CastleRights &&& CstlFlgs.WhiteLong) <> CstlFlgs.EMPTY 
                     && (piece = Piece.WKing || mfrom = A1) then 
                    { bd with CastleRights = bd.CastleRights &&& ~~~CstlFlgs.WhiteLong}
                elif (bd.CastleRights &&& CstlFlgs.BlackShort) <> CstlFlgs.EMPTY
                     && (piece = Piece.BKing || mfrom = H8) then 
                    { bd with CastleRights = bd.CastleRights &&& ~~~CstlFlgs.BlackShort}
                elif (bd.CastleRights &&& CstlFlgs.BlackLong) <> CstlFlgs.EMPTY
                     && (piece = Piece.BKing || mfrom = A8) then 
                    { bd with CastleRights = bd.CastleRights &&& ~~~CstlFlgs.BlackLong}
                else bd
            else bd
        
        let bd = 
            if move |> Move.IsEnPassant then 
                bd |> PieceRemove(Sq(mto|>Square.ToFile,move|>Move.MovingPlayer|>Player.MyRank(Rank5)))
            else bd
        
        let bd = 
            if bd.EnPassant|>Square.IsInBounds then 
                { bd with EnPassant = OUTOFBOUNDS }
            else bd
        
        let bd = 
            if move |> Move.IsPawnDoubleJump then 
                let ep = mfrom|>Square.PositionInDirectionUnsafe(move|>Move.MovingPlayer|>Player.MyNorth)
                { bd with EnPassant = ep}
            else bd
        
        let bd = 
            if bd.WhosTurn = Player.Black then { bd with Fullmove = bd.Fullmove + 1 }
            else bd
        
        let bd = 
            if piece <> Piece.WPawn && piece <> Piece.BPawn && capture = Piece.EMPTY then 
                { bd with Fiftymove = bd.Fiftymove + 1 }
            else { bd with Fiftymove = 0 }
        
        let bd = 
            { bd with WhosTurn = bd.WhosTurn|>Player.PlayerOther}
        { bd with Checkers = bd
                             |> AttacksTo(if bd.WhosTurn=Player.White then bd.WtKingPos else bd.BkKingPos)
                             &&& (if (bd.WhosTurn|>Player.PlayerOther)=Player.White then bd.WtPrBds else bd.BkPrBds) }
    
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
    
    let FromFEN (fen : Fen) = 
        let bd = BrdEMP

        let rec addpc posl ibd = 
            if List.isEmpty posl then ibd
            else 
                let pos = posl.Head
                let pc = fen.Pieceat.[pos]
                if pc = Piece.EMPTY then addpc posl.Tail ibd
                else addpc posl.Tail (ibd |> PieceAdd pos pc)
        
        let bd = addpc SQUARES bd
        { bd with CastleRights = 
                      CstlFlgs.EMPTY ||| (if fen.CastleWS then CstlFlgs.WhiteShort
                               else CstlFlgs.EMPTY) ||| (if fen.CastleWL then CstlFlgs.WhiteLong
                                              else CstlFlgs.EMPTY) ||| (if fen.CastleBS then CstlFlgs.BlackShort
                                                             else CstlFlgs.EMPTY)
                      ||| (if fen.CastleBL then CstlFlgs.BlackLong
                           else CstlFlgs.EMPTY)
                  WhosTurn = fen.Whosturn
                  EnPassant = fen.Enpassant
                  Fiftymove = fen.Fiftymove
                  Fullmove = fen.Fullmove
                  Checkers = bd
                             |> AttacksTo(if bd.WhosTurn=Player.White then bd.WtKingPos else bd.BkKingPos)
                             &&& (if (bd.WhosTurn|>Player.PlayerOther)=Player.White then bd.WtPrBds else bd.BkPrBds) }

    let ToStr (bd : Brd) = bd|>FEN.FromBd|>FEN.ToStr
    
    let Start = FEN.Start|>FromFEN
