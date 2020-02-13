using System;
using System.Collections;
using System.Security.Cryptography;

namespace CodeKata
{
    /// <summary>
    /// Simple implementation of a bloom filter.
    /// </summary>
    public class BloomFilter
    {
        private HashAlgorithm[] algorithms;
        private int bloomArraySize;
        private string[] dictionaryWords;
        private BitArray bloomData;

        /// <summary>
        /// Creates a new bloom filter with desired baseline parameters.
        /// </summary>
        public BloomFilter(HashAlgorithm[] algorithms, int bloomArraySize, string[] dictionaryWords)
        {
            this.algorithms = algorithms;
            this.bloomArraySize = bloomArraySize;
            this.dictionaryWords = dictionaryWords;

            if (ValidateInitialisation())
                bloomData = LoadDictionaryIntoBloomArray();
        }

        /// <summary>
        /// The validation for the presence of key elements required for the stateful nature of the bloom filter.
        /// </summary>
        private bool ValidateInitialisation()
        {
            if (algorithms == null || algorithms.Length == 0)
                throw new Exception("You must provide at least one algorithm for the bloom filter");

            if (dictionaryWords == null || dictionaryWords.Length == 0)
                throw new Exception("You must provide a dictionary for the bloom filter");

            return true;
        }

        /// <summary>
        /// Initializes the bloom filter array of bits and iterates through the dictionary words loading them into the filter using the configured algorithms.
        /// </summary>
        private BitArray LoadDictionaryIntoBloomArray()
        {
            BitArray bloomData = new BitArray(bloomArraySize);

            for (int wordIndex = 0; wordIndex < dictionaryWords.Length; wordIndex++)
                AddWordToFilter(bloomData, dictionaryWords[wordIndex]);

            return bloomData;
        }

        /// <summary>
        /// Convenient overload for external use without exposing the raw bloom data.
        /// </summary>
        public void AddWordToFilter(string wordToAdd)
        {
            AddWordToFilter(bloomData, wordToAdd);
        }

        /// <summary>
        /// Add word to filter using the configured array of hash algorithms.
        /// </summary>
        private void AddWordToFilter(BitArray bloomData, string wordToAdd)
        {
            for (int algoIndex = 0; algoIndex < algorithms.Length; algoIndex++)
            {
                int filterPosition = CalculateBloomPosition(wordToAdd, algorithms[algoIndex]);
                bloomData[filterPosition] = true;
            }
        }

        /// <summary>
        /// Returns a boolean indicating whether the bloom filter indicates the tested word was loaded. 
        /// Calculates and checks each hash algorithm using the test word. Returns true if all relevant array bits are true. 
        /// This can be a false positive, but will never be a false negative.
        /// </summary>
        public bool TestFilterForWord(string wordToTest)
        {
            for (int algoIndex = 0; algoIndex < algorithms.Length; algoIndex++)
            {
                int bytePosition = CalculateBloomPosition(wordToTest, algorithms[algoIndex]);

                //Short circuit further testing the remaining hash algorithms
                if (bloomData[bytePosition] == false)
                    return false;
            }

            //Assume the word was loaded in the filter, but obviously can be a false positive
            return true;
        }

        /// <summary>
        /// Returns the index inside the bloom filter fore the given word and hash algorithm.
        /// </summary>
        private int CalculateBloomPosition(string wordToTest, HashAlgorithm algorithm)
        {
            var bytes = algorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(wordToTest));
            int convertedInt = Math.Abs(BitConverter.ToInt32(bytes, 0));
            int bytePosition = convertedInt % bloomArraySize;
            return bytePosition;
        }
    }
}
