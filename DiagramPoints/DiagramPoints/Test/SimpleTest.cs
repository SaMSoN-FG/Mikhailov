using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiagramPoints.Test {
    [TestFixture]
    public class SimpleTest {
        DiagramHelper helper;
        [SetUp]
        public void SetUp() {
            helper = new DiagramHelper();
        }
        [TearDown]
        public void TearDown() {
            helper = null;
        }

        [Test, Explicit]
        public void ShowForm() {
            using (Form1 form = new Form1()) {
                helper.DeserializeFromXMLFile(@"D:\Project\DiagramPoints\DiagramPoints\Test\XMLStore\LocationsNotChanged.xml");
                form.helper = helper;
                form.ShowDialog();
            }
        }
        [Test]
        public void TwoCrossTest() {
            helper.DeserializeFromXMLFile(@"D:\Project\DiagramPoints\DiagramPoints\Test\XMLStore\TwoCrossRelation.xml");
            for (int i = 0; i < 20; i++)
                helper.DoBestFit();
            Assert.AreEqual(PointF.Empty, DiagramHelper.IntersectLines(helper.DiagramRelations[0].Item1.Location, helper.DiagramRelations[0].Item2.Location, helper.DiagramRelations[1].Item1.Location, helper.DiagramRelations[1].Item2.Location));
        }
        [Test]
        public void ProcessConverges() {
            helper.DeserializeFromXMLFile(@"D:\Project\DiagramPoints\DiagramPoints\Test\XMLStore\ProcessConverges.xml");
            for (int i = 0; i < 40; i++)
                helper.DoBestFit();
            PointF rel1item1 = helper.DiagramRelations[0].Item1.Location;
            PointF rel1item2 = helper.DiagramRelations[0].Item2.Location;
            helper.DoBestFit();
            Assert.AreEqual(rel1item1, helper.DiagramRelations[0].Item1.Location);
            Assert.AreEqual(rel1item2, helper.DiagramRelations[0].Item2.Location);
        }
        [Test]
        public void LocationOverZero() {
            helper.DeserializeFromXMLFile(@"D:\Project\DiagramPoints\DiagramPoints\Test\XMLStore\LocationOverZero.xml");
            for (int i = 0; i < 100; i++)
                helper.DoBestFit();
            foreach (var item in helper.DiagramItems) {
                Assert.IsTrue(item.Location.X >= 0 && item.Location.Y >= 0);
            }
        }
        [Test]
        public void ItemsDiverge() {
            helper.DeserializeFromXMLFile(@"D:\Project\DiagramPoints\DiagramPoints\Test\XMLStore\ItemsDiverge.xml");
            double initialDistance = DiagramHelper.GetDistanceBetweenPoints(helper.DiagramItems[0].Location, helper.DiagramItems[1].Location);
            for (int i = 0; i < 2; i++)
                helper.DoBestFit();
            double finalDistance = DiagramHelper.GetDistanceBetweenPoints(helper.DiagramItems[0].Location, helper.DiagramItems[1].Location);
            Assert.IsTrue(initialDistance < finalDistance);
        }
        [Test]
        public void IntersectionWithTwoRelatons() {
            helper.DeserializeFromXMLFile(@"D:\Project\DiagramPoints\DiagramPoints\Test\XMLStore\IntersectionWithTwoRelatons.xml");
            for (int i = 0; i < 20; i++)
                helper.DoBestFit();
            for (int i = 0; i <= 2; i++)
                for (int j = i + 1; j <= 2; j++)
                    Assert.AreEqual(PointF.Empty, DiagramHelper.IntersectLines(helper.DiagramRelations[i].Item1.Location, helper.DiagramRelations[i].Item2.Location, helper.DiagramRelations[j].Item1.Location, helper.DiagramRelations[j].Item2.Location));

        }
        [Test]
        public void LocationsNotChanged() {
            helper.DeserializeFromXMLFile(@"D:\Project\DiagramPoints\DiagramPoints\Test\XMLStore\LocationsNotChanged.xml");
            for (int i = 0; i < 100; i++)
                helper.DoBestFit();
            PointF[] initialLocations = new PointF[helper.DiagramItems.Count];
            int j = 0;
            foreach (var item in helper.DiagramItems) {
                initialLocations[j] = item.Location;
                j++;
            }
            helper.DoBestFit();
            j = 0;
            foreach (var item in helper.DiagramItems) {
                Assert.AreEqual(initialLocations[j], item.Location);
                j++;
            }
        }
        [Test, Explicit]
        public void MainTest() {
            using (Form1 form = new Form1()) {
                string[] files = Directory.GetFiles(@"D:\Project\Mikhailov\DiagramPoints\DiagramPoints\Test\XMLStore\Failed");
                for (int testId = 0, failedId = files.Count(); testId < 1000; testId++) {
                    InitializeDiagramItemsAndRelations(form);
                    helper = form.helper;
                    string file = @"D:\Project\Mikhailov\DiagramPoints\DiagramPoints\Test\XMLStore\Failed\Failed_" + failedId.ToString() + ".xml";
                    helper.SerializeToXMLFile(file);
                    for (int i = 0; i < 100; i++)
                        helper.DoBestFit();
                    bool isFailed = IsWrongBestFit(ref failedId);
                    if (!isFailed) File.Delete(file);
                    form.clear(null, null);
                }
            }
        }

        private bool IsWrongBestFit(ref int failedId) {
            foreach (var relation1 in helper.DiagramRelations) {
                foreach (var relation2 in helper.DiagramRelations) {
                    if (DiagramHelper.IntersectLines(relation1.Item1.Location, relation1.Item2.Location, relation2.Item1.Location, relation2.Item2.Location) != PointF.Empty) {
                        failedId++;
                        return true;
                    }
                }
            }
            return false;
        }

        private static void InitializeDiagramItemsAndRelations(Form1 form) {
            for (int i = 0; i < 15; i++)
                form.AddItem(null, null);

            for (int i = 0; i < 10; i++)
                form.makeRandomRelation(null, null);
        }
        [Test]
        public void RunFailedTest() {
           string[] files = Directory.GetFiles(@"D:\Project\Mikhailov\DiagramPoints\DiagramPoints\Test\XMLStore\Failed");
           foreach (string file in files) {
               helper = new DiagramHelper();
               helper.DeserializeFromXMLFile(file);
               for (int i = 0; i < 1000; i++)
                   helper.DoBestFit();
               PointF[] initialLocations = new PointF[helper.DiagramItems.Count];
               int j = 0;
               foreach (var item in helper.DiagramItems) {
                   initialLocations[j] = item.Location;
                   j++;
               }
               helper.DoBestFit();
               j = 0;
               foreach (var item in helper.DiagramItems) {
                   Assert.AreEqual(initialLocations[j], item.Location,file);
                   j++;
               }
           }
        }
        [Test]
        public void SnapToNearestPoint() {
            helper.DeserializeFromXMLFile(@"D:\Project\DiagramPoints\DiagramPoints\Test\XMLStore\SnapToNearestPoint.xml");
            helper.DoArrange();
            Assert.IsTrue(helper.DiagramItems[0].Location == new PointF(0, 0));
            Assert.IsTrue(helper.DiagramItems[1].Location == new PointF(150, 0));
            Assert.IsTrue(helper.DiagramItems[2].Location == new PointF(150, 100));
            Assert.IsTrue(helper.DiagramItems[3].Location == new PointF(0, 100));
        }
        [Test]
        public void InNodeSinglePoint() {
            helper.DeserializeFromXMLFile(@"D:\Project\DiagramPoints\DiagramPoints\Test\XMLStore\InNodeSinglePoint.xml");
            for (int i = 0; i < 20; i++)
                helper.DoBestFit();
            helper.DoArrange();
            Assert.IsFalse(helper.DiagramItems[0].Location == new PointF(150, 100) && helper.DiagramItems[1].Location == new PointF(150, 100));
        }
    }
}
