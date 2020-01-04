namespace FsChess.WinForms

open System
open System.Windows.Forms
open System.Drawing

module Main =
    [<STAThread>]
    Application.EnableVisualStyles()
    
    type FrmMain() as this =
        inherit Form(Text = "Show Pgn", Width = 910, Height = 420)

        let bd = new PnlBoard(Dock=DockStyle.Left)
        let rtpnl = new Panel(Dock=DockStyle.Fill)
        let rbpnl = new Panel(Dock=DockStyle.Bottom,Height=30)
        let ldbtn = new Button(Text="Load",Dock = DockStyle.Right)
        let nxbtn = new Button(Text="Next",Dock = DockStyle.Right)
        let pvbtn = new Button(Text="Prev",Dock = DockStyle.Right)
        let pgn = new RchPgn(Dock=DockStyle.Fill)

        do
            pgn|>rtpnl.Controls.Add
            ldbtn |>rbpnl.Controls.Add
            nxbtn |>rbpnl.Controls.Add
            pvbtn |>rbpnl.Controls.Add
            rbpnl|>rtpnl.Controls.Add
            rtpnl|>this.Controls.Add
            bd|>this.Controls.Add
    
    let frm = new FrmMain()
    
    Application.Run(frm)