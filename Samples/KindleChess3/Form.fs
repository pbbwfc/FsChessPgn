namespace FsChess.WinForms

open System
open System.Drawing
open System.Windows.Forms
open FsChess.Pgn

module Form =
    let ico nm =
        let thisExe = System.Reflection.Assembly.GetExecutingAssembly()
        let file = thisExe.GetManifestResourceStream("KindleChess3.Icons." + nm)
        new Icon(file)
    
    
    type FrmMain() as this =
        inherit Form(Text = "Kindle Chess", WindowState = FormWindowState.Maximized, Icon = ico "KindleChess.ico", IsMdiContainer = true)

        let mutable gms = []
        let mutable ct = 0

        let bd,pgn = CreateLnkBrdPgn()
        let rtpnl = new Panel(Dock=DockStyle.Fill)
        let rbpnl = new Panel(Dock=DockStyle.Bottom,Height=30)
        let ldbtn = new Button(Text="Load",Dock = DockStyle.Right)
        let nxbtn = new Button(Text="Game >",Dock = DockStyle.Right)
        let pvbtn = new Button(Text="< Game",Dock = DockStyle.Right)
        let nxmbtn = new Button(Text="Move >",Dock = DockStyle.Right)
        let pvmbtn = new Button(Text="< Move",Dock = DockStyle.Right)

        let ldpgn() =
            let dlg = new OpenFileDialog(Filter = "pgn files (*.pgn)|*.pgn")
            if dlg.ShowDialog() = DialogResult.OK then
                let pgnfil = dlg.FileName
                gms <- Games.ReadListFromFile(pgnfil)
                ct <- 0
                pgn.SetGame(gms.[ct])  
        let nxt() =
            if ct<gms.Length-1 then
                ct <- ct+1
                pgn.SetGame(gms.[ct])
        let prv() =
            if ct>0 then
                ct <- ct-1
                pgn.SetGame(gms.[ct])
        let nxtm() =
            pgn.NextMove()
        let prvm() =
            pgn.PrevMove()
    
        do
            pgn|>rtpnl.Controls.Add
            ldbtn |>rbpnl.Controls.Add
            pvbtn |>rbpnl.Controls.Add
            nxbtn |>rbpnl.Controls.Add
            pvmbtn |>rbpnl.Controls.Add
            nxmbtn |>rbpnl.Controls.Add
            rbpnl|>rtpnl.Controls.Add
            rtpnl|>this.Controls.Add
            bd|>this.Controls.Add
            //Buttons
            ldbtn.Click.Add(fun e -> ldpgn())
            nxbtn.Click.Add(fun e -> nxt())
            pvbtn.Click.Add(fun e -> prv())
            nxmbtn.Click.Add(fun e -> nxtm())
            pvmbtn.Click.Add(fun e -> prvm())

