namespace PgnLib

open System

[<AutoOpen>]
module Types = 
    /// Move type where not a simple move
    type MvTyp = 
        | Prom of char
        | CasK
        | CasQ
        | Ep
        | Standard
        | Invalid

    /// Move eval
    type MvEval = 
        | Normal
        | Excellent
        | Weak
        | Surprising
    
    /// Index of square on the board
    type Sq = int
    
    /// Move type including eval
    type Move = 
        { Mfrom : Sq
          Mto : Sq
          Mtyp : MvTyp
          Mpgn : string
          Meval : MvEval
          Scr10 : int
          Scr25 : int 
          Bresp : string
          ECO : string
          FicsPc : float}
        override x.ToString() = x.Mpgn
        member x.UCI = 
            let mv = Ref.sq.[x.Mfrom] + Ref.sq.[x.Mto]
            match x.Mtyp with
            | Prom(t) -> mv + t.ToString()
            | _ -> mv
    
    //storage of variations
    type Line = 
        { ECO : string
          Mvs : Move list }
    type Varn = 
        { Name : string
          Isw : bool
          ECO : string
          Lines : Line list }
    
    //test - records of tests
    type TestDet = 
        { Mvl : Move list
          Mv : Move
          Vnname : string
          Status : string }
    
    //tstres - test result
    type Tstres = 
        { Vname : string
          Visw : bool
          Dte : DateTime
          Res : int }
    
