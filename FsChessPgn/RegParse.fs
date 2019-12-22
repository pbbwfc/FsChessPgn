namespace FsChessPgn.Data

open System
open System.Text
open System.IO
open System.Text.RegularExpressions

module RegParse = 
    type private State = 
        | Unknown
        | InHeader
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
    
    let rec private NextGameRdr(sr : StreamReader) = 
        let nl = System.Environment.NewLine
        let rec proclin st cstr s gm = 
            if s = "" then 
                match st with
                |InMove ->
                    let mte = MoveTextEntry.Parse(cstr)
                    let ngm = {gm with MoveText=mte::gm.MoveText}
                    Unknown,"",ngm
                |InNAG ->
                    let mte = NAGEntry(cstr|>int)
                    let ngm = {gm with MoveText=mte::gm.MoveText}
                    Unknown,"",ngm
                |InSingleLineComment ->
                    let mte = CommentEntry(cstr)
                    let ngm = {gm with MoveText=mte::gm.MoveText}
                    Unknown,"",ngm
                |InRes ->
                    let bits = cstr.Split([|'{'|])
                    let ngm =
                        if bits.Length=1 then
                            let mte = GameEndEntry(cstr|>GameResult.Parse)
                            {gm with MoveText=mte::gm.MoveText}
                        else
                            let mte = GameEndEntry(bits.[0].Trim()|>GameResult.Parse)
                            let gm1 = {gm with MoveText=mte::gm.MoveText}
                            let mte1 = CommentEntry(bits.[1].Trim([|'}'|]))
                            {gm1 with MoveText=mte1::gm1.MoveText}
                    FinishedOK,"",ngm
                |InComment(_) |InRAV(_) -> st, cstr+nl, gm
                |Unknown -> st, cstr, gm
                |InHeader |InNum |Invalid |FinishedOK |FinishedInvalid -> failwith "Invalid state at end of line"
            else 
                let hd = s.[0]
                let tl = s.[1..]
                match st with
                |InComment(cl) -> 
                    if hd='}' && cl=1 then
                        let mte = CommentEntry(cstr)
                        let ngm = {gm with MoveText=mte::gm.MoveText}
                        proclin Unknown "" tl ngm
                    elif hd='}' then
                        proclin (InComment(cl-1)) (cstr+hd.ToString()) tl gm
                    elif hd='{' then
                        proclin (InComment(cl+1)) (cstr+hd.ToString()) tl gm
                    else
                        proclin st (cstr+hd.ToString()) tl gm
                |InSingleLineComment ->
                    proclin st (cstr+hd.ToString()) tl gm
                |InRAV(cl) -> 
                    if hd=')' && cl=1 then
                        let byteArray = Encoding.ASCII.GetBytes(cstr)
                        let stream = new MemoryStream(byteArray)
                        let nsr = new StreamReader(stream)
                        let gmr = NextGameRdr(nsr)
                        let mte = RAVEntry(gmr.MoveText)
                        let ngm = {gm with MoveText=mte::gm.MoveText}
                        proclin Unknown "" tl ngm
                    elif hd=')' then
                        proclin (InRAV(cl-1)) (cstr+hd.ToString()) tl gm
                    elif hd='(' then
                        proclin (InRAV(cl+1)) (cstr+hd.ToString()) tl gm
                    else
                        proclin st (cstr+hd.ToString()) tl gm
                |InNAG -> 
                    if hd=' ' then
                        let mte = NAGEntry(cstr|>int)
                        let ngm = {gm with MoveText=mte::gm.MoveText}
                        proclin Unknown "" tl ngm
                    else
                        proclin st (cstr+hd.ToString()) tl gm
                |InNum -> 
                    if hd=' ' then
                        proclin InMove (cstr+hd.ToString()) tl gm
                    elif hd='/'||hd='-' then
                        proclin InRes (cstr+hd.ToString()) tl gm
                    else
                        proclin st (cstr+hd.ToString()) tl gm
                |InRes -> 
                    proclin st (cstr+hd.ToString()) tl gm
                |Invalid -> 
                    proclin st cstr tl gm
                |InHeader -> 
                    if hd=']' then
                        let ngm = gm|>Game.AddTag cstr
                        proclin Unknown "" tl ngm
                    else
                        proclin st (cstr+hd.ToString()) tl gm
                |InMove -> 
                    if hd=' ' then
                        let mte = MoveTextEntry.Parse(cstr)
                        let ngm = {gm with MoveText=mte::gm.MoveText}
                        proclin Unknown "" tl ngm
                    else
                        proclin st (cstr+hd.ToString()) tl gm
                |FinishedOK |FinishedInvalid -> st, cstr, gm
                |Unknown -> 
                    let st, ns = 
                        match hd with
                        | '[' -> InHeader, s.[1..]
                        | '{' -> InComment(1), s.[1..]
                        | '(' -> InRAV(1), s.[1..]
                        | '$' -> InNAG, s.[1..]
                        | '*' -> InRes, s
                        | ';' -> InSingleLineComment, s.[1..]
                        | c when System.Char.IsNumber(c) || c = '.' -> InNum, s
                        | ' ' -> Unknown, s.[1..]
                        | _ -> InMove, s
                    proclin st cstr ns gm
    
        let rec getgm st cstr gm = 
            let lin = sr.ReadLine()
            if lin |> isNull then { gm with MoveText = (gm.MoveText |> List.rev) }
            else 
                let nst, ncstr, ngm = proclin st "" lin gm
                if nst = FinishedOK then { ngm with MoveText = (ngm.MoveText |> List.rev) }
                elif nst = FinishedInvalid then GameEMP
                else getgm nst ncstr ngm
    
        let gm = getgm Unknown "" GameEMP
        gm
    
    let private AllGamesRdr(sr : System.IO.StreamReader) = 
        seq { 
            while not sr.EndOfStream do
                let gm = NextGameRdr(sr)
                yield gm
        }
    
    let private ReadFromStream(stream : Stream) = 
        let sr = new StreamReader(stream)
        let db = AllGamesRdr(sr)
        db
    
    let ReadFromFile(file : string) = 
        let stream = new FileStream(file, FileMode.Open)
        let result = ReadFromStream(stream) |> Seq.toList
        stream.Close()
        result

    let ReadFromString(str : string) = 
        let byteArray = Encoding.ASCII.GetBytes(str)
        let stream = new MemoryStream(byteArray)
        let result = ReadFromStream(stream) |> Seq.toList
        stream.Close()
        result

    let ReadOneFromString(str : string) = 
        let gms = str|>ReadFromString
        gms.Head

