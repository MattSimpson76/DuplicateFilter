using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzySharp;

namespace DuplicateFilter
{
    public class Filter
    {
        // integer, valid values 0-100, which specifies threshold determining when strings qualify as duplicate
        // 0 is most permissive, 100 least
        public int DuplicateThreshold { get; set; }

        public Filter(int duplicateThreshold)
        {
            DuplicateThreshold = duplicateThreshold;
        }

        // takes a Dictionary<string, int> and a string and increments the value accessed by the key
        public void incrementVariantCount(Dictionary<string, int> nameVariations, string variant)
        {
            if (nameVariations.ContainsKey(variant))
            {
                nameVariations[variant]++;
            }
            else
            {
                nameVariations.Add(variant, 1);
            }
        }

        // finds the highest value in a given dictionary and returns the corresponding string key
        public string getMostCommonVariant(Dictionary<string, int> nameVariations)
        {
            int maxCount = 0;
            string mostCommonVariant = "";

            foreach (KeyValuePair<string, int> variant in nameVariations)
            {
                //Console.WriteLine($"{variant.Key} : {variant.Value}");
                if (variant.Value > maxCount)
                {
                    mostCommonVariant = variant.Key;
                    maxCount = variant.Value;
                }
            }

            return mostCommonVariant;
        }

        /// <summary> This methods identifies and removes duplicates from a list of strings. </summary>
        /// <param name="inputList"> The list to remove duplicates from.</param>
        /// <param name="optionalListOfDuplicates"> Optional parameter- if passed, will be populated with lists of each identified 'group' of duplicate names.</param>
        /// <returns>A new list of strings with duplicate names replaced by a single instance of its most common variant (ties broken arbitrarily).</returns>
        public List<string> RemoveAllDuplicates(List<string> inputList, List<List<string>> optionalListOfDuplicates = null)
        {
            // output list
            List<string> cleanedList = new List<string>();
            // stores list entries which have already been found to be duplicates so as to avoid redundant processing
            HashSet<string> eliminatedDuplicates = new HashSet<string>(inputList.Count);

            for (int index = 0; index < inputList.Count; index++)
            {
                // if we've already found a match for this entry, continue
                if (eliminatedDuplicates.Contains(inputList[index]))
                {
                    continue;
                }

                // stores list entries which we've identifed as matching the current element in the outer loop
                HashSet<string> matchingDuplicates = new HashSet<string>();
                matchingDuplicates.Add(inputList[index]);

                // stores a count of the number of instances of each unique name variation we've matched
                Dictionary<string, int> nameVariations = new Dictionary<string, int>();
                nameVariations[inputList[index]] = 1;

                for (int innerIndex = index + 1; innerIndex < inputList.Count; innerIndex++)
                {
                    // if we've already found a match for this entry, continue
                    if (eliminatedDuplicates.Contains(inputList[innerIndex]))
                    {                        
                        continue;
                    }

                    // if this element is identical to one we've already matched this loop, increment the count of that variant and continue
                    if (matchingDuplicates.Contains(inputList[innerIndex]))
                    {
                        incrementVariantCount(nameVariations, inputList[innerIndex]);
                        continue;
                    }

                    // if we haven't matched this element before, calculate a similarity score now
                    int similarityScore = Fuzz.WeightedRatio(inputList[index], inputList[innerIndex], FuzzySharp.PreProcess.PreprocessMode.None);
                    if (similarityScore >= DuplicateThreshold) {
                        matchingDuplicates.Add(inputList[innerIndex]);
                        incrementVariantCount(nameVariations, inputList[innerIndex]);                            
                    }

                }

                // find the most common variant among all matched duplicates of this name- this is chosen as the entry that will be preserved in the list
                string mostCommonVariant = getMostCommonVariant(nameVariations);

                if (!string.IsNullOrEmpty(mostCommonVariant)) {
                    cleanedList.Add(mostCommonVariant);
                    // update the set of all matched entries with the names matched on this loop iteration
                    eliminatedDuplicates.UnionWith(matchingDuplicates);
                    // if outputting a grouped list of all duplicates, update that now
                    if (optionalListOfDuplicates != null)
                    {
                        optionalListOfDuplicates.Add(matchingDuplicates.ToList());
                    }
                }                               
            }                

            return cleanedList;
        }

    }
}
