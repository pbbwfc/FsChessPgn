namespace Olm

open FsChess
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

        let rec proclin st cstr s mno isw msel = 
            if s = "" then 
                match st with
                |InMove ->
                    let mte = MvEntry(mno,isw,cstr)
                    let nmsel = mte::msel
                    let nmno = if isw then mno else mno+1
                    let nisw = not isw
                    Unknown,"",nmno,nisw,nmsel
                |InNAG ->
                    let mte = NagEntry(cstr|>int|>Ng)
                    let nmsel = mte::msel
                    Unknown,"",mno,isw,nmsel
                |InSingleLineComment ->
                    let mte = CommEntry(cstr)
                    let nmsel = mte::msel
                    Unknown,"",mno,isw,nmsel
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
                    FinishedOK,"",mno,isw,nmsel
                |InComment(_) |InRAV(_) -> st,cstr+nl,mno,isw,msel 
                |Unknown |InNum -> st,cstr,mno,isw,msel
                |Invalid |FinishedOK |FinishedInvalid -> failwith "Invalid state at end of line"
            else 
                let hd = s.[0]
                let tl = s.[1..]
                match st with
                |InComment(cl) -> 
                    if hd='}' && cl=1 then
                        let mte = CommEntry(cstr)
                        let nmsel = mte::msel
                        proclin Unknown "" tl mno isw nmsel
                    elif hd='}' then
                        proclin (InComment(cl-1)) (cstr+hd.ToString()) tl mno isw msel
                    elif hd='{' then
                        proclin (InComment(cl+1)) (cstr+hd.ToString()) tl mno isw msel
                    else
                        proclin st (cstr+hd.ToString()) tl mno isw msel
                |InSingleLineComment ->
                    proclin st (cstr+hd.ToString()) tl mno isw msel
                |InRAV(cl) -> 
                    if hd=')' && cl=1 then
                        let mselr = GetEntries(cstr)
                        let mte = RvEntry(mselr)
                        let nmsel = mte::msel
                        proclin Unknown "" tl mno isw nmsel
                    elif hd=')' then
                        proclin (InRAV(cl-1)) (cstr+hd.ToString()) tl mno isw msel
                    elif hd='(' then
                        proclin (InRAV(cl+1)) (cstr+hd.ToString()) tl mno isw msel
                    else
                        proclin st (cstr+hd.ToString()) tl mno isw msel
                |InNAG -> 
                    if hd=' ' then
                        let mte = NagEntry(cstr|>int|>Ng)
                        let nmsel = mte::msel
                        proclin Unknown "" tl mno isw nmsel
                    else
                        proclin st (cstr+hd.ToString()) tl mno isw msel
                |InNum -> 
                    if System.Char.IsNumber(hd) || hd = '.' || hd = ' ' //&& tl.Length>0 && tl.StartsWith(".")
                    then
                        proclin st (cstr+hd.ToString()) tl mno isw msel
                    elif hd='/'||hd='-' then
                        proclin InRes (cstr+hd.ToString()) tl mno isw msel
                    else
                        proclin InMove (cstr+hd.ToString()) tl mno isw msel
                |InRes -> 
                    proclin st (cstr+hd.ToString()) tl mno isw msel
                |Invalid -> 
                    proclin st cstr tl mno isw msel
                |InMove -> 
                    if hd=' ' then
                        let mte = MvEntry(mno,isw,cstr)
                        let nmsel = mte::msel
                        let nmno = if isw then mno else mno+1
                        let nisw = not isw
                        proclin Unknown "" tl nmno nisw nmsel
                    else
                        proclin st (cstr+hd.ToString()) tl  mno isw msel
                |FinishedOK |FinishedInvalid -> st,cstr,mno,isw,msel
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
                    proclin st cstr ns mno isw msel
    
        let rec getgm st cstr mno isw msel = 
            let lin = srm.ReadLine()
            if lin |> isNull then msel|>List.rev
            else 
                let nst,ncstr,nmno,nisw,nmsel = proclin st cstr lin mno isw msel
                if nst = FinishedOK then nmsel|>List.rev
                elif nst = FinishedInvalid then []
                else getgm nst ncstr nmno nisw nmsel
    
        let msel = getgm Unknown "" 1 true []
        msel

    
    
    
    
    //TODO need to remove all code from FsChessPgn
    let Set (cp:ChessPack,i:int) =
        let hdr = cp.Hdrs.[i]
        let mvsstr = cp.MvsStrs.[i]
        let gmtxt = hdr.ToString() + mvsstr
        let gm = gmtxt|>FsChessPgn.Games.ReadOneFromString
        gm
