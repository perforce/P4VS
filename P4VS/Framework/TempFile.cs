using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Perforce.P4VS
{
	public class FileReference : IDisposable
	{
		protected  string _path;// { get; private set; }

		public FileReference()
		{
			_path = null;
		}

		public FileReference(string Path)
		{
			_path = Path;
		}

		public static implicit operator string(FileReference t)
		{
			return t._path;
		}

		public override string ToString()
		{
			return _path;
		}
		#region IDisposable Members

		public virtual void Dispose()
		{
		}
		#endregion
	}

    /// <summary>
    /// Class to wrap a temporary file that is deleted when the object is disposed
    /// </summary>
    public class TempFile : FileReference
	{
		public TempFile()
		{
			string tempPath = Path.GetTempPath();

			int n = 1;

			string tempFileName = string.Format("vstmp({0}).tmp", n);

			_path = Path.Combine(tempPath, tempFileName);

			while (System.IO.File.Exists(_path))
			{
				tempFileName = string.Format("vstmp({0}).tmp", ++n);

				_path = Path.Combine(tempPath, tempFileName);
			}
		}

		public TempFile(string FileName)
		{
			string tempPath = Path.GetTempPath();

			string baseName = null;
			string extension = null;
 
			if (FileName[0] == '.')
			{
				// only supplied the extension
				baseName = "vstmp";
				extension = FileName;
			}
			else
			{
				baseName = Path.GetFileNameWithoutExtension(FileName);
				extension = Path.GetExtension(FileName);
			}

			string tempFileName = string.Format("{0}{1}", baseName,extension);

			_path = Path.Combine(tempPath, tempFileName);

			int n = 0;

			while (System.IO.File.Exists(_path))
			{
				tempFileName = string.Format("{0}({1}){2}", baseName, ++n, extension);

				_path = Path.Combine(tempPath, tempFileName);
			}
		}

        public TempFile(P4.FileSpec File)
		{
			string tempPath = Path.GetTempPath();

			string fileName = null;
			if (File.LocalPath != null)
			{
				fileName = File.LocalPath.GetFileName();
			}
			if (string.IsNullOrEmpty(fileName) && (File.DepotPath != null))
			{
				fileName = File.DepotPath.GetFileName();
			}
			if (string.IsNullOrEmpty(fileName) && (File.ClientPath != null))
			{
				fileName = File.ClientPath.GetFileName();
			}
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentOutOfRangeException("File", "File Specification does not contain a valid file name");
			}
			string baseName = Path.GetFileNameWithoutExtension(fileName);
			string extension = Path.GetExtension(fileName);
			string version = null;
			if (File.Version is P4.VersionRange)
			{
				// Resolve record always use ranges, even though we're only interested in the
				// final number
				version = ((P4.VersionRange)File.Version).Upper.ToString();
			}
			else
			{
				version = File.Version.ToString();
			}
			fileName = string.Format("{0}{1}{2}", baseName, version, extension);

			_path = Path.Combine(tempPath, fileName);

			int n = 1;
			while (System.IO.File.Exists(_path))
			{
				fileName = string.Format("{0}{1}({3}){2}", baseName, version, extension, ++n);

				_path = Path.Combine(tempPath, fileName);
			}
		}

		public TempFile(P4.PathSpec File, P4.VersionSpec version)
		{
			string tempPath = Path.GetTempPath();

			string fileName = null;
			if (File != null)
			{
				fileName = File.GetFileName();
			}
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentOutOfRangeException("File", "File Specification does not contain a valid file name");
			}
			string baseName = Path.GetFileNameWithoutExtension(fileName);
			string extension = Path.GetExtension(fileName);
			string versionStr = version.ToString();

			fileName = string.Format("{0}{1}{2}", baseName, versionStr, extension);

			_path = Path.Combine(tempPath, fileName);

			int n = 1;
			while (System.IO.File.Exists(_path))
			{
				fileName = string.Format("{0}{1}({3}){2}", baseName, versionStr, extension, ++n);

				_path = Path.Combine(tempPath, fileName);
			}
		}
 
		public static implicit operator string(TempFile t)
		{
			return t._path;
		}

		public override string ToString()
		{
			return _path;
		}

        public bool ReadOnly
        {
            get
            {
                if (File.Exists(_path)==false)
                {
                    throw new IOException("File doesot xist");
                }
                FileInfo fi = new FileInfo(_path);
                return fi.IsReadOnly;
            }
            set
            {
                if (File.Exists(_path)==false)
                {
                    throw new IOException("File doesot xist");
                }
                FileInfo fi = new FileInfo(_path);
                fi.IsReadOnly = value;
            }
        }

		#region IDisposable Members

		public override void Dispose()
		{
			if (File.Exists(_path))
			{
                File.SetAttributes(_path, System.IO.FileAttributes.Archive);
				File.Delete(_path);
			}
		}
		#endregion
	}
}
