using NTDLS.KitKey.Shared;

namespace KitKey.Tests.Unit
{
    public class ListPushLastAndGet(ServerFixture fixture) : IClassFixture<ServerFixture>
    {
        [Fact(DisplayName = "Push values to bottom of List (String).")]
        public void TestPersistentListOfStrings()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.ListOfStrings.Last";

            client.StoreCreate(new KkStoreConfiguration(keyStoreName)
            {
                CacheExpiration = TimeSpan.FromMinutes(1),
                PersistenceScheme = KkPersistenceScheme.Persistent,
                ValueType = KkValueType.ListOfStrings
            });

            //Push values to the bottom of the list.
            for (int i = 0; i < 100; i++)
            {
                client.PushLast(keyStoreName, "TestValueList", $"Value{i}");
                var values = client.GetList<string>(keyStoreName, "TestValueList");

                Assert.NotNull(values);
                Assert.Equal(i + 1, values.Count);

                for (int t = 0; t < values.Count; t++)
                {
                    Assert.Equal($"Value{t}", values[t].Value);
                }
            }

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            var postFlushValues = client.GetList<string>(keyStoreName, "TestValueList");
            Assert.NotNull(postFlushValues);

            for (int t = 0; t < postFlushValues.Count; t++)
            {
                Assert.Equal($"Value{t}", postFlushValues[t].Value);
            }

            client.Disconnect();
        }

        [Fact(DisplayName = "Push values to bottom of List (Int32).")]
        public void TestPersistentListOfInt32s()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.ListOfInt32s.Last";

            client.StoreCreate(new KkStoreConfiguration(keyStoreName)
            {
                CacheExpiration = TimeSpan.FromMinutes(1),
                PersistenceScheme = KkPersistenceScheme.Persistent,
                ValueType = KkValueType.ListOfInt32s
            });

            //Push values to the bottom of the list.
            for (int i = 0; i < 100; i++)
            {
                client.PushLast(keyStoreName, "TestValueList", i);
                var values = client.GetList<Int32>(keyStoreName, "TestValueList");

                Assert.NotNull(values);
                Assert.Equal(i + 1, values.Count);

                for (int t = 0; t < values.Count; t++)
                {
                    Assert.Equal(t, values[t].Value);
                }
            }

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            var postFlushValues = client.GetList<Int32>(keyStoreName, "TestValueList");
            Assert.NotNull(postFlushValues);

            for (int t = 0; t < postFlushValues.Count; t++)
            {
                Assert.Equal(t, postFlushValues[t].Value);
            }

            client.Disconnect();
        }

        [Fact(DisplayName = "Push values to bottom of List (Int64).")]
        public void TestPersistentListOfInt64s()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.ListOfInt64s.Last";

            client.StoreCreate(new KkStoreConfiguration(keyStoreName)
            {
                CacheExpiration = TimeSpan.FromMinutes(1),
                PersistenceScheme = KkPersistenceScheme.Persistent,
                ValueType = KkValueType.ListOfInt64s
            });

            //Push values to the bottom of the list.
            for (long i = 0; i < 100; i++)
            {
                client.PushLast(keyStoreName, "TestValueList", i);
                var values = client.GetList<Int64>(keyStoreName, "TestValueList");

                Assert.NotNull(values);
                Assert.Equal(i + 1, values.Count);

                for (int t = 0; t < values.Count; t++)
                {
                    Assert.Equal(t, values[t].Value);
                }
            }

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            var postFlushValues = client.GetList<Int64>(keyStoreName, "TestValueList");
            Assert.NotNull(postFlushValues);

            for (int t = 0; t < postFlushValues.Count; t++)
            {
                Assert.Equal(t, postFlushValues[t].Value);
            }

            client.Disconnect();
        }

        [Fact(DisplayName = "Push values to bottom of List (Single).")]
        public void TestPersistentListOfSingles()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.ListOfSingles.Last";

            client.StoreCreate(new KkStoreConfiguration(keyStoreName)
            {
                CacheExpiration = TimeSpan.FromMinutes(1),
                PersistenceScheme = KkPersistenceScheme.Persistent,
                ValueType = KkValueType.ListOfSingles
            });

            float pushValue = 0;

            //Push values to the bottom of the list.
            for (int i = 0; i < 100; i++)
            {
                client.PushLast(keyStoreName, "TestValueList", pushValue);
                var values = client.GetList<float>(keyStoreName, "TestValueList");

                Assert.NotNull(values);
                Assert.Equal(i + 1, values.Count);

                double testValue = 0;
                for (int index = 0; index < values.Count; index++)
                {
                    Assert.Equal(testValue, values[index].Value);
                    testValue += 0.5;
                }

                pushValue += 0.5f;
            }

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            var postFlushValues = client.GetList<Single>(keyStoreName, "TestValueList");
            Assert.NotNull(postFlushValues);

            float pfTestValue = 0;
            for (int pfIndex = 0; pfIndex < postFlushValues.Count; pfIndex++)
            {
                Assert.Equal(pfTestValue, postFlushValues[pfIndex].Value);
                pfTestValue += 0.5f;
            }

            client.Disconnect();
        }

        [Fact(DisplayName = "Push values to bottom of List (Double).")]
        public void TestPersistentListOfDoubles()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.ListOfDoubles.Last";

            client.StoreCreate(new KkStoreConfiguration(keyStoreName)
            {
                CacheExpiration = TimeSpan.FromMinutes(1),
                PersistenceScheme = KkPersistenceScheme.Persistent,
                ValueType = KkValueType.ListOfDoubles
            });

            double pushValue = 0;

            //Push values to the bottom of the list.
            for (int i = 0; i < 100; i++)
            {
                client.PushLast(keyStoreName, "TestValueList", pushValue);
                var values = client.GetList<Double>(keyStoreName, "TestValueList");

                Assert.NotNull(values);
                Assert.Equal(i + 1, values.Count);

                double testValue = 0;
                for (int index = 0; index < values.Count; index++)
                {
                    Assert.Equal(testValue, values[index].Value);
                    testValue += 0.5;
                }

                pushValue += 0.5;
            }

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            var postFlushValues = client.GetList<Double>(keyStoreName, "TestValueList");
            Assert.NotNull(postFlushValues);

            double pfTestValue = 0;
            for (int pfIndex = 0; pfIndex < postFlushValues.Count; pfIndex++)
            {
                Assert.Equal(pfTestValue, postFlushValues[pfIndex].Value);
                pfTestValue += 0.5f;
            }

            client.Disconnect();
        }

        [Fact(DisplayName = "Push values to bottom of List (DateTime).")]
        public void TestPersistentListOfDateTimes()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.ListOfDateTimes.Last";

            client.StoreCreate(new KkStoreConfiguration(keyStoreName)
            {
                CacheExpiration = TimeSpan.FromMinutes(1),
                PersistenceScheme = KkPersistenceScheme.Persistent,
                ValueType = KkValueType.ListOfDateTimes
            });

            var startDateTime = DateTime.UtcNow;

            //Push values to the bottom of the list.
            for (int i = 0; i < 100; i++)
            {
                var pushValue = startDateTime + TimeSpan.FromDays(i);

                client.PushLast(keyStoreName, "TestValueList", pushValue);
                var values = client.GetList<DateTime>(keyStoreName, "TestValueList");

                Assert.NotNull(values);
                Assert.Equal(i + 1, values.Count);

                for (int index = 0; index < values.Count; index++)
                {
                    var testValue = startDateTime + TimeSpan.FromDays(index);
                    Assert.Equal(testValue, values[index].Value);
                }
            }

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            var postFlushValues = client.GetList<DateTime>(keyStoreName, "TestValueList");
            Assert.NotNull(postFlushValues);

            for (int pfIndex = 0; pfIndex < postFlushValues.Count; pfIndex++)
            {
                var testValue = startDateTime + TimeSpan.FromDays(pfIndex);
                Assert.Equal(testValue, postFlushValues[pfIndex].Value);
            }

            client.Disconnect();
        }


        [Fact(DisplayName = "Push values to bottom of List (Guid).")]
        public void TestPersistentListOfGuids()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.ListOfGuids.Last";

            var testLookup = new Dictionary<int, Guid>();

            client.StoreCreate(new KkStoreConfiguration(keyStoreName)
            {
                CacheExpiration = TimeSpan.FromMinutes(1),
                PersistenceScheme = KkPersistenceScheme.Persistent,
                ValueType = KkValueType.ListOfGuids
            });

            //Push values to the bottom of the list.
            for (int i = 0; i < 100; i++)
            {
                var testGuid = Guid.NewGuid();
                testLookup.Add(i, testGuid);

                client.PushLast(keyStoreName, "TestValueList", testGuid);
                var values = client.GetList<Guid>(keyStoreName, "TestValueList");

                Assert.NotNull(values);
                Assert.Equal(i + 1, values.Count);

                for (int t = 0; t < values.Count; t++)
                {
                    Assert.Equal(testLookup[i], values[i].Value);
                }
            }

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            var postFlushValues = client.GetList<Guid>(keyStoreName, "TestValueList");
            Assert.NotNull(postFlushValues);

            for (int t = 0; t < postFlushValues.Count; t++)
            {
                Assert.Equal(testLookup[t], postFlushValues[t].Value);
            }

            client.Disconnect();
        }
    }
}
