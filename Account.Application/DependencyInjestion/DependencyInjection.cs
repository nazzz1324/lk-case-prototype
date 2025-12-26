using Account.Application.Mapping;
using Account.Application.Services;
using Account.Domain.Interfaces.Services;
using Account.Domain.Interfaces.Validations;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(DependencyInjection).Assembly); 

            InitServices(services);
        }

        private static void InitServices(this IServiceCollection services)
        {
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IIndicatorService, IndicatorService>();
            services.AddScoped<ICompetenceService, CompetenceService>();
            services.AddScoped<IDisciplineService, DisciplineService>();
            services.AddScoped<IProfessionalRoleService, ProfessionalRoleService>();
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IUserService, UserService>();
        }

    }
}
