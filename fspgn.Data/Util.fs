﻿namespace fspgn.Data

open System

[<AutoOpen>]
module Util =
    let Dirn i = enum<Direction> (i)
    let CasFlg i = enum<CstlFlgs> (i)
    let MovFlg i = enum<MoveFlags> (i)
    let PcTp i = enum<PieceType> (i)
    let Pc i = enum<Piece> (i)
    let Plyr i = enum<Player> (i)
    let BitB i =  Microsoft.FSharp.Core.LanguagePrimitives.EnumOfValue<uint64,Bitboard> (i)

    let (-!) (r:Rank) (i:int):Rank = (int(r)-i)
    let (+!) (r:Rank) (i:int):Rank = (int(r)+i)

    let (--) (f:File) (i:int):File = (int(f)-i)
    let (++) (f:File) (i:int):File = (int(f)+i)
    
