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

open RedLockFS
open Xunit
open StackExchange.Redis

type SingleLockTest() = 
    [<Fact>]
    let testAcquire() = 
        let singleLock = new SingleLock("localhost")
        Assert.True(singleLock.acquire "testAcquire")
        let singleLock2 = new SingleLock("localhost")
        Assert.False(singleLock2.acquire "testAcquire")

    [<Fact>]
    let testIsLocked() = 
        let singleLock = new SingleLock("localhost")
        Assert.True(singleLock.acquire "testIsLocked")
        Assert.True(singleLock.isLocked "testIsLocked")

    [<Fact>]
    let testAcquireBlockOther() =
        let singleLockOne = new SingleLock("localhost")
        Assert.True(singleLockOne.acquireValue "val1" "testAcquireBlockOther")
        Assert.False(singleLockOne.acquireValue "val2" "testAcquireBlockOther")
        
    [<Fact>]
    let testAcquireDeleteAndAcquire() = 
        let singleLock = new SingleLock("localhost")
        Assert.True(singleLock.acquireValue "val1" "test4")
        Assert.Equal<string>("0", singleLock.releaseValue "val0" "test4")
        Assert.Equal<string>("1", singleLock.releaseValue "val1" "test4")
        Assert.True(singleLock.acquireValue "val2" "test4")

    [<Fact>]
    let testWaitForLock() = 
        let singleLock = new SingleLock("localhost")
        Assert.True(singleLock.waitForLock "test5" 10000.) 

    [<Fact>]
    let testWaitForLockTimeout() = 
        let singleLock1 = new SingleLock("localhost")
        singleLock1.acquire "testWaitForLockTimeout" |> ignore
        let singleLock2 = new SingleLock("localhost")
        Assert.False(singleLock2.waitForLock "testWaitForLockTimeout" 10000.) 

    [<Fact>]
    let testWaitForLockAndAcquire() = 
        let singleLock1 = new SingleLock("localhost")
        singleLock1.acquireValueWithLeaseTime 5 "value" "testWaitForLockAndAcquire" |> ignore
        let singleLock2 = new SingleLock("localhost")
        Assert.True(singleLock2.waitForLock "testWaitForLockAndAcquire" 60000.)

    [<Fact>]
    let testIsHoldingLock() = 
        let singleLock = new SingleLock("localhost")
        singleLock.acquireValue "lock" "testIsHoldingLock" |> ignore
        Assert.False(singleLock.isHoldingLock "clock" "testIsHoldingLock")
        Assert.True(singleLock.isHoldingLock "lock" "testIsHoldingLock")
