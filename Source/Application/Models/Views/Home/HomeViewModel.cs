using System;
using System.Collections.Generic;
using System.IO;

namespace Application.Models.Views.Home
{
	public class HomeViewModel
	{
		#region Properties

		public virtual DirectoryInfo ApplicationRoot { get; set; }
		public virtual IList<FileSystemInfo> Children { get; } = new List<FileSystemInfo>();
		public virtual string Content { get; set; }
		public virtual Exception Exception { get; set; }
		public virtual DirectoryInfo Parent { get; set; }
		public virtual string Path { get; set; }
		public virtual DirectoryInfo Root { get; set; }

		#endregion
	}
}