using NTDLS.KitKey.Shared;

namespace KitKey.Tests.Unit
{
    public class SingleSetRemoveAndGet(ServerFixture fixture) : IClassFixture<ServerFixture>
    {
        [Fact(DisplayName = "Set, remove and Get single values (String).")]
        public void TestPersistentString()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.String.SingleSetRemoveAndGet";

            client.CreateStore(new KkStoreConfiguration(keyStoreName)
            {
                CacheExpiration = TimeSpan.FromMinutes(1),
                PersistenceScheme = KkPersistenceScheme.Persistent,
                ValueType = KkValueType.String
            });

            //Set and get values.
            for (int i = 0; i < 100; i++)
            {
                client.Set(keyStoreName, $"Key{i}", $"Value{i}");
                var value = client.Get<string>(keyStoreName, $"Key{i}");
                Assert.Equal($"Value{i}", value);
            }

            client.Remove(keyStoreName, $"Key50");

            //Re-get all the values.
            for (int i = 0; i < 100; i++)
            {
                var value = client.Get<string>(keyStoreName, $"Key{i}");
                if (i == 50)
                {
                    Assert.Null(value);
                }
                else
                {
                    Assert.Equal($"Value{i}", value);
                }
            }

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            //Re-get all the values.
            for (int i = 0; i < 100; i++)
            {
                var value = client.Get<string>(keyStoreName, $"Key{i}");
                if (i == 50)
                {
                    Assert.Null(value);
                }
                else
                {
                    Assert.Equal($"Value{i}", value);
                }
            }

            client.Disconnect();
        }

        [Fact(DisplayName = "Set, remove and Get single values (Int32).")]
        public void TestPersistentInt32()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.Int32.SingleSetRemoveAndGet";

            client.CreateStore(new KkStoreConfiguration(keyStoreName)
            {
                CacheExpiration = TimeSpan.FromMinutes(1),
                PersistenceScheme = KkPersistenceScheme.Persistent,
                ValueType = KkValueType.Int32
            });

            //Set and get values.
            for (int i = 0; i < 100; i++)
            {
                client.Set(keyStoreName, $"Key{i}", i);
                var value = client.Get<int>(keyStoreName, $"Key{i}");
                Assert.Equal(i, value);
            }

            client.Remove(keyStoreName, $"Key50");

            //Re-get all the values.
            for (int i = 0; i < 100; i++)
            {
                var value = client.Get<int?>(keyStoreName, $"Key{i}");
                if (i == 50)
                {
                    Assert.Null(value);
                }
                else
                {
                    Assert.Equal(i, value);
                }
            }

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            //Re-get all the values.
            for (int i = 0; i < 100; i++)
            {
                var value = client.Get<int?>(keyStoreName, $"Key{i}");
                if (i == 50)
                {
                    Assert.Null(value);
                }
                else
                {
                    Assert.Equal(i, value);
                }
            }

            client.Disconnect();
        }

        [Fact(DisplayName = "Set, remove and Get single values (Int64).")]
        public void TestPersistentInt64()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.Int64.SingleSetRemoveAndGet";

            client.CreateStore(new KkStoreConfiguration(keyStoreName)
            {
                CacheExpiration = TimeSpan.FromMinutes(1),
                PersistenceScheme = KkPersistenceScheme.Persistent,
                ValueType = KkValueType.Int64
            });

            //Set and get values.
            for (long i = 0; i < 100; i++)
            {
                client.Set(keyStoreName, $"Key{i}", i);
                var value = client.Get<long>(keyStoreName, $"Key{i}");
                Assert.Equal(i, value);
            }

            client.Remove(keyStoreName, $"Key50");

            //Re-get all the values.
            for (long i = 0; i < 100; i++)
            {
                var value = client.Get<long?>(keyStoreName, $"Key{i}");
                if (i == 50)
                {
                    Assert.Null(value);
                }
                else
                {
                    Assert.Equal(i, value);
                }
            }

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            //Re-get all the values.
            for (long i = 0; i < 100; i++)
            {
                var value = client.Get<long?>(keyStoreName, $"Key{i}");
                if (i == 50)
                {
                    Assert.Null(value);
                }
                else
                {
                    Assert.Equal(i, value);
                }
            }

            client.Disconnect();
        }

        [Fact(DisplayName = "Set, remove and Get single values (Single).")]
        public void TestPersistentSingle()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.Single.SingleSetRemoveAndGet";

            client.CreateStore(new KkStoreConfiguration(keyStoreName)
            {
                CacheExpiration = TimeSpan.FromMinutes(1),
                PersistenceScheme = KkPersistenceScheme.Persistent,
                ValueType = KkValueType.Single
            });

            //Set and get values.
            for (float i = 0; i < 100; i += 0.5f)
            {
                client.Set(keyStoreName, $"Key{i}", i);
                var value = client.Get<float>(keyStoreName, $"Key{i}");
                Assert.Equal(i, value);
            }

            client.Remove(keyStoreName, $"Key50");

            //Re-get all the values.
            for (float i = 0; i < 100; i += 0.5f)
            {
                var value = client.Get<float?>(keyStoreName, $"Key{i}");
                if (i == 50)
                {
                    Assert.Null(value);
                }
                else
                {
                    Assert.Equal(i, value);
                }
            }

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            //Re-get all the values.
            for (float i = 0; i < 100; i += 0.5f)
            {
                var value = client.Get<float?>(keyStoreName, $"Key{i}");
                if (i == 50)
                {
                    Assert.Null(value);
                }
                else
                {
                    Assert.Equal(i, value);
                }
            }

            client.Disconnect();
        }

        [Fact(DisplayName = "Set, remove and Get single values (Double).")]
        public void TestPersistentDouble()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.Double.SingleSetRemoveAndGet";

            client.CreateStore(new KkStoreConfiguration(keyStoreName)
            {
                CacheExpiration = TimeSpan.FromMinutes(1),
                PersistenceScheme = KkPersistenceScheme.Persistent,
                ValueType = KkValueType.Double
            });

            //Set and get values.
            for (double i = 0; i < 100; i += 0.5)
            {
                client.Set(keyStoreName, $"Key{i}", i);
                var value = client.Get<double>(keyStoreName, $"Key{i}");
                Assert.Equal(i, value);
            }

            client.Remove(keyStoreName, $"Key50");

            //Re-get all the values.
            for (double i = 0; i < 100; i += 0.5)
            {
                var value = client.Get<double?>(keyStoreName, $"Key{i}");
                if (i == 50)
                {
                    Assert.Null(value);
                }
                else
                {
                    Assert.Equal(i, value);
                }
            }

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            //Re-get all the values.
            for (double i = 0; i < 100; i += 0.5)
            {
                var value = client.Get<double?>(keyStoreName, $"Key{i}");
                if (i == 50)
                {
                    Assert.Null(value);
                }
                else
                {
                    Assert.Equal(i, value);
                }
            }

            client.Disconnect();
        }

        [Fact(DisplayName = "Set, remove and Get single values (DateTime).")]
        public void TestPersistentDateTime()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.DateTime.SingleSetRemoveAndGet";

            var startDateTime = DateTime.UtcNow;

            client.CreateStore(new KkStoreConfiguration(keyStoreName)
            {
                CacheExpiration = TimeSpan.FromMinutes(1),
                PersistenceScheme = KkPersistenceScheme.Persistent,
                ValueType = KkValueType.DateTime
            });

            //Set and get values.
            for (int i = 0; i < 100; i++)
            {
                var testValue = startDateTime + TimeSpan.FromDays(i);

                client.Set(keyStoreName, $"Key{i}", testValue);
                var value = client.Get<DateTime>(keyStoreName, $"Key{i}");
                Assert.Equal(testValue, value);
            }

            client.Remove(keyStoreName, $"Key50");

            //Re-get all the values.
            for (int i = 0; i < 100; i++)
            {
                var testValue = startDateTime + TimeSpan.FromDays(i);
                var value = client.Get<DateTime?>(keyStoreName, $"Key{i}");
                if (i == 50)
                {
                    Assert.Null(value);
                }
                else
                {
                    Assert.Equal(testValue, value);
                }
            }

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            //Re-get all the values.
            for (int i = 0; i < 100; i++)
            {
                var testValue = startDateTime + TimeSpan.FromDays(i);
                var value = client.Get<DateTime?>(keyStoreName, $"Key{i}");
                if (i == 50)
                {
                    Assert.Null(value);
                }
                else
                {
                    Assert.Equal(testValue, value);
                }
            }

            client.Disconnect();
        }

        [Fact(DisplayName = "Set, remove and Get single values (Guid).")]
        public void TestPersistentGuid()
        {
            var client = ClientFactory.CreateAndConnect();

            var testLookup = new Dictionary<int, Guid>();

            var keyStoreName = "Test.Guid.SingleSetRemoveAndGet";

            client.CreateStore(new KkStoreConfiguration(keyStoreName)
            {
                CacheExpiration = TimeSpan.FromMinutes(1),
                PersistenceScheme = KkPersistenceScheme.Persistent,
                ValueType = KkValueType.Guid
            });

            //Set and get values.
            for (int i = 0; i < 100; i++)
            {
                var testGuid = Guid.NewGuid();
                testLookup.Add(i, testGuid);
                client.Set(keyStoreName, $"Key{i}", testGuid);
                var value = client.Get<Guid>(keyStoreName, $"Key{i}");
                Assert.Equal(testGuid, value);
            }

            client.Remove(keyStoreName, $"Key50");

            //Re-get all the values.
            for (int i = 0; i < 100; i++)
            {
                var value = client.Get<Guid?>(keyStoreName, $"Key{i}");
                if (i == 50)
                {
                    Assert.Null(value);
                }
                else
                {
                    Assert.Equal(testLookup[i], value);
                }
            }

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            //Re-get all the values.
            for (int i = 0; i < 100; i++)
            {
                var value = client.Get<Guid?>(keyStoreName, $"Key{i}");
                if (i == 50)
                {
                    Assert.Null(value);
                }
                else
                {
                    Assert.Equal(testLookup[i], value);
                }
            }

            client.Disconnect();
        }
    }
}
