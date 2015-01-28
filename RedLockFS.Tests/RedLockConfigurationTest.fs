namespace RedLockFS.Tests

open RedLockFS
open Xunit

type RedLockConfigurationTest() = 
    [<Fact>]
    let testConfig() = 
        let config = RedLockConfiguration.current
        Assert.Equal(3, config.redisInstances.Count)
        Assert.Equal<string>("instance1", config.redisInstances.[0].Name)
        Assert.Equal<string>("localhost:6789", config.redisInstances.[0].Value)
        Assert.Equal<string>("instance2", config.redisInstances.[1].Name)
        Assert.Equal<string>("localhost:5678", config.redisInstances.[1].Value)


