using NUnit.Framework;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Serialization.Json;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Diagnostics;

namespace Reko.UnitTests.Core.Serialization.Json
{
    [TestFixture]
    public class JsonProcedureSerializerTests
    {
        private ProgramBuilder pb;

        [SetUp]
        public void Setup()
        {
            this.pb = new ProgramBuilder();
        }

        private void RunTest(string sExp, Action<ProcedureBuilder> builder)
        {
            var m = new ProcedureBuilder("JpsTest");
            builder(m);
            RunTest(sExp, m.Procedure);
        }

        private void RunTest(string sExp, Procedure proc)
        { 
            var jps = new JsonProcedureSerializer();
            var sActual = jps.Serialize(proc);
            sExp = sExp.Replace('\'', '"');
            if (sExp != sActual)
            {
                Debug.Print(sActual.Replace('"', '\''));
                Assert.AreEqual(sExp, sActual);
            }
        }

        [Test]
        public void Jps_Simple()
        {
            var sExp =
            "{" +
                "'name':'JpsTest'," +
                "'signature':''," +
                "'ids':[" +
                    "{'name':'r1','type':'w32','stg':{'kind':'reg','name':'r1'}}" + // 0
                "]," +
                "'blocks':[" +
                    "{'name':'JpsTest_entry'," +
                     "'succ':['l0010']}," +
                    "{'name':'JpsTest_exit','exit':true}," +
                    "{'name':'l0010'," +
                     "'addr':'00123400'," +
                     "'stms':[" +
                         "[0,'=','r1',[1,'w32']]," +
                         "[1,'ret']]," +
                     "'succ':['JpsTest_exit']}" +
                "]" +
            "}";


            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);
                m.Label("l0010");
                m.Assign(r1, 1);
                m.Return();
            });

        }

        [Test]
        public void Jps_Branch()
        {
            var sExp =
            "{" +
                "'name':'JpsTest'," +
                "'signature':''," +
                "'ids':[" +
                    "{'name':'f1','type':'w64','stg':{'kind':'reg','name':'f1'}}," + 
                    "{'name':'f3','type':'w64','stg':{'kind':'reg','name':'f3'}}," +
                    "{'name':'Mem0','stg':{'kind':'mem'}}," +
                    "{'name':'r1','type':'w32','stg':{'kind':'reg','name':'r1'}}," + 
                    "{'name':'r9','type':'w32','stg':{'kind':'reg','name':'r9'}}," + 
                    "{'name':'SZ','type':'w32','stg':{'kind':'flg','grf':3,'reg':'flags'}}" + 
                "]," +
                "'blocks':[" +
                    "{'name':'JpsTest_entry'," +
                     "'succ':['l0010']}," +
                    "{'name':'JpsTest_exit','exit':true}," +
                    "{'name':'l0010','addr':'00123400','stms':[" +
                          "[0,'=','SZ','cof',['-f','f1','f3']]," +
                          "[1,'bra',['test','GE','SZ'],'l_nonneg']]," +
                     "'succ':['l_neg','l_nonneg']}," +
                    "{'name':'l_nonneg','addr':'00123404','stms':[" +
                          "[0,['st',['m','Mem0',['+','r9',[8,'w32']],'r64'],['*f','f3',[2,'r64']]]]," +
                          "[1,'ret']]," +
                     "'succ':['JpsTest_exit']}," +
                    "{'name':'l_neg','addr':'00123402','stms':[" +
                          "[0,'=','f3',[0,'r64']]," +
                          "[1,'=','r1',['neg','r1']]]," +
                  "'succ':['l_nonneg']}]}";
            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 9);
                var r9 = m.Reg32("r9", 9);
                var f1 = m.Reg64("f1", 2);
                var f3 = m.Reg64("f3", 3);
                var SZ = m.Flags("SZ");
                m.Label("l0010");
                m.Assign(SZ, m.Cond(m.FSub(f1, f3)));
                m.BranchIf(m.Test(ConditionCode.GE, SZ), "l_nonneg");

                m.Label("l_neg");
                m.Assign(f3, Constant.Real64(0.0));
                m.Assign(r1, m.Neg(r1));

                m.Label("l_nonneg");
                m.MStore(m.IAdd(r9, 8), m.FMul(f3,Constant.Real64(2.5)));
                m.Return();
            });
        }

        [Test]
        public void Jps_CallsApplications()
        {
            var sExpMain =
            "{" +
                "'name':'main','signature':''," +
                "'ids':[{'name':'f1','type':'w64','stg':{'kind':'reg','name':'f1'}}]," +
                "'blocks':[" +
                    "{'name':'main_entry'," +
                     "'succ':['l0010']}," +
                    "{'name':'main_exit','exit':true}," +
                    "{'name':'l0010','addr':'00001000','stms':[" +
                        "[0,'call','sub',0,0,['r1','r2']['f1']],[1,'ret','f1']]," +
                     "'succ':['main_exit']}]}";

            var sExpSub =
            "{" +
                "'name':'sub','signature':'','ids':["+ 
                    "{'name':'f1','type':'w64','stg':{'kind':'reg','name':'f1'}},"+
                    "{'name':'Mem0','stg':{'kind':'mem'}},"+
                    "{'name':'r1','type':'w32','stg':{'kind':'reg','name':'r1'}}," + 
                    "{'name':'r2','type':'w32','stg':{'kind':'reg','name':'r2'}}],"+
                    "'blocks':["+ 
                        "{'name':'sub_entry','succ':['l1000']},"+ 
                        "{'name':'sub_exit','exit':true},"+ 
                        "{'name':'l1000','addr':'00002000','stms':["+
                            "[0,'=','f1',['cos',['m','Mem0','r1','r64']],"+
                            "[1,'=','f1',['sin',['m','Mem0','r2','r64']],"+
                            "[2,'=','f1',['-f','f1','f1']],[3,'ret','f1']],"+
                          "'succ':['sub_exit']}]}";

            var sin = new ExternalProcedure("sin", new FunctionType(
                [ new Identifier("arg", PrimitiveType.Real64, null)],
                [ new Identifier("", PrimitiveType.Real64, null) ]
                 ));
            var cos = new ExternalProcedure("cos", new FunctionType(
                [new Identifier("arg", PrimitiveType.Real64, null)],
                [new Identifier("", PrimitiveType.Real64, null)]));

            pb.Add("main", m =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                var f1 = m.Reg64("f1", 2);
                var f3 = m.Reg64("f3", 3);
                var SZ = m.Flags("SZ");
                m.Label("l0010");
                m.Call("sub", 0, new[] { r1, r2 }, new[] { f1 });
                m.Return(f1);
            });
            pb.Add("sub", m =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                var f1 = m.Reg64("f1", 2);
                var f2 = m.Reg64("f1", 2);
                m.Label("l1000");
                m.Assign(f1, m.Fn(cos, m.Mem(PrimitiveType.Real64, r1)));
                m.Assign(f2, m.Fn(sin, m.Mem(PrimitiveType.Real64, r2)));
                m.Assign(f1, m.FSub(f1, f2));
                m.Return(f1);
            });
            pb.BuildProgram();
            RunTest(sExpMain, pb.Program.Procedures.Values[0]); 
            RunTest(sExpSub, pb.Program.Procedures.Values[1]);
        }
    }
}
