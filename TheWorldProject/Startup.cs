using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using TheWorldProject.Services;
using Microsoft.Extensions.Configuration;
using TheWorldProject.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using AutoMapper;
using TheWorldProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace TheWorldProject
{
    public class Startup
    {
        private IHostingEnvironment _env;
        private IConfigurationRoot _config;

        public Startup(IHostingEnvironment env)
        {
            _env = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();
            _config = builder.Build();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_config);
            services.AddScoped<IMailService, DebugMailService>();



            services.AddDbContext<WorldContext>();
            services.AddTransient<WorldsContextSeedData>();
            services.AddTransient<GeoCoordsService>();
            services.AddScoped<IWorldRepository, WorldRepository>();
            services.AddLogging();
            services.AddIdentity<WorldUser, IdentityRole>(config =>
            {
                config.User.RequireUniqueEmail = true;  // цим ми гарантуємо, що кожен користувач повинен мати унікальний емейл (2 емейла не може бути у двох акаунтів)
                //config.SignIn.RequireConfirmedEmail = true; // щоб не міг користувач увійти в систему, поки не підтверджена реєстрація на емейлі
                config.Password.RequiredLength = 8; // мінімальна довжина пароля - 8 символів
                // В 1.0 версії ось так кукі налаштовувалось
                //config.Cookies.ApplicationCookie.LoginPath = "/Auth/Login"; // тут вказується, шлях куди ми будемо перенаправляти людей
                //                                                 // якщо вони не зареєстровані, Auth - контролер, Login - дія (Екшен)

            }).AddEntityFrameworkStores<WorldContext>() // тут ми вказуємо в якій БД, буде зберігатись наші користувачі
            .AddDefaultTokenProviders();

            // НАЛАШТУВАННЯ КУКІ В 2.0 
            services.ConfigureApplicationCookie(config =>
            {
                config.LoginPath = "/Auth/Login";
                config.Events = new CookieAuthenticationEvents() // ми встановимо тут callback, який буде викликатись, коли відбудеться авторизації
                {
                    OnRedirectToLogin = async ctx => // змінемо роботу аутентифікації кукі, тобто змінемо роботу Перенаправлення під час авторизації
                                                     // тобто так як у нас є API ми хочемо повертати СТАТУСНИЙ КОД, а не те перенаправлення,, тобто сторінку Login
                                                     // це напевно чисто для зручності і узгодження 
                    {
                        // перевіремо тут чи це є API, тобто перевіримо чи починається на .StartsWithSegments("/api")
                        if (ctx.Request.Path.StartsWithSegments("/api") &&
                        ctx.Response.StatusCode == 200)
                        {
                            ctx.Response.StatusCode = 401;
                        }
                        else
                        {
                            ctx.Response.Redirect(ctx.RedirectUri); // RedirectUri = /auth/login
                        }
                        await Task.Yield(); // каже, піти і завершити Task
                    }
                };
            });

            services.AddMvc(config =>
            {
                if (_env.IsProduction())
                {
                    // ЦЕ ПОТРІБНО ОБОВЗЯКОВО РОБИТИ, КОЛИ РОЗГОРТУЄМО ПРОГРАМУ
                    config.Filters.Add(new RequireHttpsAttribute()); // коли ти спробуєш перейти на http, то воно переадресує на https - і це працює для всього сайту
                }

            })
                .AddJsonOptions(config =>
                {
                    config.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                }); // тут ми відправляємо лямбда-вираз, в якому налаштуємо, що роботи, коли отримаємо JSON



        }




        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory factory, WorldsContextSeedData seeder)
        {
            if (env.IsEnvironment("Development"))
            {
                app.UseDeveloperExceptionPage();
                factory.AddDebug(LogLevel.Information); // тобто ми логуємо інформацію
            }
            else
            {
                factory.AddDebug(LogLevel.Error); // тут ми логуємо помилки
            }
            if (env.IsEnvironment("Development")) // глянемо в властивостях проекта у вкладці Debug і буде видно, що вставновленно в Development
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            Mapper.Initialize(config =>
            {
                config.CreateMap<TripViewModel, Trip>(); // <Source, Destination>
                config.CreateMap<StopViewModel, Stop>();
                config.CreateMap<RegisterViewModel, WorldUser>();

            });
            app.UseMvc(configRoutes =>
            {
                // нагадую, що тут використовуємо іменований виклик аргументів метода MapRoute через імяАрг: значення
                // створимо дефолтний маршрут, якщо жоден з маршутів не використовується для спеціального метода
                configRoutes.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "App", action = "Index" } // App - наш контролер, Index - метод в нашому контролері, тобто воно співставляє імя нашого контролера (AppController) з ВИДОМ в папці App (яка співставляється до нашого контролера, не знаю чому скорочене імя, а не повне AppController). Тут виходить, що і папка, де знаходиться наш ВИД повинна називатись також App, 
                    );
            });





            seeder.EnsureSeedData().Wait();
        }














    }
}