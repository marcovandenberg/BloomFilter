using Xunit;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeKata
{
    public class FilterTests
    {
        public static IEnumerable<object[]> TestData =>
            new List<object[]>
            {
            new object[] { new HashAlgorithm[] { MD5.Create() } },
            new object[] { new HashAlgorithm[] { MD5.Create(), SHA1.Create() } },
            new object[] { new HashAlgorithm[] { MD5.Create(), SHA1.Create(), SHA256.Create() } },
            new object[] { new HashAlgorithm[] { MD5.Create(), SHA1.Create(), SHA256.Create(), SHA512.Create() } },
            };

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestBeforeAfter(HashAlgorithm[] algorithms)
        {
            string[] dictionaryWords = new string[] { "Spiral", "Messier 63" };
            int bloomArraySize = 64 * 64;
            BloomFilter myFilter = new BloomFilter(algorithms, bloomArraySize, dictionaryWords);

            Assert.True(myFilter.TestFilterForWord("Spiral"));
            Assert.False(myFilter.TestFilterForWord("Andromeda"));
            myFilter.AddWordToFilter("Andromeda");
            Assert.True(myFilter.TestFilterForWord("Andromeda"));
            Assert.True(myFilter.TestFilterForWord("Spiral"));
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestSmallPositive(HashAlgorithm[] algorithms)
        {
            string[] dictionaryWords = new string[] { "Andromeda" };
            int bloomArraySize = 64 * 64;
            BloomFilter myFilter = new BloomFilter(algorithms, bloomArraySize, dictionaryWords);

            Assert.True(myFilter.TestFilterForWord("Andromeda"));
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestSmallNegative(HashAlgorithm[] algorithms)
        {
            string[] dictionaryWords = new string[] { "Missing" };
            int bloomArraySize = 16 * 16;
            BloomFilter myFilter = new BloomFilter(algorithms, bloomArraySize, dictionaryWords);

            Assert.False(myFilter.TestFilterForWord("Nonsense"));
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestManyNegatives(HashAlgorithm[] algorithms)
        {
            string[] dictionaryWords = new string[] { "ABC", "DEF", "GHI", "123", "AMA", "LUX" };
            int bloomArraySize = 512 * 512;
            BloomFilter myFilter = new BloomFilter(algorithms, bloomArraySize, dictionaryWords);

            Assert.False(myFilter.TestFilterForWord("Word"));
            Assert.False(myFilter.TestFilterForWord("Alpha"));
            Assert.False(myFilter.TestFilterForWord("Alpaca"));
            Assert.False(myFilter.TestFilterForWord("Cat"));
            Assert.False(myFilter.TestFilterForWord("Dog"));
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestManyPositives(HashAlgorithm[] algorithms)
        {
            string[] dictionaryWords = new string[] { "Word", "Alpha", "Alpaca", "Cat", "Dog" };
            int bloomArraySize = 512 * 512;
            BloomFilter myFilter = new BloomFilter(algorithms, bloomArraySize, dictionaryWords);

            Assert.True(myFilter.TestFilterForWord("Word"));
            Assert.True(myFilter.TestFilterForWord("Alpha"));
            Assert.True(myFilter.TestFilterForWord("Alpaca"));
            Assert.True(myFilter.TestFilterForWord("Cat"));
            Assert.True(myFilter.TestFilterForWord("Dog"));
        }

        // [Theory]
        // [MemberData(nameof(TestData))]
        // public void TestFullDictionary(HashAlgorithm[] algorithms)
        // {
        //     string fileName = System.IO.Path.Combine(
        //     System.IO.Path.GetDirectoryName(
        //     System.Reflection.Assembly.GetExecutingAssembly().Location), "TestData\\wordlist.txt");

        //     string[] dictionaryWords = File.ReadAllLines(fileName);
        //     int bloomArraySize = 4096 * 4096;
        //     BloomFilter myFilter = new BloomFilter(algorithms, bloomArraySize, dictionaryWords);

        //     Assert.True(myFilter.TestFilterForWord("treasure"));
        //     Assert.True(myFilter.TestFilterForWord("yellow"));
        //     Assert.True(myFilter.TestFilterForWord("absolute"));
        //     Assert.True(myFilter.TestFilterForWord("sweet"));
        //     Assert.True(myFilter.TestFilterForWord("storm"));
        // }
    }
}
