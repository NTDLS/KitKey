# KitKey

Ever wondered what it would be like if Redis hadn't become a fat beast?
Well, we wanted to keep it simple and really target the .net environment... so we wrote KitKey!

A low latency, high-performance, and reliable persistent or ephemeral key-value store over TCP/IP.

KitKey utilizes RocksDB for persistence, wrapped in a partitioned memory cache for level-1 caching.

You can run the KitKey server either from the nuget package or by downloading the dedicated [service installer](https://github.com/NTDLS/KitKey/releases)

## Nuget Packages
- **Server:** https://www.nuget.org/packages/NTDLS.KitKey.Server
- **Client:** https://www.nuget.org/packages/NTDLS.KitKey.Client

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

## Client
The client is quite configurable, but the basic connection, store creation and get, set, delete it straight forward.

```csharp
var client = new KkClient();

client.Connect("localhost", KkDefaults.DEFAULT_KEYSTORE_PORT);

client.CreateStore("MyFirstStore");

for (int i = 0; i < 100000; i++)
{
    var randomKey = Guid.NewGuid().ToString().Substring(0, 4);
    var randomValue = Guid.NewGuid().ToString();

    client.Set("MyFirstStore", randomKey, randomValue);
}

Console.WriteLine("Press [enter] to stop.");
Console.ReadLine();

client.Disconnect();
```

Getting, setting, and deleting a value from the key store server.
```csharp
//Set a value:
client.Set("MyFirstStore", "Key_Name", "Some text value");

//Get a value:
var value = client.Get("MyFirstStore", "Key_Name");

//Delete a value:
client.Delete("MyFirstStore", "Key_Name");
```

## Screenshots
![image](https://github.com/user-attachments/assets/d1f8559d-ade8-409d-8bbb-c38770f3bfdf)

![image](https://github.com/user-attachments/assets/af436c63-fe89-4629-8d0c-94a6b8c72374)

## License
[MIT](https://choosealicense.com/licenses/mit/)
