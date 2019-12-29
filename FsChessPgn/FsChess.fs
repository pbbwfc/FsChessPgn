namespace FsChess

module Board =

    ///Create a new Board given a FEN string
    let FromStr = FsChessPgn.Board.FromStr
    
    ///Create a FEN string from this Board 
    let ToStr = FsChessPgn.Board.ToStr

    ///The starting Board at the beginning of a game
    let Start = FsChessPgn.Board.Start

    ///Gets all legal moves for this Board
    let AllMoves = FsChessPgn.MoveGenerate.AllMoves

    ///Make a SAN Move such as Nf3 for this Board and return the new Board
    let PushSAN = FsChessPgn.MoveUtil.ApplySAN

    ///Is there a check on the Board
    let IsCheck = FsChessPgn.Board.IsChk
    
    ///Is the current position on the Board checkmate?
    let IsCheckMate = FsChessPgn.MoveGenerate.IsMate 

    ///Is the current position on the Board stalemate?
    let IsStaleMate = FsChessPgn.MoveGenerate.IsDrawByStalemate 

    ///Is the Square attacked by the specified Player for this Board
    let SquareAttacked = FsChessPgn.Board.SquareAttacked
    
    ///The Squares that attack the specified Square by the specified Player for this Board
    let SquareAttackers = FsChessPgn.Board.SquareAttacksTo

    ///Creates a PNG image file with the specified name, flipped if specified for the given Board 
    let ToPng = FsChessPgn.Png.BoardToPng

    ///Prints an ASCII version of this Board 
    let Print = FsChessPgn.Board.PrintAscii

module Move =

    ///Get an encoded move from a SAN string such as Nf3 for this Board
    let FromSan = FsChessPgn.MoveUtil.fromSAN

    ///Get an encoded move from a UCI string such as g1f3 for thsi Board
    let FromUci = FsChessPgn.MoveUtil.fromUci

    ///Get the UCI string such as g1f3 for a move
    let ToUci = FsChessPgn.MoveUtil.toUci

    ///Get the SAN string such as Nf3 for a move for this board
    let ToSan = FsChessPgn.MoveUtil.toPgn

module Game =

    ///The starting Game with no moves
    let Start = FsChessPgn.Game.Start

    ///Make a SAN Move such as Nf3 for this Game and return the new Game
    let PushSAN = FsChessPgn.Game.AddSan

    ///Pops a move of the end for this Game and return the new Game
    let Pop = FsChessPgn.Game.RemoveMoveEntry

module Pretty =

    let Square = FsChessPgn.Square.Name
    let Move = FsChessPgn.MoveUtil.toUci
    let Board = FsChessPgn.Board.ToStr
    let Game = FsChessPgn.Game.pretty
