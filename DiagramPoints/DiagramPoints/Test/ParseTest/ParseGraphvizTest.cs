using DiagramPoints;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Parse.Test {
    [TestFixture]
    public class SimpleParseTest {
        [Test]
        // graph { a -- b; }
        public void SimpleParse() {
            DiagramHelper graph = new DiagramHelper();
            string textProgram = "graph { a -- b; }";
            
            graph.Load(textProgram);

            Assert.IsNotNull(graph.DiagramItems);
            Assert.AreEqual(2, graph.DiagramItems.Count);
            Assert.AreEqual("a", graph.DiagramItems[0].Name);
            Assert.AreEqual("b", graph.DiagramItems[1].Name);

            Assert.IsNotNull(graph.DiagramRelations);
            Assert.AreEqual(1, graph.DiagramRelations.Count);
            Assert.AreEqual("a", graph.DiagramRelations[0].Item1.Name);
            Assert.AreEqual("b", graph.DiagramRelations[0].Item2.Name);
        }

        [Test]
        // graph { a -- b; b -- c; }
        public void AverageParse() {
            DiagramHelper graph = new DiagramHelper();
            string textProgram = "graph { a -- b; b -- c; }";

            graph.Load(textProgram);

            Assert.IsNotNull(graph.DiagramItems);
            Assert.AreEqual(3, graph.DiagramItems.Count);
            Assert.AreEqual("a", graph.DiagramItems[0].Name);
            Assert.AreEqual("b", graph.DiagramItems[1].Name);
            Assert.AreEqual("c", graph.DiagramItems[2].Name);

            Assert.IsNotNull(graph.DiagramRelations);
            Assert.AreEqual(2, graph.DiagramRelations.Count);
            Assert.AreEqual("a", graph.DiagramRelations[0].Item1.Name);
            Assert.AreEqual("b", graph.DiagramRelations[0].Item2.Name);
            Assert.AreEqual("b", graph.DiagramRelations[1].Item1.Name);
            Assert.AreEqual("c", graph.DiagramRelations[1].Item2.Name);
        }

        [Test]
        // graph { a -- { b, c }; }
        public void HighParse() {
            DiagramHelper graph = new DiagramHelper();
            string textProgram = "graph { a -- { b c }; }";

            graph.Load(textProgram);

            Assert.IsNotNull(graph.DiagramItems);
            Assert.AreEqual(3, graph.DiagramItems.Count);
            Assert.AreEqual("a", graph.DiagramItems[0].Name);
            Assert.AreEqual("b", graph.DiagramItems[1].Name);
            Assert.AreEqual("c", graph.DiagramItems[2].Name);

            Assert.IsNotNull(graph.DiagramRelations);
            Assert.AreEqual(2, graph.DiagramRelations.Count);
            Assert.AreEqual("a", graph.DiagramRelations[0].Item1.Name);
            Assert.AreEqual("b", graph.DiagramRelations[0].Item2.Name);
            Assert.AreEqual("a", graph.DiagramRelations[1].Item1.Name);
            Assert.AreEqual("c", graph.DiagramRelations[1].Item2.Name);
        }

        [Test]
        public void LargeTest() {
            DiagramHelper graph = new DiagramHelper();
            string textProgram = GetTextProgram();

            graph.Load(textProgram);

            Assert.IsNotNull(graph.DiagramItems);
            Assert.AreEqual(21, graph.DiagramItems.Count);

            Assert.IsNotNull(graph.DiagramRelations);
            Assert.AreEqual(41, graph.DiagramRelations.Count);
        }
        public static string GetTextProgram() {
            return "graph {	a -- { b c d }; b -- { c e }; }";
        }
        public static string GetTextProgramEx() {
            return "graph {	a -- { b c d }; b -- { c e }; c -- { e f }; d -- { f g }; e -- h; f -- { h i j g }; g -- k; h -- l; i -- { l m j }; j -- { m n k }; k -- { n r }; l -- { o m }; m -- { o p n }; n -- { q r }; o -- { s p }; p -- { s t q }; q -- { t r }; r -- t; s -- z;  t -- z; }";
        }

        // graph { a -> b; }
        // graph { a -> b; b -> c; }
        // graph { a -> { b, c }; }
    }

    [TestFixture]
    public class ParserTest {
        [Test]
        public void ParserText_GetNodes() {
            string textProgram = "graph { a -- b; }";
            DiagramHelper graph = new DiagramHelper();
            ParseDot parserDot = new ParseDot(graph, textProgram);

            List<DiagramItem> nodes = parserDot.GetDiagramItems();

            Assert.IsNotNull(nodes);
            Assert.AreEqual(2, nodes.Count);
            Assert.AreEqual("a", nodes[0].Name);
            Assert.AreEqual("b", nodes[1].Name);
        }

        [Test]
        public void ParserText_GetDiagramRelation() {
            string textProgram = "graph { a -- b; }";
            DiagramHelper graph = new DiagramHelper();
            ParseDot parserDot = new ParseDot(graph, textProgram);

            List<DiagramRelation> DiagramRelation = parserDot.GetDiagramRelations();
            Assert.IsNotNull(DiagramRelation);
            Assert.AreEqual(1, DiagramRelation.Count);
            Assert.AreEqual("a", DiagramRelation[0].Item1.Name);
            Assert.AreEqual("b", DiagramRelation[0].Item2.Name);
        }
    }
}
