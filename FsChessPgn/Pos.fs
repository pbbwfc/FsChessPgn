namespace PgnLib

open System
open System.Text
open System.Text.RegularExpressions

type Pos(isqs : char [], iisw : bool) = 
    let sqs = isqs
    let mutable isw = iisw
    //utility functions
    let samef a b = a % 8 = b % 8
    let samer a b = a / 8 = b / 8
    let samefr a b = samef a b || samer a b
    let samedg a b = abs (a % 8 - b % 8) = abs (a / 8 - b / 8)
    let samedgfr a b = samedg a b || samefr a b
    
    let isnmv a b = 
        let rd = abs (a / 8 - b / 8)
        let fd = abs (a % 8 - b % 8)
        rd = 2 && fd = 1 || fd = 2 && rd = 1
    
    member val Sqs = sqs with get, set
    member val IsW = isw with get, set
    member val CustomizationFunctions = [] with get, set
    
    //private member to parse string
    static member private Parse(s : string) = 
        let b = s.Split(' ')
        let isw = b.[1] = "w"
        let sqs = Array.create 64 ' '
        
        let rec getp i ps = 
            if ps = "" then sqs, isw
            else 
                match ps.[0] with
                | '/' -> getp i ps.[1..]
                | c -> 
                    let ok, p = Int32.TryParse(c.ToString())
                    if ok then getp (i + p) ps.[1..]
                    else 
                        sqs.[i] <- c
                        getp (i + 1) ps.[1..]
        getp 0 b.[0]
    
    /// loads Pos given a FEN like string
    static member FromString(s : string) = 
        let isqs, iisw = Pos.Parse(s)
        new Pos(isqs, iisw)
    
    /// Gets initial Pos
    static member Start() = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w" |> Pos.FromString
    
    /// loads Pos given a Move list
    static member FromMoves(imvl : Move list) = 
        let rec domvs mvl (pos : Pos) = 
            if List.isEmpty mvl then pos
            else 
                pos.DoMv mvl.Head
                domvs mvl.Tail pos
        domvs imvl (Pos.Start())
    
    member x.Set(s : string) = 
        let sqs, isw = Pos.Parse(s)
        sqs |> Array.iteri (fun i c -> x.Sqs.[i] <- c)
        x.IsW <- isw
    
    override x.ToString() = 
        let sb = new StringBuilder()
        
        let rec getstr i e chl = 
            if List.isEmpty chl then 
                if e > 0 then sb.Append(e.ToString()) |> ignore
                sb.ToString() + (if x.IsW then " w"
                                 else " b")
            elif i <> 8 && chl.Head = ' ' then getstr (i + 1) (e + 1) chl.Tail
            else 
                if e > 0 then sb.Append(e.ToString()) |> ignore
                if i = 8 then sb.Append("/") |> ignore
                else sb.Append(chl.Head) |> ignore
                if i = 8 then getstr 0 0 chl
                else getstr (i + 1) 0 chl.Tail
        getstr 0 0 (x.Sqs |> List.ofArray)
    
    member x.Copy() = new Pos((x.Sqs |> Array.copy), x.IsW)
    
    /// Make a move
    member x.Mv(s : string) = 
        let mv : Move = x.GetMv s
        mv |> x.DoMv
    
    /// Make a move
    member x.DoMv(mv : Move) = 
        x.IsW <- not x.IsW
        let c = x.Sqs.[mv.Mfrom]
        x.Sqs.[mv.Mfrom] <- ' '
        x.Sqs.[mv.Mto] <- c
        match mv.Mtyp with
        | Prom(c) -> 
            x.Sqs.[mv.Mto] <- if x.IsW then c |> Char.ToLower
                              else c
        | CasK -> 
            x.Sqs.[mv.Mto - 1] <- x.Sqs.[mv.Mto + 1]
            x.Sqs.[mv.Mto + 1] <- ' '
        | CasQ -> 
            x.Sqs.[mv.Mto + 1] <- x.Sqs.[mv.Mto - 2]
            x.Sqs.[mv.Mto - 2] <- ' '
        | Ep -> 
            if x.IsW then x.Sqs.[mv.Mto - 8] <- ' '
            else x.Sqs.[mv.Mto + 8] <- ' '
        | _ -> ()
    
    /// Gets Move from string
    member x.GetMv mv = 
        //Active pattern to parse move string
        let (|SimpleMove|Castle|PawnCapture|AbiguousFile|AbiguousRank|Promotion|PromCapture|) s = 
            if Regex.IsMatch(s, "^[BNRQK][a-h][1-8]$") then SimpleMove(s.[0], s.[1..])
            elif Regex.IsMatch(s, "^[a-h][1-8]$") then SimpleMove('P', s)
            elif s = "O-O" then Castle('K')
            elif s = "O-O-O" then Castle('Q')
            elif Regex.IsMatch(s, "^[a-h][a-h][1-8]$") then PawnCapture(s.[0], s.[1..])
            elif Regex.IsMatch(s, "^[BNRQK][a-h][a-h][1-8]$") then AbiguousFile(s.[0], s.[1], s.[2..])
            elif Regex.IsMatch(s, "^[BNRQK][1-8][a-h][1-8]$") then AbiguousRank(s.[0], s.[1], s.[2..])
            elif Regex.IsMatch(s, "^[a-h][1-8][BNRQ]$") then Promotion(s.[0..1], s.[2])
            elif Regex.IsMatch(s, "^[a-h][a-h][1-8][BNRQ]$") then PromCapture(s.[0], s.[1..2], s.[3])
            else failwith ("invalid move: " + s)
        
        //general failure message
        let fl() = failwith ("not done yet, mv: " + mv + " pos: " + x.ToString())
        
        let strip chars = 
            String.collect (fun c -> 
                if Seq.exists ((=) c) chars then ""
                else string c)
        
        let m = mv |> strip "+x#=!?"
        let m = m.Replace("e.p.", "")
        match m with
        //simple pawn move e.g. d4
        | SimpleMove('P', sq) -> 
            let mto = Ref.SqDct.[sq]
            if x.IsW then 
                if x.Sqs.[mto + 8] = 'P' then 
                    { Mfrom = mto + 8
                      Mto = mto
                      Mtyp = Standard
                      Mpgn = mv 
                      Meval = Normal
                      Scr10 = 0
                      Scr25 = 0 
                      Bresp = ""
                      ECO = ""
                      FicsPc = 0.0}
                else 
                    { Mfrom = mto + 16
                      Mto = mto
                      Mtyp = Standard
                      Mpgn = mv 
                      Meval = Normal
                      Scr10 = 0
                      Scr25 = 0 
                      Bresp = ""
                      ECO = ""
                      FicsPc = 0.0}
            else if x.Sqs.[mto - 8] = 'p' then 
                {   Mfrom = mto - 8
                    Mto = mto
                    Mtyp = Standard
                    Mpgn = mv 
                    Meval = Normal
                    Scr10 = 0
                    Scr25 = 0 
                    Bresp = ""
                    ECO = ""
                    FicsPc = 0.0}
            else 
                {   Mfrom = mto - 16
                    Mto = mto
                    Mtyp = Standard
                    Mpgn = mv 
                    Meval = Normal
                    Scr10 = 0
                    Scr25 = 0 
                    Bresp = ""
                    ECO = ""
                    FicsPc = 0.0}
        //simple piece move e.g. Nf3
        | SimpleMove(p, sq) -> 
            let mto = Ref.SqDct.[sq]
            
            let pc = 
                if x.IsW then p
                else p |> Char.ToLower
            
            let mfs = 
                x.Sqs
                |> Array.mapi (fun i c -> i, c)
                |> Array.filter (fun (_, c) -> c = pc)
                |> Array.map fst
            
            if mfs.Length = 1 then 
                {   Mfrom = mfs.[0]
                    Mto = mto
                    Mtyp = Standard
                    Mpgn = mv 
                    Meval = Normal
                    Scr10 = 0
                    Scr25 = 0 
                    Bresp = ""
                    ECO = ""
                    FicsPc = 0.0}
            else 
                match pc with
                | 'N' | 'n' -> 
                    let ms = mfs |> Array.filter (isnmv mto)
                    if ms.Length = 1 then 
                        {   Mfrom = ms.[0]
                            Mto = mto
                            Mtyp = Standard
                            Mpgn = mv 
                            Meval = Normal
                            Scr10 = 0
                            Scr25 = 0 
                            Bresp = ""
                            ECO = ""
                            FicsPc = 0.0}
                    else fl()
                | 'B' | 'b' -> 
                    let fmfs = mfs |> Array.filter (samedg mto)
                    if fmfs.Length = 1 then 
                        {   Mfrom = fmfs.[0]
                            Mto = mto
                            Mtyp = Standard
                            Mpgn = mv 
                            Meval = Normal
                            Scr10 = 0
                            Scr25 = 0 
                            Bresp = ""
                            ECO = ""
                            FicsPc = 0.0}
                    else fl()
                | 'Q' | 'q' -> 
                    let fmfs = mfs |> Array.filter (samedgfr mto)
                    if fmfs.Length = 1 then 
                        { Mfrom = fmfs.[0]
                          Mto = mto
                          Mtyp = Standard
                          Mpgn = mv 
                          Meval = Normal
                          Scr10 = 0
                          Scr25 = 0 
                          Bresp = ""
                          ECO = ""
                          FicsPc = 0.0}
                    else 
                        let rec getval fl = 
                            if List.isEmpty fl then 
                                failwith ("can't find valid move, mv: " + mv + " pos: " + x.ToString())
                            else 
                                let f = fl.Head
                                if samer mto f then 
                                    let betw = 
                                        if mto < f then x.Sqs.[mto + 1..f - 1]
                                        else x.Sqs.[f + 1..mto - 1]
                                    if (betw |> Array.filter (fun c -> c <> ' ')).Length = 0 then f
                                    else getval fl.Tail
                                elif samef mto f then 
                                    let betw = 
                                        if mto < f then [ mto + 8..8..f - 8 ] |> List.map (fun i -> x.Sqs.[i])
                                        else [ f + 8..8..mto - 8 ] |> List.map (fun i -> x.Sqs.[i])
                                    if (betw |> List.filter (fun c -> c <> ' ')).Length = 0 then f
                                    else getval fl.Tail
                                //onsame diagonal
                                else 
                                    let betw = 
                                        if mto < f && (f - mto) % 7 = 0 then 
                                            [ mto + 7..7..f - 7 ] |> List.map (fun i -> x.Sqs.[i])
                                        elif mto < f then [ mto + 9..9..f - 9 ] |> List.map (fun i -> x.Sqs.[i])
                                        elif (mto - f) % 7 = 0 then 
                                            [ f + 7..7..mto - 7 ] |> List.map (fun i -> x.Sqs.[i])
                                        else [ f + 9..9..mto - 9 ] |> List.map (fun i -> x.Sqs.[i])
                                    if (betw |> List.filter (fun c -> c <> ' ')).Length = 0 then f
                                    else getval fl.Tail
                        
                        let mfrom = 
                            fmfs
                            |> List.ofArray
                            |> getval
                        
                        { Mfrom = mfrom
                          Mto = mto
                          Mtyp = Standard
                          Mpgn = mv 
                          Meval = Normal
                          Scr10 = 0
                          Scr25 = 0 
                          Bresp = ""
                          ECO = ""
                          FicsPc = 0.0}
                | 'R' | 'r' -> 
                    let fmfs = mfs |> Array.filter (samefr mto)
                    if fmfs.Length = 1 then 
                        { Mfrom = fmfs.[0]
                          Mto = mto
                          Mtyp = Standard
                          Mpgn = mv 
                          Meval = Normal
                          Scr10 = 0
                          Scr25 = 0 
                          Bresp = ""
                          ECO = ""
                          FicsPc = 0.0}
                    else 
                        let rec getvals ol fl = 
                            if List.isEmpty fl then ol
                            else 
                                let f = fl.Head
                                if samer mto f then 
                                    let betw = 
                                        if mto < f then x.Sqs.[mto + 1..f - 1]
                                        else x.Sqs.[f + 1..mto - 1]
                                    if (betw |> Array.filter (fun c -> c <> ' ')).Length = 0 then 
                                        getvals (f :: ol) fl.Tail
                                    else getvals ol fl.Tail
                                else 
                                    let betw = 
                                        if mto < f then [ mto + 8..8..f - 8 ] |> List.map (fun i -> x.Sqs.[i])
                                        else [ f + 8..8..mto - 8 ] |> List.map (fun i -> x.Sqs.[i])
                                    if (betw |> List.filter (fun c -> c <> ' ')).Length = 0 then 
                                        getvals (f :: ol) fl.Tail
                                    else getvals ol fl.Tail
                        
                        let mfroms = 
                            fmfs
                            |> List.ofArray
                            |> getvals []
                        
                        if mfroms.Length <> 1 then fl()
                        else 
                            { Mfrom = mfroms.[0]
                              Mto = mto
                              Mtyp = Standard
                              Mpgn = mv 
                              Meval = Normal
                              Scr10 = 0
                              Scr25 = 0 
                              Bresp = ""
                              ECO = ""
                              FicsPc = 0.0}
                | _ -> fl()
        | Castle(c) -> 
            if c = 'K' && x.IsW then 
                { Mfrom = 60
                  Mto = 62
                  Mtyp = CasK
                  Mpgn = mv 
                  Meval = Normal
                  Scr10 = 0
                  Scr25 = 0 
                  Bresp = ""
                  ECO = ""
                  FicsPc = 0.0}
            elif c = 'K' then 
                { Mfrom = 4
                  Mto = 6
                  Mtyp = CasK
                  Mpgn = mv 
                  Meval = Normal
                  Scr10 = 0
                  Scr25 = 0 
                  Bresp = ""
                  ECO = ""
                  FicsPc = 0.0}
            elif x.IsW then 
                { Mfrom = 60
                  Mto = 58
                  Mtyp = CasQ
                  Mpgn = mv 
                  Meval = Normal
                  Scr10 = 0
                  Scr25 = 0 
                  Bresp = ""
                  ECO = ""
                  FicsPc = 0.0}
            else 
                { Mfrom = 4
                  Mto = 2
                  Mtyp = CasQ
                  Mpgn = mv 
                  Meval = Normal
                  Scr10 = 0
                  Scr25 = 0 
                  Bresp = ""
                  ECO = ""
                  FicsPc = 0.0}
        //pawn capture like exd6
        | PawnCapture(f, sq) -> 
            let mto = Ref.SqDct.[sq]
            
            let r = 
                int (m.[2].ToString()) + (if x.IsW then -1
                                          else 1)
            
            let mtyp = 
                if x.Sqs.[mto] = ' ' then Ep
                else Standard
            
            let mfrom = Ref.SqDct.[f.ToString() + r.ToString()]
            { Mfrom = mfrom
              Mto = mto
              Mtyp = mtyp
              Mpgn = mv 
              Meval = Normal
              Scr10 = 0
              Scr25 = 0 
              Bresp = ""
              ECO = ""
              FicsPc = 0.0}
        //ambiguous file like Nge2
        | AbiguousFile(p, f, sq) -> 
            let mto = Ref.SqDct.[sq]
            
            let pc = 
                if x.IsW then p
                else p |> Char.ToLower
            
            let fn = Ref.fDct.[f]
            
            let mfs = 
                x.Sqs
                |> Array.mapi (fun i c -> i, c)
                |> Array.filter (fun (_, c) -> c = pc)
                |> Array.map fst
            if mfs.Length = 1 then 
                { Mfrom = mfs.[0]
                  Mto = mto
                  Mtyp = Standard
                  Mpgn = mv 
                  Meval = Normal
                  Scr10 = 0
                  Scr25 = 0 
                  Bresp = ""
                  ECO = ""
                  FicsPc = 0.0}
            else 
                match pc with
                | 'N' | 'n' -> 
                    let ms = 
                        mfs
                        |> Array.filter (isnmv mto)
                        |> Array.filter (fun f -> f % 8 = fn)
                    if ms.Length = 1 then 
                        { Mfrom = ms.[0]
                          Mto = mto
                          Mtyp = Standard
                          Mpgn = mv 
                          Meval = Normal
                          Scr10 = 0
                          Scr25 = 0 
                          Bresp = ""
                          ECO = ""
                          FicsPc = 0.0}
                    else fl()
                | 'R' | 'r' | 'Q' | 'q' -> 
                    let fmfs = mfs |> Array.filter (fun f -> f % 8 = fn)
                    if fmfs.Length = 1 then 
                        { Mfrom = fmfs.[0]
                          Mto = mto
                          Mtyp = Standard
                          Mpgn = mv 
                          Meval = Normal
                          Scr10 = 0
                          Scr25 = 0 
                          Bresp = ""
                          ECO = ""
                          FicsPc = 0.0}
                    else fl()
                | _ -> fl()
        //ambiguous rank like R7a6
        | AbiguousRank(p, r, sq) -> 
            let mto = Ref.SqDct.[sq]
            
            let pc = 
                if x.IsW then p
                else p |> Char.ToLower
            
            let rn = Ref.rDct.[r]
            
            let mfs = 
                x.Sqs
                |> Array.mapi (fun i c -> i, c)
                |> Array.filter (fun (_, c) -> c = pc)
                |> Array.map fst
            if mfs.Length = 1 then 
                { Mfrom = mfs.[0]
                  Mto = mto
                  Mtyp = Standard
                  Mpgn = mv 
                  Meval = Normal
                  Scr10 = 0
                  Scr25 = 0 
                  Bresp = ""
                  ECO = ""
                  FicsPc = 0.0}
            else 
                match pc with
                | 'N' | 'n' -> 
                    let ms = 
                        mfs
                        |> Array.filter (isnmv mto)
                        |> Array.filter (fun f -> f / 8 = rn)
                    if ms.Length = 1 then 
                        { Mfrom = ms.[0]
                          Mto = mto
                          Mtyp = Standard
                          Mpgn = mv 
                          Meval = Normal
                          Scr10 = 0
                          Scr25 = 0 
                          Bresp = ""
                          ECO = ""
                          FicsPc = 0.0}
                    else fl()
                | 'R' | 'r' | 'Q' | 'q' -> 
                    let rmfs = mfs |> Array.filter (fun f -> f / 8 = rn)
                    if rmfs.Length = 1 then 
                        { Mfrom = rmfs.[0]
                          Mto = mto
                          Mtyp = Standard
                          Mpgn = mv 
                          Meval = Normal
                          Scr10 = 0
                          Scr25 = 0 
                          Bresp = ""
                          ECO = ""
                          FicsPc = 0.0}
                    else fl()
                | _ -> fl()
        //pawn promotion like b8=Q
        | Promotion(sq, pc) -> 
            let mto = Ref.SqDct.[sq]
            
            let r = 
                int (m.[1].ToString()) + (if x.IsW then -1
                                          else 1)
            
            let mfrom = Ref.SqDct.[m.[0].ToString() + r.ToString()]
            { Mfrom = mfrom
              Mto = mto
              Mtyp = Prom(pc)
              Mpgn = mv 
              Meval = Normal
              Scr10 = 0
              Scr25 = 0 
              Bresp = ""
              ECO = ""
              FicsPc = 0.0}
        //pawn promotion capture like a*b8=Q
        | PromCapture(f, sq, pc) -> 
            let mto = Ref.SqDct.[sq]
            
            let r = 
                int (m.[2].ToString()) + (if x.IsW then -1
                                          else 1)
            
            let mfrom = Ref.SqDct.[f.ToString() + r.ToString()]
            { Mfrom = mfrom
              Mto = mto
              Mtyp = Prom(pc)
              Mpgn = mv 
              Meval = Normal
              Scr10 = 0
              Scr25 = 0 
              Bresp = ""
              ECO = ""
              FicsPc = 0.0}
    
    /// Gets Move list give source and target
    member internal x.GetPossSqs(mfrom) = 
        let isw p = p.ToString().ToUpper() = p.ToString()
        let pc = x.Sqs.[mfrom]
        if (pc |> isw) <> x.IsW then []
        else
            match pc with
            | 'P' -> Ref.movsPW.[mfrom]
            | 'p' -> Ref.movsPB.[mfrom]
            | 'N' | 'n' -> Ref.movsN.[mfrom]
            | 'B' | 'b' -> Ref.raysB.[mfrom] |> List.concat
            | 'R' | 'r' -> Ref.raysR.[mfrom] |> List.concat
            | 'Q' | 'q' -> Ref.raysQ.[mfrom] |> List.concat
            | 'K' -> 
                if mfrom = 60 then [ 58; 62 ] @ Ref.movsK.[mfrom]
                else Ref.movsK.[mfrom]
            | 'k' -> 
                if mfrom = 4 then [ 2; 6 ] @ Ref.movsK.[mfrom]
                else Ref.movsK.[mfrom]
            | _ -> []
    
    /// Gets Move give source and target
    member x.GetMvFT(mfrom, mto) = 
        let isw p = p.ToString().ToUpper() = p.ToString()
        let pc = x.Sqs.[mfrom]
        let mpgn = pc.ToString().ToUpper() + Ref.sq.[mto]
        
        let mv = 
            { Mfrom = mfrom
              Mto = mto
              Mtyp = Standard
              Mpgn = mpgn 
              Meval = Normal
              Scr10 = 0
              Scr25 = 0 
              Bresp = ""
              ECO = ""
              FicsPc = 0.0}
        
        let pcto = x.Sqs.[mto]
        if pcto <> ' ' && (pc |> isw) = (pcto |> isw) then { mv with Mtyp = Invalid }
        else 
            match pc with
            | 'P' | 'p' -> 
                if Ref.f.[mto] = Ref.f.[mfrom] then { mv with Mpgn = Ref.sq.[mto] }
                elif pcto <> ' ' then { mv with Mpgn = Ref.f.[mfrom] + "x" + Ref.sq.[mto] }
                else 
                    { mv with Mpgn = Ref.f.[mfrom] + "x" + Ref.sq.[mto]
                              Mtyp = Ep }
            | 'N' | 'n' | 'R' | 'r' -> 
                let mpgn = 
                    if pcto = ' ' then mpgn
                    else pc.ToString().ToUpper() + "x" + Ref.sq.[mto]
                try 
                    x.GetMv(mpgn)
                with _ -> 
                    try 
                        let fmpgn = mpgn.Substring(0, 1) + Ref.f.[mfrom] + mpgn.Substring(1)
                        x.GetMv(fmpgn)
                    with _ -> { mv with Mpgn = mpgn.Substring(0, 1) + Ref.r.[mfrom] + mpgn.Substring(1) }
            | 'B' | 'b' | 'Q' | 'q' -> 
                if pcto = ' ' then mv
                else { mv with Mpgn = pc.ToString().ToUpper() + "x" + Ref.sq.[mto] }
            | 'K' -> 
                if mto = 62 && mfrom = 60 then 
                    { mv with Mpgn = "O-O"
                              Mtyp = CasK }
                elif mto = 58 && mfrom = 60 then 
                    { mv with Mpgn = "O-O-O"
                              Mtyp = CasQ }
                elif pcto = ' ' then mv
                else { mv with Mpgn = pc.ToString().ToUpper() + "x" + Ref.sq.[mto] }
            | 'k' -> 
                if mto = 6 && mfrom = 4 then 
                    { mv with Mpgn = "O-O"
                              Mtyp = CasK }
                elif mto = 2 && mfrom = 4 then 
                    { mv with Mpgn = "O-O-O"
                              Mtyp = CasQ }
                elif pcto = ' ' then mv
                else { mv with Mpgn = pc.ToString().ToUpper() + "x" + Ref.sq.[mto] }
            | _ -> { mv with Mtyp = Invalid }

    /// Gets Move given UCI string
    member x.GetMvUCI (uci:string) = 
        let mfrom = Ref.SqDct.[uci.Substring(0,2)]
        let mto = Ref.SqDct.[uci.Substring(2,2)]
        x.GetMvFT(mfrom, mto)
