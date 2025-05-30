﻿using System;
using System.Text;

namespace Dota2Simulator
{
    internal class RandomGenerator
    {
        // Instantiate random number generator.  
        // It is better to keep a single Random instance 
        // and keep using Next on the same instance.  
        private readonly Random _random = new();

        // Generates a random number within a range.      
        public int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }

        // Generates a random string with a given size.    
        public string RandomString(int size, bool lowerCase = false)
        {
            StringBuilder builder = new(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):   
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.  

            // char is a single Unicode character  
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length = 26  

            for (int i = 0; i < size; i++)
            {
                char @char = (char)_random.Next(offset, offset + lettersOffset);
                _ = builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower(System.Globalization.CultureInfo.CurrentCulture) : builder.ToString();
        }

        // Generates a random password.  
        // 4-LowerCase + 4-Digits + 2-UpperCase  
        public string RandomPassword()
        {
            StringBuilder passwordBuilder = new();

            // 4-Letters lower case   
            _ = passwordBuilder.Append(RandomString(4, true));

            // 4-Digits between 1000 and 9999  
            _ = passwordBuilder.Append(RandomNumber(1000, 9999));

            // 2-Letters upper case  
            _ = passwordBuilder.Append(RandomString(2));
            return passwordBuilder.ToString();
        }
    }
}