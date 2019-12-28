#load @"d:\GitHub\FsChessPgn\FsChessPgn\Types.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\Util.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\Direction.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\Player.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\PieceType.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\Piece.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\Rank.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\File.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\Square.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\GameResult.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\Annotation.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\Bitboard.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\Attacks.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\FEN.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\Move.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\Board.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\Png.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\MoveGenerate.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\PgnWrite.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\pMove.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\MoveTextEntry.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\Game.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\MoveUtil.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\RegParse.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\Games.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\PgnWriter.fs"
#load @"d:\GitHub\FsChessPgn\FsChessPgn\FsChess.fs"
open FsChess
do
    fsi.AddPrinter<Brd>(Pretty.Board)
    fsi.AddPrinter<Move>(Pretty.Move)
    fsi.AddPrinter<Square>(Pretty.Square)
    fsi.AddPrinter<Game>(Pretty.Game)
