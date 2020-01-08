namespace FsChess.WinForms

open System.Windows.Forms
open FsChess

[<AutoOpen>]
module Library2 =

    type WbPgn() as pgn =
        inherit WebBrowser(AllowWebBrowserDrop = false,IsWebBrowserContextMenuEnabled = false,WebBrowserShortcutsEnabled = false)
        //mutables
        let mutable game = Game.Start
        let mutable board = Board.Start
        let mutable oldstyle:(HtmlElement*string) option = None
        let mutable irs = [-1]

        //events
        let bdchngEvt = new Event<_>()
        
        //functions
        let hdr = "<html><body>"
        let ftr = "</body></html>"
        //given a rav id get then list of indexes to locate
        //[2;3;5] indicates go to RAV at index 2, withing this go to RAV at index 3 and then get item at index 5
        let rec getirs ir irl =
            if ir<256 then ir::irl
            else
                let nir = ir >>> 8
                let i = ir &&& 0x3F
                getirs nir (i::irl)
        //get a rav id from a list of indexes to locate
        //[2;,3;5] indicates go to RAV at index 2, withing this go to RAV at index 3 and then get item at index 5
        let rec getir (irl:int list) ir =
            if irl.IsEmpty then ir
            else
                let nir = irl.Head|||(ir<<<8)
                getir irl.Tail nir
        
        let highlight (mve:HtmlElement) =
            if oldstyle.IsSome then
                let omve,ostyle = oldstyle.Value
                omve.Style <- ostyle
            let curr = mve.Style
            oldstyle <- Some(mve,curr)
            mve.Style <- "BACKGROUND-COLOR: powderblue"

        
        let rec mvtag ravno i (mte:MoveTextEntry) =
            let ir = i|||(ravno<<<8)
            let idstr = "id = \"" + ir.ToString() + "\""
            match mte with
            |HalfMoveEntry(_,_,_,_) ->
                let str = mte|>Game.MoveStr
                if ravno=0 then " <span " + idstr + " class=\"mv\" style=\"color:black\">" + str + "</span>"
                else " <span " + idstr + " class=\"mv\" style=\"color:darkslategray\">" + str + "</span>"
            |CommentEntry(_) ->
                let str = (mte|>Game.MoveStr).Trim([|'{';'}'|])
                "<div " + idstr + " class=\"cm\" style=\"color:green\">" + str + "</div>"
            |GameEndEntry(_) ->
                let str = mte|>Game.MoveStr
                " <span " + idstr + " class=\"ge\" style=\"color:blue\">" + str + "</span>"
            |NAGEntry(ng) ->
                let str = ng|>Game.NAGStr
                "<span " + idstr + " class=\"ng\" style=\"color:darkred\">" + str + "</span>"
            |RAVEntry(mtel) ->
                let str = mtel|>List.mapi (mvtag ir)|>List.reduce(+)
                "<div style=\"color:darkslategray\">(" + str + ")</div>"

        let mvtags() = 
            let mt = game.MoveText
            if mt.IsEmpty then hdr+ftr
            else
                hdr +
                (game.MoveText|>List.mapi (mvtag 0)|>List.reduce(+))
                + ftr
        
        let onclick(mve:HtmlElement) = 
            let i = mve.Id|>int
            irs <- getirs i []
            let mv =
                if irs.Length>1 then 
                    let rec getmv (mtel:MoveTextEntry list) (intl:int list) =
                        if intl.Length=1 then mtel.[intl.Head]
                        else
                            let ih = intl.Head
                            let mte = mtel.[ih]
                            match mte with
                            |RAVEntry(nmtel) -> getmv nmtel intl.Tail
                            |_ -> failwith "should be a RAV"
                    getmv game.MoveText irs
                else
                    game.MoveText.[i]
            match mv with
            |HalfMoveEntry(_,_,_,amv) ->
                if amv.IsNone then failwith "should have valid aMove"
                else
                    board <- amv.Value.PostBrd
                    board|>bdchngEvt.Trigger
                    mve|>highlight

            |_ -> failwith "not done yet"
        
        let setclicks e = 
            for el in pgn.Document.GetElementsByTagName("span") do
                if el.GetAttribute("className") = "mv" then
                    el.Click.Add(fun _ -> onclick(el))
            let id = getir irs 0
            for el in pgn.Document.GetElementsByTagName("span") do
                if el.GetAttribute("className") = "mv" then
                    if el.Id=id.ToString() then
                        el|>highlight
        

        do
            pgn.DocumentText <- mvtags()
            pgn.DocumentCompleted.Add(setclicks)
            pgn.ObjectForScripting <- pgn

        member pgn.SetGame(gm:Game) = 
            game <- gm|>Game.GetaMoves
            pgn.DocumentText <- mvtags()
            board <- Board.Start
            oldstyle <- None
            irs <- [-1]
            board|>bdchngEvt.Trigger

        member pgn.NextMove() = 
            let rec getnxt oi ci (mtel:MoveTextEntry list) =
                if ci=mtel.Length then oi
                else
                    let mte = mtel.[ci]
                    match mte with
                    |HalfMoveEntry(_,_,_,amv) ->
                        if amv.IsNone then failwith "should have valid aMove"
                        else
                            board <- amv.Value.PostBrd
                            board|>bdchngEvt.Trigger
                        ci
                    |_ -> getnxt oi (ci+1) mtel
            if irs.Length>1 then 
                let rec getmv (mtel:MoveTextEntry list) (intl:int list) =
                    if intl.Length=1 then
                        let oi = intl.Head
                        let ni = getnxt oi (oi+1) mtel
                        let st = irs|>List.rev|>List.tail|>List.rev
                        irs <- st@[ni]
                    else
                        let ih = intl.Head
                        let mte = mtel.[ih]
                        match mte with
                        |RAVEntry(nmtel) -> getmv nmtel intl.Tail
                        |_ -> failwith "should be a RAV"
                getmv game.MoveText irs
            else
                let ni = getnxt irs.Head (irs.Head+1) game.MoveText
                irs <- [ni]
            //now need to select the element
            let id = getir irs 0
            for el in pgn.Document.GetElementsByTagName("span") do
                if el.GetAttribute("className") = "mv" then
                    if el.Id=id.ToString() then
                        el|>highlight
                        
        member pgn.PrevMove() = 
            let rec getprv oi ci (mtel:MoveTextEntry list) =
                if ci<0 then oi
                else
                    let mte = mtel.[ci]
                    match mte with
                    |HalfMoveEntry(_,_,_,amv) ->
                        if amv.IsNone then failwith "should have valid aMove"
                        else
                            board <- amv.Value.PostBrd
                            board|>bdchngEvt.Trigger
                        ci
                    |_ -> getprv oi (ci-1) mtel
            if irs.Length>1 then 
                let rec getmv (mtel:MoveTextEntry list) (intl:int list) =
                    if intl.Length=1 then
                        let oi = intl.Head
                        let ni = getprv oi (oi-1) mtel
                        let st = irs|>List.rev|>List.tail|>List.rev
                        irs <- st@[ni]
                    else
                        let ih = intl.Head
                        let mte = mtel.[ih]
                        match mte with
                        |RAVEntry(nmtel) -> getmv nmtel intl.Tail
                        |_ -> failwith "should be a RAV"
                getmv game.MoveText irs
            else
                let ni = getprv irs.Head (irs.Head-1) game.MoveText
                irs <- [ni]
            //now need to select the element
            let id = getir irs 0
            for el in pgn.Document.GetElementsByTagName("span") do
                if el.GetAttribute("className") = "mv" then
                    if el.Id=id.ToString() then
                        el|>highlight

        member pgn.DoMove(mv:Move) =
            let rec getnxt oi ci (mtel:MoveTextEntry list) =
                if ci=mtel.Length then ci,false
                else
                    let mte = mtel.[ci]
                    match mte with
                    |HalfMoveEntry(_,_,_,amv) ->
                        if amv.IsNone then failwith "should have valid aMove"
                        elif amv.Value.Mv=mv then
                            board <- amv.Value.PostBrd
                            ci,true
                        else ci,false
                    |_ -> getnxt oi (ci+1) mtel
            let isnxt =
                if irs.Length>1 then 
                    let rec getmv (mtel:MoveTextEntry list) (intl:int list) =
                        if intl.Length=1 then
                            let oi = intl.Head
                            let ni,fnd = getnxt oi (oi+1) mtel
                            if fnd then
                                let st = irs|>List.rev|>List.tail|>List.rev
                                irs <- st@[ni]
                            fnd
                        else
                            let ih = intl.Head
                            let mte = mtel.[ih]
                            match mte with
                            |RAVEntry(nmtel) -> getmv nmtel intl.Tail
                            |_ -> failwith "should be a RAV"
                    getmv game.MoveText irs
                else
                    let ni,fnd = getnxt irs.Head (irs.Head+1) game.MoveText
                    if fnd then irs <- [ni]
                    fnd
            if isnxt then
                //now need to select the element
                let id = getir irs 0
                for el in pgn.Document.GetElementsByTagName("span") do
                    if el.GetAttribute("className") = "mv" then
                        if el.Id=id.ToString() then
                            el|>highlight
                else
                    //Check if first move in RAV
                    let rec inrav oi ci (mtel:MoveTextEntry list) =
                        if ci=mtel.Length then ci,false //Should not hit this as means has no moves
                        else
                            let mte = mtel.[ci]
                            match mte with
                            |HalfMoveEntry(_,_,_,amv) ->
                                if amv.IsNone then failwith "should have valid aMove"
                                elif amv.Value.Mv=mv then
                                    board <- amv.Value.PostBrd
                                    ci,true
                                else ci,false
                            |_ -> inrav oi (ci+1) mtel
                    //next see if moving into RAV
                    let rec getnxtrv oi ci mct (mtel:MoveTextEntry list) =
                        if ci=mtel.Length then ci,0,false //TODO this is an extension to RAV or Moves
                        else
                            let mte = mtel.[ci]
                            if mct = 0 then
                                match mte with
                                |HalfMoveEntry(_,_,_,amv) ->
                                    getnxtrv oi (ci+1) (mct+1) mtel
                                |_ -> getnxtrv oi (ci+1) mct mtel
                            else
                                match mte with
                                |HalfMoveEntry(_,_,_,amv) ->
                                    ci,0,false
                                |RAVEntry(nmtel) ->
                                    //TODO now need to see if first move in rav is mv
                                    let sci,fnd = inrav 0 0 nmtel
                                    if fnd then
                                        ci,sci,fnd
                                    else getnxtrv oi (ci+1) mct mtel
                                |_ -> getnxtrv oi (ci+1) mct mtel
                    let isnxtrv =
                        if irs.Length>1 then 
                            let rec getmv (mtel:MoveTextEntry list) (intl:int list) =
                                if intl.Length=1 then
                                    let oi = intl.Head
                                    let ni,sci,fnd = getnxtrv oi (oi+1) 0 mtel
                                    if fnd then
                                        let st = irs|>List.rev|>List.tail|>List.rev
                                        irs <- st@[ni;sci]
                                    fnd
                                else
                                    let ih = intl.Head
                                    let mte = mtel.[ih]
                                    match mte with
                                    |RAVEntry(nmtel) -> getmv nmtel intl.Tail
                                    |_ -> failwith "should be a RAV"
                            getmv game.MoveText irs
                        else
                            let ni,sci,fnd = getnxtrv irs.Head (irs.Head+1) 0 game.MoveText
                            if fnd then irs <- [ni;sci]
                            fnd
                    if isnxtrv then
                        //now need to select the element
                        let id = getir irs 0
                        for el in pgn.Document.GetElementsByTagName("span") do
                            if el.GetAttribute("className") = "mv" then
                                if el.Id=id.ToString() then
                                    el|>highlight
                        else
                            //TODO - need to create a new RAV
                            let ngame,nirs = Game.AddRav game irs (mv|>Move.TopMove board) 
                            game <- ngame
                            irs <- nirs
                            pgn.DocumentText <- mvtags()



        //publish
        member __.BdChng = bdchngEvt.Publish
