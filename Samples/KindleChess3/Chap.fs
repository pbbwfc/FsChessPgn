namespace KindleChess

open System.IO
//open DotLiquid
open Microsoft.FSharp.Reflection
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

    ///ToVar - converts to create Variations html
    let ToVar fol i (nm : string) =
        let ch = i + 1
        let nl = System.Environment.NewLine
        let gm = get nm fol
        //1. just get moves and ravs
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
        let mtel1 = trim1 gm.MoveText []
        //2. trim off surplus moves
        let rec trim2 (imtel:MoveTextEntry list) omtel =
            let hasnoravs mtel =
                let filt mte =
                    match mte with
                    |RAVEntry(_) -> true
                    |_ -> false
                let rvs = mtel|>List.filter filt
                rvs.Length = 0
            if imtel.IsEmpty then omtel|>List.rev
            elif imtel|>hasnoravs then (imtel.Head::omtel)|>List.rev
            else
                let mte = imtel.Head
                match mte with
                |RAVEntry(mtel) ->
                    let nmtel = trim2 mtel []
                    trim2 imtel.Tail (RAVEntry(nmtel)::omtel)
                |HalfMoveEntry(_) -> trim2 imtel.Tail (mte::omtel)
                |_ -> trim2 imtel.Tail omtel
        let mtel2 = trim2 mtel1 []
        //3. Turn mains into RAVs
        let rec torav (imtel:MoveTextEntry list) ravl omtel =
            if imtel.IsEmpty then omtel
            else
                let mte = imtel.Head
                match mte with
                |RAVEntry(mtel) ->
                    torav imtel.Tail (mte::ravl) omtel
                |HalfMoveEntry(_) -> 
                    if ravl.IsEmpty then torav imtel.Tail ravl (mte::omtel)
                    else torav imtel.Tail [] (RAVEntry(mte::omtel)::ravl)
                |_ -> torav imtel.Tail ravl omtel
        let mtel3 = torav (mtel2|>List.rev) [] []
        //4. Generate html
        let rec tohtm (imtel:MoveTextEntry list) indt idl ostr =
            if imtel.IsEmpty then ostr
            else
                let mte = imtel.Head
                match mte with
                |RAVEntry(mtel) ->
                    let lnk1 = "<a href=\"CH" + ch.ToString() + ".html#"
                    let idx = idl|>List.map(fun i -> i.ToString())|>List.reduce(fun a b -> a + "." + b)
                    let lnk2 = idx + "\">" + idx + "</a> " 
                    let lnk = lnk1 + lnk2
                    let ravstr1 = nl + (indt + "  ") + "<li>" + lnk + (tohtm mtel (indt + "  ") (idl@[1]) "") + "</li>"
                    let ravstr2 = if imtel.Tail.IsEmpty then ravstr1 + nl + indt + "</ul>" else ravstr1
                    let nidl =
                        let rv = idl|>List.rev
                        let ne = rv.Head + 1
                        (ne::rv.Tail)|>List.rev
                    tohtm imtel.Tail indt nidl (ostr+ravstr2)
                |HalfMoveEntry(_) -> 
                    //need to vary this depending on what is next
                    let nimtel = imtel.Tail
                    if nimtel.IsEmpty then tohtm nimtel indt idl (ostr+(mte|>Game.MoveStr))
                    else
                        let nmte = nimtel.Head
                        match nmte with
                        |RAVEntry(_) ->
                            tohtm nimtel indt idl (ostr+(mte|>Game.MoveStr) + nl + indt + "<ul>")
                        |_ -> tohtm nimtel indt idl (ostr+(mte|>Game.MoveStr) + " ")
                |_ -> tohtm imtel.Tail indt idl ostr
  
        let tst = tohtm mtel3 "      " [ch;1] ""
        "  <ul>" + nl + "    <li>" + tst + nl + "    </li>" + nl + "  </ul>"

    /////genh - generates HTML files for chapter
    //let genh tfol hfl isw i ch =
    //    //register types used
    //    let reg ty =
    //        let fields = FSharpType.GetRecordFields(ty)
    //        Template.RegisterSafeType(ty, 
    //                                  [| for f in fields -> f.Name |])
    //    reg typeof<ChapT>
    //    let t =
    //        Path.Combine(tfol, "CH.dotl")
    //        |> File.ReadAllText
    //        |> Template.Parse

    //    let gmlbl1,gmlbl2 =
    //        if ch.Intro="" then "",""
    //        else
    //            let gmdt = ch.Intro|>GmChHdT.FromStr
    //            gmdt.White + " vs. " + gmdt.Black,gmdt.Event + ", " + gmdt.GmDate.Year.ToString()
        
    //    let ostr =
    //        t.Render
    //            (Hash.FromDictionary
    //                 (dict [ "ch", box ch
    //                         "gmlbl1", box gmlbl1 
    //                         "gmlbl2", box gmlbl2 
    //                         "i", box (i + 1)
    //                         "treetxt", box (ch.Lines |> Tree.ToHtm hfl isw i) ]))
        
    //    let ouf = Path.Combine(hfl, "CH" + (i + 1).ToString() + ".html")
    //    File.WriteAllText(ouf, ostr)
    
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
