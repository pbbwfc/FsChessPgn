namespace FsChessPgn.Test

open FsChessPgn.Data
open FsChessPgn.Data.PgnTextTypes
open FsChessPgn.PgnParsers.Move
open FsChessPgn.Test.TestBase

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type MoveParserTest() =
    [<TestMethod>]
    member this.pTarget_should_accept_a_normal_move() =
        tryParse pTarget "Rc5"

    [<TestMethod>]
    member this.pTarget_should_accept_a_normal_move_with_lower_case_piece_letter() =
        tryParse pTarget "qc5"

    [<TestMethod>]
    member this.pTarget_should_accept_a_normal_move_with_upper_case_file_letter() =
        tryParse pTarget "QF5"

    [<TestMethod>]
    member this.pTarget_should_accept_a_pawn_move_with_omitted_FigureSymbol() =
        tryParse pTarget "c5"
        tryParse pTarget "b5 "

    [<TestMethod>]
    member this.pTarget_should_fail_on_incorrect_format() =
        shouldFail pTarget "z7"
        shouldFail pTarget "a0"
        shouldFail pTarget "a9"
        shouldFail pTarget "Ka0"
        shouldFail pTarget "Fa1"

    [<TestMethod>]
    member this.pTarget_should_yield_a_MoveInfo_object() =
        let moveInfo = parse pTarget "Qf5"
        Assert.AreEqual(PieceType.Queen, moveInfo.Piece.Value)
        Assert.AreEqual(FileF, moveInfo.File.Value)
        Assert.AreEqual(Rank5, moveInfo.Rank.Value)

    [<TestMethod>]
    member this.pOrigin_should_yield_a_MoveInfo_object() =
        let moveInfo = parse pOrigin "Nd"
        Assert.AreEqual(PieceType.Knight, moveInfo.Piece.Value)
        Assert.AreEqual(FileD, moveInfo.File.Value)
        Assert.IsFalse(moveInfo.Rank.IsSome)

    [<TestMethod>]
    member this.pOrigin_should_yield_a_MoveInfo_object_by_rank() =
        let moveInfo = parse pOrigin "N5"
        Assert.AreEqual(PieceType.Knight, moveInfo.Piece.Value)
        Assert.IsFalse(moveInfo.File.IsSome)
        Assert.AreEqual(Rank5, moveInfo.Rank.Value)

    [<TestMethod>]
    member this.pOrigin_should_yield_a_MoveInfo_object_on_file_rank() =
        let moveInfo = parse pOrigin "d5"
        Assert.AreEqual(PieceType.Pawn, moveInfo.Piece.Value)
        Assert.AreEqual(FileD, moveInfo.File.Value)
        Assert.AreEqual(Rank5, moveInfo.Rank.Value)

    [<TestMethod>]
    member this.pOrigin_should_yield_a_MoveInfo_object_on_file_only() =
        let moveInfo = parse pOrigin "d"
        Assert.AreEqual(PieceType.Pawn, moveInfo.Piece.Value)
        Assert.AreEqual(FileD, moveInfo.File.Value)
        Assert.IsFalse(moveInfo.Rank.IsSome)


    [<TestMethod>]
    member this.pCapturingMove_should_accept_a_standard_capturing_move() =
        tryParse pCapturingMove "c5xQD5"
        tryParse pCapturingMove "c5:QD5"

    [<TestMethod>]
    member this.pCapturingMove_should_accept_a_capturing_move_with_capture_info_at_back() =
        tryParse pCapturingMove "Qd5x"
        tryParse pCapturingMove "Qd5:"

    [<TestMethod>]
    member this.pCapturingMove_should_accept_a_capturing_move_with_capturing_piece_file_rank() =
        tryParse pCapturingMove "Qc5xBd5"//queen on c5 captures bishop on d5
        tryParse pCapturingMove "Qc5:Bd5"//queen on c5 captures bishop on d5

    [<TestMethod>]
    member this.pCapturingMove_should_accept_a_capturing_move_with_capturing_piece_rank() =
        tryParse pCapturingMove "Q5xBd7"//queen on d5 captures bishop on d7
        tryParse pCapturingMove "Q5:Bd7"//queen on d5 captures bishop on d7

    [<TestMethod>]
    member this.pCapturingMove_should_accept_a_capturing_move_with_capturing_piece_and_file() =
        tryParse pCapturingMove "QcxBd5"//queen on C file captures bishop on D5
        tryParse pCapturingMove "Qc:Bd5"//queen on C file captures bishop on D5

    [<TestMethod>]
    member this.pCapturingMove_should_accept_a_capturing_move_with_capturing_piece() =
        tryParse pCapturingMove "QxBd5"//queen captures bishop on D5
        tryParse pCapturingMove "Q:Bd5"//queen captures bishop on D5

    [<TestMethod>]
    member this.pCapturingMove_should_accept_a_capturing_move_with_capturing_file() =
        tryParse pCapturingMove "CxQD5" //piece on C captures queen on D5
        tryParse pCapturingMove "c:QD5"

    [<TestMethod>]
    member this.pCapturingMove_should_accept_a_pawn_capturing_move_with_capturing_file() =
        tryParse pCapturingMove "cxd5" //pawn on C captures D5
        tryParse pCapturingMove "c:D5"


    [<TestMethod>]
    member this.pCapturingMove_should_accept_enpassent_suffix() =
        tryParse pCapturingMove "cxd5e.p." //pawn on c captures pawn on d5 en passant


    [<TestMethod>]
    member this.pCapturingMove_should_accept_simplified_pawn_capturing_moves() =
        tryParse pCapturingMove "cxd" //pawn on c captures pawn on d5 en passant
        tryParse pCapturingMove "cxde.p" //pawn on c captures pawn on d5 en passant

    [<TestMethod>]
    member this.pCapturingMove_should_accept_simplified_pawn_capturing_on_same_lane_should_fail() =
        shouldFail pCapturingMove "axa" //illegal move

    [<TestMethod>]
    member this.pCapturing_move_should_parse_normal_move_correctly() =
        let move = parse pCapturingMove "Qd5xBh5"
        Assert.AreEqual(PieceType.Queen, move.Piece.Value)
        Assert.AreEqual(pMoveType.Capture, move.Mtype)
        Assert.AreEqual(PieceType.Bishop, move.TargetPiece.Value)
        Assert.AreEqual(Rank.ToPosition FileD Rank5, move.OriginSquare)
        Assert.AreEqual(Rank.ToPosition FileH Rank5, move.TargetSquare)

    [<TestMethod>]
    member this.pCapturing_move_should_parse_move_correctly_with_origin_piece() =
        let move = parse pCapturingMove "QxBh5"
        Assert.AreEqual(PieceType.Queen, move.Piece.Value)
        Assert.AreEqual(pMoveType.Capture, move.Mtype)
        Assert.AreEqual(PieceType.Bishop, move.TargetPiece.Value)
        Assert.AreEqual(OUTOFBOUNDS, move.OriginSquare)
        Assert.AreEqual(Rank.ToPosition FileH Rank5, move.TargetSquare)

    [<TestMethod>]
    member this.pCapturing_move_should_parse_move_correctly_with_originSuare() =
        let move = parse pCapturingMove "a1xBh8"
        Assert.AreEqual(PieceType.Pawn, move.Piece.Value)
        Assert.AreEqual(pMoveType.Capture, move.Mtype)
        Assert.AreEqual(PieceType.Bishop, move.TargetPiece.Value)
        Assert.AreEqual(Rank.ToPosition FileA Rank1, move.OriginSquare)
        Assert.AreEqual(Rank.ToPosition FileH Rank8, move.TargetSquare)

    [<TestMethod>]
    member this.pBasicMove_should_accept_pawn_move() =
        tryParse pBasicMove "e2e4"


    [<TestMethod>]
    member this.pBasicMove_should_accept_simple_target_move() =
        tryParse pBasicMove "Rc5"
        tryParse pBasicMove "d4d5"
        tryParse pBasicMove "d5"

    [<TestMethod>]
    member this.pBasicMove_should_correctly_parse_simple_target_move() =
        let move = parse pBasicMove "Rc5"
        Assert.AreEqual(PieceType.Rook, move.Piece.Value)
        Assert.AreEqual(pMoveType.Simple, move.Mtype)
        Assert.AreEqual(Rank.ToPosition FileC Rank5, move.TargetSquare)

    [<TestMethod>]
    member this.pBasicMove_should_correctly_parse_pawnMove() =
        let move = parse pBasicMove "d4d5"
        Assert.AreEqual(pMoveType.Simple, move.Mtype)
        Assert.AreEqual(Rank.ToPosition FileD Rank4, move.OriginSquare)
        Assert.AreEqual(Rank.ToPosition FileD Rank5, move.TargetSquare)

    [<TestMethod>]
    member this.pBasicMove_should_accept_disambiguated_moves() =
        tryParse pBasicMove "Ngf3"
        tryParse pBasicMove "N5f3"
        tryParse pBasicMove "Rdd5"
        tryParse pBasicMove "R3d5"

    [<TestMethod>]
    member this.pBasicMove_should_correctly_parse_disambiguated_moves() =
        let move = parse pBasicMove "Ngf3"
        Assert.AreEqual(PieceType.Knight, move.Piece.Value)
        Assert.AreEqual(Rank.ToPosition FileF Rank3, move.TargetSquare)
        Assert.AreEqual(FileG, move.OriginFile.Value)

    [<TestMethod>]
    member this.pPawnPromotion_should_accept_pawn_promotion_moves() =
        tryParse pPawnPromotion "d8=Q"
        tryParse pPawnPromotion "d8(Q)"
        tryParse pPawnPromotion "c8=R"
        tryParse pPawnPromotion "Pc8=R"

    [<TestMethod>]
    member this.pPawnPromotion_should_accept_capturing_pawn_promotion_moves() =
        tryParse pPawnPromotion "cxd8=Q"
        tryParse pPawnPromotion "dxe8(Q)"
        tryParse pPawnPromotion "b7c8=N"

    [<TestMethod>]
    member this.pPawnPromotion_should_correctly_parse_promoting_move() =
        let move = parse pPawnPromotion "d8=Q"
        Assert.AreEqual(Rank.ToPosition FileD Rank8, move.TargetSquare)
        Assert.AreEqual(PieceType.Queen, move.PromotedPiece.Value)

    [<TestMethod>]
    member this.pCastle_should_accept_catling() =
        tryParse pCastle "O-O"
        tryParse pCastle "O-O-O"
        tryParse pCastle "0-0"
        tryParse pCastle "0-0-0"
        tryParse pCastle "O - O"
        tryParse pCastle "O - O - O"
        tryParse pCastle "0 - 0"
        tryParse pCastle "0 - 0 - 0"

    [<TestMethod>]
    member this.pCastle_should_correctly_parse_castling_move() =
        let move = parse pCastle "0-0-0"
        Assert.AreEqual(pMoveType.CastleQueenSide, move.Mtype)

    [<TestMethod>]
    member this.pMove_should_accept_all_kinds_of_moves() =
        tryParse pMove "Rc5"
        tryParse pMove "e2e4"
        tryParse pMove "QF1"
        tryParse pMove "c5xQD5"
        tryParse pMove "Qc5:Bd5"
        tryParse pMove "cxd5e.p."
        tryParse pMove "d5"
        tryParse pMove "N5f3"
        tryParse pMove "Pc8=Q"
        tryParse pMove "O-O"
        tryParse pMove "0-0-0"

    [<TestMethod>]
    member this.pMove_should_accept_moves_with_check_annotation() =
        tryParse pMove "c5xQD5+"
        tryParse pMove "Qc5:Bd5†"
        tryParse pMove "Qc5:Bd5ch"

    [<TestMethod>]
    member this.pMove_should_accept_moves_with_doublecheck_annotation() =
        tryParse pMove "c5xQD5++"
        tryParse pMove "Qc5:Bd5††"
        tryParse pMove "Qc5:Bd5dbl ch"

    [<TestMethod>]
    member this.pMove_should_accept_moves_with_check_mate_annotation() =
        tryParse pMove "Qd2#"
        tryParse pMove "Qxd2‡"

    [<TestMethod>]
    member this.pMove_should_correctly_parse_simple_move() =
        let move = parse pMove "Rc5"
        Assert.AreEqual(pMoveType.Simple, move.Mtype);
        Assert.AreEqual(PieceType.Rook, move.Piece.Value);
        Assert.AreEqual(Rank.ToPosition FileC Rank5, move.TargetSquare);

    [<TestMethod>]
    member this.pMove_should_correctly_parse_pawn_move() =
        let move = parse pMove "e2e4"
        Assert.AreEqual(pMoveType.Simple, move.Mtype);
        Assert.AreEqual(Rank.ToPosition FileE Rank2, move.OriginSquare);
        Assert.AreEqual(Rank.ToPosition FileE Rank4, move.TargetSquare);

    [<TestMethod>]
    member this.pMove_should_correctly_parse_simple_move_with_different_case() =
        let move = parse pMove "QF1"
        Assert.AreEqual(pMoveType.Simple, move.Mtype);
        Assert.AreEqual(PieceType.Queen, move.Piece.Value);
        Assert.AreEqual(Rank.ToPosition FileF Rank1, move.TargetSquare);

    [<TestMethod>]
    member this.pMove_should_correctly_parse_capturing_move() =
        let move = parse pMove "c5xQD5"
        Assert.AreEqual(pMoveType.Capture, move.Mtype);
        Assert.AreEqual(PieceType.Queen, move.TargetPiece.Value);
        Assert.AreEqual(Rank.ToPosition FileD Rank5, move.TargetSquare);
        Assert.AreEqual(Rank.ToPosition FileC Rank5, move.OriginSquare);

    [<TestMethod>]
    member this.pMove_should_correctly_parse_capturing_move_with_colon_sign() =
        let move = parse pMove "Qc5:Bd5"
        Assert.AreEqual(pMoveType.Capture, move.Mtype);
        Assert.AreEqual(PieceType.Queen, move.Piece.Value);
        Assert.AreEqual(PieceType.Bishop, move.TargetPiece.Value);
        Assert.AreEqual(Rank.ToPosition FileD Rank5, move.TargetSquare);
        Assert.AreEqual(Rank.ToPosition FileC Rank5, move.OriginSquare);

    [<TestMethod>]
    member this.pMove_should_correctly_parse_capturing_move_with_origin_file_and_no_target_piece() =
        let move = parse pMove "bxc6"
        Assert.AreEqual(pMoveType.Capture, move.Mtype);
        Assert.AreEqual(PieceType.Pawn, move.Piece.Value);
        Assert.AreEqual(Rank.ToPosition FileC Rank6, move.TargetSquare);
        Assert.AreEqual(FileB, move.OriginFile.Value);


    [<TestMethod>]
    member this.pMove_should_correctly_parse_en_passant() =
        let move = parse pMove "cxd5e.p."
        Assert.AreEqual(pMoveType.CaptureEnPassant, move.Mtype);
        Assert.AreEqual(Rank.ToPosition FileD Rank5, move.TargetSquare);
        Assert.AreEqual(FileC, move.OriginFile.Value);

    [<TestMethod>]
    member this.pMove_should_correctly_parse_simple_pawn_move() =
        let move = parse pMove "d5"
        Assert.AreEqual(pMoveType.Simple, move.Mtype);
        Assert.AreEqual(Rank.ToPosition FileD Rank5, move.TargetSquare);

    [<TestMethod>]
    member this.pMove_should_correctly_parse_disambigued_move() =
        let move = parse pMove "N5f3"
        Assert.AreEqual(pMoveType.Simple, move.Mtype);
        Assert.AreEqual(PieceType.Knight, move.Piece.Value);
        Assert.AreEqual(Rank.ToPosition FileF Rank3, move.TargetSquare);
        Assert.AreEqual(Rank5, move.OriginRank.Value);

    [<TestMethod>]
    member this.pMove_should_correctly_parse_promotion_move() =
        let move = parse pMove "c8(Q)"
        Assert.AreEqual(pMoveType.Simple, move.Mtype);
        Assert.AreEqual(Rank.ToPosition FileC Rank8, move.TargetSquare);
        Assert.AreEqual(PieceType.Queen, move.PromotedPiece.Value);

    [<TestMethod>]
    member this.pMove_should_correctly_parse_capturing_promotion_move() =
        let move = parse pMove "d7xc8(Q)"
        Assert.AreEqual(pMoveType.Capture, move.Mtype);
        Assert.AreEqual(Rank.ToPosition FileC Rank8, move.TargetSquare);
        Assert.AreEqual(Rank.ToPosition FileD Rank7, move.OriginSquare);
        Assert.AreEqual(PieceType.Queen, move.PromotedPiece.Value);

    [<TestMethod>]
    member this.pMove_should_correctly_parse_promotion_move_wiht_check() =
        let move = parse pMove "c8=Q+"
        Assert.AreEqual(pMoveType.Simple, move.Mtype);
        Assert.AreEqual(Rank.ToPosition FileC Rank8, move.TargetSquare);
        Assert.AreEqual(PieceType.Queen, move.PromotedPiece.Value);
        Assert.IsTrue(move.IsCheck);

    [<TestMethod>]
    member this.pMove_should_correctly_parse_castle_king_side() =
        let move = parse pMove "O-O"
        Assert.AreEqual(pMoveType.CastleKingSide, move.Mtype);

    [<TestMethod>]
    member this.pMove_should_correctly_parse_castle_queen_side() =
        let move = parse pMove "0-0-0"
        Assert.AreEqual(pMoveType.CastleQueenSide, move.Mtype);

    [<TestMethod>]
    member this.pMove_should_correctly_parse_check_indicator() =
        let move = parse pMove "Bb5+"

        Assert.AreEqual(pMoveType.Simple, move.Mtype)
        Assert.IsTrue(move.IsCheck)

    [<TestMethod>]
    member this.pMove_should_correctly_parse_dbl_check_indicator() =
        let move = parse pMove "Bb5++"

        Assert.AreEqual(pMoveType.Simple, move.Mtype)
        Assert.IsTrue(move.IsCheck)
        Assert.IsTrue(move.IsDoubleCheck)

    [<TestMethod>]
    member this.pMove_should_correctly_parse_checkmate_indicator() =
        let move = parse pMove "Bb5#"

        Assert.AreEqual(pMoveType.Simple, move.Mtype)
        Assert.IsTrue(move.IsCheckMate)

    [<TestMethod>]
    member this.pMove_should_correctly_parse_annotation() =
        let move = parse pMove "Bb5+!"

        Assert.AreEqual(pMoveType.Simple, move.Mtype)
        Assert.IsTrue(move.IsCheck)
        Assert.AreEqual(pMoveAnnotation.Good, move.Annotation.Value)

    [<TestMethod>]
    member this.pMove_should_correctly_parse_differentAnnotations() =
        Assert.AreEqual(pMoveAnnotation.MindBlowing, (parse pMove "Bb5!!!").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.Brilliant, (parse pMove "Bb5!!").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.Good, (parse pMove "Bb5!").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.Interesting, (parse pMove "Bb5!?").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.Dubious, (parse pMove "Bb5?!").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.Mistake, (parse pMove "Bb5?").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.Blunder, (parse pMove "Bb5??").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.Abysmal, (parse pMove "Bb5???").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.FascinatingButUnsound, (parse pMove "Bb5!?!").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.Unclear, (parse pMove "Bb5∞").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.WithCompensation, (parse pMove "Bb5=/∞").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.EvenPosition, (parse pMove "Bb5=").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.SlightAdvantageWhite, (parse pMove "Bb5+/=").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.SlightAdvantageBlack, (parse pMove "Bb5=/+").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.AdvantageWhite, (parse pMove "Bb5+/-").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.AdvantageBlack, (parse pMove "Bb5-/+").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.DecisiveAdvantageWhite, (parse pMove "Bb5+-").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.Space, (parse pMove "Bb5○").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.Initiative, (parse pMove "Bb5↑").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.Development, (parse pMove "Bb5↑↑").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.Counterplay, (parse pMove "Bb5⇄").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.Countering, (parse pMove "Bb5∇").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.Idea, (parse pMove "Bb5Δ").Annotation.Value)
        Assert.AreEqual(pMoveAnnotation.TheoreticalNovelty, (parse pMove "Bb5N").Annotation.Value)
