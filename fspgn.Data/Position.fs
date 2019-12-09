namespace fspgn.Data

open System

module Position = 
    let AllPositions = 
        [| Position.A8; Position.B8; Position.C8; Position.D8; Position.E8; Position.F8; Position.G8; Position.H8; 
           Position.A7; Position.B7; Position.C7; Position.D7; Position.E7; Position.F7; Position.G7; Position.H7; 
           Position.A6; Position.B6; Position.C6; Position.D6; Position.E6; Position.F6; Position.G6; Position.H6; 
           Position.A5; Position.B5; Position.C5; Position.D5; Position.E5; Position.F5; Position.G5; Position.H5; 
           Position.A4; Position.B4; Position.C4; Position.D4; Position.E4; Position.F4; Position.G4; Position.H4; 
           Position.A3; Position.B3; Position.C3; Position.D3; Position.E3; Position.F3; Position.G3; Position.H3; 
           Position.A2; Position.B2; Position.C2; Position.D2; Position.E2; Position.F2; Position.G2; Position.H2; 
           Position.A1; Position.B1; Position.C1; Position.D1; Position.E1; Position.F1; Position.G1; Position.H1 |]
    
    let Parse(s : string) = 
        if s.Length <> 2 then failwith (s + " is not a valid position")
        else 
            let file = File.Parse(s.[0])
            let rank = Rank.Parse(s.[1])
            file |> File.ToPosition(rank)
    
    let IsInBounds(pos : Position) = int (pos) >= 0 && int (pos) <= 63
    let ToRank(pos : Position) = (int (pos) / 8) |> Rnk
    let ToFile(pos : Position) = (int (pos) % 8) |> Fl
    
    let Name(pos : Position) = 
        (pos
         |> ToFile
         |> File.FileToString)
        + (pos
           |> ToRank
           |> Rank.RankToString)
    
    let DistanceTo (pto : Position) (pfrom : Position) = 
        let rankfrom = int (pfrom |> ToRank)
        let filefrom = int (pfrom |> ToFile)
        let rankto = int (pto |> ToRank)
        let fileto = int (pto |> ToFile)
        let rDiff = abs (rankfrom - rankto)
        let fDiff = abs (filefrom - fileto)
        if rDiff > fDiff then rDiff
        else fDiff
    
    let DistanceToNoDiag (pto : Position) (pfrom : Position) = 
        let rankfrom = int (pfrom |> ToRank)
        let filefrom = int (pfrom |> ToFile)
        let rankto = int (pto |> ToRank)
        let fileto = int (pto |> ToFile)
        let rDiff = abs (rankfrom - rankto)
        let fDiff = abs (filefrom - fileto)
        rDiff + fDiff
    
    let DirectionTo (pto : Position) (pfrom : Position) = 
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
    
    let PositionInDirectionUnsafe (dir : Direction) (pos : Position) = ((int) pos + (int) dir) |> Pos
    
    let PositionInDirection (dir : Direction) (pos : Position) = 
        if not (pos |> IsInBounds) then Position.OUTOFBOUNDS
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
                | _ -> Rank.EMPTY, File.EMPTY
            if nr = Rank.EMPTY && nf = File.EMPTY then Position.OUTOFBOUNDS
            elif (nr |> Rank.IsInBounds) && (nf |> File.IsInBounds) then nr |> Rank.ToPosition(nf)
            else Position.OUTOFBOUNDS
    
    let Reverse(pos : Position) = 
        let r = pos |> ToRank
        let f = pos |> ToFile
        
        let newrank = 
            match r with
            | Rank.Rank1 -> Rank.Rank8
            | Rank.Rank2 -> Rank.Rank7
            | Rank.Rank3 -> Rank.Rank6
            | Rank.Rank4 -> Rank.Rank5
            | Rank.Rank5 -> Rank.Rank4
            | Rank.Rank6 -> Rank.Rank3
            | Rank.Rank7 -> Rank.Rank2
            | Rank.Rank8 -> Rank.Rank1
            | _ -> Rank.EMPTY
        f |> File.ToPosition(newrank)
    
    let ToBitboard(pos : Position) = 
        if pos |> IsInBounds then (1UL <<< int (pos)) |> BitB
        else Bitboard.Empty
    
    let ToBitboardL(posl : Position list) = 
        posl
        |> List.map (ToBitboard)
        |> List.reduce (|||)
    
    let Between (pto : Position) (pfrom : Position) = 
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
