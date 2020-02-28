#load "setup.fsx"
open FsChess
open FsChess.Pgn

let pgn = __SOURCE_DIRECTORY__ + "/data/pgn/AndrewPLewis.pgn"

//loads all games
let gml = Games.ReadListFromFile(pgn)
let count = gml.Length

let plyr = "Lewis, Andrew P"

let getscrmno (i:int) (igm:Game) =
    let gm = igm|>Game.GetaMoves
    let isw = gm.WhitePlayer=plyr

    let amvl = 
        let amv(mte:MoveTextEntry) =
            match mte with
            |HalfMoveEntry(_,_,_,amv) -> [amv.Value]
            |_ -> []
        gm.MoveText|>List.map amv|>List.concat

    let famvl =
        let hamvl = amvl|>List.filter(fun amv -> amv.PreBrd.WhosTurn=if isw then Player.White else Player.Black)
        hamvl.[..19]

    let getscr (amv:aMove) = 
        let scr,mv = Stockfish.GetBestMove(amv.PostBrd,15)
        scr,amv

    let amvscrs = famvl|>List.map getscr

    let scr,mno = 
        let scr,amv = amvscrs|>List.maxBy(fun (scr,_) -> scr)
        scr,amv.Mno

    let opp = if isw then gm.BlackPlayer else gm.WhitePlayer
    (i+1),isw,scr,mno,opp

let ans = getscrmno 0 gml.[0]
let ans1 = getscrmno 0 gml.[1]

let getwb (gms:Game list) =
    let len = gml.Length
    if len<=100 then
        let ansl = gml|>List.mapi getscrmno
        let ws =
            let ws = ansl|>List.filter(fun (_,isw,_,_,_) -> isw)
            ws|>List.filter(fun (_,_,scr,_,_) -> scr>1.0)
        let bs =
            let ws = ansl|>List.filter(fun (_,isw,_,_,_) -> not isw)
            ws|>List.filter(fun (_,_,scr,_,_) -> scr>1.0)
        ws,bs
    else
        let fst = len-100
        let ansl = gml.[fst..]|>List.mapi getscrmno
        let ws =
            let ws = ansl|>List.filter(fun (_,isw,_,_,_) -> isw)
            ws
            |>List.filter(fun (_,_,scr,_,_) -> scr>1.0)
            |>List.map(fun (i,isw,scr,mno,pr)->(i+fst,isw,scr,mno,pr))
        let bs =
            let ws = ansl|>List.filter(fun (_,isw,_,_,_) -> not isw)
            ws
            |>List.filter(fun (_,_,scr,_,_) -> scr>1.0)
            |>List.map(fun (i,isw,scr,mno,pr)->(i+fst,isw,scr,mno,pr))
        ws,bs

let ws,bs = gml|>getwb