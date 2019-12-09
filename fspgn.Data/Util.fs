namespace fspgn.Data

open System

[<AutoOpen>]
module Util =
    let Pos i = enum<Position> (i)
    let Dirn i = enum<Direction> (i)
    let CasFlg i = enum<CstlFlgs> (i)
    let MovFlg i = enum<MoveFlags> (i)
    let PcTp i = enum<PieceType> (i)
    let Pc i = enum<Piece> (i)
    let Plyr i = enum<Player> (i)
    let Fl i = enum<File>(i)
    let Rnk i = enum<Rank>(i)
    let BitB i =  Microsoft.FSharp.Core.LanguagePrimitives.EnumOfValue<uint64,Bitboard> (i)

    let (-!) (r:Rank) (i:int) = (int(r)-i)|>Rnk
    let (+!) (r:Rank) (i:int) = (int(r)+i)|>Rnk

    let (--) (f:File) (i:int) = (int(f)-i)|>Fl
    let (++) (f:File) (i:int) = (int(f)+i)|>Fl
    
