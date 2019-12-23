namespace FsChessPgn.Data

module MoveGenerate = 
    
    let private legal(board: Brd) (mvs:Move list) =
        let me = board.WhosTurn

        let filt mv = 
            let bd = board |> Board.MoveApply(mv)
            let resultsInCheck = bd |> Board.IsCheck(me)
            not resultsInCheck
        mvs |> List.filter filt

    let KingMoves (board : Brd) :Move list = 
        let me = board.WhosTurn
    
        let targetLocations = 
            (if me=Player.Black then board.WtPrBds else board.BkPrBds)|||(~~~board.PieceLocationsAll)
    
        let kingPos = if me=Player.White then board.WtKingPos else board.BkKingPos
    
        let rec getKingAttacks att mvl = 
            if att = Bitboard.Empty then mvl
            else 
                let attPos, natt = Bitboard.PopFirst(att)
                let mv = Move.Create kingPos attPos board.PieceAt.[int (kingPos)] board.PieceAt.[int (attPos)]
                getKingAttacks natt (mv :: mvl)
    
        let attacks = Attacks.KingAttacks(kingPos) &&& targetLocations
        let mvl = getKingAttacks attacks []
        
        mvl|>legal(board)
    
    let CastleMoves (board : Brd) :Move list = 
        let checkerCount = board.Checkers |> Bitboard.BitCount
        if (checkerCount > 1) || (board |> Board.IsChk) then []
        else
            let mvl =
                if board.WhosTurn = Player.White then 
                    let mvl1 = 
                        let sqatt = board
                                    |> Board.SquareAttacked E1 Player.Black
                                    || board |> Board.SquareAttacked F1 Player.Black
                                    || board |> Board.SquareAttacked G1 Player.Black
                        let sqemp = 
                            board.PieceAt.[int (F1)] = Piece.EMPTY 
                            && board.PieceAt.[int (G1)] = Piece.EMPTY
                        if (int (board.CastleRights &&& CstlFlgs.WhiteShort) <> 0 
                            && board.PieceAt.[int (E1)] = Piece.WKing 
                            && board.PieceAt.[int (H1)] = Piece.WRook && sqemp && not sqatt) then 
                            let mv = 
                                Move.Create E1 G1 board.PieceAt.[int (E1)] 
                                    board.PieceAt.[int (G1)]
                            [mv]
                        else []

                    let sqatt = board
                                |> Board.SquareAttacked E1 Player.Black
                                || board |> Board.SquareAttacked D1 Player.Black
                                || board |> Board.SquareAttacked C1 Player.Black
                    let sqemp = 
                        board.PieceAt.[int (B1)] = Piece.EMPTY 
                        && board.PieceAt.[int (C1)] = Piece.EMPTY 
                        && board.PieceAt.[int (D1)] = Piece.EMPTY
                    if (int (board.CastleRights &&& CstlFlgs.WhiteLong) <> 0 
                        && board.PieceAt.[int (E1)] = Piece.WKing 
                        && board.PieceAt.[int (A1)] = Piece.WRook && sqemp && not sqatt) then 
                        let mv = 
                            Move.Create E1 C1 board.PieceAt.[int (E1)] 
                                board.PieceAt.[int (C1)]
                        mv :: mvl1
                    else mvl1
                else 
                    let mvl2 = 
                        let sqatt = board
                                    |> Board.SquareAttacked E8 Player.White
                                    || board |> Board.SquareAttacked F8 Player.White
                                    || board |> Board.SquareAttacked G8 Player.White
                        let sqemp = 
                            board.PieceAt.[int (F8)] = Piece.EMPTY 
                            && board.PieceAt.[int (G8)] = Piece.EMPTY
                        if (int (board.CastleRights &&& CstlFlgs.BlackShort) <> 0 
                            && board.PieceAt.[int (E8)] = Piece.BKing 
                            && board.PieceAt.[int (H8)] = Piece.BRook && sqemp && not sqatt) then 
                            let mv = 
                                Move.Create E8 G8 board.PieceAt.[int (E8)] 
                                    board.PieceAt.[int (G8)]
                            [mv]
                        else []

                    let sqatt = board
                                |> Board.SquareAttacked E8 Player.White
                                || board |> Board.SquareAttacked D8 Player.White
                                || board |> Board.SquareAttacked C8 Player.White
                    let sqemp = 
                        board.PieceAt.[int (B8)] = Piece.EMPTY 
                        && board.PieceAt.[int (C8)] = Piece.EMPTY 
                        && board.PieceAt.[int (D8)] = Piece.EMPTY
                    if (int (board.CastleRights &&& CstlFlgs.BlackLong) <> 0 
                        && board.PieceAt.[int (E8)] = Piece.BKing 
                        && board.PieceAt.[int (A8)] = Piece.BRook && sqemp && not sqatt) then 
                        let mv = 
                            Move.Create E8 C8 board.PieceAt.[int (E8)] 
                                board.PieceAt.[int (C8)]
                        mv :: mvl2
                    else mvl2
            mvl|>legal(board)

    let private pcMoves(board : Brd) (pt:PieceType) (fnsqbb: (Square -> Bitboard -> Bitboard)) :Move list =
        let me = board.WhosTurn
        let kingPos = if me=Player.White then board.WtKingPos else board.BkKingPos
    
        let targetLocations = 
            let checkerCount = board.Checkers |> Bitboard.BitCount
            if checkerCount = 1 then 
                let checkerPos = board.Checkers |> Bitboard.NorthMostPosition
                let evasionTargets = 
                    (kingPos |> Square.Between(checkerPos)) ||| (checkerPos |> Square.ToBitboard)
                ((if me=Player.Black then board.WtPrBds else board.BkPrBds)|||(~~~board.PieceLocationsAll)) &&& evasionTargets
            else
                (if me=Player.Black then board.WtPrBds else board.BkPrBds)|||(~~~board.PieceLocationsAll)

        let rec getAttacks psns imvl = 
            if psns = Bitboard.Empty || targetLocations = Bitboard.Empty then imvl
            else 
                let piecepos, npsns = Bitboard.PopFirst(psns)
                let piece = board.PieceAt.[int (piecepos)]
                let atts = (fnsqbb piecepos board.PieceLocationsAll) &&& targetLocations 
    
                let rec getAtts att jmvl = 
                    if att = Bitboard.Empty then jmvl
                    else 
                        let attPos, natt = Bitboard.PopFirst(att)
                        let mv = Move.Create piecepos attPos piece board.PieceAt.[int (attPos)]
                        getAtts natt (mv :: jmvl)
    
                let nimvl = getAtts atts imvl
                getAttacks npsns nimvl
        
        let piecePositions = 
            (if me=Player.White then board.WtPrBds else board.BkPrBds) &&& board.PieceTypes.[int (pt)] 
        let mvl = getAttacks piecePositions []
        mvl|>legal(board)
    
    let KnightMoves(board : Brd) :Move list = 
        let checkerCount = board.Checkers |> Bitboard.BitCount
        if checkerCount > 1 then []
        else
            let fnsqbb: (Square -> Bitboard -> Bitboard) = fun pp bb -> Attacks.KnightAttacks pp
            pcMoves board PieceType.Knight fnsqbb

    let BishopMoves(board : Brd) :Move list = 
        let checkerCount = board.Checkers |> Bitboard.BitCount
        if checkerCount > 1 then []
        else
            let fnsqbb: (Square -> Bitboard -> Bitboard) = fun pp bb -> Attacks.BishopAttacks pp bb
            pcMoves board PieceType.Bishop fnsqbb

    let RookMoves(board : Brd) :Move list = 
        let checkerCount = board.Checkers |> Bitboard.BitCount
        if checkerCount > 1 then []
        else
            let fnsqbb: (Square -> Bitboard -> Bitboard) = fun pp bb -> Attacks.RookAttacks pp bb
            pcMoves board PieceType.Rook fnsqbb

    let QueenMoves(board : Brd) :Move list = 
        let checkerCount = board.Checkers |> Bitboard.BitCount
        if checkerCount > 1 then []
        else
            let fnsqbb: (Square -> Bitboard -> Bitboard) = fun pp bb -> Attacks.QueenAttacks pp bb
            pcMoves board PieceType.Queen fnsqbb

    let PawnMoves(board : Brd) =

        let checkerCount = board.Checkers |> Bitboard.BitCount
        if checkerCount > 1 then []
        else

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

            let kingPos = if me=Player.White then board.WtKingPos else board.BkKingPos

            let evasionTargets = 
                let checkerCount = board.Checkers |> Bitboard.BitCount
                if checkerCount = 1 then 
                    let checkerPos = board.Checkers |> Bitboard.NorthMostPosition
                    (kingPos |> Square.Between(checkerPos)) ||| (checkerPos |> Square.ToBitboard)
                else
                    ~~~Bitboard.Empty

            let piecePositions = (if me=Player.White then board.WtPrBds else board.BkPrBds) &&& board.PieceTypes.[int (PieceType.Pawn)]

            let captureLocations = if me=Player.Black then board.WtPrBds else board.BkPrBds
            
            let targLocations = 
                captureLocations &&& evasionTargets ||| (if board.EnPassant |> Square.IsInBounds then 
                                                            board.EnPassant |> Square.ToBitboard
                                                         else Bitboard.Empty)
            
            let rec getPcaps capDir att imvl = 
                if att = Bitboard.Empty then 
                    if capDir = mypawneast then 
                        let attacks = (piecePositions |> Bitboard.Shift(mypawnwest)) &&& targLocations
                        getPcaps mypawnwest attacks imvl
                    else imvl
                else 
                    let targetpos, natt = Bitboard.PopFirst(att)
                    let piecepos = 
                        targetpos |> Square.PositionInDirectionUnsafe(capDir |> Direction.Opposite)
                    if (targetpos |> Square.ToRank) = myrank8 then 
                        let mv = 
                            Move.CreateProm piecepos targetpos board.PieceAt.[int (piecepos)] 
                                board.PieceAt.[int (targetpos)] PieceType.Queen
                        let imvl = mv :: imvl
                        let mv = 
                            Move.CreateProm piecepos targetpos board.PieceAt.[int (piecepos)] 
                                board.PieceAt.[int (targetpos)] PieceType.Rook
                        let imvl = mv :: imvl
                        let mv = 
                            Move.CreateProm piecepos targetpos board.PieceAt.[int (piecepos)] 
                                board.PieceAt.[int (targetpos)] PieceType.Bishop
                        let imvl = mv :: imvl
                        let mv = 
                            Move.CreateProm piecepos targetpos board.PieceAt.[int (piecepos)] 
                                board.PieceAt.[int (targetpos)] PieceType.Knight
                        let imvl = mv :: imvl
                        getPcaps capDir natt imvl
                    else 
                        let mv = 
                            Move.Create piecepos targetpos board.PieceAt.[int (piecepos)] 
                                board.PieceAt.[int (targetpos)]
                        let imvl = mv :: imvl
                        getPcaps capDir natt imvl
            
            let attacks = (piecePositions |> Bitboard.Shift(mypawneast)) &&& targLocations
            let pcaps = getPcaps mypawneast attacks []
            
            let rec getPones att imvl = 
                if att = Bitboard.Empty then imvl
                else 
                    let piecepos, natt = Bitboard.PopFirst(att)
                    let targetpos = piecepos |> Square.PositionInDirectionUnsafe(mypawnnorth)
                    if (targetpos |> Square.ToRank) = myrank8 then 
                        let mv = 
                            Move.CreateProm piecepos targetpos board.PieceAt.[int (piecepos)] 
                                board.PieceAt.[int (targetpos)] PieceType.Queen
                        let imvl = mv :: imvl
                        let mv = 
                            Move.CreateProm piecepos targetpos board.PieceAt.[int (piecepos)] 
                                board.PieceAt.[int (targetpos)] PieceType.Rook
                        let imvl = mv :: imvl
                        let mv = 
                            Move.CreateProm piecepos targetpos board.PieceAt.[int (piecepos)] 
                                board.PieceAt.[int (targetpos)] PieceType.Bishop
                        let imvl = mv :: imvl
                        let mv = 
                            Move.CreateProm piecepos targetpos board.PieceAt.[int (piecepos)] 
                                board.PieceAt.[int (targetpos)] PieceType.Knight
                        let imvl = mv :: imvl
                        getPones natt imvl
                    else 
                        let mv = 
                            Move.Create piecepos targetpos board.PieceAt.[int (piecepos)] 
                                board.PieceAt.[int (targetpos)]
                        let imvl = mv :: imvl
                        getPones natt imvl
            
            let moveLocations = (~~~board.PieceLocationsAll) &&& evasionTargets
            let attacks = (moveLocations |> Bitboard.Shift(mypawnsouth)) &&& piecePositions
            let pones = getPones attacks []
            
            let rec getPtwos att imvl = 
                if att = Bitboard.Empty then imvl
                else 
                    let piecepos, natt = Bitboard.PopFirst(att)
                    
                    let targetpos = 
                        piecepos
                        |> Square.PositionInDirectionUnsafe(mypawnnorth)
                        |> Square.PositionInDirectionUnsafe(mypawnnorth)
                    
                    let mv = 
                        Move.Create piecepos targetpos board.PieceAt.[int (piecepos)] 
                            board.PieceAt.[int (targetpos)]
                    let imvl = mv :: imvl
                    getPtwos natt imvl
            
            let attacks = 
                (myrank2 |> Rank.ToBitboard) &&& piecePositions 
                &&& ((moveLocations |> Bitboard.Shift(mypawnsouth)) |> Bitboard.Shift(mypawnsouth)) 
                &&& (~~~board.PieceLocationsAll |> Bitboard.Shift(mypawnsouth))
            let ptwos = getPtwos attacks []

            (ptwos@pones@pcaps)|>legal(board)

    let AllMoves(board : Brd) :Move list = 
        let checkerCount = board.Checkers |> Bitboard.BitCount
        if checkerCount > 1 then board|>KingMoves
        else
            (board|>CastleMoves) @
            (board|>PawnMoves) @
            (board|>KnightMoves) @
            (board|>BishopMoves) @
            (board|>RookMoves) @
            (board|>QueenMoves) @
            (board|>KingMoves)

    let IsDrawByStalemate(bd : Brd) = 
        if not (bd |> Board.IsChk) then AllMoves(bd) |> List.isEmpty
        else false
    
    let IsMate(bd : Brd) = 
        if bd |> Board.IsChk then AllMoves(bd) |> List.isEmpty
        else false


