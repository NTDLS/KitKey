using NTDLS.KitKey.Shared;

namespace KitKey.Tests.Unit
{
    public class ListPushLastRemoveAndGet(ServerFixture fixture) : IClassFixture<ServerFixture>
    {
        [Fact(DisplayName = "PushLast values to List (String).")]
        public void TestPersistentListOfStrings()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.ListOfStrings.ListPushLastRemoveAndGet";

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

            var valuesForRemove = client.GetList<string>(keyStoreName, "TestValueList");
            Assert.NotNull(valuesForRemove);
            Assert.Equal(100, valuesForRemove.Count);
            client.RemoveListItemByKey(keyStoreName, "TestValueList", valuesForRemove[50].Id);

            valuesForRemove = client.GetList<string>(keyStoreName, "TestValueList");
            Assert.NotNull(valuesForRemove);
            Assert.Equal(99, valuesForRemove.Count);

            var postFlushValues = client.GetList<string>(keyStoreName, "TestValueList");
            Assert.NotNull(postFlushValues);

            for (int t = 0; t < postFlushValues.Count; t++)
            {
                if (t >= 50)
                {
                    Assert.Equal($"Value{t + 1}", postFlushValues[t].Value);
                }
                else
                {
                    Assert.Equal($"Value{t}", postFlushValues[t].Value);
                }
            }

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            postFlushValues = client.GetList<string>(keyStoreName, "TestValueList");
            Assert.NotNull(postFlushValues);

            for (int t = 0; t < postFlushValues.Count; t++)
            {
                if (t >= 50)
                {
                    Assert.Equal($"Value{t + 1}", postFlushValues[t].Value);
                }
                else
                {
                    Assert.Equal($"Value{t}", postFlushValues[t].Value);
                }
            }

            client.Disconnect();
        }

        [Fact(DisplayName = "PushLast values to List (Int32).")]
        public void TestPersistentListOfInt32s()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.ListOfInt32s.ListPushLastRemoveAndGet";

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

            var valuesForRemove = client.GetList<int>(keyStoreName, "TestValueList");
            Assert.NotNull(valuesForRemove);
            Assert.Equal(100, valuesForRemove.Count);
            client.RemoveListItemByKey(keyStoreName, "TestValueList", valuesForRemove[50].Id);

            valuesForRemove = client.GetList<int>(keyStoreName, "TestValueList");
            Assert.NotNull(valuesForRemove);
            Assert.Equal(99, valuesForRemove.Count);

            var postFlushValues = client.GetList<int>(keyStoreName, "TestValueList");
            Assert.NotNull(postFlushValues);

            for (int t = 0; t < postFlushValues.Count; t++)
            {
                if (t >= 50)
                {
                    Assert.Equal(t + 1, postFlushValues[t].Value);
                }
                else
                {
                    Assert.Equal(t, postFlushValues[t].Value);
                }
            }

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            postFlushValues = client.GetList<int>(keyStoreName, "TestValueList");
            Assert.NotNull(postFlushValues);

            for (int t = 0; t < postFlushValues.Count; t++)
            {
                if (t >= 50)
                {
                    Assert.Equal(t + 1, postFlushValues[t].Value);
                }
                else
                {
                    Assert.Equal(t, postFlushValues[t].Value);
                }
            }

            client.Disconnect();
        }

        [Fact(DisplayName = "PushLast values to List (Int64).")]
        public void TestPersistentListOfInt64s()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.ListOfInt64s.ListPushLastRemoveAndGet";

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

            var valuesForRemove = client.GetList<long>(keyStoreName, "TestValueList");
            Assert.NotNull(valuesForRemove);
            Assert.Equal(100, valuesForRemove.Count);
            client.RemoveListItemByKey(keyStoreName, "TestValueList", valuesForRemove[50].Id);

            valuesForRemove = client.GetList<long>(keyStoreName, "TestValueList");
            Assert.NotNull(valuesForRemove);
            Assert.Equal(99, valuesForRemove.Count);

            var postFlushValues = client.GetList<long>(keyStoreName, "TestValueList");
            Assert.NotNull(postFlushValues);

            for (int t = 0; t < postFlushValues.Count; t++)
            {
                if (t >= 50)
                {
                    Assert.Equal(t + 1, postFlushValues[t].Value);
                }
                else
                {
                    Assert.Equal(t, postFlushValues[t].Value);
                }
            }

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            postFlushValues = client.GetList<long>(keyStoreName, "TestValueList");
            Assert.NotNull(postFlushValues);

            for (int t = 0; t < postFlushValues.Count; t++)
            {
                if (t >= 50)
                {
                    Assert.Equal(t + 1, postFlushValues[t].Value);
                }
                else
                {
                    Assert.Equal(t, postFlushValues[t].Value);
                }
            }

            client.Disconnect();
        }

        [Fact(DisplayName = "PushLast values to List (Single).")]
        public void TestPersistentListOfSingles()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.ListOfSingles.ListPushLastRemoveAndGet";

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

            var valuesForRemove = client.GetList<float>(keyStoreName, "TestValueList");
            Assert.NotNull(valuesForRemove);
            Assert.Equal(100, valuesForRemove.Count);
            client.RemoveListItemByKey(keyStoreName, "TestValueList", valuesForRemove[50].Id);

            valuesForRemove = client.GetList<float>(keyStoreName, "TestValueList");
            Assert.NotNull(valuesForRemove);
            Assert.Equal(99, valuesForRemove.Count);

            var postFlushValues = client.GetList<float>(keyStoreName, "TestValueList");
            Assert.NotNull(postFlushValues);

            float fpTestValue = 0;
            for (int t = 0; t < postFlushValues.Count; t++)
            {
                if (t >= 50)
                {
                    Assert.Equal(fpTestValue + 0.5f, postFlushValues[t].Value);
                }
                else
                {
                    Assert.Equal(fpTestValue, postFlushValues[t].Value);
                }
                fpTestValue += 0.5f;
            }

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            postFlushValues = client.GetList<float>(keyStoreName, "TestValueList");
            Assert.NotNull(postFlushValues);

            fpTestValue = 0;
            for (int t = 0; t < postFlushValues.Count; t++)
            {
                if (t >= 50)
                {
                    Assert.Equal(fpTestValue + 0.5f, postFlushValues[t].Value);
                }
                else
                {
                    Assert.Equal(fpTestValue, postFlushValues[t].Value);
                }
                fpTestValue += 0.5f;
            }

            client.Disconnect();
        }

        [Fact(DisplayName = "PushLast values to List (Double).")]
        public void TestPersistentListOfDoubles()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.ListOfDoubles.ListPushLastRemoveAndGet";

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

            var valuesForRemove = client.GetList<double>(keyStoreName, "TestValueList");
            Assert.NotNull(valuesForRemove);
            Assert.Equal(100, valuesForRemove.Count);
            client.RemoveListItemByKey(keyStoreName, "TestValueList", valuesForRemove[50].Id);

            valuesForRemove = client.GetList<double>(keyStoreName, "TestValueList");
            Assert.NotNull(valuesForRemove);
            Assert.Equal(99, valuesForRemove.Count);

            var postFlushValues = client.GetList<double>(keyStoreName, "TestValueList");
            Assert.NotNull(postFlushValues);

            double fpTestValue = 0;
            for (int t = 0; t < postFlushValues.Count; t++)
            {
                if (t >= 50)
                {
                    Assert.Equal(fpTestValue + 0.5, postFlushValues[t].Value);
                }
                else
                {
                    Assert.Equal(fpTestValue, postFlushValues[t].Value);
                }
                fpTestValue += 0.5;
            }

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            postFlushValues = client.GetList<double>(keyStoreName, "TestValueList");
            Assert.NotNull(postFlushValues);

            fpTestValue = 0;
            for (int t = 0; t < postFlushValues.Count; t++)
            {
                if (t >= 50)
                {
                    Assert.Equal(fpTestValue + 0.5, postFlushValues[t].Value);
                }
                else
                {
                    Assert.Equal(fpTestValue, postFlushValues[t].Value);
                }
                fpTestValue += 0.5;
            }

            client.Disconnect();
        }

        [Fact(DisplayName = "PushLast values to List (DateTime).")]
        public void TestPersistentListOfDateTimes()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.ListOfDateTimes.ListPushLastRemoveAndGet";

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

            var valuesForRemove = client.GetList<DateTime>(keyStoreName, "TestValueList");
            Assert.NotNull(valuesForRemove);
            Assert.Equal(100, valuesForRemove.Count);
            client.RemoveListItemByKey(keyStoreName, "TestValueList", valuesForRemove[50].Id);

            valuesForRemove = client.GetList<DateTime>(keyStoreName, "TestValueList");
            Assert.NotNull(valuesForRemove);
            Assert.Equal(99, valuesForRemove.Count);

            var postFlushValues = client.GetList<DateTime>(keyStoreName, "TestValueList");
            Assert.NotNull(postFlushValues);

            for (int t = 0; t < postFlushValues.Count; t++)
            {
                if (t >= 50)
                {
                    var testValue = startDateTime + TimeSpan.FromDays(t + 1);
                    Assert.Equal(testValue, postFlushValues[t].Value);
                }
                else
                {
                    var testValue = startDateTime + TimeSpan.FromDays(t);
                    Assert.Equal(testValue, postFlushValues[t].Value);
                }
            }

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            postFlushValues = client.GetList<DateTime>(keyStoreName, "TestValueList");
            Assert.NotNull(postFlushValues);

            for (int t = 0; t < postFlushValues.Count; t++)
            {
                if (t >= 50)
                {
                    var testValue = startDateTime + TimeSpan.FromDays(t + 1);
                    Assert.Equal(testValue, postFlushValues[t].Value);
                }
                else
                {
                    var testValue = startDateTime + TimeSpan.FromDays(t);
                    Assert.Equal(testValue, postFlushValues[t].Value);
                }
            }

            client.Disconnect();
        }

        [Fact(DisplayName = "PushLast values to List (Guid).")]
        public void TestPersistentListOfGuids()
        {
            var client = ClientFactory.CreateAndConnect();

            var keyStoreName = "Test.ListOfGuids.ListPushLastRemoveAndGet";

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

            var valuesForRemove = client.GetList<Guid>(keyStoreName, "TestValueList");
            Assert.NotNull(valuesForRemove);
            Assert.Equal(100, valuesForRemove.Count);
            client.RemoveListItemByKey(keyStoreName, "TestValueList", valuesForRemove[50].Id);

            valuesForRemove = client.GetList<Guid>(keyStoreName, "TestValueList");
            Assert.NotNull(valuesForRemove);
            Assert.Equal(99, valuesForRemove.Count);

            var postFlushValues = client.GetList<Guid>(keyStoreName, "TestValueList");
            Assert.NotNull(postFlushValues);

            for (int t = 0; t < postFlushValues.Count; t++)
            {
                if (t >= 50)
                {
                    Assert.Equal(testLookup[t + 1], postFlushValues[t].Value);
                }
                else
                {
                    Assert.Equal(testLookup[t], postFlushValues[t].Value);

                }
            }

            //Flush the cache so we can test persistence.
            client.FlushCache(keyStoreName);

            postFlushValues = client.GetList<Guid>(keyStoreName, "TestValueList");
            Assert.NotNull(postFlushValues);

            for (int t = 0; t < postFlushValues.Count; t++)
            {
                if (t >= 50)
                {
                    Assert.Equal(testLookup[t + 1], postFlushValues[t].Value);
                }
                else
                {
                    Assert.Equal(testLookup[t], postFlushValues[t].Value);

                }
            }

            client.Disconnect();
        }
    }
}
