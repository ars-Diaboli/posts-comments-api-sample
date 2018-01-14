using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PostsCommentsSample.Data.Repositories;
using PostsCommentsSample.Domain.Services;
using PostsCommentsSample.Web.Framework;

namespace PostsCommentsSample.Web
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
            services.AddMvc();

	        services.AddTransient<ICommentsService, CommentsService>();
	        services.AddTransient<ICommentsRepository, CommentsRepository>();
			services.AddTransient<IPostsService, PostsService>();
			services.AddTransient<IPostsRepository, PostsRepository>();
        }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
			app.UseMiddleware(typeof(ErrorHandlingMiddleware));

			//if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            app.UseMvc();
        }
    }
}
