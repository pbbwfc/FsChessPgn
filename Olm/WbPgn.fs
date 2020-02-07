﻿namespace Olm

open System.Windows.Forms
open System.Drawing
open FsChess

[<AutoOpen>]
module Library2 =

    type WbPgn() as pgn =
        inherit WebBrowser(AllowWebBrowserDrop = false,IsWebBrowserContextMenuEnabled = false,WebBrowserShortcutsEnabled = false)
        
        //mutables
        let mutable game = Game.Start
        let mutable msel:MvStrEntry list = []
        let mutable chdr:Hdr = new Hdr()
        let mutable cmvs:Move[] = [||]
        let mutable board = Board.Start
        let mutable oldstyle:(HtmlElement*string) option = None
        let mutable irs = [-1]
        let mutable rirs = [-1]
        let mutable ccm = ""
        let mutable cng = NAG.Null

        //events
        let bdchngEvt = new Event<_>()
        let gmchngEvt = new Event<_>()
        let hdrchngEvt = new Event<_>()
        
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
        let getir (iirl:int list) =
            let rec dogetir (irl:int list) ir =
                if irl.IsEmpty then ir
                else
                    let nir = irl.Head|||(ir<<<8)
                    dogetir irl.Tail nir
            dogetir iirl 0
        
        let highlight (mve:HtmlElement) =
            if oldstyle.IsSome then
                let omve,ostyle = oldstyle.Value
                omve.Style <- ostyle
            let curr = mve.Style
            oldstyle <- Some(mve,curr)
            mve.Style <- "BACKGROUND-COLOR: powderblue"
        
        let rec mvtag ravno i (mte:MvStrEntry) =
            let ir = i|||(ravno<<<8)
            let idstr = "id = \"" + ir.ToString() + "\""
            let writer = new System.IO.StringWriter()
            match mte with
            |MvEntry(_,_,_) ->
                let str = mte|>Olm.Game.MoveStr writer
                if ravno=0 then " <span " + idstr + " class=\"mv\" style=\"color:black\">" + str + "</span>"
                else " <span " + idstr + " class=\"mv\" style=\"color:darkslategray\">" + str + "</span>"
            |CommEntry(_) ->
                let str = (mte|>Olm.Game.MoveStr writer).Trim([|'{';'}'|])
                "<div " + idstr + " class=\"cm\" style=\"color:green\">" + str + "</div>"
            |EndEntry(_) ->
                let str = mte|>Olm.Game.MoveStr writer
                " <span " + idstr + " class=\"ge\" style=\"color:blue\">" + str + "</span>"
            |NagEntry(ng) ->
                let str = mte|>Olm.Game.MoveStr writer
                "<span " + idstr + " class=\"ng\" style=\"color:darkred\">" + str + "</span>"
            |RvEntry(mtel) ->
                let indent = 
                    let rirs = irs|>getirs ir
                    let ind = rirs.Length * 2
                    ";margin-left: " + ind.ToString() + "px"
                let str = mtel|>List.mapi (mvtag ir)|>List.reduce(+)
                "<div style=\"color:darkslategray" + indent + "\">(" + str + ")</div>"

        let mvtags() = 
            if msel.IsEmpty then hdr+ftr
            else
                hdr +
                (msel|>List.mapi (mvtag 0)|>List.reduce(+))
                + ftr
        
        //dialogs
        let dlgcomm(offset,cm) = 
            let txt = if offset= -1 then "Add Comment Before" elif offset=1 then "Add Comment After" else "Edit Comment"
            let dlg = new Form(Text = txt, Height = 400, Width = 400, FormBorderStyle = FormBorderStyle.FixedToolWindow,StartPosition=FormStartPosition.CenterParent)
            let hc2 =
                new FlowLayoutPanel(FlowDirection = FlowDirection.RightToLeft, 
                                    Height = 30, Width = 400,Dock=DockStyle.Bottom)
            let okbtn = new Button(Text = "OK")
            let cnbtn = new Button(Text = "Cancel")
            //if edit need to load content
            let comm =
                new TextBox(Text = cm, Dock = DockStyle.Fill, 
                            Multiline = true, 
                            Font = new Font("Microsoft Sans Serif", 10.0f))
            let dook(e) = 
                //write comm.Text to comment
                if offset = -1 then
                    game <- Game.CommentBefore game rirs comm.Text
                    pgn.DocumentText <- mvtags()
                elif offset = 1 then 
                    game <- Game.CommentAfter game rirs comm.Text
                    pgn.DocumentText <- mvtags()
                else 
                    game <- Game.EditComment game rirs comm.Text
                    pgn.DocumentText <- mvtags()

                game|>gmchngEvt.Trigger
                dlg.Close()

            do 
                dlg.MaximizeBox <- false
                dlg.MinimizeBox <- false
                dlg.ShowInTaskbar <- false
                dlg.StartPosition <- FormStartPosition.CenterParent
                hc2.Controls.Add(cnbtn)
                hc2.Controls.Add(okbtn)
                dlg.Controls.Add(hc2)
                dlg.Controls.Add(comm)
                dlg.CancelButton <- cnbtn
                //events
                cnbtn.Click.Add(fun _ -> dlg.Close())
                okbtn.Click.Add(dook)

            dlg
       
        let dlgnag(offset,ing:NAG) = 
            let txt = if offset=1 then "Add NAG" else "Edit NAG"
            let dlg = new Form(Text = txt, Height = 300, Width = 400, FormBorderStyle = FormBorderStyle.FixedToolWindow,StartPosition=FormStartPosition.CenterParent)
            let hc2 =
                new FlowLayoutPanel(FlowDirection = FlowDirection.RightToLeft, 
                                    Height = 30, Width = 400,Dock=DockStyle.Bottom)
            let okbtn = new Button(Text = "OK")
            let cnbtn = new Button(Text = "Cancel")
            //if edit need to load NAG
            let tc = 
                new TableLayoutPanel(ColumnCount = 2, RowCount = 6, 
                                    Height = 260, Width = 200,Dock=DockStyle.Fill)
            let nags =
                //need to add radio buttons for each possible NAG
                let rbs = 
                   Game.NAGlist|>List.toArray
                   |>Array.map(fun ng -> (ng|>Game.NAGStr) + "   " + (ng|>Game.NAGDesc),ng)
                   |>Array.map(fun (lb,ng) -> new RadioButton(Text=lb,Width=200,Checked=(ng=ing)))
                rbs

            let dook(e) = 
                //get selected nag
                let indx = nags|>Array.findIndex(fun rb -> rb.Checked)
                let selNag = Game.NAGlist.[indx]
                //write nag to NAGEntry
                if offset = 1 && indx<>0 then 
                    game <- Game.AddNag game rirs selNag
                    pgn.DocumentText <- mvtags()
                elif offset = 0 && indx<>0 then 
                    game <- Game.EditNag game rirs selNag
                    pgn.DocumentText <- mvtags()
                else 
                    game <- Game.DeleteNag game rirs
                    pgn.DocumentText <- mvtags()

                game|>gmchngEvt.Trigger
                dlg.Close()

            do 
                dlg.MaximizeBox <- false
                dlg.MinimizeBox <- false
                dlg.ShowInTaskbar <- false
                dlg.StartPosition <- FormStartPosition.CenterParent
                hc2.Controls.Add(cnbtn)
                hc2.Controls.Add(okbtn)
                dlg.Controls.Add(hc2)
                nags.[0..6]|>Array.iteri(fun i rb -> tc.Controls.Add(rb,0,i))
                nags.[7..13]|>Array.iteri(fun i rb -> tc.Controls.Add(rb,1,i))
                dlg.Controls.Add(tc)
                dlg.CancelButton <- cnbtn
                //events
                cnbtn.Click.Add(fun _ -> dlg.Close())
                okbtn.Click.Add(dook)

            dlg

        let dlghdr() = 
            let dlg = new Form(Text = "Edit Headers", Height = 370, Width = 370, FormBorderStyle = FormBorderStyle.FixedToolWindow,StartPosition=FormStartPosition.CenterParent)
            let hc2 =
                new FlowLayoutPanel(FlowDirection = FlowDirection.RightToLeft, 
                                    Height = 30, Width = 400,Dock=DockStyle.Bottom)
            let okbtn = new Button(Text = "OK")
            let cnbtn = new Button(Text = "Cancel")
            //if edit need to load NAG
            let tc = 
                new TableLayoutPanel(ColumnCount = 2, RowCount = 9, 
                                    Height = 350, Width = 360,Dock=DockStyle.Fill)
            let wlbl = new Label(Text="White")
            let wtb = new TextBox(Text=game.WhitePlayer,Width=200)
            let welbl = new Label(Text="White Elo")
            let wetb = new TextBox(Text=game.WhiteElo,Width=200)
            let blbl = new Label(Text="Black")
            let btb = new TextBox(Text=game.BlackPlayer,Width=200)
            let belbl = new Label(Text="Black Elo")
            let betb = new TextBox(Text=game.BlackElo,Width=200)
            let rslbl = new Label(Text="Result")
            let rscb = new ComboBox(Text=(game.Result|>Result.ToStr),Width=200)
            let dtlbl = new Label(Text="Date")
            let dttb = new TextBox(Text=(game|>GameDate.ToStr),Width=200)
            let evlbl = new Label(Text="Event")
            let evtb = new TextBox(Text=game.Event,Width=200)
            let rdlbl = new Label(Text="Round")
            let rdtb = new TextBox(Text=game.Round,Width=200)
            let stlbl = new Label(Text="Site")
            let sttb = new TextBox(Text=game.Site,Width=200)

            
            let dook(e) = 
                let results = [|GameResult.WhiteWins;GameResult.BlackWins;GameResult.Draw;GameResult.Open|]
                let res = if rscb.SelectedIndex= -1 then game.Result else results.[rscb.SelectedIndex]
                let yo,mo,dyo =
                    let bits=dttb.Text.Split([|'.'|])
                    if bits.Length=3 then
                        let tryToInt (s:string) = 
                            match System.Int32.TryParse s with
                            | true, v -> Some v
                            | false, _ -> None
                        
                        bits.[0]|>tryToInt,bits.[1]|>tryToInt,bits.[2]|>tryToInt
                    else None,None,None
                game <- {game with WhitePlayer=wtb.Text;WhiteElo=wetb.Text;
                                   BlackPlayer=btb.Text;BlackElo=betb.Text;
                                   Result=res;Year=yo;Month=mo;Day=dyo;
                                   Event=evtb.Text;Round=rdtb.Text;Site=sttb.Text}
                game|>gmchngEvt.Trigger
                game|>hdrchngEvt.Trigger
                dlg.Close()


            do 
                dlg.MaximizeBox <- false
                dlg.MinimizeBox <- false
                dlg.ShowInTaskbar <- false
                dlg.StartPosition <- FormStartPosition.CenterParent
                hc2.Controls.Add(cnbtn)
                hc2.Controls.Add(okbtn)
                dlg.Controls.Add(hc2)
                tc.Controls.Add(wlbl,0,0)
                tc.Controls.Add(wtb,1,0)
                tc.Controls.Add(welbl,0,1)
                tc.Controls.Add(wetb,1,1)
                tc.Controls.Add(blbl,0,2)
                tc.Controls.Add(btb,1,2)
                tc.Controls.Add(belbl,0,3)
                tc.Controls.Add(betb,1,3)
                tc.Controls.Add(rslbl,0,4)
                [|GameResult.WhiteWins;GameResult.BlackWins;GameResult.Draw;GameResult.Open|]
                |>Array.map(Result.ToStr)
                |>Array.iter(fun r -> rscb.Items.Add(r)|>ignore)
                tc.Controls.Add(rscb,1,4)
                tc.Controls.Add(dtlbl,0,5)
                tc.Controls.Add(dttb,1,5)
                tc.Controls.Add(evlbl,0,6)
                tc.Controls.Add(evtb,1,6)
                tc.Controls.Add(rdlbl,0,7)
                tc.Controls.Add(rdtb,1,7)
                tc.Controls.Add(stlbl,0,8)
                tc.Controls.Add(sttb,1,8)

                dlg.Controls.Add(tc)
                dlg.CancelButton <- cnbtn
                //events
                cnbtn.Click.Add(fun _ -> dlg.Close())
                okbtn.Click.Add(dook)

            dlg

        
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
        
        let mvctxmnu = 
            let m = new ContextMenuStrip()
            //do edit comm before
            let adb =
                new ToolStripMenuItem(Text = "Add Comment Before")
            adb.Click.Add(fun _ -> dlgcomm(-1,"").ShowDialog() |> ignore)
            m.Items.Add(adb) |> ignore
            //do edit comm after
            let ada =
                new ToolStripMenuItem(Text = "Add Comment After")
            ada.Click.Add(fun _ -> dlgcomm(1,"").ShowDialog() |> ignore)
            m.Items.Add(ada) |> ignore
            //do add nag 
            let nag =
                new ToolStripMenuItem(Text = "Add NAG")
            nag.Click.Add(fun _ -> dlgnag(1,NAG.Null).ShowDialog() |> ignore)
            m.Items.Add(nag) |> ignore
            //do edit hdrs
            let hdr =
                new ToolStripMenuItem(Text = "Edit Game Headers")
            hdr.Click.Add(fun _ -> dlghdr().ShowDialog() |> ignore)
            m.Items.Add(hdr) |> ignore
            m

        let cmctxmnu = 
            let m = new ContextMenuStrip()
            //do edit comm 
            let ed =
                new ToolStripMenuItem(Text = "Edit Comment")
            ed.Click.Add(fun _ -> dlgcomm(0,ccm).ShowDialog() |> ignore)
            m.Items.Add(ed) |> ignore
            m

        let ngctxmnu = 
            let m = new ContextMenuStrip()
            //do edit comm 
            let ed =
                new ToolStripMenuItem(Text = "Edit NAG")
            ed.Click.Add(fun _ -> dlgnag(0,cng).ShowDialog() |> ignore)
            m.Items.Add(ed) |> ignore
            m

        let onrightclick(el:HtmlElement,psn) = 
            rirs <- getirs (el.Id|>int) []
            if el.GetAttribute("className") = "mv" then mvctxmnu.Show(pgn,psn)
            elif el.GetAttribute("className") = "cm" then 
                ccm <- el.InnerText
                cmctxmnu.Show(pgn,psn)
            elif el.GetAttribute("className") = "ng" then 
                cng <- el.InnerText|>Game.NAGFromStr
                ngctxmnu.Show(pgn,psn)


        let setclicks e = 
            for el in pgn.Document.GetElementsByTagName("span") do
                if el.GetAttribute("className") = "mv" then
                    el.MouseDown.Add(fun e -> if e.MouseButtonsPressed=MouseButtons.Left then onclick(el) else onrightclick(el,e.MousePosition))
                elif el.GetAttribute("className") = "ng" then
                    el.MouseDown.Add(fun e -> if e.MouseButtonsPressed=MouseButtons.Left then () else onrightclick(el,e.MousePosition))
            for el in pgn.Document.GetElementsByTagName("div") do
                if el.GetAttribute("className") = "cm" then 
                    el.MouseDown.Add(fun e -> if e.MouseButtonsPressed=MouseButtons.Left then () else onrightclick(el,e.MousePosition))

            let id = getir irs
            for el in pgn.Document.GetElementsByTagName("span") do
                if el.GetAttribute("className") = "mv" then
                    if el.Id=id.ToString() then
                        el|>highlight
        

        do
            pgn.DocumentText <- mvtags()
            pgn.DocumentCompleted.Add(setclicks)
            pgn.ObjectForScripting <- pgn

        ///Gets the Game that is displayed
        member pgn.GetGame() = 
            game

        ///Switches to another game with the same position
        member pgn.SwitchGame(gm:Game,hdr:Hdr,mvstr:string,mvs:Move[]) = 
            chdr <- hdr
            cmvs <- mvs
            //parse string into entries
            msel <- mvstr|>Game.GetEntries
            game <- gm|>Game.GetaMoves
            pgn.DocumentText <- mvtags()
            //need to select move that matches current board
            let rec getnxt cbd ci (mtel:MvStrEntry list) =
                if mtel.IsEmpty then -1
                else
                    let mte = mtel.Head
                    match mte with
                    |MvEntry(_,_,_) ->
                        let mv = cmvs.[ci]
                        let nbd = cbd|>Board.Push mv
                        if nbd=board then ci
                        else getnxt nbd (ci+1) mtel.Tail
                    |_ -> getnxt cbd (ci+1) mtel.Tail
            let ni = getnxt Board.Start 0 msel
            irs <- [ni]
            //now need to select the element
            let id = getir irs
            for el in pgn.Document.GetElementsByTagName("span") do
                if el.GetAttribute("className") = "mv" then
                    if el.Id=id.ToString() then
                        el|>highlight

 
        ///Sets the Game to be displayed
        member pgn.SetGame(gm:Game) = 
            game <- gm|>Game.GetaMoves
            pgn.DocumentText <- mvtags()
            board <- Board.Start
            oldstyle <- None
            irs <- [-1]
            board|>bdchngEvt.Trigger

        ///Goes to the next Move in the Game
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
            let id = getir irs
            for el in pgn.Document.GetElementsByTagName("span") do
                if el.GetAttribute("className") = "mv" then
                    if el.Id=id.ToString() then
                        el|>highlight
        
        ///Goes to the last Move in the Variation
        member pgn.LastMove() = 
            let rec gofwd lirs =
                pgn.NextMove()
                if irs<>lirs then gofwd irs
            gofwd irs
        
        ///Goes to the previous Move in the Game
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
            let id = getir irs
            for el in pgn.Document.GetElementsByTagName("span") do
                if el.GetAttribute("className") = "mv" then
                    if el.Id=id.ToString() then
                        el|>highlight

        ///Goes to the first Move in the Variation
        member pgn.FirstMove() = 
            let rec goback lirs =
                pgn.PrevMove()
                if irs<>lirs then goback irs
            goback irs

        ///Make a Move in the Game - may change the Game or just select a Move
        member pgn.DoMove(mv:Move) =
            let rec getnxt oi ci (mtel:MoveTextEntry list) =
                if ci=mtel.Length then ci,false,true//implies is an extension
                else
                    let mte = mtel.[ci]
                    match mte with
                    |HalfMoveEntry(_,_,_,amv) ->
                        if amv.IsNone then failwith "should have valid aMove"
                        elif amv.Value.Mv=mv then
                            board <- amv.Value.PostBrd
                            ci,true,false
                        else ci,false,false
                    |_ -> getnxt oi (ci+1) mtel
            let isnxt,isext =
                if irs.Length>1 then 
                    let rec getmv (mtel:MoveTextEntry list) (intl:int list) =
                        if intl.Length=1 then
                            let oi = intl.Head
                            let ni,fnd,isext = getnxt oi (oi+1) mtel
                            if fnd then
                                let st = irs|>List.rev|>List.tail|>List.rev
                                irs <- st@[ni]
                            fnd,isext
                        else
                            let ih = intl.Head
                            let mte = mtel.[ih]
                            match mte with
                            |RAVEntry(nmtel) -> getmv nmtel intl.Tail
                            |_ -> failwith "should be a RAV"
                    getmv game.MoveText irs
                else
                    let ni,fnd,isext = getnxt irs.Head (irs.Head+1) game.MoveText
                    if fnd then irs <- [ni]
                    fnd,isext
            if isnxt then
                //now need to select the element
                let id = getir irs
                for el in pgn.Document.GetElementsByTagName("span") do
                    if el.GetAttribute("className") = "mv" then
                        if el.Id=id.ToString() then
                            el|>highlight
            elif isext then
                let pmv = mv|>Move.TopMove board
                let ngame,nirs = Game.AddMv game irs pmv 
                game <- ngame
                irs <- nirs
                board <- board|>Board.Push mv
                pgn.DocumentText <- mvtags()
                game|>gmchngEvt.Trigger
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
                                //now need to see if first move in rav is mv
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
                    let id = getir irs
                    for el in pgn.Document.GetElementsByTagName("span") do
                        if el.GetAttribute("className") = "mv" then
                            if el.Id=id.ToString() then
                                el|>highlight
                    else
                        //need to create a new RAV
                        let ngame,nirs = Game.AddRav game irs (mv|>Move.TopMove board) 
                        game <- ngame
                        irs <- nirs
                        board <- board|>Board.Push mv
                        pgn.DocumentText <- mvtags()
                        game|>gmchngEvt.Trigger

        //publish
        ///Provides the new Board after a change
        member __.BdChng = bdchngEvt.Publish

        ///Provides the new Game after a change
        member __.GmChng = gmchngEvt.Publish

        ///Provides the new Game after a change to the header
        member __.HdrChng = hdrchngEvt.Publish
