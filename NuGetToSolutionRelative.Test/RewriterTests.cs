/*
   Copyright 2018 David Solimano

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License. 
*/
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGetToSolutionRelative.Test
{
    [TestFixture]
    public class RewriterTests
    {
        string desired = "$(SolutionDir)packages\\log4net.2.0.8\\lib\\net45-full\\log4net.dll";
        [Test]
        public void NotRewritten()
        {
            string initial = "..\\packages\\log4net.2.0.8\\lib\\net45-full\\log4net.dll";
            string actual = RefTransformer.MakeRefRelative(initial);
            Assert.AreEqual(desired, actual);
        }

        [Test]
        public void AlreadyRewritten()
        {
            string initial = "$(SolutionDir)packages\\log4net.2.0.8\\lib\\net45-full\\log4net.dll";

            string actual = RefTransformer.MakeRefRelative(initial);
            Assert.AreEqual(desired, actual);
        }
    }
}
