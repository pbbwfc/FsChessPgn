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

        let ldpgn() =
            let dlg = new OpenFileDialog(Filter = "pgn files (*.pgn)|*.pgn")
            if dlg.ShowDialog() = DialogResult.OK then
                let pgnfil = dlg.FileName
                gms.SetPgn(pgnfil)

        let mm =
            let m = new MenuStrip()
            //do file menu
            let bk = new ToolStripMenuItem("File")
            //do open
            let opnb =
                new ToolStripMenuItem(Image = img "opn.png", Text = "&Open")
            opnb.Click.Add(fun _ -> ldpgn())
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
            opnb.Click.Add(fun _ -> ldpgn())
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

        let rtpnl = new Panel(Height=450,Width=1100,Dock=DockStyle.Left)
        let tppnl = new Panel(Height=450,Dock=DockStyle.Top)
        let lblpnl = new Panel(Height=30,Dock=DockStyle.Top)
        let lbl = new Label(Text="Game Title",Width=200)
        let btpnl = new Panel(Dock=DockStyle.Fill)
        
        do
            lbl|>lblpnl.Controls.Add
            lblpnl|>btpnl.Controls.Add
            pgn|>btpnl.Controls.Add
            btpnl|>this.Controls.Add
            sts.Height <- 200
            gms.Height <- 250
            gms|>rtpnl.Controls.Add
            sts|>rtpnl.Controls.Add
            rtpnl|>tppnl.Controls.Add
            bd|>tppnl.Controls.Add
            tppnl|>this.Controls.Add
            tb|>this.Controls.Add
            mm|>this.Controls.Add
            

            //events
            gms.GmSel|>Observable.add (fun gm -> lbl.Text <- gm.WhitePlayer + " vs. " + gm.BlackPlayer) 
