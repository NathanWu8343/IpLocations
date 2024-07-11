using IpLocations.Api.Infra;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Writers;
using StackExchange.Redis;

namespace IpLocations.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //redis
            var redisConnection = builder.Configuration.GetConnectionString("Redis")
                                  ?? throw new ArgumentNullException("RedisConnection");

            builder.Services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(redisConnection));

            builder.Services.AddSingleton<IpRangeStore>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            Initialize(app);
            app.Run();
        }

        private static void Initialize(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            scope.ServiceProvider.GetRequiredService<IpRangeStore>().WarmUp();
        }
    }
}