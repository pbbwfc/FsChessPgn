namespace FsChessPgn.NET

open System.IO
open FsChessPgn.PgnParsers

module PgnReader =

    let ReadGamesFromStream(stream:Stream) =
        let p = new Parser()
        seq{for game in p.ReadGamesFromStream(stream) -> game}

    let ReadGamesFromFile(file:string) =
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
