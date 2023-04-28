
using Amazon.S3;
using AwsS3LifeBackup.Core.Communication.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;
using AwsS3LifeBackup.Infrastructure.Repositories;

namespace AwsS3LifeBackup.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args)!;

            // Add services to the container.
            RegisterServices(builder.Services, builder.Configuration);

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

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

            app.UseExceptionHandler(a => a?.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature?.Error;

                var result = JsonSerializer.Serialize(new { error = exception?.Message });
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }));

            app.Run();
        }
        

        private static void RegisterServices(IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddAWSService<IAmazonS3>(configuration.GetAWSOptions());

            services.AddSingleton<IBucketRepository, BucketRepository>();
            services.AddSingleton<IFilesRepository, FilesRepository>();
        }
    }
}