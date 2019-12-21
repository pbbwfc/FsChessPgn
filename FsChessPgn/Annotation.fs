namespace FsChessPgn.Data

module Annotation = 
    let All = ["!!";"!?";"?!";"??";"!";"?"]

    let Parse(s:string) =
         match s with
         | "!!" -> MoveAnnotation.Brilliant
         | "!" -> MoveAnnotation.Good
         | "!?" -> MoveAnnotation.Interesting
         | "?!" -> MoveAnnotation.Dubious
         | "?" -> MoveAnnotation.Mistake
         | "??" -> MoveAnnotation.Blunder
         |_ -> failwith "unknown annotation"

    let Get(mv : string) = 
        let mvan (an:string) (fmv:string) =
            let fln = fmv.Length
            let aln = an.Length
            if fmv.EndsWith(an) then 
                fmv.Substring(0,fln-aln),an|>Parse|>Some
                else fmv,None
        
        let rec getmvan anl =
            if anl|>List.isEmpty then mv,None
            else
                let an = anl.Head
                let tmv,tan = mvan an mv
                if tan.IsSome then tmv,tan
                else getmvan anl.Tail
        getmvan All
