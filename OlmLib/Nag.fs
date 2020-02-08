namespace Olm

module Nag = 
    
    let All = [0..6]@[10]@[14..19]|>List.map Ng

    let ToStr(nag:Nag) =
        match nag with
        | Nag.Good -> "!"
        | Nag.Poor -> "?"
        | Nag.VeryGood -> "!!"
        | Nag.VeryPoor -> "??"
        | Nag.Speculative -> "!?"
        | Nag.Questionable -> "?!"
        | Nag.Even -> "="
        | Nag.Wslight -> "⩲"
        | Nag.Bslight -> "⩱"
        | Nag.Wmoderate -> "±"
        | Nag.Bmoderate -> "∓"
        | Nag.Wdecisive -> "+−"
        | Nag.Bdecisive -> "−+"
        |_ -> ""

    let FromStr(str:string) =
        let stra = All|>List.map ToStr|>List.toArray
        let indx = stra|>Array.findIndex(fun s -> s=str)
        All.[indx]

    let ToHtm(nag:Nag) =
        match nag with
        | Nag.Good -> "&#33;"
        | Nag.Poor -> "&#63;"
        | Nag.VeryGood -> "&#33;&#33;"
        | Nag.VeryPoor -> "&#63;&#63;"
        | Nag.Speculative -> "&#33;&#63;"
        | Nag.Questionable -> "&#63;&#33;"
        | Nag.Even -> "&#61;"
        | Nag.Wslight -> "&#10866;"
        | Nag.Bslight -> "&#10865;"
        | Nag.Wmoderate -> "&#0177;"
        | Nag.Bmoderate -> "&#8723;"
        | Nag.Wdecisive -> "&#43;&minus;"
        | Nag.Bdecisive -> "&minus;&#43;"
        |_ -> ""

    let Desc(nag:Nag) =
        match nag with
        | Nag.Good -> "Good"
        | Nag.Poor -> "Poor"
        | Nag.VeryGood -> "Very Good"
        | Nag.VeryPoor -> "Very Poor"
        | Nag.Speculative -> "Speculative"
        | Nag.Questionable -> "Questionable"
        | Nag.Even -> "Even"
        | Nag.Wslight -> "W slight adv" 
        | Nag.Bslight -> "B slight adv"
        | Nag.Wmoderate -> "W mod adv"
        | Nag.Bmoderate -> "B mod adv"
        | Nag.Wdecisive -> "W dec adv"
        | Nag.Bdecisive -> "B dec adv"
        |_ -> "None"
