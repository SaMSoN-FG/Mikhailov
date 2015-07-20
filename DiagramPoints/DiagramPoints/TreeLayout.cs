using System.Collections.Generic;
using System.Linq;

namespace DiagramPoints {
   class Graph {
        List<DiagramItem> vertices;
        List<Edge> edges;
        public Graph(List<DiagramItem> vertices, List<DiagramRelation> edges) {
            this.vertices = vertices;
            this.edges = new List<Edge>();
            foreach(var item in edges) {
                int item1Id = item.Item1.Id;
                int item2Id = item.Item2.Id;
                this.edges.Add(new Edge() { Relation = item, Weight = edges.Count(e => e.Item1.Id == item1Id || e.Item2.Id == item1Id || e.Item1.Id == item2Id || e.Item2.Id == item2Id && item != e) });
            }

        }
        public List<DiagramItem> Vertices {
            get { return vertices; }
        }
        public List<Edge> Edges {
            get { return edges; }
        }
        public bool IsTree {
            get { return vertices.Count - edges.Count == 1; }
        }
    }
    class Edge {
        public DiagramRelation Relation { get; set; }
        public int Weight { get; set; }
    }
    class GraphProcessor {
        List<DiagramItem> vertices;
        List<DiagramRelation> edges;
        List<DiagramItem> list;
        List<bool> used;
        public GraphProcessor(List<DiagramItem> vertices, List<DiagramRelation> edges) {
            this.vertices = vertices;
            this.edges = edges;
            this.list = new List<DiagramItem>();

            InitUsed(vertices);
        }
        void InitUsed(List<DiagramItem> vertices) {
            used = new List<bool>();
            foreach (var v in vertices) {
                used.Add(false);
            }
        }
        public void DepthFirstSearch(DiagramItem item) {
            used[item.Id] = true;
            list.Add(item);
            var edgesList = edges.Where(x => (x.Item1 == item && x.Item2 != item) || x.Item2 == item);
            foreach (var e in edgesList) {
                if (e.Item1 != item) {
                    var temp = e.Item2;
                    e.Item2 = e.Item1;
                    e.Item1 = temp;
                }
            }
            foreach (var edge in edgesList) {
                if (!used[edge.Item2.Id]) {
                    DepthFirstSearch(edge.Item2);
                }
            }
        }
        public List<Graph> SearchConnectedComponents() {
            var result = new List<Graph>();
            InitUsed(vertices);
            foreach (var v in vertices) {
                if (!used[v.Id]) {
                    list.Clear();
                    DepthFirstSearch(v);
                    AddToResult(result, list);
                }
            }
            return result;
        }
        void AddToResult(List<Graph> result, IEnumerable<DiagramItem> list) {
            var graphEdges = new List<DiagramRelation>();
            SelectEdges(list, graphEdges);
            var graph = new Graph(list.ToList(), graphEdges);
            result.Add(graph);
        }
        void SelectEdges(IEnumerable<DiagramItem> list, List<DiagramRelation> graphEdges) {
            foreach (var v in list) {
                var vertexEdges = edges.Where(x => x.Item1 == v);
                foreach (var e in vertexEdges) {
                    if (!graphEdges.Contains(e)) {
                        graphEdges.Add(e);
                    }
                }
            }
        }
        public List<DiagramRelation> GetMST(List<Edge> edges) {
            edges.OrderBy(x => x.Weight);
            List<DiagramRelation> result = new List<DiagramRelation>();
            List<int> subtreeIds = InitSubTreeIds();
            for (int i = 0; i < edges.Count; i++) {
                var edge = edges[i];
                int item1Id = edge.Relation.Item1.Id;
                int item2Id = edge.Relation.Item2.Id;
                if (subtreeIds[item1Id] != subtreeIds[item2Id]) {
                    result.Add(edge.Relation);
                    CombineTrees(subtreeIds, item1Id, item2Id);
                }
            }
            return result;
        }
        void CombineTrees(List<int> subtreeIds, int item1Id, int item2Id) {
            int currentId = subtreeIds[item2Id];
            int newId = subtreeIds[item1Id];
            for (int i = 0; i < vertices.Count; i++) {
                if (subtreeIds[i] == currentId) {
                    subtreeIds[i] = newId;
                }
            }
        }
        List<int> InitSubTreeIds() {
            var subtreeIds = new List<int>();
            for (int i = 0; i < vertices.Count; i++) {
                subtreeIds.Add(i);
            }
            return subtreeIds;
        }
    }
}
