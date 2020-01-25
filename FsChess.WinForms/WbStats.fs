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
        let hdr = "<html><body>"
        let ftr = "</body></html>"


        let mvsttag i (mvst:MvStats) = "" 
        
        let bdsttags() = 
            let mvsts = cbdst.Mvstats
            if mvsts.IsEmpty then hdr+ftr
            else
                hdr +
                (mvsts|>List.mapi mvsttag|>List.reduce(+))
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
  