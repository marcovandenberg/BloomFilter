using System;
using System.IO;
using System.Security.Cryptography;

namespace CodeKata
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] dictionaryWords = File.ReadAllLines("wordlist.txt");
            int bloomArraySize = 4096 * 4096;
            var algorithmsToUse = new HashAlgorithm[] { MD5.Create() };
            BloomFilter myFilter = new BloomFilter(algorithmsToUse, bloomArraySize, dictionaryWords);

            Console.WriteLine($"Dictionary Words: {dictionaryWords.Length}");
            Console.WriteLine($"Bloom Filter Array Size: {bloomArraySize}");

            string wordTest = "Xdusr@dx";
            Console.WriteLine($"Test For {wordTest}: {myFilter.TestFilterForWord(wordTest)}");

            wordTest = "Microsoft";
            Console.WriteLine($"Test For {wordTest}: {myFilter.TestFilterForWord(wordTest)}");

            wordTest = "azure";
            Console.WriteLine($"Test For {wordTest}: {myFilter.TestFilterForWord(wordTest)}");

            wordTest = "stack";
            Console.WriteLine($"Test For {wordTest}: {myFilter.TestFilterForWord(wordTest)}");

            wordTest = "edge";
            Console.WriteLine($"Test For {wordTest}: {myFilter.TestFilterForWord(wordTest)}");

            return;
        }
    }
}
