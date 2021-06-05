using LtxMagayaCore.Core.Application.Interfaces;
using LtxMagayaCore.Core.Application.Services;
using LtxMagayaCore.Infrastructure.Data;
using LtxMagayaCore.Infrastructure.Interfaces;
using LtxMagayaCore.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LtxMagayaCore.Infrastructure.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEntitiesMagaya, EntitiesMagaya>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEntitiesService, EntitiesService>();
            services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(typeof(DataContext).Assembly.FullName)));
            return services;
        }
    }
}
