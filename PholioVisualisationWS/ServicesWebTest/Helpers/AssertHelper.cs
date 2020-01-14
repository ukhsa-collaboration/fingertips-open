using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Parsers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PholioVisualisation.ServicesWebTest.Helpers
{
    public class AssertHelper
    {
        public static void AssertHeaderText(string content, string expectedHeaderText)
        {
            AssertContent(content, expectedHeaderText);

            // Assert: header is correct
            Assert.IsTrue(content.Contains(expectedHeaderText));

            AssertSameNumberOfFieldsInContentAsHeader(content, expectedHeaderText);
        }

        public static void AssertErrorMessagesContent(string content, string errorMessages, int expectedErrorNumber)
        {
            AssertContent(content, errorMessages);
            AssertExpectedNumberOfMessages(content, expectedErrorNumber);
        }

        public static void IsType(Type type, object objectTarget)
        {
            if (objectTarget != null && type != null )
            {
                if (objectTarget.GetType() == type)
                {
                    Assert.IsTrue(true);
                }
                else
                {
                    Assert.Fail("The object is not the same type");
                }
            }
            else
            {
                Assert.Fail("Null object cannot to be compared");
            }
        }

        /// <summary>
        /// Check if the target object is contained into the enumerable.
        /// </summary>
        /// <param name="enumerableObject">IEnumerable of any type or class</param>
        /// <param name="targetObject">Any type or class</param>
        public static void DoesContain<T>(IEnumerable<T> enumerableObject, T targetObject)
        {
            var enumerable = enumerableObject as T[] ?? enumerableObject.ToArray();
            var isContain = enumerable.Any();
            foreach (var element in enumerable)
            {
                isContain = AreEqualTwoObjects(element, targetObject);
                if (isContain) break;
            }
                
            if (!isContain)
            {
                Assert.Fail("Objects is not contained in the list");
            }
        }

        /// <summary>
        /// Check if the target object is contained into the enumerable.
        /// </summary>
        /// <param name="enumerableObject">IEnumerable of any type or class</param>
        /// <param name="targetObject">Any type or class</param>
        public static void DoesNotContain<T>(IEnumerable<T> enumerableObject, T targetObject)
        {
            var enumerable = enumerableObject as T[] ?? enumerableObject.ToArray();
            var isContain = enumerable.Any();

            foreach (var element in enumerable)
            {
                isContain = AreEqualTwoObjects(element, targetObject);
                if (!isContain) break;
            }

            if (isContain)
            {
                Assert.Fail("Objects is contained in the list");
            }
        }

        /// <summary>
        /// Make a deep recursively search for all objects properties, checking values and asserting fail when are not equal
        /// </summary>
        /// <param name="expectedObject">Any type or class</param>
        /// <param name="targetObject">Any type or class</param>
        public static void AreEqual<T>(T expectedObject, T targetObject)
        {
            var equal = AreEqualTwoObjects(expectedObject, targetObject);
            if (!equal)
            {
                Assert.Fail("Objects are not equals");
            }
        }

        /// <summary>
        /// Make a deep recursively search for all objects properties, checking values and asserting fail when are equal
        /// </summary>
        /// <param name="expectedObject">Any type or class</param>
        /// <param name="targetObject">Any type or class</param>
        public static void AreNotEqual<T>(T expectedObject, T targetObject)
        {
            var equal = AreEqualTwoObjects(expectedObject, targetObject);
            if (equal)
            {
                Assert.Fail("Objects are equals");
            }
        }

        private static bool AreEqualTwoObjects<T>(T expectedObject, T targetObject)
        {
            if (expectedObject == null || targetObject == null) return expectedObject == null && targetObject == null;

            if (IsPrimitiveType(expectedObject.GetType()))
            {
                return expectedObject.Equals(targetObject);
            }

            if (expectedObject.GetType().GetProperties().Length == 0)
            {
                Assert.Fail("The objects have not properties to be compared");
                return false;
            }

            foreach (var property in expectedObject.GetType().GetProperties())
            {
                var enumerableExpected = expectedObject as IEnumerable;
                var enumerableTarget = targetObject as IEnumerable;
                if (enumerableExpected != null || enumerableTarget != null)
                {
                    if (enumerableExpected == null || enumerableTarget == null) return false;

                    var enumerable = enumerableTarget as object[] ?? enumerableTarget.Cast<object>().ToArray();
                    var expected = enumerableExpected as object[] ?? enumerableExpected.Cast<object>().ToArray();
                    if (expected.Length != enumerable.Length) return false;

                    var expectedEnum = expected.GetEnumerator();
                    var targetEnum = enumerable.GetEnumerator();

                    var expectedValid = expectedEnum.MoveNext();
                    var targetsValid = targetEnum.MoveNext();
                    if (!expectedValid || !targetsValid)
                    {
                        return !expectedValid && !targetsValid;
                    }
                    while (expectedValid && targetsValid)
                    {
                        var areEqualInList = AreEqualTwoObjects(expectedEnum.Current, targetEnum.Current);

                        if (!areEqualInList) return false;

                        expectedValid = expectedEnum.MoveNext();
                        targetsValid = targetEnum.MoveNext();
                    }
                    return true;
                }

                var value1 = property.GetValue(expectedObject, null);
                var value2 = property.GetValue(targetObject, null);

                var areEqual = AreEqualTwoObjects(value1, value2);
                if (!areEqual)
                    return false;
            }
            return true;
        }


        private static bool IsPrimitiveType(Type propertyType)
        {
            var typeCode = Type.GetTypeCode(propertyType);
            switch (typeCode)
            {
                case TypeCode.String:
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Boolean:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.DBNull:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                    return true;
                case TypeCode.Empty:
                case TypeCode.Object:
                    return false;
                default:
                    return false;
            }
        }

        private static void AssertSameNumberOfFieldsInContentAsHeader(string content, string expectedHeaderText)
        {
            // Remove text between quotes as it may contain commas
            var contentQuoteTextRemoved = Regex.Replace(content, @"""[^""]+""", "");

            var commaCountContent = contentQuoteTextRemoved.Count(x => x == ',');
            var commaCountHeader = expectedHeaderText.Count(x => x == ',');

            Assert.AreEqual(0, commaCountContent % commaCountHeader);
        }

        private static void AssertExpectedNumberOfMessages(string content, int expectedErrorNumber)
        {
            if (content == null)
            {
                Assert.AreEqual(expectedErrorNumber, 0);
            }
            else
            {
                var listOfStringElements = new StringListParser(content).StringList;

                Assert.AreEqual(expectedErrorNumber, listOfStringElements.Count);
            }
        }

        private static void AssertContent(string content, string text)
        {
            Console.WriteLine(content);
            Console.WriteLine(text);

            Assert.IsNotNull(content);
            Assert.IsNotNull(text);
            // Assert: header is correct
            Assert.IsTrue(content.Contains(text));
        }
    }

    [TestClass]
    public class AssertHelperTest
    {
        private ChildTestClass _testClass1;
        private ChildTestClass _testClass2;
        private ChildTestClass _testClass3;
        private ChildTestClass _testClass4;
        private ChildTestClass _testClass5;

        [TestInitialize]
        public void StartUp()
        {
            _testClass1 = new ChildTestClass
            {
                StringTestValue = "testValue1",
                DateTimeValue = new DateTime(2019, 06, 13),
                DecimalValue = Decimal.Zero,
                BooleanValue = true,
                IntTestValue = Int16.MinValue,
                Int32TestValue = Int32.MinValue,
                Int64Value = Int64.MinValue,
                DbNullValue = null,
                CharValue = Char.MinValue,
                SByteValue = SByte.MinValue,
                ByteValue = Byte.MinValue,
                UInt16Value = UInt16.MinValue,
                UInt32Value = UInt32.MinValue,
                UInt64Value = UInt64.MinValue,
                SingleValue = Single.MinValue,
                DoubleValue = Double.NaN
            };
            _testClass2 = new ChildTestClass
            {
                StringTestValue = "testValue2",
                DateTimeValue = new DateTime(2019, 06, 14),
                DecimalValue = Decimal.One,
                BooleanValue = false,
                IntTestValue = Int16.MaxValue,
                Int32TestValue = Int32.MaxValue,
                Int64Value = Int64.MaxValue,
                DbNullValue = null,
                CharValue = Char.MaxValue,
                SByteValue = SByte.MaxValue,
                ByteValue = Byte.MaxValue,
                UInt16Value = UInt16.MaxValue,
                UInt32Value = UInt32.MaxValue,
                UInt64Value = UInt64.MaxValue,
                SingleValue = Single.MaxValue,
                DoubleValue = Double.MaxValue
            };
            _testClass3 = new ChildTestClass
            {
                StringTestValue = "testValue3",
                DateTimeValue = new DateTime(2019, 06, 15),
                DecimalValue = Decimal.MinusOne,
                BooleanValue = false,
                IntTestValue = -12345,
                Int32TestValue = -1234567890,
                Int64Value = -1234567890987654321,
                DbNullValue = null,
                CharValue = 'j',
                SByteValue = 32,
                ByteValue = 128,
                UInt16Value = 12345,
                UInt32Value = 1234567890,
                UInt64Value = 1234567890987654321,
                SingleValue = 2.356f,
                DoubleValue = Double.Epsilon
            };
            _testClass4 = new ChildTestClass
            {
                StringTestValue = "testValue1",
                DateTimeValue = new DateTime(2019, 06, 13),
                DecimalValue = Decimal.Zero,
                BooleanValue = true,
                IntTestValue = Int16.MinValue,
                Int32TestValue = Int32.MinValue,
                Int64Value = Int64.MinValue,
                DbNullValue = null,
                CharValue = Char.MinValue,
                SByteValue = SByte.MinValue,
                ByteValue = Byte.MinValue,
                UInt16Value = UInt16.MinValue,
                UInt32Value = UInt32.MinValue,
                UInt64Value = UInt64.MinValue,
                SingleValue = Single.MinValue,
                DoubleValue = Double.NaN
            };
            _testClass5 = new ChildTestClass
            {
                StringTestValue = "testValue1",
                DateTimeValue = new DateTime(2019, 06, 13),
                DecimalValue = Decimal.Zero,
                BooleanValue = true,
                IntTestValue = Int16.MinValue,
                Int32TestValue = Int32.MinValue,
                Int64Value = Int64.MinValue,
                DbNullValue = null,
                CharValue = Char.MinValue,
                SByteValue = SByte.MinValue,
                ByteValue = Byte.MinValue,
                UInt16Value = UInt16.MinValue,
                UInt32Value = UInt32.MinValue,
                UInt64Value = UInt64.MinValue,
                SingleValue = Single.MinValue,
                DoubleValue = Double.PositiveInfinity
            };
        }

        [TestMethod]
        public void ExampleOfAreEqualEnumerable()
        {
            var fatherTestList = new List<FatherTestClass>
            {
                new FatherTestClass { ChildTestClassList = null, ChildTestClass = null },
                new FatherTestClass { ChildTestClassList = null, ChildTestClass = _testClass1 },
                new FatherTestClass { ChildTestClassList = new List<ChildTestClass>(), ChildTestClass = null },
                new FatherTestClass { ChildTestClassList = new List<ChildTestClass>(), ChildTestClass = _testClass2 },
                new FatherTestClass { ChildTestClassList = new List<ChildTestClass>{ _testClass3 }, ChildTestClass = null },
                new FatherTestClass { ChildTestClassList = new List<ChildTestClass>{ _testClass1, _testClass3, _testClass2 }, ChildTestClass = null },
                new FatherTestClass { ChildTestClassList = new List<ChildTestClass>{ _testClass1, _testClass3, _testClass2 }, ChildTestClass = _testClass1 }
            };

            var fatherTestList2 = new List<FatherTestClass>
            {
                new FatherTestClass { ChildTestClassList = null, ChildTestClass = null },
                new FatherTestClass { ChildTestClassList = null, ChildTestClass = _testClass1 },
                new FatherTestClass { ChildTestClassList = new List<ChildTestClass>(), ChildTestClass = null },
                new FatherTestClass { ChildTestClassList = new List<ChildTestClass>(), ChildTestClass = _testClass2 },
                new FatherTestClass { ChildTestClassList = new List<ChildTestClass>{ _testClass3 }, ChildTestClass = null },
                new FatherTestClass { ChildTestClassList = new List<ChildTestClass>{ _testClass1, _testClass3, _testClass2 }, ChildTestClass = null },
                new FatherTestClass { ChildTestClassList = new List<ChildTestClass>{ _testClass1, _testClass3, _testClass2 }, ChildTestClass = _testClass1 }
            };

            AssertHelper.AreEqual(fatherTestList, fatherTestList2);
        }

        [TestMethod]
        public void ExampleOfAreDifferentEnumerable()
        {
            var fatherTestList = new List<FatherTestClass>();
            var fatherTestList2 = new List<FatherTestClass> { new FatherTestClass { ChildTestClassList = null, ChildTestClass = null } };
            var fatherTestList3 = new List<FatherTestClass> { new FatherTestClass { ChildTestClassList = null, ChildTestClass = _testClass1 } };
            var fatherTestList4 = new List<FatherTestClass> { new FatherTestClass { ChildTestClassList = new List<ChildTestClass>(), ChildTestClass = null } };
            var fatherTestList5 = new List<FatherTestClass> { new FatherTestClass { ChildTestClassList = new List<ChildTestClass>(), ChildTestClass = _testClass1 } };
            var fatherTestList6 = new List<FatherTestClass> { new FatherTestClass { ChildTestClassList = new List<ChildTestClass>{ _testClass1 }, ChildTestClass = _testClass1 } };
            var fatherTestList7 = new List<FatherTestClass> { new FatherTestClass { ChildTestClassList = new List<ChildTestClass>{ _testClass1 , _testClass2, _testClass3 }, ChildTestClass = _testClass1 } };
            var fatherTestList8 = new List<FatherTestClass> { new FatherTestClass { ChildTestClassList = new List<ChildTestClass> { _testClass1, _testClass2, _testClass3, _testClass3 }, ChildTestClass = _testClass2 } };
            var fatherTestList9 = new List<FatherTestClass> { new FatherTestClass { ChildTestClassList = new List<ChildTestClass> { _testClass1, _testClass2, _testClass3, _testClass3 }, ChildTestClass = _testClass1 } };
            var fatherTestList10 = new List<FatherTestClass> { new FatherTestClass { ChildTestClassList = new List<ChildTestClass> { _testClass1, _testClass2, _testClass3, _testClass3 }, ChildTestClass = _testClass5 } };
            var fatherTestList11 = new List<FatherTestClass> { new FatherTestClass { ChildTestClassList = new List<ChildTestClass> { _testClass2, _testClass3, _testClass3, _testClass1 }, ChildTestClass = _testClass5 } };
            var fatherTestList12 = new List<FatherTestClass> { new FatherTestClass { ChildTestClassList = new List<ChildTestClass> { _testClass2, _testClass3, _testClass3, _testClass5 }, ChildTestClass = _testClass5 } };

            AssertHelper.AreNotEqual(fatherTestList, fatherTestList2);
            AssertHelper.AreNotEqual(fatherTestList2, fatherTestList3);
            AssertHelper.AreNotEqual(fatherTestList3, fatherTestList4);
            AssertHelper.AreNotEqual(fatherTestList4, fatherTestList5);
            AssertHelper.AreNotEqual(fatherTestList5, fatherTestList6);
            AssertHelper.AreNotEqual(fatherTestList6, fatherTestList7);
            AssertHelper.AreNotEqual(fatherTestList7, fatherTestList8);
            AssertHelper.AreNotEqual(fatherTestList8, fatherTestList9);
            AssertHelper.AreNotEqual(fatherTestList9, fatherTestList10);
            AssertHelper.AreNotEqual(fatherTestList10, fatherTestList11);
            AssertHelper.AreNotEqual(fatherTestList11, fatherTestList12);
        }

        [TestMethod]
        public void ExampleOfAreEqualObject()
        {
            AssertHelper.AreEqual(_testClass1, _testClass4);
        }

        [TestMethod]
        public void ExampleOfAreDifferentObjects()
        {
            AssertHelper.AreNotEqual(_testClass1, _testClass5);
        }

        [TestMethod]
        public void ShouldContain()
        {
            var childTestClassList = new List<ChildTestClass> {_testClass1, _testClass2, _testClass3 };

            AssertHelper.DoesContain(childTestClassList, _testClass3);
        }

        [TestMethod]
        public void ShouldNotContain()
        {
            var childTestClassList = new List<ChildTestClass> { _testClass1, _testClass2 };

            AssertHelper.DoesNotContain(childTestClassList, _testClass3);
        }

        [TestMethod]
        public void ShouldContainNull()
        {
            var childTestClassList = new List<ChildTestClass> { _testClass1, _testClass2, _testClass3, null };

            AssertHelper.DoesContain(childTestClassList, null);
        }

        [TestMethod]
        public void ShouldNotContainNull()
        {
            var childTestClassList = new List<ChildTestClass>();

            AssertHelper.DoesNotContain(childTestClassList, null);
        }

        [TestMethod]
        public void ShouldNotContainElementInEmptyList()
        {
            var childTestClassList = new List<ChildTestClass>();

            AssertHelper.DoesNotContain(childTestClassList, _testClass1);
        }

        public class FatherTestClass
        {
            public IList<ChildTestClass> ChildTestClassList { get; set; }
            public ChildTestClass ChildTestClass { get; set; }
        }

        public class ChildTestClass
        {
            public string StringTestValue { get; set; }
            public DateTime DateTimeValue { get; set; }
            public Decimal DecimalValue { get; set; }
            public Boolean BooleanValue { get; set; }
            public int IntTestValue { get; set; }
            public Int32 Int32TestValue { get; set; }
            public Int64 Int64Value { get; set; }
            public DBNull DbNullValue { get; set; }
            public Char CharValue { get; set; }
            public SByte SByteValue { get; set; }
            public Byte ByteValue { get; set; }
            public UInt16 UInt16Value { get; set; }
            public UInt32 UInt32Value { get; set; }
            public UInt64 UInt64Value { get; set; }
            public Single SingleValue { get; set; }
            public Double DoubleValue { get; set; }
        }
    }
}
