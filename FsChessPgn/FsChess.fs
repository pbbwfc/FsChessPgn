namespace FsChess

module Board =

    ///Create a new Board given a FEN string
    let FromStr = FsChessPgn.Board.FromStr
    
    ///The starting Board at the beginning of a game
    let Start = FsChessPgn.Board.Start

    ///Gets all legal moves for this Board
    let AllMoves = FsChessPgn.MoveGenerate.AllMoves

    ///Make a SAN Move such as Nf3 for this Board and return the new Board
    let PushSAN = FsChessPgn.MoveUtil.ApplySAN

    ///Is the current position on the Board checkmate?
    let IsCheckMate = FsChessPgn.MoveGenerate.IsMate 

    ///Is the current position on the Board stalemate?
    let IsStaleMate = FsChessPgn.MoveGenerate.IsDrawByStalemate 

    ///Creates a PNG image file with the specified name, flipped if specified for the given Board 
    let ToPng = FsChessPgn.Png.BoardToPng

    ///Prints an ASCII version of this Board 
    let Print = FsChessPgn.Board.PrintAscii


module Game =

    ///The starting Game with no moves
    let Start = FsChessPgn.Game.Start

    ///Make a SAN Move such as Nf3 for this Game and return the new Game
    let PushSAN = FsChessPgn.Game.AddSan

    ///Pops a move of the end for this Game and return the new Game
    let Pop = FsChessPgn.Game.RemoveMoveEntry

module Pretty =

    let Move = FsChessPgn.MoveUtil.Desc
    let Board = FsChessPgn.Board.ToStr
    let Game = FsChessPgn.Game.pretty
