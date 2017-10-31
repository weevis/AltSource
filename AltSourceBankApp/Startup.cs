using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AltSourceBankAppAPI.Entity;
using AltSourceBankAppAPI.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using AltSourceBankAppAPI.Repository;
using AltSourceBankAppAPI.Service;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using System.Text;

namespace AltSourceBankApp
{
    public class Error
    {
        public string Message { get; set; }
        public string Stacktrace { get; set; }
    }

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
            try
            {
                //use an in memory database, entity framework core
                services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("BankAPI"));

                //set up our user system to use the database and provide tokens
                services.AddIdentity<UserEntity, IdentityRole>().AddEntityFrameworkStores<ApiContext>().AddDefaultTokenProviders();

                //cors to allow cross domain access
                services.AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy", 
                        builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod().AllowAnyHeader().AllowCredentials());
                });

                services.AddMvc();

                this.ConfigureCookies(services);
                services.AddTransient<ITransactionRepository, TransactionRepository>();
                services.AddTransient<ITransactionService, TransactionService>();
            }
            catch( Exception ex )
            {
                var err = JsonConvert.SerializeObject(new Error()
                {
                    Stacktrace = ex.StackTrace,
                    Message = ex.Message
                });

                //if we get an exception this early, let's log it and then stop the app
                System.IO.File.WriteAllText(@"exception.txt", err);
                throw;
            }
        }

        /// <summary>
        /// Configure system cookies to set us to the right status code
        /// </summary>
        /// <param name="services">our current services so we can add cookie configuration</param>
        public void ConfigureCookies(IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = ctx =>
                    {
                        if( ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200 )
                        {
                            ctx.Response.StatusCode = 401;
                            return Task.FromResult<object>(null);
                        }

                        ctx.Response.Redirect(ctx.RedirectUri);
                        return Task.FromResult<object>(null);
                    },
                    OnRedirectToAccessDenied = ctx =>
                    {
                        if( ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200 )
                        {
                            ctx.Response.StatusCode = 403;
                            return Task.FromResult<object>(null);
                        }

                        ctx.Response.Redirect(ctx.RedirectUri);
                        return Task.FromResult<object>(null);
                    }
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //global exception handler
            try
            {
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseAuthentication();
                app.UseExceptionHandler(
                    builder =>
                    {
                        builder.Run(
                            async context =>
                            {
                                context.Response.StatusCode = 500;
                                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                                var error = context.Features.Get<IExceptionHandlerFeature>();

                                if (error != null)
                                {
                                    var err = JsonConvert.SerializeObject(new Error()
                                    {
                                        Stacktrace = error.Error.StackTrace,
                                        Message = error.Error.Message
                                    });
                                    await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(err), 0, err.Length).ConfigureAwait(false);
                                }
                            });
                    }
                );
                app.UseCors("CorsPolicy");
                app.UseMvc(routes => routes.MapRoute(name: "Account", template: "account/{action}/{id}",
                    defaults: new { controller = "Account", action = "Index" }));
            }
            catch( Exception ex )
            {
                app.Run(
                    async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";
                        if( ex != null )
                        {
                            var err = JsonConvert.SerializeObject(new Error()
                            {
                                Stacktrace = ex.StackTrace,
                                Message = ex.Message
                            });
                            await context.Response.Body
                                .WriteAsync(Encoding.UTF8.GetBytes(err), 0, err.Length)
                                .ConfigureAwait(false);
                        }
                    });
            }
        }
    }
}
