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

type RedLockConfigurationTest() = 
    [<Fact>]
    let testConfig() = 
        let config = RedLockConfiguration.current
        Assert.Equal(30, config.defaultLeaseTime.value)
        Assert.Equal(3, config.redisInstances.Count)
        Assert.Equal<string>("instance1", config.redisInstances.[0].Name)
        Assert.Equal<string>("localhost:6789", config.redisInstances.[0].Value)
        Assert.Equal<string>("instance2", config.redisInstances.[1].Name)
        Assert.Equal<string>("localhost:5678", config.redisInstances.[1].Value)


