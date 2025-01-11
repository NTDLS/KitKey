# KitKey
Ever wondered what it would be like if Redis hadn't become a fat beast? 👊💩

Well, we wanted to keep it simple and really target the .net environment... so we wrote KitKey. A low latency, high-performance, and reliable persistent or ephemeral key-value store over TCP/IP utilizing RocksDB for persistence, wrapped in a partitioned memory cache for level-1 caching, an optional web-service API, management interface, and accompanying nuget packages for server and client.

You can run the KitKey server either from the nuget package or by downloading the dedicated [service installer](https://github.com/NTDLS/KitKey/releases)

KitKey key-value stores are "strongly typed", it supports stores for int32, int64, float, double, date-time and guid.
In addition to single value key-value stores, KeyKey also supports lists of values for a single key, where you can push-first, push-last, get the entire list, get the first or last item and of course remove items from the list.

## Testing Status
[![Regression Tests](https://github.com/NTDLS/KitKey/actions/workflows/Regression%20Tests.yaml/badge.svg)](https://github.com/NTDLS/KitKey/actions/workflows/Regression%20Tests.yaml)

## Packages 📦
- **Server:** https://www.nuget.org/packages/NTDLS.KitKey.Server
- **Client:** https://www.nuget.org/packages/NTDLS.KitKey.Client

## See also:
 - https://github.com/NTDLS/CatMQ
 - 
## Server
Besides running the dedicated service using the installer, you can run the server from code:

```csharp
var serverConfig = new KkServerConfiguration()
{
    PersistencePath = Path.GetDirectoryName(Environment.ProcessPath)
};

var server = new KkServer(serverConfig);
            
server.Start(KkDefaults.DEFAULT_KEYSTORE_PORT);

Console.WriteLine("Press [enter] to stop.");
Console.ReadLine();

server.Stop();
```

## Client (single value)
The client is quite configurable, but the basic connection, store creation and get, set, delete it straight forward.

```csharp
var client = new KkClient();

client.Connect("localhost", KkDefaults.DEFAULT_KEYSTORE_PORT);

var storeConfig = new KkStoreConfiguration("MyFirstStore")
{
    PersistenceScheme = KkPersistenceScheme.Ephemeral,
    ValueType = KkValueType.String
};

client.CreateStore(storeConfig);

for (int i = 0; i < 100000; i++)
{
    var randomKey = Guid.NewGuid().ToString().Substring(0, 4);
    var randomValue = Guid.NewGuid().ToString();

    //Add a string value
    client.Set("MyFirstStore", randomKey, randomValue);

    //Get the value we just set.
    var retrievedValue = client.Get<string>("MyFirstStore", randomKey);
}

Console.WriteLine("Press [enter] to stop.");
Console.ReadLine();

client.Disconnect();
```

Getting, setting, and deleting a key-value to/from the key store server.
```csharp
//Set a value:
client.Set("MyFirstStore", "Key_Name", "Some text value");

//Get a value:
var value = client.Get<string>("MyFirstStore", "Key_Name");

//Delete a value:
client.Remove("MyFirstStore", "Key_Name");
```

## Client (list value)
KitKey also supports creating lists of values for a given key.

```csharp
var client = new KkClient();

client.Connect("localhost", KkDefaults.DEFAULT_KEYSTORE_PORT);

var storeConfig = new KkStoreConfiguration("MyFirstStore")
{
    PersistenceScheme = KkPersistenceScheme.Ephemeral,
    ValueType = KkValueType.ListOfStrings
};

client.CreateStore(storeConfig);

for (int i = 0; i < 100000; i++)
{
    var randomValue = Guid.NewGuid().ToString();

    //Push a list item to the key-store. You can also PushFirst().
    client.PushLast("MyFirstStore", "KeyOfListValue", randomValue);

    //Get the value we just set.
    var retrievedList = client.GetList<string>("MyFirstStore", "KeyOfListValue");
    var firstListValue = client.GetLast<string>("MyFirstStore", "KeyOfListValue");
    var lastListValue = client.GetFirst<string>("MyFirstStore", "KeyOfListValue");

    //Remove item from the list.
    client.RemoveListItemByKey("MyFirstStore", "KeyOfListValue", lastListValue.Id);
}

Console.WriteLine("Press [enter] to stop.");
Console.ReadLine();

client.Disconnect();
```

## Screenshots
![image](https://github.com/user-attachments/assets/d1f8559d-ade8-409d-8bbb-c38770f3bfdf)

![image](https://github.com/user-attachments/assets/af436c63-fe89-4629-8d0c-94a6b8c72374)

## License
[MIT](https://choosealicense.com/licenses/mit/)
