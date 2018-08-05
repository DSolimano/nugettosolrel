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
using Microsoft.Build.Construction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NuGetToSolutionRelative
{
    class Program
    {
        static int Main(string[] args)
        {
            if(args.Length == 0 || args.Length > 2)
            {
                ShowHelp();
                return 1;
            }
            else if(args.Length == 1 && new[] { "-?","-h","-help"}.Contains(args[0]))
            {
                ShowHelp();
                return 1;
            }

            System.IO.FileInfo fi = new System.IO.FileInfo(args[0]);

            if(!fi.Exists)
            {
                throw new System.IO.FileNotFoundException("Unable to find file given as first argument", args[0]);
            }

            bool write = true;

            if(args.Length == 2 && args[1] == "-test")
            {
                write = false;
            }


            SolutionFile sf = SolutionFile.Parse(fi.FullName);
            foreach(var proj in sf.ProjectsInOrder)
            {
                Console.WriteLine("Starting " + proj.ProjectName);
                XmlDocument xd = new XmlDocument();
                xd.Load(proj.AbsolutePath);

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(
                    xd.NameTable);
                nsmgr.AddNamespace("ms", "http://schemas.microsoft.com/developer/msbuild/2003");

                var nodes = xd.SelectNodes("/ms:Project/ms:ItemGroup/ms:Reference/ms:HintPath", nsmgr);
                foreach(XmlNode node in nodes)
                {
                    string projectPath = node.InnerText;
                    string relativePath = RefTransformer.MakeRefRelative(projectPath);
                    Console.WriteLine($"{proj.ProjectName}: {projectPath} -> {relativePath}");
                    node.InnerText = relativePath;
                }

                if (write)
                {
                    xd.Save(proj.AbsolutePath);
                    Console.WriteLine("Written");
                }
                else
                {
                    Console.WriteLine("Not written");
                }

            }

            return 0;
        }

        private static void ShowHelp()
        {
            Console.Error.WriteLine("Usage: NuGetToSolutionRelative.exe whatnot.sln [-test]");
            Console.Error.WriteLine("-test flag optional if you want to see what will be changed");
        }
    }
}
