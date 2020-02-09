using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace CodeKata
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] dictionaryWords = File.ReadAllLines("wordlist.txt");

            int bloomArraySize = 4096 * 4096;
            var algorithmsToUse = new HashAlgorithm[] { MD5.Create(), SHA256.Create() };
            BloomFilter myFilter = new BloomFilter(algorithmsToUse, bloomArraySize, dictionaryWords);

            Console.WriteLine($"Dictionary Words: {dictionaryWords.Length}");
            Console.WriteLine($"Bloom Filter Array Size: {bloomArraySize}");

            BasicTests(myFilter);
            FalsePositiveScan(dictionaryWords, myFilter);

            return;
        }

        private static void BasicTests(BloomFilter myFilter)
        {
            Console.WriteLine("\nBasic Filter Check: ");
            string[] wordTests = new string[] { "Xdusr@dx", "Microsoft", "azure", "stack", "edge" };
            foreach (string wordToTest in wordTests)
                Console.WriteLine($"Test For {wordToTest}: {myFilter.TestFilterForWord(wordToTest)}");
        }

        private static void FalsePositiveScan(string[] dictionaryWords, BloomFilter myFilter)
        {
            int totalRandomWords = 1000;
            int wordLength = 5;
            string[] generatedRandomWords = GenerateRandomWords(totalRandomWords, wordLength);

            Console.WriteLine($"\nFalse Positive Scan: {totalRandomWords} random words.");
            int falseCount = 0;
            for (int wordIndex = 0; wordIndex < totalRandomWords; wordIndex++)
            {
                string randomWord = generatedRandomWords[wordIndex];
                if (myFilter.TestFilterForWord(randomWord))
                {
                    bool inDictionary = Array.BinarySearch(dictionaryWords, 0, dictionaryWords.Length - 1, randomWord) >= 0;
                    if (!inDictionary)
                    {
                        falseCount++;
                        Console.Write($"{randomWord} ");
                    }
                }
            }

            float falsePct = ((float)falseCount / totalRandomWords) * 100;
            Console.WriteLine($"\nFalse Positive: {falseCount} --> {falsePct} %");
        }

        private static string[] GenerateRandomWords(int wordCount, int wordLength)
        {
            string[] randomWords = new string[wordCount];
            Random random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyz";

            for (int i = 0; i < wordCount; i++)
            {
                randomWords[i] = new string(Enumerable.Repeat(chars, wordLength)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            }

            return randomWords;
        }
    }
}
