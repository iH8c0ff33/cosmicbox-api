using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CosmicBox.Models;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;

namespace CosmicBox {
    public class Startup {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("Events"));
            services.AddMvc();

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info {
                    Title = "CosmicBox",
                    Version = "v1",
                    Description = "Used by cosmicbox clients to send data to a DB",
                    Contact = new Contact { Name = "Daniele Monteleone", Email = "daniele.monteleone.it@gmail.com" }
                });

                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "CosmicBox.xml"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CosmicBox API v1");
                });

                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
