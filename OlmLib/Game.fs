namespace Olm

open FsChess

module Game =
    
    //TODO need to remove all code from FsChessPgn
    let Set (cp:ChessPack,i:int) =
        let hdr = cp.Hdrs.[i]
        let mvsstr = cp.MvsStrs.[i]
        let gmtxt = hdr.ToString() + mvsstr
        let gm = gmtxt|>FsChessPgn.Games.ReadOneFromString
        gm
