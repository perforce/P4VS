using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CenterImages = Perforce.P4VS.P4ObjectTreeListView.CenterImages;
using LeftImages = Perforce.P4VS.P4ObjectTreeListView.LeftImages;
using RightImages = Perforce.P4VS.P4ObjectTreeListView.RightImages;

namespace Perforce.P4VS
{
	public class P4JobTreeListViewItem : P4ObjectTreeListViewItem
	{
        private P4.Job _jobData = null;
        public P4.Job JobData
        {
            get { return _jobData; }
            set
            {
                _jobData = value;

                InitSubitems(Fields);
                SelectImagesFromMetaData();
            }
        }

        private string _jobId = null;
        public string JobId
        {
            get { return _jobId; }
            set
            {
                _jobId = value;

                InitSubitems(Fields);
                SelectImagesFromMetaData();
            }
        }

		IList<object> Fields = null;

		//public string FileName { get; set; }
		//public string FileFolder { get; set; }

        public P4JobTreeListViewItem(TreeListViewItem parentItem, P4.Job jobData, params object[] fields)
            : this(parentItem, jobData, fields.ToList<object>())
        {
        }

		public P4JobTreeListViewItem(TreeListViewItem parentItem, P4.Job jobData, IList<object> fields)
			:base()
		{
            _jobId = null;
			ParentItem = parentItem;
			Fields = fields;
			JobData = jobData;

			Tag = jobData;

			//FileName = fileData.LocalPath.GetFileName();
			//FileFolder = fileData.LocalPath.GetFileName();
		}

		public P4JobTreeListViewItem(TreeListViewItem parentItem, string jobId)
			:base()
		{
            _jobData = null; 
			ParentItem = parentItem;
			Fields = null;
			_jobId = jobId;
            Tag = jobId;

			ImageIndex = (int)CenterImages.Job;
            AddSubitem(jobId, 0);

            this.FullLine = true;
        }

		private void SelectImagesFromMetaData()
		{
			if (JobData == null)
			{
				return;
			}

			ImageIndex = (int)CenterImages.Job;
		}

		public enum SubItemFlag
		{
			None = 0,
			Id,
			Description
		}

		class JobField  
		{
			string Key { get; set; }
			public JobField(string key)
			{
				Key = key;
			}
			public static implicit operator string(JobField jf) { return jf.Key; }
			public static implicit operator JobField(string key) { return new JobField(key); }
		};

		private void InitSubitems(IList<object> fields)
		{
			this.SubItems.Clear();
			for (int idx = 0; idx < fields.Count; idx++)
			{
				object value = null;
				object field = fields[idx];
				if (field is SubItemFlag)
				{
					SubItemFlag flag = (SubItemFlag)field;
					switch (flag)
					{
						case SubItemFlag.None:
							value = string.Empty;
							break;
						case SubItemFlag.Id:
							value = JobData.Id;
							break;
						case SubItemFlag.Description:
							value = string.Empty;
							if (JobData.ContainsKey("Description"))
							{
								value = JobData["Description"];
							}
							break;
					}
				}
				if (field is JobField)
				{
					JobField jf = field as JobField;

					value = string.Empty;
					if ((jf != null) && (JobData.ContainsKey(jf)))
					{
						value = JobData[jf];
					}
				}
				else
				{
					value = field;
				}
				AddSubitem(value, idx);
			}
		}
	}
}
