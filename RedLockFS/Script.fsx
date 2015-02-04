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

let singleLock3 = new SingleLock("localhost")
printfn "lock3 acquired: %A" (singleLock3.acquire "test1")
let singleLock2 = new SingleLock("localhost")
printfn "lock2 acquired: %A" (singleLock2.acquire "test1")

let chrono = new Stopwatch()
chrono.Start()
let singleLock = new SingleLock("localhost")
singleLock.waitForLock "test4" 10000.
printfn "first lock acquired after %d ms" chrono.ElapsedMilliseconds
let singleLock1 = new SingleLock("localhost")
singleLock1.waitForLock "test4" 5000.
printfn "second lock acquired after %d ms" chrono.ElapsedMilliseconds
