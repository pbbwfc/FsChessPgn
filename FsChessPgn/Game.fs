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
            let l5 = rl.[0..4]|>List.rev
            let mvstr = l5|>List.map PgnWrite.MoveTextEntryStr|>List.reduce(fun a b -> a + " " + b)
            "moves: ..." + mvstr
   
    let SetaMoves(gm:Game) =
        let rec setamv (pmvl:MoveTextEntry list) prebd bd opmvl =
            if pmvl|>List.isEmpty then opmvl|>List.rev
            else
                let mte = pmvl.Head
                match mte with
                |HalfMoveEntry(mn,ic,mv,_) -> 
                    let amv = mv|>pMove.ToaMove bd
                    let nmte = HalfMoveEntry(mn,ic,mv,Some(amv))
                    setamv pmvl.Tail amv.PreBrd amv.PostBrd (nmte::opmvl)
                |RAVEntry(mtel) -> 
                    let nmtel = setamv mtel prebd prebd []
                    let nmte = RAVEntry(nmtel)
                    setamv pmvl.Tail prebd bd (nmte::opmvl)
                |_ -> setamv pmvl.Tail prebd bd (mte::opmvl)
        
        let ibd = if gm.BoardSetup.IsSome then gm.BoardSetup.Value else Board.Start
        let nmt = setamv gm.MoveText ibd ibd []
        {gm with MoveText=nmt}

    let AddRav (gm:Game) (irs:int list) (pmv:pMove) = 
        let rec getadd mct ci nmte (imtel:MoveTextEntry list) (omtel:MoveTextEntry list) =
            if ci>omtel.Length then getadd mct ci nmte imtel.Tail (imtel.Head::omtel)
            elif imtel.IsEmpty then (RAVEntry([nmte])::omtel)|>List.rev,omtel.Length
            else
                //ignore first move
                let mte = imtel.Head
                if mct=0 then
                    match mte with
                    |HalfMoveEntry(_) -> getadd 1 ci nmte imtel.Tail (imtel.Head::omtel)
                    |_ -> getadd 0 ci nmte imtel.Tail (imtel.Head::omtel)
                else
                    match mte with
                    |HalfMoveEntry(_) |GameEndEntry(_) -> 
                        //need to include before this
                        ((RAVEntry([nmte])::omtel)|>List.rev)@imtel,omtel.Length
                    |_ -> getadd 1 ci nmte imtel.Tail (imtel.Head::omtel)
        let filtmv mte =
            match mte with
            |HalfMoveEntry(_) -> true
            |_ -> false
        
        if irs.Length=1 then
            let cmv = gm.MoveText.[irs.Head]
            let mvs = gm.MoveText.[0..irs.Head]|>List.filter filtmv
            let mn = (mvs.Length+2)/2|>Some
            let bd =
                match cmv with
                |HalfMoveEntry(_,_,_,amv) -> amv.Value.PostBrd
                |_ -> failwith "should be a move"
            let amv = pmv|>pMove.ToaMove bd
            let nmte = HalfMoveEntry(mn,bd.WhosTurn=Player.Black,pmv,Some(amv))

            let nmtel,ni = getadd 0 (irs.Head+1) nmte gm.MoveText []
            {gm with MoveText=nmtel},[ni;0]
        else
            let rec getmncur indx cmn (cirs:int list) (mtel:MoveTextEntry list) =
                if cirs.Length=1 && indx=cirs.Head then mtel.Head,cmn
                elif indx=cirs.Head then
                    let rv = mtel.Head
                    match rv with
                    |RAVEntry(nmtel) -> getmncur 0 (cmn-1) cirs.Tail nmtel
                    |_ -> failwith "should be RAV"
                else
                    let mte = mtel.Head
                    match mte with
                    |HalfMoveEntry(_) -> getmncur (indx+1) (cmn+1) cirs mtel.Tail
                    |_ -> getmncur (indx+1) cmn cirs mtel.Tail
            let cmv,dmn = getmncur 0 0 irs gm.MoveText        
            let mn = (dmn+2)/2|>Some
            let bd =
                match cmv with
                |HalfMoveEntry(_,_,_,amv) -> amv.Value.PostBrd
                |_ -> failwith "should be a move"
            let amv = pmv|>pMove.ToaMove bd
            let nmte = HalfMoveEntry(mn,bd.WhosTurn=Player.Black,pmv,Some(amv))
            let rec getnmtel (cirs:int list) (mtel:MoveTextEntry list) =
                if cirs.Length=1 then 
                    let nmtel,_ = getadd 0 (cirs.Head+1) nmte mtel []
                    nmtel
                else
                    let i = cirs.Head
                    let rav = mtel.[i]
                    match rav with
                    |RAVEntry(nmtel) ->
                        mtel.[..i-1]@[RAVEntry(getnmtel cirs.Tail nmtel)]@mtel.[i+1..]
                    |_ -> failwith "should be RAV"
            let rec getnmirs (cirs:int list) (pirs:int list) (mtel:MoveTextEntry list) =
                if cirs.Length=1 then 
                    let _,ni = getadd 0 (cirs.Head+1) nmte mtel []
                    (0::ni::pirs)|>List.rev
                else
                    let i = cirs.Head
                    let rav = mtel.[i]
                    match rav with
                    |RAVEntry(nmtel) ->
                        getnmirs cirs.Tail (i::pirs) nmtel
                    |_ -> failwith "should be RAV"
            let nmtel = getnmtel irs gm.MoveText
            let nirs = getnmirs irs [] gm.MoveText
            {gm with MoveText=nmtel},nirs

    let AddMv (gm:Game) (irs:int list) (pmv:pMove) = 
        let rec getext ci nmte (imtel:MoveTextEntry list) (omtel:MoveTextEntry list) =
            if ci>omtel.Length then getext ci nmte imtel.Tail (imtel.Head::omtel)
            elif imtel.IsEmpty then (nmte::omtel)|>List.rev,omtel.Length
            else
                //ignore first move
                let mte = imtel.Head
                match mte with
                |GameEndEntry(_) -> 
                    //need to include before this
                    ((nmte::omtel)|>List.rev)@imtel,omtel.Length
                |_ -> getext ci nmte imtel.Tail (imtel.Head::omtel)
        let filtmv mte =
            match mte with
            |HalfMoveEntry(_) -> true
            |_ -> false
        if irs.Length=1 then
            let cmv = gm.MoveText.[irs.Head]
            let mvs = gm.MoveText.[0..irs.Head]|>List.filter filtmv
            let mn = (mvs.Length+2)/2|>Some
            let bd =
                match cmv with
                |HalfMoveEntry(_,_,_,amv) -> amv.Value.PostBrd
                |_ -> failwith "should be a move"
            let amv = pmv|>pMove.ToaMove bd
            let nmte = HalfMoveEntry(mn,bd.WhosTurn=Player.Black,pmv,Some(amv))

            let nmtel,ni = getext (irs.Head+1) nmte gm.MoveText []
            {gm with MoveText=nmtel},[ni]
        else
            let rec getmncur indx cmn (cirs:int list) (mtel:MoveTextEntry list) =
                if cirs.Length=1 && indx=cirs.Head then mtel.Head,cmn
                elif indx=cirs.Head then
                    let rv = mtel.Head
                    match rv with
                    |RAVEntry(nmtel) -> getmncur 0 (cmn-1) cirs.Tail nmtel
                    |_ -> failwith "should be RAV"
                else
                    let mte = mtel.Head
                    match mte with
                    |HalfMoveEntry(_) -> getmncur (indx+1) (cmn+1) cirs mtel.Tail
                    |_ -> getmncur (indx+1) cmn cirs mtel.Tail
            let cmv,dmn = getmncur 0 0 irs gm.MoveText        
            let mn = (dmn+2)/2|>Some
            let bd =
                match cmv with
                |HalfMoveEntry(_,_,_,amv) -> amv.Value.PostBrd
                |_ -> failwith "should be a move"
            let amv = pmv|>pMove.ToaMove bd
            let nmte = HalfMoveEntry(mn,bd.WhosTurn=Player.Black,pmv,Some(amv))
            let rec getnmtel (cirs:int list) (mtel:MoveTextEntry list) =
                if cirs.Length=1 then 
                    let nmtel,_ = getext (cirs.Head+1) nmte mtel []
                    nmtel
                else
                    let i = cirs.Head
                    let rav = mtel.[i]
                    match rav with
                    |RAVEntry(nmtel) ->
                        mtel.[..i-1]@[RAVEntry(getnmtel cirs.Tail nmtel)]@mtel.[i+1..]
                    |_ -> failwith "should be RAV"
            let rec getnmirs (cirs:int list) (pirs:int list) (mtel:MoveTextEntry list) =
                if cirs.Length=1 then 
                    let _,ni = getext (cirs.Head+1) nmte mtel []
                    (ni::pirs)|>List.rev
                else
                    let i = cirs.Head
                    let rav = mtel.[i]
                    match rav with
                    |RAVEntry(nmtel) ->
                        getnmirs cirs.Tail (i::pirs) nmtel
                    |_ -> failwith "should be RAV"
            let nmtel = getnmtel irs gm.MoveText
            let nirs = getnmirs irs [] gm.MoveText
            {gm with MoveText=nmtel},nirs
