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
            let dosaveb (e) = stt.SaveBook() |> ignore
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
            // add separator
            bk.DropDownItems.Add(new ToolStripSeparator()) |> ignore
            //do generate html
            //let dogenhb (e) = stt.GenHTML() |> ignore
            let genhb =
                new ToolStripMenuItem(Image = img "sava.png", 
                                      Text = "Generate &HTML")
            //genhb.Click.Add(dogenhb)
            bk.DropDownItems.Add(genhb) |> ignore
            //do chapter menu
            let chp = new ToolStripMenuItem("Chapter")
            //do rename
            let chopn = new ToolStripMenuItem("Open")
            //rnm.ShortcutKeys <- Keys.Control ||| Keys.O
            chopn.Click.Add(fun _ -> if stt.Chi>=0 then (new DlgOpn()).ShowDialog() |> ignore)
            chp.DropDownItems.Add(chopn) |> ignore
            //do save
            let chsav = new ToolStripMenuItem("Save")
            chsav.Click.Add(fun _ -> stt.ChSave(pgn.GetGame()))
            chp.DropDownItems.Add(chsav) |> ignore
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
            //do insert
            let ins = new ToolStripMenuItem("Insert")
            //ad.ShortcutKeys <- Keys.Control ||| Keys.N
            //ins.Click.Add(fun _ -> if stt.Chi>=0 then (new DlgIns()).ShowDialog() |> ignore)
            chp.DropDownItems.Add(ins) |> ignore
            // add separator
            chp.DropDownItems.Add(new ToolStripSeparator()) |> ignore
            //do move up
            let mvu = new ToolStripMenuItem("Move Up")
            //ad.ShortcutKeys <- Keys.Control ||| Keys.N
            //mvu.Click.Add(fun _ -> if stt.Chi>=0 then stt.MoveUpChapter())
            chp.DropDownItems.Add(mvu) |> ignore
            //do move down
            let mvd = new ToolStripMenuItem("Move Down")
            //ad.ShortcutKeys <- Keys.Control ||| Keys.N
            //mvd.Click.Add(fun _ -> if stt.Chi>=0 then stt.MoveDownChapter())
            chp.DropDownItems.Add(mvd) |> ignore
            // add separator
            chp.DropDownItems.Add(new ToolStripSeparator()) |> ignore
            //do copy
            let cop = new ToolStripMenuItem("Copy")
            //ad.ShortcutKeys <- Keys.Control ||| Keys.N
            //cop.Click.Add(fun _ -> if stt.Chi>=0 then stt.CopyChapter())
            chp.DropDownItems.Add(cop) |> ignore
            //do paste
            let pst = new ToolStripMenuItem("Paste")
            //ad.ShortcutKeys <- Keys.Control ||| Keys.N
            //pst.Click.Add(fun _ -> if stt.Chi>=0 then stt.PasteChapter())
            chp.DropDownItems.Add(pst) |> ignore
            // add separator
            chp.DropDownItems.Add(new ToolStripSeparator()) |> ignore
            //do add game chapter
            let adg = new ToolStripMenuItem("Add Game")
            //ad.ShortcutKeys <- Keys.Control ||| Keys.N
            //adg.Click.Add(fun _ -> (new DlgAddGm()).ShowDialog() |> ignore)
            chp.DropDownItems.Add(adg) |> ignore
            //do paste game chapter
            let psg = new ToolStripMenuItem("Paste Game")
            //ad.ShortcutKeys <- Keys.Control ||| Keys.N
            //psg.Click.Add(fun _ -> if stt.Chi>=0 then stt.PasteGame())
            chp.DropDownItems.Add(psg) |> ignore
            //do edit game details
            let edg = new ToolStripMenuItem("Edit Game Details")
            //ad.ShortcutKeys <- Keys.Control ||| Keys.N
            //edg.Click.Add(fun _ -> if stt.Chi>=0 then (new DlgEditGm(stt.Chi)).ShowDialog() |> ignore)
            chp.DropDownItems.Add(edg) |> ignore
   
            //do line menu
            let ln = new ToolStripMenuItem("Line")
    
            //do set as sub line
            //let dosubline (e) =
            //    let vid = stt.Vid
            //    if vid <> "" && not (vid.Contains("s")) then do stt.SetSub()
    
            let subl =
                new ToolStripMenuItem(Image = img "down-32-red.png", 
                                      Text = "Set as Sub Line")
            //subl.Click.Add(dosubline)
            ln.DropDownItems.Add(subl) |> ignore
            //do set as not sub line
            //let domainline (e) =
            //    let vid = stt.Vid
            //    if vid <> "" && vid.Contains("s") then do stt.SetMain()
    
            let mainl =
                new ToolStripMenuItem(Image = img "up-32-red.png", 
                                      Text = "Set as Main Line")
            //mainl.Click.Add(domainline)
            ln.DropDownItems.Add(mainl) |> ignore
            //do move up line
            //let domoveupline (e) =
            //    let vid = stt.Vid
            //    if vid <> "" then do stt.MoveUp()
    
            let moveupl =
                new ToolStripMenuItem(Image = img "up-32-yell.png", 
                                      Text = "Move Up")
            moveupl.ShortcutKeys <- Keys.Alt ||| Keys.Up
            //moveupl.Click.Add(domoveupline)
            ln.DropDownItems.Add(moveupl) |> ignore
            //do move down line
            //let domovedownline (e) =
            //    let vid = stt.Vid
            //    if vid <> "" then do stt.MoveDown()
    
            let movedownl =
                new ToolStripMenuItem(Image = img "down-32-yell.png", 
                                      Text = "Move Down")
            movedownl.ShortcutKeys <- Keys.Alt ||| Keys.Down
            //movedownl.Click.Add(domovedownline)
            ln.DropDownItems.Add(movedownl) |> ignore
            // do delete line
            //let dodelline (e) = stt.DelLine()
            let dell = new ToolStripMenuItem("Delete")
            //dell.ShortcutKeys <- Keys.Control ||| Keys.L
            //dell.Click.Add(dodelline)
            ln.DropDownItems.Add(dell) |> ignore
            // add separator
            ln.DropDownItems.Add(new ToolStripSeparator()) |> ignore
            // do set NAG
            let setnag =
                new ToolStripMenuItem(Image = img "plus-minus-32.png", 
                                      Text = "Set Evaluation")
            //setnag.Click.Add
            //    (fun _ -> 
            //    if not (stt.Tel |> List.isEmpty) && stt.Mct > 0 then 
            //        (new DlgSetNAG()).ShowDialog() |> ignore)
            ln.DropDownItems.Add(setnag) |> ignore
            m.Items.Add(ln) |> ignore
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
            //nwb.Click.Add(fun _ -> (new DlgNewBk()).ShowDialog() |> ignore)
            ts.Items.Add(nwb) |> ignore
            //do open
            let opnb =
                new ToolStripButton(Image = img "opn.png", Text = "&Open")
            //opnb.Click.Add(fun _ -> (new DlgOpnBk()).ShowDialog() |> ignore)
            ts.Items.Add(opnb) |> ignore
            //do save
            //let dosaveb (e) = stt.SaveBook() |> ignore
            let savb =
                new ToolStripButton(Image = img "sav.png", Text = "&Save")
            //savb.Click.Add(dosaveb)
            ts.Items.Add(savb) |> ignore
            //do save as 
            let savab =
                new ToolStripButton(Image = img "sava.png", Text = "Save &As")
            //savab.Click.Add(fun _ -> (new DlgSaveAsBk()).ShowDialog() |> ignore)
            ts.Items.Add(savab) |> ignore
            //do generate html
            //let dogenhb (e) = stt.GenHTML() |> ignore
            let genhb =
                new ToolStripButton(Image = img "sava.png", 
                                    Text = "Generate &HTML")
            //genhb.Click.Add(dogenhb) |> ignore
            ts.Items.Add(genhb) |> ignore
            //add separator
            ts.Items.Add(new ToolStripSeparator()) |> ignore
            //do set as sub line
            //let dosubline (e) =
            //    let vid = stt.Vid
            //    if vid <> "" && not (vid.Contains("s")) then do stt.SetSub()
    
            let subl =
                new ToolStripButton(Image = img "down-32-red.png", 
                                    Text = "Set as Sub Line")
            //subl.Click.Add(dosubline)
            ts.Items.Add(subl) |> ignore
            //do set as not sub line
            //let domainline (e) =
            //    let vid = stt.Vid
            //    if vid <> "" && vid.Contains("s") then do stt.SetMain()
    
            let mainl =
                new ToolStripButton(Image = img "up-32-red.png", 
                                    Text = "Set as Main Line")
            //mainl.Click.Add(domainline)
            ts.Items.Add(mainl) |> ignore
            //do move up line
            //let domoveupline (e) =
            //    let vid = stt.Vid
            //    if vid <> "" then do stt.MoveUp()
    
            let moveupl =
                new ToolStripButton(Image = img "up-32-yell.png", 
                                    Text = "Move Up")
            //moveupl.Click.Add(domoveupline)
            ts.Items.Add(moveupl) |> ignore
            //do move down line
            //let domovedownline (e) =
            //    let vid = stt.Vid
            //    if vid <> "" then do stt.MoveDown()
    
            let movedownl =
                new ToolStripButton(Image = img "down-32-yell.png", 
                                    Text = "Move Down")
            //movedownl.Click.Add(domovedownline)
            ts.Items.Add(movedownl) |> ignore
            // do set NAG
            let setnag =
                new ToolStripMenuItem(Image = img "plus-minus-32.png", 
                                      Text = "Set Evaluation")
            //setnag.Click.Add
            //    (fun _ -> 
            //    if not (stt.Tel |> List.isEmpty) && stt.Mct > 0 then 
            //        (new DlgSetNAG()).ShowDialog() |> ignore)
            ts.Items.Add(setnag) |> ignore
            ts

        let pnl = new Panel(Dock=DockStyle.Top,Height=30)
        let nmlbl =
            new Label(Text = "Not loaded", Dock = DockStyle.Left, 
                      TextAlign = ContentAlignment.MiddleLeft,Width=400)
        let updb = new ToolStripButton(Text = "Update Description")
        let homeb = new ToolStripButton(Image = img "homeButton.png")
        let prevb = new ToolStripButton(Image = img "prevButton.png")
        let nextb = new ToolStripButton(Image = img "nextButton.png")
        let endb = new ToolStripButton(Image = img "endButton.png")
        let ts =
            new ToolStrip(Anchor = AnchorStyles.Right, 
                          GripStyle = ToolStripGripStyle.Hidden, 
                          Dock = DockStyle.None, Left = 100)

        let rtpnl = new Panel(Dock=DockStyle.Fill)
    
        // add chapter tab
        let addchap (nm, ch : Game) =
            ch|>pgn.SetGame
            nmlbl.Text <- nm
        // chapter renamed
        let chaprnm nm =
            nmlbl.Text <- nm

        
        do
            pgn|>rtpnl.Controls.Add
            [ updb; homeb; prevb; nextb; endb ] 
            |> List.iter (fun c -> ts.Items.Add(c) |> ignore)
            pnl.Controls.Add(ts)
            pnl.Controls.Add(nmlbl)
            pnl|>rtpnl.Controls.Add
            rtpnl|>this.Controls.Add
            bd|>this.Controls.Add
            tb|>this.Controls.Add
            mm|>this.Controls.Add

            //events
            stt.ChAdd |> Observable.add addchap
            stt.ChRnm |> Observable.add chaprnm
            stt.ChChg |> Observable.add addchap
            stt.Ornt |> Observable.add bd.Orient