﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
namespace MDProcessor
{
    [TestFixture]
    class MarkDownProcessorTests
    {
        public Processor MarkDownProcessor = new Processor();
        [Test]
        public void SplitToParagraphs_ParagraphsSeparatedBySingleEmptyLine_SingleParagraph()
        {
            var text = @"a  \r\nb";
            var expected = new string[1] {@"a  \r\nb"};
            var actual = MarkDownProcessor.SplitToParagraphs(text);
            CollectionAssert.AreEqual(expected, actual);
        }
        [Test]
        public void SplitToParagraphs_ParagraphsSeparatedByTwoOrMoreEmptyLine_SplitToTwoParagraphs()
        {
            var text = @"a
    
            
b";
            var expected = new string[2] {"a", "b" };
            var actual = MarkDownProcessor.SplitToParagraphs(text);
            CollectionAssert.AreEqual(expected, actual);
        }
        [Test]
        public void SplitToParagraphs_ParagraphsWithSpacesAtStartAndEnd_DontLoseSpaces()
        {
            var text = @"a  
    
            
    b";
            var expected = new string[2] { "a  ", "    b" };
            var actual = MarkDownProcessor.SplitToParagraphs(text);
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void FindCodeTagIndices_CodeTagInsideOfText_Find()
        {
            var text = @"abc`d`efg";
            var expected = new int[1] {3};
            var actual = MarkDownProcessor.FindCodeTagIndices(text);
            CollectionAssert.AreEqual(expected, actual);
        }
        [Test]
        public void FindCodeTagIndices_SecondBackQuoteIsEscaped_IgnoreEscapeSymbolAndFind()
        {
            var text = @"abc`d\`efg";
            var expected = new int[1] {3};
            var actual = MarkDownProcessor.FindCodeTagIndices(text);
            CollectionAssert.AreEqual(expected, actual);
        }
        [Test]
        public void FindCodeTagIndices_FirstBackQuoteIsEscaped_CantFind()
        {
            var text = @"\`d`efg";
            var expected = new int[0];
            var actual = MarkDownProcessor.FindCodeTagIndices(text);
            CollectionAssert.AreEqual(expected, actual);
        }
        [Test]
        public void FindCodeTagIndices_ThreeBackQuotes_GetMatchOnlyAtFirst()
        {
            var text = @"a`b`c`d";
            var expected = new int[1] {1};
            var actual = MarkDownProcessor.FindCodeTagIndices(text);
            CollectionAssert.AreEqual(expected, actual);
        }
        [Test]
        public void FindCodeTagIndices_MultipleBackQuotes_FindAllMatches()
        {
            var text = @"`a`b`c`d`e`f`g`";
            var expected = new int[4] { 0,4,8,12 };
            var actual = MarkDownProcessor.FindCodeTagIndices(text);
            CollectionAssert.AreEqual(expected, actual);
        }
        [Test]
        public void FindCodeTagIndices_BackQuotesOnDifferentLines_Find()
        {
            var text = @"d`a

bc`";
            var expected = new int[1] {1};
            var actual = MarkDownProcessor.FindCodeTagIndices(text);
            CollectionAssert.AreEqual(expected, actual);
        }
        [Test]
        public void FindCodeTagIndices_BackQuoteIsDoubleEscaped_DontFind()
        {
            var text = @"\\`b`";
            var expected = new int[1] {2};
            var actual = MarkDownProcessor.FindCodeTagIndices(text);
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeTextTree_SimpleText_GetSingleTextNode()
        {
            var text = @"abc";
            var codeIndices = new int[0];
            var expected = new Node();
            expected.AddChild("abc");
            var tm = new TreeMaker(text, codeIndices);
            var actual = tm.MakeTextTree();
            CollectionAssert.AreEqual(expected.Children, actual.Children);
        }
        [Test]
        public void MakeTextTree_SimpleTextWithEscapeSymbols_ParseEscapeSymbolCorrectly()
        {         
            var text = @"a\\b\c";
            var codeIndices = new int[0];
            var tm = new TreeMaker(text, codeIndices);
            var expected = new Node();
            expected.AddChild(@"a\b\c");
            var actual = tm.MakeTextTree();
            CollectionAssert.AreEqual(expected.Children, actual.Children);
        }
        [Test]
        public void MakeTextTree_EscapeSymbolsBeforeTags_EscapeSymbolsEscapeTags()
        {
            var text = @"\_a b_ _\_c__ d";
            var codeIndices = new int[0];
            var tm = new TreeMaker(text, codeIndices);
            var expected = new Node();
            expected.AddChild(@"_a b_ ");
            var emNode = new Node() {Tag = Tag.Em};
            expected.AddChild(emNode);
            expected.AddChild(@"_c__ d");
            var actual = tm.MakeTextTree();
            CollectionAssert.AreEqual(expected.Children, actual.Children);
        }
    }
}
