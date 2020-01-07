namespace FsChess.WinForms

open System.Drawing
open System.Windows.Forms
open FsChess

[<AutoOpen>]
module Library =
    
    let private img nm =
        let thisExe = System.Reflection.Assembly.GetExecutingAssembly()
        let file = thisExe.GetManifestResourceStream("FsChess.WinForms.Images." + nm)
        Image.FromStream(file)

    let private cur nm =
        let thisExe = System.Reflection.Assembly.GetExecutingAssembly()
        //let nms = thisExe.GetManifestResourceNames()
        let file = thisExe.GetManifestResourceStream("FsChess.WinForms.Cursors." + nm)
        new Cursor(file)
 
    type PnlBoard() as bd =
        inherit Panel(Width = 400, Height = 450)

        let mutable board = Board.Start
        let mutable sqTo = -1
        let mutable cCur = Cursors.Default
        let bdpnl = new Panel(Dock = DockStyle.Top, Height = 400)
        let sqpnl = new Panel(Width = 420, Height = 420, Left = 29, Top = 13)
        
        let edges =
            [ new Panel(BackgroundImage = img "Back.jpg", Width = 342, 
                        Height = 8, Left = 24, Top = 6)
              
              new Panel(BackgroundImage = img "Back.jpg", Width = 8, 
                        Height = 350, Left = 24, Top = 8)
              
              new Panel(BackgroundImage = img "Back.jpg", Width = 8, 
                        Height = 350, Left = 366, Top = 6)
              
              new Panel(BackgroundImage = img "Back.jpg", Width = 342, 
                        Height = 8, Left = 32, Top = 350) ]
        
        let sqs : PictureBox [] = Array.zeroCreate 64
        let flbls : Label [] = Array.zeroCreate 8
        let rlbls : Label [] = Array.zeroCreate 8

        /// get cursor given char
        let getcur c =
            match c with
            | "P" -> cur "WhitePawn.cur"
            | "B" -> cur "WhiteBishop.cur"
            | "N" -> cur "WhiteKnight.cur"
            | "R" -> cur "WhiteRook.cur"
            | "K" -> cur "WhiteKing.cur"
            | "Q" -> cur "WhiteQueen.cur"
            | "p" -> cur "BlackPawn.cur"
            | "b" -> cur "BlackBishop.cur"
            | "n" -> cur "BlackKnight.cur"
            | "r" -> cur "BlackRook.cur"
            | "k" -> cur "BlackKing.cur"
            | "q" -> cur "BlackQueen.cur"
            | _ -> failwith "invalid piece"
 
        /// get image given char
        let getim c =
            match c with
            | "P" -> img "WhitePawn.png"
            | "B" -> img "WhiteBishop.png"
            | "N" -> img "WhiteKnight.png"
            | "R" -> img "WhiteRook.png"
            | "K" -> img "WhiteKing.png"
            | "Q" -> img "WhiteQueen.png"
            | "p" -> img "BlackPawn.png"
            | "b" -> img "BlackBishop.png"
            | "n" -> img "BlackKnight.png"
            | "r" -> img "BlackRook.png"
            | "k" -> img "BlackKing.png"
            | "q" -> img "BlackQueen.png"
            | _ -> failwith "invalid piece"

        ///set pieces on squares
        let setpcsmvs () =
            let setpcsmvs() =
                board.PieceAt
                |>List.map Piece.ToStr
                |> List.iteri (fun i c -> sqs.[i].Image <- if c = " " then null else getim c)
                //mvstb.Text <- stt.GetMvsStr()
            if (bd.InvokeRequired) then 
                try 
                    bd.Invoke(MethodInvoker(setpcsmvs)) |> ignore
                with _ -> ()
            else setpcsmvs()

        ///highlight possible squares
        let highlightsqs sl =
            sqs
            |> Array.iteri (fun i sq -> 
                   sqs.[i].BackColor <- if (i % 8 + i / 8) % 2 = 1 then 
                                            Color.Green
                                        else Color.PaleGreen)
            sl
            |> List.iter (fun s -> 
                   sqs.[s].BackColor <- if (s % 8 + s / 8) % 2 = 1 then 
                                            Color.YellowGreen
                                        else Color.Yellow)

        /// Action for GiveFeedback
        let giveFeedback (e : GiveFeedbackEventArgs) =
            e.UseDefaultCursors <- false
            sqpnl.Cursor <- cCur

        /// Action for Drag Over
        let dragOver (e : DragEventArgs) = e.Effect <- DragDropEffects.Move

        /// Action for Drag Drop
        let dragDrop (p : PictureBox, e) =
            sqTo <- System.Convert.ToInt32(p.Tag)
            sqpnl.Cursor <- Cursors.Default

        /// Action for Mouse Down
        let mouseDown (p : PictureBox, e : MouseEventArgs) =
            if e.Button = MouseButtons.Left then 
                let sqFrom = System.Convert.ToInt32(p.Tag)
                let sqf:Square = sqFrom|>int16
                let psmvs = sqf|>Board.PossMoves board
                let pssqs = psmvs|>List.map(fun m -> m|>Move.To|>int)
                pssqs|>highlightsqs
                let oimg = p.Image
                p.Image <- null
                p.Refresh()
                let c = board.PieceAt.[sqFrom]|>Piece.ToStr
                cCur <- getcur c
                sqpnl.Cursor <- cCur
                if pssqs.Length > 0 && (p.DoDragDrop(oimg, DragDropEffects.Move) = DragDropEffects.Move) then 
                    let mvl = psmvs|>List.filter(fun m ->m|>Move.To|>int=sqTo)
                    if mvl.Length=1 then
                        board <- board|>Board.Push mvl.Head
                        setpcsmvs()
                    else p.Image <- oimg
                else p.Image <- oimg
                sqpnl.Cursor <- Cursors.Default
                []|>highlightsqs

        /// creates file label
        let flbl i lbli =
            let lbl = new Label()
            lbl.Text <- FILE_NAMES.[i]
            lbl.Font <- new Font("Arial", 12.0F, FontStyle.Bold, 
                                 GraphicsUnit.Point, byte (0))
            lbl.ForeColor <- Color.Green
            lbl.Height <- 21
            lbl.Width <- 42
            lbl.TextAlign <- ContentAlignment.MiddleCenter
            lbl.Left <- i * 42 + 30
            lbl.Top <- 8 * 42 + 24
            flbls.[i] <- lbl

        /// creates rank label
        let rlbl i lbli =
            let lbl = new Label()
            lbl.Text <- (i + 1).ToString()
            lbl.Font <- new Font("Arial", 12.0F, FontStyle.Bold, 
                                 GraphicsUnit.Point, byte (0))
            lbl.ForeColor <- Color.Green
            lbl.Height <- 42
            lbl.Width <- 21
            lbl.TextAlign <- ContentAlignment.MiddleCenter
            lbl.Left <- 0
            lbl.Top <- 7 * 42 - i * 42 + 16
            rlbls.[i] <- lbl
       
        ///set board colours and position of squares
        let setsq i sqi =
            let r = i / 8
            let f = i % 8
            let sq =
                new PictureBox(Height = 42, Width = 42, 
                               SizeMode = PictureBoxSizeMode.CenterImage)
            sq.BackColor <- if (f + r) % 2 = 1 then Color.Green
                            else Color.PaleGreen
            sq.Left <- f * 42 + 1
            sq.Top <- r * 42 + 1
            sq.Tag <- i
            //events
            sq.MouseDown.Add(fun e -> mouseDown (sq, e))
            sq.DragDrop.Add(fun e -> dragDrop (sq, e))
            sq.AllowDrop <- true
            sq.DragOver.Add(dragOver)
            sq.GiveFeedback.Add(giveFeedback)
            sqs.[i] <- sq
        
        do 
            //mvstb |> mvspnl.Controls.Add
            //mvspnl |> bd.Controls.Add
            sqs |> Array.iteri setsq
            sqs |> Array.iter sqpnl.Controls.Add
            setpcsmvs()
            edges |> List.iter bdpnl.Controls.Add
            flbls |> Array.iteri flbl
            flbls |> Array.iter bdpnl.Controls.Add
            rlbls |> Array.iteri rlbl
            rlbls |> Array.iter bdpnl.Controls.Add
            sqpnl |> bdpnl.Controls.Add
            bdpnl |> bd.Controls.Add

        member bd.SetBoard(ibd:Brd) =
            board<-ibd
            setpcsmvs()

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
        

        do
            pgn.DocumentText <- mvtags()
            pgn.DocumentCompleted.Add(setclicks)
            pgn.ObjectForScripting <- pgn

        //member val Game = game with get,set

        member pgn.SetGame(gm:Game) = 
            game <- gm|>Game.GetaMoves
            pgn.DocumentText <- mvtags()

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





        //publish
        member __.BdChng = bdchngEvt.Publish
