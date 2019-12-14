namespace fspgn.NET

open System.IO
open fspgn.Data.PgnTextTypes
open fspgn.Data

module PgnWriter =

    let WriteStream(stream:Stream,pgnDatabase:pGame list) =
        use writer = new StreamWriter(stream)
        for game in pgnDatabase do
            Formatter.Format(game, writer)

    let WriteFile(file:string, pgnDatabase:pGame list) =
        let stream = new FileStream(file, FileMode.OpenOrCreate)
        WriteStream(stream,pgnDatabase)

    let WriteString(pgnDatabase:pGame list) =
        use writer = new StringWriter()
        for game in pgnDatabase do
            Formatter.Format(game, writer)
        writer.ToString()
 