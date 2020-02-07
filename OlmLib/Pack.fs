namespace Olm

module Pack =

    let Save(fn:string, cp:ChessPack, log:string -> unit) = 
        log("Starting saving file " + fn + " to disk")
        fn|>cp.Save
        log("Finished saving file")

    let Load(fn:string) = fn|>ChessPack.Load
