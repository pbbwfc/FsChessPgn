namespace FsChess.WinForms

open System.Windows.Forms

[<AutoOpen>]
module Library3 =
    let CreateLnkBrdPgn() =
        let bd = new PnlBoard(Dock=DockStyle.Left)
        let pgn = new WbPgn(Dock=DockStyle.Fill)

        pgn.BdChng |> Observable.add bd.SetBoard
        bd.MvMade|>Observable.add pgn.DoMove
        bd,pgn
