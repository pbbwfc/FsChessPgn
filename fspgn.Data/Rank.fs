namespace fspgn.Data

open System

module Rank = 
    let Rankdesclookup = "87654321"
    let AllRanks = [| Rank.Rank8; Rank.Rank7; Rank.Rank6; Rank.Rank5; Rank.Rank4; Rank.Rank3; Rank.Rank2; Rank.Rank1 |]
    
    let Parse(c : char) = 
        let idx = Rankdesclookup.IndexOf(c.ToString().ToLower())
        if idx < 0 then failwith (c.ToString() + " is not a valid rank")
        else idx |> Rnk
    
    let RankToString(rank : Rank) = Rankdesclookup.Substring(int (rank), 1)
    let IsInBounds(rank : Rank) = int (rank) >= 0 && int (rank) <= 7
    let ToPosition (file : File) (rank : Rank) = (int (rank) * 8 + int (file)) |> Pos
    
    let ToBitboard(rank : Rank) = 
        match rank with
        | Rank.Rank1 -> Bitboard.Rank1
        | Rank.Rank2 -> Bitboard.Rank2
        | Rank.Rank3 -> Bitboard.Rank3
        | Rank.Rank4 -> Bitboard.Rank4
        | Rank.Rank5 -> Bitboard.Rank5
        | Rank.Rank6 -> Bitboard.Rank6
        | Rank.Rank7 -> Bitboard.Rank7
        | Rank.Rank8 -> Bitboard.Rank8
        | _ -> Bitboard.Empty
