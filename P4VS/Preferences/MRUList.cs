using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perforce.P4VS
{
	public class MRUList : IList<object>, IPreference//, IEnumerable
	{
		public MRUList() 
		{ 
			_maxCapacity = 5;
			items = new object[_maxCapacity];
		}

		private int _maxCapacity = 5;
		private object[] items;

		public MRUList(int MaxItems)
		{
			_maxCapacity = MaxItems;

			items = new object[_maxCapacity];
		}

		public MRUList(MRUList mru)
		{
			_maxCapacity = mru._maxCapacity;

			items = new object[_maxCapacity];
			for (int idx = 0; idx < _maxCapacity; idx++)
			{
				items[idx] = mru.items[idx];
			}
		}

		public static MRUList Clone(MRUList mru)
		{
			return new MRUList(mru);
		}

		public MRUList Clone()
		{
			return new MRUList(this);
		}

		#region IList<object> Members

		public int IndexOf(object item)
		{
			if ((item == null) || (items == null))
			{
				return -1; ;
			}
			for (int idx = 0; idx < _maxCapacity; idx++)
			{
				if (items[idx] == null)
				{
					continue;
				}
				else if (item.GetType() != items[idx].GetType())
				{
					continue;
				}
				else if (item.GetType() == typeof(int))
				{
					if ((int)item == (int)items[idx])
					{
						return idx;
					}
				}
				else if (item.GetType() == typeof(string))
				{
					if ((string)item == (string)items[idx])
					{
						return idx;
					}
				}
				else if (item.GetType() == typeof(bool))
				{
					if ((bool)item == (bool)items[idx])
					{
						return idx;
					}
				}
				else if (items[idx].Equals(item))
				{
					return idx;
				}
			}
			return -1;
		}

		public void Insert(int index, object item)
		{
			for (int idx = _maxCapacity -1; idx > index; idx--)
			{
				items[idx] = items[idx - 1];
			}
			items[index] = item;
		}

		public void RemoveAt(int index)
		{
			for (int idx = index; idx < _maxCapacity - 1; idx++)
			{
				items[idx] = items[idx + 1];
			}
			items[_maxCapacity - 1] = null;
		}

		public object this[int index]
		{
			get
			{
				return items[index];
			}
			set
			{
				items[index] = value;
			}
		}

		#endregion

		#region ICollection<object> Members

		public void Add(object item)
		{
			// if already in the list (but not at the top),
			// remove it from the list
			int idx = this.IndexOf(item);
			if (idx > 0)
			{
				this.RemoveAt(idx);
			}
			// add at the top of the list only if object is 
			// not already at the top of the list
			if (idx != 0)
			{
				this.Insert(0, item);
			}
		}

		public void Clear()
		{
			for (int idx = 0; idx < _maxCapacity; idx++)
			{
				items[idx] = null;
			}
		}

		public bool Contains(object item)
		{
			return this.IndexOf(item) >= 0;
		}

		public int Capacity
		{
			get { return _maxCapacity; }
			private set 
			{ 
				_maxCapacity = value;
			}
		}

		public bool Remove(object item)
		{
			int idx = this.IndexOf(item);
			if (idx >= 0)
			{
				this.RemoveAt(idx);
				return true;
			}
			return false;
		}

		public void CopyTo(object[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { return Capacity; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return (IEnumerator)items.GetEnumerator();
		}

		#endregion

		#region IPreference Members

		public void WriteTag(StringBuilder sb, ref int tabLevel, string tag)
		{
			string t = string.Format("{0} maxCapacity=\"{1}\"", tag, _maxCapacity);
			PreferenceCache.StartBlock(sb, ref tabLevel, t, GetType().FullName);
			for (int idx = 0; idx < _maxCapacity; idx++)
			{
				PreferenceCache.SavePreference(sb, ref tabLevel, tag, items[idx]);
			}
			PreferenceCache.EndBlock(sb, ref tabLevel, tag);
		}

		public IPreference FromXmlNode(System.Xml.XmlNode node)
		{
			if (node.Attributes != null && node.Attributes["maxCapacity"] != null)
			{
				int v;
				int.TryParse(node.Attributes["maxCapacity"].InnerText, out v);
				Capacity = v;
			}
			else
			{
				Capacity = node.ChildNodes.Count;
			}
			items = new object[Capacity];

			for (int idx = 0; idx < node.ChildNodes.Count; idx++)
			{
				items[idx] = (object)PreferenceCache.LoadPreference(node.ChildNodes[idx]);
			}
			return this;
		}

		#endregion

		#region IEnumerable<object> Members

		IEnumerator<object> IEnumerable<object>.GetEnumerator()
		{
			return (IEnumerator<object>)items.GetEnumerator(); ;
		}

		#endregion
	}
}
