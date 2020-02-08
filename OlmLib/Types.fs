namespace Olm

[<AutoOpen>]
module Types =
    type GmResult = 
        | Draw = 0
        | WhiteWins = 1
        | BlackWins = -1
        | Open = 9

    type Nag =
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
    let Ng i = enum<Nag> (i)

    type aMove =
        {
            PreBrd : FsChess.Types.Brd
            Mv : FsChess.Types.Move
            PostBrd : FsChess.Types.Brd
        }
    type MvStrEntry =
        |MvEntry of int * bool * string * aMove
        |CommEntry of string
        |EndEntry of GmResult
        |NagEntry of Nag
        |RvEntry of MvStrEntry list

    type MvStats =
        {
            Mv : FsChess.Types.Move
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


