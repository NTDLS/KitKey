using NTDLS.KitKey.Shared;

namespace KitKey.Tests.Unit
{
    public class ListPushLastAndGetFirstAndLast(ServerFixture fixture) : IClassFixture<ServerFixture>
    {
        [Fact(DisplayName = "PushLast to List, Test First & Last (String).")]
        public void TestPersistentListOfStrings()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.ListOfStrings.ListPushLastAndGetFirstAndLast";

            client.CreateStore(new KkStoreConfiguration(keyStoreName)
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

            var first = client.GetFirst<string>(keyStoreName, "TestValueList");
            Assert.NotNull(first);
            Assert.Equal("Value0", first.Value);

            var last = client.GetLast<string>(keyStoreName, "TestValueList");
            Assert.NotNull(last);
            Assert.Equal("Value99", last.Value);

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            first = client.GetFirst<string>(keyStoreName, "TestValueList");
            Assert.NotNull(first);
            Assert.Equal("Value0", first.Value);

            last = client.GetLast<string>(keyStoreName, "TestValueList");
            Assert.NotNull(last);
            Assert.Equal("Value99", last.Value);

            client.Disconnect();
        }

        [Fact(DisplayName = "PushLast to List, Test First & Last (Int32).")]
        public void TestPersistentListOfInt32s()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.ListOfInt32s.ListPushLastAndGetFirstAndLast";

            client.CreateStore(new KkStoreConfiguration(keyStoreName)
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

            var first = client.GetFirst<int>(keyStoreName, "TestValueList");
            Assert.NotNull(first);
            Assert.Equal(0, first.Value);

            var last = client.GetLast<int>(keyStoreName, "TestValueList");
            Assert.NotNull(last);
            Assert.Equal(99, last.Value);

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            first = client.GetFirst<int>(keyStoreName, "TestValueList");
            Assert.NotNull(first);
            Assert.Equal(0, first.Value);

            last = client.GetLast<int>(keyStoreName, "TestValueList");
            Assert.NotNull(last);
            Assert.Equal(99, last.Value);

            client.Disconnect();
        }

        [Fact(DisplayName = "PushLast to List, Test First & Last (Int64).")]
        public void TestPersistentListOfInt64s()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.ListOfInt64s.ListPushLastAndGetFirstAndLast";

            client.CreateStore(new KkStoreConfiguration(keyStoreName)
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

            var first = client.GetFirst<long>(keyStoreName, "TestValueList");
            Assert.NotNull(first);
            Assert.Equal(0, first.Value);

            var last = client.GetLast<long>(keyStoreName, "TestValueList");
            Assert.NotNull(last);
            Assert.Equal(99, last.Value);

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            first = client.GetFirst<long>(keyStoreName, "TestValueList");
            Assert.NotNull(first);
            Assert.Equal(0, first.Value);

            last = client.GetLast<long>(keyStoreName, "TestValueList");
            Assert.NotNull(last);
            Assert.Equal(99, last.Value);

            client.Disconnect();
        }

        [Fact(DisplayName = "PushLast to List, Test First & Last (Single).")]
        public void TestPersistentListOfSingles()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.ListOfSingles.ListPushLastAndGetFirstAndLast";

            client.CreateStore(new KkStoreConfiguration(keyStoreName)
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

            var first = client.GetFirst<float>(keyStoreName, "TestValueList");
            Assert.NotNull(first);
            Assert.Equal(0, first.Value);

            var last = client.GetLast<float>(keyStoreName, "TestValueList");
            Assert.NotNull(last);
            Assert.Equal(49.5, last.Value);

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            first = client.GetFirst<float>(keyStoreName, "TestValueList");
            Assert.NotNull(first);
            Assert.Equal(0, first.Value);

            last = client.GetLast<float>(keyStoreName, "TestValueList");
            Assert.NotNull(last);
            Assert.Equal(49.5, last.Value);

            client.Disconnect();
        }

        [Fact(DisplayName = "PushLast to List, Test First & Last (Double).")]
        public void TestPersistentListOfDoubles()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.ListOfDoubles.ListPushLastAndGetFirstAndLast";

            client.CreateStore(new KkStoreConfiguration(keyStoreName)
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
                var values = client.GetList<double>(keyStoreName, "TestValueList");

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

            var first = client.GetFirst<double>(keyStoreName, "TestValueList");
            Assert.NotNull(first);
            Assert.Equal(0, first.Value);

            var last = client.GetLast<double>(keyStoreName, "TestValueList");
            Assert.NotNull(last);
            Assert.Equal(49.5, last.Value);

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            first = client.GetFirst<double>(keyStoreName, "TestValueList");
            Assert.NotNull(first);
            Assert.Equal(0, first.Value);

            last = client.GetLast<double>(keyStoreName, "TestValueList");
            Assert.NotNull(last);
            Assert.Equal(49.5, last.Value);

            client.Disconnect();
        }

        [Fact(DisplayName = "PushLast to List, Test First & Last (DateTime).")]
        public void TestPersistentListOfDateTimes()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.ListOfDateTimes.ListPushLastAndGetFirstAndLast";

            client.CreateStore(new KkStoreConfiguration(keyStoreName)
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

            var first = client.GetFirst<DateTime>(keyStoreName, "TestValueList");
            Assert.NotNull(first);
            Assert.Equal(startDateTime, first.Value);

            var last = client.GetLast<DateTime>(keyStoreName, "TestValueList");
            Assert.NotNull(last);
            Assert.Equal(startDateTime + TimeSpan.FromDays(99), last.Value);

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            first = client.GetFirst<DateTime>(keyStoreName, "TestValueList");
            Assert.NotNull(first);
            Assert.Equal(startDateTime, first.Value);

            last = client.GetLast<DateTime>(keyStoreName, "TestValueList");
            Assert.NotNull(last);
            Assert.Equal(startDateTime + TimeSpan.FromDays(99), last.Value);

            client.Disconnect();
        }

        [Fact(DisplayName = "PushLast to List, Test First & Last (Guid).")]
        public void TestPersistentListOfGuids()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.ListOfGuids.ListPushLastAndGetFirstAndLast";

            var testLookup = new Dictionary<int, Guid>();

            client.CreateStore(new KkStoreConfiguration(keyStoreName)
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

            var first = client.GetFirst<Guid>(keyStoreName, "TestValueList");
            Assert.NotNull(first);
            Assert.Equal(testLookup[0], first.Value);

            var last = client.GetLast<Guid>(keyStoreName, "TestValueList");
            Assert.NotNull(last);
            Assert.Equal(testLookup[99], last.Value);

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            first = client.GetFirst<Guid>(keyStoreName, "TestValueList");
            Assert.NotNull(first);
            Assert.Equal(testLookup[0], first.Value);

            last = client.GetLast<Guid>(keyStoreName, "TestValueList");
            Assert.NotNull(last);
            Assert.Equal(testLookup[99], last.Value);

            client.Disconnect();
        }
    }
}
