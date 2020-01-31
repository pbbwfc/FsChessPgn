namespace Storage

//open FsChessPgn
open FsChess
open System.IO

module T1 =

    type Hdr =
        {
            Num : int
            White : string
            W_Elo : string
            Black : string
            B_Elo : string
            Result : string
            Date : string
            Event : string
            Round : string
            Site : string
        }
        member x.ToArr() = [|x.White;x.W_Elo;x.Black;x.B_Elo;x.Result;x.Date;x.Event;x.Round;x.Site|]
        static member FromArr (i:int)(a:string[]) = {Num=i;White=a.[0];W_Elo=a.[1];Black=a.[2];B_Elo=a.[3];Result=a.[4];Date=a.[5];Event=a.[6];Round=a.[7];Site=a.[8]}
        override x.ToString() =
            let nl = System.Environment.NewLine
            "[Event \"" + x.Event + "\"]" + nl +
            "[Site \"" + x.Site + "\"]" + nl +
            "[Date \"" + x.Date + "\"]" + nl +
            "[Round \"" + x.Round + "\"]" + nl +
            "[White \"" + x.White + "\"]" + nl +
            "[Black \"" + x.Black + "\"]" + nl +
            "[WhiteElo \"" + x.W_Elo + "\"]" + nl +
            "[BlackElo \"" + x.B_Elo + "\"]" + nl +
            "[Result \"" + x.Result + "\"]" + nl

    type Mvs = Move[]
    type BinPgn1 =
        {
            mutable Hdrs:Hdr[]
            mutable MvsStrs:string[]
            mutable Mvss:Mvs[]
            mutable Indx:System.Collections.Generic.Dictionary<Set<Square>,int list>
        }

    let mutable CurPgn = {Hdrs=[||];MvsStrs=[||];Mvss=[||];Indx=System.Collections.Generic.Dictionary<Set<Square>,int list>()}
    
    //idea is to load pgn text file as array of Hdr plus array of MvsStr
    //then parse as needed to change the MvsStr as MoveText
    //Hdrs and this can use to create a normal pgn
    //the also create Mvs for quick processing
    //Then new binaries are Hdrs,MvsStr and Mvs
    //Never need to laod the pgn text file again unless it is externally edited

    //load pgn into Hdrs and MvsStr arrays
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

    let ReadFromPgn(file : string) =
        use stream = new FileStream(file, FileMode.Open)
        use sr = new StreamReader(stream)
        let nl = System.Environment.NewLine
        let hdremp i =
            {Num=i;White="?";W_Elo="-";Black="?";B_Elo="-";Result="*";Date="???.??.??";Event="?";Round="?";Site="?"}

        let addtag (tagstr:string) (ihdr:Hdr) =
            let k,v = tagstr.Trim([|'[';']'|]).Split([|'"'|])|>Array.map(fun s -> s.Trim())|>fun a -> a.[0],a.[1].Trim('"')
            match k with
            | "Event" -> {ihdr with Event = v},true
            | "Site" -> {ihdr with Site = v},true
            | "Date" -> {ihdr with Date = v},true
            | "Round" -> {ihdr with Round = v},true
            | "White" -> {ihdr with White = v},true
            | "Black" -> {ihdr with Black = v},true
            | "Result" -> {ihdr with Result = v},true
            | "WhiteElo" -> {ihdr with W_Elo = v},true
            | "BlackElo" -> {ihdr with B_Elo = v},true
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
        CurPgn.Hdrs <- hdrs|>List.rev|>List.toArray
        CurPgn.MvsStrs <- mvstrs|>List.rev|>List.toArray
        //now need to extract the Mvs form each MvsStr
        let getmvs(mvsStr:string) =
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
        CurPgn.Mvss<- CurPgn.MvsStrs|>Array.map getmvs
        //now need to create index from mvs
        let dct =
            let prnks = [|A2; B2; C2; D2; E2; F2; G2; H2; A7; B7; C7; D7; E7; F7; G7; H7|]
            //keys list of empty squares,values is list of game indexes 
            let dct0 = new System.Collections.Generic.Dictionary<Set<Square>,int list>()
            let n_choose_k k = 
                let rec choose lo hi =
                    if hi = 0 then [[]]
                    else
                        [for j=lo to (Array.length prnks)-1 do
                             for ks in choose (j+1) (hi-1) do
                                    yield prnks.[j] :: ks ]
                choose 0 k                           
            let full = [1..16]|>List.map(n_choose_k)|>List.concat
            let dct =
                full|>List.iter(fun sql -> dct0.Add(sql|>Set.ofList,[]))
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
                    let nsql = sq::sql
                    let cvl = dct.[nsql|>Set.ofList]
                    let nvl = id::cvl
                    dct.[nsql|>Set.ofList] <- nvl
                    addgm id nsql imvl.Tail
                elif cpc=Piece.WPawn && rnkto=Rank2 || cpc=Piece.BPawn && rnkto=Rank7 then
                        let nsql = sqto::sql
                        let cvl = dct.[nsql|>Set.ofList]
                        let nvl = id::cvl
                        dct.[nsql|>Set.ofList] <- nvl
                        addgm id nsql imvl.Tail
                else
                    addgm id sql imvl.Tail
        let dogm i mvs =
            addgm i [] (mvs|>List.ofArray)
        CurPgn.Mvss|>Array.iteri dogm
        CurPgn.Indx <- dct

    let ExportPgn(fn:string) = 
        use stream = new FileStream(fn, FileMode.Create)
        use writer = new StreamWriter(stream)
        for i = 0 to CurPgn.Hdrs.Length-1 do
            writer.Write(CurPgn.Hdrs.[i].ToString())
            writer.WriteLine(CurPgn.MvsStrs.[i])
            writer.WriteLine()
    
    let Save(fn:string) = 
        let formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
        let hdrsfn = fn + ".hdrs"
        let stream = new FileStream(hdrsfn, FileMode.Create, FileAccess.Write, FileShare.None)
        let hdrsarr = CurPgn.Hdrs|>Array.map(fun h -> h.ToArr())
        formatter.Serialize(stream, hdrsarr)
        stream.Close()
        let mvsstrsfn = fn + ".mvsstrs"
        let stream = new FileStream(mvsstrsfn, FileMode.Create, FileAccess.Write, FileShare.None)
        formatter.Serialize(stream, CurPgn.MvsStrs)
        stream.Close()
        let mvssfn = fn + ".mvss"
        let stream = new FileStream(mvssfn, FileMode.Create, FileAccess.Write, FileShare.None)
        formatter.Serialize(stream, CurPgn.Mvss)
        stream.Close()
        let indxfn = fn + ".indx"
        let stream = new FileStream(indxfn, FileMode.Create, FileAccess.Write, FileShare.None)
        formatter.Serialize(stream, CurPgn.Indx)
        stream.Close()

    let Load(fn:string) =
        let formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
        let hdrsfn = fn + ".hdrs"
        if File.Exists(hdrsfn) then
            let stream = new FileStream(hdrsfn, FileMode.Open, FileAccess.Read, FileShare.Read)
            let hdrsarr:string[][] = formatter.Deserialize(stream):?>string[][]
            stream.Close()
            CurPgn.Hdrs <- hdrsarr|>Array.mapi Hdr.FromArr
        else failwith "hdrs missing"
        let mvsstrsfn = fn + ".mvsstrs"
        if File.Exists(mvsstrsfn) then
            let stream = new FileStream(mvsstrsfn, FileMode.Open, FileAccess.Read, FileShare.Read)
            CurPgn.MvsStrs <- formatter.Deserialize(stream):?>string[]
            stream.Close()
        else failwith "mvsstrs missing"
        let mvssfn = fn + ".mvss"
        if File.Exists(mvssfn) then
            let stream = new FileStream(mvssfn, FileMode.Open, FileAccess.Read, FileShare.Read)
            CurPgn.Mvss <- formatter.Deserialize(stream):?>Mvs[]
            stream.Close()
        else failwith "mvss missing"
        let indxfn = fn + ".indx"
        if File.Exists(indxfn) then
            let stream = new FileStream(indxfn, FileMode.Open, FileAccess.Read, FileShare.Read)
            CurPgn.Indx <- formatter.Deserialize(stream):?>System.Collections.Generic.Dictionary<Set<Square>,int list>
            stream.Close()
        else failwith "indx missing"
