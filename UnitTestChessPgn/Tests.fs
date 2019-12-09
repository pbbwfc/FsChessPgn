namespace UnitTestChessPgn

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open FsChess

[<TestClass>]
type SquareTestCase () =

    [<TestMethod>]
    member this.test_square () =
        for square in chess.SQUARES do
            let file_index = chess.square_file(square)
            let rank_index = chess.square_rank(square)
            Assert.AreEqual(chess.square(file_index, rank_index), square,chess.square_name(square))
