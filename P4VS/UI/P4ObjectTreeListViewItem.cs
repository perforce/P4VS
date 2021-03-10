using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perforce.P4VS
{
	public class P4ObjectTreeListViewItem : TreeListViewItem
	{
		public P4ObjectTreeListViewItem()
			: base()
		{
			NodeType = nodeType.None;
		}

		public P4ObjectTreeListViewItem(TreeListViewItem parentItem, string itemText, bool fullLine)
			: base(parentItem, itemText, fullLine)
		{
			NodeType = nodeType.None;
		}

		[Flags]
		public enum nodeTypeFlags : ushort
		{
			None = 0,

			// base node types
			Changelist = 0x1000,
			Job = 0x2000,
			ShelvedFile = 0x4000,
			File = 0x8000,

			// Bottom 8 bits (0x0001-0x00FF) can be reused for attributes of the
			// different base types

			#region Change List attributes

			
			Our = 0x0001,    // Who owns a changelist
			//Other = 0x0004,// vs our
			Pending = 0x0002,// vs submitted
			//Submitted = 0x0008,// vs pending

			// Changelist states
			UnderReview = 0x0010,
			NeedsResolve = 0x0020,
			HasShelved = 0x0040,
			Default = 0x0080,

			#endregion
		}

		public class nodeType
		{
			public nodeTypeFlags Flags { get; private set; }

			public nodeType(nodeTypeFlags v)
			{
				Flags = v;
			}

			public const nodeTypeFlags None = nodeTypeFlags.None;

			public const nodeTypeFlags Changelist = nodeTypeFlags.Changelist;
			public const nodeTypeFlags Job = nodeTypeFlags.Job;
			public const nodeTypeFlags ShelvedFile = nodeTypeFlags.ShelvedFile;
			public const nodeTypeFlags File = nodeTypeFlags.File;

			public const nodeTypeFlags Our = nodeTypeFlags.Our;
			public const nodeTypeFlags Pending = nodeTypeFlags.Pending;
			public const nodeTypeFlags OurPending = nodeTypeFlags.Our | nodeTypeFlags.Pending;
			public const nodeTypeFlags OtherPending = nodeTypeFlags.Pending;
			public const nodeTypeFlags NeedsResolve = nodeTypeFlags.NeedsResolve;
			public const nodeTypeFlags HasShelved = nodeTypeFlags.HasShelved;
			public const nodeTypeFlags UnderReview = nodeTypeFlags.UnderReview;
			public const nodeTypeFlags Default = nodeTypeFlags.Default;

			public const nodeTypeFlags OurDefault = nodeTypeFlags.Our | nodeTypeFlags.Pending | nodeTypeFlags.Default;
			public const nodeTypeFlags OtherDefault = nodeTypeFlags.Pending | nodeTypeFlags.Default;

			public const nodeTypeFlags PendingShelve = OurPending | HasShelved;
			public const nodeTypeFlags PendingResolveShelve = OurPending | NeedsResolve | HasShelved;
			public const nodeTypeFlags PendingReview = OurPending | UnderReview;
			public const nodeTypeFlags PendingShelveReview = OurPending | HasShelved | UnderReview;
			public const nodeTypeFlags PendingResolve = OurPending | NeedsResolve;
			
			public const nodeTypeFlags PendingOther = OtherPending;
			public const nodeTypeFlags PendingOtherShelve = OtherPending | HasShelved;
			public const nodeTypeFlags PendingOtherReview = OtherPending | UnderReview;
			public const nodeTypeFlags PendingOtherShelveReview = OtherPending | HasShelved | UnderReview;

			public static implicit operator nodeTypeFlags(nodeType v)
			{
				if (v == null)
				{
					return 0;
				}
				return v.Flags;
			}

			public static implicit operator int(nodeType v)
			{
				if (v == null)
				{
					return 0;
				}
				return (int)v.Flags;
			}

			public static bool operator ==(nodeType a, nodeType b)
			{
				return a.Equals(b);
			}
			public static bool operator ==(nodeType a, nodeTypeFlags b)
			{
				return a.Equals(b);
			}
			public static bool operator ==(nodeTypeFlags a, nodeType b)
			{
				return b.Equals(a);
			}

			public static bool operator !=(nodeType a, nodeType b) { return !a.Equals(b); }
			public static bool operator !=(nodeType a, nodeTypeFlags b) { return !a.Equals(b); }
			public static bool operator !=(nodeTypeFlags a, nodeType b) { return !b.Equals(a); }

			public override bool Equals(object obj)
			{
				if (obj is nodeType)
				{
					return ((nodeType)obj).Flags == this.Flags;
				}
				else if (obj is nodeTypeFlags)
				{
					return ((nodeTypeFlags)obj) == this.Flags;
				}
				return false;
			}
            
            public override int GetHashCode()
            {
                return Flags.GetHashCode();
            }

			public static implicit operator nodeType(nodeTypeFlags v)
			{
				return new nodeType(v);
			}

			public bool Test(nodeTypeFlags f)
			{
				return TestAny(f);
			}

			public bool TestAny(nodeTypeFlags f)
			{
				return ((Flags & f) != 0);
			}

			public bool TestAll(nodeTypeFlags f)
			{
				return ((Flags & f) == f);
			}

			public bool TestNone(nodeTypeFlags f)
			{
				return ((Flags & f) == 0);
			}

			public bool TestOnly(nodeTypeFlags f)
			{
				return ((Flags & f) == f);
			}

			public nodeTypeFlags Set(nodeTypeFlags f)
			{
				return Flags |= f;
			}

			public nodeTypeFlags Clear(nodeTypeFlags f)
			{
				return Flags &= ~f;
			}
		}

		public nodeType NodeType { get; set; }

		protected void AddSubitem(object value, int idx)
		{
			if (idx == 0)
			{
				if (value != null)
				{
					this.Text = value.ToString();
				}
				else
				{
					this.Text = string.Empty;
				}
			}
			else
			{
				if (value != null)
				{
					this.SubItems.Add(value.ToString());
				}
				else
				{
					this.SubItems.Add(string.Empty);
				}
			}
		}

		private const float kBytes = 1024;
		private const float mBytes = 1024 * kBytes;
		private const float gBytes = 1024 * mBytes;
		private const float tBytes = 1024 * gBytes;

		/// <summary>
		/// Pretty Print File Size
		/// </summary>
		/// <param name="fileSize"></param>
		/// <returns></returns>
		public static string PrettyPrintFileSize(long fileSize)
		{
			float fSize = fileSize;

			if (fileSize < 1024)
			{
				return string.Format(Resources.SccHistoryDetailsControl_FileSizeBytes, fileSize);
			}
			else if (fileSize < mBytes)
			{
				fSize = fSize / kBytes;
				return string.Format(Resources.SccHistoryDetailsControl_FileSizeKiloBytes, fSize);
			}
			else if (fileSize < gBytes)
			{
				fSize = fSize / mBytes;
				return string.Format(Resources.SccHistoryDetailsControl_FileSizeMegaBytes, fSize);
			}
			else if (fileSize < tBytes)
			{
				fSize = fSize / gBytes;
				return string.Format(Resources.SccHistoryDetailsControl_FileSizeGigaBytes, fSize);
			}
			fSize = fSize / tBytes;
			return string.Format(Resources.SccHistoryDetailsControl_FileSizeTeraBytes, fSize);
		}

	}
}
