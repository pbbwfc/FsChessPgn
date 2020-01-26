namespace PgnEditor

open System
open System.Drawing
open System.Windows.Forms
open FsChess
open FsChess.WinForms

module Form =
    let img nm =
        let thisExe = System.Reflection.Assembly.GetExecutingAssembly()
        let file = thisExe.GetManifestResourceStream("PgnEditor.Images." + nm)
        Image.FromStream(file)
    let ico nm =
        let thisExe = System.Reflection.Assembly.GetExecutingAssembly()
        let file = thisExe.GetManifestResourceStream("PgnEditor.Icons." + nm)
        new Icon(file)

    type FrmMain() as this =
        inherit Form(Text = "PGN Editor", WindowState = FormWindowState.Maximized, Icon = ico "KindleChess.ico", IsMdiContainer = true)

        
        let bd,pgn,gms,sts = CreateLnkAll()

        let mm =
            let m = new MenuStrip()
            //do book menu
            let bk = new ToolStripMenuItem("File")
            //do open
            let opnb =
                new ToolStripMenuItem(Image = img "opn.png", Text = "&Open")
            //opnb.Click.Add(fun _ -> (new DlgOpnBk()).ShowDialog() |> ignore)
            bk.DropDownItems.Add(opnb) |> ignore
            //do save
            //let dosaveb (e) = 
            //    let cho = if stt.Chi>=0 then pgn.GetGame()|>Some else None
            //    stt.SaveBook(cho) |> ignore
            let savb =
                new ToolStripMenuItem(Image = img "sav.png", Text = "&Save")
            //savb.Click.Add(dosaveb)
            bk.DropDownItems.Add(savb) |> ignore
            m.Items.Add(bk) |> ignore
            //do save as 
            let savab =
                new ToolStripMenuItem(Image = img "sava.png", Text = "Save &As")
            //savab.Click.Add(fun _ -> (new DlgSaveAsBk()).ShowDialog() |> ignore)
            bk.DropDownItems.Add(savab) |> ignore

            //do chapter menu
            let chp = new ToolStripMenuItem("Game")
   
            //do about menu
            let abt = new ToolStripMenuItem("About")
            //do docs
            let onl = new ToolStripMenuItem("Online Documentation")
            onl.Click.Add
                (fun _ -> 
                System.Diagnostics.Process.Start
                    ("https://pbbwfc.github.io/FsChessPgn/") |> ignore)
            abt.DropDownItems.Add(onl) |> ignore
            //do source code
            let src = new ToolStripMenuItem("Source Code")
            src.Click.Add
                (fun _ -> 
                System.Diagnostics.Process.Start
                    ("https://github.com/pbbwfc/FsChessPgn") |> ignore)
            abt.DropDownItems.Add(src) |> ignore
            m.Items.Add(abt) |> ignore
            m

        let tb =
            let ts = new ToolStrip()
            //do open
            let opnb =
                new ToolStripButton(Image = img "opn.png", Text = "&Open")
            //opnb.Click.Add(fun _ -> (new DlgOpnBk()).ShowDialog() |> ignore)
            ts.Items.Add(opnb) |> ignore
            //do save
            //let dosaveb (e) = 
            //    let cho = if stt.Chi>=0 then pgn.GetGame()|>Some else None
            //    stt.SaveBook(cho) |> ignore
            let savb =
                new ToolStripButton(Image = img "sav.png", Text = "&Save")
            //savb.Click.Add(dosaveb)
            ts.Items.Add(savb) |> ignore
            //do save as 
            let savab =
                new ToolStripButton(Image = img "sava.png", Text = "Save &As")
            //savab.Click.Add(fun _ -> (new DlgSaveAsBk()).ShowDialog() |> ignore)
            ts.Items.Add(savab) |> ignore
            ts

        let pnl = new Panel(Dock=DockStyle.Top,Height=30)
        let rtpnl = new Panel(Dock=DockStyle.Fill)
        
        do
            pgn|>rtpnl.Controls.Add
            rtpnl|>this.Controls.Add
            bd|>this.Controls.Add
            tb|>this.Controls.Add
            mm|>this.Controls.Add
            //events
            //TODO 
