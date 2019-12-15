namespace FsChessPgn.Test

open FsChessPgn.Data

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type BoardSetupTest () =

    [<TestMethod>]
    member this.Index_accessor_should_set_and_get_Pieces() =
        let isut = FEN.Start()
        let upd33 i p = if i=33 then Piece.BKnight else p 
        let sut = {isut with Pieceat=isut.Pieceat|>List.mapi upd33}

        Assert.AreEqual(Piece.BKnight, sut.Pieceat.[33])
 
    [<TestMethod>]
    member this.square_number_correspondation_test() =
        let isut = FEN.Start()
        let upd i p =
            if i=0 then Piece.BPawn
            elif i=1 then Piece.WKing
            elif i=59 then Piece.BRook
            elif i=36 then Piece.WQueen
            elif i=63 then Piece.BBishop
            else p
        let sut = {isut with Pieceat=isut.Pieceat|>List.mapi upd}

        Assert.AreEqual(Piece.BPawn, sut.Pieceat.[Sq(FileA, Rank8)])
        Assert.AreEqual(Piece.WKing, sut.Pieceat.[Sq(FileB, Rank8)])
        Assert.AreEqual(Piece.BRook, sut.Pieceat.[Sq(FileD, Rank1)])
        Assert.AreEqual(Piece.WQueen, sut.Pieceat.[Sq(FileE, Rank4)])
        Assert.AreEqual(Piece.BBishop, sut.Pieceat.[Sq(FileH, Rank1)])
