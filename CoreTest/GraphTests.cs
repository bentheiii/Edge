using System;
using System.Collections.Generic;
using System.Linq;
using Edge.Arrays;
using Edge.Comparison;
using Edge.Graphs;
using Edge.Looping;
using Edge.NumbersMagic;
using Edge.Tuples;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    public static class AssertGraph
    {
        //expected vertex size is 8
        public static void Check(this IDirectedGraph<int, bool> @this)
        {
            IsTrue(@this.getVertexes().OrderBy().SequenceEqual(Loops.Range(8)));
            @this.FillEdges(true, 0, 2, 0, 3, 1, 2, 1, 6, 4, 2, 4, 7, 5, 1, 5, 6, 5, 7, 6, 7);
            IsTrue(@this[1, 6]);
            IsTrue(@this.existsEdge(5, 6));
            IsFalse(@this.existsEdge(2, 1));
            @this[5, 2] = true;
            IsTrue(@this.existsEdge(5, 2));
            @this.ClearEdge(5, 2);
            IsFalse(@this.existsEdge(5, 2));
            var one = @this.GetNode(1);
            AreEqual(one.val, 1);
            IsTrue(one.connections.Select(a => a.Item2).OrderBy().SequenceEqual(new int[] {2, 6}));
            var two = @this.GetNode(2);
            AreEqual(two.val, 2);
            IsTrue(two.connections.Select(a => a.Item2).OrderBy().SequenceEqual(new int[] {}));
        }
        //expected vertex size is 8
        public static void Check(this IGraph<int, bool> @this)
        {
            IsTrue(@this.getVertexes().OrderBy().SequenceEqual(Loops.Range(8)));
            @this.FillEdges(true, 0, 2, 0, 3, 1, 2, 1, 6, 4, 2, 4, 7, 5, 1, 5, 6, 5, 7, 6, 7);
            IsTrue(@this[1, 6]);
            IsTrue(@this.existsEdge(5, 6));
            IsTrue(@this.existsEdge(6, 5));
            IsFalse(@this.existsEdge(7, 1));
            @this[5, 2] = true;
            IsTrue(@this.existsEdge(5, 2));
            IsTrue(@this.existsEdge(2, 5));
            @this.ClearEdge(5, 2);
            IsFalse(@this.existsEdge(5, 2));
            IsFalse(@this.existsEdge(2, 5));
            @this[2, 5] = true;
            IsTrue(@this.existsEdge(5, 2));
            IsTrue(@this.existsEdge(2, 5));
            @this.ClearEdge(2, 5);
            IsFalse(@this.existsEdge(5, 2));
            IsFalse(@this.existsEdge(2, 5));
            var one = @this.GetNode(1);
            AreEqual(one.val, 1);
            IsTrue(one.connections.Select(a => a.Item2).OrderBy().SequenceEqual(new int[] {2, 5, 6}));
            var two = @this.GetNode(2);
            AreEqual(two.val, 2);
            IsTrue(two.connections.Select(a => a.Item2).OrderBy().SequenceEqual(new int[] {0, 1, 4}));
        }
        //expected vertex size is 8
        public static void CheckRW(this IRWGraph<int, bool> @this)
        {
            @this.AddVertex(10);
            @this[10, 2] = true;
            IsTrue(@this.existsEdge(10, 2));
            @this.ClearEdge(10, 2);
            IsFalse(@this.existsEdge(10, 2));
            @this.RemoveVertex(10);
            IsFalse(@this.getVertexes().Contains(10));
        }
    }
    [TestClass]
    public class DirectedGraphTests
    {
        [TestMethod] public void Sparce()
        {
            new SparceDirectedGraph<int, bool>(Loops.Range(8).ToArray()).Check();
        }
        [TestMethod] public void DirectedGraph()
        {
            new DirectedGraph<int, bool>(Loops.Range(8).ToArray()).Check();
        }
    }
    [TestClass]
    public class UnDirectedGraphTests
    {
        [TestMethod] public void Sparce()
        {
            new SparceGraph<int, bool>(Loops.Range(8).ToArray()).Check();
        }
        [TestMethod] public void Graph()
        {
            new Graph<int, bool>(Loops.Range(8).ToArray()).Check();
        }
    }
    [TestClass]
    public class RWGraphTest
    {
        [TestMethod] public void DirectedRW()
        {
            new SparceDirectedGraph<int, bool>(Loops.Range(8).ToArray()).CheckRW();
        }
        [TestMethod] public void UnDirectedRW()
        {
            new SparceGraph<int, bool>(Loops.Range(8).ToArray()).CheckRW();
        }
    }
    [TestClass]
    public class VirtualGraphTest
    {
        [TestMethod] public void Simple()
        {
            var g = new VirtualGraph<int, bool>(a => a.factors().Except(a).Attach(k => true).Select(n => n.FlipTuple()), Loops.Range(8).ToArray());
            IsTrue(g.getVertexes().OrderBy().SequenceEqual(Loops.Range(8)));
            IsTrue(g.existsEdge(8, 4));
            IsFalse(g.existsEdge(3, 5));
            AreEqual(true, g[5, 1]);
        }
    }
    [TestClass]
    public class GraphExtentionTest
    {
        [TestMethod] public void HasPath()
        {
            var g = new DirectedGraph<int, bool>(Loops.Range(8).ToArray());
            g.FillEdges(true, 0, 2, 1, 0, 5, 1, 5, 3, 5, 6, 6, 2, 6, 4, 6, 7);
            IsTrue(g.HasPath(5, 0));
            IsFalse(g.HasPath(6, 0));
        }
        [TestMethod] public void Path()
        {
            var g = new DirectedGraph<int, bool>(Loops.Range(8).ToArray());
            g.FillEdges(true, 0, 2, 1, 0, 5, 1, 5, 3, 5, 6, 6, 2, 6, 4, 6, 7);
            IsTrue(g.Path(5, 0).SequenceEqual(new int[] {5, 1, 0}));
            IsNull(g.Path(6, 0));
        }
        [TestMethod] public void GetAllEdges()
        {
            var g = new DirectedGraph<int, bool>(Loops.Range(8).ToArray());
            g.FillEdges(true, 0, 2, 1, 0, 5, 1, 5, 3, 5, 6, 6, 2, 6, 4, 6, 7);
            IsTrue(
                g.getAllEdges().OrderBy(new PriorityComparer<GraphEdge<int, bool>>(a => a.from, a => a.to)).SequenceEqual(
                    new int[] {0, 2, 1, 0, 5, 1, 5, 3, 5, 6, 6, 2, 6, 4, 6, 7}.Group2().Select(a => new GraphEdge<int, bool>(a.Item1, a.Item2, true))));
        }
        [TestMethod] public void GetComponentss()
        {
            var g = new Graph<int, bool>(Loops.Range(8).ToArray());
            g.FillEdges(true, 0, 1, 0, 2, 0, 3, 1, 2, 1, 3, 1, 5, 2, 5, 4, 6, 6, 7);
            IsTrue(
                g.Components().Select(a => a.OrderBy()).OrderBy(a => a.First()).SequenceEqual(
                    new int[][] {new int[] {0, 1, 2, 3, 5}, new int[] {4, 6, 7}}, new EnumerableEqualityCompararer<int>()));
        }
        [TestMethod] public void IsConnectedUnDirected()
        {
            var g = new Graph<int, bool>(Loops.Range(8).ToArray());
            g.FillEdges(true, 0, 1, 0, 2, 0, 3, 1, 2, 1, 3, 1, 5, 2, 5, 4, 6, 6, 7);
            IsFalse(g.isConnected());
            g[5, 6] = true;
            IsTrue(g.isConnected());
        }
        [TestMethod] public void GetDecendants()
        {
            var g = new DirectedGraph<int, bool>(Loops.Range(8).ToArray());
            g.FillEdges(true, 1, 3, 1, 6, 2, 1, 2, 4, 5, 3, 6, 4);
            var d = g.getDecendants(1);
            IsTrue(d.OrderBy().SequenceEqual(new int[] {3, 4, 6}));
        }
        [TestMethod] public void getOutNehibors()
        {
            var g = new DirectedGraph<int, bool>(Loops.Range(8).ToArray());
            g.FillEdges(true, 1, 3, 1, 6, 2, 1, 2, 4, 5, 3, 6, 4);
            var d = g.getOutNeighbors(1);
            IsTrue(d.OrderBy().SequenceEqual(new int[] {3, 6}));
        }
        [TestMethod] public void getInNehibors()
        {
            var g = new DirectedGraph<int, bool>(Loops.Range(8).ToArray());
            g.FillEdges(true, 1, 3, 1, 6, 2, 1, 2, 4, 5, 3, 6, 4);
            var d = g.getInNeighbors(4);
            IsTrue(d.OrderBy().SequenceEqual(new int[] {2, 6}));
        }
        [TestMethod] public void IsConnectedDirected()
        {
            var g = new DirectedGraph<int, bool>(Loops.Range(4).ToArray());
            g.FillEdges(true, 0, 1, 1, 2, 2, 3);
            IsFalse(g.isConnected());
            g[3, 0] = true;
            IsTrue(g.isConnected());
        }
        [TestMethod] public void ShortestPath()
        {
            var g = new Graph<int, int>(Loops.Range(8).ToArray());
            g[0, 1] = 1;
            g[0, 2] = 2;
            g[0, 3] = 3;
            g[1, 5] = 5;
            g[2, 4] = 3;
            g[2, 6] = 9;
            g[3, 5] = 8;
            g[4, 7] = 9;
            g[5, 6] = 7;
            var s = g.ShortestPath(0, 7);
            IsTrue(s.SequenceEqual(new int[] { 0, 2, 4, 7}));
            IsTrue(g.ShortestPath(5,4).SequenceEqual(new int[] { 5,1,0, 2, 4 }));
        }
        [TestMethod] public void Degree()
        {
            var g = new Graph<int, bool>(Loops.Range(8).ToArray());
            g.FillEdges(true, 0, 1, 0, 2, 0, 3, 1, 2, 1, 3, 1, 5, 2, 5, 4, 6, 6, 7);
            AreEqual(g.getDegree(1),4);
        }
        [TestMethod] public void TopologicalSort()
        {
            var g = new DirectedGraph<int,bool>(Loops.Range(8).ToArray());
            g.FillEdges(true, 0,1,0,2,0,3,3,7,5,7,6,7);
            var t = g.TopologicalSort();
            IsTrue(t.SequenceEqual(new int[] { 6,5,4,0,3,7,2,1}));
        }
        [TestMethod]
        public void AddNode()
        {
            var g = new SparceDirectedGraph<int, bool>(Loops.Range(1,8).ToArray());
            g.FillEdges(true, 3, 7, 5, 7,6,7);
            var links = new int[] {1, 2, 3}.Attach(a => true).Select(a => a.FlipTuple()).ToArray();
            g.Add(new GraphNode<int,bool>(0, links));
            var t = g.TopologicalSort();
            IsTrue(t.SequenceEqual(new int[] { 0,6, 5, 4,3, 7, 2, 1 }));
        }
        [TestMethod] public void AddEdge()
        {
            var g = new DirectedGraph<int, bool>(Loops.Range(8).ToArray());
            g.FillEdges(true, 0, 1, 0, 2, 0, 3, 3, 7, 5, 7);
            g.Add(new GraphEdge<int, bool>(6,7,true));
            var t = g.TopologicalSort();
            IsTrue(t.SequenceEqual(new int[] { 6, 5, 4, 0, 3, 7, 2, 1 }));
        }
        [TestMethod] public void GetPathWeight()
        {
            var g = new Graph<int, int>(Loops.Range(8).ToArray());
            g[0, 1] = 1;
            g[0, 2] = 3;
            g[0, 3] = 3;
            g[1, 5] = 5;
            g[2, 4] = 3;
            g[2, 6] = 9;
            g[3, 5] = 8;
            g[4, 7] = 9;
            g[5, 6] = 7;
            var s = g.ShortestPath(0, 7);
            AreEqual(g.getPathWeight(s),15);
        }
        [TestMethod]
        public void Dfs()
        {
            var g = new DirectedGraph<int, bool>(Loops.Range(4).ToArray());
            g.FillEdges(true, 0, 1, 1, 2, 2, 3);
            LinkedList<int> r = new LinkedList<int>();
            g.DepthFirstSearch(a =>
            {
                r.AddLast(a);
                return GraphExtentions.ProcedureLooping.Cont;
            });
            IsTrue(r.SequenceEqual(new int[] {0,1,2,3}));
        }
        [TestMethod]
        public void FniteDfs()
        {
            var g = new DirectedGraph<int, bool>(Loops.Range(4).ToArray());
            g.FillEdges(true, 0, 1, 1, 2, 2, 3,3,1);
            LinkedList<int> r = new LinkedList<int>();
            g.DepthFirstSearchFinite(a =>
            {
                r.AddLast(a);
            });
            IsTrue(r.SequenceEqual(new int[] { 0, 1, 2, 3 }));
        }
        [TestMethod]
        public void HasCircularPath()
        {
            var g = new DirectedGraph<int, bool>(Loops.Range(5).ToArray());
            g.FillEdges(true, 0, 1, 1, 2, 2, 3);
            IsFalse(g.hasCircularPath());
            g[3, 0] = true;
            IsTrue(g.hasCircularPath());
        }
        [TestMethod] public void FillEdges2()
        {
            var g = new DirectedGraph<int, double>(Loops.Range(5).ToArray());
            g.FillEdges(1.5, 0, 1, 1, 2, 2, 3, 3, 4, 4, 0);
            IsTrue(Loops.Range(5).Join().All(a=> (a.Item1+1)%5 == a.Item2 ? g[a.Item1,a.Item2] == 1.5 : !g.existsEdge(a.Item1,a.Item2)));
        }
        [TestMethod]
        public void FillEdges3()
        {
            var g = new DirectedGraph<int, int>(Loops.Range(5).ToArray());
            g.FillEdges3(0, 1, 1, 1, 2 ,4, 2, 3 ,9, 3, 4, 16, 4, 0, 0);
            IsTrue(Loops.Range(5).Join().All(a => (a.Item1 + 1) % 5 == a.Item2 ? g[a.Item1, a.Item2] == a.Item2*a.Item2 : !g.existsEdge(a.Item1, a.Item2)));
        }
    }
}