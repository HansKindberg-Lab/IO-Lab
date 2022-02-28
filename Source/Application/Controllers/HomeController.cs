using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.Models.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Controllers
{
	public class HomeController : Controller
	{
		#region Constructors

		public HomeController(IHostEnvironment hostEnvironment, ILoggerFactory loggerFactory)
		{
			this.HostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
			this.Logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
		}

		#endregion

		#region Properties

		protected internal virtual IHostEnvironment HostEnvironment { get; }
		protected internal virtual ILogger Logger { get; }

		#endregion

		#region Methods

		[SuppressMessage("Design", "CA1031:Do not catch general exception types")]
		[SuppressMessage("Security", "CA3003:Review code for file path injection vulnerabilities")]
		public virtual async Task<IActionResult> Index(string path)
		{
			var model = new HomeViewModel
			{
				ApplicationRoot = new DirectoryInfo(this.HostEnvironment.ContentRootPath),
				Path = path,
				Root = new DirectoryInfo(Directory.GetDirectoryRoot(this.HostEnvironment.ContentRootPath))
			};

			// ReSharper disable InvertIf
			if(!string.IsNullOrWhiteSpace(path))
			{
				try
				{
					FileSystemInfo fileSystemInfo = System.IO.File.GetAttributes(path).HasFlag(FileAttributes.Directory) ? new DirectoryInfo(path) : new FileInfo(path);

					if(fileSystemInfo.Exists)
					{
						if(fileSystemInfo is DirectoryInfo directoryInfo)
						{
							model.Parent = directoryInfo.Parent;

							foreach(var child in directoryInfo.GetFileSystemInfos().OrderBy(item => !item.Attributes.HasFlag(FileAttributes.Directory)).ThenBy(item => item.Name))
							{
								model.Children.Add(child);
							}
						}
						else
						{
							model.Parent = (fileSystemInfo as FileInfo)?.Directory;

							model.Content = await System.IO.File.ReadAllTextAsync(path);
						}
					}
					else
					{
						model.Exception = new InvalidOperationException($"Path \"{path}\" not found.");
					}
				}
				catch(Exception exception)
				{
					model.Exception = exception;
				}
			}
			// ReSharper restore InvertIf

			return this.View(model);
		}

		#endregion
	}
}