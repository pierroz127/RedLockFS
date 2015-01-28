namespace RedLockFS

open System
open System.Collections.Generic
open System.Configuration
open System.Xml.Serialization

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


    static member current = load()

