namespace Olm

module Stats = 
    
    let Get (igms:(Hdr*Move)[],bd:Brd,cp:ChessPack) =
        let rec createstats (cigmmvl:(Hdr*Move) list) (bdst:BrdStats) =
            if cigmmvl.IsEmpty then bdst
            else
                let hdr,mv = cigmmvl.Head
                let result = hdr.Result|>GmResult.Parse
                let cmvstl = bdst.Mvstats|>List.filter(fun mvst -> mvst.Mv=mv)
                if cmvstl.IsEmpty then 
                    let mvst = {Mv=mv;Mvstr=mv|>Convert.MoveToSan bd;Count=1;Pc=0.0;
                                WhiteWins=(if result=GmResult.WhiteWins then 1 else 0);
                                Draws=(if result=GmResult.Draw then 1 else 0);
                                BlackWins=(if result=GmResult.BlackWins then 1 else 0);
                                Score=0.0;DrawPc=0.0}
                    createstats cigmmvl.Tail {bdst with TotCount=(bdst.TotCount+1);
                                                        TotWhiteWins=(if result=GmResult.WhiteWins then bdst.TotWhiteWins+1 else bdst.TotWhiteWins);
                                                        TotDraws=(if result=GmResult.Draw then bdst.TotDraws+1 else bdst.TotDraws);
                                                        TotBlackWins=(if result=GmResult.BlackWins then bdst.TotBlackWins+1 else bdst.TotBlackWins);
                                                        Mvstats=mvst::bdst.Mvstats}
                else
                    let cmvst = cmvstl.Head
                    let mvst = {cmvst with Count=(cmvst.Count+1);
                                           WhiteWins=(if result=GmResult.WhiteWins then cmvst.WhiteWins+1 else cmvst.WhiteWins);
                                           Draws=(if result=GmResult.Draw then cmvst.Draws+1 else cmvst.Draws);
                                           BlackWins=(if result=GmResult.BlackWins then cmvst.BlackWins+1 else cmvst.BlackWins)}
                    let nmvstl = bdst.Mvstats|>List.filter(fun mvst -> mvst.Mv<>mv)
                    createstats cigmmvl.Tail {bdst with TotCount=(bdst.TotCount+1);
                                                        TotWhiteWins=(if result=GmResult.WhiteWins then bdst.TotWhiteWins+1 else bdst.TotWhiteWins);
                                                        TotDraws=(if result=GmResult.Draw then bdst.TotDraws+1 else bdst.TotDraws);
                                                        TotBlackWins=(if result=GmResult.BlackWins then bdst.TotBlackWins+1 else bdst.TotBlackWins);
                                                        Mvstats=mvst::nmvstl}
        let nbdst = createstats (igms|>List.ofArray) BrdStatsEMP
        let popmvst (mvst:MvStats) =
            let npc = float(mvst.Count)/float(nbdst.TotCount)
            let nscr = (float(mvst.WhiteWins) + 0.5 * float(mvst.Draws))/float(mvst.Count)
            let ndpc = float(mvst.Draws)/float(mvst.Count)
            {mvst with Pc=npc;Score=nscr;DrawPc=ndpc}
        let ntscr =(float(nbdst.TotWhiteWins) + 0.5 * float(nbdst.TotDraws))/float(nbdst.TotCount)
        let ntdpc = float(nbdst.TotDraws)/float(nbdst.TotCount)
        {nbdst with TotScore=ntscr;TotDrawPc=ntdpc;Mvstats=nbdst.Mvstats|>List.map popmvst|>List.sortBy(fun mvst -> -mvst.Count)}
