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
        private HashAlgorithm[] _algorithms;
        private int _bloomArraySize;
        private string[] _dictionaryWords;
        private BitArray _bloomData;

        public BloomFilter(HashAlgorithm[] algorithms, int bloomArraySize, string[] dictionaryWords)
        {
            _algorithms = algorithms;
            _bloomArraySize = bloomArraySize;
            _dictionaryWords = dictionaryWords;

            if (ValidateInitialisation())
                _bloomData = LoadDictionaryIntoBloomArray();
        }

        /// <summary>
        /// The validation for the presence of key elements required for the stateful nature of the bloom filter.
        /// </summary>
        private bool ValidateInitialisation()
        {
            if (_algorithms == null || _algorithms.Length == 0)
                throw new Exception("You must provide at least one algorithm for the bloom filter");

            if (_dictionaryWords == null || _dictionaryWords.Length == 0)
                throw new Exception("You must provide a dictionary for the bloom filter");

            return true;
        }

        /// <summary>
        /// Initializes the bloom filter array of bits and iterates through the dictionary words loading them into the filter using the configured algorithms.
        /// </summary>
        private BitArray LoadDictionaryIntoBloomArray()
        {
            BitArray bloomData = new BitArray(_bloomArraySize);

            for (int wordIndex = 0; wordIndex < _dictionaryWords.Length; wordIndex++)
                AddWordToFilter(bloomData, _dictionaryWords[wordIndex]);

            return bloomData;
        }

        /// <summary>
        /// Convenient overload for external use without exposing the raw bloom data.
        /// </summary>
        public void AddWordToFilter(string wordToAdd)
        {
            AddWordToFilter(_bloomData, wordToAdd);
        }

        /// <summary>
        /// Add word to filter using the configured array of hash algorithms.
        /// </summary>
        private void AddWordToFilter(BitArray bloomData, string wordToAdd)
        {
            for (int algoIndex = 0; algoIndex < _algorithms.Length; algoIndex++)
            {
                int filterPosition = CalculateBloomPosition(wordToAdd, _algorithms[algoIndex]);
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
            for (int algoIndex = 0; algoIndex < _algorithms.Length; algoIndex++)
            {
                int bytePosition = CalculateBloomPosition(wordToTest, _algorithms[algoIndex]);

                //Short circuit further testing the remaining hash algorithms
                if (_bloomData[bytePosition] == false)
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
            int bytePosition = convertedInt % _bloomArraySize;
            return bytePosition;
        }
    }
}
