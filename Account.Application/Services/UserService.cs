using Account.Application.Resources;
using Account.Domain.DTO.User;
using Account.Domain.DTO.Users;
using Account.Domain.Entity;
using Account.Domain.Entity.AuthRole;
using Account.Domain.Enum;
using Account.Domain.Interfaces.Databases;
using Account.Domain.Interfaces.Repositories;
using Account.Domain.Interfaces.Services;
using Account.Domain.Result;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Student> _studentRepository;
        private readonly IBaseRepository<Teacher> _teacherRepository;
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<UserRole> _userRoleRepository;
        private readonly IBaseRepository<UserToken> _userTokenRepository;
        private readonly ITokenService _tokenService;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public UserService(IBaseRepository<User> userRepository, IBaseRepository<Student> studentRepository,
            IBaseRepository<Teacher> teacherRepository, IBaseRepository<Role> roleRepository, ILogger logger,
            IMapper mapper, IUnitOfWork unitOfWork, IBaseRepository<UserRole> userRoleRepository,
            IBaseRepository<UserToken> userTokenRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _roleRepository = roleRepository;
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userRoleRepository = userRoleRepository;
            _userTokenRepository = userTokenRepository;
            _tokenService = tokenService;
        }

        public async Task<BaseResult<bool>> DeleteUserAsync(long id)
        {
            try
            {
                var user = await _userRepository.GetAll()
                    .Include(u => u.Student)
                    .Include(u => u.Teacher)
                    .Include(u => u.UserToken)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    _logger.Error("Пользователь не найден. Id: {UserId}", id);
                    throw new ExceptionResult(
                        ErrorCodes.UserNotFound,
                        ErrorMessage.UserNotFound
                    );
                }

                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var userRoles = await _userRoleRepository.GetAll()
                            .Where(ur => ur.UserId == id)
                            .ToListAsync();

                        foreach (var userRole in userRoles)
                        {
                            _userRoleRepository.Remove(userRole);
                        }

                        if (user.Student != null)
                        {
                            _studentRepository.Remove(user.Student);
                        }

                        if (user.Teacher != null)
                        {
                            _teacherRepository.Remove(user.Teacher);
                        }

                        if (user.UserToken != null)
                        {
                            _userTokenRepository.Remove(user.UserToken);
                        }

                        _userRepository.Remove(user);

                        await _unitOfWork.SaveChangesAsync();

                        await transaction.CommitAsync();

                        _logger.Information("Пользователь удален. Id: {UserId}", id);

                        return new BaseResult<bool>
                        {
                            Data = true
                        };
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка при удалении пользователя. Id: {UserId}", id);

                throw new ExceptionResult(
                    ErrorCodes.InternalServerError,
                    ErrorMessage.InternalServerError
                );
            }
        }

        public async Task<CollectionResult<UsersDto>> GetUsersAsync()
        {
            var users = await _userRepository.GetAll().ToListAsync();
            var students = await _studentRepository.GetAll().ToListAsync();
            var teachers = await _teacherRepository.GetAll().ToListAsync();
            var userRoles = await _userRoleRepository.GetAll().ToListAsync();
            var roles = await _roleRepository.GetAll().ToListAsync();

            var result = new List<UsersDto>();

            foreach (var user in users)
            {
                var student = students.FirstOrDefault(s => s.Id == user.Id);
                var teacher = teachers.FirstOrDefault(t => t.Id == user.Id);

                string fullName = "";
                bool isActive = false;

                if (student != null)
                {
                    fullName = $"{student.Lastname}   {student.Firstname}   {student.Middlename}".Trim();
                    isActive = student.IsActive;
                }
                else if (teacher != null)
                {
                    fullName = $"{teacher.Lastname}   {teacher.Firstname}   {teacher.Middlename}".Trim();
                    isActive = teacher.IsActive;
                }

                var userRole = userRoles.FirstOrDefault(ur => ur.UserId == user.Id);
                var role = userRole != null
                    ? roles.FirstOrDefault(r => r.Id == userRole.RoleId)
                    : null;
                var roleName = role?.Name ?? "Не назначена";

                result.Add(new UsersDto
                {
                    Id = user.Id,
                    Fullname = fullName,
                    Email = user.Login,
                    Role = roleName,
                    IsActive = isActive
                });
            }

            _logger.Information("Получен список пользователей. Количество: {Count}", result.Count);

            return new CollectionResult<UsersDto>
            {
                Data = result
            };
        }

        public async Task<BaseResult<bool>> UpdateUserAsync(UpdateUserDto dto)
        {
            // Теперь ищем по Id из DTO
            var user = await _userRepository.GetAll()
                .Include(u => u.Student)
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.Id == dto.Id);

            if (user == null)
            {
                _logger.Error("Пользователь не найден. Id: {UserId}", dto.Id);
                throw new ExceptionResult(
                    ErrorCodes.UserNotFound,
                    ErrorMessage.UserNotFound
                );
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Обновляем email если изменился
                    if (user.Login != dto.Email)
                    {
                        // Проверяем что новый email не занят другим пользователем
                        var existingUser = await _userRepository.GetAll()
                            .FirstOrDefaultAsync(u => u.Login == dto.Email && u.Id != dto.Id);

                        if (existingUser != null)
                        {
                            throw new ExceptionResult(
                                ErrorCodes.UserAlreadyExists,
                                ErrorMessage.UserAlreadyExists
                            );
                        }

                        user.Login = dto.Email;
                        _userRepository.Update(user);
                    }

                    if (!string.IsNullOrEmpty(dto.Password))
                    {
                        var hashPassword = HashPassword(dto.Password);
                        user.Password = hashPassword;
                        _userRepository.Update(user);
                    }

                    if (user.Student != null)
                    {
                        user.Student.Firstname = dto.Firstname;
                        user.Student.Lastname = dto.Lastname;
                        user.Student.Middlename = dto.Middlename;
                        user.Student.IsActive = dto.IsActive;
                        _studentRepository.Update(user.Student);
                    }
                    else if (user.Teacher != null)
                    {
                        user.Teacher.Firstname = dto.Firstname;
                        user.Teacher.Lastname = dto.Lastname;
                        user.Teacher.Middlename = dto.Middlename;
                        user.Teacher.IsActive = dto.IsActive;
                        _teacherRepository.Update(user.Teacher);
                    }

                    var currentUserRole = await _userRoleRepository.GetAll()
                        .FirstOrDefaultAsync(ur => ur.UserId == user.Id);

                    if (currentUserRole != null && currentUserRole.RoleId != dto.RoleId)
                    {
                        _userRoleRepository.Remove(currentUserRole);

                        var newUserRole = new UserRole
                        {
                            UserId = user.Id,
                            RoleId = dto.RoleId
                        };
                        await _userRoleRepository.CreateAsync(newUserRole);
                    }

                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.Information("Пользователь обновлен. Id: {UserId}", dto.Id);

                    return new BaseResult<bool>
                    {
                        Data = true
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.Error(ex, "Ошибка при обновлении пользователя. Id: {UserId}", dto.Id);
                    throw new ExceptionResult(
                        ErrorCodes.InternalServerError,
                        ErrorMessage.InternalServerError
                    );
                }
            }
        }
        private string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
