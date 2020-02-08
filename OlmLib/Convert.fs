namespace Olm

open System.IO
open System.Text.RegularExpressions

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


    let MoveToSan (board:Brd) (move:Move) =
        let piece = board.PieceAt.[int(move|>Move.From)]
        let pct = piece|>Piece.ToPieceType
        let fromrank = move|>Move.From|>Square.ToRank
        let fromfile = move|>Move.From|>Square.ToFile
        let pcprom = move|>Move.Promote
        let isprom = pcprom <> Piece.EMPTY
        let ptprom = pcprom|>Piece.ToPieceType
        let sTo = move|>Move.To
        let sFrom = move|>Move.From
    
        let iscap = 
            if (sTo = board.EnPassant && (piece = Piece.WPawn || piece = Piece.BPawn)) then true
            else board.PieceAt.[int(sTo)] <> Piece.EMPTY

        let nbd = board|>Board.Push(move)
        let ischk = nbd|>Board.IsChk
        let ismt = nbd|>MoveGenerate.IsMate

        if piece = Piece.WKing && sFrom = E1 && sTo = G1 then 
            "O-O"
        elif piece = Piece.BKing && sFrom = E8 && sTo = G8 then 
            "O-O"
        elif piece = Piece.WKing && sFrom = E1 && sTo = C1 then 
            "O-O-O"
        elif piece = Piece.BKing && sFrom = E8 && sTo = C8 then 
            "O-O-O"
        else 
            //do not need this check for pawn moves
            let rec getuniqs pu fu ru attl = 
                if List.isEmpty attl then pu, fu, ru
                else 
                    let att = attl.Head
                    if att = sFrom then getuniqs pu fu ru attl.Tail
                    else 
                        let otherpiece = board.PieceAt.[int(att)]
                        if otherpiece = piece then 
                            let npu = false
                            let nru = 
                                if (att|>Square.ToRank) = fromrank then false
                                else ru
                        
                            let nfu = 
                                if (att|>Square.ToFile) = fromfile then false
                                else fu
                        
                            getuniqs npu nfu nru attl.Tail
                        else getuniqs pu fu ru attl.Tail
        
            let pu, fu, ru = 
                if ((piece=Piece.WPawn)||(piece=Piece.BPawn)) then
                    if iscap then false,true,false else true,true,true
                else getuniqs true true true ((board|>Board.AttacksTo sTo (piece|>Piece.PieceToPlayer))|>Bitboard.ToSquares)

            let uf,ur =
                if pu then None,None
                else
                    if fu then Some(fromfile), None
                    elif ru then None,Some(fromrank)
                    else Some(fromfile),Some(fromrank)
            let pcstr = 
                match pct with
                |PieceType.Pawn -> ""
                |PieceType.Knight -> "N"
                |PieceType.Bishop -> "B"
                |PieceType.Rook -> "R"
                |PieceType.Queen -> "Q"
                |PieceType.King -> "K"
                |_ -> ""
            let origf = if uf.IsSome then FILE_NAMES.[int(uf.Value)] else ""
            let origr = if ur.IsSome then RANK_NAMES.[int(ur.Value)] else ""
            let capstr = if iscap then "x" else ""
            let targstr =
                if sTo <> OUTOFBOUNDS then
                    SQUARE_NAMES.[int(sTo)]
                else ""
            let promstr =
                if isprom then
                    match ptprom with
                     |PieceType.Pawn -> ""
                     |PieceType.Knight -> "=N"
                     |PieceType.Bishop -> "=B"
                     |PieceType.Rook -> "=R"
                     |PieceType.Queen -> "=Q"
                     |PieceType.King -> "=K"
                     |_ -> ""
                else ""
            let extrastr =
                if ismt then "#"
                //elif ischk then "++"
                elif ischk then "+"
                else ""

            pcstr + origf + origr + capstr + targstr + promstr + extrastr

    let SanToMove (bd:Brd) (san:string) =
        //Active pattern to parse move string
        let (|SimpleMove|Castle|PawnCapture|AmbiguousFile|AmbiguousRank|Promotion|PromCapture|) s =
            if Regex.IsMatch(s, "^[BNRQK][a-h][1-8]$") then 
                SimpleMove(s.[0]|>PieceType.Parse, s.[1..]|>Square.Parse)
            elif Regex.IsMatch(s, "^[a-h][1-8]$") then SimpleMove(PieceType.Pawn, s|>Square.Parse)
            elif s = "O-O" then Castle('K')
            elif s = "O-O-O" then Castle('Q')
            elif Regex.IsMatch(s, "^[a-h][a-h][1-8]$") then 
                PawnCapture(s.[0]|>File.Parse, s.[1..]|>Square.Parse)
            elif Regex.IsMatch(s, "^[BNRQK][a-h][a-h][1-8]$") then 
                AmbiguousFile(s.[0]|>PieceType.Parse, s.[1]|>File.Parse, s.[2..]|>Square.Parse)
            elif Regex.IsMatch(s, "^[BNRQK][1-8][a-h][1-8]$") then 
                AmbiguousRank(s.[0]|>PieceType.Parse, s.[1]|>Rank.Parse, s.[2..]|>Square.Parse)
            elif Regex.IsMatch(s, "^[a-h][1-8][BNRQ]$") then 
                Promotion(s.[0..1]|>Square.Parse, s.[2]|>PieceType.Parse)
            elif Regex.IsMatch(s, "^[a-h][a-h][1-8][BNRQ]$") then 
                PromCapture(s.[0]|>File.Parse, s.[1..2]|>Square.Parse, s.[3]|>PieceType.Parse)
            else failwith ("invalid move: " + s)

        let strip chars =
            String.collect (fun c -> 
                if Seq.exists ((=) c) chars then ""
                else string c)
          
        let m = san |> strip "+x#="|>fun x ->x.Replace("e.p.", "")
        
        let mv =
            match m with
            | SimpleMove(p, sq) -> 
                match p with
                 |PieceType.Pawn ->
                     let mvs = 
                        bd|>MoveGenerate.PawnMoves
                        |>List.filter(fun mv -> sq=(mv|>Move.To))
                     if mvs.Length=1 then mvs.Head
                     else
                         failwith ("p " + san)
                 |PieceType.Knight ->
                     let mvs = 
                         bd|>MoveGenerate.KnightMoves
                         |>List.filter(fun mv -> sq=(mv|>Move.To))
                     if mvs.Length=1 then mvs.Head
                     else
                         failwith ("n " + san)
                 |PieceType.Bishop ->
                     let mvs = 
                         bd|>MoveGenerate.BishopMoves
                         |>List.filter(fun mv -> sq=(mv|>Move.To))
                     if mvs.Length=1 then mvs.Head
                     else
                         failwith ("b " + san)
                 |PieceType.Rook ->
                     let mvs = 
                         bd|>MoveGenerate.RookMoves
                         |>List.filter(fun mv -> sq=(mv|>Move.To))
                     if mvs.Length=1 then mvs.Head
                     else
                         failwith ("r " + san)
                 |PieceType.Queen ->
                     let mvs = 
                         bd|>MoveGenerate.QueenMoves
                         |>List.filter(fun mv -> sq=(mv|>Move.To))
                     if mvs.Length=1 then mvs.Head
                     else
                         failwith ("q " + san)
                 |PieceType.King ->
                     let mvs = 
                         bd|>MoveGenerate.KingMoves
                         |>List.filter(fun mv -> sq=(mv|>Move.To))
                     if mvs.Length=1 then mvs.Head
                     else
                         failwith ("k " + san)
                 |_ -> failwith ("all " + san)

            | Castle(c) ->
                if c='K' then
                    let mvs = 
                        bd|>MoveGenerate.CastleMoves
                        |>List.filter(fun mv -> FileG=(mv|>Move.To|>Square.ToFile))
                    if mvs.Length=1 then mvs.Head
                    else failwith "kc"
                else
                    let mvs = 
                        bd|>MoveGenerate.CastleMoves
                        |>List.filter(fun mv -> FileC=(mv|>Move.To|>Square.ToFile))
                    if mvs.Length=1 then mvs.Head
                    else failwith "qc"
            | PawnCapture(f, sq) -> 
                let mvs = 
                    bd|>MoveGenerate.PawnMoves
                    |>List.filter(fun mv -> sq=(mv|>Move.To))
                if mvs.Length=1 then mvs.Head
                else
                    let mvs1=mvs|>List.filter(fun mv -> f=(mv|>Move.From|>Square.ToFile))
                    if mvs1.Length=1 then mvs1.Head else failwith ("pf " + san)
            | AmbiguousFile(p, f, sq) -> 
                match p with
                |PieceType.Pawn ->
                    let mvs = 
                        bd|>MoveGenerate.PawnMoves
                        |>List.filter(fun mv -> sq=(mv|>Move.To))
                    if mvs.Length=1 then mvs.Head
                    else
                        let mvs1=mvs|>List.filter(fun mv -> f=(mv|>Move.From|>Square.ToFile))
                        if mvs1.Length=1 then mvs1.Head else failwith("pf " + san)
                |PieceType.Knight ->
                    let mvs = 
                        bd|>MoveGenerate.KnightMoves
                        |>List.filter(fun mv -> sq=(mv|>Move.To))
                    if mvs.Length=1 then mvs.Head
                    else
                        let mvs1=mvs|>List.filter(fun mv -> f=(mv|>Move.From|>Square.ToFile))
                        if mvs1.Length=1 then mvs1.Head else failwith("nf " + san)
                |PieceType.Bishop ->
                    let mvs = 
                        bd|>MoveGenerate.BishopMoves
                        |>List.filter(fun mv -> sq=(mv|>Move.To))
                    if mvs.Length=1 then mvs.Head
                    else
                        let mvs1=mvs|>List.filter(fun mv -> f=(mv|>Move.From|>Square.ToFile))
                        if mvs1.Length=1 then mvs1.Head else failwith("bf " + san)
                |PieceType.Rook ->
                    let mvs = 
                        bd|>MoveGenerate.RookMoves
                        |>List.filter(fun mv -> sq=(mv|>Move.To))
                    if mvs.Length=1 then mvs.Head
                    else
                        let mvs1=mvs|>List.filter(fun mv -> f=(mv|>Move.From|>Square.ToFile))
                        if mvs1.Length=1 then mvs1.Head else failwith("rf " + san)
                |PieceType.Queen ->
                    let mvs = 
                        bd|>MoveGenerate.QueenMoves
                        |>List.filter(fun mv -> sq=(mv|>Move.To))
                    if mvs.Length=1 then mvs.Head
                    else
                        let mvs1=mvs|>List.filter(fun mv -> f=(mv|>Move.From|>Square.ToFile))
                        if mvs1.Length=1 then mvs1.Head else failwith("qf " + san)
                |PieceType.King ->
                    let mvs = 
                        bd|>MoveGenerate.KingMoves
                        |>List.filter(fun mv -> sq=(mv|>Move.To))
                    if mvs.Length=1 then mvs.Head
                    else
                        failwith("k " + san)
                |_ -> failwith "all"

            | AmbiguousRank(p, r, sq) -> 
                match p with
                |PieceType.Pawn ->
                    let mvs = 
                        bd|>MoveGenerate.PawnMoves
                        |>List.filter(fun mv -> sq=(mv|>Move.To))
                    if mvs.Length=1 then mvs.Head
                    else
                        failwith ("p " + san)
                |PieceType.Knight ->
                    let mvs = 
                        bd|>MoveGenerate.KnightMoves
                        |>List.filter(fun mv -> sq=(mv|>Move.To))
                    if mvs.Length=1 then mvs.Head
                    else
                        let mvs1=mvs|>List.filter(fun mv -> r=(mv|>Move.From|>Square.ToRank))
                        if mvs1.Length=1 then mvs1.Head else failwith("nr " + san)
                |PieceType.Bishop ->
                    let mvs = 
                        bd|>MoveGenerate.BishopMoves
                        |>List.filter(fun mv -> sq=(mv|>Move.To))
                    if mvs.Length=1 then mvs.Head
                    else
                        let mvs1=mvs|>List.filter(fun mv -> r=(mv|>Move.From|>Square.ToRank))
                        if mvs1.Length=1 then mvs1.Head else failwith("br " + san)
                |PieceType.Rook ->
                    let mvs = 
                        bd|>MoveGenerate.RookMoves
                        |>List.filter(fun mv -> sq=(mv|>Move.To))
                    if mvs.Length=1 then mvs.Head
                    else
                        let mvs1=mvs|>List.filter(fun mv -> r=(mv|>Move.From|>Square.ToRank))
                        if mvs1.Length=1 then mvs1.Head else failwith("rr " + san)
                |PieceType.Queen ->
                    let mvs = 
                        bd|>MoveGenerate.QueenMoves
                        |>List.filter(fun mv -> sq=(mv|>Move.To))
                    if mvs.Length=1 then mvs.Head
                    else
                        let mvs1=mvs|>List.filter(fun mv -> r=(mv|>Move.From|>Square.ToRank))
                        if mvs1.Length=1 then mvs1.Head else failwith("qr " + san)
                |PieceType.King ->
                    let mvs = 
                        bd|>MoveGenerate.KingMoves
                        |>List.filter(fun mv -> sq=(mv|>Move.To))
                    if mvs.Length=1 then mvs.Head
                    else
                        failwith ("k " + san)
                |_ -> failwith "all"

            | Promotion(sq, p) -> 
                let mvs = 
                    bd|>MoveGenerate.PawnMoves
                    |>List.filter(fun mv -> sq=(mv|>Move.To))
                    |>List.filter(fun mv -> p=(mv|>Move.PromoteType))
                if mvs.Length=1 then mvs.Head
                else
                    failwith ("p " + san)

            | PromCapture(f, sq, p) -> 
                let mvs = 
                    bd|>MoveGenerate.PawnMoves
                    |>List.filter(fun mv -> sq=(mv|>Move.To))
                    |>List.filter(fun mv -> p=(mv|>Move.PromoteType))
                if mvs.Length=1 then mvs.Head
                else
                    let mvs1=mvs|>List.filter(fun mv -> f=(mv|>Move.From|>Square.ToFile))
                    if mvs1.Length=1 then mvs1.Head else failwith ("pf " + san)
      
        mv
    
    
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
                            let mv = cstr|>SanToMove bd
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
                                let mv = cstr|>SanToMove bd
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
                let pc = mv|>Move.MovingPiece
                let sq = mv|>Move.From
                let rnk = sq|>Square.ToRank
                let cpc = mv|>Move.CapturedPiece
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
    
    let PgnToFch(pgnfn:string, fchfn:string, log:string -> unit) =
        let cp = FromPgn(pgnfn,log)
        Pack.Save(fchfn,cp,log)
