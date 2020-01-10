namespace KindleChess

open System.Windows.Forms
open System.Drawing
open State

module Dialogs =
    //dialogs to inherit from
    type Dialog() as this =
        inherit Form(Text = "NOT SET", Height = 110, Width = 280, 
                     FormBorderStyle = FormBorderStyle.FixedDialog)
        let vc =
            new TableLayoutPanel(Dock = DockStyle.Fill, ColumnCount = 1, 
                                 RowCount = 2)
        let hc1 =
            new FlowLayoutPanel(FlowDirection = FlowDirection.LeftToRight, 
                                Height = 30, Width = 260)
        let hc2 =
            new FlowLayoutPanel(FlowDirection = FlowDirection.RightToLeft, 
                                Height = 30, Width = 260)
        let okbtn = new Button(Text = "OK")
        let cnbtn = new Button(Text = "Cancel")
    
        do 
            this.MaximizeBox <- false
            this.MinimizeBox <- false
            this.ShowInTaskbar <- false
            this.StartPosition <- FormStartPosition.CenterParent
            hc2.Controls.Add(cnbtn)
            hc2.Controls.Add(okbtn)
            [ hc1; hc2 ] |> List.iteri (fun i c -> vc.Controls.Add(c, 0, i))
            this.Controls.Add(vc)
            this.AcceptButton <- okbtn
            this.CancelButton <- cnbtn
            //events
            cnbtn.Click.Add(fun _ -> this.Close())
            okbtn.Click.Add(this.DoOK)
        member this.AddControl(cnt) = hc1.Controls.Add(cnt)
        abstract member DoOK: System.EventArgs -> unit
        default this.DoOK (e) =
            MessageBox.Show("OK pressed")|>ignore
            this.Close()
    
    type DlgTb() as this =
        inherit Dialog()
        let tb = new TextBox(Text = "NOT SET", Width = 250)
        do
            this.AddControl(tb)
        member this.SetText(txt) = tb.Text <- txt
        member this.Tb = tb

    type DlgTbDd() as this =
        inherit Dialog()
        let tb = new TextBox(Text = "NOT SET", Width = 120)
        let dd = new ComboBox()
        do
            this.AddControl(tb)
            this.AddControl(dd)
        member this.SetText(txt) = tb.Text <- txt
        member this.Tb = tb
        member this.SetDd(vals) = 
            dd.Items.Clear()
            vals |>Array.map(fun v -> box v)
            |> dd.Items.AddRange
            if dd.Items.Count > 0 then dd.SelectedIndex <- 0
        member this.Dd = dd

    type DlgDdDd() as this =
        inherit Dialog()
        let dd1 = new ComboBox()
        let dd2 = new ComboBox()
        do
            this.AddControl(dd1)
            this.AddControl(dd2)
            dd1.SelectedIndexChanged.Add(this.DropDown1Changed)
        member this.SetDd1(vals) = 
            dd1.Items.Clear()
            vals |>Array.map(fun v -> box v)
            |> dd1.Items.AddRange
            if dd1.Items.Count > 0 then dd1.SelectedIndex <- 0
        member this.SetDd2(vals) = 
            dd2.Items.Clear()
            vals |>Array.map(fun v -> box v)
            |> dd2.Items.AddRange
            if dd2.Items.Count > 0 then dd2.SelectedIndex <- 0
        member this.Dd1 = dd1
        member this.Dd2 = dd2
        abstract member DropDown1Changed: System.EventArgs -> unit
        default this.DropDown1Changed (e) =
            MessageBox.Show("DropDown1 changed")|>ignore

    //book dialogs
    type DlgEditDet(icurb:BookT) as this =
        inherit Form(Text = "Edit Book Details", Height = 630, Width = 680, 
                     FormBorderStyle = FormBorderStyle.FixedDialog,StartPosition=FormStartPosition.CenterParent)
        let hc =
            new FlowLayoutPanel(FlowDirection = FlowDirection.LeftToRight, 
                                Dock = DockStyle.Top, AutoSize = true)
        let bklbl =
            new Label(Text = "Please Choose Book", 
                      Font = new Font("Arial", 12.0F, FontStyle.Bold, 
                                      GraphicsUnit.Point, byte (0)), 
                      AutoSize = true, TextAlign = ContentAlignment.MiddleLeft)
        let updbtn = new Button(Text = "Update")
        let tc =
            new TableLayoutPanel(Dock = DockStyle.Top, ColumnCount = 2, 
                                 RowCount = 4, AutoSize = true)
        let cllbl =
            new Label(Text = "Colour", Dock = DockStyle.Left, 
                      Font = new Font("Arial", 10.0F, FontStyle.Bold, 
                                      GraphicsUnit.Point, byte (0)))
        let cl =
            new Label(Text = "Not Set", Dock = DockStyle.Left, 
                      Font = new Font("Arial", 10.0F, FontStyle.Regular, 
                                      GraphicsUnit.Point, byte (0)))
        let crlbl =
            new Label(Text = "Creator", TextAlign = ContentAlignment.MiddleLeft)
        let cr = new TextBox(Text = "Not Set")
        let dtlbl =
            new Label(Text = "Publication Date", 
                      TextAlign = ContentAlignment.MiddleLeft)
        let dt = new DateTimePicker()
        let dclbl =
            new Label(Text = "Description", 
                      TextAlign = ContentAlignment.MiddleLeft)
        let dc = new TextBox(Text = "Not Set", Width = 400)
        let wpnl = new Panel(Dock = DockStyle.Fill)
        let tpnl =
            new Panel(Dock = DockStyle.Top, Height = 20, 
                      BackColor = Color.LightGray)
        let wlbl =
            new Label(Text = "Book Welcome", 
                      TextAlign = ContentAlignment.MiddleLeft)
        let wel =
            new TextBox(Text = "Not Set", Dock = DockStyle.Fill, 
                        Multiline = true, 
                        Font = new Font("Microsoft Sans Serif", 10.0f))

        // load Book
        let ldBook (curb : BookT) =
            bklbl.Text <- curb.Title
            cl.Text <- if curb.IsW then "White"
                       else "Black"
            cr.Text <- curb.Creator
            dt.Value <- curb.Date
            dc.Text <- curb.Desc
            wel.Text <- curb.Welcome
        // update book info
        let doupd (e) = stt.UpdateBook(cr.Text, dt.Value, dc.Text, wel.Text)

        do
            icurb|>ldBook
            tpnl.Controls.Add(wlbl)
            wpnl.Controls.Add(wel)
            wpnl.Controls.Add(tpnl)
            this.Controls.Add(wpnl)
            tc.Controls.Add(bklbl, 0, 0)
            tc.Controls.Add(updbtn, 1, 0)
            tc.Controls.Add(cllbl, 0, 0)
            tc.Controls.Add(cl, 1, 0)
            tc.Controls.Add(crlbl, 0, 1)
            tc.Controls.Add(cr, 1, 1)
            tc.Controls.Add(dtlbl, 0, 2)
            tc.Controls.Add(dt, 1, 2)
            tc.Controls.Add(dclbl, 0, 3)
            tc.Controls.Add(dc, 1, 3)
            this.Controls.Add(tc)
            hc.Controls.Add(bklbl)
            hc.Controls.Add(updbtn)
            this.Controls.Add(hc)
            updbtn.Click.Add(doupd)

    type DlgNewBk() as this =
        inherit DlgTbDd(Text = "Create New Book")
        do
            this.SetText("Type Title Here")
            this.SetDd([| "White";"Black" |])
        override this.DoOK (e) =
            stt.NewBook(this.Tb.Text, this.Dd.SelectedIndex = 0)
            (new DlgEditDet(stt.CurBook)).ShowDialog()|>ignore
            this.Close()

    //type DlgOpnBk() as this =
    //    inherit DlgDdDd(Text = "Open Book")

    //    do
    //        this.SetDd1([| "White";"Black" |])
    //        this.SetDd2(stt.Books(true)|>List.toArray)
    //    override this.DoOK (e) =
    //        stt.OpenBook(this.Dd2.Text, this.Dd1.SelectedIndex = 0)
    //        this.Close()
    //    override this.DropDown1Changed(e) =
    //        let isw = this.Dd1.SelectedIndex = 0
    //        this.Dd2.Items.Clear()
    //        this.SetDd2(stt.Books(isw)|>List.toArray)
    
    //type DlgSaveAsBk() as this =
    //    inherit DlgTb(Text = "Save Book As")
    //    do
    //        this.SetText("Type Name Here")
    //    override this.DoOK (e) =
    //        stt.SaveAsBook(this.Tb.Text)
    //        this.Close()
    

    ////chapter dialogs
    //type DlgAdd() as this =
    //    inherit DlgTb(Text = "Add New Chapter")
    //    do
    //        this.SetText("Type Name Here")
    //    override this.DoOK (e) =
    //        stt.AddChapter(this.Tb.Text)
    //        this.Close()

    //type DlgIns() as this =
    //    inherit DlgTb(Text = "Insert New Chapter")
    //    do
    //        this.SetText("Type Name Here")
    //    override this.DoOK (e) =
    //        stt.InsChapter(this.Tb.Text)
    //        this.Close()

    //type DlgRnm(i) as this =
    //    inherit DlgTb(Text = "Rename Chapter")
    //    do
    //        this.SetText(stt.CurBook.Chapters.[i].Name)
    //    override this.DoOK (e) =
    //        stt.ChRename(this.Tb.Text)
    //        this.Close()

    //type DlgAddGm() as this =
    //    inherit Form(Text = "Add New Game Chapter", Height = 230, Width = 280, 
    //                 FormBorderStyle = FormBorderStyle.FixedDialog)
    //    let vc =
    //        new TableLayoutPanel(Dock = DockStyle.Fill, ColumnCount = 1, 
    //                             RowCount = 2)
    //    let hc1 =
    //        new FlowLayoutPanel(FlowDirection = FlowDirection.LeftToRight, 
    //                            Height = 30, Width = 260)
    //    let hc2 =
    //        new FlowLayoutPanel(FlowDirection = FlowDirection.RightToLeft, 
    //                            Height = 30, Width = 260)
    //    let nm = new TextBox(Text = "Type Chapter Name Here", Width = 260)
    //    let wh = new TextBox(Text = "Type White Player Name Here", Width = 260)
    //    let bl = new TextBox(Text = "Type Black Player Name Here", Width = 260)
    //    let ev = new TextBox(Text = "Type Event Here", Width = 260)
    //    let dtlbl =
    //        new Label(Text = "Date", 
    //                  TextAlign = ContentAlignment.MiddleLeft, Width=30)
    //    let dt = new DateTimePicker()
    //    let okbtn = new Button(Text = "OK")
    //    let cnbtn = new Button(Text = "Cancel")
    
    //    let donew (e) =
    //        let gmdt = {White=wh.Text;Black=bl.Text;Event=ev.Text;GmDate=dt.Value}
    //        stt.AddGameChapter(nm.Text,gmdt.ToString())
    //        this.Close()
    
    //    do 
    //        this.MaximizeBox <- false
    //        this.MinimizeBox <- false
    //        this.ShowInTaskbar <- false
    //        this.StartPosition <- FormStartPosition.CenterParent
    //        hc1.Controls.Add(dtlbl)
    //        hc1.Controls.Add(dt)
    //        hc2.Controls.Add(cnbtn)
    //        hc2.Controls.Add(okbtn)
    //        vc.Controls.Add(nm)
    //        vc.Controls.Add(wh)
    //        vc.Controls.Add(bl)
    //        vc.Controls.Add(ev)
    //        vc.Controls.Add(hc1)
    //        vc.Controls.Add(hc2)

    //        this.Controls.Add(vc)
    //        this.AcceptButton <- okbtn
    //        this.CancelButton <- cnbtn
    //        //events
    //        cnbtn.Click.Add(fun _ -> this.Close())
    //        okbtn.Click.Add(donew)

    //type DlgEditGm(i) as this =
    //    inherit Form(Text = "Edit Game Details", Height = 230, Width = 280, 
    //                 FormBorderStyle = FormBorderStyle.FixedDialog)
    //    let vc =
    //        new TableLayoutPanel(Dock = DockStyle.Fill, ColumnCount = 1, 
    //                             RowCount = 2)
    //    let hc1 =
    //        new FlowLayoutPanel(FlowDirection = FlowDirection.LeftToRight, 
    //                            Height = 30, Width = 260)
    //    let hc2 =
    //        new FlowLayoutPanel(FlowDirection = FlowDirection.RightToLeft, 
    //                            Height = 30, Width = 260)
    //    let nm = new TextBox(Text = "Type Chapter Name Here", Width = 260)
    //    let wh = new TextBox(Text = "Type White Player Name Here", Width = 260)
    //    let bl = new TextBox(Text = "Type Black Player Name Here", Width = 260)
    //    let ev = new TextBox(Text = "Type Event Here", Width = 260)
    //    let dtlbl =
    //        new Label(Text = "Date", 
    //                  TextAlign = ContentAlignment.MiddleLeft, Width=30)
    //    let dt = new DateTimePicker()
    //    let okbtn = new Button(Text = "OK")
    //    let cnbtn = new Button(Text = "Cancel")
    //    let mutable isnmch = false

    //    let loadch() =
    //        let ch = stt.CurBook.Chapters.[i]
    //        isnmch <- ch.Intro = ""
    //        if isnmch then
    //            MessageBox.Show("This is not a Game Chapter!")|>ignore
    //        else
    //            nm.Text <- ch.Name
    //            let gmdt = ch.Intro|>GmChHdT.FromStr
    //            wh.Text <- gmdt.White
    //            bl.Text <- gmdt.Black
    //            ev.Text <- gmdt.Event
    //            dt.Value <- gmdt.GmDate

    //    let doedit (e) =
    //        let gmdt = {White=wh.Text;Black=bl.Text;Event=ev.Text;GmDate=dt.Value}
    //        stt.EditGameDet(nm.Text,gmdt.ToString())
    //        this.Close()

    //    do 
    //        this.MaximizeBox <- false
    //        this.MinimizeBox <- false
    //        this.ShowInTaskbar <- false
    //        this.StartPosition <- FormStartPosition.CenterParent
    //        loadch()
    //        hc1.Controls.Add(dtlbl)
    //        hc1.Controls.Add(dt)
    //        hc2.Controls.Add(cnbtn)
    //        hc2.Controls.Add(okbtn)
    //        vc.Controls.Add(nm)
    //        vc.Controls.Add(wh)
    //        vc.Controls.Add(bl)
    //        vc.Controls.Add(ev)
    //        vc.Controls.Add(hc1)
    //        vc.Controls.Add(hc2)

    //        this.Controls.Add(vc)
    //        this.AcceptButton <- okbtn
    //        this.CancelButton <- cnbtn
    //        //events
    //        cnbtn.Click.Add(fun _ -> this.Close())
    //        okbtn.Click.Add(doedit)
    //        if isnmch then this.Load.Add(fun _ -> this.Close())

    ////move dialogs
    //type DlgSetNAG() as this =
    //    inherit Form(Text = "Set Evaluation", Height = 200, Width = 400, 
    //                 FormBorderStyle = FormBorderStyle.FixedDialog)
    //    let vc =
    //        new TableLayoutPanel(Dock = DockStyle.Fill, ColumnCount = 1, 
    //                             RowCount = 2, Height = 90)
    //    let vc1 =
    //        new TableLayoutPanel(Dock = DockStyle.Fill, ColumnCount = 4, 
    //                             RowCount = 4, Height = 120)
    //    let hc =
    //        new FlowLayoutPanel(FlowDirection = FlowDirection.RightToLeft, 
    //                            Height = 30, Width = 380)
        
    //    let nagstr i =
    //        match i with
    //        | 0 -> "None"
    //        | 1 -> "!"
    //        | 2 -> "?"
    //        | 3 -> "!!"
    //        | 4 -> "??"
    //        | 5 -> "!?"
    //        | 6 -> "?!"
    //        | 10 -> "="
    //        | 14 -> "+/="
    //        | 15 -> "=/+"
    //        | 16 -> "+/-"
    //        | 17 -> "-/+"
    //        | 18 -> "+−"
    //        | 19 -> "−+"
    //        | _ -> "NotUsed"
        
    //    let rbs =
    //        [ 0..19 ]
    //        |> List.map (fun i -> i, i |> nagstr)
    //        |> List.filter (fun (i, n) -> n <> "NotUsed")
    //        |> List.map 
    //               (fun (i, n) -> 
    //               new RadioButton(Text = n, Tag = box i, 
    //                               Font = new Font("Microsoft Sans Serif", 10.0f)))
        
    //    let okbtn = new Button(Text = "OK")
    //    let cnbtn = new Button(Text = "Cancel")
        
    //    let loadmv() =
    //        let tel = stt.Tel
    //        let mct = stt.Mct
    //        let te = tel.[mct - 1]
    //        let nag = te.NAG
    //        this.Text <- "Set Evaluation for " + te.Mv.Mpgn
    //        rbs
    //        |> List.filter (fun r -> r.Tag
    //                                 |> unbox = nag)
    //        |> List.iter (fun r -> r.Checked <- true)
        
    //    let dosetnag (e) =
    //        let nag : int =
    //            (rbs |> List.filter (fun r -> r.Checked)).[0].Tag |> unbox
    //        stt.SetNag(nag)
    //        this.Close()
        
    //    do 
    //        this.MaximizeBox <- false
    //        this.MinimizeBox <- false
    //        this.ShowInTaskbar <- false
    //        this.StartPosition <- FormStartPosition.CenterParent
    //        rbs.[..3] |> List.iteri (fun i c -> vc1.Controls.Add(c, i, 0))
    //        rbs.[4..8] |> List.iteri (fun i c -> vc1.Controls.Add(c, i, 1))
    //        rbs.[9..12] |> List.iteri (fun i c -> vc1.Controls.Add(c, i, 2))
    //        rbs.[13..] |> List.iteri (fun i c -> vc1.Controls.Add(c, i, 3))
    //        hc.Controls.Add(cnbtn)
    //        hc.Controls.Add(okbtn)
    //        vc.Controls.Add(hc, 0, 1)
    //        vc.Controls.Add(vc1, 0, 0)
    //        this.Controls.Add(vc)
    //        loadmv()
    //        this.AcceptButton <- okbtn
    //        this.CancelButton <- cnbtn
    //        //events
    //        cnbtn.Click.Add(fun _ -> this.Close())
    //        okbtn.Click.Add(dosetnag)
