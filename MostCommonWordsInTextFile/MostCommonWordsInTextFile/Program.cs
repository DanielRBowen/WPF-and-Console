using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace MostCommonWordsInTextFile
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();

            //all versions of .NET:
            /*byte[] buffer;

			using (FileStream inputStream = File.OpenRead("Bible.txt"))
			{
				int fileSize = (int)inputStream.Length;
				buffer = new byte[fileSize];
				inputStream.Read(buffer, 0, fileSize);
			}

			string text = Encoding.UTF8.GetString(buffer).ToLower();
			string[] words = text.Split();
			Hashtable wordCounts = new Hashtable();

			foreach (string word in words)
			{
				if (word.Length == 0)
				{
					continue;
				}

				int count;

				if (wordCounts.ContainsKey(word))
				{
					count = (int)wordCounts[word];
				}
				else
				{
					count = 0;
				}

				wordCounts[word] = count + 1;
			}

			DictionaryEntry[] wordCountsArray = new DictionaryEntry[wordCounts.Count];
			wordCounts.CopyTo(wordCountsArray, 0);

			Array.Sort(wordCountsArray, new WordCountComparer());

			for (int index = 0, count = Math.Min(wordCountsArray.Length, 10); index != count; ++index)
			{
				DictionaryEntry word = wordCountsArray[index];
				Console.Write((string)word.Key);
				Console.Write('\t');
				Console.WriteLine(((int)word.Value).ToString("#,##0", NumberFormatInfo.CurrentInfo));
			}*/

            var text = File.ReadAllText("ThePictureOfDorianGray.txt", Encoding.UTF8).ToLowerInvariant();
            var words = text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            var wordCounts =
                from word in words
                group word by word into wordSet
                select new KeyValuePair<string, int>(wordSet.Key, wordSet.Count());

            var top10 = wordCounts
                .OrderByDescending(word => word.Value)
                .Take(10);

            foreach (var word in top10)
            {
                Console.Write(word.Key);
                Console.Write('\t');
                Console.WriteLine(word.Value.ToString("#,##0", NumberFormatInfo.CurrentInfo));
            }

            Console.WriteLine(stopwatch.Elapsed.TotalSeconds.ToString(NumberFormatInfo.InvariantInfo));

            Console.ReadLine();
            return;
        }
    }
}
