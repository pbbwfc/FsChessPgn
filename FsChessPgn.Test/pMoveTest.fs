#nowarn "25"
namespace FsChessPgn.Test

open FsChess
open FsChessPgn

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type pMoveTest()=

    let NormalGame = @"
[Event ""Breslau""]
[Site ""Breslau""]
[Date ""1879.??.??""]
[Round ""?""]
[White ""Tarrasch, Siegbert""]
[Black ""Mendelsohn, J.""]
[Result ""1-0""]
[WhiteElo """"]
[BlackElo """"]
[ECO ""C49""]

1.e4 e5 2.Nf3 Nc6 3.Nc3 Nf6 4.Bb5 Bb4 5.Nd5 Nxd5 6.exd5 Nd4 7.Ba4 b5 8.Nxd4 bxa4
9.Nf3 O-O 10.O-O d6 11.c3 Bc5 12.d4 exd4 13.Nxd4 Ba6 14.Re1 Bc4 15.Nc6 Qf6
16.Be3 Rfe8 17.Bxc5 Rxe1+ 18.Qxe1 dxc5 19.Qe4 Bb5 20.d6 Kf8 21.Ne7 Re8 22.Qxh7 Qxd6
23.Re1 Be2 24.Nf5  1-0"
    let gm = NormalGame|>Games.ReadFromString|>List.head

    [<TestMethod>]
    member this.pMove_king_castle_move() =
        let bd = "r1bq1rk1/p1pp1ppp/8/3Pp3/pb6/5N2/PPPP1PPP/R1BQK2R w KQ - 2 10"|>Board.FromStr
        let mvte1 = gm.MoveText.[18]
        let (HalfMoveEntry (_,_,mv,_)) = mvte1
        let amv=mv|>pMove.ToaMove bd 1
        let mv1=amv.Mv
        Assert.AreEqual(E1,mv1|>Move.From)
        Assert.AreEqual(G1,mv1|>Move.To)
        Assert.AreEqual(Piece.WKing,mv1|>Move.MovingPiece)
        Assert.AreEqual(true,mv1|>Move.IsW)
        Assert.AreEqual(PieceType.King,mv1|>Move.MovingPieceType)
        Assert.AreEqual(Player.White,mv1|>Move.MovingPlayer)
        Assert.AreEqual(false,mv1|>Move.IsCapture)
        Assert.AreEqual(Piece.EMPTY,mv1|>Move.CapturedPiece)
        Assert.AreEqual(PieceType.EMPTY,mv1|>Move.CapturedPieceType)
        Assert.AreEqual(false,mv1|>Move.IsPromotion)
        Assert.AreEqual(PieceType.EMPTY,mv1|>Move.PromoteType)
        Assert.AreEqual(Piece.EMPTY,mv1|>Move.Promote)
        Assert.AreEqual(false,mv1|>Move.IsEnPassant)
        Assert.AreEqual(true,mv1|>Move.IsCastle)
        Assert.AreEqual(false,mv1|>Move.IsPawnDoubleJump)

    [<TestMethod>]
    member this.pMove_simple_pawn_move() =
        let bd = Board.Start
        let mvte1 = gm.MoveText.Head
        let (HalfMoveEntry (mn,ic,mv,_)) = mvte1
        let amv=mv|>pMove.ToaMove bd 1
        let mv1=amv.Mv
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

    [<TestMethod>]
    member this.pMove_simple_knight_move() =
        let bd = "rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 2"|>Board.FromStr
        let mvte1 = gm.MoveText.[2]
        let (HalfMoveEntry (_,_,mv,_)) = mvte1
        let amv=mv|>pMove.ToaMove bd 1
        let mv1=amv.Mv
        Assert.AreEqual(G1,mv1|>Move.From)
        Assert.AreEqual(F3,mv1|>Move.To)
        Assert.AreEqual(Piece.WKnight,mv1|>Move.MovingPiece)
        Assert.AreEqual(true,mv1|>Move.IsW)
        Assert.AreEqual(PieceType.Knight,mv1|>Move.MovingPieceType)
        Assert.AreEqual(Player.White,mv1|>Move.MovingPlayer)
        Assert.AreEqual(false,mv1|>Move.IsCapture)
        Assert.AreEqual(Piece.EMPTY,mv1|>Move.CapturedPiece)
        Assert.AreEqual(PieceType.EMPTY,mv1|>Move.CapturedPieceType)
        Assert.AreEqual(false,mv1|>Move.IsPromotion)
        Assert.AreEqual(PieceType.EMPTY,mv1|>Move.PromoteType)
        Assert.AreEqual(Piece.EMPTY,mv1|>Move.Promote)
        Assert.AreEqual(false,mv1|>Move.IsEnPassant)
        Assert.AreEqual(false,mv1|>Move.IsCastle)
        Assert.AreEqual(false,mv1|>Move.IsPawnDoubleJump)

    [<TestMethod>]
    member this.pMove_simple_bishop_move() =
        let bd = "r1bqkb1r/pppp1ppp/2n2n2/4p3/4P3/2N2N2/PPPP1PPP/R1BQKB1R w KQkq - 4 4"|>Board.FromStr
        let mvte1 = gm.MoveText.[6]
        let (HalfMoveEntry (_,_,mv,_)) = mvte1
        let amv=mv|>pMove.ToaMove bd 1
        let mv1=amv.Mv
        Assert.AreEqual(F1,mv1|>Move.From)
        Assert.AreEqual(B5,mv1|>Move.To)
        Assert.AreEqual(Piece.WBishop,mv1|>Move.MovingPiece)
        Assert.AreEqual(true,mv1|>Move.IsW)
        Assert.AreEqual(PieceType.Bishop,mv1|>Move.MovingPieceType)
        Assert.AreEqual(Player.White,mv1|>Move.MovingPlayer)
        Assert.AreEqual(false,mv1|>Move.IsCapture)
        Assert.AreEqual(Piece.EMPTY,mv1|>Move.CapturedPiece)
        Assert.AreEqual(PieceType.EMPTY,mv1|>Move.CapturedPieceType)
        Assert.AreEqual(false,mv1|>Move.IsPromotion)
        Assert.AreEqual(PieceType.EMPTY,mv1|>Move.PromoteType)
        Assert.AreEqual(Piece.EMPTY,mv1|>Move.Promote)
        Assert.AreEqual(false,mv1|>Move.IsEnPassant)
        Assert.AreEqual(false,mv1|>Move.IsCastle)
        Assert.AreEqual(false,mv1|>Move.IsPawnDoubleJump)

    [<TestMethod>]
    member this.pMove_simple_rook_move() =
        let bd = "r2q1rk1/p1p2ppp/b2p4/2bP4/p2N4/2P5/PP3PPP/R1BQ1RK1 w - - 1 14"|>Board.FromStr
        let mvte1 = gm.MoveText.[26]
        let (HalfMoveEntry (_,_,mv,_)) = mvte1
        let amv=mv|>pMove.ToaMove bd 1
        let mv1=amv.Mv
        Assert.AreEqual(F1,mv1|>Move.From)
        Assert.AreEqual(E1,mv1|>Move.To)
        Assert.AreEqual(Piece.WRook,mv1|>Move.MovingPiece)
        Assert.AreEqual(true,mv1|>Move.IsW)
        Assert.AreEqual(PieceType.Rook,mv1|>Move.MovingPieceType)
        Assert.AreEqual(Player.White,mv1|>Move.MovingPlayer)
        Assert.AreEqual(false,mv1|>Move.IsCapture)
        Assert.AreEqual(Piece.EMPTY,mv1|>Move.CapturedPiece)
        Assert.AreEqual(PieceType.EMPTY,mv1|>Move.CapturedPieceType)
        Assert.AreEqual(false,mv1|>Move.IsPromotion)
        Assert.AreEqual(PieceType.EMPTY,mv1|>Move.PromoteType)
        Assert.AreEqual(Piece.EMPTY,mv1|>Move.Promote)
        Assert.AreEqual(false,mv1|>Move.IsEnPassant)
        Assert.AreEqual(false,mv1|>Move.IsCastle)
        Assert.AreEqual(false,mv1|>Move.IsPawnDoubleJump)

    [<TestMethod>]
    member this.pMove_simple_queen_move() =
        let bd = "r2q1rk1/p1p2ppp/2Np4/2bP4/p1b5/2P5/PP3PPP/R1BQR1K1 b - - 4 15"|>Board.FromStr
        let mvte1 = gm.MoveText.[29]
        let (HalfMoveEntry (_,_,mv,_)) = mvte1
        let amv=mv|>pMove.ToaMove bd 1
        let mv1=amv.Mv
        Assert.AreEqual(D8,mv1|>Move.From)
        Assert.AreEqual(F6,mv1|>Move.To)
        Assert.AreEqual(Piece.BQueen,mv1|>Move.MovingPiece)
        Assert.AreEqual(false,mv1|>Move.IsW)
        Assert.AreEqual(PieceType.Queen,mv1|>Move.MovingPieceType)
        Assert.AreEqual(Player.Black,mv1|>Move.MovingPlayer)
        Assert.AreEqual(false,mv1|>Move.IsCapture)
        Assert.AreEqual(Piece.EMPTY,mv1|>Move.CapturedPiece)
        Assert.AreEqual(PieceType.EMPTY,mv1|>Move.CapturedPieceType)
        Assert.AreEqual(false,mv1|>Move.IsPromotion)
        Assert.AreEqual(PieceType.EMPTY,mv1|>Move.PromoteType)
        Assert.AreEqual(Piece.EMPTY,mv1|>Move.Promote)
        Assert.AreEqual(false,mv1|>Move.IsEnPassant)
        Assert.AreEqual(false,mv1|>Move.IsCastle)
        Assert.AreEqual(false,mv1|>Move.IsPawnDoubleJump)

    [<TestMethod>]
    member this.pMove_simple_king_move() =
        let bd = "r5k1/p1p2ppp/2NP1q2/1bp5/p3Q3/2P5/PP3PPP/R5K1 b - - 0 20"|>Board.FromStr
        let mvte1 = gm.MoveText.[39]
        let (HalfMoveEntry (_,_,mv,_)) = mvte1
        let amv=mv|>pMove.ToaMove bd 1
        let mv1=amv.Mv
        Assert.AreEqual(G8,mv1|>Move.From)
        Assert.AreEqual(F8,mv1|>Move.To)
        Assert.AreEqual(Piece.BKing,mv1|>Move.MovingPiece)
        Assert.AreEqual(false,mv1|>Move.IsW)
        Assert.AreEqual(PieceType.King,mv1|>Move.MovingPieceType)
        Assert.AreEqual(Player.Black,mv1|>Move.MovingPlayer)
        Assert.AreEqual(false,mv1|>Move.IsCapture)
        Assert.AreEqual(Piece.EMPTY,mv1|>Move.CapturedPiece)
        Assert.AreEqual(PieceType.EMPTY,mv1|>Move.CapturedPieceType)
        Assert.AreEqual(false,mv1|>Move.IsPromotion)
        Assert.AreEqual(PieceType.EMPTY,mv1|>Move.PromoteType)
        Assert.AreEqual(Piece.EMPTY,mv1|>Move.Promote)
        Assert.AreEqual(false,mv1|>Move.IsEnPassant)
        Assert.AreEqual(false,mv1|>Move.IsCastle)
        Assert.AreEqual(false,mv1|>Move.IsPawnDoubleJump)

    [<TestMethod>]
    member this.pMove_complex_rook_move() =
        let bd = "r4rk1/p1p2ppp/2Np1q2/2bP4/p1b5/2P1B3/PP3PPP/R2QR1K1 b - - 6 16"|>Board.FromStr
        let mvte1 = gm.MoveText.[31]
        let (HalfMoveEntry (_,_,mv,_)) = mvte1
        let amv=mv|>pMove.ToaMove bd 1
        let mv1=amv.Mv
        Assert.AreEqual(F8,mv1|>Move.From)
        Assert.AreEqual(E8,mv1|>Move.To)
        Assert.AreEqual(Piece.BRook,mv1|>Move.MovingPiece)
        Assert.AreEqual(false,mv1|>Move.IsW)
        Assert.AreEqual(PieceType.Rook,mv1|>Move.MovingPieceType)
        Assert.AreEqual(Player.Black,mv1|>Move.MovingPlayer)
        Assert.AreEqual(false,mv1|>Move.IsCapture)
        Assert.AreEqual(Piece.EMPTY,mv1|>Move.CapturedPiece)
        Assert.AreEqual(PieceType.EMPTY,mv1|>Move.CapturedPieceType)
        Assert.AreEqual(false,mv1|>Move.IsPromotion)
        Assert.AreEqual(PieceType.EMPTY,mv1|>Move.PromoteType)
        Assert.AreEqual(Piece.EMPTY,mv1|>Move.Promote)
        Assert.AreEqual(false,mv1|>Move.IsEnPassant)
        Assert.AreEqual(false,mv1|>Move.IsCastle)
        Assert.AreEqual(false,mv1|>Move.IsPawnDoubleJump)
