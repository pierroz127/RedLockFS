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
        Assert.True(distLock.acquire "distTest1")

    [<Fact>]
    let testAcquireLockTwice() =
        let distLock = new DistLock()
        Assert.True(distLock.acquireValue "valueTest" "distTest2")
        Assert.False(distLock.acquireValue "valueTest" "distTest2")

    [<Fact>]
    let testAcquireAndReleaseLock() =
        let distLock = new DistLock()
        Assert.True(distLock.acquireValue "valueTest" "distTest3")
        distLock.releaseValue "valueTest" "distTest3" 
        Assert.True(distLock.acquireValue "valueTest2" "distTest3")

    [<Fact>]
    let testDistIsLocked() =
        let distLock = new DistLock()
        Assert.True(distLock.acquireValue "value" "testDistIsLocked")
        Assert.False(distLock.isLocked "testDistIsLockedFalse")
        Assert.True(distLock.isLocked "testDistIsLocked")



    interface IUseFixture<RedisInstance> with
        member this.SetFixture(data) = ()

    