namespace fspgn.Data

open System

module Square = 
    
    let Parse(s : string) = 
        if s.Length <> 2 then failwith (s + " is not a valid position")
        else 
            let file = File.Parse(s.[0])
            let rank = Rank.Parse(s.[1])
            file |> File.ToPosition(rank)
    
    let IsInBounds(pos : Square) = int (pos) >= 0 && int (pos) <= 63
    let ToRank(pos : Square) :Rank = (int (pos) / 8)
    let ToFile(pos : Square) :File = (int (pos) % 8)
    
    let Name(pos : Square) = 
        (pos
         |> ToFile
         |> File.FileToString)
        + (pos
           |> ToRank
           |> Rank.RankToString)
    
    let DistanceTo (pto : Square) (pfrom : Square) = 
        let rankfrom = int (pfrom |> ToRank)
        let filefrom = int (pfrom |> ToFile)
        let rankto = int (pto |> ToRank)
        let fileto = int (pto |> ToFile)
        let rDiff = abs (rankfrom - rankto)
        let fDiff = abs (filefrom - fileto)
        if rDiff > fDiff then rDiff
        else fDiff
    
    let DistanceToNoDiag (pto : Square) (pfrom : Square) = 
        let rankfrom = int (pfrom |> ToRank)
        let filefrom = int (pfrom |> ToFile)
        let rankto = int (pto |> ToRank)
        let fileto = int (pto |> ToFile)
        let rDiff = abs (rankfrom - rankto)
        let fDiff = abs (filefrom - fileto)
        rDiff + fDiff
    
    let DirectionTo (pto : Square) (pfrom : Square) = 
        let rankfrom = int (pfrom |> ToRank)
        let filefrom = int (pfrom |> ToFile)
        let rankto = int (pto |> ToRank)
        let fileto = int (pto |> ToFile)
        if fileto = filefrom then 
            if rankfrom < rankto then Direction.DirS
            else Direction.DirN
        elif rankfrom = rankto then 
            if filefrom > fileto then Direction.DirW
            else Direction.DirE
        else 
            let rankchange = rankto - rankfrom
            let filechange = fileto - filefrom
            
            let rankchangeabs = 
                if rankchange > 0 then rankchange
                else -rankchange
            
            let filechangeabs = 
                if filechange > 0 then filechange
                else -filechange
            
            if (rankchangeabs = 1 && filechangeabs = 2) || (rankchangeabs = 2 && filechangeabs = 1) then 
                ((rankchange * 8) + filechange) |> Dirn
            elif rankchangeabs <> filechangeabs then 0 |> Dirn
            elif rankchange < 0 then 
                if filechange > 0 then Direction.DirNE
                else Direction.DirNW
            else if filechange > 0 then Direction.DirSE
            else Direction.DirSW
    
    let PositionInDirectionUnsafe (dir : Direction) (pos : Square) :Square= ((int) pos + (int) dir)
    
    let PositionInDirection (dir : Direction) (pos : Square) = 
        if not (pos |> IsInBounds) then OUTOFBOUNDS
        else 
            let f = pos |> ToFile
            let r = pos |> ToRank
            
            let nr, nf = 
                match dir with
                | Direction.DirN -> r -! 1, f
                | Direction.DirE -> r, f ++ 1
                | Direction.DirS -> r +! 1, f
                | Direction.DirW -> r, f -- 1
                | Direction.DirNE -> r -! 1, f ++ 1
                | Direction.DirSE -> r +! 1, f ++ 1
                | Direction.DirSW -> r +! 1, f -- 1
                | Direction.DirNW -> r -! 1, f -- 1
                | Direction.DirNNE -> r -! 2, f ++ 1
                | Direction.DirEEN -> r -! 1, f ++ 2
                | Direction.DirEES -> r +! 1, f ++ 2
                | Direction.DirSSE -> r +! 2, f ++ 1
                | Direction.DirSSW -> r +! 2, f -- 1
                | Direction.DirWWS -> r +! 1, f -- 2
                | Direction.DirWWN -> r -! 1, f -- 2
                | Direction.DirNNW -> r -! 2, f -- 1
                | _ -> RANK_EMPTY, FILE_EMPTY
            if nr = RANK_EMPTY && nf = FILE_EMPTY then OUTOFBOUNDS
            elif (nr |> Rank.IsInBounds) && (nf |> File.IsInBounds) then nr |> Rank.ToPosition(nf)
            else OUTOFBOUNDS
    
    let Reverse(pos : Square) = 
        let r = pos |> ToRank
        let f = pos |> ToFile
        
        let newrank = 
            if r=Rank1 then Rank8
            elif r=Rank2 then Rank7
            elif r=Rank3 then Rank6
            elif r=Rank4 then Rank5
            elif r=Rank5 then Rank4
            elif r=Rank6 then Rank3
            elif r=Rank7 then Rank2
            elif r=Rank8 then Rank1
            else RANK_EMPTY
        f |> File.ToPosition(newrank)
    
    let ToBitboard(pos : Square) = 
        if pos |> IsInBounds then (1UL <<< int (pos)) |> BitB
        else Bitboard.Empty
    
    let ToBitboardL(posl : Square list) = 
        posl
        |> List.map (ToBitboard)
        |> List.reduce (|||)
    
    let Between (pto : Square) (pfrom : Square) = 
        let dir = pfrom |> DirectionTo(pto)
        
        let rec getb f rv = 
            if f = pto then rv
            else 
                let nf = f |> PositionInDirectionUnsafe(dir)
                let nrv = rv ||| (nf |> ToBitboard)
                getb nf nrv
        
        let rv = 
            if int (dir) = 0 then Bitboard.Empty
            else getb pfrom Bitboard.Empty
        
        rv &&& ~~~(pto |> ToBitboard)
