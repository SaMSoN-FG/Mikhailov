using DiagramPoints;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Parse {
    public enum TypeGraph { Graph, None };

    public class DiagramItemFactory {
        Hashtable ht = new Hashtable();
        public DiagramItem Create(DiagramHelper owner, string id) {
            if(ht.ContainsKey(id)) return (DiagramItem)ht[id];
            else {
                var result = new DiagramItem(owner) { Name = id };
                ht.Add(id, result);
                return result;
            }
        }
    }
    public class ParseDot {
        string textProgram;
        TypeGraph typeGraph;
        DiagramItemFactory factory = new DiagramItemFactory();
        List<DiagramItem> diagramItemsFull;
        List<DiagramRelation> diagramRelationsFull;
        DiagramHelper owner;

        public ParseDot(DiagramHelper owner, string textProgram) {
            this.owner = owner;
            this.textProgram = textProgram;
            Parse();
        }

        void Parse() {
            CreateDiagramItemsAndDiagramRelations();
            FillingDiagramItemsAndDiagramRelations();
        }

        void FillingDiagramItemsAndDiagramRelations() {
            foreach (string DiagramRelationData in GetDiagramRelationsData(GetTailProgram()))
                if (IsDiagramRelation(DiagramRelationData))
                    AddDiagramRelationsFull(DiagramRelationData);
        }

        void AddDiagramRelationsFull(string DiagramRelationData) {
            if (IsDiagramRelationCombine(DiagramRelationData))
                AddDiagramRelationsFullCombineDiagramRelation(DiagramRelationData);
            else diagramRelationsFull.Add(GetDiagramRelation(DiagramRelationData));
        }

        void AddDiagramRelationsFullCombineDiagramRelation(string DiagramRelationData) {
            string source = DiagramRelationData.Substring(0, DiagramRelationData.IndexOf("-")).Trim();
            string[] targets = DiagramRelationData.Substring(DiagramRelationData.IndexOf("{") + 1, DiagramRelationData.IndexOf("}") - DiagramRelationData.IndexOf("{") - 1).Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < targets.Length; i++) {
                DiagramRelation DiagramRelation = CreateDiagramRelation(new string[] { source, targets[i] });
                diagramRelationsFull.Add(DiagramRelation);
                FullDiagramItemsContainDiagramItemsDiagramRelation(DiagramRelation);
            }
        }

        bool IsDiagramRelationCombine(string DiagramRelationData) {
            return DiagramRelationData.Contains("{") && DiagramRelationData.Contains("}");
        }

        bool IsDiagramRelation(string DiagramRelationData) {
            return DiagramRelationData.Contains("--");
        }

        string[] GetDiagramRelationsData(string teil) {
            return teil.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        string GetTailProgram() {
            string teil = textProgram.Remove(0, textProgram.IndexOf("{"));
            return teil.Trim(new Char[] { '{', '}', ' ' });  
        }

        void CreateDiagramItemsAndDiagramRelations() {
            diagramItemsFull = new List<DiagramItem>();
            diagramRelationsFull = new List<DiagramRelation>();
        }

        public TypeGraph GetTypeGraph() {
            string typeGraphStr = textProgram.Split(new Char[] { ' ' })[0];
            
            switch (typeGraphStr) {
                case "graph":
                    typeGraph = TypeGraph.Graph; break;
                default:
                    typeGraph = TypeGraph.None; break;
            }
            
            return typeGraph;
        }

        public List<DiagramItem> GetDiagramItems() {
            return diagramItemsFull;
        }

        public List<DiagramRelation> GetDiagramRelations() {
            return diagramRelationsFull;
        }

        DiagramRelation GetDiagramRelation(string DiagramRelations) {
            var DiagramItemsData = GetDiagramItemsData(DiagramRelations);
            // TODO replace in IsDiagramRelation
            if (DiagramItemsData.Length > 2) return null;
            
            DiagramRelation DiagramRelation = CreateDiagramRelation(DiagramItemsData);
            FullDiagramItemsContainDiagramItemsDiagramRelation(DiagramRelation);
            return DiagramRelation;
        }

        string[] GetDiagramItemsData(string DiagramRelations) {
            return DiagramRelations.Split(new char[] { '-', ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        void FullDiagramItemsContainDiagramItemsDiagramRelation(DiagramRelation DiagramRelation) {
            // TODO rename AddDiagramItemInFullDiagramItems
            AddDiagramItemInFullDiagramItems(DiagramRelation.Item1);
            AddDiagramItemInFullDiagramItems(DiagramRelation.Item2);
        }

        void AddDiagramItemInFullDiagramItems(DiagramItem DiagramItem) {
            if (!IsFullDiagramItemsContain(DiagramItem)) diagramItemsFull.Add(DiagramItem);
        }

        DiagramRelation CreateDiagramRelation(string[] DiagramItemsData) {
            DiagramRelation DiagramRelation = new DiagramRelation();
            DiagramRelation.Item1 = factory.Create(owner, GetDiagramItemName(DiagramItemsData[0]));
            DiagramRelation.Item2 = factory.Create(owner, GetDiagramItemName(DiagramItemsData[1]));
            return DiagramRelation;
        }

        string GetDiagramItemName(string DiagramItemData) {
            return DiagramItemData.Trim(new Char[] { ' ' });
        }

        bool IsFullDiagramItemsContain(DiagramItem DiagramItem) {
            foreach(DiagramItem item in diagramItemsFull)
                if(item.Name == DiagramItem.Name)
                    return true;
            return false;
        }
    }
}
