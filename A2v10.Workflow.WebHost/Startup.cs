using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using A2v10.Data.Extensions;
using A2v10.Workflow.Interfaces;
using A2v10.Workflow.SqlServer;
using A2v10.Workflow.Serialization;
using A2v10.Workflow.Interfaces.Api;

namespace A2v10.Workflow.WebHost
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers(SetControllerOptions).AddNewtonsoftJson(ConfigureNewtonsoft);
			services.AddMvc(ConfigureMvc).AddNewtonsoftJson(ConfigureNewtonsoft);
			services.UseSimpleDbContext();
			
			services.AddSingleton<IWorkflowStorage, SqlServerWorkflowStorage>();
			services.AddSingleton<IInstanceStorage, SqlServerInstanceStorage>();
			services.AddSingleton<ISerializer, Serializer>();
			services.AddSingleton<IScriptNativeObjectProvider, ScriptNativeObjects>();

			services.AddScoped<IDeferredTarget, WorkflowDeferred>();

			services.AddScoped<WorkflowEngine>();
			services.AddScoped<IWorkflowEngine>(s => s.GetService<WorkflowEngine>());
			services.AddScoped<IWorkflowApi>(s => s.GetService<WorkflowEngine>());

			services.AddScoped<ITracker, InstanceTracker>();
		}

		private static void ConfigureMvc(MvcOptions opt)
		{
			
		}

		private static void ConfigureNewtonsoft(MvcNewtonsoftJsonOptions opt)
		{
			
		}

		public static void SetControllerOptions(MvcOptions options)
		{
			options.EnableEndpointRouting = false;
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapGet("/", async context =>
				{
					await context.Response.WriteAsync("Hello World!");
				});
			});
		}
	}
}
