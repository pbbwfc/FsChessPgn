namespace FsChess.WinForms

open System
open System.Windows.Forms
open FsChess.Pgn

module Main =
    [<STAThread>]
    Application.EnableVisualStyles()
    
    type FrmMain() as this =
        inherit Form(Text = "Show Pgn", Width = 910, Height = 420)

        let mutable gms = []
        let mutable ct = 0

        let bd = new PnlBoard(Dock=DockStyle.Left)
        let rtpnl = new Panel(Dock=DockStyle.Fill)
        let rbpnl = new Panel(Dock=DockStyle.Bottom,Height=30)
        let ldbtn = new Button(Text="Load",Dock = DockStyle.Right)
        let nxbtn = new Button(Text="Next",Dock = DockStyle.Right)
        let pvbtn = new Button(Text="Prev",Dock = DockStyle.Right)
        let pgn = new WbPgn(Dock=DockStyle.Fill)

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
        
        do
            pgn|>rtpnl.Controls.Add
            ldbtn |>rbpnl.Controls.Add
            nxbtn |>rbpnl.Controls.Add
            pvbtn |>rbpnl.Controls.Add
            rbpnl|>rtpnl.Controls.Add
            rtpnl|>this.Controls.Add
            bd|>this.Controls.Add
            //Buttons
            ldbtn.Click.Add(fun e -> ldpgn())
            nxbtn.Click.Add(fun e -> nxt())
            pvbtn.Click.Add(fun e -> prv())
    
    let frm = new FrmMain()
    
    Application.Run(frm)