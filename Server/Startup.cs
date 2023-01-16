﻿using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using EventsManager.Server.Data;
using EventsManager.Server.Handlers.Queries;
using EventsManager.Server.Models;
using EventsManager.Shared;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EventsManager.Server;

public class Startup {
    
    public IConfiguration configRoot {
        get;
    }
    
    public Startup(IConfiguration configuration) {
        configRoot = configuration;
    }
    
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration) {
        // Add services to the container.
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddDefaultIdentity<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 3;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredUniqueChars = 0;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddIdentityServer(options => {})
            .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(opt => 
            {
                opt.IdentityResources["openid"].UserClaims.Add("role");
                opt.ApiResources.Single().UserClaims.Add("role");
            });
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("role");

        services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId = "104708052542-uqishvtn1l1kue90l72j7l8q6ciipfmc.apps.googleusercontent.com";
                options.ClientSecret = "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";
            })
            .AddIdentityServerJwt();

        services.AddControllersWithViews();
        services.AddRazorPages();
        
        services.AddMediatR(typeof(GetMyUserCommandHandler).GetTypeInfo().Assembly);
    }
    
    public void Configure(WebApplication app, IWebHostEnvironment env, IServiceProvider services) {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();    
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseIdentityServer();
        app.UseAuthorization();
        
        app.MapRazorPages();
        app.MapControllers();
        app.MapFallbackToFile("index.html");
        
        app.Run();
    }
}
