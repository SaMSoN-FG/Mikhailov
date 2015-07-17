using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace DiagramPoints{
    public partial class Form1 : XtraForm {
        public DiagramHelper helper { get { return diagramControl1.helper; } set { diagramControl1.helper = value; } }
        Timer timer = new Timer();
        
        public Form1() {
            InitializeComponent();
            timer.Interval = 1;
            timer.Tick += timer_Tick;
        }
        void timer_Tick(object sender, EventArgs e) {
            helper.DoBestFit();
            
        }
        public void AddItem(object sender, EventArgs e) {
            DiagramItem item = new DiagramItem(helper);
            rectangleXTextEdit.Value = DiagramConstant.Random.Next(10, 800);
            rectangleYTextEdit.Value = DiagramConstant.Random.Next(10, 800);
            item.Location = new Point((int)rectangleXTextEdit.Value, (int)rectangleYTextEdit.Value);
            helper.DiagramItems.Add(item);
        }

        static Point GetCenter(Rectangle rect) {
            return new Point(rect.Right / 2, rect.Height / 2);
        }

        private void timerStart(object sender, EventArgs e) {
            timer.Start();
        }

        private void makeRelation(object sender, EventArgs e) {
            try {
                DiagramRelation relation = new DiagramRelation();
                int id1 = Convert.ToInt32(item1IDTextEdit.Value);
                int id2 = Convert.ToInt32(item2IdTextEdit.Value);
                if (id1 < 0 || id2 < 0 || id1 == id2) return;
                relation.Item1 = helper.DiagramItems[id1];
                relation.Item2 = helper.DiagramItems[id2];
                helper.DiagramRelations.Add(relation);
            } catch { }
            
        }

        public void makeRandomRelation(object sender, EventArgs e) {
            if (helper.DiagramItems.Count < 2) return;
           int first = DiagramConstant.Random.Next(0, helper.DiagramItems.Count);
           int second = DiagramConstant.Random.Next(0, helper.DiagramItems.Count);
           while (first == second) {
               second = DiagramConstant.Random.Next(0, helper.DiagramItems.Count);
           }
           DiagramRelation relation = new DiagramRelation();
           relation.Item1 = helper.DiagramItems[first];
           relation.Item2 = helper.DiagramItems[second];
           helper.DiagramRelations.Add(relation);
           
        }

        private void timerStop(object sender, EventArgs e) {
            timer.Stop();
        }

        private void bestFit(object sender, EventArgs e) {
            helper.DoBestFit();
            
        }

        public void clear(object sender, EventArgs e) {
            helper = new DiagramHelper();
            
        }

        private void generateRandom(object sender, EventArgs e) {
            for (int i = 0; i < 15; i++) {
                AddItem(null, null);
            }
            for (int i = 0; i < 10; i++) {
                makeRandomRelation(null, null);
            }
        }

        private void save(object sender, EventArgs e) {
            SaveFileDialog dialog = new SaveFileDialog();
            DirectoryInfo di = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\..\..\Test\XMLStore\Failed");
            dialog.InitialDirectory = di.FullName;
            dialog.Filter = "XML Files (*.xml)|*.xml";
            dialog.DefaultExt = "*.xml";
            dialog.AddExtension = true;
            dialog.OverwritePrompt = true;
            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                helper.SerializeToXMLFile(dialog.FileName);
        }

        private void load(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            DirectoryInfo di = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\..\..\Test\XMLStore");
            dialog.InitialDirectory = di.FullName;
            dialog.DefaultExt = "*.xml";
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            helper = new DiagramHelper();
            helper.DeserializeFromXMLFile(dialog.FileName);
            
        }

        private void doArrange(object sender, EventArgs e) {
            helper.DoArrange();
        }

        private void goToItemID_Click(object sender, EventArgs e) {
            int id = (int)gotoItemIdSpinEdit.Value;
            if (id >= 0 && id < helper.DiagramItems.Count)
                diagramControl1.globalOffset = new Size((-(int)helper.DiagramItems[id].Location.X) + diagramControl1.Width / 2, (-(int)helper.DiagramItems[id].Location.Y) + diagramControl1.Height / 2);   
        }

        private void prepareForBestFit_Click(object sender, EventArgs e) {
            helper.PrepareForBestFit(diagramControl1.Size);
        }

        private void generateGraph(object sender, EventArgs e) {
            clear(null, null);
            GraphItem graphItem = new GraphItem(helper);
            graphItem.CreateRandom(10,4);
            helper.AddGraphItem(graphItem);
        }

        private void simpleButton10_Click(object sender, EventArgs e) {
            if(helper.DiagramItems.Count != 0 && helper.DiagramItems[0] is GraphItem) {
                foreach(var item in helper.DiagramItems) {
                    item.Location = Point.Empty;
                }
                int X = (helper.DiagramItems[0] as GraphItem).Size.Width;
                (helper.DiagramItems[0] as GraphItem).DoBestFit(ref X, (helper.DiagramItems[0] as GraphItem).Size.Height + 10);
            }
        }

        private void simpleButton16_Click(object sender, EventArgs e) {
            foreach(var item in helper.DiagramItems) {
                item.Location = new PointF(item.Location.Y, item.Location.X);
            }
        }
    }
}
