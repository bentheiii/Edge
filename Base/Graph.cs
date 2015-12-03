using System;
using System.Collections.Generic;
using System.Linq;
using CCDefault.Annotations;
using Edge.Arrays;
using Edge.Arrays.Arr2D;
using Edge.Comparison;
using Edge.Fielding;
using Edge.Looping;

namespace Edge.Graphs
{
    public interface IDirectedGraph<VT,ET>
    {

        ET this[VT src, VT dst] { get; set; }
        [Pure]
        IEnumerable<VT> getVertexes();
        [Pure]
        bool existsEdge(VT src, VT dst);
        void ClearEdge(VT src, VT dst);
        [Pure]
	    GraphNode<VT, ET> GetNode(VT val);
    }
	public class GraphNode<VT, ET>
	{
		public VT val { get; }
		public IEnumerable<Tuple<ET,VT>> connections { get; }
		public GraphNode(VT val,params Tuple<ET, VT>[] connections)
		{
			this.val = val;
			this.connections = connections;
		}
        public GraphNode(VT val, IEnumerable<Tuple<ET, VT>> connections)
        {
            this.val = val;
            this.connections = connections;
        }
    }
    public class GraphEdge<VT, ET> : IEquatable<GraphEdge<VT,ET>>
    {
        public bool Equals(GraphEdge<VT, ET> other)
        {
            return EqualityComparer<VT>.Default.Equals(@from, other.@from) && EqualityComparer<VT>.Default.Equals(to, other.to) && EqualityComparer<ET>.Default.Equals(weight, other.weight);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = EqualityComparer<VT>.Default.GetHashCode(@from);
                hashCode = (hashCode * 397) ^ EqualityComparer<VT>.Default.GetHashCode(to);
                hashCode = (hashCode * 397) ^ EqualityComparer<ET>.Default.GetHashCode(weight);
                return hashCode;
            }
        }
        public GraphEdge(VT @from, VT to, ET weight)
        {
            this.@from = @from;
            this.to = to;
            this.weight = weight;
        }
        public GraphEdge(IDirectedGraph<VT, ET> g, VT from, VT to):this(from,to,g[from,to])
        {
            
        } 
        public VT from { get; }
        public VT to { get; }
        public ET weight { get; }
        public override bool Equals(object obj)
        {
            GraphEdge<VT, ET> edge = obj as GraphEdge<VT, ET>;
            // ReSharper disable once BaseObjectEqualsIsObjectEquals
            return edge != null ? this.Equals(edge) : base.Equals(obj);
        }
    }
    public interface IGraph<VT, ET> : IDirectedGraph<VT, ET> { }
    // ReSharper disable once InconsistentNaming
    public interface IRWGraph<VT, ET> : IDirectedGraph<VT, ET>
    {
        void AddVertex(VT ver);
        void RemoveVertex(VT ver);
    }
    public class SparceDirectedGraph<VT, ET> : IRWGraph<VT,ET>
    {
        protected readonly IDictionary<VT, IDictionary<VT,ET>> _links;
        public SparceDirectedGraph(params VT[] verts)
        {
            this._links = new Dictionary<VT, IDictionary<VT, ET>>(verts.Length);
            foreach (VT v in verts)
            {
                _links[v] = new Dictionary<VT, ET>(verts.Length);
            }
        }
        public virtual ET this[VT src, VT dst]
        {
            get
            {
                if (!_links.ContainsKey(src))
                    this.AddVertex(src);
                if (!_links.ContainsKey(dst))
                    this.AddVertex(dst);
                return this._links[src].ContainsKey(dst) ? this._links[src][dst] : default(ET);
            }
            set
            {
                if (!_links.ContainsKey(src))
                    this.AddVertex(src);
                if (!_links.ContainsKey(dst))
                    this.AddVertex(dst);
                _links[src][dst] = value;
            }
        }
        public void AddVertex(VT ver)
        {
            _links.Add(ver,new Dictionary<VT, ET>(_links.Count));
        }
        public void RemoveVertex(VT ver)
        {
            _links.Remove(ver);
            foreach (var keyValuePair in _links)
            {
                keyValuePair.Value.Remove(ver);
            }
        }
        public IEnumerable<VT> getVertexes()
        {
            return _links.ToArray().SelectToArray(p => p.Key);
        }
        public virtual bool existsEdge(VT src, VT dst)
        {
            if (!_links.ContainsKey(src) || !_links.ContainsKey(dst))
                throw new Exception("vertex outside of graph");
            return this._links[src].ContainsKey(dst);
        }
        public virtual void ClearEdge(VT src, VT dst)
        {
            _links[src].Remove(dst);
        }
	    public virtual GraphNode<VT, ET> GetNode(VT val)
	    {
		    return new GraphNode<VT, ET>(val,_links[val].SelectToArray(a=>new Tuple<ET,VT>(a.Value,a.Key)));
	    }
    }
    public class SparceGraph<VT, ET> : SparceDirectedGraph<VT,ET>, IGraph<VT,ET> where VT:IComparable<VT>
    {
        public SparceGraph(params VT[] verts) : base(verts)
        {}
        public override ET this[VT src, VT dst]
        {
            get
            {
                if (src.CompareTo(dst) > 0)
                    return this[dst, src];
                return this._links[src].ContainsKey(dst) ? this._links[src][dst] : default(ET);
            }
            set
            {
                if (src.CompareTo(dst) > 0)
                    this[dst, src] = value;
                _links[src][dst] = value;
            }
        }
        public override void ClearEdge(VT src, VT dst)
        {
            if (src.CompareTo(dst) > 0)
                ClearEdge(dst, src);
            base.ClearEdge(src, dst);
        }
        public override bool existsEdge(VT src, VT dst)
        {
            while (true)
            {
                if (src.CompareTo(dst) > 0)
                {
                    var src1 = src;
                    src = dst;
                    dst = src1;
                    continue;
                }
                return base.existsEdge(src, dst);
            }
        }
        public override GraphNode<VT, ET> GetNode(VT val)
        {
            return new GraphNode<VT, ET>(val,this.getVertexes().Where(a=>this.existsEdge(val,a)).Attach(a=>this[val,a]).Select(a=>Tuple.Create(a.Item2,a.Item1)));
        }
    }
    public sealed class DirectedGraph<VT, ET> : IDirectedGraph<VT, ET>
    {
        private readonly IDictionary<VT,int> _indexer;
        private readonly ET[,] _links;
        private readonly ET _def;
        public DirectedGraph(params VT[] verts)
            : this(default(ET), verts)
        {

        }
        public DirectedGraph(ET defaultvalue,params VT[] verts)
        {
            _indexer = new Dictionary<VT, int>(verts.Length);
            _def = defaultvalue;
            _links = Arr2D.Fill(verts.Length, verts.Length, defaultvalue);
            int ind = 0;
            foreach (VT v in verts)
            {
                _indexer[v] = ind;
                ind++;
            }
        }
        public ET this[VT src, VT dst]
        {
            get
            {
                if (!_indexer.ContainsKey(src) || !_indexer.ContainsKey(dst))
                    throw new ArgumentOutOfRangeException("vertex outside of graph");
                return this._links[_indexer[src],_indexer[dst]];
            }
            set
            {
                if (!_indexer.ContainsKey(src) || !_indexer.ContainsKey(dst))
                    throw new Exception("vertex outside of graph");
                this._links[_indexer[src], _indexer[dst]] = value;
            }
        }
        public IEnumerable<VT> getVertexes()
        {
            return _indexer.ToArray().SelectToArray(p => p.Key);
        }
        public bool existsEdge(VT src, VT dst)
        {
            if (!_indexer.ContainsKey(src) || !_indexer.ContainsKey(dst))
                throw new Exception("vertex outside of graph");
            return !this._links[_indexer[src], _indexer[dst]].Equals(_def);
        }
        public void ClearEdge(VT src, VT dst)
        {
            this[src, dst] = _def;
        }
	    public GraphNode<VT, ET> GetNode(VT val)
	    {
		    return new GraphNode<VT, ET>(val,
			    this.getVertexes().Where(a => existsEdge(val, a))
				.SelectToArray(a => new Tuple<ET, VT>(this[val, a], a)));
	    }
    }
    public sealed class Graph<VT, ET> : IGraph<VT, ET>
    {
        private readonly IDictionary<VT,int> _indexer;
        private readonly SymmetricMatrix<ET> _links;
        private readonly ET _def;
        public Graph(params VT[] verts) : this(default(ET), verts)
        {

        }
        public Graph(ET defaultvalue, params VT[] verts)
        {
            _indexer = new Dictionary<VT, int>(verts.Length);
            _def = defaultvalue;
            _links = new SymmetricMatrix<ET>(verts.Length);
            int ind = 0;
            foreach (VT v in verts)
            {
                _indexer[v] = ind;
                ind++;
            }
            for (int i = 0; i < verts.Length; i++)
            {
                for (int j = 0; j < verts.Length; j++)
                {
                    _links[i, j] = _def;
                }
            }
        }
        public ET this[VT src, VT dst]
        {
            get
            {
                if (!_indexer.ContainsKey(src) || !_indexer.ContainsKey(dst))
                    throw new ArgumentOutOfRangeException("vertex outside of graph");
                return this._links[_indexer[src],_indexer[dst]];
            }
            set
            {
                if (!_indexer.ContainsKey(src) || !_indexer.ContainsKey(dst))
                    throw new Exception("vertex outside of graph");
                this._links[_indexer[src], _indexer[dst]] = value;
            }
        }
        public IEnumerable<VT> getVertexes()
        {
            return _indexer.ToArray().SelectToArray(p => p.Key);
        }
        public bool existsEdge(VT src, VT dst)
        {
            if (!_indexer.ContainsKey(src) || !_indexer.ContainsKey(dst))
                throw new Exception("vertex outside of graph");
            return !this._links[_indexer[src], _indexer[dst]].Equals(_def);
        }
        public void ClearEdge(VT src, VT dst)
        {
            this[src, dst] = _def;
        }
	    public GraphNode<VT, ET> GetNode(VT val)
	    {
			return new GraphNode<VT, ET>(val,
				this.getVertexes().Where(a => existsEdge(val, a))
				.SelectToArray(a => new Tuple<ET, VT>(this[val, a], a)));
		}
    }
    public class VirtualGraph<VT, ET> : IDirectedGraph<VT, ET>
    {
        private readonly IEnumerable<VT> _vertexes;
        private readonly Func<VT, IEnumerable<Tuple<ET, VT>>> _edgefunction;
        public VirtualGraph(Func<VT, IEnumerable<Tuple<ET, VT>>> edgefunction, IEnumerable<VT> vertexes)
        {
            _edgefunction = edgefunction;
            _vertexes = vertexes;
        }
        public ET this[VT src, VT dst]
        {
            get
            {
                var applicable = _edgefunction(src).Where(a => a.Item2.Equals(dst));
                return applicable.Any() ? applicable.FirstOrDefault().Item1 : default(ET);
            }
            set
            {
                throw new AccessViolationException("Cannot change value of virtual graph");
            }
        }
        public IEnumerable<VT> getVertexes()
        {
            return _vertexes;
        }
        public bool existsEdge(VT src, VT dst)
        {
            return _edgefunction(src).Any(a => a.Item2.Equals(dst));
        }
        public void ClearEdge(VT src, VT dst)
        {
            throw new AccessViolationException("Cannot change value of virtual graph");
        }
        public GraphNode<VT, ET> GetNode(VT val)
        {
            return new GraphNode<VT, ET>(val,_edgefunction(val));
        }
    }
    public static class GraphExtentions
    {
        public static void SetEdge<VT, ET>(this IDirectedGraph<VT, ET> @this, VT start, VT dest, ET val)
        {
            @this[start, dest] = val;
        }
        public static bool HasPath<VT, ET>(this IDirectedGraph<VT, ET> @this, VT start, VT dest)
		{
			return @this.Path(start,dest) != null;
		}
		[CanBeNull] public static VT[] Path<VT, ET>(this IDirectedGraph<VT, ET> @this, VT start, VT dest)
	    {
		    return @this.Path(start, dest,new HashSet<VT>())?.ToArray();
	    }
        private static LinkedList<VT> Path<VT, ET>(this IDirectedGraph<VT, ET> @this, VT start, VT dest, ISet<VT> toIgnore)
		{
			toIgnore.Add(start);
			foreach (VT vertex in @this.getOutNeighbors(start))
			{
				if (toIgnore.Contains(vertex))
					continue;
				if (vertex.Equals(dest))
					return new LinkedList<VT>(new []{start,dest});
				var ret = @this.Path(vertex, dest, toIgnore);
				if (ret != null)
				{
					ret.AddFirst(start);
					return ret;
				}
			}
			return null;
		}
        public static GraphEdge<VT, ET> getEdge<VT, ET>(this IDirectedGraph<VT, ET> @this, VT from, VT to) => new GraphEdge<VT, ET>(@this,@from,to);
        public static GraphEdge<VT, ET>[] getAllEdges<VT, ET>(this IDirectedGraph<VT, ET> @this)
        {
            ISet<GraphEdge < VT, ET >> ret = new HashSet<GraphEdge<VT, ET>>();
            foreach (Tuple<VT, VT> tuple in @this.getVertexes().Join(@this is IGraph<VT,ET> ? Loops.CartesianType.NoSymmatry : Loops.CartesianType.AllPairs))
            {
                if (@this.existsEdge(tuple.Item1, tuple.Item2))
                    ret.Add(@this.getEdge(tuple.Item1, tuple.Item2));
            }
            return ret.ToArray();
        }
        public static VT[][] Components<VT, ET>(this IGraph<VT, ET> @this)
	    {
		    ISet<VT> toignore = new HashSet<VT>();
			IList<VT[]> ret = new List<VT[]>();
		    foreach (VT vertex in @this.getVertexes())
		    {
				if(toignore.Contains(vertex))
					continue;
			    var t = @this.Components(vertex, new HashSet<VT>());
				toignore.UnionWith(t);
				ret.Add(t.ToArray());
		    }
		    return ret.ToArray();
	    }
		private static ISet<VT> Components<VT, ET>(this IGraph<VT, ET> @this,VT start,ISet<VT> toIgnore)
		{
			toIgnore.Add(start);
			HashSet<VT> ret = new HashSet<VT>();
			ret.Add(start);
			foreach (VT vertex in @this.getOutNeighbors(start))
			{
				if (toIgnore.Contains(vertex))
					continue;
				ret.UnionWith(@this.Components(vertex,toIgnore));
			}
			return ret;
		}
	    public static bool isConnected<VT, ET>(this IGraph<VT, ET> @this)
	    {
		    return @this.Components().Length == 1;
	    }
	    public static VT[] getDecendants<VT, ET>(this IDirectedGraph<VT, ET> @this, VT start)
	    {
		    return @this.getDecendants(start, new HashSet<VT>()).ToArray();
	    }
		private static ISet<VT> getDecendants<VT, ET>(this IDirectedGraph<VT, ET> @this, VT start,ISet<VT> toIgnore)
		{
			ISet<VT> ret = new HashSet<VT>();
			foreach (VT vertex in @this.getOutNeighbors(start))
			{
				if (toIgnore.Contains(vertex))
					continue;
			    ret.Add(vertex);
                toIgnore.Add(vertex);
                ret.UnionWith(@this.getDecendants(vertex,toIgnore));
			}
			return ret;
		}
        public static VT[] getInNeighbors<VT, ET>(this IDirectedGraph<VT, ET> @this, VT start)
        {
            ISet<VT> ret = new HashSet<VT>();
            foreach (VT vertex in @this.getVertexes())
            {
                if (!@this.existsEdge(vertex,start))
                    continue;
                ret.Add(vertex);
            }
            return ret.ToArray();
        }
        public static VT[] getOutNeighbors<VT, ET>(this IDirectedGraph<VT, ET> @this, VT start)
        {
            return @this.GetNode(start).connections.SelectToArray(a => a.Item2);
        }
        public static bool isConnected<VT, ET>(this IDirectedGraph<VT, ET> @this)
		{
			VT[] allv = @this.getVertexes().ToArray();
			foreach (VT vertex in @this.getVertexes())
			{
				VT[] decendants = @this.getDecendants(vertex);
				if (!decendants.ContainsAll(allv))
					return false;
			}
			return true;
		}
		[CanBeNull] public static VT[] ShortestPath<VT, ET>(this IDirectedGraph<VT, ET> @this, VT start, VT dest, Func<ET, double> map)
		{
			if (!@this.HasPath(start,dest))
				return null;
            VT[] vertexes = @this.getVertexes().ToArray();
            IDictionary<VT, double> potential = new Dictionary<VT, double>(vertexes.Length);
            IDictionary<VT, VT> prev = new Dictionary<VT, VT>(vertexes.Length);
            ISet<VT> unscanned = new HashSet<VT>();
            foreach (VT v in vertexes)
            {
                potential[v] = double.PositiveInfinity;
                prev[v] = default(VT);
                unscanned.Add(v);
            }
            potential[start] = 0;
            while (unscanned.Count != 0)
            {
                VT minvertex = unscanned.getMin(new FunctionComparer<VT>(v => potential[v]));
                if (minvertex.Equals(dest))
                    break;
                unscanned.Remove(minvertex);
                foreach (VT v in unscanned)
                {
                    if (!@this.existsEdge(minvertex, v))
                        continue;
                    double alt = map(@this[minvertex, v]) + potential[minvertex];
                    if (alt < potential[v])
                    {
                        potential[v] = alt;
                        prev[v] = minvertex;
                    }
                }
            }
            LinkedList<VT> ret = new LinkedList<VT>();
            VT current = dest;
            while (true)
            {
                ret.AddFirst(current);
                if (current.Equals(start))
                    break;
                current = prev[current];
            }
            return ret.ToArray();
        }
        public static VT[] ShortestPath<VT, ET>(this IDirectedGraph<VT, ET> g, VT start, VT dest)
        {
            return g.ShortestPath(start, dest, et => (double)et.ToFieldWrapper());
        }
        public static int getDegree<VT, ET>(this IDirectedGraph<VT, ET> g, VT node)
        {
            return g.GetNode(node).connections.Count();
        }
	    [CanBeNull] public static VT[] TopologicalSort<VT, EG>(this IDirectedGraph<VT, EG> @this)
	    {
		    LinkedList<VT> ret = new LinkedList<VT>();
		    VT[] vertexes = @this.getVertexes().ToArray();
		    Dictionary<VT,int> d = @this.getVertexes().ToDictionary(a=>a,a => 0);
            while (d.Any(a=>a.Value==0))
		    {
			    VT node = @this.getVertexes().First(a => d[a] == 0);
			    if (!@this.TopologicalSort(node, d, ret))
				    return null;
		    }
		    return ret.ToArray();
	    }
	    private static bool TopologicalSort<VT, EG>(this IDirectedGraph<VT, EG> @this, VT node, IDictionary<VT, int> d,LinkedList<VT> l)
	    {
		    switch (d[node]) {
			    case 1:
				    return false;
			    case 2:
				    return true;
		    }
		    d[node] = 1;
		    foreach (VT vertex in @this.getOutNeighbors(node))
		    {
			    if (!@this.TopologicalSort(vertex, d, l))
				    return false;
		    }
		    d[node] = 2;
		    l.AddFirst(node);
		    return true;
	    }
	    public static void Add<VT, ET>(this IRWGraph<VT, ET> @this, GraphNode<VT, ET> n)
	    {
		    @this.AddVertex(n.val);
		    foreach (var t in n.connections)
		    {
			    @this[n.val, t.Item2] = t.Item1;
		    }
	    }
        public static ET getPathWeight<VT, ET>(this IDirectedGraph<VT, ET> @this, IEnumerable<VT> path)
        {
            return getPathWeight(@this, path, a=>a);
        }
        public static RT getPathWeight<RT, VT, ET>(this IDirectedGraph<VT, ET> @this, IEnumerable<VT> path, Func<ET, RT> f)
	    {
		    return path.Trail2().Select(a => f(@this[a.Item1, a.Item2])).getSum();
	    }
        public enum ProcedureLooping { Cont, EndAll, EndCurrent}
        public static void DepthFirstSearch<VT, ET>(this IDirectedGraph<VT, ET> @this, Func<VT, ProcedureLooping> procedure)
        {
            ISet<VT> visited = new HashSet<VT>();
            ISet<VT> all = new HashSet<VT>(@this.getVertexes());
            ISet<VT> unvisited = new HashSet<VT>(all);
            unvisited.ExceptWith(visited);
            while (unvisited.Any())
            {
                @this.DepthFirstSearch(unvisited.First(), vert =>
                {
                    visited.Add(vert);
                    return procedure(vert);
                });
                unvisited.ExceptWith(visited);
            }
        }
        public static void DepthFirstSearch<VT, ET>(this IDirectedGraph<VT, ET> @this, VT start, Func<VT, ProcedureLooping> procedure)
        {
            DepthFirstSearchHelper(@this, start, procedure,start);
        }
        private static ProcedureLooping DepthFirstSearchHelper<VT, ET>(this IDirectedGraph<VT, ET> @this, VT start, Func<VT, ProcedureLooping> procedure,VT prev)
        {
            var cont = procedure(start);
            var iEnumerable = @this.getOutNeighbors(start);
            var outNeighbors = iEnumerable.Except(prev);
            foreach (VT outNeighbor in outNeighbors)
            {
                if(cont != ProcedureLooping.Cont)
                    break;
                cont = DepthFirstSearchHelper(@this, outNeighbor, procedure,start);
            }
            return cont == ProcedureLooping.EndAll ? ProcedureLooping.EndAll : ProcedureLooping.Cont;
        }
        public static void DepthFirstSearchFinite<VT, ET>(this IDirectedGraph<VT, ET> @this, Action<VT> procedure)
        {
            DepthFirstSearchFinite(@this, a =>
            {
                procedure(a);
                return ProcedureLooping.Cont;
            });
        }
        public static void DepthFirstSearchFinite<VT, ET>(this IDirectedGraph<VT, ET> @this, Func<VT, ProcedureLooping> procedure)
        {
            ISet<VT> visited = new HashSet<VT>();
            ISet<VT> all = new HashSet<VT>(@this.getVertexes());
            ISet<VT> unvisited = new HashSet<VT>(all);
            unvisited.ExceptWith(visited);
            while (unvisited.Count > 0)
            {
                @this.DepthFirstSearch(unvisited.First(), vert =>
                {
                    if (visited.ContainsAll(all))
                        return ProcedureLooping.EndAll;
                    if (visited.Contains(vert))
                        return ProcedureLooping.EndCurrent;
                    visited.Add(vert);
                    return procedure(vert);
                });
                unvisited.ExceptWith(visited);
            }
            
        }
        public static bool hasCircularPath<VT,ET>(this IDirectedGraph<VT, ET> @this)
        {
            ISet<VT> visited = new HashSet<VT>();
            bool found = false;
            @this.DepthFirstSearch(a =>
            {
                if (visited.Contains(a))
                {
                    found = true;
                    return ProcedureLooping.EndAll;
                }
                visited.Add(a);
                return ProcedureLooping.Cont;
            });
            return found;
        }
        public static void FillEdges<VT, ET>(this IDirectedGraph<VT, ET> @this, ET edgeval, params VT[] edges)
        {
            foreach (Tuple<VT, VT> tuple in edges.Group2())
            {
                @this[tuple.Item1, tuple.Item2] = edgeval;
            }
        }
        public static void FillEdges3<VT>(this IDirectedGraph<VT,VT> @this, params VT[] edges)
        {
            foreach (Tuple<VT, VT, VT> tuple in edges.Group3())
            {
                @this[tuple.Item1, tuple.Item2] = tuple.Item3;
            }
        }
        public static void Add<VT, ET>(this IDirectedGraph<VT, ET> @this, GraphEdge<VT, ET> edge)
        {
            @this[edge.from, edge.to] = edge.weight;
        }
        public static void Remove<VT, ET>(this IDirectedGraph<VT, ET> @this, GraphEdge<VT, ET> edge)
        {
            @this.ClearEdge(edge.from,edge.to);
        }
        public static IDirectedGraph<VT, ET> getMinimumSpanningTree<VT, ET>(this IDirectedGraph<VT, ET> @this, Comparer<ET> comparer)
        {
            IDirectedGraph<VT, ET> ret = new DirectedGraph<VT, ET>(@this.getVertexes().ToArray());
            GraphEdge<VT, ET>[] edges = @this.getAllEdges().Sort(new FunctionComparer<GraphEdge<VT,ET>>(a=>a.weight,comparer));
            foreach (GraphEdge<VT, ET> graphEdge in edges)
            {
                if (ret.isConnected())
                    return ret;
                ret.Add(graphEdge);
                if (ret.hasCircularPath())
                    ret.Remove(graphEdge);
            }
            throw new Exception("graph is not connected");
        }
    }
}
