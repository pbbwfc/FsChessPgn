namespace FsChessPgn

module AssemblyInfo=

    open System.Runtime.CompilerServices

    [<assembly: InternalsVisibleTo("FsChessPgn.Test")>]
    do()

[<AutoOpen>]
module Types = 
    type Move = int
    let MoveEmpty = 0
    
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
    
    type GameResult = 
        | Draw = 0
        | WhiteWins = 1
        | BlackWins = -1
        | Open = 9
    
    type File = int
    let FileA, FileB, FileC, FileD, FileE, FileF, FileG, FileH :File * File * File * File * File * File * File * File = 0,1,2,3,4,5,6,7
    let FILES = [ FileA; FileB; FileC; FileD; FileE; FileF; FileG; FileH ]
    let FILE_NAMES = ["a"; "b"; "c"; "d"; "e"; "f"; "g"; "h"]
    let FILE_EMPTY :File = 8

    type Rank = int
    let Rank8, Rank7, Rank6, Rank5, Rank4, Rank3, Rank2, Rank1 :Rank * Rank * Rank * Rank * Rank * Rank * Rank * Rank = 0,1,2,3,4,5,6,7
    let RANKS = [Rank8; Rank7; Rank6; Rank5; Rank4; Rank3; Rank2; Rank1]
    let RANK_NAMES = ["8"; "7"; "6"; "5"; "4"; "3"; "2"; "1"]
    let RANK_EMPTY :Rank = 8

    type Square = int
    let A1, B1, C1, D1, E1, F1, G1, H1 :Square * Square * Square * Square * Square * Square * Square * Square =  56,57,58,59,60,61,62,63
    let A2, B2, C2, D2, E2, F2, G2, H2 = A1-8, B1-8, C1-8, D1-8, E1-8, F1-8, G1-8, H1-8 
    let A3, B3, C3, D3, E3, F3, G3, H3 = A2-8, B2-8, C2-8, D2-8, E2-8, F2-8, G2-8, H2-8 
    let A4, B4, C4, D4, E4, F4, G4, H4 = A3-8, B3-8, C3-8, D3-8, E3-8, F3-8, G3-8, H3-8 
    let A5, B5, C5, D5, E5, F5, G5, H5 = A4-8, B4-8, C4-8, D4-8, E4-8, F4-8, G4-8, H4-8 
    let A6, B6, C6, D6, E6, F6, G6, H6 = A5-8, B5-8, C5-8, D5-8, E5-8, F5-8, G5-8, H5-8 
    let A7, B7, C7, D7, E7, F7, G7, H7 = A6-8, B6-8, C6-8, D6-8, E6-8, F6-8, G6-8, H6-8 
    let A8, B8, C8, D8, E8, F8, G8, H8 = A7-8, B7-8, C7-8, D7-8, E7-8, F7-8, G7-8, H7-8
    let OUTOFBOUNDS:Square = 64
    let SQUARES = [
        A8; B8; C8; D8; E8; F8; G8; H8
        A7; B7; C7; D7; E7; F7; G7; H7;
        A6; B6; C6; D6; E6; F6; G6; H6;
        A5; B5; C5; D5; E5; F5; G5; H5;
        A4; B4; C4; D4; E4; F4; G4; H4;
        A3; B3; C3; D3; E3; F3; G3; H3;
        A2; B2; C2; D2; E2; F2; G2; H2;
        A1; B1; C1; D1; E1; F1; G1; H1;
        ] 
    let SQUARE_NAMES = [for r in RANK_NAMES do for f in FILE_NAMES -> f+r]
    let Sq(f:File,r:Rank) :Square = (int (r) * 8 + int (f))
    
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
        { Pieceat : Piece list
          Whosturn : Player
          CastleWS : bool
          CastleWL : bool
          CastleBS : bool
          CastleBL : bool
          Enpassant : Square
          Fiftymove : int
          Fullmove : int }
    
    [<System.Flags>]
    type CstlFlgs = 
        | EMPTY = 0
        | WhiteShort = 1
        | WhiteLong = 2
        | BlackShort = 4
        | BlackLong = 8
        | All = 15
    
    [<System.Flags>]
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

    type MoveType =
        | Simple
        | Capture
        | CastleKingSide
        | CastleQueenSide

    type MoveAnnotation =
        |Brilliant
        |Good
        |Interesting
        |Dubious
        |Mistake
        |Blunder

    type pMove = 
        {Mtype:MoveType 
         TargetSquare:Square 
         Piece: PieceType option
         OriginSquare:Square
         OriginFile:File option
         OriginRank:Rank option
         PromotedPiece: PieceType option
         IsCheck:bool
         IsDoubleCheck:bool
         IsCheckMate:bool
         Annotation:MoveAnnotation option}

    type Brd = 
        { PieceAt : Piece list
          WtKingPos : Square
          BkKingPos : Square
          PieceTypes : Bitboard list
          WtPrBds : Bitboard
          BkPrBds : Bitboard
          PieceLocationsAll : Bitboard
          Checkers : Bitboard
          WhosTurn : Player
          CastleRights : CstlFlgs
          EnPassant : Square
          Fiftymove : int
          Fullmove : int
          MovesSinceNull : int
          }

    let BrdEMP = 
        { PieceAt = Array.create 64 Piece.EMPTY|>List.ofArray
          WtKingPos = OUTOFBOUNDS
          BkKingPos = OUTOFBOUNDS
          PieceTypes = Array.create 7 Bitboard.Empty|>List.ofArray
          WtPrBds = Bitboard.Empty
          BkPrBds = Bitboard.Empty
          PieceLocationsAll = Bitboard.Empty
          Checkers = Bitboard.Empty
          WhosTurn = Player.White
          CastleRights = CstlFlgs.EMPTY
          EnPassant = OUTOFBOUNDS
          Fiftymove = 0
          Fullmove = 0
          MovesSinceNull = 100
          }

    type aMove =
        {
            PreBrd : Brd
            Mv : Move
            PostBrd : Brd
        }

    type MoveTextEntry =
        |HalfMoveEntry of int option * bool * pMove * aMove option
        |CommentEntry of string
        |GameEndEntry of GameResult
        |NAGEntry of int
        |RAVEntry of MoveTextEntry list
 
    type Game =
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
            AdditionalInfo : Map<string,string>
            MoveText : MoveTextEntry list
        }

    let GameEMP =
        {
            Event = "?"
            Site = "?"
            Year = None
            Month = None
            Day = None
            Round = "?"
            WhitePlayer = "?"
            BlackPlayer = "?"
            Result = GameResult.Open
            BoardSetup = None
            AdditionalInfo = Map.empty
            MoveText = []
        }

