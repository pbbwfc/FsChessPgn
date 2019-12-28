namespace FsChessPgn

open FsChess

module Game =

    let Start = GameEMP

    let MoveCount(mtel:MoveTextEntry list) =
        let mc(mte:MoveTextEntry) =
            match mte with
            |HalfMoveEntry(_) -> 1
            |_ -> 0
        if mtel.IsEmpty then 0
        else
            mtel|>List.map mc|>List.reduce(+)
        
    let FullMoveCount(mtel:MoveTextEntry list) = MoveCount(mtel)/2

    let GetMoves(mtel:MoveTextEntry list) =
        let gm(mte:MoveTextEntry) =
            match mte with
            |HalfMoveEntry(_,_,mv,_) -> [mv]
            |_ -> []
        mtel|>List.map gm|>List.concat
    
    let AddTag (tagstr:string) (gm:Game) =
        let getdt (dtstr:string) =
            let a = dtstr.Split([|'.'|])|>Array.map(fun s -> s.Trim())
            let y,m,d = if a.Length=3 then a.[0],a.[1],a.[2] else a.[0],"??","??"
            let yop = if y="????" then None else Some(int y)
            let mop = if m="??" then None else Some(int m)
            let dop = if d="??" then None else Some(int d)
            yop,mop,dop
        
        let k,v = tagstr.Trim().Split([|'"'|])|>Array.map(fun s -> s.Trim())|>fun a -> a.[0],a.[1].Trim('"')
        match k with
        | "Event" -> {gm with Event = v}
        | "Site" -> {gm with Site = v}
        | "Date" -> 
            let yop,mop,dop = v|>getdt
            {gm with Year = yop; Month = mop; Day = dop}
        | "Round" -> {gm with Round = v}
        | "White" -> {gm with WhitePlayer = v}
        | "Black" -> {gm with BlackPlayer = v}
        | "Result" -> {gm with Result = v|>GameResult.Parse}
        | "FEN" -> {gm with BoardSetup = v|>FEN.Parse|>Board.FromFEN|>Some}
        | _ ->
            {gm with AdditionalInfo=gm.AdditionalInfo.Add(k,v)}
    
    let AddMoveEntry (mte:MoveTextEntry) (gm:Game) =
        {gm with MoveText=gm.MoveText@[mte]}

    let RemoveMoveEntry (gm:Game) =
        let mtel = gm.MoveText
        let nmtel =
            if mtel.IsEmpty then mtel
            else
                mtel|>List.rev|>List.tail|>List.rev
        {gm with MoveText=nmtel}

    let AddpMove (pmv:pMove) (gm:Game) =
        let mtel = gm.MoveText
        let mc = mtel|>MoveCount
        let mn = if mc%2=0 then Some(mc/2+1) else None
        let mte = HalfMoveEntry(mn,false,pmv,None)
        gm|>AddMoveEntry mte
            
    let AddSan (san:string) (gm:Game) =
        let pmv = san|>pMove.Parse
        gm|>AddpMove pmv
     
    let pretty(gm:Game) = 
        let mtel = gm.MoveText
        if mtel.IsEmpty then "No moves"
        elif mtel.Length<6 then
            let mvstr =mtel|>List.map PgnWrite.MoveTextEntryStr|>List.reduce(fun a b -> a + " " + b)
            "moves: " + mvstr
        else
            let rl = mtel|>List.rev
            let l5 = mtel.[0..4]|>List.rev
            let mvstr = l5|>List.map PgnWrite.MoveTextEntryStr|>List.reduce(fun a b -> a + " " + b)
            "moves: ..." + mvstr
   
    
    //TODO
    let SetaMoves(gm:Game) =
        let rec setamv (pmvl:MoveTextEntry list) bd opmvl =
            if pmvl|>List.isEmpty then opmvl|>List.rev
            else
                let mte = pmvl.Head
                match mte with
                |HalfMoveEntry(mn,ic,mv,_) -> 
                    let amv = mv|>pMove.ToaMove bd
                    let nmte = HalfMoveEntry(mn,ic,mv,Some(amv))
                    setamv pmvl.Tail amv.PostBrd (nmte::opmvl)
                |_ -> setamv pmvl.Tail bd (mte::opmvl)
        
        let ibd = if gm.BoardSetup.IsSome then gm.BoardSetup.Value else Board.Start
        let nmt = setamv gm.MoveText ibd []
        {gm with MoveText=nmt}

