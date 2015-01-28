namespace RedLockFS.Tests

open System
open System.Diagnostics
open RedLockFS
open Xunit

type RedisInstance() =
    let runRedisLocalInstance port = 
        let proc = new Process()
        proc.StartInfo.UseShellExecute <- false
        proc.StartInfo.FileName <- @"C:\Program Files\Redis\redis-server.exe"
        proc.StartInfo.CreateNoWindow <- true
        proc.StartInfo.Arguments <- "--port " + port
        proc.Start() |> ignore
        printf "start redis instance (port: %s)" port
        proc    

    let runThreeLocalInstances() = 
        [runRedisLocalInstance "4567"; runRedisLocalInstance "5678"; runRedisLocalInstance "6789"]
    
    let procs = runThreeLocalInstances()

    interface IDisposable with
        member this.Dispose() =
            printf "killing redis instances"
            procs |> List.map (fun p -> p.Kill()) |> ignore
            
type DistLockTest() = 
    [<Fact>]
    let testAcquireLock() = 
        let distLock = new DistLock()
        Assert.True(distLock.acquire "lockTest1")

    [<Fact>]
    let testAcquireLockTwice() =
        let distLock = new DistLock()
        Assert.True(distLock.acquireValue "lockTest2" "valueTest")
        Assert.False(distLock.acquireValue "lockTest2" "valueTest")

    [<Fact>]
    let testAcquireAndReleaseLock() =
        let distLock = new DistLock()
        Assert.True(distLock.acquireValue "lockTest3" "valueTest")
        distLock.releaseValue "lockTest3" "valueTest"
        Assert.True(distLock.acquireValue "lockTest3" "valueTest2")

    interface IUseFixture<RedisInstance> with
        member this.SetFixture(data) = ()

    