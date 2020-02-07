namespace Olm

open FsChess
open System.IO

module Convert =

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

    let FromPgn(file:string, log:string -> unit) =
        log("Starting conversion from " + file)
        log("Starting loading headers")
        
        let cp = new ChessPack()
        use stream = new FileStream(file, FileMode.Open)
        use sr = new StreamReader(stream)
        let nl = System.Environment.NewLine
        let hdremp i =
            let h = new Hdr()
            h.Num<-i;h.White<-"?";h.W_Elo<-"-";h.Black<-"?";h.B_Elo<-"-";h.Result<-"*";h.Date<-"???.??.??";h.Event<-"?";h.Round<-"?";h.Site<-"?"
            h

        let addtag (tagstr:string) (ihdr:Hdr) =
            let k,v = tagstr.Trim([|'[';']'|]).Split([|'"'|])|>Array.map(fun s -> s.Trim())|>fun a -> a.[0],a.[1].Trim('"')
            match k with
            | "Event" -> ihdr.Event <- v;ihdr,true
            | "Site" -> ihdr.Site  <- v;ihdr,true
            | "Date" -> ihdr.Date <- v;ihdr,true
            | "Round" -> ihdr.Round <- v;ihdr,true
            | "White" -> ihdr.White <- v;ihdr,true
            | "Black" -> ihdr.Black <- v;ihdr,true
            | "Result" -> ihdr.Result <- v;ihdr,true
            | "WhiteElo" -> ihdr.W_Elo <- v;ihdr,true
            | "BlackElo" -> ihdr.B_Elo <- v;ihdr,true
            | "FEN" -> ihdr,false
            | _ ->
                ihdr,true
        
        let rec getgms i inhdr ok chdr cmvtx hdrs mvtxs = 
            let lin = sr.ReadLine()
            if lin |> isNull then chdr::hdrs,cmvtx::mvtxs
            else
                let ln = lin.Trim()
                if lin = "" then getgms i inhdr ok chdr cmvtx hdrs mvtxs
                elif ln.StartsWith("[") then
                    if inhdr then
                        let nhdr,nok = chdr|>addtag ln
                        getgms i true nok nhdr cmvtx hdrs mvtxs
                    else
                        let nhdrs = if ok then chdr::hdrs else hdrs
                        let nmvtxs = if ok then cmvtx::mvtxs else mvtxs
                        let nhdr,nok = (i+1)|>hdremp|>addtag ln
                        getgms (i+1) true nok nhdr "" nhdrs nmvtxs
                else
                    getgms i false ok chdr (cmvtx+nl+lin) hdrs mvtxs
    
        let hdrs,mvstrs = getgms 0 true true (0|>hdremp) "" [] []
        cp.Hdrs <- hdrs|>List.rev|>List.toArray
        cp.MvsStrs <- mvstrs|>List.rev|>List.toArray
        log("Finished loading header")
        log("Starting encoding moves")
        //now need to extract the Mvs form each MvsStr
        let getmvs i (mvsStr:string) =
            try
                let byteArray = System.Text.Encoding.ASCII.GetBytes(mvsStr)
                use streamm = new MemoryStream(byteArray)
                use srm = new StreamReader(streamm)
                let nl = System.Environment.NewLine

                let rec proclin st cstr s bd mvl = 
                    if s = "" then 
                        match st with
                        |InMove ->
                            let mv = cstr|>FsChessPgn.MoveUtil.fromSAN bd
                            let nbd = bd|>Board.Push mv
                            Unknown,"",nbd, mv::mvl
                        |InComment(_) |InRAV(_) -> st, "", bd, mvl
                        |Unknown |InNum -> st, "", bd, mvl
                        |Invalid |FinishedOK |FinishedInvalid -> failwith "Invalid state at end of line"
                        |_ ->  Unknown,"",bd,mvl
                    else 
                        let hd = s.[0]
                        let tl = s.[1..]
                        match st with
                        |InComment(cl) -> 
                            if hd='}' && cl=1 then
                                proclin Unknown "" tl bd mvl
                            elif hd='}' then
                                proclin (InComment(cl-1)) "" tl bd mvl
                            elif hd='{' then
                                proclin (InComment(cl+1)) "" tl bd mvl
                            else
                                proclin st "" tl bd mvl
                        |InSingleLineComment ->
                            proclin st "" tl bd mvl
                        |InRAV(cl) -> 
                            if hd=')' && cl=1 then
                                proclin Unknown "" tl bd mvl
                            elif hd=')' then
                                proclin (InRAV(cl-1)) "" tl bd mvl
                            elif hd='(' then
                                proclin (InRAV(cl+1)) "" tl bd mvl
                            else
                                proclin st "" tl bd mvl
                        |InNAG -> 
                            if hd=' ' then
                                proclin Unknown "" tl bd mvl
                            else
                                proclin st "" tl bd mvl
                        |InNum -> 
                            if System.Char.IsNumber(hd) || hd = '.' || hd = ' ' //&& tl.Length>0 && tl.StartsWith(".")
                            then
                                proclin st "" tl bd mvl
                            elif hd='/'||hd='-' then
                                proclin InRes "" tl bd mvl
                            else
                                proclin InMove (hd.ToString()) tl bd mvl
                        |InRes -> 
                            proclin st "" tl bd mvl
                        |Invalid -> 
                            proclin st "" tl bd mvl
                        |InMove -> 
                            if hd=' ' then
                                let mv = cstr|>FsChessPgn.MoveUtil.fromSAN bd
                                let nbd = bd|>Board.Push mv
                                proclin Unknown "" tl nbd (mv::mvl)
                            else
                                proclin st (cstr+hd.ToString()) tl bd mvl
                        |FinishedOK |FinishedInvalid -> st, cstr, bd, mvl
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
                            proclin st cstr ns bd mvl

                let rec getmvs st cstr bd mvl = 
                    let lin = srm.ReadLine()
                    if lin |> isNull then mvl|>List.rev|>List.toArray
                    else 
                        let nst, ncstr, nbd, nmvl = proclin st cstr lin bd mvl
                        if nst = FinishedOK then nmvl|>List.rev|>List.toArray
                        elif nst = FinishedInvalid then [||]
                        else getmvs nst ncstr nbd nmvl

                let mvs = getmvs Unknown "" Board.Start []
                mvs
            with ex ->
                log("Exception: " + ex.Message)
                log("Game number: " + i.ToString())
                log("White: " + cp.Hdrs.[i].White)
                log("Black: " + cp.Hdrs.[i].Black)
                [||]

        cp.Mvss<- cp.MvsStrs|>Array.Parallel.mapi getmvs
        log("Finished encoding moves")
        log("Starting creating index")
        //now need to create index from mvs
        let dct =
            let prnks = 
                [|A2; B2; C2; D2; E2; F2; G2; H2; A7; B7; C7; D7; E7; F7; G7; H7|]
                |>Array.map(Square.Name)
            //keys list of empty squares,values is list of game indexes 
            let dct0 = new System.Collections.Generic.Dictionary<string,int[]>()
            let n_choose_k k = 
                let rec choose lo hi =
                    if hi = 0 then [[]]
                    else
                        [for j=lo to (Array.length prnks)-1 do
                             for ks in choose (j+1) (hi-1) do
                                    yield prnks.[j] :: ks ]
                choose 0 k                           
            let full = 
                [1..16]|>List.map(n_choose_k)|>List.concat
                |>List.map(fun il -> il|>Set.ofList|>Set.toArray|>Array.reduce(+))
            let dct =
                full|>List.iter(fun sql -> dct0.Add(sql,[||]))
                dct0
            dct
        let rec addgm id sql (imvl:Move list) =
            if not imvl.IsEmpty then
                let mv = imvl.Head
                //now check if a pawn move which is not on search board
                //need to also include captures of pawns on starts square
                let pc = mv|>FsChessPgn.Move.MovingPiece
                let sq = mv|>FsChessPgn.Move.From
                let rnk = sq|>Square.ToRank
                let cpc = mv|>FsChessPgn.Move.CapturedPiece
                let sqto = mv|>Move.To
                let rnkto = sqto|>Square.ToRank
                if pc=Piece.WPawn && rnk=Rank2 || pc=Piece.BPawn && rnk=Rank7 then
                    let nsql = (sq|>Square.Name)::sql|>List.sort
                    let nsqlstr = nsql|>List.toArray|>Array.reduce(+)
                    let cvl = dct.[nsqlstr]
                    let nvl = Array.append cvl [|id|]
                    dct.[nsqlstr] <- nvl
                    addgm id nsql imvl.Tail
                elif cpc=Piece.WPawn && rnkto=Rank2 || cpc=Piece.BPawn && rnkto=Rank7 then
                    let nsql = (sqto|>Square.Name)::sql|>List.sort
                    let nsqlstr = nsql|>List.toArray|>Array.reduce(+)
                    let cvl = dct.[nsqlstr]
                    let nvl = Array.append cvl [|id|]
                    dct.[nsqlstr] <- nvl
                    addgm id nsql imvl.Tail
                else
                    addgm id sql imvl.Tail
        let dogm i mvs =
            addgm i [] (mvs|>List.ofArray)
        cp.Mvss|>Array.iteri dogm
        cp.Indx <- dct
        log("Finished creating index")
        cp

    let ToPgn(fn:string, cp:ChessPack, log:string -> unit) = 
        log("Starting saving file " + fn + " to disk")

        use stream = new FileStream(fn, FileMode.Create)
        use writer = new StreamWriter(stream)
        for i = 0 to cp.Hdrs.Length-1 do
            writer.Write(cp.Hdrs.[i].ToString())
            writer.WriteLine(cp.MvsStrs.[i])
            writer.WriteLine()
        log("Finished saving file")
    
    let Save(fn:string, cp:ChessPack, log:string -> unit) = 
        log("Starting saving file " + fn + " to disk")
        fn|>cp.Save
        log("Finished saving file")

    let Load(fn:string) = fn|>ChessPack.Load

    let PgnToFch(pgnfn:string, fchfn:string, log:string -> unit) =
        let cp = FromPgn(pgnfn,log)
        Save(fchfn,cp,log)
