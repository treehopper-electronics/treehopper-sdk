using System;
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
                var library = JsonConvert.DeserializeObject<Library>(File.ReadAllText(file));
                library.Namespace = path.Replace("Libraries\\", "").Replace("\\", ".");
                library.Name = Path.GetFileNameWithoutExtension(file);
                library.Preprocess();

                var csharpOut = $"{treehopperRoot}\\NET\\Libraries\\{path.Replace("Libraries", "Treehopper.Libraries")}\\{library.Name}Registers.cs";
                var relativePath = $"{path.Replace("Libraries\\", "")}\\{library.Name}Registers.cs";
                Render.FileToFile("Templates\\Registers.cs", library, csharpOut);
                if(project.Items.Count(i => i.UnevaluatedInclude == relativePath) == 0)
                    project.AddItem("Compile", relativePath, new[] { new KeyValuePair<string, string>("DependentUpon", $"{library.Name}.cs")});
            }
        }


    }
}
