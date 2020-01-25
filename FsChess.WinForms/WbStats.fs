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

        //functions
        let nl = System.Environment.NewLine
        let hdr = 
            "<html><body>" + nl +
            "<table style=\"width:100%;border-collapse: collapse;\">" + nl

        let ftr = 
            "</table>" + nl +
            "</body></html>" + nl


        let mvsttag i (mvst:MvStats) =  
            "<tr><td>" + mvst.Mvstr + "</td><td>" + mvst.Count.ToString() + "</td>" + 
            "<td>" + mvst.Pc.ToString("###.0%") + "</td><td>Results</td>" + 
            "<td>" + mvst.Score.ToString("###.0%") + "</td><td>" + mvst.DrawPc.ToString("###.0%") + "</td></tr>" + nl

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
                "<td style=\"border-top: 1px solid black;\">Results</td><td style=\"border-top: 1px solid black;\">" + 
                cbdst.TotScore.ToString("###.0%") + "</td><td style=\"border-top: 1px solid black;\">" + 
                cbdst.TotDrawPc.ToString("###.0%") + "</td></tr>" + nl
                + ftr

        do
            stats.DocumentText <- bdsttags()
            //stats.DocumentCompleted.Add(setclicks)
            stats.ObjectForScripting <- stats


        ///Sets the Stats to be displayed
        member stats.SetStats(sts:BrdStats) = 
            cbdst <- sts
            stats.DocumentText <- bdsttags()
  
        ///Calcs the Stats to be displayed
        member stats.CalcStats(fgms:(int * Game * string) list) = 
            cbdst <- fgms|>Stats.Get
            stats.DocumentText <- bdsttags()
  