using System;
using System.Collections;
using System.Security.Cryptography;

namespace CodeKata
{
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

        private bool ValidateInitialisation()
        {
            if (_algorithms == null || _algorithms.Length == 0)
                throw new Exception("You must provide at least one algorithm for the bloom filter");

            if (_dictionaryWords == null || _dictionaryWords.Length == 0)
                throw new Exception("You must provide a dictionary for the bloom filter");

            return true;
        }

        private BitArray LoadDictionaryIntoBloomArray()
        {
            BitArray bloomData = new BitArray(_bloomArraySize);

            for (int wordIndex = 0; wordIndex < _dictionaryWords.Length; wordIndex++)
                AddWordToFilter(bloomData, _dictionaryWords[wordIndex]);

            return bloomData;
        }

        public void AddWordToFilter(string wordToAdd)
        {
            AddWordToFilter(_bloomData, wordToAdd);
        }

        private void AddWordToFilter(BitArray bloomData, string wordToAdd)
        {
            for (int algoIndex = 0; algoIndex < _algorithms.Length; algoIndex++)
            {
                int filterPosition = CalculateBloomPosition(wordToAdd, _algorithms[algoIndex]);
                bloomData[filterPosition] = true;
            }
        }

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

        private int CalculateBloomPosition(string wordToTest, HashAlgorithm algorithm)
        {
            var bytes = algorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(wordToTest));
            int convertedInt = Math.Abs(BitConverter.ToInt32(bytes, 0));
            int bytePosition = convertedInt % _bloomArraySize;
            return bytePosition;
        }
    }
}
