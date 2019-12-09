namespace fspgn.Data

open System

[<AutoOpen>]
module Types = 
    type Move = int
    let MoveEmpty = 0
    
    [<Flags>]
    type MoveFlags = 
        | Killer = 1
        | Capture = 2
        | Promote = 4
        | TransTable = 8
    
    type ChessMoveData = 
        { Move : Move
          SEE : int
          Score : int
          Flags : MoveFlags }

    let CMDemp = 
        { Move = MoveEmpty
          SEE = 0
          Score = 0
          Flags = enum<MoveFlags> (0) }
    
    type PieceType = 
        | EMPTY = 0
        | Pawn = 1
        | Knight = 2
        | Bishop = 3
        | Rook = 4
        | Queen = 5
        | King = 6
    
    type Piece = 
        | WPawn = 1
        | WKnight = 2
        | WBishop = 3
        | WRook = 4
        | WQueen = 5
        | WKing = 6
        | BPawn = 9
        | BKnight = 10
        | BBishop = 11
        | BRook = 12
        | BQueen = 13
        | BKing = 14
        | EMPTY = 0
    
    type Player = 
        | None = -1
        | White = 0
        | Black = 1
    
    type GameResultReason = 
        | NotDecided = 0
        | Checkmate = 1
        | Resign = 2
        | OutOfTime = 3
        | Adjudication = 4
        | Stalemate = 5
        | FiftyMoveRule = 6
        | InsufficientMaterial = 7
        | MutualAgreement = 8
        | Repetition = 9
        | Unknown = 10
        | IllegalMove = 11
    
    type GameResult = 
        | Draw = 0
        | WhiteWins = 1
        | BlackWins = -1
        | Open = 9
    
    type Position = 
        | A8 = 0
        | B8 = 1
        | C8 = 2
        | D8 = 3
        | E8 = 4
        | F8 = 5
        | G8 = 6
        | H8 = 7
        | A7 = 8
        | B7 = 9
        | C7 = 10
        | D7 = 11
        | E7 = 12
        | F7 = 13
        | G7 = 14
        | H7 = 15
        | A6 = 16
        | B6 = 17
        | C6 = 18
        | D6 = 19
        | E6 = 20
        | F6 = 21
        | G6 = 22
        | H6 = 23
        | A5 = 24
        | B5 = 25
        | C5 = 26
        | D5 = 27
        | E5 = 28
        | F5 = 29
        | G5 = 30
        | H5 = 31
        | A4 = 32
        | B4 = 33
        | C4 = 34
        | D4 = 35
        | E4 = 36
        | F4 = 37
        | G4 = 38
        | H4 = 39
        | A3 = 40
        | B3 = 41
        | C3 = 42
        | D3 = 43
        | E3 = 44
        | F3 = 45
        | G3 = 46
        | H3 = 47
        | A2 = 48
        | B2 = 49
        | C2 = 50
        | D2 = 51
        | E2 = 52
        | F2 = 53
        | G2 = 54
        | H2 = 55
        | A1 = 56
        | B1 = 57
        | C1 = 58
        | D1 = 59
        | E1 = 60
        | F1 = 61
        | G1 = 62
        | H1 = 63
        | OUTOFBOUNDS = 64
    
    type Rank = 
        | Rank8 = 0
        | Rank7 = 1
        | Rank6 = 2
        | Rank5 = 3
        | Rank4 = 4
        | Rank3 = 5
        | Rank2 = 6
        | Rank1 = 7
        | EMPTY = 8
    
    type File = 
        | FileA = 0
        | FileB = 1
        | FileC = 2
        | FileD = 3
        | FileE = 4
        | FileF = 5
        | FileG = 6
        | FileH = 7
        | EMPTY = 8
    
    type Direction = 
        | DirN = -8
        | DirE = 1
        | DirS = 8
        | DirW = -1
        | DirNE = -7
        | DirSE = 9
        | DirSW = 7
        | DirNW = -9
        | DirNNE = -15
        | DirEEN = -6
        | DirEES = 10
        | DirSSE = 17
        | DirSSW = 15
        | DirWWS = 6
        | DirWWN = -10
        | DirNNW = -17
    
    type Fen = 
        { Pieceat : Piece []
          Whosturn : Player
          CastleWS : bool
          CastleWL : bool
          CastleBS : bool
          CastleBL : bool
          Enpassant : Position
          Fiftymove : int
          Fullmove : int }
    
    [<Flags>]
    type CstlFlgs = 
        | WhiteShort = 1
        | WhiteLong = 2
        | BlackShort = 4
        | BlackLong = 8
        | All = 15
    
    [<Flags>]
    type Bitboard = 
        | A8 = 1UL
        | B8 = 2UL
        | C8 = 4UL
        | D8 = 8UL
        | E8 = 16UL
        | F8 = 32UL
        | G8 = 64UL
        | H8 = 128UL
        | A7 = 256UL
        | B7 = 512UL
        | C7 = 1024UL
        | D7 = 2048UL
        | E7 = 4096UL
        | F7 = 8192UL
        | G7 = 16384UL
        | H7 = 32768UL
        | A6 = 65536UL
        | B6 = 131072UL
        | C6 = 262144UL
        | D6 = 524288UL
        | E6 = 1048576UL
        | F6 = 2097152UL
        | G6 = 4194304UL
        | H6 = 8388608UL
        | A5 = 16777216UL
        | B5 = 33554432UL
        | C5 = 67108864UL
        | D5 = 134217728UL
        | E5 = 268435456UL
        | F5 = 536870912UL
        | G5 = 1073741824UL
        | H5 = 2147483648UL
        | A4 = 4294967296UL
        | B4 = 8589934592UL
        | C4 = 17179869184UL
        | D4 = 34359738368UL
        | E4 = 68719476736UL
        | F4 = 137438953472UL
        | G4 = 274877906944UL
        | H4 = 549755813888UL
        | A3 = 1099511627776UL
        | B3 = 2199023255552UL
        | C3 = 4398046511104UL
        | D3 = 8796093022208UL
        | E3 = 17592186044416UL
        | F3 = 35184372088832UL
        | G3 = 70368744177664UL
        | H3 = 140737488355328UL
        | A2 = 281474976710656UL
        | B2 = 562949953421312UL
        | C2 = 1125899906842624UL
        | D2 = 2251799813685248UL
        | E2 = 4503599627370496UL
        | F2 = 9007199254740992UL
        | G2 = 18014398509481984UL
        | H2 = 36028797018963968UL
        | A1 = 72057594037927936UL
        | B1 = 144115188075855872UL
        | C1 = 288230376151711744UL
        | D1 = 576460752303423488UL
        | E1 = 1152921504606846976UL
        | F1 = 2305843009213693952UL
        | G1 = 4611686018427387904UL
        | H1 = 9223372036854775808UL
        | Rank1 = 18374686479671623680UL
        | Rank2 = 71776119061217280UL
        | Rank3 = 280375465082880UL
        | Rank4 = 1095216660480UL
        | Rank5 = 4278190080UL
        | Rank6 = 16711680UL
        | Rank7 = 65280UL
        | Rank8 = 255UL
        | FileA = 72340172838076673UL
        | FileB = 144680345676153346UL
        | FileC = 289360691352306692UL
        | FileD = 578721382704613384UL
        | FileE = 1157442765409226768UL
        | FileF = 2314885530818453536UL
        | FileG = 4629771061636907072UL
        | FileH = 9259542123273814144UL
        | Empty = 0UL
        | Full = 18446744073709551615UL

    let FileBits = [|Bitboard.FileA;Bitboard.FileB;Bitboard.FileC;Bitboard.FileD;Bitboard.FileE;Bitboard.FileF;Bitboard.FileG;Bitboard.FileH|]