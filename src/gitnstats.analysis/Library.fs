namespace GitNStats.CommitAnalysis

module FileChanges =

    open LibGit2Sharp

    type PathCount = {
        Path: string;
        Count: int;
    }

    let incrementedCount filename (pathcounts:Map<string, int>) =
        match Map.tryFind filename pathcounts with
        | Some x -> x + 1
        | None -> 1

    let Count diffs =
        diffs 
        |> Seq.fold (fun pathcounts (diff:TreeEntryChanges) -> 
            pathcounts
            |> Map.filter (fun key _ -> key <> diff.Path)                       // preserve state we're note currently working on
            |> Map.add diff.Path (pathcounts |> incrementedCount diff.OldPath)  // increment count for current record
            |> Map.filter (fun key _ ->                                         // remove obsolte file names
                match diff.Status with
                | ChangeKind.Renamed -> key <> diff.OldPath // keep all path counts except the old file name we just discoverd
                | _ -> true                                 // otherwise no-op, keep everything because we've not renamed anything
            )
        ) Map.empty
        |> Seq.map (fun x -> {Path = x.Key; Count = x.Value}) // give c# a type it can easily understand