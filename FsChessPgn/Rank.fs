namespace FsChessPgn.Data

module Rank = 
    
    let Parse(c : char) :Rank = 
        let Rankdesclookup = RANK_NAMES|>Array.reduce(+)
        let idx = Rankdesclookup.IndexOf(c.ToString().ToLower())
        if idx < 0 then failwith (c.ToString() + " is not a valid rank")
        else idx 
    
    let RankToString(rank : Rank) = RANK_NAMES.[int(rank)]
    let IsInBounds(rank : Rank) = int (rank) >= 0 && int (rank) <= 7
    
    let ToBitboard(rank : Rank) = 
        if rank=Rank1 then Bitboard.Rank1
        elif rank=Rank2 then Bitboard.Rank2
        elif rank=Rank3 then Bitboard.Rank3
        elif rank=Rank4 then Bitboard.Rank4
        elif rank=Rank5 then Bitboard.Rank5
        elif rank=Rank6 then Bitboard.Rank6
        elif rank=Rank7 then Bitboard.Rank7
        elif rank=Rank8 then Bitboard.Rank8
        else Bitboard.Empty
