namespace KindleChess

open System.IO
//open DotLiquid
open Microsoft.FSharp.Reflection
open FsChess
open FsChess.Pgn

module Chap =
    /////create - creates a chapter
    //let create i nm =
    //    { Name = nm
    //      Intro = ""
    //      Lines = i |> Tree.create }
    
    ///rnm - renames a chapter
    let rnm nm oldnm fol = 
        let old = Path.Combine(fol,oldnm + ".pgn")
        if File.Exists(old) then 
            let nw = Path.Combine(fol,nm + ".pgn")
            File.Copy(old,nw,true)
            File.Delete(old)

    ///get - renames a chapter
    let get nm fol = 
        let fn = Path.Combine(fol,nm + ".pgn")
        let gms = fn|>Games.ReadListFromFile
        gms.Head

    ///save - saves a chapter
    let save nm fol (ch:Game) = 
        let fn = Path.Combine(fol,nm + ".pgn")
        [ch]|>Games.WriteFile fn

    ///del - deletes a chapter
    let del nm fol = 
        let fn = Path.Combine(fol,nm)
        if File.Exists(fn) then File.Delete(fn)

    /////editGmDt - renames a chapter and changes the game details
    //let editGmDt nm intro (ch : ChapT) = { ch with Name = nm; Intro = intro }
    
    /////genh - generates HTML files for chapter
    //let genh tfol hfl isw i ch =
    //    //register types used
    //    let reg ty =
    //        let fields = FSharpType.GetRecordFields(ty)
    //        Template.RegisterSafeType(ty, 
    //                                  [| for f in fields -> f.Name |])
    //    reg typeof<ChapT>
    //    let t =
    //        Path.Combine(tfol, "CH.dotl")
    //        |> File.ReadAllText
    //        |> Template.Parse

    //    let gmlbl1,gmlbl2 =
    //        if ch.Intro="" then "",""
    //        else
    //            let gmdt = ch.Intro|>GmChHdT.FromStr
    //            gmdt.White + " vs. " + gmdt.Black,gmdt.Event + ", " + gmdt.GmDate.Year.ToString()
        
    //    let ostr =
    //        t.Render
    //            (Hash.FromDictionary
    //                 (dict [ "ch", box ch
    //                         "gmlbl1", box gmlbl1 
    //                         "gmlbl2", box gmlbl2 
    //                         "i", box (i + 1)
    //                         "treetxt", box (ch.Lines |> Tree.ToHtm hfl isw i) ]))
        
    //    let ouf = Path.Combine(hfl, "CH" + (i + 1).ToString() + ".html")
    //    File.WriteAllText(ouf, ostr)
    
    /////delLine - deletes a line
    //let delLine vid (ch : ChapT) =
    //    let tr = Tree.delLine vid ch.Lines
    //    { ch with Lines = tr }
    
    /////addmv - adds a new move to a chapter
    //let addmv (mvs : Move list) (mv : Move) (ch : ChapT) =
    //    let tr, chg, vid = Tree.addmv mvs mv ch.Lines
    //    { ch with Lines = tr }, chg, vid
    
    /////getdsc -gets a description for a move in a chapter
    //let getdsc (vid : string) mct (ch : ChapT) =
    //    let itr = ch.Lines
    //    Tree.getdsc vid mct itr
    
    /////upddsc - updates a description for a move in a chapter
    //let upddsc (vid : string) mct pm (ch : ChapT) =
    //    let itr = ch.Lines
    //    let tr = Tree.upddsc vid mct pm itr
    //    { ch with Lines = tr }
    
    /////setsub - sets a line as sub
    //let setsub (vid : string) (ch : ChapT) =
    //    let itr = ch.Lines
    //    let tr = Tree.setsub vid itr
    //    { ch with Lines = tr }
    
    /////setmain - sets a line as main
    //let setmain (vid : string) (ch : ChapT) =
    //    let itr = ch.Lines
    //    let tr = Tree.setmain vid itr
    //    { ch with Lines = tr }
    
    /////moveup - move up a line
    //let moveup (vid : string) (ch : ChapT) =
    //    let itr = ch.Lines
    //    let tr = Tree.moveup vid itr
    //    { ch with Lines = tr }
    
    /////movedown - move down a line
    //let movedown (vid : string) (ch : ChapT) =
    //    let itr = ch.Lines
    //    let tr = Tree.movedown vid itr
    //    { ch with Lines = tr }
    
    /////setnag - sets NAG for move
    //let setnag nag mct (vid : string) (ch : ChapT) =
    //    let itr = ch.Lines
    //    let tr, ntel = Tree.setnag nag mct vid itr
    //    { ch with Lines = tr }, ntel
