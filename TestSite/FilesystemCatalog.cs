using A2v10.Workflow.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSite
{
	public class FilesystemCatalog : IWorkflowCatalog
	{
		private const string ConfigPathKey = "fsCatalogPath";

		public static bool IsConfigured(IConfiguration configuration)
		{
			return !String.IsNullOrEmpty(configuration.GetValue<string>(ConfigPathKey));
		}

		public FilesystemCatalog(IConfiguration configuration)
		{
			if (!IsConfigured(configuration)) throw new ArgumentException("Provided configuration does not configure FilesystemCatalog", nameof(configuration));
			CatalogRoot = configuration.GetValue<string>(ConfigPathKey);
		}

		public readonly string CatalogRoot;

		private string FilePath(string wfid)
		{
			return Path.Combine(CatalogRoot, wfid + ".bpmn");
		}

		public IEnumerable<(string name, DateTime createdTime)> GetWorkflowsList()
		{
			return Directory.GetFiles(CatalogRoot).Select(f => new FileInfo(f)).Where(fi => fi.Extension == ".bpmn").Select(fi => (fi.Name[..^5], fi.CreationTime));
		}

		public async Task<WorkflowElem> LoadBodyAsync(string id)
		{
			var fp = FilePath(id);
			var txt = await File.ReadAllTextAsync(fp, Encoding.UTF8);
			return new WorkflowElem()
			{
				Body = txt,
				Format = "text/xml"
			};
		}

		public Task<WorkflowThumbElem> LoadThumbAsync(string id)
		{
			throw new NotImplementedException();
		}

		public Task SaveAsync(IWorkflowDescriptor workflow)
		{
			if (workflow.Format != "text/xml") throw new NotImplementedException();
			var fp = FilePath(workflow.Id);
			return File.WriteAllTextAsync(fp, workflow.Body, Encoding.UTF8);
		}
	}
}
