using System;
using System.IO;
using System.Threading.Tasks;
using CosmicBox.Hubs;
using CosmicBox.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace CosmicBox {
    public class Startup {
        public Startup(IHostingEnvironment env) {
            var builder = new ConfigurationBuilder();

            if (env.IsDevelopment()) {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<CosmicContext>();
            services.AddMvc();
            services.AddSignalR();

            services
                .AddAuthentication(c => {
                    c.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    c.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(c => {
                    c.Authority = "https://e3auth.eu.auth0.com/";
                    c.Audience = "https://eee.lsgalfer.it/api";

                    c.Events = new JwtBearerEvents {
                        OnMessageReceived = context => {
                            var accessToken = context.Request.Query["access_token"];

                            bool isWebSocketRequest = context.HttpContext.WebSockets.IsWebSocketRequest;
                            if (
                                !string.IsNullOrEmpty(accessToken) &&
                                (isWebSocketRequest ||
                                    context.Request.Headers["Upgrade"] == "websocket" ||
                                    context.Request.Headers["Accept"] == "text/event-stream")
                            ) {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(c => {
                c.AddPolicy("test", p => p.RequireClaim("scope", "read:test"));
                c.AddPolicy("write:events", p => p.RequireClaim("scope", "write:events"));
                c.AddPolicy("add:boxes", p => p.RequireClaim("scope", "add:boxes"));
                c.AddPolicy("delete:boxes", p => p.RequireClaim("scope", "delete:boxes"));
            });

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info {
                    Title = "CosmicBox",
                    Version = "v1",
                    Description = "Used by cosmicbox clients to send data to a DB",
                    Contact = new Contact {
                        Name = "Daniele Monteleone",
                        Email = "daniele.monteleone.it@gmail.com"
                    }
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

            app.UseAuthentication();
            app.UseSignalR(r => {
                r.MapHub<EventHub>("/evhub");
            });

            app.UseMvc();
        }
    }
}
