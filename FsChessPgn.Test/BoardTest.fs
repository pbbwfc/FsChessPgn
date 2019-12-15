namespace FsChessPgn.Test

open FsChessPgn.Data

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type BoardTest()=
    let brd1 = Board.Start
    let mv1 = "e4"|>MoveUtil.Parse brd1

    [<TestMethod>]
    member this.Board_Utils() =
        Assert.AreEqual(E2,mv1|>Board.From)
        Assert.AreEqual(E4,mv1|>Board.To)
        Assert.AreEqual(Piece.WPawn,mv1|>Board.MovingPiece)
        Assert.AreEqual(true,mv1|>Board.IsW)
        Assert.AreEqual(PieceType.Pawn,mv1|>Board.MovingPieceType)
        Assert.AreEqual(Player.White,mv1|>Board.MovingPlayer)
        Assert.AreEqual(false,mv1|>Board.IsCapture)
        Assert.AreEqual(Piece.EMPTY,mv1|>Board.CapturedPiece)
        Assert.AreEqual(PieceType.EMPTY,mv1|>Board.CapturedPieceType)
        Assert.AreEqual(false,mv1|>Board.IsPromotion)
        Assert.AreEqual(PieceType.EMPTY,mv1|>Board.PromoteType)
        Assert.AreEqual(Piece.EMPTY,mv1|>Board.Promote)
        Assert.AreEqual(false,mv1|>Board.IsEnPassant)
        Assert.AreEqual(false,mv1|>Board.IsCastle)
        Assert.AreEqual(true,mv1|>Board.IsPawnDoubleJump)

    [<TestMethod>]
    member this.Board_PieceMove() =
        let brd2 = brd1|>Board.PieceMove E2 E4
        let str = brd2|>FEN.FromBd|>FEN.ToStr
        Assert.AreEqual("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1",str)
