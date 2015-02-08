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

// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.
#r "bin/Debug/Stackexchange.redis.dll"
#r "bin/Debug/RedLockFS.dll"
#r "System.Configuration"

open System.Xml
open System.Xml.Serialization
open System.IO
open System.Diagnostics
open RedLockFS

//let singleLock3 = new SingleLock("localhost")
//printfn "lock3 acquired: %A" (singleLock3.acquire "test1")
//let singleLock2 = new SingleLock("localhost")
//printfn "lock2 acquired: %A" (singleLock2.acquire "test1")

let singleLock4 = new SingleLock("localhost")
singleLock4.acquireValueWithLeaseTime 10 "val" "testScript4"
singleLock4.isLocked "testScript4"

//let chrono = new Stopwatch()
//chrono.Start()
//let singleLock = new SingleLock("localhost")
//singleLock.waitForLock "test4" 10000.
//printfn "first lock acquired after %d ms" chrono.ElapsedMilliseconds
//let singleLock1 = new SingleLock("localhost")
//singleLock1.waitForLock "test4" 5000.
//printfn "second lock acquired after %d ms" chrono.ElapsedMilliseconds
