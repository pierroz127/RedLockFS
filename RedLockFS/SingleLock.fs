namespace RedLockFS

open System
open StackExchange.Redis

module helper = 
    let inline (!>) (x:^a) : ^b = ((^a or ^b) : (static member op_Implicit : ^a -> ^b) x) 
    let toRedisValue : string -> RedisValue = (!>)
    let toRedisKey : string -> RedisKey = (!>)
    let resultToString: RedisResult -> string = RedisResult.op_Explicit

type SingleLock(cnx: string) = 
    let redis = ConnectionMultiplexer.Connect(cnx)
    let token = Guid.NewGuid().ToString()    

    member this.acquireValue lockName lockValue =
        let db = redis.GetDatabase()
        db.StringSet(helper.toRedisKey lockName, helper.toRedisValue lockValue, Nullable(new TimeSpan(0, 0, 30)), When.NotExists)

    member this.acquire = token |> this.acquireValue

    member this.releaseValue lockName lockValue = 
        let db = redis.GetDatabase()
        helper.resultToString (
            db.ScriptEvaluate(@"if redis.call(""get"",KEYS[1]) == ARGV[1] 
                                then return redis.call(""del"",KEYS[1]) 
                                else return 0 end",
                              [| helper.toRedisKey lockName |], 
                              [|helper.toRedisValue lockValue|]))
         
    member this.release = token |> this.releaseValue    