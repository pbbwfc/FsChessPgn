module fspgn.Test.TestBase

open FParsec

let parse p str =
    match run p str with
    | Success(result, _, _)   -> result
    | Failure(errorMsg, _, _) -> failwith errorMsg

let tryParse p str =
    match run p str with
    | Success(result, _, _)   -> ()
    | Failure(errorMsg, _, _) -> failwith errorMsg

let shouldFail p str =
    match run p str with
    | Success(result, _, _)   -> failwith "Expected parser did not fail"
    | Failure(errorMsg, _, _) -> ()