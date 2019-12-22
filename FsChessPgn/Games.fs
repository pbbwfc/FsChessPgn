namespace FsChessPgn

open System.IO
open System.Text
open FsChessPgn.Data

module Games =

    let ReadFromStream(stream : Stream) = 
        let sr = new StreamReader(stream)
        let db = RegParse.AllGamesRdr(sr)
        db

    let ReadFromFile(file : string) = 
        let stream = new FileStream(file, FileMode.Open)
        let result = ReadFromStream(stream) |> Seq.toList
        stream.Close()
        result

    let ReadFromString(str : string) = 
        let byteArray = Encoding.ASCII.GetBytes(str)
        let stream = new MemoryStream(byteArray)
        let result = ReadFromStream(stream) |> Seq.toList
        stream.Close()
        result

    let ReadOneFromString(str : string) = 
        let gms = str|>ReadFromString
        gms.Head

    let SetaMoves(gml:Game list) =
        gml|>List.map Game.SetaMoves
