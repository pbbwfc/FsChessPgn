namespace KindleChess

open System
open System.Drawing
open System.Windows.Forms
open FsChess
open FsChess.WinForms
open Dialogs
open State

module Form =
    let img nm =
        let thisExe = System.Reflection.Assembly.GetExecutingAssembly()
        let file = thisExe.GetManifestResourceStream("KindleChess3.Images." + nm)
        Image.FromStream(file)
    let ico nm =
        let thisExe = System.Reflection.Assembly.GetExecutingAssembly()
        let file = thisExe.GetManifestResourceStream("KindleChess3.Icons." + nm)
        new Icon(file)
    type Nav =
        | Home
        | Prev
        | Next
        | End

    type FrmMain() as this =
        inherit Form(Text = "Kindle Chess", WindowState = FormWindowState.Maximized, Icon = ico "KindleChess.ico", IsMdiContainer = true)

        let mutable gms = []
        let mutable ct = 0
        
        let bd,pgn = CreateLnkBrdPgn()

        let mm =
            let m = new MenuStrip()
            //do book menu
            let bk = new ToolStripMenuItem("Book")
            //do new
            let nwb =
                new ToolStripMenuItem(Image = img "new.png", Text = "&New")
            nwb.Click.Add(fun _ -> (new DlgNewBk()).ShowDialog() |> ignore)
            bk.DropDownItems.Add(nwb) |> ignore
            //do open
            let opnb =
                new ToolStripMenuItem(Image = img "opn.png", Text = "&Open")
            opnb.Click.Add(fun _ -> (new DlgOpnBk()).ShowDialog() |> ignore)
            bk.DropDownItems.Add(opnb) |> ignore
            //do save
            let dosaveb (e) = 
                let cho = if stt.Chi>=0 then pgn.GetGame()|>Some else None
                stt.SaveBook(cho) |> ignore
            let savb =
                new ToolStripMenuItem(Image = img "sav.png", Text = "&Save")
            savb.Click.Add(dosaveb)
            bk.DropDownItems.Add(savb) |> ignore
            m.Items.Add(bk) |> ignore
            //do save as 
            let savab =
                new ToolStripMenuItem(Image = img "sava.png", Text = "Save &As")
            savab.Click.Add(fun _ -> (new DlgSaveAsBk()).ShowDialog() |> ignore)
            bk.DropDownItems.Add(savab) |> ignore
            //do delete
            let dodelb (e) =
                let nm, isw = stt.CurBook.Title, stt.CurBook.IsW
                if MessageBox.Show
                       ("Do you want to delete book " + nm + "?", "Delete Book", 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes then 
                    stt.DelBook(nm, isw)
    
            let delb = new ToolStripMenuItem("Delete")
            //delb.ShortcutKeys <- Keys.Alt ||| Keys.D
            delb.Click.Add(dodelb)
            bk.DropDownItems.Add(delb) |> ignore
            //do edit book details
            let doeditbk (e) = (new DlgEditDet(stt.CurBook)).ShowDialog() |> ignore
            let editbk =
                new ToolStripMenuItem(Text = "Edit Details")
            editbk.Click.Add(doeditbk)
            bk.DropDownItems.Add(editbk) |> ignore
            // add separator
            bk.DropDownItems.Add(new ToolStripSeparator()) |> ignore
            //do generate html
            let dogenhb (e) = stt.GenHTML() |> ignore
            let genhb =
                new ToolStripMenuItem(Text = "Generate &HTML")
            genhb.Click.Add(dogenhb)
            bk.DropDownItems.Add(genhb) |> ignore
            //do chapter menu
            let chp = new ToolStripMenuItem("Chapter")
            //do rename
            let chopn = new ToolStripMenuItem("Open")
            //rnm.ShortcutKeys <- Keys.Control ||| Keys.O
            chopn.Click.Add(fun _ -> if stt.Chi>=0 then (new DlgOpn()).ShowDialog() |> ignore)
            chp.DropDownItems.Add(chopn) |> ignore
            //do rename
            let rnm = new ToolStripMenuItem("Rename")
            //rnm.ShortcutKeys <- Keys.Control ||| Keys.O
            rnm.Click.Add(fun _ -> if stt.Chi>=0 then (new DlgRnm(stt.Chi)).ShowDialog() |> ignore)
            chp.DropDownItems.Add(rnm) |> ignore
            // add separator
            chp.DropDownItems.Add(new ToolStripSeparator()) |> ignore
            //do add
            let ad = new ToolStripMenuItem("Add")
            //ad.ShortcutKeys <- Keys.Control ||| Keys.N
            ad.Click.Add(fun _ -> (new DlgAdd()).ShowDialog() |> ignore)
            chp.DropDownItems.Add(ad) |> ignore
            //do delete
            let dodelc (e) =
                if stt.Chi>=0 then
                    let nm = stt.CurBook.Chapters.[stt.Chi]
                    if MessageBox.Show
                           ("Do you want to delete chapter " + nm + "?", 
                            "Delete Chapter", MessageBoxButtons.YesNo, 
                            MessageBoxIcon.Question) = DialogResult.Yes then 
                        stt.ChDelete(nm)
    
            let delc = new ToolStripMenuItem("Delete")
            //delc.ShortcutKeys <- Keys.Alt ||| Keys.D
            delc.Click.Add(dodelc)
            chp.DropDownItems.Add(delc) |> ignore
            m.Items.Add(chp) |> ignore
            // add separator
            chp.DropDownItems.Add(new ToolStripSeparator()) |> ignore
            //do move up
            let mvu = new ToolStripMenuItem("Move Up")
            //ad.ShortcutKeys <- Keys.Control ||| Keys.N
            mvu.Click.Add(fun _ -> if stt.Chi>=0 then stt.MoveUpChapter())
            chp.DropDownItems.Add(mvu) |> ignore
            //do move down
            let mvd = new ToolStripMenuItem("Move Down")
            //ad.ShortcutKeys <- Keys.Control ||| Keys.N
            mvd.Click.Add(fun _ -> if stt.Chi>=0 then stt.MoveDownChapter())
            chp.DropDownItems.Add(mvd) |> ignore
            // add separator
            chp.DropDownItems.Add(new ToolStripSeparator()) |> ignore
            //do edit game details
            let edg = new ToolStripMenuItem("Edit Game Details")
            //ad.ShortcutKeys <- Keys.Control ||| Keys.N
            //edg.Click.Add(fun _ -> if stt.Chi>=0 then (new DlgEditGm(stt.Chi)).ShowDialog() |> ignore)
            chp.DropDownItems.Add(edg) |> ignore
   
            //do about menu
            let abt = new ToolStripMenuItem("About")
            //do docs
            let onl = new ToolStripMenuItem("Online Documentation")
            onl.Click.Add
                (fun _ -> 
                System.Diagnostics.Process.Start
                    ("https://pbbwfc.github.io/KindleChess/") |> ignore)
            abt.DropDownItems.Add(onl) |> ignore
            //do source code
            let src = new ToolStripMenuItem("Source Code")
            src.Click.Add
                (fun _ -> 
                System.Diagnostics.Process.Start
                    ("https://github.com/pbbwfc/KindleChess") |> ignore)
            abt.DropDownItems.Add(src) |> ignore
            m.Items.Add(abt) |> ignore
            m

        let tb =
            let ts = new ToolStrip()
            //do new
            let nwb = new ToolStripButton(Image = img "new.png", Text = "&New")
            nwb.Click.Add(fun _ -> (new DlgNewBk()).ShowDialog() |> ignore)
            ts.Items.Add(nwb) |> ignore
            //do open
            let opnb =
                new ToolStripButton(Image = img "opn.png", Text = "&Open")
            opnb.Click.Add(fun _ -> (new DlgOpnBk()).ShowDialog() |> ignore)
            ts.Items.Add(opnb) |> ignore
            //do save
            let dosaveb (e) = 
                let cho = if stt.Chi>=0 then pgn.GetGame()|>Some else None
                stt.SaveBook(cho) |> ignore
            let savb =
                new ToolStripButton(Image = img "sav.png", Text = "&Save")
            savb.Click.Add(dosaveb)
            ts.Items.Add(savb) |> ignore
            //do save as 
            let savab =
                new ToolStripButton(Image = img "sava.png", Text = "Save &As")
            savab.Click.Add(fun _ -> (new DlgSaveAsBk()).ShowDialog() |> ignore)
            ts.Items.Add(savab) |> ignore
            //do edit book details
            let doeditbk (e) = (new DlgEditDet(stt.CurBook)).ShowDialog() |> ignore
            let editbk =
                new ToolStripButton(Text = "Edit Details")
            editbk.Click.Add(doeditbk)
            ts.Items.Add(editbk) |> ignore
            //do generate html
            let dogenhb (e) = stt.GenHTML() |> ignore
            let genhb =
                new ToolStripButton(Text = "Generate &HTML")
            genhb.Click.Add(dogenhb) |> ignore
            ts.Items.Add(genhb) |> ignore
            ts

        let pnl = new Panel(Dock=DockStyle.Top,Height=30)
        let nmlbl =
            new Label(Text = "Not loaded", Dock = DockStyle.Left, 
                      TextAlign = ContentAlignment.MiddleLeft,Width=400)
        let homeb = new ToolStripButton(Image = img "homeButton.png")
        let prevb = new ToolStripButton(Image = img "prevButton.png")
        let nextb = new ToolStripButton(Image = img "nextButton.png")
        let endb = new ToolStripButton(Image = img "endButton.png")
        let ts =
            new ToolStrip(Anchor = AnchorStyles.Right, 
                          GripStyle = ToolStripGripStyle.Hidden, 
                          Dock = DockStyle.None, Left = 100)

        let rtpnl = new Panel(Dock=DockStyle.Fill)
    
        // do navigation
        let donav (n) =
            match n with
            | Next -> pgn.NextMove()
            | End -> pgn.LastMove()
            | Prev -> pgn.PrevMove()
            | Home -> pgn.FirstMove()
        
        // keyDown
        let dokeydown (e : KeyEventArgs) =
            e.Handled <- true
            let s = new obj()
            if (e.KeyCode = Keys.Home) then donav (Home)
            if (e.KeyCode = Keys.Left) then donav (Prev)
            if (e.KeyCode = Keys.Right) then donav (Next)
            if (e.KeyCode = Keys.End) then donav (End)

        // add chapter tab
        let addchap (nm, ch : Game) =
            ch|>pgn.SetGame
            nmlbl.Text <- nm
        // chapter renamed
        let chaprnm nm =
            nmlbl.Text <- nm

        
        do
            pgn|>rtpnl.Controls.Add
            [ homeb; prevb; nextb; endb ] 
            |> List.iter (fun c -> ts.Items.Add(c) |> ignore)
            pnl.Controls.Add(ts)
            pnl.Controls.Add(nmlbl)
            pnl|>rtpnl.Controls.Add
            rtpnl|>this.Controls.Add
            bd|>this.Controls.Add
            tb|>this.Controls.Add
            mm|>this.Controls.Add
            this.KeyPreview <- true
            //events
            //TODO does not work
            this.KeyDown.Add(dokeydown)
            homeb.Click.Add(fun _ -> donav (Home))
            prevb.Click.Add(fun _ -> donav (Prev))
            nextb.Click.Add(fun _ -> donav (Next))
            endb.Click.Add(fun _ -> donav (End))
            
            stt.ChAdd |> Observable.add addchap
            stt.ChRnm |> Observable.add chaprnm
            stt.ChChg |> Observable.add addchap
            stt.Ornt |> Observable.add bd.Orient