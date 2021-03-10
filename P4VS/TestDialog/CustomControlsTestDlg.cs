using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Perforce.P4VS.TestDialog
{
    public partial class CustomControlsTestDlg : Form
    {
        public CustomControlsTestDlg()
        {
            InitializeComponent();

            #region MultiselectTreeListViewTab init
            FolderItem F1 = new FolderItem("Stuff");
            treeListView1.Nodes.Add(F1);

            FileItem F1f1 = new FileItem(F1, "Alpha", 1234, "This is the first file");
            F1.ChildNodes.Add(F1f1);
            FileItem F1f2 = new FileItem(F1, "Beta", 1234, "This is the second file");
            F1.ChildNodes.Add(F1f2);
            FileItem F1f3 = new FileItem(F1, "Gama", 1234, "This is the third file");
            F1.ChildNodes.Add(F1f3);
            FileItem F1f4 = new FileItem(F1, "Omega", 1234, "This is the last file");
            F1.ChildNodes.Add(F1f4);

            LabelItem F1l1 = new LabelItem("This is a label");
            F1.ChildNodes.Add(F1l1);
            LabelItem F1l2 = new LabelItem("This is also a label");
            F1.ChildNodes.Add(F1l2);

            FolderItem F2 = new FolderItem("More Stuff");

            treeListView1.Nodes.Add(F2);

            FileItem F2f1 = new FileItem(F2, "Alice", 1234, "This is the sender");
            F2.ChildNodes.Add(F2f1);
            FileItem F2f2 = new FileItem(F2, "Bob", 1234, "This is the receiver");
            F2.ChildNodes.Add(F2f2);
            FileItem F2f3 = new FileItem(F2, "Carol", 1234, "This is woman in the middle");
            F2.ChildNodes.Add(F2f3);
            FileItem F2f4 = new FileItem(F2, "Don", 1234, "This is the man in the middle");
            F2.ChildNodes.Add(F2f4);

            LabelItem F2l1 = new LabelItem("Red");
            F2.ChildNodes.Add(F2l1);
            LabelItem F2l2 = new LabelItem("Green");
            F2.ChildNodes.Add(F2l2);

            FolderItem F3 = new FolderItem("Even More Stuff");
            treeListView1.Nodes.Add(F3);

            FileItem F3f1 = new FileItem(F3, "Chevy", 1234, "Vega");
            F3.ChildNodes.Add(F3f1);
            FileItem F3f2 = new FileItem(F3, "Ford", 1234, "Falcon");
            F3.ChildNodes.Add(F3f2);
            FileItem F3f3 = new FileItem(F3, "Dodge", 1234, "Corenet");
            F3.ChildNodes.Add(F3f3);
            FileItem F3f4 = new FileItem(F3, "AMC", 1234, "Gremlin");
            F3.ChildNodes.Add(F3f4);

            LabelItem F3l1 = new LabelItem("Rusted");
            F3.ChildNodes.Add(F3l1);
            LabelItem F3l2 = new LabelItem("Busted");
            F3.ChildNodes.Add(F3l2);

            treeListView1.BuildTreeList();
            #endregion
        }
        
        private void SetSelectionConditions()
        {
            TreeListView.MultiSelectCondition selectWhen = TreeListView.MultiSelectCondition.none;

            if (SameParentCB.Checked)
            {
                selectWhen |= TreeListView.MultiSelectCondition.SameParent;
            }
            if (SameLevelCB.Checked)
            {
                selectWhen |= TreeListView.MultiSelectCondition.SameLevel;
            }
            if (SameTypeCB.Checked)
            {
                selectWhen |= TreeListView.MultiSelectCondition.SameClass;
            }
            treeListView1.MultiSelectConditions = selectWhen;
        }

        private void SameParentCB_CheckedChanged(object sender, EventArgs e)
        {
            SetSelectionConditions();
        }

        private void SameLevelCB_CheckedChanged(object sender, EventArgs e)
        {
            SetSelectionConditions();
        }

        private void SameTypeCB_CheckedChanged(object sender, EventArgs e)
        {
            SetSelectionConditions();
        }
    }

    public class FolderItem : Perforce.P4VS.TreeListViewItem
    {
        public FolderItem(string name)
            : base(null, name, true)
        {
            ImageIndex = 0;
        }
    }

    public class FileItem : Perforce.P4VS.TreeListViewItem
    {
        public FileItem(FolderItem parent, string name, int size, string comment)
            : base(parent, new string[] { name, size.ToString(), comment }, 1)
        {
        }
    }

    public class LabelItem : Perforce.P4VS.TreeListViewItem
    {
        public LabelItem(string name)
            : base(null, name, true)
        {
            ImageIndex = 2;
        }
    }
}
