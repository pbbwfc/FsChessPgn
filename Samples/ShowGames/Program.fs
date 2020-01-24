namespace FsChess.WinForms

open System
open System.Windows.Forms
open FsChess.Pgn

module Main =
    [<STAThread>]
    Application.EnableVisualStyles()
    
    type FrmMain() as this =
        inherit Form(Text = "Show Games", Width = 900, Height = 470)

        let gms = new DgvGames()
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
            gms|>rtpnl.Controls.Add
            ldbtn |>rbpnl.Controls.Add
            rbpnl|>rtpnl.Controls.Add
            rtpnl|>this.Controls.Add
            bd|>this.Controls.Add
            //Buttons
            ldbtn.Click.Add(fun e -> ldpgn())
            //events
            bd.BdChng |> Observable.add gms.SetBoard

    
    let frm = new FrmMain()
    
    Application.Run(frm)