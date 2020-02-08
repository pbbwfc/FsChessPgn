namespace Olm

open System.IO

module Game =
    
    type private State = 
        | Unknown
        | InMove
        | InComment of int
        | InSingleLineComment
        | InRAV of int
        | InNAG
        | InNum
        | InRes
        | FinishedOK
        | Invalid
        | FinishedInvalid
    
    let rec GetEntries(mvstr : string) = 
        let byteArray = System.Text.Encoding.ASCII.GetBytes(mvstr)
        use streamm = new MemoryStream(byteArray)
        use srm = new StreamReader(streamm)
        let nl = System.Environment.NewLine

        let rec proclin st cstr s mno isw bd msel = 
            if s = "" then 
                match st with
                |InMove ->
                    let mv = cstr|>Convert.SanToMove bd
                    let nbd = bd|>Board.Push mv
                    let mte = MvEntry(mno,isw,cstr,{PreBrd=bd;Mv=mv;PostBrd=nbd})
                    let nmsel = mte::msel
                    let nmno = if isw then mno else mno+1
                    let nisw = not isw
                    Unknown,"",nmno,nisw,nbd,nmsel
                |InNAG ->
                    let mte = NagEntry(cstr|>int|>Ng)
                    let nmsel = mte::msel
                    Unknown,"",mno,isw,bd,nmsel
                |InSingleLineComment ->
                    let mte = CommEntry(cstr)
                    let nmsel = mte::msel
                    Unknown,"",mno,isw,bd,nmsel
                |InRes ->
                    let bits = cstr.Split([|'{'|])
                    let nmsel =
                        if bits.Length=1 then
                            let mte = EndEntry(cstr|>GmResult.Parse)
                            mte::msel
                        else
                            let mte = EndEntry(bits.[0].Trim()|>GmResult.Parse)
                            let msel1 = mte::msel
                            let mte1 = CommEntry(bits.[1].Trim([|'}'|]))
                            mte1::msel1
                    FinishedOK,"",mno,isw,bd,nmsel
                |InComment(_) |InRAV(_) -> st,cstr+nl,mno,isw,bd,msel 
                |Unknown |InNum -> st,cstr,mno,isw,bd,msel
                |Invalid |FinishedOK |FinishedInvalid -> failwith "Invalid state at end of line"
            else 
                let hd = s.[0]
                let tl = s.[1..]
                match st with
                |InComment(cl) -> 
                    if hd='}' && cl=1 then
                        let mte = CommEntry(cstr)
                        let nmsel = mte::msel
                        proclin Unknown "" tl mno isw bd nmsel
                    elif hd='}' then
                        proclin (InComment(cl-1)) (cstr+hd.ToString()) tl mno isw bd msel
                    elif hd='{' then
                        proclin (InComment(cl+1)) (cstr+hd.ToString()) tl mno isw bd msel
                    else
                        proclin st (cstr+hd.ToString()) tl mno isw bd msel
                |InSingleLineComment ->
                    proclin st (cstr+hd.ToString()) tl mno isw bd msel
                |InRAV(cl) -> 
                    if hd=')' && cl=1 then
                        let mselr = GetEntries(cstr)
                        let mte = RvEntry(mselr)
                        let nmsel = mte::msel
                        proclin Unknown "" tl mno isw bd nmsel
                    elif hd=')' then
                        proclin (InRAV(cl-1)) (cstr+hd.ToString()) tl mno isw bd msel
                    elif hd='(' then
                        proclin (InRAV(cl+1)) (cstr+hd.ToString()) tl mno isw bd msel
                    else
                        proclin st (cstr+hd.ToString()) tl mno isw bd msel
                |InNAG -> 
                    if hd=' ' then
                        let mte = NagEntry(cstr|>int|>Ng)
                        let nmsel = mte::msel
                        proclin Unknown "" tl mno isw bd nmsel
                    else
                        proclin st (cstr+hd.ToString()) tl mno isw bd msel
                |InNum -> 
                    if System.Char.IsNumber(hd) || hd = '.' || hd = ' ' //&& tl.Length>0 && tl.StartsWith(".")
                    then
                        proclin st (cstr+hd.ToString()) tl mno isw bd msel
                    elif hd='/'||hd='-' then
                        proclin InRes (cstr+hd.ToString()) tl mno isw bd msel
                    else
                        proclin InMove (hd.ToString()) tl mno isw bd msel
                |InRes -> 
                    proclin st (cstr+hd.ToString()) tl mno isw bd msel
                |Invalid -> 
                    proclin st cstr tl mno isw bd msel
                |InMove -> 
                    if hd=' ' then
                        let mv = cstr|>Convert.SanToMove bd
                        let nbd = bd|>Board.Push mv
                        let mte = MvEntry(mno,isw,cstr,{PreBrd=bd;Mv=mv;PostBrd=nbd})
                        let nmsel = mte::msel
                        let nmno = if isw then mno else mno+1
                        let nisw = not isw
                        proclin Unknown "" tl nmno nisw nbd nmsel
                    else
                        proclin st (cstr+hd.ToString()) tl mno isw bd msel
                |FinishedOK |FinishedInvalid -> st,cstr,mno,isw,bd,msel
                |Unknown -> 
                    let st, ns = 
                        match hd with
                        | '{' -> InComment(1), s.[1..]
                        | '(' -> InRAV(1), s.[1..]
                        | '$' -> InNAG, s.[1..]
                        | '*' -> InRes, s
                        | ';' -> InSingleLineComment, s.[1..]
                        | c when System.Char.IsNumber(c) || c = '.' -> InNum, s
                        | ' ' -> Unknown, s.[1..]
                        | _ -> InMove, s
                    proclin st cstr ns mno isw bd msel
    
        let rec getgm st cstr mno isw bd msel = 
            let lin = srm.ReadLine()
            if lin |> isNull then msel|>List.rev
            else 
                let nst,ncstr,nmno,nisw,nbd,nmsel = proclin st cstr lin mno isw bd msel
                if nst = FinishedOK then nmsel|>List.rev
                elif nst = FinishedInvalid then []
                else getgm nst ncstr nmno nisw nbd nmsel
    
        let msel = getgm Unknown "" 1 true Board.Start []
        msel

    let rec MoveStr(writer:TextWriter) (entry:MvStrEntry) =
        match entry with
        |MvEntry(mn,isw,mv,_) -> 
            if isw then
                writer.Write(mn)
                writer.Write(". ")
            writer.Write(mv)
        |CommEntry(str) -> writer.Write("{" + str + "}")
        |EndEntry(gr) -> writer.Write(GmResult.ToUnicode(gr))
        |NagEntry(cd) -> writer.Write("$" + (cd|>int).ToString())
        |RvEntry(ml) -> 
            writer.Write("(")
            writer.Write(MoveText(ml))
            writer.Write(")")
        writer.ToString()

     and MoveText(ml:MvStrEntry list) :string =
         let writer = new StringWriter()
         let doent i m =
             let str = MoveStr writer m
             if i<ml.Length-1 then str + " " else str

         let str = ml|>List.mapi doent|>List.reduce(+)
         str
    
    let AddRav (msel:MvStrEntry list) (irs:int list) (amv:aMove) = 
        let rec getadd mct ci nmse (imsel:MvStrEntry list) (omsel:MvStrEntry list) =
            if ci>omsel.Length then getadd mct ci nmse imsel.Tail (imsel.Head::omsel)
            elif imsel.IsEmpty then (RvEntry([nmse])::omsel)|>List.rev,omsel.Length
            else
                //ignore first move
                let mte = imsel.Head
                if mct=0 then
                    match mte with
                    |MvEntry(_) -> getadd 1 ci nmse imsel.Tail (imsel.Head::omsel)
                    |_ -> getadd 0 ci nmse imsel.Tail (imsel.Head::omsel)
                else
                    match mte with
                    |EndEntry(_) -> ((RvEntry([nmse])::omsel)|>List.rev)@imsel,omsel.Length 
                    |MvEntry(mn,isw,_,amv) -> 
                        //need to fix black move
                        (mte::(RvEntry([nmse])::omsel)|>List.rev)@imsel.Tail,omsel.Length
                    |_ -> getadd 1 ci nmse imsel.Tail (imsel.Head::omsel)
        if irs.Length=1 then
            //before moves
            if irs.Head= -1 then
                //TODO
                let nmse = MvEntry(1,true,"",amv)
                msel.Head::RvEntry([nmse])::msel.Tail,[1;0]
            else
                let cmv = msel.[irs.Head]
                let bd,lmn,lisw =
                    match cmv with
                    |MvEntry(mn,isw,_,amv) -> amv.PostBrd, mn, isw
                    |_ -> failwith "should be a move"
                let mn = if lisw then lmn else lmn+1
                //TODO
                let nmse = MvEntry(mn,bd.WhosTurn=Player.Black,"",amv)

                let nmsel,ni = getadd 0 (irs.Head+1) nmse msel []
                nmsel,[ni;0]
        else
            let rec getcur indx (cirs:int list) (imsel:MvStrEntry list) =
                if cirs.Length=1 && indx=cirs.Head then imsel.Head
                elif indx=cirs.Head then
                    let rv = imsel.Head
                    match rv with
                    |RvEntry(nmsel) -> getcur 0 cirs.Tail nmsel
                    |_ -> failwith "should be RAV"
                else
                    let mte = imsel.Head
                    match mte with
                    |MvEntry(_) -> getcur (indx+1) cirs imsel.Tail
                    |_ -> getcur (indx+1) cirs imsel.Tail
            let cmv = getcur 0 irs msel        
            let bd,lmn,lisw =
                match cmv with
                |MvEntry(mn,isw,_,amv) -> amv.PostBrd, mn, isw
                |_ -> failwith "should be a move"
            let mn = if lisw then lmn else lmn+1
            //TODO
            let nmse = MvEntry(mn,bd.WhosTurn=Player.Black,"",amv)
            let rec getnmsel (cirs:int list) (imsel:MvStrEntry list) =
                if cirs.Length=1 then 
                    let nmsel,_ = getadd 0 (cirs.Head+1) nmse imsel []
                    nmsel
                else
                    let i = cirs.Head
                    let rav = imsel.[i]
                    match rav with
                    |RvEntry(nmsel) ->
                        imsel.[..i-1]@[RvEntry(getnmsel cirs.Tail nmsel)]@imsel.[i+1..]
                    |_ -> failwith "should be RAV"
            let rec getnmirs (cirs:int list) (pirs:int list) (imsel:MvStrEntry list) =
                if cirs.Length=1 then 
                    let _,ni = getadd 0 (cirs.Head+1) nmse imsel []
                    (0::ni::pirs)|>List.rev
                else
                    let i = cirs.Head
                    let rav = imsel.[i]
                    match rav with
                    |RvEntry(nmsel) ->
                        getnmirs cirs.Tail (i::pirs) nmsel
                    |_ -> failwith "should be RAV"
            let nmsel = getnmsel irs msel
            let nirs = getnmirs irs [] msel
            nmsel,nirs

    let AddMv (msel:MvStrEntry list) (irs:int list) (amv:aMove)  = 
        let rec getext ci nmse (imsel:MvStrEntry list) (omsel:MvStrEntry list) =
            if ci>omsel.Length then getext ci nmse imsel.Tail (imsel.Head::omsel)
            elif imsel.IsEmpty then 
                (nmse::omsel)|>List.rev,omsel.Length
            else
                let mte = imsel.Head
                match mte with
                |EndEntry(_) -> 
                    ((nmse::omsel)|>List.rev)@imsel,omsel.Length
                |_ -> getext ci nmse imsel.Tail (imsel.Head::omsel)
        if irs.Length=1 then
            //allow for empty list
            if msel.IsEmpty then
                let bd = Board.Start
                let mn = 1
                //TODO
                let nmse = MvEntry(mn,true,"",amv)
                [nmse],[0]
            //if not with a selected move
            elif irs.Head = -1 then
                let bd = Board.Start
                let mn = 1
                //TODO
                let nmse = MvEntry(mn,true,"",amv)
                let nmsel,ni = getext (irs.Head+1) nmse msel []
                nmsel,[ni]
            else
                let cmv = msel.[irs.Head]
                let bd,lmn,lisw =
                    match cmv with
                    |MvEntry(mn,isw,_,amv) -> amv.PostBrd, mn, isw
                    |_ -> failwith "should be a move"
                let mn = if lisw then lmn else lmn+1
                //TODO
                let nmse = MvEntry(mn,bd.WhosTurn=Player.Black,"",amv)
                let nmsel,ni = getext (irs.Head+1) nmse msel []
                nmsel,[ni]
        else
            let rec getcur indx (cirs:int list) (imsel:MvStrEntry list) =
                if cirs.Length=1 && indx=cirs.Head then imsel.Head
                elif indx=cirs.Head then
                    let rv = imsel.Head
                    match rv with
                    |RvEntry(nmsel) -> getcur 0 cirs.Tail nmsel
                    |_ -> failwith "should be RAV"
                else
                    let mte = imsel.Head
                    match mte with
                    |MvEntry(_) -> getcur (indx+1) cirs imsel.Tail
                    |_ -> getcur (indx+1) cirs imsel.Tail
            let cmv = getcur 0 irs msel        
            let bd,lmn,lisw =
                match cmv with
                |MvEntry(mn,isw,_,amv) -> amv.PostBrd, mn, isw
                |_ -> failwith "should be a move"
            let mn = if lisw then lmn else lmn+1
            //TODO
            let nmse = MvEntry(mn,bd.WhosTurn=Player.Black,"",amv)
            let rec getnmsel (cirs:int list) (imsel:MvStrEntry list) =
                if cirs.Length=1 then 
                    let nmsel,_ = getext (cirs.Head+1) nmse imsel []
                    nmsel
                else
                    let i = cirs.Head
                    let rav = imsel.[i]
                    match rav with
                    |RvEntry(nmsel) ->
                        imsel.[..i-1]@[RvEntry(getnmsel cirs.Tail nmsel)]@imsel.[i+1..]
                    |_ -> failwith "should be RAV"
            let rec getnmirs (cirs:int list) (pirs:int list) (imsel:MvStrEntry list) =
                if cirs.Length=1 then 
                    let _,ni = getext (cirs.Head+1) nmse imsel []
                    (ni::pirs)|>List.rev
                else
                    let i = cirs.Head
                    let rav = imsel.[i]
                    match rav with
                    |RvEntry(nmsel) ->
                        getnmirs cirs.Tail (i::pirs) nmsel
                    |_ -> failwith "should be RAV"
            let nmsel = getnmsel irs msel
            let nirs = getnmirs irs [] msel
            nmsel,nirs

    let CommentBefore (msel:MvStrEntry list) (irs:int list) (str:string) =
         let mte = CommEntry(str)
         if irs.Length=1 then
             //allow for empty list
             if msel.IsEmpty then
                 [mte]
             else
                 let i = irs.Head
                 let nmsel = 
                     if i=0 then mte::msel
                     else
                         msel.[..i-1]@[mte]@msel.[i..]
                 nmsel
         else
             let rec getnmsel (cirs:int list) (imsel:MvStrEntry list) =
                 if cirs.Length=1 then 
                     let i = cirs.Head
                     let nmsel = 
                         if i=0 then mte::imsel
                         else
                             imsel.[..i-1]@[mte]@imsel.[i..]
                     nmsel
                 else
                     let i = cirs.Head
                     let rav = imsel.[i]
                     match rav with
                     |RvEntry(nmsel) ->
                         imsel.[..i-1]@[RvEntry(getnmsel cirs.Tail nmsel)]@imsel.[i+1..]
                     |_ -> failwith "should be RAV"
             let nmsel = getnmsel irs msel
             nmsel

    let CommentAfter (msel:MvStrEntry list) (irs:int list) (str:string) =
        let mte = CommEntry(str)
        if irs.Length=1 then
            //allow for empty list
            if msel.IsEmpty then
                [mte]
            else
                let i = irs.Head
                let nmsel = 
                    if i=msel.Length-1 then msel@[mte]
                    else
                        msel.[..i]@[mte]@msel.[i+1..]
                nmsel
        else
            let rec getnmsel (cirs:int list) (imsel:MvStrEntry list) =
                if cirs.Length=1 then 
                    let i = cirs.Head
                    let nmsel = 
                        if i=imsel.Length-1 then imsel@[mte]
                        else
                            imsel.[..i]@[mte]@imsel.[i+1..]
                    nmsel
                else
                    let i = cirs.Head
                    let rav = imsel.[i]
                    match rav with
                    |RvEntry(nmsel) ->
                        imsel.[..i-1]@[RvEntry(getnmsel cirs.Tail nmsel)]@imsel.[i+1..]
                    |_ -> failwith "should be RAV"
            let nmsel = getnmsel irs msel
            nmsel
 
    let EditComment (msel:MvStrEntry list) (irs:int list) (str:string) =
        let mte = CommEntry(str)
        if irs.Length=1 then
            //allow for empty list
            if msel.IsEmpty then
                [mte]
            else
                let i = irs.Head
                let nmsel = 
                    if i=0 then mte::msel.Tail
                    else
                        msel.[..i-1]@[mte]@msel.[i+1..]
                nmsel
        else
            let rec getnmsel (cirs:int list) (imsel:MvStrEntry list) =
                if cirs.Length=1 then 
                    let i = cirs.Head
                    let nmsel = 
                        if i=0 then mte::imsel.Tail
                        elif i=imsel.Length-1 then imsel.[..i-1]@[mte]
                        else
                            imsel.[..i-1]@[mte]@imsel.[i+1..]
                    nmsel
                else
                    let i = cirs.Head
                    let rav = imsel.[i]
                    match rav with
                    |RvEntry(nmsel) ->
                        imsel.[..i-1]@[RvEntry(getnmsel cirs.Tail nmsel)]@imsel.[i+1..]
                    |_ -> failwith "should be RAV"
            let nmsel = getnmsel irs msel
            nmsel

    let AddNag (msel:MvStrEntry list) (irs:int list) (ng:Nag) =
        let mte = NagEntry(ng)
        if irs.Length=1 then
            //allow for empty list
            if msel.IsEmpty then
                [mte]
            else
                let i = irs.Head
                let nmsel = 
                    if i=msel.Length-1 then msel@[mte]
                    else
                        msel.[..i]@[mte]@msel.[i+1..]
                nmsel
        else
            let rec getnmsel (cirs:int list) (imsel:MvStrEntry list) =
                if cirs.Length=1 then 
                    let i = cirs.Head
                    let nmsel = 
                        if i=imsel.Length-1 then imsel@[mte]
                        else
                            imsel.[..i]@[mte]@imsel.[i+1..]
                    nmsel
                else
                    let i = cirs.Head
                    let rav = imsel.[i]
                    match rav with
                    |RvEntry(nmsel) ->
                        imsel.[..i-1]@[RvEntry(getnmsel cirs.Tail nmsel)]@imsel.[i+1..]
                    |_ -> failwith "should be RAV"
            let nmsel = getnmsel irs msel
            nmsel

    let EditNag (msel:MvStrEntry list) (irs:int list) (ng:Nag) =
         let mte = NagEntry(ng)
         if irs.Length=1 then
             //allow for empty list
             if msel.IsEmpty then
                 [mte]
             else
                 let i = irs.Head
                 let nmsel = 
                     if i=0 then mte::msel.Tail
                     else
                         msel.[..i-1]@[mte]@msel.[i+1..]
                 nmsel
         else
             let rec getnmsel (cirs:int list) (imsel:MvStrEntry list) =
                 if cirs.Length=1 then 
                     let i = cirs.Head
                     let nmsel = 
                         if i=0 then mte::imsel.Tail
                         elif i=imsel.Length-1 then imsel.[..i-1]@[mte]
                         else
                             imsel.[..i-1]@[mte]@imsel.[i+1..]
                     nmsel
                 else
                     let i = cirs.Head
                     let rav = imsel.[i]
                     match rav with
                     |RvEntry(nmsel) ->
                         imsel.[..i-1]@[RvEntry(getnmsel cirs.Tail nmsel)]@imsel.[i+1..]
                     |_ -> failwith "should be RAV"
             let nmsel = getnmsel irs msel
             nmsel

    let DeleteNag (msel:MvStrEntry list) (irs:int list)  =
        if irs.Length=1 then
            let i = irs.Head
            let nmsel = 
                if i=0 then msel.Tail
                else
                    msel.[..i-1]@msel.[i+1..]
            nmsel
        else
            let rec getnmsel (cirs:int list) (imsel:MvStrEntry list) =
                if cirs.Length=1 then 
                    let i = cirs.Head
                    let nmsel = 
                        if i=0 then imsel.Tail
                        elif i=imsel.Length-1 then imsel.[..i-1]
                        else
                            imsel.[..i-1]@imsel.[i+1..]
                    nmsel
                else
                    let i = cirs.Head
                    let rav = imsel.[i]
                    match rav with
                    |RvEntry(nmsel) ->
                        imsel.[..i-1]@[RvEntry(getnmsel cirs.Tail nmsel)]@imsel.[i+1..]
                    |_ -> failwith "should be RAV"
            let nmsel = getnmsel irs msel
            nmsel

