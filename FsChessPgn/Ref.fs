namespace PgnLib

open System

module Ref = 
    let fs = [ 'a'..'h' ]
    let private rs = [ 1..8 ] |> List.rev
    
    ///Dictionary of files
    let fDct = 
        fs
        |> List.mapi (fun i c -> c, i)
        |> dict
    
    ///Dictionary of ranks
    let rDct = 
        rs
        |> List.mapi (fun i c -> char (c.ToString()), i)
        |> dict
    
    ///Dictionary of squares
    let SqDct = 
        [ for r = 8 downto 1 do
              for f in fs do
                  yield f.ToString() + r.ToString() ]
        |> List.mapi (fun i s -> s, i)
        |> dict
    
    let f = 
        let fileName (sq) = fs.[sq % 8].ToString()
        [ for i in 0..63 -> fileName (i) ]
    
    let r = 
        let rankName (sq) = rs.[sq / 8].ToString()
        [ for i in 0..63 -> rankName (i) ]
    
    let sq = List.map2 (+) f r
    
    let fi = 
        [ for i in 0..63 -> i % 8 ]
    
    let ri = 
        [ for i in 0..63 -> 7 - i / 8 ]
    
    let GetOrdFR(file, rank) = ((7 - rank) <<< 3) ||| file
    
    let ord (l, (n : char)) = 
        let fis = [| 'a'; 'b'; 'c'; 'd'; 'e'; 'f'; 'g'; 'h' |]
        let f = Array.IndexOf(fis, l)
        let r = Convert.ToInt32(n.ToString()) - 1
        GetOrdFR(f, r)
    
    //Do all possible moves for each piece from each square
    let raysB = 
        let rec upl i l = 
            if f.[i] = "a" || r.[i] = "8" then List.rev l
            else upl (i - 9) (i - 9 :: l)
        
        let rec upr i l = 
            if f.[i] = "h" || r.[i] = "8" then List.rev l
            else upr (i - 7) (i - 7 :: l)
        
        let rec dwl i l = 
            if f.[i] = "a" || r.[i] = "1" then List.rev l
            else dwl (i + 7) (i + 7 :: l)
        
        let rec dwr i l = 
            if f.[i] = "h" || r.[i] = "1" then List.rev l
            else dwr (i + 9) (i + 9 :: l)
        
        let ray i = 
            let uplr = upl i []
            let uprr = upr i []
            let dwlr = dwl i []
            let dwrr = dwr i []
            uplr :: uprr :: dwlr :: [ dwrr ]
        
        [ for i in 0..63 -> ray i ]
    
    let raysR = 
        let rec up i l = 
            if r.[i] = "8" then List.rev l
            else up (i - 8) (i - 8 :: l)
        
        let rec lef i l = 
            if f.[i] = "a" then List.rev l
            else lef (i - 1) (i - 1 :: l)
        
        let rec rgt i l = 
            if f.[i] = "h" then List.rev l
            else rgt (i + 1) (i + 1 :: l)
        
        let rec dwn i l = 
            if r.[i] = "1" then List.rev l
            else dwn (i + 8) (i + 8 :: l)
        
        let ray i = 
            let upr = up i []
            let lefr = lef i []
            let rgtr = rgt i []
            let dwnr = dwn i []
            upr :: lefr :: rgtr :: [ dwnr ]
        
        [ for i in 0..63 -> ray i ]
    
    let raysQ = 
        [ for i in 0..63 -> raysB.[i] @ raysR.[i] ]
    
    let private u2l = 
        [ for i in 0..63 -> 
              if r.[i] = "8" || r.[i] = "7" || f.[i] = "a" then []
              else [ i - 17 ] ]
    
    let private l2u = 
        [ for i in 0..63 -> 
              if r.[i] = "8" || f.[i] = "a" || f.[i] = "b" then []
              else [ i - 10 ] ]
    
    let private u2r = 
        [ for i in 0..63 -> 
              if r.[i] = "8" || r.[i] = "7" || f.[i] = "h" then []
              else [ i - 15 ] ]
    
    let private r2u = 
        [ for i in 0..63 -> 
              if r.[i] = "8" || f.[i] = "g" || f.[i] = "h" then []
              else [ i - 6 ] ]
    
    let private d2l = 
        [ for i in 0..63 -> 
              if r.[i] = "1" || r.[i] = "2" || f.[i] = "a" then []
              else [ i + 15 ] ]
    
    let private l2d = 
        [ for i in 0..63 -> 
              if r.[i] = "1" || f.[i] = "a" || f.[i] = "b" then []
              else [ i + 6 ] ]
    
    let private d2r = 
        [ for i in 0..63 -> 
              if r.[i] = "1" || r.[i] = "2" || f.[i] = "h" then []
              else [ i + 17 ] ]
    
    let private r2d = 
        [ for i in 0..63 -> 
              if r.[i] = "1" || f.[i] = "g" || f.[i] = "h" then []
              else [ i + 10 ] ]
    
    let movsN = [ u2l; l2u; u2r; r2u; d2l; l2d; d2r; r2d ] |> List.reduce (fun la lb -> List.map2 (@) la lb)
    
    let private u = 
        [ for i in 0..63 -> 
              if r.[i] = "8" then []
              else [ i - 8 ] ]
    
    let private ul = 
        [ for i in 0..63 -> 
              if r.[i] = "8" || f.[i] = "a" then []
              else [ i - 9 ] ]
    
    let private ur = 
        [ for i in 0..63 -> 
              if r.[i] = "8" || f.[i] = "h" then []
              else [ i - 7 ] ]
    
    let private rt = 
        [ for i in 0..63 -> 
              if f.[i] = "h" then []
              else [ i + 1 ] ]
    
    let private lf = 
        [ for i in 0..63 -> 
              if f.[i] = "a" then []
              else [ i - 1 ] ]
    
    let private d = 
        [ for i in 0..63 -> 
              if r.[i] = "1" then []
              else [ i + 8 ] ]
    
    let private dr = 
        [ for i in 0..63 -> 
              if r.[i] = "1" || f.[i] = "h" then []
              else [ i + 9 ] ]
    
    let private dl = 
        [ for i in 0..63 -> 
              if r.[i] = "1" || f.[i] = "a" then []
              else [ i + 7 ] ]
    
    let movsK = [ u; ul; ur; rt; lf; d; dr; dl ] |> List.reduce (fun la lb -> List.map2 (@) la lb)
    
    //Pawn related moves
    let attsPW = 
        let ar i l = 
            if r.[i] = "8" || r.[i] = "1" || f.[i] = "h" then l
            else i - 7 :: l
        
        let al i l = 
            if r.[i] = "8" || r.[i] = "1" || f.[i] = "a" then l
            else i - 9 :: l
        
        let mvs i = 
            let movs = ar i []
            al i movs
        
        [ for i in 0..63 -> mvs i ]
    
    let attsPB = 
        let ar i l = 
            if r.[i] = "8" || r.[i] = "1" || f.[i] = "h" then l
            else i + 9 :: l
        
        let al i l = 
            if r.[i] = "8" || r.[i] = "1" || f.[i] = "a" then l
            else i + 7 :: l
        
        let mvs i = 
            let movs = ar i []
            al i movs
        
        [ for i in 0..63 -> mvs i ]
    
    let movPW = 
        r |> List.mapi (fun i r -> 
                 if r <> "1" && r <> "8" then i - 8
                 else -1)
    
    let movPB = 
        r |> List.mapi (fun i r -> 
                 if r <> "1" && r <> "8" then i + 8
                 else -1)
    
    let movPW2 = 
        r |> List.mapi (fun i r -> 
                 if r = "2" then i - 16
                 else -1)
    
    let movPB2 = 
        r |> List.mapi (fun i r -> 
                 if r = "7" then i + 16
                 else -1)
    
    let movsPW = 
        let getmvs i mv = 
            let mvs = mv :: attsPW.[i]
            if movPW2.[i] <> -1 then movPW2.[i] :: mvs
            else mvs
        movPW |> List.mapi getmvs
    
    let movsPB = 
        let getmvs i mv = 
            let mvs = mv :: attsPB.[i]
            if movPB2.[i] <> -1 then movPB2.[i] :: mvs
            else mvs
        movPB |> List.mapi getmvs
