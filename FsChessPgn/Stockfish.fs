namespace FsChessPgn

open FsChess
open System.IO

module Stockfish =
    let mutable prc = new System.Diagnostics.Process()
    
    ///send message to engine
    let Send(command:string) = 
        prc.StandardInput.WriteLine(command)
    
    ///set up engine
    let ComputeAnswer(fen, depth) = 
        Send("ucinewgame")
        Send("setoption name Threads value " + (System.Environment.ProcessorCount - 1).ToString())
        Send("position startpos")
        Send("position fen " + fen + " ")
        Send("go depth " + depth.ToString())
        prc.WaitForExit()
    
    ///set up process
    let SetUpPrc () = 
        prc.StartInfo.CreateNoWindow <- true
        #if INTERACTIVE
        let wd = __SOURCE_DIRECTORY__
        #else
        let wd = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        #endif
        prc.StartInfo.FileName <- wd + "/stockfish.exe"
        prc.StartInfo.WorkingDirectory <- wd
        prc.StartInfo.RedirectStandardOutput <- true
        prc.StartInfo.UseShellExecute <- false
        prc.StartInfo.RedirectStandardInput <- true
        prc.StartInfo.WindowStyle <- System.Diagnostics.ProcessWindowStyle.Hidden
        prc.Start() |> ignore
        prc.BeginOutputReadLine()
    
    ///Gets the Score and Line from a message
    let GetScrLn(msg:string,bd:Brd) =
        if msg.StartsWith("info") then
            let mv = 
                let st = msg.LastIndexOf("pv")
                let ucis = msg.Substring(st+2)
                //need to change to SAN format
                let bits = ucis.Trim().Split([|' '|])//|>Convert.UcisToSans bd mno
                bits.[0]|>MoveUtil.fromUci bd|>MoveUtil.toPgn bd
            let scr =
                let st = msg.LastIndexOf("cp")
                let ss = msg.Substring(st+2,10).Trim()
                let bits = ss.Split([|' '|])
                let cp = float(bits.[0])/100.0
                cp

            scr,mv
        else
            0.0,msg

    //anl
    let Stop() = 
        if prc <> null then prc.Kill()

    let GetBestMove(cbd:Brd,dpth:int) = 
        prc <- new System.Diagnostics.Process()
        
        let mutable scrln = []
        //p_out
        let pOut (e : System.Diagnostics.DataReceivedEventArgs) = 
            if not (e.Data = null || e.Data = "") then 
                let msg = e.Data.ToString().Trim()
                if not (msg.StartsWith("info") && not (msg.Contains(" cp "))) then 
                    System.Console.WriteLine(msg)
                    if msg.StartsWith("bestmove") then Stop()
                    else
                        scrln <- ((msg,cbd)|>GetScrLn)::scrln
        prc.OutputDataReceived.Add(pOut)
        //Start process
        SetUpPrc()
        // call calcs
        // need to send game position moves as UCI
        let fen = cbd|>Board.ToStr
        ComputeAnswer(fen, dpth)
        scrln.Head


