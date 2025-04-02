using Blogs.Application.Mapping;
using Blogs.Application.Services;
using Blogs.Domain.Interfaces.Database;
using Blogs.Domain.Interfaces.Repositories;
using Blogs.Domain.Interfaces.Services;
using Blogs.Infrastructure.Database;
using Blogs.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Blogs.API;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("Default"));
        });
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<IBlogRepository, BlogRepository>();
        
        services.AddScoped<IBlogService, BlogService>();
        
        services.AddRouting();
        services.AddControllers();
        services.AddOpenApi();
    }

    public static void Configure(WebApplication app)
    {
        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
    }
}