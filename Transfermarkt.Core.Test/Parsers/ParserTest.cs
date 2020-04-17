using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.Test.Parsers
{
    [TestClass]
    public class ParserTest
    {
        [TestMethod, TestCategory("Core"), TestCategory("Nomenclatures")]
        public void ParsersClassNameEndsWithSuffix()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            //TODO: create method to get all different implementations of IElementParser
            var parserClasses = TestParserOfType<HtmlNode>(assemblies);
            foreach (var parser in parserClasses)
            {
                Assert.IsTrue(parser.EndsWith("Parser"), $"{parser} parser class doesn't have the suffix 'Parser'.");
            }
        }

        [TestMethod, TestCategory("Core"), TestCategory("Nomenclatures")]
        public void ParsersClassNameEndsWithSuffix2()
        {
            Type[] typelist = GetTypesInNamespace(Assembly.Load("Transfermarkt.Core"), "Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player");
            for (int i = 0; i < typelist.Length; i++)
            {
                Console.WriteLine(typelist[i].Name);
            }
        }

        private List<string> TestParserOfType<TNode>(Assembly[] assemblies)
        {
            return assemblies.SelectMany(x => x.GetTypes())
                .Where(x => typeof(IElementParser<IElement<IValue>, IValue, TNode>).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(x => x.Name).ToList();
        }

        private Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return
              assembly.GetTypes()
                      .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
                      .ToArray();
        }
    }
}
