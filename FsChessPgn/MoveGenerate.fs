namespace FsChessPgn.Data

module MoveGenerate = 
    let Create (pfrom : Square) (pto : Square) (piece : Piece) (captured : Piece) = 
        (int (pfrom) ||| (int (pto) <<< 6) ||| (int (piece) <<< 12) ||| (int (captured) <<< 16))
    let CreateProm (pfrom : Square) (pto : Square) (piece : Piece) (captured : Piece) (promoteType : PieceType) = 
        (int (pfrom) ||| (int (pto) <<< 6) ||| (int (piece) <<< 12) ||| (int (captured) <<< 16) 
         ||| (int (promoteType) <<< 20))
    
    let GenCapsNonCaps (board : Brd) captures :Move list = 
        let mypawnwest = 
            if board.WhosTurn = Player.White then Direction.DirNW
            else Direction.DirSW
        
        let mypawneast = 
            if board.WhosTurn = Player.White then Direction.DirNE
            else Direction.DirSE
        
        let mypawnnorth = 
            if board.WhosTurn = Player.White then Direction.DirN
            else Direction.DirS
        
        let mypawnsouth = 
            if board.WhosTurn = Player.White then Direction.DirS
            else Direction.DirN
        
        let myrank8 = 
            if board.WhosTurn = Player.White then Rank8
            else Rank1
        
        let myrank2 = 
            if board.WhosTurn = Player.White then Rank2
            else Rank7
        
        let me = board.WhosTurn
        
        let targetLocations = 
            if captures then (if (me |> Player.PlayerOther)=Player.White then board.WtPrBds else board.BkPrBds)
            else ~~~board.PieceLocationsAll
        
        let kingPos = if me=Player.White then board.WtKingPos else board.BkKingPos
        
        let rec getKingAttacks att mvl = 
            if att = Bitboard.Empty then mvl
            else 
                let attPos, natt = Bitboard.PopFirst(att)
                let mv = Create kingPos attPos board.PieceAt.[int (kingPos)] board.PieceAt.[int (attPos)]
                getKingAttacks natt (mv :: mvl)
        
        let attacks = Attacks.KingAttacks(kingPos) &&& targetLocations
        let mvl = getKingAttacks attacks []
        
        let targetLocations, evasionTargets, fin = 
            if board |> Board.IsChk then 
                let checkerCount = board.Checkers |> Bitboard.BitCount
                if checkerCount = 1 then 
                    let checkerPos = board.Checkers |> Bitboard.NorthMostPosition
                    let evasionTargets = 
                        (kingPos |> Square.Between(checkerPos)) ||| (checkerPos |> Square.ToBitboard)
                    let targetlocations = targetLocations &&& evasionTargets
                    targetlocations, evasionTargets, false
                else targetLocations, ~~~Bitboard.Empty, true
            else targetLocations, ~~~Bitboard.Empty, false
        if fin then mvl
        else 
            let rec getOtherAttacks psns imvl = 
                if psns = Bitboard.Empty || targetLocations = Bitboard.Empty then imvl
                else 
                    let piecepos, npsns = Bitboard.PopFirst(psns)
                    let piece = board.PieceAt.[int (piecepos)]
                    let pieceType = piece |> Piece.ToPieceType
                    
                    let atts = 
                        if pieceType = PieceType.Knight then Attacks.KnightAttacks(piecepos) &&& targetLocations
                        elif pieceType = PieceType.Bishop then 
                            (Attacks.BishopAttacks piecepos board.PieceLocationsAll) &&& targetLocations
                        elif pieceType = PieceType.Rook then 
                            (Attacks.RookAttacks piecepos board.PieceLocationsAll) &&& targetLocations
                        elif pieceType = PieceType.Queen then 
                            (Attacks.QueenAttacks piecepos board.PieceLocationsAll) &&& targetLocations
                        elif pieceType = PieceType.King then Attacks.KingAttacks(piecepos) &&& targetLocations
                        else failwith "invalid piece type"
                    
                    let rec getAtts att jmvl = 
                        if att = Bitboard.Empty then jmvl
                        else 
                            let attPos, natt = Bitboard.PopFirst(att)
                            let mv = Create piecepos attPos piece board.PieceAt.[int (attPos)]
                            getAtts natt (mv :: jmvl)
                    
                    let nimvl = getAtts atts imvl
                    getOtherAttacks npsns nimvl
            
            let piecePositions = 
                (if me=Player.White then board.WtPrBds else board.BkPrBds) &&& ~~~board.PieceTypes.[int (PieceType.Pawn)] 
                &&& ~~~board.PieceTypes.[int (PieceType.King)]
            let mvl = getOtherAttacks piecePositions mvl
            let piecePositions = (if me=Player.White then board.WtPrBds else board.BkPrBds) &&& board.PieceTypes.[int (PieceType.Pawn)]
            
            let mvl = 
                if piecePositions <> Bitboard.Empty then 
                    if captures then 
                        let targetLocations = 
                            (if (me |> Player.PlayerOther)=Player.White then board.WtPrBds else board.BkPrBds) ||| (if board.EnPassant|> Square.IsInBounds then board.EnPassant|> Square.ToBitboard else Bitboard.Empty)
                        
                        let targetLocations = 
                            targetLocations &&& evasionTargets ||| (if board.EnPassant |> Square.IsInBounds then 
                                                                        board.EnPassant |> Square.ToBitboard
                                                                    else Bitboard.Empty)
                        
                        let rec getPcaps capDir att imvl = 
                            if att = Bitboard.Empty then 
                                if capDir = mypawneast then 
                                    let attacks = (piecePositions |> Bitboard.Shift(mypawnwest)) &&& targetLocations
                                    getPcaps mypawnwest attacks imvl
                                else imvl
                            else 
                                let targetpos, natt = Bitboard.PopFirst(att)
                                let piecepos = 
                                    targetpos |> Square.PositionInDirectionUnsafe(capDir |> Direction.Opposite)
                                if (targetpos |> Square.ToRank) = myrank8 then 
                                    let mv = 
                                        CreateProm piecepos targetpos board.PieceAt.[int (piecepos)] 
                                            board.PieceAt.[int (targetpos)] PieceType.Queen
                                    let imvl = mv :: imvl
                                    let mv = 
                                        CreateProm piecepos targetpos board.PieceAt.[int (piecepos)] 
                                            board.PieceAt.[int (targetpos)] PieceType.Rook
                                    let imvl = mv :: imvl
                                    let mv = 
                                        CreateProm piecepos targetpos board.PieceAt.[int (piecepos)] 
                                            board.PieceAt.[int (targetpos)] PieceType.Bishop
                                    let imvl = mv :: imvl
                                    let mv = 
                                        CreateProm piecepos targetpos board.PieceAt.[int (piecepos)] 
                                            board.PieceAt.[int (targetpos)] PieceType.Knight
                                    let imvl = mv :: imvl
                                    getPcaps capDir natt imvl
                                else 
                                    let mv = 
                                        Create piecepos targetpos board.PieceAt.[int (piecepos)] 
                                            board.PieceAt.[int (targetpos)]
                                    let imvl = mv :: imvl
                                    getPcaps capDir natt imvl
                        
                        let attacks = (piecePositions |> Bitboard.Shift(mypawneast)) &&& targetLocations
                        let nmvl = getPcaps mypawneast attacks mvl
                        nmvl
                    else 
                        let rec getPones att imvl = 
                            if att = Bitboard.Empty then imvl
                            else 
                                let piecepos, natt = Bitboard.PopFirst(att)
                                let targetpos = piecepos |> Square.PositionInDirectionUnsafe(mypawnnorth)
                                if (targetpos |> Square.ToRank) = myrank8 then 
                                    let mv = 
                                        CreateProm piecepos targetpos board.PieceAt.[int (piecepos)] 
                                            board.PieceAt.[int (targetpos)] PieceType.Queen
                                    let imvl = mv :: imvl
                                    let mv = 
                                        CreateProm piecepos targetpos board.PieceAt.[int (piecepos)] 
                                            board.PieceAt.[int (targetpos)] PieceType.Rook
                                    let imvl = mv :: imvl
                                    let mv = 
                                        CreateProm piecepos targetpos board.PieceAt.[int (piecepos)] 
                                            board.PieceAt.[int (targetpos)] PieceType.Bishop
                                    let imvl = mv :: imvl
                                    let mv = 
                                        CreateProm piecepos targetpos board.PieceAt.[int (piecepos)] 
                                            board.PieceAt.[int (targetpos)] PieceType.Knight
                                    let imvl = mv :: imvl
                                    getPones natt imvl
                                else 
                                    let mv = 
                                        Create piecepos targetpos board.PieceAt.[int (piecepos)] 
                                            board.PieceAt.[int (targetpos)]
                                    let imvl = mv :: imvl
                                    getPones natt imvl
                        
                        let targetLocations = ~~~board.PieceLocationsAll
                        let targetLocations = targetLocations &&& evasionTargets
                        let attacks = (targetLocations |> Bitboard.Shift(mypawnsouth)) &&& piecePositions
                        let mvl = getPones attacks mvl
                        
                        let rec getPtwos att imvl = 
                            if att = Bitboard.Empty then imvl
                            else 
                                let piecepos, natt = Bitboard.PopFirst(att)
                                
                                let targetpos = 
                                    piecepos
                                    |> Square.PositionInDirectionUnsafe(mypawnnorth)
                                    |> Square.PositionInDirectionUnsafe(mypawnnorth)
                                
                                let mv = 
                                    Create piecepos targetpos board.PieceAt.[int (piecepos)] 
                                        board.PieceAt.[int (targetpos)]
                                let imvl = mv :: imvl
                                getPtwos natt imvl
                        
                        let attacks = 
                            (myrank2 |> Rank.ToBitboard) &&& piecePositions 
                            &&& ((targetLocations |> Bitboard.Shift(mypawnsouth)) |> Bitboard.Shift(mypawnsouth)) 
                            &&& (~~~board.PieceLocationsAll |> Bitboard.Shift(mypawnsouth))
                        let mvl = getPtwos attacks mvl
                        mvl
                else mvl
            
            let mvl = 
                if not captures && not (board |> Board.IsChk) then 
                    if board.WhosTurn = Player.White then 
                        let mvl = 
                            let sqatt = board
                                        |> Board.PositionAttacked E1 Player.Black
                                        || board |> Board.PositionAttacked F1 Player.Black
                                        || board |> Board.PositionAttacked G1 Player.Black
                            let sqemp = 
                                board.PieceAt.[int (F1)] = Piece.EMPTY 
                                && board.PieceAt.[int (G1)] = Piece.EMPTY
                            if (int (board.CastleRights &&& CstlFlgs.WhiteShort) <> 0 
                                && board.PieceAt.[int (E1)] = Piece.WKing 
                                && board.PieceAt.[int (H1)] = Piece.WRook && sqemp && not sqatt) then 
                                let mv = 
                                    Create E1 G1 board.PieceAt.[int (E1)] 
                                        board.PieceAt.[int (G1)]
                                let mvl = mv :: mvl
                                mvl
                            else mvl
                        
                        let sqatt = board
                                    |> Board.PositionAttacked E1 Player.Black
                                    || board |> Board.PositionAttacked D1 Player.Black
                                    || board |> Board.PositionAttacked C1 Player.Black
                        let sqemp = 
                            board.PieceAt.[int (B1)] = Piece.EMPTY 
                            && board.PieceAt.[int (C1)] = Piece.EMPTY 
                            && board.PieceAt.[int (D1)] = Piece.EMPTY
                        if (int (board.CastleRights &&& CstlFlgs.WhiteLong) <> 0 
                            && board.PieceAt.[int (E1)] = Piece.WKing 
                            && board.PieceAt.[int (A1)] = Piece.WRook && sqemp && not sqatt) then 
                            let mv = 
                                Create E1 C1 board.PieceAt.[int (E1)] 
                                    board.PieceAt.[int (C1)]
                            let mvl = mv :: mvl
                            mvl
                        else mvl
                    else 
                        let mvl = 
                            let sqatt = board
                                        |> Board.PositionAttacked E8 Player.White
                                        || board |> Board.PositionAttacked F8 Player.White
                                        || board |> Board.PositionAttacked G8 Player.White
                            let sqemp = 
                                board.PieceAt.[int (F8)] = Piece.EMPTY 
                                && board.PieceAt.[int (G8)] = Piece.EMPTY
                            if (int (board.CastleRights &&& CstlFlgs.BlackShort) <> 0 
                                && board.PieceAt.[int (E8)] = Piece.BKing 
                                && board.PieceAt.[int (H8)] = Piece.BRook && sqemp && not sqatt) then 
                                let mv = 
                                    Create E8 G8 board.PieceAt.[int (E8)] 
                                        board.PieceAt.[int (G8)]
                                let mvl = mv :: mvl
                                mvl
                            else mvl
                        
                        let sqatt = board
                                    |> Board.PositionAttacked E8 Player.White
                                    || board |> Board.PositionAttacked D8 Player.White
                                    || board |> Board.PositionAttacked C8 Player.White
                        let sqemp = 
                            board.PieceAt.[int (B8)] = Piece.EMPTY 
                            && board.PieceAt.[int (C8)] = Piece.EMPTY 
                            && board.PieceAt.[int (D8)] = Piece.EMPTY
                        if (int (board.CastleRights &&& CstlFlgs.BlackLong) <> 0 
                            && board.PieceAt.[int (E8)] = Piece.BKing 
                            && board.PieceAt.[int (A8)] = Piece.BRook && sqemp && not sqatt) then 
                            let mv = 
                                Create E8 C8 board.PieceAt.[int (E8)] 
                                    board.PieceAt.[int (C8)]
                            let mvl = mv :: mvl
                            mvl
                        else mvl
                else mvl
            
            mvl
    
    let GenMoves(board : Brd) :Move list = 
        let capsl = GenCapsNonCaps board true
        let ncapsl = GenCapsNonCaps board false
        capsl @ ncapsl
    
    let GenMovesLegal(board : Brd) = 
        let me = board.WhosTurn
        let mvs = GenMoves(board)
        
        let filt mv = 
            let bd = board |> Board.MoveApply(mv)
            let resultsInCheck = bd |> Board.IsCheck(me)
            not resultsInCheck
        mvs |> List.filter filt
    
    let IsDrawByStalemate(bd : Brd) = 
        if not (bd |> Board.IsChk) then GenMovesLegal(bd) |> List.isEmpty
        else false
    
    let IsMate(bd : Brd) = 
        if bd |> Board.IsChk then GenMovesLegal(bd) |> List.isEmpty
        else false


