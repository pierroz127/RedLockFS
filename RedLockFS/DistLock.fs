﻿namespace RedLockFS

open System

type DistLock() =
    let token = Guid.NewGuid().ToString()
    let singleLocks = 
        [0 .. RedLockConfiguration.current.redisInstances.Count - 1]
        |> List.map (fun i -> new SingleLock(RedLockConfiguration.current.redisInstances.[i].Value)) 
        
    /// acquire a lock with a given value
    /// and return a boolean success or fail.  
    /// If the lock can't be acquired globally, it's is released on all the Redis instances. 
    member this.acquireValue lockName lockValue = 
        let count = singleLocks |> List.fold (fun acc x -> if x.acquireValue lockName lockValue then acc + 1 else acc) 0
        match count with
        | c when c >= (singleLocks.Length / 2 + 1) -> true
        | _ -> this.releaseValue lockName lockValue; false

    /// acquire a lock with a private token (always the same token for an instance of DistLock)
    member this.acquire lockName =
        token |> this.acquireValue lockName
        
    /// release a lock for a given value
    member this.releaseValue lockName lockValue = 
        singleLocks |> List.map (fun x -> x.releaseValue lockName lockValue) |> ignore

    /// release a lock for the private token
    member this.release = token |> this.releaseValue