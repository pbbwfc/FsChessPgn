namespace FsChessPgn

open FsChess

module Stats = 
    
    let Get (igmmvl:(int*Game*string)list) =
        let rec createstats (cigmmvl:(int*Game*string)list) (bdst:BrdStats) =
            if cigmmvl.IsEmpty then bdst
            else
                let i,gm,mv = cigmmvl.Head
                let cmvstl = bdst.Mvstats|>List.filter(fun mvst -> mvst.Mvstr=mv)
                if cmvstl.IsEmpty then 
                    let mvst = {Mvstr=mv;Count=1;Pc=0.0;WhiteWins=(if gm.Result=GameResult.WhiteWins then 1 else 0);
                                Draws=(if gm.Result=GameResult.Draw then 1 else 0);BlackWins=(if gm.Result=GameResult.BlackWins then 1 else 0);
                                Score=0.0;DrawPc=0.0}
                    createstats cigmmvl.Tail {bdst with TotCount=(bdst.TotCount+1);Mvstats=mvst::bdst.Mvstats}
                else
                    let cmvst = cmvstl.Head
                    let mvst = {cmvst with Count=(cmvst.Count+1);
                                           WhiteWins=(if gm.Result=GameResult.WhiteWins then cmvst.WhiteWins+1 else cmvst.WhiteWins);
                                           Draws=(if gm.Result=GameResult.Draw then cmvst.Draws+1 else cmvst.Draws);
                                           BlackWins=(if gm.Result=GameResult.BlackWins then cmvst.BlackWins+1 else cmvst.BlackWins)}
                    let nmvstl = bdst.Mvstats|>List.filter(fun mvst -> mvst.Mvstr<>mv)
                    createstats cigmmvl.Tail {bdst with TotCount=(bdst.TotCount+1);Mvstats=mvst::nmvstl}
        createstats igmmvl BrdStatsEMP


    let pretty (bdstat:BrdStats) =
        let nl  = System.Environment.NewLine
        let hdr = "|  Move  | Count | Percent | WhiteWins |  Draws  | BlackWins | Score | DrawPc |"
        let unl = "  |--------|-------|---------|-----------|---------|-----------|-------|--------|"
        let domvst (mvst:MvStats) =
            let mv = "  | " + mvst.Mvstr + ("       |").Substring(mvst.Mvstr.Length)
            let ct = "  " + mvst.Count.ToString() + ("     |").Substring(mvst.Count.ToString().Length)
            let pc = "  " + mvst.Pc.ToString() + ("       |").Substring(mvst.Pc.ToString().Length)
            let ww = "  " + mvst.WhiteWins.ToString() + ("         |").Substring(mvst.WhiteWins.ToString().Length)
            let dw = "  " + mvst.Draws.ToString() + ("       |").Substring(mvst.Draws.ToString().Length)
            let bw = "  " + mvst.BlackWins.ToString() + ("         |").Substring(mvst.BlackWins.ToString().Length)
            let sc = "  " + mvst.Score.ToString() + ("     |").Substring(mvst.Score.ToString().Length)
            let dp = "  " + mvst.DrawPc.ToString() + ("      |").Substring(mvst.DrawPc.ToString().Length)
            mv + ct + pc + ww + dw + bw + sc + dp

        let bdstl =
            let mv = "  | TOTAL  |"
            let ct = "  " + bdstat.TotCount.ToString() + ("     |").Substring(bdstat.TotCount.ToString().Length)
            let pc = "  100%   |"
            let ww = "  " + bdstat.TotWhiteWins.ToString() + ("         |").Substring(bdstat.TotWhiteWins.ToString().Length)
            let dw = "  " + bdstat.TotDraws.ToString() + ("       |").Substring(bdstat.TotDraws.ToString().Length)
            let bw = "  " + bdstat.TotBlackWins.ToString() + ("         |").Substring(bdstat.TotBlackWins.ToString().Length)
            let sc = "  " + bdstat.TotScore.ToString() + ("     |").Substring(bdstat.TotScore.ToString().Length)
            let dp = "  " + bdstat.TotDrawPc.ToString() + ("      |").Substring(bdstat.TotDrawPc.ToString().Length)
            mv + ct + pc + ww + dw + bw + sc + dp


        hdr + nl + unl + nl +
        (bdstat.Mvstats|>List.map domvst|>List.reduce(fun a b -> a + nl + b)) + 
        nl + unl + nl + bdstl
    

