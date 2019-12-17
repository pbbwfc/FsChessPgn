namespace FsChessPgn.PgnParsers

open FParsec
open System.IO
open FsChessPgn.PgnParsers.Game

type Parser() =
    member this.ReadFromFile(file:string) =
        let stream = new FileStream(file, FileMode.Open)
        let result = this.ReadFromStream(stream)
        stream.Close()

        result

    member this.ReadFromStream(stream: System.IO.Stream) =
        let parserResult = runParserOnStream pDatabase () "pgn" stream System.Text.Encoding.UTF8

        let db =
            match parserResult with
            | Success(result, _, _)   -> result
            | Failure(errorMsg, _, _) -> failwith errorMsg

        db

    member this.ReadFromString(input: string) =
        let parserResult = run pDatabase input

        let db =
            match parserResult with
            | Success(result, _, _)   -> result
            | Failure(errorMsg, _, _) -> failwith errorMsg

        db

    member this.ReadGamesFromStream(stream: System.IO.Stream) =
        seq {
            let charStream = new CharStream<Unit>(stream, true, System.Text.Encoding.UTF8);
            while not charStream.IsEndOfStream do
                 yield this.ParseGame(charStream)
            }

    member this.ReadGamesFromFile(file: string) =
        seq {
            let charStream = new CharStream<Unit>(file, System.Text.Encoding.UTF8);
            while not charStream.IsEndOfStream do
                 yield this.ParseGame(charStream)
            }

    member this.ParseGame(charStream: CharStream<Unit>) =

        let parserResult = pGame(charStream)
        let game  =
            match parserResult.Status with
            | ReplyStatus.Ok   -> parserResult.Result
            | _ -> failwith (ErrorMessageList.ToSortedArray(parserResult.Error) |> Array.map(fun e -> e.ToString()) |> String.concat "\n" );

        game