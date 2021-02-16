using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using A2v10.Data.Extensions;
using A2v10.Workflow.Interfaces;
using A2v10.Workflow.SqlServer;
using A2v10.Workflow;
using A2v10.Workflow.Serialization;

using TestSite.Config;
using A2v10.System.Xaml;

namespace TestSite
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews();
			services.UseSimpleDbContext();
			if (FilesystemCatalog.IsConfigured(Configuration))
				services.AddSingleton<IWorkflowCatalog, FilesystemCatalog>();
			else
				services.AddSingleton<IWorkflowCatalog, SqlServerWorkflowCatalog>();
			services.AddSingleton<IWorkflowStorage, SqlServerWorkflowStorage>();
			services.AddSingleton<IInstanceStorage, SqlServerInstanceStorage>();
			services.AddSingleton<IScriptNativeObjectProvider, AppScriptNativeObjects>();
			services.AddSingleton<IXamlReaderService, XamlReaderService>();

			services.AddSingleton<ISerializer, Serializer>();

			services.AddScoped<IDeferredTarget, WorkflowDeferred>();
			services.AddScoped<ITracker, InstanceTracker>();
			services.AddScoped<IWorkflowEngine, WorkflowEngine>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
