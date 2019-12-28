namespace FsChessPgn.Test

open FsChess
open FsChessPgn

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type MoveTest()=
    let brd1 = Board.Start
    let mv1 = "e4"|>MoveUtil.fromSAN brd1

    [<TestMethod>]
    member this.Move_Utils() =
        Assert.AreEqual(E2,mv1|>Move.From)
        Assert.AreEqual(E4,mv1|>Move.To)
        Assert.AreEqual(Piece.WPawn,mv1|>Move.MovingPiece)
        Assert.AreEqual(true,mv1|>Move.IsW)
        Assert.AreEqual(PieceType.Pawn,mv1|>Move.MovingPieceType)
        Assert.AreEqual(Player.White,mv1|>Move.MovingPlayer)
        Assert.AreEqual(false,mv1|>Move.IsCapture)
        Assert.AreEqual(Piece.EMPTY,mv1|>Move.CapturedPiece)
        Assert.AreEqual(PieceType.EMPTY,mv1|>Move.CapturedPieceType)
        Assert.AreEqual(false,mv1|>Move.IsPromotion)
        Assert.AreEqual(PieceType.EMPTY,mv1|>Move.PromoteType)
        Assert.AreEqual(Piece.EMPTY,mv1|>Move.Promote)
        Assert.AreEqual(false,mv1|>Move.IsEnPassant)
        Assert.AreEqual(false,mv1|>Move.IsCastle)
        Assert.AreEqual(true,mv1|>Move.IsPawnDoubleJump)

    