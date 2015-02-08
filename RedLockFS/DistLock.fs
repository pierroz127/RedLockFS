//   Copyright 2015 Pierre Leroy
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace RedLockFS

open System

type DistLock() =
    let token = Guid.NewGuid().ToString()
    let singleLocks = 
        [0 .. RedLockConfiguration.current.redisInstances.Count - 1]
        |> List.map (fun i -> new SingleLock(RedLockConfiguration.current.redisInstances.[i].Value)) 
        
    /// acquire a lock with a given value
    /// and return a boolean success or fail.  
    /// If the lock can't be acquired globally, it's is released on all the Redis instances. 
    member this.acquireValue lockValue lockName = 
        let count = singleLocks |> List.fold (fun acc x -> if x.acquireValue lockValue lockName then acc + 1 else acc) 0
        match count with
        | c when c >= (singleLocks.Length / 2 + 1) -> true
        | _ -> this.releaseValue lockName lockValue; false

    /// acquire a lock with a private token (always the same token for an instance of DistLock)
    member this.acquire = token |> this.acquireValue
        
    /// release a lock for a given value
    member this.releaseValue lockValue lockName = 
        singleLocks |> List.map (fun x -> x.releaseValue lockValue lockName) |> ignore

    /// release a lock for the private token
    member this.release = token |> this.releaseValue

    /// test if a resource is currently locked
    member this.isLocked lockName = 
        let count = singleLocks |> List.fold(fun acc x -> if x.isLocked lockName then acc + 1 else acc) 0
        count > singleLocks.Length / 2 + 1