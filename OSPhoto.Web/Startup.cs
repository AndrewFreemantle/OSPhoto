using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OSPhoto.Services;
using OSPhoto.Services.Interfaces;
using OSPhoto.Web.Converters;
using OSPhoto.Web.ViewModels;

namespace OSPhoto.Web
{
    public class Startup
    {
        readonly string CorsDevelopmentOrigins = "_corsDevelopmentOrigins";
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IAlbumService>(provider =>
            {
                var webHostEnvironment = provider.GetService<IWebHostEnvironment>();
                var contentRoot = new DirectoryInfo(webHostEnvironment?.ContentRootPath);
                return new AlbumService(contentRoot.Parent?.FullName);
            });

            services.AddAutoMapper(typeof(ViewModelMappingProfile));
            
#if DEBUG
            services.AddCors(options =>
            {
                options.AddPolicy(
                    name: CorsDevelopmentOrigins,
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:8080")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
#endif

            services.AddControllers();

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "WebApi", Version = "v1"}); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1"));
            }

            app.UseCors(CorsDevelopmentOrigins);

            app.UseFileServer();

            // app.UseHttpsRedirection();
            
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
