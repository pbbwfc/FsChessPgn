namespace FsChess.WinForms

open System
open System.Windows.Forms
open FsChess.Pgn

module Main =
    [<STAThread>]
    Application.EnableVisualStyles()
    
    type FrmMain() as this =
        inherit Form(Text = "Show Stats", Width = 1000, Height = 850)

        let sts = new WbStats(Dock=DockStyle.Fill) 
        let gms = new DgvGames(Dock=DockStyle.Bottom)
        let bd = new PnlBoard(Dock=DockStyle.Left)
        let rtpnl = new Panel(Dock=DockStyle.Fill)
        let rbpnl = new Panel(Dock=DockStyle.Bottom,Height=30)
        let ldbtn = new Button(Text="Load",Dock = DockStyle.Right)


        let ldpgn() =
            let dlg = new OpenFileDialog(Filter = "pgn files (*.pgn)|*.pgn")
            if dlg.ShowDialog() = DialogResult.OK then
                let pgnfil = dlg.FileName
                gms.SetPgn(pgnfil)


        do
            ldbtn |>rbpnl.Controls.Add
            rbpnl|>rtpnl.Controls.Add
            sts|>rtpnl.Controls.Add
            rtpnl|>this.Controls.Add
            bd|>this.Controls.Add
            gms|>this.Controls.Add
            //Buttons
            ldbtn.Click.Add(fun e -> ldpgn())
            //events
            bd.BdChng |> Observable.add gms.SetBoard
            gms.FiltChng |> Observable.add sts.CalcStats
    
    let frm = new FrmMain()
    
    Application.Run(frm)