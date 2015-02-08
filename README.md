#RedLockFS

Distributed Locking System based on Redis and written in F#.

This project offers 2 classes: 
`SingleLock` and `DistLock`.

* `SingleLock` works with a single instance of Redis.
* `DistLock` works with several instances of Redis to improve high availability. These instances have to be configured in the App.config file 

See also http://redis.io/topics/distlock
