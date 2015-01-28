// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.
#r "bin/Debug/Stackexchange.redis.dll"
#r "bin/Debug/RedLockFS.dll"
#r "System.Configuration"

open System.Xml
open System.Xml.Serialization
open System.IO

let config = new RedLockFS.RedLockConfiguration()
let instance = new RedLockFS.RedisInstanceElement()
instance.Value <- "http://localhost"
config.redisInstances <- [|instance |]
let serializer = new XmlSerializer(config.GetType())
let wr = new StreamWriter("redisConfig.xml")
serializer.Serialize(wr, config)


// Define your library scripting code here

