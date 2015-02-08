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
open System.Threading
open System.Timers
open StackExchange.Redis

module helper = 
    let inline (!>) (x:^a) : ^b = ((^a or ^b) : (static member op_Implicit : ^a -> ^b) x) 
    let toRedisValue : string -> RedisValue = (!>)
    let toRedisKey : string -> RedisKey = (!>)
    let resultToString: RedisResult -> string = RedisResult.op_Explicit

type Status = LockAcquired | LockTimeout | Pending 
type Message = Acquire | Timeout | Fetch of AsyncReplyChannel<Status>

type SingleLock(cnx: string) = 
    let redis = ConnectionMultiplexer.Connect(cnx)
    let token = Guid.NewGuid().ToString()    

    member this.acquireValueWithLeaseTime leaseTime lockValue lockName = 
        let db = redis.GetDatabase()
        db.StringSet(helper.toRedisKey lockName, helper.toRedisValue lockValue, Nullable(new TimeSpan(0, 0, leaseTime)), When.NotExists)   

    member this.acquireValue = RedLockConfiguration.current.defaultLeaseTime.value |> this.acquireValueWithLeaseTime
        
    member this.acquire = token |> this.acquireValue

    member this.releaseValue lockValue lockName= 
        let db = redis.GetDatabase()
        helper.resultToString (
            db.ScriptEvaluate(@"if redis.call(""get"",KEYS[1]) == ARGV[1] 
                                then return redis.call(""del"",KEYS[1]) 
                                else return 0 end",
                              [| helper.toRedisKey lockName |], 
                              [| helper.toRedisValue lockValue |]))
         
    member this.release = token |> this.releaseValue    

    member this.isLocked lockName = 
        let db = redis.GetDatabase()
        not (db.StringGet(helper.toRedisKey lockName).IsNull)
        
    member this.isHoldingLock lockValue lockName = 
        let db = redis.GetDatabase()
        helper.toRedisValue(lockValue).Equals(db.StringGet(helper.toRedisKey lockName))

    member this.waitForLock lockName timeout = 
        let counter = MailboxProcessor<Message>.Start(fun inbox ->
            let rec loop(status) = 
                async {
                    let! msg = inbox.Receive()
                    match msg with
                    | Acquire -> 
                        match status with
                        | Pending -> do! loop(LockAcquired)
                        | _ -> do! loop(status)
                    | Timeout -> 
                        match status with 
                        | Pending -> do! loop(LockTimeout)
                        | _ -> do! loop(status)
                    | Fetch(replyChannel) -> 
                            replyChannel.Reply(status)
                }
            loop(Pending)
        )
           
        let createObservable p f = 
            let timer = new Timer(p)
            timer.Elapsed.Add(f)
            timer.Start();
            timer

        let postAndInterrupt msg (t: Thread) = 
            counter.Post(msg)
            t.Interrupt()

        let tryGetLock (t: Thread) = 
            if this.acquire lockName then postAndInterrupt Acquire t
                
        let work() = 
            try
                Thread.Sleep(Timeout.Infinite)
            with 
            | :? ThreadInterruptedException -> printfn "Thread interrupted!"
            
        let rec getResult() = 
            let r = counter.PostAndReply(fun replyChannel -> Fetch(replyChannel))
            match r with 
            | LockAcquired -> true
            | LockTimeout -> false
            | Pending -> 
                Thread.Sleep(1000)
                getResult() 

        let thread = new System.Threading.Thread(work);
        thread.Start()
        let timers = 
            [createObservable 1000. (fun _ -> tryGetLock thread);
            createObservable timeout (fun _ -> postAndInterrupt Timeout thread)]
        thread.Join()
        timers |> List.map (fun t -> t.Stop()) |> ignore
        getResult()
        