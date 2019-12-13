namespace fspgn.NET

open System.IO
open fspgn.Data.PgnTextTypes
open fspgn.Data

module PgnWriter =

    let WriteStream(stream:Stream,pgnDatabase:pGame list) =
        let writer = new StreamWriter(stream)
        for game in pgnDatabase do
            Formatter.Format(game, writer)
        writer.Close()

    let WriteFile(file:string, pgnDatabase:pGame list) =
        let stream = new FileStream(file, FileMode.OpenOrCreate)
        WriteStream(stream,pgnDatabase)
 