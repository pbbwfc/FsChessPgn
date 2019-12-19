namespace FsChessPgn.NET

open System.IO
open FsChessPgn.PgnParsers
open FsChessPgn.Data

module Games =

    let ReadSeqFromStream(stream:Stream) =
        let p = new Parser()
        seq{for game in p.ReadGamesFromStream(stream) -> game}

    let ReadSeqFromFile(file:string) =
        let p = new Parser()
        seq{for game in p.ReadGamesFromFile(file) -> game}

    let ReadFromString(str:string) =
        let p = new Parser()
        p.ReadFromString(str)

    let ReadFromStream(stream:Stream) =
        let p = new Parser()
        p.ReadFromStream(stream)

    let ReadFromFile(file:string) =
        let p = new Parser()
        p.ReadFromFile(file)

    let SetaMoves(gml:Game list) =
        gml|>List.map Game.SetaMoves