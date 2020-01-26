namespace FsChess.WinForms

open System.Windows.Forms

[<AutoOpen>]
module Library6 =
    let CreateLnkAll() =
        let bd = new PnlBoard(Dock=DockStyle.Left)
        let pgn = new WbPgn(Dock=DockStyle.Fill)
        let sts = new WbStats(Dock=DockStyle.Bottom)
        let gms = new DgvGames(Dock=DockStyle.Fill)

        pgn.BdChng |> Observable.add bd.SetBoard
        bd.MvMade|>Observable.add pgn.DoMove
        bd.BdChng |> Observable.add gms.SetBoard
        gms.FiltChng |> Observable.add sts.CalcStats

        bd,pgn,gms,sts
