using Example.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    internal class AllObjects
    {
        public static InBaseTestClass CreateInBaseTestClass()
        {
            return new InBaseTestClass
            {
                // --- InBaseTestClass own properties ---
                Count = int.MaxValue,
                IsActive = true,
                LargeNumber = long.MaxValue,
                Ratio = 999999999999.999999m,
                Name = short.MaxValue,
                TinyNumber = byte.MaxValue,
                LessPreciseFloat = float.MaxValue,
                DoublePrecision = double.MaxValue,
                SingleChar = char.MaxValue,
                SByteValue = sbyte.MaxValue,
                UShortValue = ushort.MaxValue,
                UIntValue = uint.MaxValue,
                ULongValue = ulong.MaxValue,
                NIntValue = nint.MaxValue,
                NUIntValue = nuint.MaxValue,
                OccurredOn = DateTime.MaxValue,
                OccurredAt = DateTimeOffset.MaxValue,
                Duration = new TimeSpan(0, 23, 59, 59, 999).Add(TimeSpan.FromTicks(9999)),
                UniqueId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                BinaryData = new byte[] { 0xFF, 0xFF, 0xFF },
                CharArray = new char[] { char.MaxValue, char.MaxValue, char.MaxValue },
                StatusType = StatusType.Active,
                NameList = new List<InBaseTestClass2>
                {
                    new InBaseTestClass2
                    {
                        Count6 = int.MaxValue,
                        IsActive6 = true,
                        LargeNumber6 = long.MaxValue,
                        Ratio6 = 999999999999.999999m,
                        Name6 = short.MaxValue,
                        TinyNumber6 = byte.MaxValue,
                        LessPreciseFloat6 = float.MaxValue,
                        DoublePrecision6 = double.MaxValue,
                        SingleChar6 = char.MaxValue,
                        SByteValue6 = sbyte.MaxValue,
                        UShortValue6 = ushort.MaxValue,
                        UIntValue6 = uint.MaxValue,
                        ULongValue6 = ulong.MaxValue,
                        NIntValue6 = nint.MaxValue,
                        NUIntValue6 = nuint.MaxValue,
                        OccurredOn6 = DateTime.MaxValue,
                        OccurredAt6 = DateTimeOffset.MaxValue,
                        Duration6 = new TimeSpan(0, 23, 59, 59, 999).Add(TimeSpan.FromTicks(9999)),
                        UniqueId6 = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                        StatusType6 = StatusType.Active
                    }
                },

                // Nullable properties
                Count1 = int.MaxValue,
                IsActive1 = true,
                LargeNumber1 = long.MaxValue,
                Ratio1 = 999999999999.999999m,
                Name1 = short.MaxValue,
                TinyNumber1 = byte.MaxValue,
                LessPreciseFloat1 = float.MaxValue,
                DoublePrecision1 = double.MaxValue,
                SingleChar1 = char.MaxValue,
                SByteValue1 = sbyte.MaxValue,
                UShortValue1 = ushort.MaxValue,
                UIntValue1 = uint.MaxValue,
                ULongValue1 = ulong.MaxValue,
                NIntValue1 = nint.MaxValue,
                NUIntValue1 = nuint.MaxValue,
                OccurredOn1 = DateTime.MaxValue,
                OccurredAt1 = DateTimeOffset.MaxValue,
                Duration1 = new TimeSpan(0, 23, 59, 59, 999).Add(TimeSpan.FromTicks(9999)),
                UniqueId1 = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                BinaryData1 = new byte[] { 0xFF, 0xFF },
                CharArray1 = new char[] { char.MaxValue, char.MaxValue },
                StatusType1 = StatusType.Active,
                NameList1 = new List<InBaseTestClass2>
                {
                    new InBaseTestClass2
                    {
                        Count6 = int.MaxValue,
                        IsActive6 = true,
                        LargeNumber6 = long.MaxValue,
                        Ratio6 = 999999999999.999999m,
                        Name6 = short.MaxValue,
                        TinyNumber6 = byte.MaxValue,
                        LessPreciseFloat6 = float.MaxValue,
                        DoublePrecision6 = double.MaxValue,
                        SingleChar6 = char.MaxValue,
                        SByteValue6 = sbyte.MaxValue,
                        UShortValue6 = ushort.MaxValue,
                        UIntValue6 = uint.MaxValue,
                        ULongValue6 = ulong.MaxValue,
                        NIntValue6 = nint.MaxValue,
                        NUIntValue6 = nuint.MaxValue,
                        OccurredOn6 = DateTime.MaxValue,
                        OccurredAt6 = DateTimeOffset.MaxValue,
                        Duration6 = new TimeSpan(0, 23, 59, 59, 999).Add(TimeSpan.FromTicks(9999)),
                        UniqueId6 = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                        StatusType6 = StatusType.Active
                    }
                },

                // OutParam properties (with ParamConfig DBParam + OutParam)
                Count2 = int.MaxValue,
                IsActive2 = true,
                LargeNumber2 = long.MaxValue,
                Ratio2 = 999999999999.999999m,
                Name2 = short.MaxValue,
                TinyNumber2 = byte.MaxValue,
                LessPreciseFloat2 = float.MaxValue,
                DoublePrecision2 = double.MaxValue,
                SingleChar2 = char.MaxValue,
                SByteValue2 = sbyte.MaxValue,
                UShortValue2 = ushort.MaxValue,
                UIntValue2 = uint.MaxValue,
                ULongValue2 = ulong.MaxValue,
                NIntValue2 = nint.MaxValue,
                NUIntValue2 = nuint.MaxValue,
                OccurredOn2 = DateTime.MaxValue,
                OccurredAt2 = DateTimeOffset.MaxValue,
                Duration2 = new TimeSpan(0, 23, 59, 59, 999).Add(TimeSpan.FromTicks(9999)),
                UniqueId2 = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                BinaryData2 = new byte[] { 0xFF, 0xFF, 0xFF },
                CharArray2 = new char[] { char.MaxValue, char.MaxValue, char.MaxValue },
                StatusType2 = StatusType.Active,

                // ParamExclusion properties
                Count3 = int.MaxValue,
                IsActive3 = true,
                LargeNumber3 = long.MaxValue,
                Ratio3 = 999999999999.999999m,
                Name3 = short.MaxValue,
                TinyNumber3 = byte.MaxValue,
                LessPreciseFloat3 = float.MaxValue,
                DoublePrecision3 = double.MaxValue,
                SingleChar3 = char.MaxValue,
                SByteValue3 = sbyte.MaxValue,
                UShortValue3 = ushort.MaxValue,
                UIntValue3 = uint.MaxValue,
                ULongValue3 = ulong.MaxValue,
                NIntValue3 = nint.MaxValue,
                NUIntValue3 = nuint.MaxValue,
                OccurredOn3 = DateTime.MaxValue,
                OccurredAt3 = DateTimeOffset.MaxValue,
                Duration3 = new TimeSpan(0, 23, 59, 59, 999).Add(TimeSpan.FromTicks(9999)),
                UniqueId3 = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                BinaryData3 = new byte[] { 0xFF, 0xFF },
                CharArray3 = new char[] { char.MaxValue, char.MaxValue },
                StatusType3 = StatusType.Active,
                NameList3 = new List<InBaseTestClass2>
                {
                    new InBaseTestClass2
                    {
                        Count6 = int.MaxValue,
                        IsActive6 = true,
                        LargeNumber6 = long.MaxValue,
                        Ratio6 = 999999999999.999999m,
                        Name6 = short.MaxValue,
                        TinyNumber6 = byte.MaxValue,
                        LessPreciseFloat6 = float.MaxValue,
                        DoublePrecision6 = double.MaxValue,
                        SingleChar6 = char.MaxValue,
                        SByteValue6 = sbyte.MaxValue,
                        UShortValue6 = ushort.MaxValue,
                        UIntValue6 = uint.MaxValue,
                        ULongValue6 = ulong.MaxValue,
                        NIntValue6 = nint.MaxValue,
                        NUIntValue6 = nuint.MaxValue,
                        OccurredOn6 = DateTime.MaxValue,
                        OccurredAt6 = DateTimeOffset.MaxValue,
                        Duration6 = new TimeSpan(0, 23, 59, 59, 999).Add(TimeSpan.FromTicks(9999)),
                        UniqueId6 = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                        StatusType6 = StatusType.Active
                    }
                },

                // Override properties (from InBaseTestClass1 virtual -> InBaseTestClass override)
                Count4 = int.MaxValue,
                IsActive4 = true,
                LargeNumber4 = long.MaxValue,
                Ratio4 = 999999999999.999999m,
                Name4 = short.MaxValue,
                TinyNumber4 = byte.MaxValue,
                LessPreciseFloat4 = float.MaxValue,
                DoublePrecision4 = double.MaxValue,
                SingleChar4 = char.MaxValue,
                SByteValue4 = sbyte.MaxValue,
                UShortValue4 = ushort.MaxValue,
                UIntValue4 = uint.MaxValue,
                ULongValue4 = ulong.MaxValue,
                NIntValue4 = nint.MaxValue,
                NUIntValue4 = nuint.MaxValue,
                OccurredOn4 = DateTime.MaxValue,
                OccurredAt4 = DateTimeOffset.MaxValue,
                Duration4 = new TimeSpan(0, 23, 59, 59, 999).Add(TimeSpan.FromTicks(9999)),
                UniqueId4 = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                StatusType4 = StatusType.Active,

                // --- Inherited from InBaseTestClass1 (non-virtual) ---
                Count5 = int.MaxValue,
                IsActive5 = true,
                LargeNumber5 = long.MaxValue,
                Ratio5 = 999999999999.999999m,
                Name5 = short.MaxValue,
                TinyNumber5 = byte.MaxValue,
                LessPreciseFloat5 = float.MaxValue,
                DoublePrecision5 = double.MaxValue,
                SingleChar5 = char.MaxValue,
                SByteValue5 = sbyte.MaxValue,
                UShortValue5 = ushort.MaxValue,
                UIntValue5 = uint.MaxValue,
                ULongValue5 = ulong.MaxValue,
                NIntValue5 = nint.MaxValue,
                NUIntValue5 = nuint.MaxValue,
                OccurredOn5 = DateTime.MaxValue,
                OccurredAt5 = DateTimeOffset.MaxValue,
                Duration5 = new TimeSpan(0, 23, 59, 59, 999).Add(TimeSpan.FromTicks(9999)),
                UniqueId5 = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                StatusType5 = StatusType.Active
            };
        }

        public static TestClass111 CreateTestClass111()
        {
            return new TestClass111
            {
                HeaderId = int.MaxValue,
                Name = "MaxTestName",
                MyName = "MaxMyName",
                Count = int.MaxValue,
                IsActive = "true",
                LargeNumber = long.MaxValue,
                Ratio = 999999999999.999999m,
                BinaryData = new byte[] { 0xFF, 0xFF, 0xFF },
                OccurredOn = DateTime.MaxValue,
                OccurredAt = DateTimeOffset.MaxValue,
                Duration = new TimeSpan(0, 23, 59, 59, 999).Add(TimeSpan.FromTicks(9999))
            };
        }

        public static TestClass111 CreateTestClass111ForTest4()
        {
            return new TestClass111
            {
                HeaderId = 1,
                Name = "Test4Name",
                MyName = "Test4MyName",
                Count = 5,
                IsActive = "true",
                LargeNumber = 5000L,
                Ratio = 50.50m,
                BinaryData = new byte[] { 0xEE, 0xFF },
                OccurredOn = new DateTime(2025, 5, 1),
                OccurredAt = new DateTimeOffset(2025, 5, 1, 12, 0, 0, TimeSpan.Zero),
                Duration = TimeSpan.FromHours(1)
            };
        }

        public static TestClass111 CreateTestClass111ForTest6(int conditionValue)
        {
            return new TestClass111
            {
                HeaderId = conditionValue,
                Name = "Test6Name",
                MyName = "Test6MyName",
                Count = 10,
                IsActive = "true",
                LargeNumber = 6000L,
                Ratio = 60.60m,
                BinaryData = new byte[] { 0xAA },
                OccurredOn = new DateTime(2025, 6, 1),
                OccurredAt = new DateTimeOffset(2025, 6, 1, 10, 0, 0, TimeSpan.Zero),
                Duration = TimeSpan.FromMinutes(30)
            };
        }
    }
}
