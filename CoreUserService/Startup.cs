using AutoMapper;
using CoreUserService.Entities;
using CoreUserService.Models;
using CoreUserService.Repositories;
using CoreUserService.Repositories.Interfaces;
using CoreUserService.Services;
using CoreUserService.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using System.Text;

namespace CoreUserService
{
    /// <summary>
    /// Handles .NET Core application startup tasks
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// The IConfiguration object, used for reading application settings
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// CoreUserService Startup ctor
        /// </summary>
        /// <param name="configuration">IConfiguration container instance</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Method is automatically called at runtime
        /// Use to add additional services to the IOC container
        /// </summary>
        /// <param name="services">IServiceCollection implementation</param>
        public void ConfigureServices(IServiceCollection services)
        {
            //Add authentication service using JWTs
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Tokens:Issuer"],
                        ValidAudience = Configuration["Tokens:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
                    };
                });

            //Add CORS service to the container
            services.AddCors(o => o.AddPolicy("AllowAllCorsPolicy", builder => 
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }));

            //Add mvc service with optional support for xml
            services.AddMvc()
                .AddMvcOptions(x => x.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter()))
                .AddMvcOptions(x => x.InputFormatters.Add(new XmlDataContractSerializerInputFormatter()));

            //Add DomainDbContext service to the container;
            services.AddDbContext<DomainDbContext>(x => x.UseSqlServer(Configuration["ConnectionStrings:SqlConnectionString"]));

            //Add Repositories to the container
            services.AddScoped<IUserRepository, UserRepository>();

            //Add logging to the container
            services.AddLogging();

            //Add ITokenIssuerService to the container
            services.AddScoped<ITokenIssuerService, JwtTokenIssuerService>();
        }

        /// <summary>
        /// Method is automatically called at runtime
        /// Use to configure the request pipeline
        /// </summary>
        /// <param name="app">IApplicationBuilder implementation</param>
        /// <param name="env">IHostingEnvironment implementation</param>
        /// <param name="loggerFactory">ILoggerFactory implementation</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //Configure logger
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
            loggerFactory.AddNLog();

            //Configure exception handling middleware based on environment
            if (env.IsDevelopment())
            {
                //Use developer exception middleware
                app.UseDeveloperExceptionPage();

                //Allow all CORS requests while in development
                app.UseCors("AllowAllCorsPolicy");
            }
            else
            {
                //Use exception handling/logging middleware
                app.UseExceptionHandler();
            }

            //Add authentication middleware for http requests
            app.UseAuthentication();

            //Configure auto mapper entity/dto mappings
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<User, UserDto>().ForMember(x => x.Password, opt => opt.Ignore());
                cfg.CreateMap<UserDto, User>().ForMember(x => x.Id, opt => opt.Ignore());
                cfg.CreateMap<UserInformationDto, User>();
                cfg.CreateMap<Address, AddressDto>();
                cfg.CreateMap<AddressDto, Address>();
            });

            //Add mvc middleware for http requests
            app.UseMvc();
        }
    }
}
