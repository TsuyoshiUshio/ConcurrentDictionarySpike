using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace ConcurrentDictionarySpike
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().execConcurrent().GetAwaiter().GetResult();
        }

        private async Task execConcurrent()
        {
            // ConcurrentDictionary
            // https://docs.microsoft.com/ja-jp/dotnet/standard/collections/thread-safe/how-to-add-and-remove-items

            var dictionary = new ConcurrentDictionary<string, string>();
            ConcurrentBag<Task> taskList = new ConcurrentBag<Task>();

            Parallel.For(0, 20, i =>
            {
                taskList.Add(Task.Run(() =>
                {
                    dictionary.TryAdd($"key{i.ToString("00")}", $"value{i.ToString("00")}");
                }));
            });

            await Task.WhenAll(taskList.ToArray());
            var sortedDictionary = dictionary.OrderBy((x) => x.Key);
            // Writing a file 
            // https://docs.microsoft.com/ja-jp/dotnet/csharp/programming-guide/file-system/how-to-write-to-a-text-file
            //  Console.WriteLine(JsonConvert.SerializeObject(sortedDictionary, Formatting.Indented)); // Not as expected.
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("some.json"))
            {
                file.WriteLine("{");
                var last = sortedDictionary.Last();
                foreach(var line in sortedDictionary)
                {
                    var delimiter = ",";
                    if (line.Key == last.Key)
                    {
                        delimiter = "";
                    }
                   
                    file.WriteLine($"\"{line.Key}\":\"{line.Value}\"{delimiter}");
                }
                file.WriteLine("}");
            }
            Console.ReadLine();
        }

    }
}
