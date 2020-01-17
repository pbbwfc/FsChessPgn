namespace KindleChess

open System.IO
open DotLiquid
open Microsoft.FSharp.Reflection
open FSharp.Markdown
open FsChess
open FsChess.Pgn

module Chap =
    /////create - creates a chapter
    //let create i nm =
    //    { Name = nm
    //      Intro = ""
    //      Lines = i |> Tree.create }
    
    ///rnm - renames a chapter
    let rnm nm oldnm fol = 
        let old = Path.Combine(fol,oldnm + ".pgn")
        if File.Exists(old) then 
            let nw = Path.Combine(fol,nm + ".pgn")
            File.Copy(old,nw,true)
            File.Delete(old)

    ///get - renames a chapter
    let get nm fol = 
        let fn = Path.Combine(fol,nm + ".pgn")
        let gms = fn|>Games.ReadListFromFile
        gms.Head

    ///save - saves a chapter
    let save nm fol (ch:Game) = 
        let fn = Path.Combine(fol,nm + ".pgn")
        [ch]|>Games.WriteFile fn

    ///del - deletes a chapter
    let del nm fol = 
        let fn = Path.Combine(fol,nm)
        if File.Exists(fn) then File.Delete(fn)

    ///torav - changes game so has rav in all cases - easier to then process
    let rec private torav (imtel:MoveTextEntry list) ravl omtel =
        //need to leave RAV than are subs
        if imtel.IsEmpty then omtel
        else
            let mte = imtel.Head
            match mte with
            |RAVEntry(mtel) ->
                let mte = mtel.Head
                match mte with
                |CommentEntry(str) ->
                    if str.Contains("[sub]") then
                        torav imtel.Tail ravl (mte::omtel)
                    else
                        let nmtel = torav (mtel|>List.rev) [] []
                        torav imtel.Tail (RAVEntry(nmtel)::ravl) omtel
                |_ ->
                    let nmtel = torav (mtel|>List.rev) [] []
                    torav imtel.Tail (RAVEntry(nmtel)::ravl) omtel
            |HalfMoveEntry(_) -> 
                if ravl.IsEmpty then torav imtel.Tail ravl (mte::omtel)
                else torav imtel.Tail [] (RAVEntry(mte::omtel)::ravl)
            |_ -> torav imtel.Tail ravl (mte::omtel)

    ///ToVar - converts to create Variations html
    let ToVar fol i (nm : string) =
        let ch = (i + 1).ToString()
        let nl = System.Environment.NewLine
        let gm = get nm fol
        //1. Turn mains into RAVs
        let mtel1 = torav (gm.MoveText|>List.rev) [] []
        //let tst1 = mtel1|>Game.MovesStr
        //2. just get moves and ravs
        let rec trim1 (imtel:MoveTextEntry list) omtel =
            if imtel.IsEmpty then omtel|>List.rev
            else
                let mte = imtel.Head
                match mte with
                |RAVEntry(mtel) ->
                    let nmtel = trim1 mtel []
                    trim1 imtel.Tail (RAVEntry(nmtel)::omtel)
                |HalfMoveEntry(_) -> trim1 imtel.Tail (mte::omtel)
                |_ -> trim1 imtel.Tail omtel
        let mtel2 = trim1 mtel1 []
        //let tst2 = mtel2|>Game.MovesStr
        //3. trim off surplus moves
        let rec trim2 (imtel:MoveTextEntry list) omtel =
            let hasnoravs mtel =
                let filt mte =
                    match mte with
                    |RAVEntry(_) -> true
                    |_ -> false
                let rvs = mtel|>List.filter filt
                rvs.Length = 0
            if imtel.IsEmpty then omtel|>List.rev
            elif imtel|>hasnoravs then omtel|>List.rev//(imtel.Head::omtel)|>List.rev
            else
                let mte = imtel.Head
                match mte with
                |RAVEntry(mtel) ->
                    let nmtel = trim2 mtel []
                    let nmtel2 = if nmtel.IsEmpty then [mtel.Head] else nmtel
                    trim2 imtel.Tail (RAVEntry(nmtel2)::omtel)
                |HalfMoveEntry(_) -> trim2 imtel.Tail (mte::omtel)
                |_ -> trim2 imtel.Tail omtel
        let mtel3 = trim2 mtel2 []
        //let tst3 = mtel3|>Game.MovesStr
        //4. Generate html
        let mvltostr (mvl:MoveTextEntry list) =
            let mvtostr mv = 
                let str0 = mv|>Game.MoveStr
                if str0.Contains("...") then
                    let bits = str0.Split([|' '|])
                    bits.[bits.Length-1]
                else str0
            if mvl.IsEmpty then ""
            else
                mvl|>List.map mvtostr|>List.reduce(fun a b -> a + " " + b)
        let mvfilt mte =
            match mte with
            |HalfMoveEntry(_) -> true
            |_ -> false
        let ravfilt mte =
            match mte with
            |RAVEntry(_) -> true
            |_ -> false
        let rec ravtohtm indt idstr i (rav:MoveTextEntry) =
            let id = idstr + "." + (i+1).ToString()
            let lnk = "<li><a href=\"CH" + ch + ".html#" + id + "\">" + id + "</a>"
            match rav with
            |RAVEntry(mtel) ->
                let mvl = mtel|>List.filter mvfilt
                let ravl = mtel|>List.filter ravfilt
                let ravstr = 
                    if ravl.IsEmpty then "</li>" + nl
                    else
                        nl
                        + indt + "  <ul>" + nl 
                        + (ravl|>List.mapi(ravtohtm (indt + "    ") id)|>List.reduce(+))
                        + indt + "  </ul>" + nl
                        + indt + "</li>" + nl
                indt + lnk + (mvl|>mvltostr) + ravstr
            |_ -> failwith "must be RAV"

        let mvl = mtel3|>List.filter mvfilt
        let ravl = mtel3 |>List.filter ravfilt
        let ravstr =
            if ravl.IsEmpty then ""
            else 
                "      <ul>" + nl 
                + (ravl|>List.mapi(ravtohtm "        " ch)|>List.reduce(+))
                + "      </ul>" + nl
        let htm =
              "  <ul>" + nl + "    <li>" + (mvl|>mvltostr) + nl
              + ravstr
              + "    </li>" + nl + "  </ul>"

        htm

    ///genh - generates HTML files for chapter
    let genh cfl tfol hfl isw i nm =
        let nl = System.Environment.NewLine
        let ch = (get nm cfl)|>Game.GetaMoves
        let chno = (i+1).ToString()
        let mtel = torav (ch.MoveText|>List.rev) [] []
        let initbd = if ch.BoardSetup.IsSome then ch.BoardSetup.Value else Board.Start
        let link bd dct id =
            let alias = (id + "_" + dct.ToString()).Replace('.', '_')
            let png = alias + ".png"
            // generate image
            let imgfol = Path.Combine(hfl, "board")
            do imgfol
               |> Directory.CreateDirectory
               |> ignore
            let flip = not isw
            do bd|>Board.ToPng (Path.Combine(imgfol, png)) flip
            @"<p class=""centered"">" + @"<img alt=""" + alias 
            + @""" src=""board/" + png 
            + @""" width=""352"" height=""352"" border=""2"" />" + "</p>"
        let dod bd dct id (pd : string) =
            if not (pd.Contains("[diag]")) then pd,dct
            else 
                let pos = pd.IndexOf("[diag]")
                let npd =
                    pd.Substring(0, pos) + (link bd dct id) 
                    + pd.Substring(pos + 6)
                npd,(dct+1)
         
        let ravfilt mte =
            match mte with
            |RAVEntry(_) -> true
            |_ -> false
        let noravfilt mte =
            match mte with
            |RAVEntry(_)|GameEndEntry(_) -> false
            |_ -> true
        let rec getmvs bd dct id (imtel:MoveTextEntry list) inmv ostr =
            if imtel.IsEmpty then (if inmv then ostr + "</strong></p>" else ostr),bd
            else
                let mte = imtel.Head
                match mte with
                |HalfMoveEntry(_,_,_,amv) ->
                    let nbd = amv.Value.PostBrd
                    if inmv then getmvs nbd dct id imtel.Tail true (ostr + " " + (mte|>Game.MoveStr))
                    else getmvs nbd dct id imtel.Tail true (ostr + nl + "<p><strong>" + (mte|>Game.MoveStr))
                |CommentEntry(str) -> 
                    let nstr,ndct = str|>dod bd dct id
                    let htm = nstr|> Markdown.Parse|> Markdown.WriteHtml
                    if inmv then getmvs bd ndct id imtel.Tail false (ostr + "</strong></p>" + nl + htm)
                    else getmvs bd ndct id imtel.Tail false (ostr + htm)
                |_ -> getmvs bd dct id imtel.Tail false (ostr + "</strong></p>")
        
        let rec getravs bd id (iravl:MoveTextEntry list) ostr =
            let rec getfirst (mtel:MoveTextEntry list) =
                if mtel.IsEmpty then failwith "should have one move"
                else
                    let mte = mtel.Head
                    match mte with
                    |HalfMoveEntry(_) -> mte
                    |_ -> getfirst mtel.Tail
            let getravlnk i (rav:MoveTextEntry) =
                match rav with
                |RAVEntry(mtel) ->
                    let mv = getfirst mtel
                    let mvstr = mv|>Game.MoveStr
                    let sec = (i+1).ToString()
                    let lnk = id + "." + sec
                    "<li><a href=\"CH" + chno + ".html#" + lnk + "\">" + lnk + " - " + mvstr + "</a></li>" + nl
                |_ -> failwith "should be RAV"
            
            let getravhd i (rav:MoveTextEntry) =
                match rav with
                |RAVEntry(mtel) ->
                    let mv = getfirst mtel
                    let mvstr = mv|>Game.MoveStr
                    let sec = (i+1).ToString()
                    let lnk = id + "." + sec
                    "<div id=\"" + lnk + "\"></div>" + nl
                     + "<h3>" + lnk + " - " + mvstr + "</h3>" + nl
                |_ -> failwith "should be RAV"
            
            let dorav i (rav:MoveTextEntry) =
                let hdr = getravhd i rav
                match rav with
                |RAVEntry(mtel) ->
                    let sec = (i+1).ToString()
                    let lnk = id + "." + sec
                    let mvl = mtel.Tail|>List.filter noravfilt
                    let ravl = mtel.Tail|>List.filter ravfilt
                    let mvtxt,nbd = getmvs bd 1 lnk mvl false ""
                    let ravtxt = getravs nbd lnk ravl ""
                    hdr + mvtxt + ravtxt + nl

                |_ -> failwith "should be RAV"

            
            if iravl.IsEmpty then ""
            else
                "<ul>" + nl
                + (iravl|>List.mapi getravlnk|>List.reduce(+))
                + "</ul>" + nl
                + (iravl|>List.mapi dorav|>List.reduce(+))

            
            
 
        
        
        let mvl = mtel|>List.filter noravfilt
        let mvtxt,nbd = getmvs initbd 1 chno mvl false ""
        let ravl = mtel|>List.filter ravfilt
        let ravtxt = getravs nbd chno ravl ""

        
        
        let gmtxt = mvtxt + ravtxt
        
        //register types used
        let reg ty =
            let fields = FSharpType.GetRecordFields(ty)
            Template.RegisterSafeType(ty, 
                                      [| for f in fields -> f.Name |])
        reg typeof<Game>
        let t =
            Path.Combine(tfol, "CH.dotl")
            |> File.ReadAllText
            |> Template.Parse

        let ostr =
            t.Render
                (Hash.FromDictionary
                     (dict [ "nm", box nm; "chno", box chno; "gmtxt", box gmtxt ]))
        
        let ouf = Path.Combine(hfl, "CH" + (i + 1).ToString() + ".html")
        File.WriteAllText(ouf, ostr)
    
    
    
    
    
    
    
    
    
    /////delLine - deletes a line
    //let delLine vid (ch : ChapT) =
    //    let tr = Tree.delLine vid ch.Lines
    //    { ch with Lines = tr }
    
    /////addmv - adds a new move to a chapter
    //let addmv (mvs : Move list) (mv : Move) (ch : ChapT) =
    //    let tr, chg, vid = Tree.addmv mvs mv ch.Lines
    //    { ch with Lines = tr }, chg, vid
    
    /////getdsc -gets a description for a move in a chapter
    //let getdsc (vid : string) mct (ch : ChapT) =
    //    let itr = ch.Lines
    //    Tree.getdsc vid mct itr
    
    /////upddsc - updates a description for a move in a chapter
    //let upddsc (vid : string) mct pm (ch : ChapT) =
    //    let itr = ch.Lines
    //    let tr = Tree.upddsc vid mct pm itr
    //    { ch with Lines = tr }
    
    /////setsub - sets a line as sub
    //let setsub (vid : string) (ch : ChapT) =
    //    let itr = ch.Lines
    //    let tr = Tree.setsub vid itr
    //    { ch with Lines = tr }
    
    /////setmain - sets a line as main
    //let setmain (vid : string) (ch : ChapT) =
    //    let itr = ch.Lines
    //    let tr = Tree.setmain vid itr
    //    { ch with Lines = tr }
    
    /////moveup - move up a line
    //let moveup (vid : string) (ch : ChapT) =
    //    let itr = ch.Lines
    //    let tr = Tree.moveup vid itr
    //    { ch with Lines = tr }
    
    /////movedown - move down a line
    //let movedown (vid : string) (ch : ChapT) =
    //    let itr = ch.Lines
    //    let tr = Tree.movedown vid itr
    //    { ch with Lines = tr }
    
    /////setnag - sets NAG for move
    //let setnag nag mct (vid : string) (ch : ChapT) =
    //    let itr = ch.Lines
    //    let tr, ntel = Tree.setnag nag mct vid itr
    //    { ch with Lines = tr }, ntel
