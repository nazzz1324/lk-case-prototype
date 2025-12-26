using Account.DAL.Interceptor;
using Account.DAL.Repositories;
using Account.Domain.Entity;
using Account.Domain.Entity.AuthRole;
using Account.Domain.Entity.LinkedEntites;
using Account.Domain.Interfaces.Databases;
using Account.Domain.Interfaces.Repositories;
using Account.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Account.DAL.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
         {
            var connectionString = configuration.GetConnectionString("PostgreSQL"); 
            

            services.AddDbContext<ApplicationDbContext>(options => 
            { 
                options.UseNpgsql(connectionString);
            });
            services.InitRepositories();
        }

        private static void InitRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IBaseRepository<User>, BaseRepository<User>>();
            services.AddScoped<IBaseRepository<UserToken>, BaseRepository<UserToken>>();
            services.AddScoped<IBaseRepository<Role>, BaseRepository<Role>>();
            services.AddScoped<IBaseRepository<UserRole>, BaseRepository<UserRole>>();

            services.AddScoped<IBaseRepository<Competence>, BaseRepository<Competence>>();
            services.AddScoped<IBaseRepository<CompetenceScore>, BaseRepository<CompetenceScore>>();
            services.AddScoped<IBaseRepository<Discipline>, BaseRepository<Discipline>>();
            services.AddScoped<IBaseRepository<DisciplineScore>, BaseRepository<DisciplineScore>>();
            services.AddScoped<IBaseRepository<Group>, BaseRepository<Group>>();
            services.AddScoped<IBaseRepository<Indicator>, BaseRepository<Indicator>>();
            services.AddScoped<IBaseRepository<IndicatorScore>, BaseRepository<IndicatorScore>>();
            services.AddScoped<IBaseRepository<Student>, BaseRepository<Student>>();
            services.AddScoped<IBaseRepository<Teacher>, BaseRepository<Teacher>>();
            services.AddScoped<IBaseRepository<EducationForm>, BaseRepository<EducationForm>>();
            services.AddScoped<IBaseRepository<Faculty>, BaseRepository<Faculty>>();
            services.AddScoped<IBaseRepository<ProfessionalRole>, BaseRepository<ProfessionalRole>>();

            services.AddScoped<IBaseRepository<GroupDiscipline>, BaseRepository<GroupDiscipline>>();
            services.AddScoped<IBaseRepository<TeacherDiscipline>, BaseRepository<TeacherDiscipline>>();
        }
    }
}
