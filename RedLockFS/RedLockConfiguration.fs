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
open System.Collections.Generic
open System.Configuration
open System.Xml.Serialization

type DefaultLeaseTimeElement()=
    inherit ConfigurationElement()

    [<ConfigurationProperty("value")>]
    member this.value
        with get() = this.["value"] :?> int
        and set(value: int) = this.["value"] <- value    

type RedisInstanceElement() = 
    inherit ConfigurationElement()

    [<ConfigurationProperty("name")>]
    member this.Name
        with get() = this.["name"] :?> string
        and set(value: string) = this.["name"] <- value

    [<ConfigurationProperty("value")>]
    member this.Value
        with get() = this.["value"] :?> string
        and set(value: string) = this.["value"] <- value

type RedisInstanceCollection() = 
    inherit ConfigurationElementCollection()
        override this.CreateNewElement() = new RedisInstanceElement() :> ConfigurationElement
        override this.GetElementKey(element) = (element :?> RedisInstanceElement).Name :> obj

    member this.Item 
        with get(index) = this.BaseGet(index) :?> RedisInstanceElement
        and set index value = 
            if this.BaseGet(index) <> null then this.BaseRemoveAt(index)
            this.BaseAdd(index, value)

    member this.Add (redisInstance: RedisInstanceElement) = 
        this.BaseAdd(redisInstance);
 
    member this.Clear() = this.BaseClear()

    member this.RemoveAt index = this.BaseRemoveAt(index)

    member this.Remove(name: string) = this.BaseRemove(name)

    member this.Remove(element: RedisInstanceElement) = this.BaseRemove(element.Name)
        
type RedLockConfiguration() =
    inherit ConfigurationSection()
    
    static let load() = 
        let config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
        config.GetSection("redlockfs") :?> RedLockConfiguration

    [<ConfigurationProperty("redisInstances", IsRequired = true)>]
    [<ConfigurationCollection(typeof<RedisInstanceElement>, AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")>]
    member this.redisInstances 
        with get() = this.["redisInstances"] :?> RedisInstanceCollection

    [<ConfigurationProperty("defaultLeaseTime", IsRequired = true)>]
    member this.defaultLeaseTime
        with get() = this.["defaultLeaseTime"] :?> DefaultLeaseTimeElement
        and set(value: DefaultLeaseTimeElement) = this.["defaultLeaseTime"] <- value
        
    static member current = load()

