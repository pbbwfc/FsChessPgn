namespace FsChess.WinForms

open System.Windows.Forms
open System.Drawing
open FsChess
open FsChess.Pgn

[<AutoOpen>]
module Library5 =

    type WbStats() as stats =
        inherit WebBrowser(AllowWebBrowserDrop = false,IsWebBrowserContextMenuEnabled = false,WebBrowserShortcutsEnabled = false)

        //mutables
        let mutable cbdst = BrdStatsEMP

        //events
        let mvselEvt = new Event<_>()

        //functions
        let nl = System.Environment.NewLine
        let hdr = 
            "<html><body>" + nl +
            "<table style=\"width:100%;border-collapse: collapse;\">" + nl

        let ftr = 
            "</table>" + nl +
            "</body></html>" + nl


        let getdiv ww dr bw =
            let sww = if ww+dr+bw=0 then "0" else ((80*ww)/(ww+dr+bw)).ToString()
            let sdr = if ww+dr+bw=0 then "0" else ((80*dr)/(ww+dr+bw)).ToString()
            let sbw = if ww+dr+bw=0 then "0" else ((80*bw)/(ww+dr+bw)).ToString()
            let wwd = "<span style=\"border-top: 1px solid black;border-left: 1px solid black;border-bottom: 1px solid black;background-color:white;height:18px;width:" + sww + "px\"></span>" 
            let drd = "<span style=\"border-top: 1px solid black;border-bottom: 1px solid black;background-color:gray;height:18px;width:" + sdr + "px\"></span>"
            let bwd = "<span style=\"border-top: 1px solid black;border-right: 1px solid black;border-bottom: 1px solid black;background-color:black;height:18px;width:" + sbw + "px\"></span>"
            wwd + drd + bwd
        
        let mvsttag i (mvst:MvStats) =  
            "<tr id=\"" + i.ToString() + "\"><td>" + mvst.Mvstr + "</td><td>" + mvst.Count.ToString() + "</td>" + 
            "<td>" + mvst.Pc.ToString("##0.0%") + "</td><td>" + (getdiv mvst.WhiteWins mvst.Draws mvst.BlackWins) + "</td>" + 
            "<td>" + mvst.Score.ToString("##0.0%") + "</td><td>" + mvst.DrawPc.ToString("##0.0%") + "</td></tr>" + nl

        let bdsttags() = 
            let mvsts = cbdst.Mvstats
            if mvsts.IsEmpty then hdr+ftr
            else
                hdr +
                "<tr><th style=\"text-align: left;\">Move</th><th style=\"text-align: left;\">Count</th>" +
                "<th style=\"text-align: left;\">Percent</th><th style=\"text-align: left;\">Results</th>" + 
                "<th style=\"text-align: left;\">Score</th><th style=\"text-align: left;\">DrawPc</th></tr>" + nl + 
                (mvsts|>List.mapi mvsttag|>List.reduce(+)) +
                "<tr><td style=\"border-top: 1px solid black;\"></td><td style=\"border-top: 1px solid black;\">" + 
                cbdst.TotCount.ToString() + "</td><td style=\"border-top: 1px solid black;\">100.0%</td>" +
                "<td style=\"border-top: 1px solid black;\">" + (getdiv cbdst.TotWhiteWins cbdst.TotDraws cbdst.TotBlackWins) + "</td><td style=\"border-top: 1px solid black;\">" + 
                cbdst.TotScore.ToString("##0.0%") + "</td><td style=\"border-top: 1px solid black;\">" + 
                cbdst.TotDrawPc.ToString("##0.0%") + "</td></tr>" + nl
                + ftr


        let onclick(el:HtmlElement) = 
            let i = el.Id|>int
            let san = cbdst.Mvstats.[i].Mvstr
            san|>mvselEvt.Trigger

        let setclicks e = 
             for el in stats.Document.GetElementsByTagName("tr") do
                 el.Click.Add(fun _ -> onclick(el))

        do
            stats.DocumentText <- bdsttags()
            stats.DocumentCompleted.Add(setclicks)
            stats.ObjectForScripting <- stats


        ///Sets the Stats to be displayed
        member stats.SetStats(sts:BrdStats) = 
            cbdst <- sts
            stats.DocumentText <- bdsttags()
  
        ///Calculates the Stats to be displayed
        member stats.CalcStats(fgms:(int * Game * string) list) = 
            cbdst <- fgms|>Stats.Get
            stats.DocumentText <- bdsttags()
  
        //publish
        ///Provides the selected move in SAN format
        member __.MvSel = mvselEvt.Publish
