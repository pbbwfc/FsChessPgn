namespace FsChessPgn.Test

open FsChessPgn.Data

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type BoardTest()=
    let brd1 = Board.Start
    let mv1 = "e4"|>MoveUtil.Parse brd1

    [<TestMethod>]
    member this.Board_PieceMove() =
        let brd2 = brd1|>Board.PieceMove E2 E4
        let str = brd2|>Board.ToStr
        Assert.AreEqual("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1",str)

    [<TestMethod>]
    member this.Board_PieceAdd() =
        let brd2 = BrdEMP|>Board.PieceAdd A1 Piece.WRook
        let str = brd2|>Board.ToStr
        Assert.AreEqual("8/8/8/8/8/8/8/R7 w - - 0 0",str)
        Assert.AreEqual(Bitboard.A1,brd2.WtPrBds)
        Assert.AreEqual(Bitboard.A1,brd2.PieceLocationsAll)

    [<TestMethod>]
    member this.Board_PieceRemove() =
        let brd0 = BrdEMP|>Board.PieceAdd A1 Piece.WRook
        let brd2 = brd0|>Board.PieceRemove A1
        let str = brd2|>Board.ToStr
        Assert.AreEqual("8/8/8/8/8/8/8/8 w - - 0 0",str)
        Assert.AreEqual(Bitboard.Empty,brd2.WtPrBds)
        Assert.AreEqual(Bitboard.Empty,brd2.PieceLocationsAll)

    [<TestMethod>]
    member this.Board_PieceChange() =
        let brd2 = BrdEMP|>Board.PieceChange A1 Piece.WRook
        let str = brd2|>Board.ToStr
        Assert.AreEqual("8/8/8/8/8/8/8/R7 w - - 0 0",str)
        Assert.AreEqual(Bitboard.A1,brd2.WtPrBds)
        Assert.AreEqual(Bitboard.A1,brd2.PieceLocationsAll)

    [<TestMethod>]
    member this.Board_Sliders() =
        let rs = brd1|>Board.RookSliders
        let bs = brd1|>Board.BishopSliders
        Assert.AreEqual(Bitboard.A1 ||| Bitboard.A8 ||| Bitboard.D1 ||| Bitboard.D8 ||| Bitboard.H1 ||| Bitboard.H8,rs)
        Assert.AreEqual(Bitboard.C1 ||| Bitboard.C8 ||| Bitboard.D1 ||| Bitboard.D8 ||| Bitboard.F1 ||| Bitboard.F8,bs)

    [<TestMethod>]
    member this.Board_Attacks() =
        let ats = brd1|>Board.AttacksTo D2
        let atws = brd1|>Board.AttacksTo2 D2 Player.White
        let atbs = brd1|>Board.AttacksTo2 D2 Player.Black
        Assert.AreEqual(Bitboard.B1 ||| Bitboard.C1 ||| Bitboard.D1 ||| Bitboard.E1,ats)
        Assert.AreEqual(Bitboard.B1 ||| Bitboard.C1 ||| Bitboard.D1 ||| Bitboard.E1,atws)
        Assert.AreEqual(Bitboard.Empty,atbs)

    [<TestMethod>]
    member this.Board_PositionAttacked() =
        let pas = brd1|>Board.PositionAttacked D2 Player.White
        Assert.AreEqual(true,pas)

    [<TestMethod>]
    member this.Board_MoveApply() =
        let brd2 = brd1|>Board.MoveApply mv1
        let str = brd2|>Board.ToStr
        Assert.AreEqual("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1",str)

    [<TestMethod>]
    member this.Board_IsCheck() =
        let ic = brd1|>Board.IsChk
        let ic2 = brd1|>Board.IsCheck Player.White
        Assert.AreEqual(false,ic)
        Assert.AreEqual(false,ic2)

    [<TestMethod>]
    member this.Board_PieceInDirection() =
        let pc,sq = brd1|>Board.PieceInDirection D2 Direction.DirN
        Assert.AreEqual(Piece.BPawn,pc)
        Assert.AreEqual(D7,sq)

    [<TestMethod>]
    member this.Board_FromFEN() =
        let fen = FEN.Parse "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1"
        let brd2 = Board.FromFEN fen
        let str = brd2|>Board.ToStr
        Assert.AreEqual("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1",str)
