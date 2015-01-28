namespace RedLockFS.Tests

open RedLockFS
open Xunit
open StackExchange.Redis

type SingleLockTest() = 
    [<Fact>]
    let testAcquire() = 
        let singleLock = new SingleLock("localhost")
        Assert.True(singleLock.acquire "test1")
        
    [<Fact>]
    let testAcquireBlockOther() =
        let singleLockOne = new SingleLock("localhost")
        Assert.True(singleLockOne.acquireValue "test2" "val1")
        Assert.False(singleLockOne.acquireValue "test2" "val2")
        
    [<Fact>]
    let testAcquireDeleteAndAcquire() = 
        let singleLock = new SingleLock("localhost")
        Assert.True(singleLock.acquireValue "test3" "val1")
        Assert.Equal<string>("0", singleLock.releaseValue "test3" "val0")
        Assert.Equal<string>("1", singleLock.releaseValue "test3" "val1")
        Assert.True(singleLock.acquireValue "test3" "val2")
