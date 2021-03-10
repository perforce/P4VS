using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perforce.P4Scm
{
	// This class defines basic source control status values
	[Flags]
	public enum SourceControlStatusFlags : uint
	{
		scsNone = 0x00000000,

		scsStale = 0x00000001,
		scsOtherCheckedOut = 0x00000002,
		scsCheckedOut = 0x00000004,
		scsLockedSelf = 0x00000008,

		scsLockedOther = 0x00000010,
		scsNeedsResolve = 0x00000020,
		scsMarkedAdd = 0x00000040,
		scsMarkedDelete = 0x00000080,

		scsBranched = 0x00000100,
		scsNotOnDisk = 0x00000200,
		scsMoved = 0x00000400,
		scsDeletedAtHead = 0x00000800,

		scsIntegrated = 0x00001000,
		scsIgnored = 0x00002000,
		scsCheckedIn = 0x00004000,
		scsUncontrolled = 0x00008000,

		scsUnknown = 0x00010000, // File status has not been loaded dou to Lazy load

		scsLocked = scsLockedSelf | scsLockedOther,
		scsOutMultiple = scsOtherCheckedOut | scsCheckedOut
	};


	public class SourceControlStatus
	{
		public SourceControlStatusFlags Flags { get; private set; }

		public SourceControlStatus(SourceControlStatusFlags v)
		{
			Flags = v;
		}

		public SourceControlStatus(P4.FileMetaData f)
		{
			Flags = SourceControlStatusFlags.scsUncontrolled;

			try
			{
				if (f != null)
				{
					if (f.HeadAction == P4.FileAction.Delete)
					{
						Flags = SourceControlStatusFlags.scsDeletedAtHead;
						if (f.Action == P4.FileAction.Add)
						{
							Flags |= SourceControlStatusFlags.scsMarkedAdd | SourceControlStatusFlags.scsCheckedOut;
						}
					}
					else if (f.Action == P4.FileAction.Add)
					{
						Flags = SourceControlStatusFlags.scsMarkedAdd | SourceControlStatusFlags.scsCheckedOut;
					}
					else if (f.Action == P4.FileAction.Branch)
					{
						Flags = SourceControlStatusFlags.scsBranched | SourceControlStatusFlags.scsCheckedOut;
					}
					else if (f.Action == P4.FileAction.Delete)
					{
						Flags = SourceControlStatusFlags.scsMarkedDelete | SourceControlStatusFlags.scsCheckedOut;
					}
					else if (f.Action == P4.FileAction.MoveAdd)
					{
						Flags = SourceControlStatusFlags.scsMoved;
					}
					else if (f.IsInDepot)
					{
						Flags = SourceControlStatusFlags.scsCheckedIn;
					}
					if (f.Action == P4.FileAction.Edit)
					{
						Flags = SourceControlStatusFlags.scsCheckedOut;
					}
					if (f.OurLock)
					{
						Flags |= SourceControlStatusFlags.scsLockedSelf;
					}
					if (f.OtherOpen > 0)
					{
						for (int idx = 0; idx < f.OtherOpen; idx++)
						{
							if (f.OtherActions[idx] == P4.FileAction.Edit || f.OtherActions[idx] == P4.FileAction.Integrate)
							{
								Flags |= SourceControlStatusFlags.scsOtherCheckedOut;
							}
							if (f.OtherActions[idx] == P4.FileAction.Delete)
							{
								Flags |= SourceControlStatusFlags.scsMarkedDelete | SourceControlStatus.scsOtherCheckedOut;
							}
						}
					}

					if (f.OtherLock)
					{
						Flags |= SourceControlStatusFlags.scsLockedOther;
					}

					if (f.IsStale)
					{
						Flags |= SourceControlStatusFlags.scsStale;
					}
					if (f.Unresolved)
					{
						Flags |= SourceControlStatusFlags.scsNeedsResolve;
					}
					if (f.Action == P4.FileAction.Integrate)
					{
						Flags |= SourceControlStatusFlags.scsIntegrated;
					}
					if ((f.HaveRev == 0) && (f.Action != P4.FileAction.Add) && (f.Action != P4.FileAction.MoveAdd) && (f.Action != P4.FileAction.Branch))
					{
						Flags |= SourceControlStatusFlags.scsNotOnDisk;
					}
				}
			}
			catch (Exception)
			{
				Flags = SourceControlStatusFlags.scsUncontrolled;
			}
		}

		public const SourceControlStatusFlags scsCheckedIn = SourceControlStatusFlags.scsCheckedIn;
		public const SourceControlStatusFlags scsStale = SourceControlStatusFlags.scsStale;
		public const SourceControlStatusFlags scsOtherCheckedOut = SourceControlStatusFlags.scsOtherCheckedOut;
		public const SourceControlStatusFlags scsCheckedOut = SourceControlStatusFlags.scsCheckedOut;
		public const SourceControlStatusFlags scsLockedSelf = SourceControlStatusFlags.scsLockedSelf;
		public const SourceControlStatusFlags scsLockedOther = SourceControlStatusFlags.scsLockedOther;
		public const SourceControlStatusFlags scsNeedsResolve = SourceControlStatusFlags.scsNeedsResolve;
		public const SourceControlStatusFlags scsMarkedAdd = SourceControlStatusFlags.scsMarkedAdd;
		public const SourceControlStatusFlags scsMarkedDelete = SourceControlStatusFlags.scsMarkedDelete;
		public const SourceControlStatusFlags scsBranched = SourceControlStatusFlags.scsBranched;
		public const SourceControlStatusFlags scsNotOnDisk = SourceControlStatusFlags.scsNotOnDisk;
		public const SourceControlStatusFlags scsMoved = SourceControlStatusFlags.scsMoved;
		public const SourceControlStatusFlags scsDeletedAtHead = SourceControlStatusFlags.scsDeletedAtHead;
		public const SourceControlStatusFlags scsIntegrated = SourceControlStatusFlags.scsIntegrated;

		public const SourceControlStatusFlags scsIgnored = SourceControlStatusFlags.scsIgnored;
		public const SourceControlStatusFlags scsUncontrolled = SourceControlStatusFlags.scsUncontrolled;

		public const SourceControlStatusFlags scsUnknown = SourceControlStatusFlags.scsUnknown;

		public const SourceControlStatusFlags scsLocked = scsLockedSelf | scsLockedOther;
		public const SourceControlStatusFlags scsOutMultiple = scsOtherCheckedOut | scsCheckedOut;

		public static implicit operator SourceControlStatusFlags(SourceControlStatus v)
		{
			if (v == null)
			{
				return 0;
			}
			return v.Flags;
		}

		public static implicit operator int(SourceControlStatus v)
		{
			if (v == null)
			{
				return 0;
			}
			return (int)v.Flags;
		}

		public static bool operator ==(SourceControlStatus a, SourceControlStatus b) 
		{
			return a.Equals(b); 
		}
		public static bool operator ==(SourceControlStatus a, SourceControlStatusFlags b) 
		{
			return a.Equals(b);
		}
		public static bool operator ==(SourceControlStatusFlags a, SourceControlStatus b) 
		{
			return b.Equals(a);
		}

		public static bool operator !=(SourceControlStatus a, SourceControlStatus b) { return !a.Equals(b); }
		public static bool operator !=(SourceControlStatus a, SourceControlStatusFlags b) { return !a.Equals(b); }
		public static bool operator !=(SourceControlStatusFlags a, SourceControlStatus b) { return !b.Equals(a); }

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is SourceControlStatus)
			{
				return ((SourceControlStatus)obj).Flags == this.Flags;
			}
			else if (obj is SourceControlStatusFlags)
			{
				return ((SourceControlStatusFlags)obj) == this.Flags;
			}
			return false;
		}

        public override int GetHashCode()
        {
            return Flags.GetHashCode();
        }

		public static implicit operator SourceControlStatus(SourceControlStatusFlags v)
		{
			return new SourceControlStatus(v);
		}

		public bool Test(SourceControlStatusFlags f)
		{
			return TestAny(f);
		}

		public bool TestAny(SourceControlStatusFlags f)
		{
			return ((Flags & f) != 0);
		}

		public bool TestAll(SourceControlStatusFlags f)
		{
			return ((Flags & f) == f);
		}

		public bool TestNone(SourceControlStatusFlags f)
		{
			return ((Flags & f) == 0);
		}

		public bool TestOnly(SourceControlStatusFlags f)
		{
			return ((Flags & ~f) == 0);
		}

		public SourceControlStatusFlags Set(SourceControlStatusFlags f)
		{
			return Flags |= f;
		}

		public SourceControlStatusFlags Clear(SourceControlStatusFlags f)
		{
			return Flags &= ~f;
		}
	}
}
