﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Evaluation;
using Newtonsoft.Json;
using Nustache.Core;

namespace RegisterGenerator
{
    class Program
    {
        private static ProjectCollection collection = new ProjectCollection();
        private static Project project;
        private static string treehopperRoot;

        static void Main(string[] args)
        {
            treehopperRoot = args[0];
            project = collection.LoadProject($"{treehopperRoot}\\NET\\Libraries\\Treehopper.Libraries\\Treehopper.Libraries.csproj");
            ProcessDirectory("Libraries");
            project.Save();
        }

        public static void ProcessDirectory(string path)
        {
            foreach(var dir in Directory.GetDirectories(path))
                ProcessDirectory(dir);

            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                if (Path.GetExtension(file) != ".json") continue;
                var library = JsonConvert.DeserializeObject<Library>(File.ReadAllText(file));
                library.Namespace = path.Replace("Libraries\\", "").Replace("\\", ".");
                library.Name = Path.GetFileNameWithoutExtension(file);
                library.Preprocess();

                {
                    // C Sharp
                    var outPath = $"{treehopperRoot}\\NET\\Libraries\\{path.Replace("Libraries", "Treehopper.Libraries")}\\{library.Name}.generated.cs";
                    Render.FileToFile("Templates\\Registers.cs", library, outPath);

                    // add to the csproj file
                    var relativePath = $"{path.Replace("Libraries\\", "")}\\{library.Name}.generated.cs";
                    if (project.Items.Count(i => i.UnevaluatedInclude == relativePath) == 0)
                        project.AddItem("Compile", relativePath, new[] { new KeyValuePair<string, string>("DependentUpon", $"{library.Name}.cs") });
                }
                {
                    // C++
                    var outPath = $"{treehopperRoot}\\C++\\API\\Treehopper.Libraries\\inc\\{path.Replace("Libraries\\", "")}\\{library.Name}Registers.h";
                    Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                    Render.FileToFile("Templates\\Registers.h", library, outPath);
                }
                {
                    // Java
                    var outPath = $"{treehopperRoot}\\Java\\api\\treehopper.libraries\\src\\main\\java\\io\\treehopper\\{path.ToLower()}\\{library.Name.ToLower()}";
                    Directory.CreateDirectory(outPath);
                    Render.FileToFile("Templates\\Registers.java", library, outPath+$"\\{library.Name}Registers.java");
                    foreach(var reg in library.RegisterList)
                    {
                        foreach (var value in reg.Values.Values)
                        {
                            if (value.Enum != null)
                            {
                                Render.FileToFile("Templates\\Enum.java", new { Enum = value.Enum, Package = library.NamespaceLower+"."+library.NameLower }, outPath + $"\\{value.Enum.Name}.java");
                            }
                        }
                    }
                    
                }
            }
        }
    }
}
