namespace Olm

[<AutoOpen>]
module Types =
    type GameResult = 
        | Draw = 0
        | WhiteWins = 1
        | BlackWins = -1
        | Open = 9

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


