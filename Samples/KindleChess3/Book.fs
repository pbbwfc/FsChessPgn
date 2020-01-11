namespace KindleChess

open System.IO
open FSharp.Json
//open DotLiquid
open Microsoft.FSharp.Reflection
//open FSharp.Markdown
open FsChess

module Book =
    let emp =
        { Title = "NotSet"
          Creator = "Type Creator"
          Date = System.DateTime.Now
          Desc = "Type Description"
          Welcome = ""
          IsW = true
          Chapters = [] }
    
    ///cur - create Current Book
    let cur (nm, isw) =
        { Title = nm
          Creator = "Type Creator"
          Date = System.DateTime.Now
          Desc = "Type Description"
          Welcome = ""
          IsW = isw
          Chapters = [] }
    
    ///upd - update
    let upd (cr, dt, dc, wel) curb =
        { curb with Creator = cr
                    Date = dt
                    Desc = dc
                    Welcome = wel }
    
    //STORAGE elements
    //set up paths
    let tfol =
        Path.Combine
            (Path.GetDirectoryName
                 (System.Reflection.Assembly.GetExecutingAssembly().Location), 
             "Templates")
    let bkfol =
        Path.Combine
            (Path.GetDirectoryName
                 (System.Reflection.Assembly.GetExecutingAssembly().Location), 
             "Books")
    
    let wfol =
        let ans = Path.Combine(bkfol, "White")
        Directory.CreateDirectory(ans) |> ignore
        ans
    
    let bfol =
        let ans = Path.Combine(bkfol, "Black")
        Directory.CreateDirectory(ans) |> ignore
        ans
    
    ///wbks gets list of white books
    let wbks() =
        Directory.GetDirectories(wfol)
        |> Array.map Path.GetFileName
        |> List.ofArray
    
    let wbkso() = wbks() |> List.map (fun s -> s :> obj)
    
    ///bbks gets list of black books
    let bbks() =
        Directory.GetDirectories(bfol)
        |> Array.map Path.GetFileName
        |> List.ofArray
    
    let bbkso() = bbks() |> List.map (fun s -> s :> obj)
    
    ///save - serializes the book to a file in json
    let save (cur : BookT) =
        try 
            let fol = if cur.IsW then wfol else bfol
            let cfl = Path.Combine(fol, cur.Title)
            Directory.CreateDirectory(cfl) |> ignore
            let cfn = Path.Combine(cfl, "book.json")
            let str = Json.serialize (cur)
            File.WriteAllText(cfn, str)
            "Save successful for book: " + cur.Title
        with e -> "Save failed with error: " + e.ToString()
    
    ///saveas - saves the cur with a new name and returns the renamed cur
    let saveas (cur, nm) =
        let ans = { cur with Title = nm }
        save ans |> ignore
        ans
    
    ///load - deserializes to a book from a file
    let load (nm, isw) : BookT =
        //set this to file in White/Black folder with filename same as name
        let fol = if isw then wfol else bfol
        let cfl = Path.Combine(fol, nm)
        let cfn = Path.Combine(cfl, "book.json")
        let str = File.ReadAllText(cfn)
        Json.deserialize (str)
    
    ///delete - deletes the book folder  
    let delete (nm, isw) =
        let fol = if isw then wfol else bfol
        let cfl = Path.Combine(fol, nm)
        Directory.Delete(cfl, true) |> ignore
    
    /////genh - generates HTML files for book
    //let genh (cur : BookT) =
    //    try 
    //        let cfl =
    //            Path.Combine((if cur.IsW then wfol
    //                          else bfol), cur.Title)
    //        Directory.CreateDirectory(cfl) |> ignore
    //        let hfl = Path.Combine(cfl, "HTML")
    //        Directory.CreateDirectory(hfl) |> ignore
    //        //do label for games
    //        let glbls =
    //            let intros = cur.Chapters|>Array.map(fun c -> c.Intro)
    //            let getgmlbl i = 
    //                if i="" then ""
    //                else
    //                    let gmdt = i|>GmChHdT.FromStr
    //                    gmdt.White + " vs. " + gmdt.Black
    //            intros|>Array.map getgmlbl

    //        //write files
    //        //register types used
    //        let reg ty =
    //            let fields = FSharpType.GetRecordFields(ty)
    //            Template.RegisterSafeType(ty, 
    //                                      [| for f in fields -> f.Name |])
    //        reg typeof<BookT>
    //        reg typeof<ChapT>
    //        Template.RegisterFilter typeof<TreeT>
    //        //create output
    //        //do simple options
    //        let gendoc inp oup =
    //            let t =
    //                Path.Combine(tfol, inp)
    //                |> File.ReadAllText
    //                |> Template.Parse
                
    //            let ostr =
    //                t.Render(Hash.FromDictionary(dict [ "book", box cur; "glbls", box glbls ]))
    //            let ouf = Path.Combine(hfl, oup)
    //            File.WriteAllText(ouf, ostr)
    //        gendoc "opf.dotl" "book.opf"
    //        gendoc "ncx.dotl" "book.ncx"
    //        gendoc "toc.dotl" "toc.html"
    //        //copy standard files
    //        let css = Path.Combine(tfol, "book.css")
    //        let ocss = Path.Combine(hfl, "book.css")
    //        File.Copy(css, ocss, true)
    //        let gif =
    //            Path.Combine(tfol, 
    //                         if cur.IsW then "white.gif"
    //                         else "black.gif")
            
    //        let ogif = Path.Combine(hfl, "cover.gif")
    //        File.Copy(gif, ogif, true)
    //        //do welcome
    //        let t =
    //            Path.Combine(tfol, "Welcome.dotl")
    //            |> File.ReadAllText
    //            |> Template.Parse
            
    //        let wel =
    //            cur.Welcome
    //            |> Markdown.Parse
    //            |> Markdown.WriteHtml
            
    //        let ostr = t.Render(Hash.FromDictionary(dict [ "wel", box wel ]))
    //        let ouf = Path.Combine(hfl, "Welcome.html")
    //        File.WriteAllText(ouf, ostr)
    //        // variations
    //        let t =
    //            Path.Combine(tfol, "Variations.dotl")
    //            |> File.ReadAllText
    //            |> Template.Parse
            
    //        let vars =
    //            cur.Chapters |> Array.mapi (fun i c -> c.Lines |> Tree.ToVar i)
    //        let ostr = t.Render(Hash.FromDictionary(dict [ "vars", box vars; "book", box cur ]))
    //        let ouf = Path.Combine(hfl, "Variations.html")
    //        File.WriteAllText(ouf, ostr)
    //        // chapters
    //        cur.Chapters |> Array.iteri (Chap.genh tfol hfl cur.IsW)
    //        "Successfully generated HTML for book: " + cur.Title
    //    with e -> "Generation failed with error: " + e.ToString()
    
    //Chapter elements
    ///rnmChap - renames a chapter
    let rnmChap i nm (cur : BookT) =
        let chs = cur.Chapters
        let fol = if cur.IsW then wfol else bfol
        let cfl = Path.Combine(fol, cur.Title)
        Chap.rnm nm chs.[i] cfl
        let nchs = 
            let achs = chs|>List.toArray
            achs.[i] <- nm
            achs|>List.ofArray
        { cur with Chapters = nchs }

    ///getChap - gets a chapter
    let getChap i (cur : BookT) =
        let nm = cur.Chapters.[i]
        let fol = if cur.IsW then wfol else bfol
        let cfl = Path.Combine(fol, cur.Title)
        Chap.get nm cfl

    ///saveChap - save a chapter
    let saveChap i (cur : BookT) (ch : Game) =
        let nm = cur.Chapters.[i]
        let fol = if cur.IsW then wfol else bfol
        let cfl = Path.Combine(fol, cur.Title)
        Chap.save nm cfl ch

    /////editGmDt - renames a chapter and changes the game details
    //let editGmDt i nm intro (cur : BookT) =
    //    let chs = cur.Chapters
    //    let ch = Chap.editGmDt nm intro chs.[i]
    //    chs.[i] <- ch
    //    { cur with Chapters = chs }

    /////addGameChap - adds a new game chapter to the book
    //let addGameChap gmdt nm (cur : BookT) =
    //    let ch0 = Chap.create cur.Chapters.Length nm
    //    let ch = {ch0 with Intro=gmdt}
    //    { cur with Chapters = Array.append cur.Chapters [| ch |] },ch 

    /////pstGameChap - paste a new game chapter to the book
    //let pstGameChap txt (cur : BookT) =
    //    let ch0 = Chap.create cur.Chapters.Length "Pasted Game"
    //    let gm = txt|>PGN.ReadOneFromString
    //    let intro =
    //        let wbits = gm.White.Split([|','|])|>Array.map(fun s ->s.Trim())
    //        let wh = if wbits.Length=2 then wbits.[1] + " " + wbits.[0] else gm.White
    //        let bbits = gm.Black.Split([|','|])|>Array.map(fun s ->s.Trim())
    //        let bl = if bbits.Length=2 then bbits.[1] + " " + bbits.[0] else gm.Black
    //        {White=wh;Black=bl;Event=gm.Event;GmDate=System.DateTime(int(gm.Year.Value),int(gm.Month.Value),int(gm.Day.Value))}.ToString()
    //    let lines =
    //        let rec addmv imvl mvs otr =
    //            if imvl|>List.isEmpty then otr
    //            else
    //                let mv = imvl.Head
    //                let ntr, chg, vid = Tree.addmv mvs mv otr
    //                let nmvs = mvs@[mv]
    //                addmv imvl.Tail nmvs ntr
    //        addmv gm.Moves [] ch0.Lines        
        
    //    let ch = {ch0 with Intro=intro;Lines=lines}
    //    { cur with Chapters = Array.append cur.Chapters [| ch |] },ch 

    ///addChap - adds a new chapter to the book
    let addChap nm (cur : BookT) =
        let ch = Game.Start
        { cur with Chapters = cur.Chapters@[nm] },ch 
    
    ///delChap - deletes a chapter
    let delChap nm (cur : BookT) =
        let chs = cur.Chapters
        let nchs = chs |> List.filter (fun c -> c <> nm)
        let fol = if cur.IsW then wfol else bfol
        let cfl = Path.Combine(fol, cur.Title)
        Chap.del nm cfl
        { cur with Chapters = nchs }
    
    /////insChap - inserts a new chapter in the book
    //let insChap nm chi (cur : BookT) =
    //    let ch = Chap.create chi nm
    //    let upd i (ch:ChapT) = 
    //        let nwls = ch.Lines |> Tree.glbrelbl ((i+chi+2).ToString())
    //        {ch with Lines=nwls}
    //    let latechs = cur.Chapters.[chi..] |>Array.mapi upd
    //    { cur with Chapters = Array.append(Array.append cur.Chapters.[0..chi-1] [| ch|]) latechs },ch

    /////pstChap - pastes a new chapter in the book
    //let pstChap pch chi (cur : BookT) =
    //    let upd i (ch:ChapT) = 
    //        let nwls = ch.Lines |> Tree.glbrelbl ((i+chi+2).ToString())
    //        {ch with Lines=nwls}
    //    let latechs = cur.Chapters.[chi..] |>Array.mapi upd
    //    let ch = {pch with Lines=pch.Lines |> Tree.glbrelbl ((chi+1).ToString())}
    //    { cur with Chapters = Array.append(Array.append cur.Chapters.[0..chi-1] [| ch|]) latechs },ch

    /////mvuChap - moves up a chapter in the book
    //let mvuChap chi (cur : BookT) =
    //    let prechs = cur.Chapters.[0..chi-1]
    //    let prech = 
    //        let ch = cur.Chapters.[chi+1]
    //        let nwls = ch.Lines |> Tree.glbrelbl ((chi+1).ToString())
    //        {ch with Lines=nwls}
    //    let ch = 
    //        let ch = cur.Chapters.[chi]
    //        let nwls = ch.Lines |> Tree.glbrelbl ((chi+2).ToString())
    //        {ch with Lines=nwls}
    //    let latechs = cur.Chapters.[chi+2..]
    //    { cur with Chapters = Array.append(Array.append prechs [| prech;ch |]) latechs },ch

    /////mvdChap - moves down a chapter in the book
    //let mvdChap chi (cur : BookT) =
    //    let prechs = cur.Chapters.[0..chi-2]
    //    let postch = 
    //        let ch = cur.Chapters.[chi-1]
    //        let nwls = ch.Lines |> Tree.glbrelbl ((chi+1).ToString())
    //        {ch with Lines=nwls}
    //    let ch = 
    //        let ch = cur.Chapters.[chi]
    //        let nwls = ch.Lines |> Tree.glbrelbl ((chi).ToString())
    //        {ch with Lines=nwls}
    //    let latechs = cur.Chapters.[chi+1..]
    //    { cur with Chapters = Array.append(Array.append prechs [| ch;postch |]) latechs },ch


    /////delLine - deletes a line
    //let delLine i vid (cur : BookT) =
    //    let chs = cur.Chapters
    //    let ch = Chap.delLine vid chs.[i]
    //    chs.[i] <- ch
    //    { cur with Chapters = chs }
    
    /////addmv - adds a new move to a chapter
    //let addmv (mvs : Move list) (mv : Move) i (cur : BookT) =
    //    let chs = cur.Chapters
    //    let ch, chg, vid = Chap.addmv mvs mv chs.[i]
    //    chs.[i] <- ch
    //    { cur with Chapters = chs }, chg, vid
    
    /////getdsc -gets a description for a move in a chapter
    //let getdsc (vid : string) mct i (cur : BookT) =
    //    let chs = cur.Chapters
    //    Chap.getdsc vid mct chs.[i]
    
    /////upddsc - updates a description for a move in a chapter
    //let upddsc (vid : string) mct i pm (cur : BookT) =
    //    let chs = cur.Chapters
    //    let ch = Chap.upddsc vid mct pm chs.[i]
    //    chs.[i] <- ch
    //    { cur with Chapters = chs }
    
    /////setsub - sets a line as sub
    //let setsub (vid : string) i (cur : BookT) =
    //    let chs = cur.Chapters
    //    let ch = Chap.setsub vid chs.[i]
    //    chs.[i] <- ch
    //    { cur with Chapters = chs }
    
    /////setmain - sets a line as main
    //let setmain (vid : string) i (cur : BookT) =
    //    let chs = cur.Chapters
    //    let ch = Chap.setmain vid chs.[i]
    //    chs.[i] <- ch
    //    { cur with Chapters = chs }
    
    /////moveup - move up a line
    //let moveup (vid : string) i (cur : BookT) =
    //    let chs = cur.Chapters
    //    let ch = Chap.moveup vid chs.[i]
    //    chs.[i] <- ch
    //    { cur with Chapters = chs }
    
    /////movedown - move down a line
    //let movedown (vid : string) i (cur : BookT) =
    //    let chs = cur.Chapters
    //    let ch = Chap.movedown vid chs.[i]
    //    chs.[i] <- ch
    //    { cur with Chapters = chs }
    
    /////setnag - sets NAG for move
    //let setnag nag mct (vid : string) i (cur : BookT) =
    //    let chs = cur.Chapters
    //    let ch, ntel = Chap.setnag nag mct vid chs.[i]
    //    chs.[i] <- ch
    //    { cur with Chapters = chs }, ntel
