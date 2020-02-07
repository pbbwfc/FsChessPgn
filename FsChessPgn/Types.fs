namespace FsChess

module AssemblyInfo=

    open System.Runtime.CompilerServices

    [<assembly: InternalsVisibleTo("FsChessPgn.Test")>]
    do()

[<AutoOpen>]
module Types = 
    type Move = uint32
    let MoveEmpty:Move = 0u
    
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
        | White = 0
        | Black = 1
    
    type GameResult = 
        | Draw = 0
        | WhiteWins = 1
        | BlackWins = -1
        | Open = 9
    
    type File = int16
    let FileA, FileB, FileC, FileD, FileE, FileF, FileG, FileH :File * File * File * File * File * File * File * File = 0s,1s,2s,3s,4s,5s,6s,7s
    let FILES = [FileA; FileB; FileC; FileD; FileE; FileF; FileG; FileH]
    let FILE_NAMES = ["a"; "b"; "c"; "d"; "e"; "f"; "g"; "h"]
    let FILE_EMPTY :File = 8s

    type Rank = int16
    let Rank8, Rank7, Rank6, Rank5, Rank4, Rank3, Rank2, Rank1 :Rank * Rank * Rank * Rank * Rank * Rank * Rank * Rank = 0s,1s,2s,3s,4s,5s,6s,7s
    let RANKS = [Rank8; Rank7; Rank6; Rank5; Rank4; Rank3; Rank2; Rank1]
    let RANK_NAMES = ["8"; "7"; "6"; "5"; "4"; "3"; "2"; "1"]
    let RANK_EMPTY :Rank = 8s

    type Square = int16
    let A1, B1, C1, D1, E1, F1, G1, H1 :Square * Square * Square * Square * Square * Square * Square * Square =  56s,57s,58s,59s,60s,61s,62s,63s
    let A2, B2, C2, D2, E2, F2, G2, H2 = A1-8s, B1-8s, C1-8s, D1-8s, E1-8s, F1-8s, G1-8s, H1-8s 
    let A3, B3, C3, D3, E3, F3, G3, H3 = A2-8s, B2-8s, C2-8s, D2-8s, E2-8s, F2-8s, G2-8s, H2-8s 
    let A4, B4, C4, D4, E4, F4, G4, H4 = A3-8s, B3-8s, C3-8s, D3-8s, E3-8s, F3-8s, G3-8s, H3-8s 
    let A5, B5, C5, D5, E5, F5, G5, H5 = A4-8s, B4-8s, C4-8s, D4-8s, E4-8s, F4-8s, G4-8s, H4-8s 
    let A6, B6, C6, D6, E6, F6, G6, H6 = A5-8s, B5-8s, C5-8s, D5-8s, E5-8s, F5-8s, G5-8s, H5-8s 
    let A7, B7, C7, D7, E7, F7, G7, H7 = A6-8s, B6-8s, C6-8s, D6-8s, E6-8s, F6-8s, G6-8s, H6-8s 
    let A8, B8, C8, D8, E8, F8, G8, H8 = A7-8s, B7-8s, C7-8s, D7-8s, E7-8s, F7-8s, G7-8s, H7-8s
    let OUTOFBOUNDS:Square = 64s
    let SQUARES = [
        A8; B8; C8; D8; E8; F8; G8; H8;
        A7; B7; C7; D7; E7; F7; G7; H7;
        A6; B6; C6; D6; E6; F6; G6; H6;
        A5; B5; C5; D5; E5; F5; G5; H5;
        A4; B4; C4; D4; E4; F4; G4; H4;
        A3; B3; C3; D3; E3; F3; G3; H3;
        A2; B2; C2; D2; E2; F2; G2; H2;
        A1; B1; C1; D1; E1; F1; G1; H1
        ] 
    let SQUARE_NAMES = [for r in RANK_NAMES do for f in FILE_NAMES -> f+r]
    let Sq(f:File,r:Rank) :Square = r * 8s + f
    
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

    type NAG =
        |Null = 0
        |Good = 1
        |Poor = 2
        |VeryGood = 3
        |VeryPoor = 4
        |Speculative = 5
        |Questionable = 6
        |Even = 10
        |Wslight = 14
        |Bslight = 15
        |Wmoderate = 16
        |Bmoderate =17
        |Wdecisive = 18
        |Bdecisive = 19

    type pMove = 
        {
         Mtype:MoveType 
         TargetSquare:Square 
         Piece: PieceType option
         OriginFile:File option
         OriginRank:Rank option
         PromotedPiece: PieceType option
         IsCheck:bool
         IsDoubleCheck:bool
         IsCheckMate:bool
         }

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
          }
         member this.Item with get(sq:Square) = this.PieceAt.[int(sq)]
              

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
          }

    type aMove =
        {
            PreBrd : Brd
            Mno : int
            Isw : bool
            Mv : Move
            PostBrd : Brd
        }

    type MoveTextEntry =
        |HalfMoveEntry of int option * bool * pMove * aMove option
        |CommentEntry of string
        |GameEndEntry of GameResult
        |NAGEntry of NAG
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
            WhiteElo : string
            BlackElo : string
            BoardSetup : Brd option
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
            WhiteElo = "-"
            BlackElo = "-"
            BoardSetup = None
            AdditionalInfo = Map.empty
            MoveText = []
        }

    type MvStats =
        {
            Mvstr : string
            Count : int
            Pc : float
            WhiteWins : int 
            Draws : int 
            BlackWins :int
            Score : float
            DrawPc : float
        }
    
    type BrdStats = 
        {
            Mvstats : MvStats list
            TotCount : int
            Pc : float
            TotWhiteWins : int 
            TotDraws : int 
            TotBlackWins :int
            TotScore : float
            TotDrawPc : float
        }

    let BrdStatsEMP = 
        {
            Mvstats = []
            TotCount = 0
            Pc = 0.0
            TotWhiteWins = 0 
            TotDraws = 0
            TotBlackWins = 0
            TotScore = 0.0
            TotDrawPc = 0.0
        }
