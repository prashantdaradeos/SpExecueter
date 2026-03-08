using Microsoft.Data.SqlClient;
using SpExecuter.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Example.DataAccess;


#region Stored‑procedure handler interfaces

[SpHandler(Lifetime.Transient)]
public interface ITransientSpExecutor
{
    [StoredProcedure("TransientTest1")]
    Task<BaseClass0> Test1_With_Single(string connectionString, TestClass parameters);
    [StoredProcedure("TransientTest2")]
    Task<List<TestClass>> Test2_With_List(string connectionString, HeaderParameters parameters);
    [StoredProcedure("TransientTest3")]
    Task<(TestClass, HeaderResult)> Test3_With_TupleContainingSingle(string connectionString);
    [StoredProcedure("TransientTest4")]
    Task<(List<TestClass>, List<HeaderResult>)> Test4_With_TupleContaing2List(string connectionString, HeaderParameters parameters);
    [StoredProcedure("TransientTest5")]
    Task<(List<HeaderResult>, TestClass)> Test5_With_TupleContaingSingleNList(string connectionString, HeaderParameters parameters);

    [StoredProcedure("TransientTest6")]
    Task<((List<TestClass1>, HeaderResult)?, TestClass1)> Test6_With_Tuple_N_Single(string connectionString, HeaderParameters parameters);

    [StoredProcedure("TransientTest7")]
    Task<((HeaderResult, List<Record4Result>)?, List<HeaderResult>)> Test7_With_Tuple_N_List(string connectionString, HeaderParameters parameters);

    [StoredProcedure("TransientTest8")]
    Task<((List<TestClass1>, TestClass2)?,
        (List<TestClass1>, TestClass2, List<TestClass3>, TestClass8)?
        , (List<TestClass1>, TestClass10, List<TestClass11>, TestClass12)?)> Test8_With_Tuple_Single_List(string connectionString, HeaderParameters parameters);

    [StoredProcedure("TransientTest9")]
    Task<((List<HeaderResult>, TestClass1)?,
          (List<HeaderResult>, List<Record4Result>, TestClass1)?,
          List<HeaderResult>)> Test9_With_2Tuple_Single(string connectionString, HeaderParameters parameters);


    [StoredProcedure("TransientTest10", ConditionType = ConditionType.OR)]
    Task<(List<HeaderResult>, TestClass1)> Test10_With_OR(string connectionString, HeaderParameters parameters);


}
[SpHandler(Lifetime.Scoped)]
public interface IScopedSpExecutor
{
    // === BASICS ===

    // Single record
    [StoredProcedure("ScopedTest1")]
    Task<HeaderResult> Test1_Single(string connectionString, HeaderParameters parameters);

    // Single no params
    [StoredProcedure("ScopedTest2")]
    Task<Record4Result> Test2_Single_NoParams(string connectionString);

    // List
    [StoredProcedure("ScopedTest3")]
    Task<List<HeaderResult>> Test3_List(string connectionString, HeaderParameters parameters);

    // List no params
    [StoredProcedure("ScopedTest4")]
    Task<List<AuditInfoResult>> Test4_List_NoParams(string connectionString);

    // === FLAT TUPLES (varying sizes) ===

    // Flat tuple: 2 singles
    [StoredProcedure("ScopedTest5")]
    Task<(HeaderResult, Record4Result)> Test5_Flat_2Singles(string connectionString, HeaderParameters parameters);

    // Flat tuple: 2 lists
    [StoredProcedure("ScopedTest6")]
    Task<(List<HeaderResult>, List<Record4Result>)> Test6_Flat_2Lists(string connectionString, HeaderParameters parameters);

    // Flat tuple: 3 mixed (single, list, single)
    [StoredProcedure("ScopedTest7")]
    Task<(TestClass1, List<HeaderResult>, AuditInfoResult)> Test7_Flat_3Mixed(string connectionString, HeaderParameters parameters);

    // Flat tuple: 4 alternating (list, single, list, single)
    [StoredProcedure("ScopedTest8")]
    Task<(List<TestClass1>, Record4Result, List<AuditInfoResult>, TestClass2)> Test8_Flat_4Alternating(string connectionString, HeaderParameters parameters);

    // Flat tuple: 5 elements (single, list, single, list, single)
    [StoredProcedure("ScopedTest9")]
    Task<(TestClass3, List<TestClass4>, TestClass5, List<TestClass6>, TestClass7)> Test9_Flat_5Elements(string connectionString, HeaderParameters parameters);

    // Flat tuple: 5 all lists
    [StoredProcedure("ScopedTest10")]
    Task<(List<HeaderResult>, List<Record4Result>, List<AuditInfoResult>, List<TestClass1>, List<TestClass2>)> Test10_Flat_5AllLists(string connectionString, HeaderParameters parameters);

    // Flat tuple: 5 all singles
    [StoredProcedure("ScopedTest11")]
    Task<(HeaderResult, Record4Result, AuditInfoResult, TestClass1, TestClass2)> Test11_Flat_5AllSingles(string connectionString, HeaderParameters parameters);

    // === 1 NESTED TUPLE (varying inner lengths 2–4) ===

    // 1 nested(2 elem) + single
    [StoredProcedure("ScopedTest12")]
    Task<((List<TestClass1>, HeaderResult)?, TestClass2)> Test12_1Nested_2Elem_Single(string connectionString, HeaderParameters parameters);

    // 1 nested(2 elem) + list
    [StoredProcedure("ScopedTest13")]
    Task<((List<TestClass3>, TestClass4)?, List<TestClass5>)> Test13_1Nested_2Elem_List(string connectionString, HeaderParameters parameters);

    // 1 nested(3 elem) + single
    [StoredProcedure("ScopedTest14")]
    Task<((TestClass6, List<TestClass7>, TestClass8)?, TestClass9)> Test14_1Nested_3Elem_Single(string connectionString, HeaderParameters parameters);

    // 1 nested(4 elem) + list
    [StoredProcedure("ScopedTest15")]
    Task<((List<TestClass1>, TestClass2, List<TestClass3>, TestClass4)?, List<TestClass5>)> Test15_1Nested_4Elem_List(string connectionString, HeaderParameters parameters);

    // === 2 NESTED TUPLES (different inner lengths) ===

    // 2 nested: (2, 2) same + single
    [StoredProcedure("ScopedTest16")]
    Task<((List<TestClass1>, TestClass2)?,
        (TestClass3, List<TestClass4>)?,
        TestClass5)> Test16_2Nested_2_2_Single(string connectionString, HeaderParameters parameters);

    // 2 nested: (2, 3) incremental + list
    [StoredProcedure("ScopedTest17")]
    Task<((HeaderResult, List<TestClass1>)?,
        (TestClass2, List<TestClass3>, TestClass4)?,
        List<TestClass5>)> Test17_2Nested_2_3_List(string connectionString, HeaderParameters parameters);

    // 2 nested: (2, 4) incremental + single
    [StoredProcedure("ScopedTest18")]
    Task<((List<TestClass6>, TestClass7)?,
        (TestClass8, List<TestClass9>, TestClass10, List<TestClass1>)?,
        TestClass2)> Test18_2Nested_Inc_2_4_Single(string connectionString, HeaderParameters parameters);

    // 2 nested: (4, 2) decremental + list
    [StoredProcedure("ScopedTest19")]
    Task<((List<TestClass3>, TestClass4, List<TestClass5>, TestClass6)?,
        (TestClass7, List<TestClass8>)?,
        List<HeaderResult>)> Test19_2Nested_Dec_4_2_List(string connectionString, HeaderParameters parameters);

    // 2 nested: (3, 3) same + single
    [StoredProcedure("ScopedTest20")]
    Task<((TestClass9, List<TestClass10>, TestClass1)?,
        (List<TestClass2>, TestClass3, List<TestClass4>)?,
        Record4Result)> Test20_2Nested_3_3_Single(string connectionString, HeaderParameters parameters);

    // 2 nested: (4, 4) max-max + list
    [StoredProcedure("ScopedTest21")]
    Task<((List<TestClass5>, TestClass6, List<TestClass7>, TestClass8)?,
        (TestClass9, List<TestClass10>, TestClass1, List<TestClass2>)?,
        List<AuditInfoResult>)> Test21_2Nested_4_4_List(string connectionString, HeaderParameters parameters);

    // 2 nested: (3, 4) + single
    [StoredProcedure("ScopedTest22")]
    Task<((List<TestClass3>, TestClass4, List<TestClass5>)?,
        (TestClass6, List<TestClass7>, TestClass8, List<TestClass9>)?,
        TestClass10)> Test22_2Nested_3_4_Single(string connectionString, HeaderParameters parameters);

    // 2 nested: (4, 3) + list
    [StoredProcedure("ScopedTest23")]
    Task<((TestClass1, List<TestClass2>, TestClass3, List<TestClass4>)?,
        (List<TestClass5>, TestClass6, List<TestClass7>)?,
        List<TestClass8>)> Test23_2Nested_4_3_List(string connectionString, HeaderParameters parameters);

    // === 3 NESTED TUPLES (varying inner lengths) ===

    // 3 nested: incremental (2, 3, 4) + single
    [StoredProcedure("ScopedTest24")]
    Task<((List<TestClass1>, TestClass2)?,
        (TestClass3, List<TestClass4>, TestClass5)?,
        (List<TestClass6>, TestClass7, List<TestClass8>, TestClass9)?,
        TestClass10)> Test24_3Nested_Inc_2_3_4_Single(string connectionString, HeaderParameters parameters);

    // 3 nested: decremental (4, 3, 2) + list
    [StoredProcedure("ScopedTest25")]
    Task<((List<TestClass1>, TestClass2, List<TestClass3>, TestClass4)?,
        (TestClass5, List<TestClass6>, TestClass7)?,
        (List<TestClass8>, TestClass9)?,
        List<HeaderResult>)> Test25_3Nested_Dec_4_3_2_List(string connectionString, HeaderParameters parameters);

    // 3 nested: spike (2, 4, 2) — short-long-short + single
    [StoredProcedure("ScopedTest26")]
    Task<((TestClass10, List<TestClass1>)?,
        (List<TestClass2>, TestClass3, List<TestClass4>, TestClass5)?,
        (TestClass6, List<TestClass7>)?,
        TestClass8)> Test26_3Nested_Spike_2_4_2_Single(string connectionString, HeaderParameters parameters);

    // 3 nested: valley (4, 2, 4) — long-short-long + list
    [StoredProcedure("ScopedTest27")]
    Task<((TestClass9, List<TestClass10>, TestClass1, List<TestClass2>)?,
        (List<TestClass3>, TestClass4)?,
        (TestClass5, List<TestClass6>, TestClass7, List<TestClass8>)?,
        List<Record4Result>)> Test27_3Nested_Valley_4_2_4_List(string connectionString, HeaderParameters parameters);

    // 3 nested: all same (3, 3, 3) + single
    [StoredProcedure("ScopedTest28")]
    Task<((List<TestClass9>, TestClass10, List<TestClass1>)?,
        (TestClass2, List<TestClass3>, TestClass4)?,
        (List<TestClass5>, TestClass6, List<TestClass7>)?,
        TestClass8)> Test28_3Nested_AllSame_3_3_3_Single(string connectionString, HeaderParameters parameters);

    // 3 nested: all max (4, 4, 4) + list
    [StoredProcedure("ScopedTest29")]
    Task<((List<TestClass9>, TestClass10, List<TestClass1>, TestClass2)?,
        (TestClass3, List<TestClass4>, TestClass5, List<TestClass6>)?,
        (List<TestClass7>, TestClass8, List<TestClass9>, TestClass10)?,
        List<AuditInfoResult>)> Test29_3Nested_AllMax_4_4_4_List(string connectionString, HeaderParameters parameters);

    // 3 nested: staircase (2, 3, 2) + single
    [StoredProcedure("ScopedTest30")]
    Task<((TestClass1, List<TestClass2>)?,
        (List<TestClass3>, TestClass4, List<TestClass5>)?,
        (TestClass6, List<TestClass7>)?,
        AuditInfoResult)> Test30_3Nested_Staircase_2_3_2_Single(string connectionString, HeaderParameters parameters);

    // === 4 NESTED TUPLES ===

    // 4 nested: incremental (2, 2, 3, 4) + single
    [StoredProcedure("ScopedTest31")]
    Task<((List<TestClass3>, TestClass4)?,
        (TestClass5, List<TestClass6>)?,
        (List<TestClass7>, TestClass8, List<TestClass9>)?,
        (TestClass10, List<TestClass1>, TestClass2, List<TestClass3>)?,
        HeaderResult)> Test31_4Nested_Inc_2_2_3_4_Single(string connectionString, HeaderParameters parameters);

    // 4 nested: decremental (4, 3, 2, 2) + list
    [StoredProcedure("ScopedTest32")]
    Task<((List<TestClass4>, TestClass5, List<TestClass6>, TestClass7)?,
        (TestClass8, List<TestClass9>, TestClass10)?,
        (List<TestClass1>, TestClass2)?,
        (TestClass3, List<TestClass4>)?,
        List<Record4Result>)> Test32_4Nested_Dec_4_3_2_2_List(string connectionString, HeaderParameters parameters);

    // 4 nested: alternating (2, 4, 2, 4) + single
    [StoredProcedure("ScopedTest33")]
    Task<((TestClass5, List<TestClass6>)?,
        (List<TestClass7>, TestClass8, List<TestClass9>, TestClass10)?,
        (TestClass1, List<TestClass2>)?,
        (List<TestClass3>, TestClass4, List<TestClass5>, TestClass6)?,
        AuditInfoResult)> Test33_4Nested_Alt_2_4_2_4_Single(string connectionString, HeaderParameters parameters);

    // 4 nested: bookend (4, 2, 2, 4) + list
    [StoredProcedure("ScopedTest34")]
    Task<((List<TestClass7>, TestClass8, List<TestClass9>, TestClass10)?,
        (TestClass1, List<TestClass2>)?,
        (List<TestClass3>, TestClass4)?,
        (TestClass5, List<TestClass6>, TestClass7, List<TestClass8>)?,
        List<HeaderResult>)> Test34_4Nested_Bookend_4_2_2_4_List(string connectionString, HeaderParameters parameters);

    // 4 nested: all same (3, 3, 3, 3) + single
    [StoredProcedure("ScopedTest35")]
    Task<((TestClass9, TestClass10, TestClass1)?,
        (List<TestClass2>, TestClass3, List<TestClass4>)?,
        (TestClass5, List<TestClass6>, TestClass7)?,
        (List<TestClass8>, TestClass9, List<TestClass10>)?,
        Record4Result)> Test35_4Nested_AllSame_3_3_3_3_Single(string connectionString, HeaderParameters parameters);

    // flat + 4 nested: list + (2, 3, 2, 3) + single
    [StoredProcedure("ScopedTest36")]
    Task<(List<TestClass1>,
        (TestClass2, List<TestClass3>)?,
        (List<TestClass4>, TestClass5, List<TestClass6>)?,
        (TestClass7, List<TestClass8>)?,
        (List<TestClass9>, TestClass10, List<TestClass1>)?,
        TestClass2)> Test36_4Nested_FlatPrefix_2_3_2_3_Single(string connectionString, HeaderParameters parameters);

    // === 5 NESTED TUPLES ===

    // 5 nested: incremental (2, 2, 3, 3, 4) + list
    [StoredProcedure("ScopedTest37")]
    Task<((List<TestClass3>, TestClass4)?,
        (TestClass5, List<TestClass6>)?,
        (List<TestClass7>, TestClass8, List<TestClass9>)?,
        (TestClass10, List<TestClass1>, TestClass2)?,
        (List<TestClass3>, TestClass4, List<TestClass5>, TestClass6)?,
        List<TestClass7>)> Test37_5Nested_Inc_2_2_3_3_4_List(string connectionString, HeaderParameters parameters);

    // 5 nested: decremental (4, 3, 3, 2, 2) + single
    [StoredProcedure("ScopedTest38")]
    Task<((TestClass8, List<TestClass9>, TestClass10, List<TestClass1>)?,
        (List<TestClass2>, TestClass3, List<TestClass4>)?,
        (TestClass5, List<TestClass6>, TestClass7)?,
        (List<TestClass8>, TestClass9)?,
        (TestClass10, List<TestClass1>)?,
        HeaderResult)> Test38_5Nested_Dec_4_3_3_2_2_Single(string connectionString, HeaderParameters parameters);

    // 5 nested: pyramid (2, 3, 4, 3, 2) + list
    [StoredProcedure("ScopedTest39")]
    Task<((List<TestClass2>, TestClass3)?,
        (TestClass4, List<TestClass5>, TestClass6)?,
        (List<TestClass7>, TestClass8, List<TestClass9>, TestClass10)?,
        (TestClass1, List<TestClass2>, TestClass3)?,
        (List<TestClass4>, TestClass5)?,
        List<AuditInfoResult>)> Test39_5Nested_Pyramid_2_3_4_3_2_List(string connectionString, HeaderParameters parameters);

    // 5 nested: inverted pyramid (4, 3, 2, 3, 4) + single
    [StoredProcedure("ScopedTest40")]
    Task<((List<TestClass6>, TestClass7, TestClass8, TestClass9)?,
        (TestClass10, List<TestClass1>, TestClass2)?,
        (List<TestClass3>, TestClass4)?,
        (TestClass5, List<TestClass6>, TestClass7)?,
        (List<TestClass8>, TestClass9, List<TestClass10>, TestClass1)?,
        Record4Result)> Test40_5Nested_InvPyramid_4_3_2_3_4_Single(string connectionString, HeaderParameters parameters);

    // 5 nested: all same (2, 2, 2, 2, 2) + list
    [StoredProcedure("ScopedTest41")]
    Task<((List<TestClass2>, TestClass3)?,
        (TestClass4, List<TestClass5>)?,
        (List<TestClass6>, TestClass7)?,
        (TestClass8, List<TestClass9>)?,
        (List<TestClass10>, TestClass1)?,
        List<HeaderResult>)> Test41_5Nested_AllSame_2_2_2_2_2_List(string connectionString, HeaderParameters parameters);

    // === 6 NESTED TUPLES ===

    // 6 nested: paired incremental (2, 2, 3, 3, 4, 4) + single
    [StoredProcedure("ScopedTest42")]
    Task<((List<TestClass2>, TestClass3)?,
        (TestClass4, List<TestClass5>)?,
        (List<TestClass6>, TestClass7, List<TestClass8>)?,
        (TestClass9, List<TestClass10>, TestClass1)?,
        (List<TestClass2>, TestClass3, List<TestClass4>, TestClass5)?,
        (TestClass6, List<TestClass7>, TestClass8, List<TestClass9>)?,
        HeaderResult)> Test39_6Nested_PairInc_2_2_3_3_4_4_Single(string connectionString, HeaderParameters parameters);

    // 6 nested: all minimal (2, 2, 2, 2, 2, 2) + list
    [StoredProcedure("ScopedTest43")]
    Task<((TestClass10, List<TestClass1>)?,
        (List<TestClass2>, TestClass3)?,
        (TestClass4, List<TestClass5>)?,
        (List<TestClass6>, List<TestClass7>)?,
        (TestClass8, List<TestClass9>)?,
        (List<TestClass10>, TestClass1)?,
        List<TestClass2>)> Test40_6Nested_AllMinimal_2_List(string connectionString, HeaderParameters parameters);

    // 6 nested: wave (2, 4, 2, 4, 2, 4) + single
    [StoredProcedure("ScopedTest44")]
    Task<((List<TestClass3>, TestClass4)?,
        (TestClass5, List<TestClass6>, TestClass7, List<TestClass8>)?,
        (TestClass9, List<TestClass10>)?,
        (List<TestClass1>, TestClass2, List<TestClass3>, TestClass4)?,
        (List<TestClass5>, TestClass6)?,
        (TestClass7, List<TestClass8>, TestClass9, List<TestClass10>)?,
        AuditInfoResult)> Test41_6Nested_Wave_2_4_2_4_2_4_Single(string connectionString, HeaderParameters parameters);

    // === CONDITIONTYPE.OR VARIANTS ===

    // OR single
    [StoredProcedure("ScopedTest45", ConditionType = ConditionType.OR)]
    Task<TestClass1> Test42_OR_Single(string connectionString, HeaderParameters parameters);

    // OR list
    [StoredProcedure("ScopedTest46", ConditionType = ConditionType.OR)]
    Task<List<HeaderResult>> Test43_OR_List(string connectionString, HeaderParameters parameters);

    // OR flat tuple
    [StoredProcedure("ScopedTest47", ConditionType = ConditionType.OR)]
    Task<(TestClass1, List<Record4Result>)> Test44_OR_FlatTuple(string connectionString, HeaderParameters parameters);

    // OR no params
    [StoredProcedure("ScopedTest48", ConditionType = ConditionType.OR)]
    Task<AuditInfoResult> Test45_OR_NoParams(string connectionString);

    // === INHERITED CLASS ===

    // Inherited single
    [StoredProcedure("ScopedTest49")]
    Task<BaseClass0> Test46_Inherited_Single(string connectionString, HeaderParameters parameters);

    // Inherited list
    [StoredProcedure("ScopedTest50")]
    Task<List<BaseClass0>> Test47_Inherited_List(string connectionString, HeaderParameters parameters);
}

[SpHandler(Lifetime.Singleton)]
public interface ISingletonSpExecutor
{
    // === BASICS ===

    // Single
    [StoredProcedure("SingletonTest1")]
    Task<TestClass1> Test1_Single(string connectionString, HeaderParameters parameters);

    // Single no params
    [StoredProcedure("SingletonTest2")]
    Task<HeaderResult> Test2_Single_NoParams(string connectionString);

    // List
    [StoredProcedure("SingletonTest3")]
    Task<List<Record4Result>> Test3_List(string connectionString, HeaderParameters parameters);

    // List no params
    [StoredProcedure("SingletonTest4")]
    Task<List<TestClass1>> Test4_List_NoParams(string connectionString);

    // === FLAT TUPLES (varying sizes) ===

    // Flat 2 singles
    [StoredProcedure("SingletonTest5")]
    Task<(TestClass2, AuditInfoResult)> Test5_Flat_2Singles(string connectionString, HeaderParameters parameters);

    // Flat 2 lists
    [StoredProcedure("SingletonTest6")]
    Task<(List<HeaderResult>, List<AuditInfoResult>)> Test6_Flat_2Lists(string connectionString, HeaderParameters parameters);

    // Flat 3 (list, single, list)
    [StoredProcedure("SingletonTest7")]
    Task<(List<TestClass1>, Record4Result, List<AuditInfoResult>)> Test7_Flat_3Mixed(string connectionString, HeaderParameters parameters);

    // Flat 4 (single, list, single, list)
    [StoredProcedure("SingletonTest8")]
    Task<(TestClass3, List<TestClass4>, TestClass5, List<TestClass6>)> Test8_Flat_4Alternating(string connectionString, HeaderParameters parameters);

    // Flat 5 all lists
    [StoredProcedure("SingletonTest9")]
    Task<(List<TestClass1>, List<TestClass3>, List<TestClass5>, List<TestClass7>, List<TestClass9>)> Test9_Flat_5AllLists(string connectionString, HeaderParameters parameters);

    // Flat 5 all singles
    [StoredProcedure("SingletonTest10")]
    Task<(TestClass1, TestClass3, TestClass5, TestClass7, TestClass9)> Test10_Flat_5AllSingles(string connectionString, HeaderParameters parameters);

    // Flat 6 zigzag (list, single, list, single, list, single)
    [StoredProcedure("SingletonTest11")]
    Task<(List<HeaderResult>, TestClass1, List<Record4Result>, TestClass2, List<AuditInfoResult>, TestClass3)> Test11_Flat_6Zigzag(string connectionString, HeaderParameters parameters);

    // === 1 NESTED TUPLE (at different positions with flat elements) ===

    // 1 nested(2 elem) at start + single + list
    [StoredProcedure("SingletonTest12")]
    Task<((List<TestClass4>, TestClass5)?, TestClass6, List<TestClass7>)> Test12_1Nested_2Elem_StartPlusFlat(string connectionString, HeaderParameters parameters);

    // single + 1 nested(3 elem) at middle + list
    [StoredProcedure("SingletonTest13")]
    Task<(TestClass8, (TestClass9, List<TestClass10>, TestClass1)?, List<TestClass2>)> Test13_1Nested_3Elem_AtMiddle(string connectionString, HeaderParameters parameters);

    // single + list + 1 nested(4 elem) at end
    [StoredProcedure("SingletonTest14")]
    Task<(TestClass3, List<TestClass4>, (List<TestClass5>, TestClass6, List<TestClass7>, TestClass8)?)> Test14_1Nested_4Elem_AtEnd(string connectionString, HeaderParameters parameters);

    // list + 1 nested(2 elem) at end
    [StoredProcedure("SingletonTest15")]
    Task<(List<HeaderResult>, (TestClass9, List<TestClass10>)?)> Test15_1Nested_2Elem_AtEnd(string connectionString, HeaderParameters parameters);

    // 1 nested(4 elem) at start + single
    [StoredProcedure("SingletonTest16")]
    Task<((TestClass1, List<TestClass2>, TestClass3, List<TestClass4>)?, TestClass5)> Test16_1Nested_4Elem_AtStart(string connectionString, HeaderParameters parameters);

    // === 2 NESTED TUPLES (varied positions & sizes) ===

    // 2 nested: (2, 3) + single
    [StoredProcedure("SingletonTest17")]
    Task<((List<TestClass6>, TestClass7)?,
        (TestClass8, List<TestClass9>, TestClass10)?,
        TestClass1)> Test17_2Nested_2_3_Single(string connectionString, HeaderParameters parameters);

    // 2 nested: (3, 2) + list
    [StoredProcedure("SingletonTest18")]
    Task<((TestClass2, List<TestClass3>, TestClass4)?,
        (List<TestClass5>, TestClass6)?,
        List<TestClass7>)> Test18_2Nested_3_2_List(string connectionString, HeaderParameters parameters);

    // single + 2 nested: (2, 4) after flat + list
    [StoredProcedure("SingletonTest19")]
    Task<(TestClass8,
        (List<TestClass9>, TestClass10)?,
        (TestClass1, List<TestClass2>, TestClass3, List<TestClass4>)?,
        List<TestClass5>)> Test19_2Nested_AfterFlat_2_4_List(string connectionString, HeaderParameters parameters);

    // list + 2 nested: (4, 4) both max + single
    [StoredProcedure("SingletonTest20")]
    Task<(List<TestClass6>,
        (List<TestClass7>, TestClass8, List<TestClass9>, TestClass10)?,
        (TestClass1, List<TestClass2>, TestClass3, List<TestClass4>)?,
        TestClass5)> Test20_2Nested_BothMax_4_4_Single(string connectionString, HeaderParameters parameters);

    // 2 nested: (4, 3) + single
    [StoredProcedure("SingletonTest21")]
    Task<((List<TestClass6>, TestClass7, List<TestClass8>, TestClass9)?,
        (TestClass10, List<TestClass1>, TestClass2)?,
        HeaderResult)> Test21_2Nested_4_3_Single(string connectionString, HeaderParameters parameters);

    // === 3 NESTED TUPLES ===

    // 3 nested: incremental (2, 3, 4) + list
    [StoredProcedure("SingletonTest22")]
    Task<((List<TestClass3>, TestClass4)?,
        (TestClass5, List<TestClass6>, TestClass7)?,
        (List<TestClass8>, TestClass9, List<TestClass10>, TestClass1)?,
        List<TestClass2>)> Test22_3Nested_Inc_2_3_4_List(string connectionString, HeaderParameters parameters);

    // 3 nested: decremental (4, 3, 2) + single
    [StoredProcedure("SingletonTest23")]
    Task<((TestClass3, List<TestClass4>, TestClass5, List<TestClass6>)?,
        (List<TestClass7>, TestClass8, List<TestClass9>)?,
        (TestClass10, List<TestClass1>)?,
        HeaderResult)> Test23_3Nested_Dec_4_3_2_Single(string connectionString, HeaderParameters parameters);

    // 3 nested: spike (2, 4, 2) + list
    [StoredProcedure("SingletonTest24")]
    Task<((List<TestClass2>, TestClass3)?,
        (TestClass4, List<TestClass5>, TestClass6, List<TestClass7>)?,
        (TestClass8, List<TestClass9>)?,
        List<TestClass10>)> Test24_3Nested_Spike_2_4_2_List(string connectionString, HeaderParameters parameters);

    // 3 nested: valley (4, 2, 4) + single
    [StoredProcedure("SingletonTest25")]
    Task<((List<TestClass1>, TestClass2, List<TestClass3>, TestClass4)?,
        (TestClass5, List<TestClass6>)?,
        (List<TestClass7>, TestClass8, List<TestClass9>, TestClass10)?,
        Record4Result)> Test25_3Nested_Valley_4_2_4_Single(string connectionString, HeaderParameters parameters);

    // flat + 3 nested: single + (3, 2, 3) + list
    [StoredProcedure("SingletonTest26")]
    Task<(TestClass1,
        (List<TestClass2>, TestClass3, List<TestClass4>)?,
        (TestClass5, List<TestClass6>)?,
        (TestClass7, List<TestClass8>, List<TestClass9>)?,
        List<TestClass10>)> Test26_3Nested_FlatPlus_3_2_3_List(string connectionString, HeaderParameters parameters);

    // 3 nested: all max (4, 4, 4) + single
    [StoredProcedure("SingletonTest27")]
    Task<((List<TestClass1>, TestClass2, List<TestClass3>, TestClass4)?,
        (TestClass5, List<TestClass6>, TestClass7, List<TestClass8>)?,
        (List<TestClass9>, TestClass10, List<TestClass1>, TestClass2)?,
        AuditInfoResult)> Test27_3Nested_AllMax_4_4_4_Single(string connectionString, HeaderParameters parameters);

    // === 4 NESTED TUPLES ===

    // 4 nested: incremental (2, 2, 3, 4) + single
    [StoredProcedure("SingletonTest28")]
    Task<((List<TestClass3>, TestClass4)?,
        (TestClass5, List<TestClass6>)?,
        (List<TestClass7>, TestClass8, List<TestClass9>)?,
        (TestClass10, List<TestClass1>, TestClass2, List<TestClass3>)?,
        HeaderResult)> Test28_4Nested_Inc_2_2_3_4_Single(string connectionString, HeaderParameters parameters);

    // 4 nested: decremental (4, 3, 2, 2) + list
    [StoredProcedure("SingletonTest29")]
    Task<((List<TestClass4>, TestClass5, List<TestClass6>, TestClass7)?,
        (TestClass8, List<TestClass9>, TestClass10)?,
        (List<TestClass1>, TestClass2)?,
        (TestClass3, List<TestClass4>)?,
        List<Record4Result>)> Test29_4Nested_Dec_4_3_2_2_List(string connectionString, HeaderParameters parameters);

    // 4 nested: alternating (2, 4, 2, 4) + single
    [StoredProcedure("SingletonTest30")]
    Task<((TestClass5, List<TestClass6>)?,
        (List<TestClass7>, TestClass8, List<TestClass9>, TestClass10)?,
        (TestClass1, List<TestClass2>)?,
        (List<TestClass3>, TestClass4, List<TestClass5>, TestClass6)?,
        AuditInfoResult)> Test30_4Nested_Alt_2_4_2_4_Single(string connectionString, HeaderParameters parameters);

    // 4 nested: bookend (4, 2, 2, 4) + list
    [StoredProcedure("SingletonTest31")]
    Task<((List<TestClass7>, TestClass8, List<TestClass9>, TestClass10)?,
        (TestClass1, List<TestClass2>)?,
        (List<TestClass3>, TestClass4)?,
        (TestClass5, List<TestClass6>, TestClass7, List<TestClass8>)?,
        List<HeaderResult>)> Test31_4Nested_Bookend_4_2_2_4_List(string connectionString, HeaderParameters parameters);

    // 4 nested: all same (3, 3, 3, 3) + single
    [StoredProcedure("SingletonTest32")]
    Task<((TestClass9, TestClass10, TestClass1)?,
        (List<TestClass2>, TestClass3, List<TestClass4>)?,
        (TestClass5, List<TestClass6>, TestClass7)?,
        (List<TestClass8>, TestClass9, List<TestClass10>)?,
        Record4Result)> Test32_4Nested_AllSame_3_3_3_3_Single(string connectionString, HeaderParameters parameters);

    // flat + 4 nested: list + (2, 3, 2, 3) + single
    [StoredProcedure("SingletonTest33")]
    Task<(List<TestClass1>,
        (TestClass2, List<TestClass3>)?,
        (List<TestClass4>, TestClass5, List<TestClass6>)?,
        (TestClass7, List<TestClass8>)?,
        (List<TestClass9>, TestClass10, List<TestClass1>)?,
        TestClass2)> Test33_4Nested_FlatPrefix_2_3_2_3_Single(string connectionString, HeaderParameters parameters);

    // === 5 NESTED TUPLES ===

    // 5 nested: incremental (2, 2, 3, 3, 4) + list
    [StoredProcedure("SingletonTest34")]
    Task<((List<TestClass3>, TestClass4)?,
        (TestClass5, List<TestClass6>)?,
        (List<TestClass7>, TestClass8, List<TestClass9>)?,
        (TestClass10, List<TestClass1>, TestClass2)?,
        (List<TestClass3>, TestClass4, List<TestClass5>, TestClass6)?,
        List<TestClass7>)> Test34_5Nested_Inc_2_2_3_3_4_List(string connectionString, HeaderParameters parameters);

    // 5 nested: decremental (4, 3, 3, 2, 2) + single
    [StoredProcedure("SingletonTest35")]
    Task<((TestClass8, List<TestClass9>, TestClass10, List<TestClass1>)?,
        (List<TestClass2>, TestClass3, List<TestClass4>)?,
        (TestClass5, List<TestClass6>, TestClass7)?,
        (List<TestClass8>, TestClass9)?,
        (TestClass10, List<TestClass1>)?,
        HeaderResult)> Test35_5Nested_Dec_4_3_3_2_2_Single(string connectionString, HeaderParameters parameters);

    // 5 nested: pyramid (2, 3, 4, 3, 2) + list
    [StoredProcedure("SingletonTest36")]
    Task<((List<TestClass2>, TestClass3)?,
        (TestClass4, List<TestClass5>, TestClass6)?,
        (List<TestClass7>, TestClass8, List<TestClass9>, TestClass10)?,
        (TestClass1, List<TestClass2>, TestClass3)?,
        (List<TestClass4>, TestClass5)?,
        List<AuditInfoResult>)> Test36_5Nested_Pyramid_2_3_4_3_2_List(string connectionString, HeaderParameters parameters);

    // 5 nested: inverted pyramid (4, 3, 2, 3, 4) + single
    [StoredProcedure("SingletonTest37")]
    Task<((List<TestClass6>, TestClass7, TestClass8, TestClass9)?,
        (TestClass10, List<TestClass1>, TestClass2)?,
        (List<TestClass3>, TestClass4)?,
        (TestClass5, List<TestClass6>, TestClass7)?,
        (List<TestClass8>, TestClass9, List<TestClass10>, TestClass1)?,
        Record4Result)> Test37_5Nested_InvPyramid_4_3_2_3_4_Single(string connectionString, HeaderParameters parameters);

    // 5 nested: all same (2, 2, 2, 2, 2) + list
    [StoredProcedure("SingletonTest38")]
    Task<((List<TestClass2>, TestClass3)?,
        (TestClass4, List<TestClass5>)?,
        (List<TestClass6>, TestClass7)?,
        (TestClass8, List<TestClass9>)?,
        (List<TestClass10>, TestClass1)?,
        List<HeaderResult>)> Test38_5Nested_AllSame_2_2_2_2_2_List(string connectionString, HeaderParameters parameters);

    // === 6 NESTED TUPLES ===

    // 6 nested: paired incremental (2, 2, 3, 3, 4, 4) + single
    [StoredProcedure("SingletonTest39")]
    Task<((List<TestClass2>, TestClass3)?,
        (TestClass4, List<TestClass5>)?,
        (List<TestClass6>, TestClass7, List<TestClass8>)?,
        (TestClass9, List<TestClass10>, TestClass1)?,
        (List<TestClass2>, TestClass3, List<TestClass4>, TestClass5)?,
        (TestClass6, List<TestClass7>, TestClass8, List<TestClass9>)?,
        HeaderResult)> Test39_6Nested_PairInc_2_2_3_3_4_4_Single(string connectionString, HeaderParameters parameters);

    // 6 nested: all minimal (2, 2, 2, 2, 2, 2) + list
    [StoredProcedure("SingletonTest40")]
    Task<((TestClass10, List<TestClass1>)?,
        (List<TestClass2>, TestClass3)?,
        (TestClass4, List<TestClass5>)?,
        (List<TestClass6>, List<TestClass7>)?,
        (TestClass8, List<TestClass9>)?,
        (List<TestClass10>, TestClass1)?,
        List<TestClass2>)> Test40_6Nested_AllMinimal_2_List(string connectionString, HeaderParameters parameters);

    // 6 nested: wave (2, 4, 2, 4, 2, 4) + single
    [StoredProcedure("SingletonTest41")]
    Task<((List<TestClass3>, TestClass4)?,
        (TestClass5, List<TestClass6>, TestClass7, List<TestClass8>)?,
        (TestClass9, List<TestClass10>)?,
        (List<TestClass1>, TestClass2, List<TestClass3>, TestClass4)?,
        (List<TestClass5>, TestClass6)?,
        (TestClass7, List<TestClass8>, TestClass9, List<TestClass10>)?,
        AuditInfoResult)> Test41_6Nested_Wave_2_4_2_4_2_4_Single(string connectionString, HeaderParameters parameters);

    // === CONDITIONTYPE.OR VARIANTS ===

    // OR single
    [StoredProcedure("SingletonTest42", ConditionType = ConditionType.OR)]
    Task<TestClass1> Test42_OR_Single(string connectionString, HeaderParameters parameters);

    // OR list
    [StoredProcedure("SingletonTest43", ConditionType = ConditionType.OR)]
    Task<List<HeaderResult>> Test43_OR_List(string connectionString, HeaderParameters parameters);

    // OR flat tuple
    [StoredProcedure("SingletonTest44", ConditionType = ConditionType.OR)]
    Task<(TestClass1, List<Record4Result>)> Test44_OR_FlatTuple(string connectionString, HeaderParameters parameters);

    // OR no params
    [StoredProcedure("SingletonTest45", ConditionType = ConditionType.OR)]
    Task<AuditInfoResult> Test45_OR_NoParams(string connectionString);

    // === INHERITED CLASS ===

    // Inherited single
    [StoredProcedure("SingletonTest46")]
    Task<BaseClass0> Test46_Inherited_Single(string connectionString, HeaderParameters parameters);

    // Inherited list
    [StoredProcedure("SingletonTest47")]
    Task<List<BaseClass0>> Test47_Inherited_List(string connectionString, HeaderParameters parameters);
}
#endregion

#region DTOs
public class TestClass122//: TestClass
{
    [ParamConfig(Unique =true)]
    public  string Name { get; set; }
    public  int Count { get; set; }
    public  DateTime OccuredOn { get; set;}
}
public class TestClass : HeaderResult
{
    [ParamConfig(DBParam = "CustomeName")]
    public override string Name { get; set; }
    public override string MyName { get; set; }
}
public class TestClass22: HeaderInfo
{
    public  int Count { get; set; }
    public  DateTime OccuredOn { get; set; }
    public  bool IsActive { get; set; }
    [ParamConfig(Unique = true)]
    public  long LargeNumber { get; set; }

    //[ParamConfig(DBParam ="MyNew")]
    //
    [ParamConfig(DBParam = "MyNameDBParam")]
     public  List<BaseClass1> NameList { get; set; }
     [ParamConfig(ResultExclusion =true)]
    public long LargeNumber1 { get; set; }
    public decimal Ratio { get; set; }
    public short Name1 { get; set; }
    public short? Name2 { get; set; }
    [ParamConfig(OutParam = true)]
    public short Name3 { get; set; }
    public short SmallNumber { get; set; }
    public byte TinyNumber { get; set; }
    public float LessPreciseFloat { get; set; }
    public double DoublePrecision { get; set; }
    public char SingleChar { get; set; }
    public sbyte SByteValue { get; set; }
    public ushort UShortValue { get; set; }
    public uint UIntValue { get; set; }
    public ulong ULongValue { get; set; }
    public nint NIntValue { get; set; }
    public nuint NUIntValue { get; set; }
    [ParamConfig(DBParam = "OccurredOnDBParam", ResultExclusion = true)]
    public DateTime OccurredOn { get; set; }
    [ParamConfig(ParamExclusion = true, DBParam = "OccurredOnExcludeDBParam")]
    public DateTime OccurredOnExclude { get; set; }
    [ParamConfig(DBParam = "OccurredAtDBParam")]
    public DateTimeOffset OccurredAt { get; set; }
    [ParamConfig(DBParam = "DurationDBParam", OutParam = true)]
    public TimeSpan Duration { get; set; }
    [ParamConfig(DBParam = "OccurredOnDBParam", ResultExclusion = true)]
    public Guid UniqueId { get; set; }
    public int? Count1 { get; set; }
    public bool? IsActive1 { get; set; }
    [ParamConfig(OutParam = true, ParamExclusion = true, DBParam = "Ratio1DBParam")]
    //public long? LargeNumber1 { get; set; }
    public decimal? Ratio1 { get; set; }
    public short? SmallNumber1 { get; set; }
    public byte? TinyNumber1 { get; set; }
    public float? LessPreciseFloat1 { get; set; }
    public double? DoublePrecision1 { get; set; }
    public char? SingleChar1 { get; set; }
    public sbyte? NullableSByteValue { get; set; }
    public ushort? NullableUShortValue { get; set; }
    public uint? NullableUIntValue { get; set; }
    public ulong? NullableULongValue { get; set; }
    public nint? NullableNIntValue { get; set; }
    public nuint? NullableNUIntValue { get; set; }
    public DateTime? OccurredOn1 { get; set; }
    public DateTimeOffset? OccurredAt1 { get; set; }
    public TimeSpan? Duration1 { get; set; }
    public Guid? UniqueId1 { get; set; }
     public List<HeaderInfo>? Status1 { get; set; }
    public List<HeaderInfo> Status { get; set; }
    public StatusType StatusType { get; set; }
    public StatusType? StatusType1 { get; set; }
    public byte[] BinaryData { get; set; }
    public char[] CharArray { get; set; }
    public byte[]? BinaryData1 { get; set; }
    public char[]? CharArray1 { get; set; }
}
public class HeaderInfo
{


    public string MyName { get; set; }
    public decimal Ratio { get; set; }
    public byte[] BinaryData { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public TimeSpan Duration { get; set; }
   
    
}
public enum StatusType
{
    Unknown = 0,
    Active = 1,
    Inactive = 2
}
//You can pass TVPs as parameters to stored procedures as given below with list.
// '.dbo' will be added automatically to the type name.
// If [TVP] is specified, the names given will be passed to stored procedure as table-valued type.
public class HeaderParameters : HeaderInfo
{
 
    public List<Record4TableType> Record4Items { get; set; } = new();
    public List<AuditInfoTableType> AuditItems { get; set; } = new();
}
[TVP("dbo.Record4TableType")]
public class Record4TableType 
{

    public string Name { get; set; }
    public int Count { get; set; }
    public bool IsActive { get; set; }
    public long LargeNumber { get; set; }
}


public class AuditInfoTableType
{
    public string Description { get; set; }
    public int Count { get; set; }
    public bool IsActive { get; set; }
    public long LargeNumber { get; set; }
    public double Ratio { get; set; }
    public byte[] BinaryData { get; set; }
    public DateTime OccurredOn { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public TimeSpan Duration { get; set; }
}

//Order of properties and datatype matter. they must match order of table coming from SP.
public class HeaderResult
{
    
    public int HeaderId { get; set; }
    [ParamConfig(DBParam = "CustomeName",Unique =true)]
    public virtual string Name { get; set; }
    [ParamConfig(ResultExclusion =true)]
    public virtual string MyName { get; set; }
    public int Count { get; set; }
    [ParamConfig(OutParam = true)]
    public string IsActive { get; set; }
    public long LargeNumber { get; set; }
    public decimal Ratio { get; set; }
    public byte[] BinaryData { get; set; }
    public DateTime OccurredOn { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public TimeSpan Duration { get; set; }
    [ParamConfig(DBParam = "HeaderId", OutParam = true)]
    public int HeaderIdOut { get; set; }
    [ParamConfig(OutParam = true)]
    public int CountOut { get; set; }
    [ParamConfig(OutParam = true)]
    public bool IsActiveOut { get; set; }
    [ParamConfig(OutParam = true)]
    public long LargeNumberOut { get; set; }
    [ParamConfig(OutParam = true)]
    public decimal RatioOut { get; set; }
    [ParamConfig(OutParam = true)]
    public byte[] BinaryDataOut { get; set; }
    [ParamConfig(OutParam = true)]
    public DateTime OccurredOnOut { get; set; }
    [ParamConfig(OutParam = true)]
    public DateTimeOffset OccurredAtOut { get; set; }
    [ParamConfig(OutParam = true)]
    public TimeSpan DurationOut { get; set; }
}
public class Record4Result 
{
    public int Record4Id { get; set; }
    public int HeaderId { get; set; }
    [ParamConfig(Unique =true)]
    public string Name1 { get; set; }
    public int Count { get; set; }
    public bool IsActive { get; set; }
    public long LargeNumber { get; set; }
}
public class AuditInfoResult 
{
    public int AuditInfoId { get; set; }
    public int HeaderId { get; set; }
    public string Description { get; set; }
    [ParamConfig(Unique =true)]
    public int Count { get; set; }
    public bool IsActive { get; set; }
    public long LargeNumber { get; set; }
    public decimal Ratio { get; set; }
    public byte[] BinaryData { get; set; }
    public DateTime OccurredOn { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public TimeSpan Duration { get; set; }
}
public class BaseClass4
{

    public string Name { get; set; }
    public int Count { get; set; }
    public DateTime OccuredOn { get; set; }
    [ParamConfig(DBParam ="IsActiveDBParam",ParamExclusion =true)]
    public bool IsActive { get; set; }
    public long LargeNumber { get; set; }
}
public class BaseClass3: BaseClass4
    {

    public string Name { get; set; }
    public int Count { get; set; }
    public DateTime OccuredOn { get; set; }
    public bool IsActive { get; set; }
    public long LargeNumber { get; set; }
}
public class BaseClass2 : BaseClass3
{

    public string Name { get; set; }
    public int Count { get; set; }
    public DateTime OccuredOn { get; set; }
    public bool IsActive { get; set; }
    public long LargeNumber { get; set; }
}
//[TVP("dbo.BaseClass1TableTypeCustom",IncludeInherited=true)]
public class BaseClass1 
{
    
    public virtual string Name { get; set; }
    [ParamConfig(ParamExclusion =true)]
    public virtual int Count { get; set; }

    public virtual DateTime OccuredOn { get; set; }
    public virtual bool IsActive { get; set; }
   
    public virtual long LargeNumber { get; set; }
    public byte[] Binary { get; set; }
}
public class BaseClass0 :BaseClass1
{

    public int Count { get; set; }
    [ParamConfig(DBParam = "MyNew")]
    public DateTime OccuredOn { get; set; }
    public bool IsActive { get; set; }
    [ParamConfig(Unique = true)]
    public long LargeNumber { get; set; }
    [ParamConfig(ResultExclusion =true)]
    public long LargeNumber1 { get; set; }
    public decimal Ratio { get; set; }
    public short SmallNumber { get; set; }
    public byte TinyNumber { get; set; }
    public float LessPreciseFloat { get; set; }
    public double DoublePrecision { get; set; }
    public char SingleChar { get; set; }
    public sbyte SByteValue { get; set; }
    public ushort UShortValue { get; set; }
    public uint UIntValue { get; set; }
    public ulong ULongValue { get; set; }
    public nint NIntValue { get; set; }
    public nuint NUIntValue { get; set; }
    public DateTime OccurredOn { get; set; }
    [ParamConfig( DBParam = "OccurredOnExcludeDBParam")]
    public DateTime OccurredOnExclude { get; set; }
    [ParamConfig(DBParam = "OccurredAtDBParam")]
    public DateTimeOffset OccurredAt { get; set; }
    [ParamConfig(DBParam = "DurationDBParam")]
    public TimeSpan Duration { get; set; }
    public Guid UniqueId { get; set; }
    public int? Count1 { get; set; }
    public bool? IsActive1 { get; set; }
    [ParamConfig( DBParam = "Ratio1DBParam")]
    public decimal? Ratio1 { get; set; }
    public short? SmallNumber1 { get; set; }
    public byte? TinyNumber1 { get; set; }
    public float? LessPreciseFloat1 { get; set; }
    public double? DoublePrecision1 { get; set; }
    public char? SingleChar1 { get; set; }
    public sbyte? NullableSByteValue { get; set; }
    public ushort? NullableUShortValue { get; set; }
    public uint? NullableUIntValue { get; set; }
    public ulong? NullableULongValue { get; set; }
    public nint? NullableNIntValue { get; set; }
    public nuint? NullableNUIntValue { get; set; }
    public DateTime? OccurredOn1 { get; set; }
    public DateTimeOffset? OccurredAt1 { get; set; }
    public TimeSpan? Duration1 { get; set; }
    public Guid? UniqueId1 { get; set; }
    public StatusType StatusType { get; set; }
    public StatusType? StatusType1 { get; set; }
    public byte[] BinaryData { get; set; }
    public char[] CharArray { get; set; }
    public byte[]? BinaryData1 { get; set; }
    public char[]? CharArray1 { get; set; }

}


public class TestClass1
{
    public string Name { get; set; }
    public int Count { get; set; }
    [ParamConfig(Unique =true)]
    public DateTime OccuredOn { get; set; }
    public bool IsActive { get; set; }
}

public class TestClass2
{
    public int Count { get; set; }
    public DateTime OccuredOn { get; set; }
    public bool IsActive { get; set; }
    [ParamConfig(Unique = true)]
    public long LargeNumber { get; set; }
}

public class TestClass3
{
    [ParamConfig(Unique = true)]
    public string Title { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public bool IsEnabled { get; set; }
}

public class TestClass4
{
    [ParamConfig(Unique = true)]
    public string Code { get; set; }
    public double Weight { get; set; }
    public DateTime CreatedAt { get; set; }
    public long Serial { get; set; }
}

public class TestClass5
{
    [ParamConfig(Unique = true)]
    public string Label { get; set; }
    public int Score { get; set; }
    public bool IsVerified { get; set; }
    public short Level { get; set; }
}

public class TestClass6
{
    [ParamConfig(Unique = true)]
    public string MyLargeNumber { get; set; }
    public decimal Budget { get; set; }
    public DateTime StartDate { get; set; }
    public int Population { get; set; }
}

public class TestClass7
{
    [ParamConfig(Unique = true)]
    public string SKU { get; set; }
    public float Rating { get; set; }
    public long ViewCount { get; set; }
    public bool InStock { get; set; }
}

public class TestClass8
{
    [ParamConfig(Unique = true,DBParam = "CodeDBParam")]
    public string Code { get; set; }
    public int Rank { get; set; }
    public decimal Balance { get; set; }
    public DateTime UpdatedOn { get; set; }
}

public class TestClass9
{
    [ParamConfig(Unique = true)]
    public string Category { get; set; }
    public bool IsPublished { get; set; }
    public double Latitude { get; set; }
    public int Revision { get; set; }
}

public class TestClass10
{
    [ParamConfig(Unique = true)]
    public string Tag { get; set; }
    public long Frequency { get; set; }
    public short Priority { get; set; }
    public DateTime ExpiresOn { get; set; }
}

public class TestClass11
{
    public string Vendor { get; set; }
    public int Units { get; set; }
    public decimal Discount { get; set; }
    public bool IsApproved { get; set; }
}

public class TestClass12
{
    [ParamConfig(Unique = false)]
    public string Channel { get; set; }
    public double Bandwidth { get; set; }
    public DateTime RegisteredOn { get; set; }
    public long Throughput { get; set; }
}

public class TestClass13
{
    [ParamConfig(Unique = false)]
    public string Tenant { get; set; }
    public int Capacity { get; set; }
    public bool IsLocked { get; set; }
    public float Threshold { get; set; }
}

public class TestClass14
{
    [ParamConfig(Unique = false)]
    public string Origin { get; set; }
    public decimal Mileage { get; set; }
    public short Altitude { get; set; }
    public DateTime DepartedOn { get; set; }
}

public class TestClass15
{
    [ParamConfig(Unique = false)]
    public string Batch { get; set; }
    public long Sequence { get; set; }
    public int Iteration { get; set; }
    public bool IsCompleted { get; set; }
}

public class TestClass16
{
    [ParamConfig(Unique = false)]
    public string Segment { get; set; }
    public double Coverage { get; set; }
    public DateTime ScheduledOn { get; set; }
    public int Attempts { get; set; }
}

#endregion