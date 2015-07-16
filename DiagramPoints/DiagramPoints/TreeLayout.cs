using System.Collections.Generic;
using System.Linq;

namespace DiagramPoints {
    class Graph {
        List<DiagramItem> vertices;
        List<DiagramRelation> edges;
        public Graph(List<DiagramItem> vertices, List<DiagramRelation> edges) {
            this.vertices = vertices;
            this.edges = edges;
        }
        public List<DiagramItem> Vertices {
            get { return vertices; }
        }
        public List<DiagramRelation> Edges {
            get { return edges; }
        }
        public bool IsTree {
            get { return vertices.Count - edges.Count == 1; }
        }
    }
    class GraphProcessor {
        List<DiagramItem> vertices;
        List<DiagramRelation> edges;
        List<List<DiagramItem>> connectedComponents;
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
            var edgesList = edges.Where(x => x.Item1 == item);
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
    }
}
