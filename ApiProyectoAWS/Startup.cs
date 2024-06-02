using ApiCoreProyectoEventos.Helpers;
using ApiCoreProyectoEventos.Models;
using ApiProyectoAWS.Data;
using ApiProyectoAWS.Helpers;
using ApiProyectoAWS.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace ApiProyectoAWS;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        // Obtener el secreto
        string miSecreto = HelperSecretManager.GetSecretAsync().GetAwaiter().GetResult();
        KeysModel model = JsonConvert.DeserializeObject<KeysModel>(miSecreto);

        string issuer = model.Issuer;
        string audience = model.Audience;
        string secretKey = model.SecretKey;

        string connectionString = model.ConnectionString;

        services.AddTransient<HelperPathProvider>();
        services.AddTransient<HelperCryptography>();
        services.AddTransient<HelperMails>();
        services.AddTransient<HelperTools>();

        services.AddTransient<RepositoryEventos>();
        services.AddDbContext<EventosContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        services.AddCors(options =>
        {
            options.AddPolicy("AllowOrigin", x => x.AllowAnyOrigin());
        });

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Api Proyecto AWS",
                Version = "v1",
            });
        });

        services.AddControllers();

        HelperActionServicesOAuth helper = new HelperActionServicesOAuth(issuer, audience, secretKey);
        services.AddSingleton<HelperActionServicesOAuth>(helper);
        services.AddAuthentication(helper.GetAuthenticateSchema()).AddJwtBearer(helper.GetJwtBearerOptions());

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseCors("AllowOrigin");

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("swagger/v1/swagger.json", "Api Proyecto AWS v1");
            options.RoutePrefix = string.Empty;
        });

        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });
    }
}
