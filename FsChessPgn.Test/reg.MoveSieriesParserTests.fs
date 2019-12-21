﻿#nowarn "25"
namespace FsChessPgn.Test

open FsChessPgn.Data

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type MoveSeriesRegParserTest() =
    [<TestMethod>]
    member this.parse_simple_piece_move() =
        let s = "Qf5"
        let gml = RegParse.ReadFromString s
        Assert.AreEqual(1, gml.Length)
        let gm = gml.Head
        let mt = gm.MoveText
        Assert.AreEqual(1, mt.Length)
        let (HalfMoveEntry (mn,ic,mv,_)) = mt.Head
        Assert.AreEqual(None,mn)
        Assert.AreEqual(false,ic)
        Assert.AreEqual(MoveType.Simple,mv.Mtype)
        Assert.AreEqual(PieceType.Queen, mv.Piece.Value)
        Assert.AreEqual(F5, mv.TargetSquare)
        Assert.AreEqual(s,mt.Head|>PgnWrite.MoveTextEntryStr)

    [<TestMethod>]
    member this.parse_simple_white_move() =
        let s = "12. Qf5"
        let gml = RegParse.ReadFromString s
        Assert.AreEqual(1, gml.Length)
        let gm = gml.Head
        let mt = gm.MoveText
        Assert.AreEqual(1, mt.Length)
        let (HalfMoveEntry (mn,ic,mv,_)) = mt.Head
        Assert.AreEqual(12,mn.Value)
        Assert.AreEqual(false,ic)
        Assert.AreEqual(MoveType.Simple,mv.Mtype)
        Assert.AreEqual(PieceType.Queen, mv.Piece.Value)
        Assert.AreEqual(F5, mv.TargetSquare)
        Assert.AreEqual(s,mt.Head|>PgnWrite.MoveTextEntryStr)

    [<TestMethod>]
    member this.parse_simple_cont_move() =
        let s = "12... Qf5"
        let gml = RegParse.ReadFromString s
        Assert.AreEqual(1, gml.Length)
        let gm = gml.Head
        let mt = gm.MoveText
        Assert.AreEqual(1, mt.Length)
        let (HalfMoveEntry (mn,ic,mv,_)) = mt.Head
        Assert.AreEqual(12,mn.Value)
        Assert.AreEqual(true,ic)
        Assert.AreEqual(MoveType.Simple,mv.Mtype)
        Assert.AreEqual(PieceType.Queen, mv.Piece.Value)
        Assert.AreEqual(F5, mv.TargetSquare)
        Assert.AreEqual(s,mt.Head|>PgnWrite.MoveTextEntryStr)

    [<TestMethod>]
    member this.parse_simple_moves() =
        let s = "1. e4 c5 2. Nf3 d6 3. Bb5+ Bd7"
        let gml = RegParse.ReadFromString s
        Assert.AreEqual(1, gml.Length)
        let gm = gml.Head
        let mt = gm.MoveText
        Assert.AreEqual(6, mt.Length)
        let (HalfMoveEntry (mn,ic,mv,_)) = mt.Head
        Assert.AreEqual(1,mn.Value)
        Assert.AreEqual(false,ic)
        Assert.AreEqual(MoveType.Simple,mv.Mtype)
        Assert.AreEqual(PieceType.Pawn, mv.Piece.Value)
        Assert.AreEqual(E4, mv.TargetSquare)
        Assert.AreEqual(s,mt|>PgnWrite.MoveTextStr)

    [<TestMethod>]
    member this.parse_split_moves() =
        let s = "1. e4 c5 2. Nf3 \n 2... d6 3. Bb5+ Bd7"
        let gml = RegParse.ReadFromString s
        Assert.AreEqual(1, gml.Length)
        let gm = gml.Head
        let mt = gm.MoveText
        Assert.AreEqual(6, mt.Length)
        let (HalfMoveEntry (mn,ic,mv,_)) = mt.Head
        Assert.AreEqual(1,mn.Value)
        Assert.AreEqual(false,ic)
        Assert.AreEqual(MoveType.Simple,mv.Mtype)
        Assert.AreEqual(PieceType.Pawn, mv.Piece.Value)
        Assert.AreEqual(E4, mv.TargetSquare)
        //Assert.AreEqual(s,mt|>PgnWrite.MoveTextStr)

    [<TestMethod>]
    member this.parse_comment() =
        let s = "{this is a comment}"
        let gml = RegParse.ReadFromString s
        Assert.AreEqual(1, gml.Length)
        let gm = gml.Head
        let mt = gm.MoveText
        Assert.AreEqual(1, mt.Length)
        let (CommentEntry (str)) = mt.Head
        Assert.AreEqual("this is a comment",str)
        Assert.AreEqual(s,mt|>PgnWrite.MoveTextStr)

    [<TestMethod>]
    member this.parse_single_line_comment() =
        let s = "1. e4 e5 2. Nf3 Nc6 3. Bb5 ;This opening is called the Ruy Lopez.
3... a6"
        let gml = RegParse.ReadFromString s
        Assert.AreEqual(1, gml.Length)
        let gm = gml.Head
        let mt = gm.MoveText
        Assert.AreEqual(7, mt.Length)
        let (CommentEntry (str)) = mt.[5]
        Assert.AreEqual("This opening is called the Ruy Lopez.",str)
        Assert.AreEqual("1. e4 e5 2. Nf3 Nc6 3. Bb5 {This opening is called the Ruy Lopez.} 3... a6",mt|>PgnWrite.MoveTextStr)

    [<TestMethod>]
    member this.parse_single_line_comments() =
        let s = "1. e4 e5 2. Nf3 Nc6 3. Bb5 ;This opening is called the Ruy Lopez.
;Another comment
3... a6"
        let gml = RegParse.ReadFromString s
        Assert.AreEqual(1, gml.Length)
        let gm = gml.Head
        let mt = gm.MoveText
        Assert.AreEqual(8, mt.Length)
        let (CommentEntry (str)) = mt.[5]
        Assert.AreEqual("This opening is called the Ruy Lopez.",str)
        let (CommentEntry (str)) = mt.[6]
        Assert.AreEqual("Another comment",str)
        Assert.AreEqual("1. e4 e5 2. Nf3 Nc6 3. Bb5 {This opening is called the Ruy Lopez.} {Another comment} 3... a6",mt|>PgnWrite.MoveTextStr)

    [<TestMethod>]
    member this.parse_draw() =
        let s = "1/2-1/2"
        let gml = RegParse.ReadFromString s
        Assert.AreEqual(1, gml.Length)
        let gm = gml.Head
        let mt = gm.MoveText
        Assert.AreEqual(1, mt.Length)
        let (GameEndEntry(res)) = mt.Head
        Assert.AreEqual(GameResult.Draw,res)
        Assert.AreEqual(s,mt|>PgnWrite.MoveTextStr)

    [<TestMethod>]
    member this.parse_win() =
        let s = "1-0"
        let gml = RegParse.ReadFromString s
        Assert.AreEqual(1, gml.Length)
        let gm = gml.Head
        let mt = gm.MoveText
        Assert.AreEqual(1, mt.Length)
        let (GameEndEntry(res)) = mt.Head
        Assert.AreEqual(GameResult.WhiteWins,res)
        Assert.AreEqual(s,mt|>PgnWrite.MoveTextStr)

    [<TestMethod>]
    member this.parse_loss() =
        let s = "0-1"
        let gml = RegParse.ReadFromString s
        Assert.AreEqual(1, gml.Length)
        let gm = gml.Head
        let mt = gm.MoveText
        Assert.AreEqual(1, mt.Length)
        let (GameEndEntry(res)) = mt.Head
        Assert.AreEqual(GameResult.BlackWins,res)
        Assert.AreEqual(s,mt|>PgnWrite.MoveTextStr)

    [<TestMethod>]
    member this.parse_unknown() =
        let s = "*"
        let gml = RegParse.ReadFromString s
        Assert.AreEqual(1, gml.Length)
        let gm = gml.Head
        let mt = gm.MoveText
        Assert.AreEqual(1, mt.Length)
        let (GameEndEntry(res)) = mt.Head
        Assert.AreEqual(GameResult.Open,res)
        Assert.AreEqual(s,mt|>PgnWrite.MoveTextStr)

    [<TestMethod>]
    member this.parse_post_comment() =
        let s = "1-0 {impressive game!}"
        let gml = RegParse.ReadFromString s
        Assert.AreEqual(1, gml.Length)
        let gm = gml.Head
        let mt = gm.MoveText
        Assert.AreEqual(2, mt.Length)
        let (GameEndEntry(res)) = mt.Head
        Assert.AreEqual(GameResult.WhiteWins,res)
        let (CommentEntry(str)) = mt.[1]
        Assert.AreEqual("impressive game!",str)
        Assert.AreEqual(s,mt|>PgnWrite.MoveTextStr)

    [<TestMethod>]
    member this.parse_pre_comment() =
        let s = "{This game is gonna be awesome! Watch this}
1. e4 e5 2. Nf3"
        let gml = RegParse.ReadFromString s
        Assert.AreEqual(1, gml.Length)
        let gm = gml.Head
        let mt = gm.MoveText
        Assert.AreEqual(4, mt.Length)
        let (CommentEntry(str)) = mt.Head
        Assert.AreEqual("This game is gonna be awesome! Watch this",str)
        let (HalfMoveEntry (mn,ic,mv,_)) = mt.[1]
        Assert.AreEqual(1,mn.Value)
        Assert.AreEqual(false,ic)
        Assert.AreEqual(MoveType.Simple,mv.Mtype)
        Assert.AreEqual(PieceType.Pawn, mv.Piece.Value)
        Assert.AreEqual(E4, mv.TargetSquare)
        Assert.AreEqual("{This game is gonna be awesome! Watch this} 1. e4 e5 2. Nf3",mt|>PgnWrite.MoveTextStr)


    [<TestMethod>]
    member this.parse_between_comment() =
        let s = "1. e4 {[%emt 0.0]} c5 {[%emt 0.0]} 2. Nc3"
        let gml = RegParse.ReadFromString s
        Assert.AreEqual(1, gml.Length)
        let gm = gml.Head
        let mt = gm.MoveText
        Assert.AreEqual(5, mt.Length)
        let (CommentEntry(str)) = mt.[1]
        Assert.AreEqual("[%emt 0.0]",str)
        let (HalfMoveEntry (mn,ic,mv,_)) = mt.Head
        Assert.AreEqual(1,mn.Value)
        Assert.AreEqual(false,ic)
        Assert.AreEqual(MoveType.Simple,mv.Mtype)
        Assert.AreEqual(PieceType.Pawn, mv.Piece.Value)
        Assert.AreEqual(E4, mv.TargetSquare)
        Assert.AreEqual(s,mt|>PgnWrite.MoveTextStr)



    //[<TestMethod>]
    //member this.pMoveSeries_should_accept_comment_between_moves() =
    //    let entries = parse pMoveSeries "1. e4 {[%emt 0.0]} c5 {[%emt 0.0]} 2. Nc3"

    //    Assert.AreEqual(5, entries.Length)
    //    let (CommentEntry str) = entries.[1]
    //    Assert.AreEqual("[%emt 0.0]", str)

    //[<TestMethod>]
    //member this.pMoveSeries_should_accept_NAGs() =
    //    let entries = parse pMoveSeries "1. e4 c5 $6 "

    //    Assert.AreEqual(3, entries.Length)
    //    let (NAGEntry cd) = entries.[2]
    //    Assert.AreEqual(6, cd)

    //[<TestMethod>]
    //member this.pMoveSeries_should_accept_RAVs() =
    //    let entries = parse pMoveSeries "6. d5 $6 (6. Bd3 cxd4 7. exd4 d5 { - B14 }) 6... exd5"

    //    Assert.AreEqual(4, entries.Length)
    //    let (RAVEntry ml) = entries.[2]
    //    Assert.AreEqual(5, ml.Length)
    //    let (HalfMoveEntry (_, _, mv0,None)) = ml.[0] 
    //    let (HalfMoveEntry (_, _, mv1,None)) = ml.[2]
    //    let (CommentEntry str) = ml.[4]
    //    Assert.AreEqual(parse pMove "Bd3", mv0)
    //    Assert.AreEqual(parse pMove "exd4", mv1)
    //    Assert.AreEqual(" - B14 ", str)

    //[<TestMethod>]
    //member this.pMoveSeries_should_accept_nested_RAVs() =
    //    let entries = parse pMoveSeries "6. d5 (6. Bd3 cxd4 7. exd4 d5 (7... Qa4)) 6... exd5"

    //    Assert.AreEqual(3, entries.Length)
    //    let (RAVEntry ml) = entries.[1]
    //    Assert.AreEqual(5, ml.Length)

    //    let (RAVEntry ml2) = ml.[4]
    //    Assert.AreEqual(1, ml2.Length)
    //    let (HalfMoveEntry (_,_,mv,_)) = ml2.[0]
    //    Assert.AreEqual(parse pMove "Qa4", mv)
        