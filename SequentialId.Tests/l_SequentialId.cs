using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SequentialId.Tests
{
    [TestClass]
    public class l_SequentialId
    {
        [TestMethod]
        public void CollisionTest()
        {
            int count = 5000000;
            var set = new HashSet<SequentialId>();

            for (int i = 0; i < count; i++)
            {
                set.Add(SequentialId.NewId());
            }

            Assert.AreEqual(count, set.Count);
        }


        [TestMethod]
        public void GuidRace()
        {
            int count = 15000000;

            var st = new Stopwatch();
            st.Start();
            for (int i = 0; i < count; i++)
            {
                var newId = SequentialId.NewId();
                //Debug.WriteLine(newId);
            }
            st.Stop();

            Debug.WriteLine($"{count.ToString("N0")} SequentialIds created in {st.Elapsed}");


            st.Restart();
            for (int i = 0; i < count; i++)
            {
                var newId = Guid.NewGuid();
                //Debug.WriteLine(newId);
            }
            st.Stop();

            Debug.WriteLine($"{count.ToString("N0")} Guids created in {st.Elapsed}");


            st.Restart();
            for (int i = 0; i < count; i++)
            {
                var newId = UuidUtil.NewSequentialId();
                //Debug.WriteLine(newId);
            }
            st.Stop();

            Debug.WriteLine($"{count.ToString("N0")} WinSequentialIds created in {st.Elapsed}");
        }
    }

    public class UuidUtil
    {
        [DllImport("rpcrt4.dll", SetLastError = true)]
        static extern int UuidCreateSequential(out Guid guid);

        public static Guid NewSequentialId()
        {
            Guid guid;
            UuidCreateSequential(out guid);
            return guid;
        }
    }
}
