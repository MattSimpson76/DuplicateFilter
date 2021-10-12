using Microsoft.VisualStudio.TestTools.UnitTesting;
using DuplicateFilter;
using System.Collections.Generic;

namespace DuplicateFilterTests
{
    [TestClass]
    public class InputSanitiserTests
    {
        [TestMethod]
        public void RemoveAccentsLeavesPlaintextUntouched()
        {
            string baseTestString = "This is unaccented text";
            string outputString = InputSanitiser.RemoveAccents(baseTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void RemoveAccentsFunctionsOnAccents()
        {
            string accentedString = "Thîs is téxt that hôrribly misusès âccented characters ñüï";
            string baseTestString = "This is text that horribly misuses accented characters nui";
            string outputString = InputSanitiser.RemoveAccents(accentedString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void RemoveAccentsPreservesPunctuation()
        {
            string baseTestString = "This, 'text! has? some. punctuation!";
            string outputString = InputSanitiser.RemoveAccents(baseTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void RemoveExcessWhitespacePreservesSingleSpacing()
        {
            string baseTestString = "The spaces in this should be preserved";
            string outputString = InputSanitiser.RemoveExcessWhitespace(baseTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void RemoveExcessWhitespaceFunctions()
        {
            string excessSpaceString = "    The excess  spaces in this         should  not be  preserved   ";
            string baseTestString = "The excess spaces in this should not be preserved";
            string outputString = InputSanitiser.RemoveExcessWhitespace(excessSpaceString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseNumbersPreservesRegularText()
        {
            string baseTestString = "This text should not be changed";
            string outputString = InputSanitiser.StandardiseNumbers(baseTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseNumbersFunctionsOnSingleNumber()
        {
            string baseTestString = "343";
            string outputString = InputSanitiser.StandardiseNumbers(baseTestString);
            string goalString = "three hundred and forty-three";
            Assert.AreEqual(goalString, outputString);
        }

        [TestMethod]
        public void StandardiseNumbersFunctionsOnMixedNumbersAndWords()
        {
            string baseTestString = "fake company 20";
            string outputString = InputSanitiser.StandardiseNumbers(baseTestString);
            string goalString = "fake company twenty";
            Assert.AreEqual(goalString, outputString);
        }

        [TestMethod]
        public void StandardiseNumbersFunctionsOnMultipleNumbers()
        {
            string baseTestString = "333 fake 17 company 20";
            string outputString = InputSanitiser.StandardiseNumbers(baseTestString);
            string goalString = "three hundred and thirty-three fake seventeen company twenty";
            Assert.AreEqual(goalString, outputString);
        }

        [TestMethod]
        public void StandardiseNumbersFunctionsOnNegativeNumbers()
        {
            string baseTestString = "-5 fake company";
            string outputString = InputSanitiser.StandardiseNumbers(baseTestString);
            string goalString = "-five fake company";
            Assert.AreEqual(goalString, outputString);
        }

        [TestMethod]
        public void StandardiseCorpPreservesRegularText()
        {
            string baseTestString = "This text should not be changed";
            string outputString = InputSanitiser.StandardiseCorp(baseTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseCorpPreservesRegularTextIfStripping()
        {
            string baseTestString = "This text should not be changed";
            string outputString = InputSanitiser.StandardiseCorp(baseTestString, true);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseCorpFunctions()
        {
            string corpTestString = "This text should be changed as it ends in corp.";
            string baseTestString = "This text should be changed as it ends in corporation ";
            string outputString = InputSanitiser.StandardiseCorp(corpTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseCorpFunctionsIfStripping()
        {
            string corpTestString = "This text should be changed as it ends in corp.";
            string baseTestString = "This text should be changed as it ends in ";
            string outputString = InputSanitiser.StandardiseCorp(corpTestString, true);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseCorpFunctionsCaseInsensitive()
        {
            string corpTestString = "This text should be changed as it ends in Corp. plus CORP. and corp. and cOrP.";
            string baseTestString = "This text should be changed as it ends in corporation plus corporation and corporation and corporation ";
            string outputString = InputSanitiser.StandardiseCorp(corpTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseCorpFunctionsWithCorpPartwayThroughString()
        {
            string corpTestString = "This text Corp. should be changed";
            string baseTestString = "This text corporation should be changed";
            string outputString = InputSanitiser.StandardiseCorp(corpTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseCorpFunctionsWithCorpPartwayThroughStringIfStripping()
        {
            string corpTestString = "This text Corp. should be changed";
            string baseTestString = "This text should be changed";
            string outputString = InputSanitiser.StandardiseCorp(corpTestString, true);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseCorpFunctionsWithoutPeriod()
        {
            string corpTestString = "This text should be changed as it ends in corp";
            string baseTestString = "This text should be changed as it ends in corporation ";
            string outputString = InputSanitiser.StandardiseCorp(corpTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseCorpPreservesInternalCorp()
        {
            string baseTestString = "Text like Corpus or Corporeal should not be changed";
            string outputString = InputSanitiser.StandardiseCorp(baseTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseCorpPreservesInternalCorpIfStripping()
        {
            string baseTestString = "Text like Corpus or Corporeal should not be changed";
            string outputString = InputSanitiser.StandardiseCorp(baseTestString, true);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseCorpCorrectlyStripsLongForm()
        {
            string compTestString = "This corporation text should be changed as it ends in corporation";
            string baseTestString = "This text should be changed as it ends in ";
            string outputString = InputSanitiser.StandardiseCorp(compTestString, true);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseCorpCorrectlyPreservesSingleWordLongForm()
        {
            string compTestString = "corporation";
            string baseTestString = "corporation";
            string outputString = InputSanitiser.StandardiseCorp(compTestString, true);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseLimitedPreservesRegularText()
        {
            string baseTestString = "This text should not be changed";
            string outputString = InputSanitiser.StandardiseLimited(baseTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseLimitedPreservesRegularTextIfStripping()
        {
            string baseTestString = "This text should not be changed";
            string outputString = InputSanitiser.StandardiseLimited(baseTestString, true);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseLimitedFunctions()
        {
            string corpTestString = "This text should be changed as it ends in ltd";
            string baseTestString = "This text should be changed as it ends in limited ";
            string outputString = InputSanitiser.StandardiseLimited(corpTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseLimitedFunctionsIfStripping()
        {
            string corpTestString = "This text should be changed as it ends in ltd";
            string baseTestString = "This text should be changed as it ends in ";
            string outputString = InputSanitiser.StandardiseLimited(corpTestString, true);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseLimitedFunctionsCaseInsensitive()
        {
            string ltdTestString = "This text should be changed as it ends in ltd plus LTD";
            string baseTestString = "This text should be changed as it ends in limited plus limited ";
            string outputString = InputSanitiser.StandardiseLimited(ltdTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseLimitedFunctionsWithLtdPartwayThroughString()
        {
            string ltdTestString = "This text LTD should be changed";
            string baseTestString = "This text limited should be changed";
            string outputString = InputSanitiser.StandardiseLimited(ltdTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseLimitedFunctionsWithLtdPartwayThroughStringIfStripping()
        {
            string ltdTestString = "This text LTD should be changed";
            string baseTestString = "This text should be changed";
            string outputString = InputSanitiser.StandardiseLimited(ltdTestString, true);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseLimitedFunctionsWithPeriod()
        {
            string ltdTestString = "This text should be changed as it ends in ltd.";
            string baseTestString = "This text should be changed as it ends in limited ";
            string outputString = InputSanitiser.StandardiseLimited(ltdTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseLimitedPreservesInternalLtd()
        {
            string baseTestString = "Text like ltdword or woltdrd should not be changed";
            string outputString = InputSanitiser.StandardiseLimited(baseTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseLimitedCorrectlyStripsLongForm()
        {
            string compTestString = "This limited text should be changed as it ends in limited";
            string baseTestString = "This text should be changed as it ends in ";
            string outputString = InputSanitiser.StandardiseLimited(compTestString, true);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseLimitedCorrectlyPreservesSingleWordLongForm()
        {
            string compTestString = "Limited";
            string baseTestString = "Limited";
            string outputString = InputSanitiser.StandardiseLimited(compTestString, true);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseCompanyPreservesRegularText()
        {
            string baseTestString = "This text should not be changed";
            string outputString = InputSanitiser.StandardiseCompany(baseTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseCompanyPreservesRegularTextIfStripping()
        {
            string baseTestString = "This text should not be changed";
            string outputString = InputSanitiser.StandardiseCompany(baseTestString, true);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseCompanyFunctions()
        {
            string compTestString = "This text should be changed as it ends in co";
            string baseTestString = "This text should be changed as it ends in company ";
            string outputString = InputSanitiser.StandardiseCompany(compTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseCompanyFunctionsIfStripping()
        {
            string compTestString = "This text should be changed as it ends in co";
            string baseTestString = "This text should be changed as it ends in ";
            string outputString = InputSanitiser.StandardiseCompany(compTestString, true);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseCompanyCorrectlyStripsLongForm()
        {
            string compTestString = "This company text should be changed as it ends in company";
            string baseTestString = "This text should be changed as it ends in ";
            string outputString = InputSanitiser.StandardiseCompany(compTestString, true);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseCompanyCorrectlyPreservesSingleWordLongForm()
        {
            string compTestString = "Company";
            string baseTestString = "Company";
            string outputString = InputSanitiser.StandardiseCompany(compTestString, true);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseCompanyFunctionsCaseInsensitive()
        {
            string compTestString = "This text should be changed as it ends in co plus Co";
            string baseTestString = "This text should be changed as it ends in company plus company ";
            string outputString = InputSanitiser.StandardiseCompany(compTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseCompanyFunctionsWithCompPartwayThroughString()
        {
            string compTestString = "This text co should be changed";
            string baseTestString = "This text company should be changed";
            string outputString = InputSanitiser.StandardiseCompany(compTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseCompanyFunctionsWithCompPartwayThroughStringIfStripping()
        {
            string compTestString = "This text co should be changed";
            string baseTestString = "This text should be changed";
            string outputString = InputSanitiser.StandardiseCompany(compTestString, true);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseCompanyFunctionsWithPeriod()
        {
            string compTestString = "This text should be changed as it ends in co.";
            string baseTestString = "This text should be changed as it ends in company ";
            string outputString = InputSanitiser.StandardiseCompany(compTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseCompanyPreservesInternalComp()
        {
            string baseTestString = "Text like compass or costco should not be changed";
            string outputString = InputSanitiser.StandardiseCompany(baseTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseIncPreservesRegularText()
        {
            string baseTestString = "This text should not be changed";
            string outputString = InputSanitiser.StandardiseInc(baseTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseIncFunctions()
        {
            string incTestString = "This text should be changed as it ends in inc";
            string baseTestString = "This text should be changed as it ends in incorporated ";
            string outputString = InputSanitiser.StandardiseInc(incTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseIncFunctionsCaseInsensitive()
        {
            string incTestString = "This text should be changed as it ends in inc plus Inc";
            string baseTestString = "This text should be changed as it ends in incorporated plus incorporated ";
            string outputString = InputSanitiser.StandardiseInc(incTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseIncFunctionsWithIncPartwayThroughString()
        {
            string incTestString = "This text inc should be changed";
            string baseTestString = "This text incorporated should be changed";
            string outputString = InputSanitiser.StandardiseInc(incTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseIncFunctionsWithPeriod()
        {
            string incTestString = "This text should be changed as it ends in inc.";
            string baseTestString = "This text should be changed as it ends in incorporated ";
            string outputString = InputSanitiser.StandardiseInc(incTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseIncPreservesInternalComp()
        {
            string baseTestString = "Text like zinc or tincture should not be changed";
            string outputString = InputSanitiser.StandardiseInc(baseTestString);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseIncCorrectlyStripsLongForm()
        {
            string compTestString = "This incorporated text should be changed as it ends in incorporated";
            string baseTestString = "This text should be changed as it ends in ";
            string outputString = InputSanitiser.StandardiseInc(compTestString, true);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseIncCorrectlyPreservesSingleWordLongForm()
        {
            string compTestString = "incorporated";
            string baseTestString = "incorporated";
            string outputString = InputSanitiser.StandardiseInc(compTestString, true);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseIncPreservesRegularTextIfStripping()
        {
            string baseTestString = "This text should not be changed";
            string outputString = InputSanitiser.StandardiseInc(baseTestString, true);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardisIncFunctionsWithIncPartwayThroughStringIfStripping()
        {
            string compTestString = "This text inc should be changed";
            string baseTestString = "This text should be changed";
            string outputString = InputSanitiser.StandardiseInc(compTestString, true);
            Assert.AreEqual(baseTestString, outputString);
        }

        [TestMethod]
        public void StandardiseAmpersandFunctions()
        {
            string baseTestString = "This text & this other text should be twisted & changed";
            string goalString = "This text and this other text should be twisted and changed";
            string outputString = InputSanitiser.StandardiseAmpersand(baseTestString);
            Assert.AreEqual(goalString, outputString);
        }

        [TestMethod]
        public void StandardiseAmpersandAddsSpaces()
        {
            string baseTestString = "Mary&Friends should have spaces added";
            string goalString = "Mary and Friends should have spaces added";
            string outputString = InputSanitiser.StandardiseAmpersand(baseTestString);
            Assert.AreEqual(goalString, outputString);
        }

        [TestMethod]
        public void StandardiseInputFunctions()
        {
            List<string> baseTestStringList = new List<string>();
            baseTestStringList.Add("this text should not be changed as it's already in standard form");
            baseTestStringList.Add("     This text      should be changed as it has inc LTD co.");
            List<string> goalTestStringList = new List<string>();
            goalTestStringList.Add("this text should not be changed as it's already in standard form");
            goalTestStringList.Add("this text should be changed as it has incorporated limited company");
            InputSanitiser.StandardiseInput(baseTestStringList);
            CollectionAssert.AreEqual(goalTestStringList, baseTestStringList);
        }

        [TestMethod]
        public void StandardiseInputFunctionsIfStripping()
        {
            List<string> baseTestStringList = new List<string>();
            baseTestStringList.Add("this text should not be changed as it's already in standard form");
            baseTestStringList.Add("     This text      should be changed as it has inc LTD co.");
            List<string> goalTestStringList = new List<string>();
            goalTestStringList.Add("this text should not be changed as it's already in standard form");
            goalTestStringList.Add("this text should be changed as it has");
            InputSanitiser.StandardiseInput(baseTestStringList, true);
            CollectionAssert.AreEqual(goalTestStringList, baseTestStringList);
        }

        [TestMethod]
        public void StandardiseInputHandlesEmptyList()
        {
            List<string> baseTestStringList = new List<string>();
            List<string> goalTestStringList = new List<string>();
            InputSanitiser.StandardiseInput(baseTestStringList);
            CollectionAssert.AreEqual(baseTestStringList, goalTestStringList);
        }

        [TestMethod]
        public void StandardiseInputHandlesEmptyValues()
        {
            List<string> baseTestStringList = new List<string>();
            baseTestStringList.Add("this text should not be changed as it is already in standard form");
            baseTestStringList.Add(string.Empty);
            List<string> goalTestStringList = new List<string>();
            goalTestStringList.Add("this text should not be changed as it is already in standard form");
            goalTestStringList.Add(string.Empty);
            InputSanitiser.StandardiseInput(baseTestStringList);
            CollectionAssert.AreEqual(goalTestStringList, baseTestStringList);
        }

    }
}
