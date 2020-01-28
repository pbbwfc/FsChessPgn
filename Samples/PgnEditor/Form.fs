﻿namespace PgnEditor

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
                this.Text <- "PGN Editor - " + pgnfil

        let svpgn() = gms.SavePgn()

        let svapgn() =
            let dlg = new SaveFileDialog(Filter = "pgn files (*.pgn)|*.pgn")
            if dlg.ShowDialog() = DialogResult.OK then
                let pgnfil = dlg.FileName
                gms.SaveAsPgn(pgnfil)
                this.Text <- "PGN Editor - " + pgnfil

        let nwgm() = gms.NewGame()
        
        let mm =
            let m = new MenuStrip()
            //do file menu
            let fl = new ToolStripMenuItem("File")
            //do open
            let opnb =
                new ToolStripMenuItem(Image = img "opn.png", Text = "&Open")
            opnb.Click.Add(fun _ -> ldpgn())
            fl.DropDownItems.Add(opnb) |> ignore
            //do save
            let savb =
                new ToolStripMenuItem(Image = img "sav.png", Text = "&Save")
            savb.Click.Add(fun _ -> svpgn())
            fl.DropDownItems.Add(savb) |> ignore
            //do save as 
            let savab =
                new ToolStripMenuItem(Image = img "sava.png", Text = "Save &As")
            savab.Click.Add(fun _ -> svapgn())
            fl.DropDownItems.Add(savab) |> ignore
            m.Items.Add(fl) |> ignore

            //do gamer menu
            let gm = new ToolStripMenuItem("Game")
            //do new
            let newg =
                new ToolStripMenuItem(Text = "&New Game")
            newg.Click.Add(fun _ -> nwgm())
            gm.DropDownItems.Add(newg) |> ignore
            m.Items.Add(gm) |> ignore
   
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
            let savb =
                new ToolStripButton(Image = img "sav.png", Text = "&Save")
            savb.Click.Add(fun _ -> svpgn())
            ts.Items.Add(savb) |> ignore
            //do save as 
            let savab =
                new ToolStripButton(Image = img "sava.png", Text = "Save &As")
            savab.Click.Add(fun _ -> svapgn())
            ts.Items.Add(savab) |> ignore
            ts

        let rtpnl = new Panel(Height=450,Width=1100,Dock=DockStyle.Left,BorderStyle=BorderStyle.FixedSingle)
        let tppnl = new Panel(Height=450,Dock=DockStyle.Top,BorderStyle=BorderStyle.FixedSingle)
        let lblpnl = new Panel(Height=30,Dock=DockStyle.Top,BorderStyle=BorderStyle.FixedSingle)
        let lbl = new Label(Text="Game Title",Width=600,TextAlign=ContentAlignment.MiddleCenter,Font = new Font(new FontFamily("Arial"), 14.0f))
        let btpnl = new Panel(Dock=DockStyle.Fill,BorderStyle=BorderStyle.FixedSingle)
        
        do
            pgn|>btpnl.Controls.Add
            lbl|>lblpnl.Controls.Add
            lblpnl|>btpnl.Controls.Add
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
