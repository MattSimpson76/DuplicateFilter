using Microsoft.VisualStudio.TestTools.UnitTesting;
using DuplicateFilter;
using System.Collections.Generic;
using System;

namespace DuplicateFilterTests
{
    [TestClass]
    public class FilterTests
    {
        [TestMethod]
        public void RemoveAllDuplicatesGroupsExtremelySimilarNames()
        {
            List<string> baseTestList = new List<string>();
            baseTestList.Add("generic company name");
            baseTestList.Add("generic company namer");
            baseTestList.Add("generic company nam3");
            baseTestList.Add("generic companyname");
            baseTestList.Add("genaric company name");
            baseTestList.Add("generic company name.");

            List<string> goalList = new List<string>();
            goalList.Add("generic company name");
            goalList.Add("generic company namer");
            goalList.Add("generic company nam3");
            goalList.Add("generic companyname");
            goalList.Add("genaric company name");
            goalList.Add("generic company name.");
            List<List<string>> goalListOfLists = new List<List<string>>();
            goalListOfLists.Add(goalList);

            Filter testFilter = new Filter(90);
            List<List<string>> outputListOfLists = new List<List<string>>();
            List<string> _outputNameList = testFilter.RemoveAllDuplicates(baseTestList, outputListOfLists);
            CollectionAssert.AreEquivalent(goalListOfLists[0], outputListOfLists[0]);
        }

        [TestMethod]
        public void RemoveAllDuplicatesDoesNotGroupExtremelyDifferentNames()
        {
            List<string> baseTestList = new List<string>();
            baseTestList.Add("generic company name");
            baseTestList.Add("completely different organisation title");
            baseTestList.Add("12313ad'sakd'k313");
            baseTestList.Add("z");

            List<string> goalList = new List<string>();
            List<List<string>> goalListOfLists = new List<List<string>>();
            goalListOfLists.Add(new List<string> { "generic company name" });
            goalListOfLists.Add(new List<string> { "completely different organisation title" });
            goalListOfLists.Add(new List<string> { "12313ad'sakd'k313" });
            goalListOfLists.Add(new List<string> { "z" });

            Filter testFilter = new Filter(90);
            List<List<string>> outputListOfLists = new List<List<string>>();
            List<string> _outputNameList = testFilter.RemoveAllDuplicates(baseTestList, outputListOfLists);
            Assert.AreEqual(outputListOfLists.Count, goalListOfLists.Count);
            for (int index = 0; index < outputListOfLists.Count; index++)
            {
                CollectionAssert.AreEquivalent(goalListOfLists[index], outputListOfLists[index]);
            }

        }

        [TestMethod]
        public void RemoveAllDuplicatesReturnsReducedListCorrectly()
        {
            List<string> baseTestList = new List<string>();
            baseTestList.Add("generic company name");
            baseTestList.Add("generic company name");
            baseTestList.Add("generic company nam3");
            baseTestList.Add("generic companyname");
            baseTestList.Add("genaric company name");
            baseTestList.Add("generic company name.");

            List<string> goalList = new List<string>();
            goalList.Add("generic company name");

            Filter testFilter = new Filter(90);
            List<string> outputNameList = testFilter.RemoveAllDuplicates(baseTestList);
            CollectionAssert.AreEqual(goalList, outputNameList);
        }

        [TestMethod]
        public void RemoveAllDuplicatesFunctionsWithMultipleDistinctNames()
        {
            List<string> baseTestList = new List<string>();
            baseTestList.Add("generic company name");
            baseTestList.Add("generic company name");
            baseTestList.Add("generic company nam3");
            baseTestList.Add("generic companyname");
            baseTestList.Add("genaric company name");
            baseTestList.Add("generic company nane");
            baseTestList.Add("second org name");
            baseTestList.Add("second 0rg name");
            baseTestList.Add("sec org name");
            baseTestList.Add("second org name");
            baseTestList.Add("second orgn name");
            baseTestList.Add("second org name");

            List<string> goalList = new List<string>();
            goalList.Add("generic company name");
            goalList.Add("second org name");
            goalList.Reverse();

            Filter testFilter = new Filter(75);
            List<string> outputNameList = testFilter.RemoveAllDuplicates(baseTestList);
            CollectionAssert.AreEquivalent(goalList, outputNameList);
        }

        [TestMethod]
        public void RemoveAllDuplicatesReducesAllToOneWhenThresholdIsZero()
        {
            List<string> baseTestList = new List<string>();
            baseTestList.Add("generic company name");
            baseTestList.Add("generic company name");
            baseTestList.Add("generic company name");
            baseTestList.Add("generic company nam3");
            baseTestList.Add("generic companyname");
            baseTestList.Add("genaric company name");
            baseTestList.Add("generic company name.");
            baseTestList.Add("second org name");
            baseTestList.Add("second 0rg name");
            baseTestList.Add("sec org name");
            baseTestList.Add("second org name");
            baseTestList.Add("second organisation name");
            baseTestList.Add("second org name.");

            List<string> goalList = new List<string>();
            goalList.Add("generic company name");

            Filter testFilter = new Filter(0);
            List<string> outputNameList = testFilter.RemoveAllDuplicates(baseTestList);
            CollectionAssert.AreEquivalent(goalList, outputNameList);
        }

        [TestMethod]
        public void RemoveAllDuplicatesEliminatesOnlyExactMatchesWhenThresholdIsOneHundred()
        {
            List<string> baseTestList = new List<string>();
            baseTestList.Add("generic company name");
            baseTestList.Add("generic company name");
            baseTestList.Add("sec org name");
            baseTestList.Add("second org name");

            List<string> goalList = new List<string>();
            goalList.Add("generic company name");
            goalList.Add("sec org name");
            goalList.Add("second org name");

            Filter testFilter = new Filter(100);
            List<string> outputNameList = testFilter.RemoveAllDuplicates(baseTestList);
            CollectionAssert.AreEquivalent(goalList, outputNameList);
        }

        [TestMethod]
        public void IncrementVariantCountAddsEntryIfNotPresent()
        {
            Dictionary<string, int> testDict = new Dictionary<string, int>();
            Filter testFilter = new Filter(100);
            testFilter.incrementVariantCount(testDict, "test string");
            Assert.AreEqual(1, testDict["test string"]);
        }

        [TestMethod]
        public void IncrementVariantCountAddsExactlyOneToValue()
        {
            Dictionary<string, int> testDict = new Dictionary<string, int>();
            testDict["test string"] = 97;
            Filter testFilter = new Filter(100);
            testFilter.incrementVariantCount(testDict, "test string");
            Assert.AreEqual(98, testDict["test string"]);
        }

        [TestMethod]
        public void getMostCommonVariantReturnsCorrectly()
        {
            Dictionary<string, int> testDict = new Dictionary<string, int>();
            testDict["test string"] = 97;
            testDict["second test string"] = 2;
            Filter testFilter = new Filter(100);            
            Assert.AreEqual("test string", testFilter.getMostCommonVariant(testDict));
        }

        [TestMethod]
        public void getMostCommonVariantReturnsCorrectlyOnTie()
        {
            Dictionary<string, int> testDict = new Dictionary<string, int>();
            testDict["test string"] = 97;
            testDict["second test string"] = 97;
            Filter testFilter = new Filter(100);
            Assert.AreEqual("test string", testFilter.getMostCommonVariant(testDict));
        }

    }

}
