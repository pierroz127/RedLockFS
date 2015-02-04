namespace RedLockFS.Tests

open RedLockFS
open Xunit
open StackExchange.Redis

type SingleLockTest() = 
    [<Fact>]
    let testAcquire() = 
        let singleLock = new SingleLock("localhost")
        Assert.True(singleLock.acquire "test1")
        let singleLock2 = new SingleLock("localhost")
        Assert.False(singleLock2.acquire "test1")
        
    [<Fact>]
    let testAcquireBlockOther() =
        let singleLockOne = new SingleLock("localhost")
        Assert.True(singleLockOne.acquireValue "val1" "test2")
        Assert.False(singleLockOne.acquireValue "val2" "test2")
        
    [<Fact>]
    let testAcquireDeleteAndAcquire() = 
        let singleLock = new SingleLock("localhost")
        Assert.True(singleLock.acquireValue "val1" "test3")
        Assert.Equal<string>("0", singleLock.releaseValue "val0" "test3")
        Assert.Equal<string>("1", singleLock.releaseValue "val1" "test3")
        Assert.True(singleLock.acquireValue "val2" "test3")

    [<Fact>]
    let testWaitForLock() = 
        let singleLock = new SingleLock("localhost")
        Assert.True(singleLock.waitForLock "test4" 10000.) 

    [<Fact>]
    let testWaitForLockTimeout() = 
        let singleLock1 = new SingleLock("localhost")
        singleLock1.acquire "test5" |> ignore
        let singleLock2 = new SingleLock("localhost")
        Assert.False(singleLock2.waitForLock "test5" 10000.) 

    [<Fact>]
    let testWaitForLockAndAcquire() = 
        let singleLock1 = new SingleLock("localhost")
        singleLock1.acquire "test6" |> ignore
        let singleLock2 = new SingleLock("localhost")
        // very long test since it waits for the first lock to timeout
        Assert.True(singleLock2.waitForLock "test6" 60000.)
