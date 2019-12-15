namespace FsChessPgn.Test

open FsChessPgn.Data

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type BoardSetupTest () =

    [<TestMethod>]
    member this.Index_accessor_should_set_and_get_Pieces() =
        let sut = FEN.Start()
        sut.Pieceat.[33] <- Piece.BKnight

        Assert.AreEqual(Piece.BKnight, sut.Pieceat.[33])
 
    [<TestMethod>]
    member this.square_number_correspondation_test() =
        let sut = FEN.Start()
        sut.Pieceat.[0] <- Piece.BPawn
        sut.Pieceat.[1] <- Piece.WKing
        sut.Pieceat.[59] <- Piece.BRook
        sut.Pieceat.[36] <- Piece.WQueen
        sut.Pieceat.[63] <- Piece.BBishop
        let sq = Sq(FileA, Rank1)
        Assert.AreEqual(Piece.BPawn, sut.Pieceat.[int(Sq(FileA, Rank8))])
        Assert.AreEqual(Piece.WKing, sut.Pieceat.[int(Sq(FileB, Rank8))])
        Assert.AreEqual(Piece.BRook, sut.Pieceat.[int(Sq(FileD, Rank1))])
        Assert.AreEqual(Piece.WQueen, sut.Pieceat.[int(Sq(FileE, Rank4))])
        Assert.AreEqual(Piece.BBishop, sut.Pieceat.[int(Sq(FileH, Rank1))])
