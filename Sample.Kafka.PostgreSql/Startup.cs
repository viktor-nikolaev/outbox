using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sample.Kafka.PostgreSql.Model;
using ZionApps.Outbox.AspNetCore;
using ZionApps.Outbox.Kafka;
using ZionApps.Outbox.PostgreSql;

namespace Sample.Kafka.PostgreSql
{
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
            services.AddControllers();

            services
                .AddOutbox(x =>
                {
                    x.MapTopicNameToEventName("orders", "orders.kafka.topic");
                    x.SequentialEventNames.Add("orders");
                })
                .UseKafka(x => x.ProducerConfig.BootstrapServers = Configuration.GetValue<string>("Kafka:Hosts"))
                .UsePostgreSql(x => x.ConnectionString = Configuration.GetConnectionString("Postgre"));

            var connectionString = Configuration.GetConnectionString("Postgre");
            services.AddDbContext<OrdersDbContext>(options => options.UseNpgsql(connectionString));
            
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, OrdersDbContext context)
        {
            context.Database.Migrate();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}