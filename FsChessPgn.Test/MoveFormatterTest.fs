namespace fspgn.Test

open System.IO
open fspgn.Data
open fspgn.Data.PgnTextTypes

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type MoveFormatterTest()=

    [<TestMethod>]
    member this.Format_should_accept_TextWriter() =
        let writer = new StringWriter()
        writer.Write("Foo ")
        let move = pMoveCreate(pMoveType.Simple,None,Sq(FileC,Rank5),None,PieceType.Rook|>Some)
        Formatter.FormatMove(move, writer)

        Assert.AreEqual("Foo Rc5", writer.ToString())

    [<TestMethod>]
    member this.Format_should_format_castling_moves() =
        let writer = new StringWriter()
        let mv1 = pMoveCreateCastle(pMoveType.CastleKingSide)
        let mv2 = pMoveCreateCastle(pMoveType.CastleQueenSide)

        Assert.AreEqual("O-O", Formatter.FormatMoveStr(mv1))
        Assert.AreEqual("O-O-O", Formatter.FormatMoveStr(mv2))

    [<TestMethod>]
    member this.Format_should_format_simple_target_only_move() =
        let move = pMoveCreate(pMoveType.Simple,None,Sq(FileC,Rank5),None,PieceType.Rook|>Some)
        let str = Formatter.FormatMoveStr(move)

        Assert.AreEqual("Rc5", str)

    [<TestMethod>]
    member this.Format_should_format_simple_pawn_move() =
        let move = pMoveCreate(pMoveType.Simple,None,Sq(FileC,Rank5),None,None)
        let str = Formatter.FormatMoveStr(move)

        Assert.AreEqual("c5", str)

    [<TestMethod>]
    member this.Format_should_format_simple_pawn_move_with_explict_pawn() =
        let move = pMoveCreate(pMoveType.Simple,None,Sq(FileC,Rank5),None,PieceType.Pawn|>Some)
        let str = Formatter.FormatMoveStr(move)

        Assert.AreEqual("c5", str)

    [<TestMethod>]
    member this.Format_should_format_origin_to_target_move() =
        let move = pMoveCreateOrig(pMoveType.Simple,None,Sq(FileC,Rank5),None,PieceType.Knight|>Some,Sq(FileB,Rank7),None,None)
        let str = Formatter.FormatMoveStr(move)

        Assert.AreEqual("Nb7c5", str)

    [<TestMethod>]
    member this.Format_should_format_origin_file_to_target_move() =
        let move = pMoveCreateOrig(pMoveType.Simple,None,Sq(FileC,Rank5),None,PieceType.Knight|>Some,OUTOFBOUNDS,Some(FileB),None)
        let str = Formatter.FormatMoveStr(move)

        Assert.AreEqual("Nbc5", str)

    [<TestMethod>]
    member this.Format_should_format_origin_rank_to_target_move() =
        let move = pMoveCreateOrig(pMoveType.Simple,None,Sq(FileC,Rank5),None,PieceType.Knight|>Some,OUTOFBOUNDS,None,Some(Rank7))
        let str = Formatter.FormatMoveStr(move)

        Assert.AreEqual("N7c5", str)

    [<TestMethod>]
    member this.Format_should_format_a_capturing_move() =
        let move = pMoveCreate(pMoveType.Capture,PieceType.Bishop|>Some,Sq(FileC,Rank5),None,PieceType.Knight|>Some)
        let str = Formatter.FormatMoveStr(move)

        Assert.AreEqual("NxBc5", str)

    [<TestMethod>]
    member this.Format_should_format_a_pawn_capturing_move_with_origin_file_info() =
        let move = pMoveCreateOrig(pMoveType.Capture,None,Sq(FileC,Rank6),None,None,OUTOFBOUNDS,Some(FileB),None)
        let str = Formatter.FormatMoveStr(move)

        Assert.AreEqual("bxc6", str)

    [<TestMethod>]
    member this.Format_should_format_a_capturing_move_with_origin_square() =
        let move = pMoveCreateOrig(pMoveType.Capture,PieceType.Bishop|>Some,Sq(FileC,Rank5),None,PieceType.Knight|>Some,Sq(FileB,Rank7),None,None)
        let str = Formatter.FormatMoveStr(move)

        Assert.AreEqual("Nb7xBc5", str)

    [<TestMethod>]
    member this.Format_should_format_Nb7xc5() =
        let move = pMoveCreateOrig(pMoveType.Capture,PieceType.Pawn|>Some,Sq(FileC,Rank5),None,PieceType.Knight|>Some,Sq(FileB,Rank7),None,None)
        let str = Formatter.FormatMoveStr(move)

        Assert.AreEqual("Nb7xc5", str)

    [<TestMethod>]
    member this.Format_should_format_exd5() =
        let move = pMoveCreateOrig(pMoveType.Capture,PieceType.Pawn|>Some,Sq(FileD,Rank5),None,PieceType.Pawn|>Some,OUTOFBOUNDS,Some(FileE),None)
        let str = Formatter.FormatMoveStr(move)

        Assert.AreEqual("exd5", str)

    [<TestMethod>]
    member this.Format_should_format_e4xd5ep() =
        let move = pMoveCreateOrig(pMoveType.CaptureEnPassant,PieceType.Pawn|>Some,Sq(FileD,Rank5),None,PieceType.Pawn|>Some,Sq(FileE,Rank4),None,None)
        let str = Formatter.FormatMoveStr(move)

        Assert.AreEqual("e4xd5e.p.", str)

    [<TestMethod>]
    member this.Format_should_format_piece_promotion() =
        let move = pMoveCreateAll(pMoveType.Simple,None,Sq(FileE,Rank8),None,PieceType.Pawn|>Some,OUTOFBOUNDS,None,None,PieceType.Queen|>Some,false,false,false,None)
        let str = Formatter.FormatMoveStr(move)

        Assert.AreEqual("e8=Q", str)

    [<TestMethod>]
    member this.Format_should_format_piece_promotion_after_capture() =
        let move = pMoveCreateAll(pMoveType.Capture,PieceType.Rook|>Some,Sq(FileE,Rank8),None,PieceType.Pawn|>Some,Sq(FileD,Rank7),None,None,PieceType.Queen|>Some,false,false,false,None)
        let str = Formatter.FormatMoveStr(move)

        Assert.AreEqual("d7xRe8=Q", str)

    [<TestMethod>]
    member this.Format_should_format_check_annotation() =
        let move = pMoveCreateAll(pMoveType.Capture,None,Sq(FileC,Rank5),None,PieceType.Knight|>Some,Sq(FileB,Rank7),None,None,None,true,false,false,None)
        let str = Formatter.FormatMoveStr(move)

        Assert.AreEqual("Nb7xc5+", str)

    [<TestMethod>]
    member this.Format_should_format_any_annotation() =
        let move = pMoveCreateAll(pMoveType.Capture,None,Sq(FileB,Rank8),None,PieceType.Rook|>Some,Sq(FileB,Rank1),None,None,None,false,false,true,pMoveAnnotation.Brilliant|>Some)
        let str = Formatter.FormatMoveStr(move)

        Assert.AreEqual("Rb1xb8#!!", str)

    [<TestMethod>]
    member this.Format_should_ommit_redudant_piece_definition___N7c5_and_not_N7Nc5() =
        let move = pMoveCreateOrig(pMoveType.Simple,PieceType.Knight|>Some,Sq(FileC,Rank5),None,PieceType.Knight|>Some,OUTOFBOUNDS,None,Some(Rank7))
        let str = Formatter.FormatMoveStr(move)

        Assert.AreEqual("N7c5", str)

    [<TestMethod>]
    member this.Format_should_include_captured_piece_even_if_its_the_same() =
        let move = pMoveCreateOrig(pMoveType.Capture,PieceType.Knight|>Some,Sq(FileC,Rank5),None,PieceType.Knight|>Some,Sq(FileB,Rank7),None,None)
        let str = Formatter.FormatMoveStr(move)

        Assert.AreEqual("Nb7xNc5", str)
