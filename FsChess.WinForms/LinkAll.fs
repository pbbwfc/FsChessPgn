namespace FsChess.WinForms

open System.Windows.Forms

[<AutoOpen>]
module Library6 =
    let CreateLnkAll() =
        let bd = new PnlBoard(Dock=DockStyle.Left)
        let pgn = new WbPgn(Dock=DockStyle.Fill)
        let sts = new WbStats(Dock=DockStyle.Top)
        let gms = new DgvGames(Dock=DockStyle.Top)

        pgn.BdChng |> Observable.add bd.SetBoard
        bd.MvMade|>Observable.add pgn.DoMove
        bd.BdChng |> Observable.add gms.SetBoard
        pgn.BdChng |> Observable.add gms.SetBoard
        gms.FiltChng |> Observable.add sts.CalcStats
        sts.MvSel |> Observable.add bd.DoMove
        gms.GmSel |> Observable.add pgn.SwitchGame
        pgn.GmChng |> Observable.add gms.ChangeGame
        pgn.HdrChng |> Observable.add gms.ChangeGameHdr
        gms.PgnChng |> Observable.add bd.SetBoard 

        bd,pgn,gms,sts
