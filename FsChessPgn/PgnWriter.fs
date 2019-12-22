namespace FsChessPgn

open System.IO
open FsChessPgn.Data

module PgnWriter =

    let WriteStream(stream:Stream,pgnDatabase:Game list) =
        use writer = new StreamWriter(stream)
        for game in pgnDatabase do
            PgnWrite.Game(game, writer)

    let WriteFile(file:string, pgnDatabase:Game list) =
        let stream = new FileStream(file, FileMode.OpenOrCreate)
        WriteStream(stream,pgnDatabase)

    let WriteString(pgnDatabase:Game list) =
        use writer = new StringWriter()
        for game in pgnDatabase do
            PgnWrite.Game(game, writer)
        writer.ToString()
 