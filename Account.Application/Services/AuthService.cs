using Account.Application.Resources;
using Account.Domain.DTO;
using Account.Domain.DTO.User;
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
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly IBaseRepository<UserRole> _userRoleRepository;
        private readonly IBaseRepository<UserToken> _userTokenRepository;
        private readonly ITokenService _tokenService;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public AuthService(IBaseRepository<User> userRepository, IBaseRepository<UserToken> userTokenRepository,
            ILogger logger, IMapper mapper, ITokenService tokenService, IBaseRepository<Role> roleRepository,
            IBaseRepository<UserRole> userRoleRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _userTokenRepository = userTokenRepository;
            _logger = logger;
            _mapper = mapper;
            _tokenService = tokenService;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResult<TokenDto>> Login(LoginUserDto dto)
        {
            var user = await _userRepository.GetAll()
                   .Include(x => x.Roles)
                   .FirstOrDefaultAsync(x => x.Login == dto.Login);

            if (user == null)
            {
                throw new ExceptionResult(  
                    ErrorCodes.UserNotFound,
                    ErrorMessage.UserNotFound
                );
            }

            if (user == null)
            {
                throw new ExceptionResult(
                    ErrorCodes.InternalServerError,
                    ErrorMessage.InternalServerError
                );
            }

            if (!IsVerifyPassword(user.Password, dto.Password))
            {
                throw new ExceptionResult(
                    ErrorCodes.PasswordIsWrong,
                    ErrorMessage.PasswordIsWrong
                );
            }

            var userToken = await _userTokenRepository.GetAll().FirstOrDefaultAsync(x => x.UserId == user.Id);

            var userRoles = user.Roles;
            var claims = userRoles.Select(x => new Claim(ClaimTypes.Role, x.Name)).ToList();
            claims.Add(new Claim(ClaimTypes.Name, user.Login));
            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            if (userToken == null)
            {
                userToken = new UserToken()
                {
                    UserId = user.Id,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
                };
                await _userTokenRepository.CreateAsync(userToken);
            }

            else
            {
                userToken.RefreshToken = refreshToken;
                userToken.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                _userTokenRepository.Update(userToken);
                await _userTokenRepository.SaveChangesAsync();

            }

            return new BaseResult<TokenDto>()
            {
                Data = new TokenDto()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    Role = userRoles.FirstOrDefault()?.Name ?? "Student",//добавил
                    Id = user.Id // добавил ID пользователя
                }
            };
        }

        public async Task<BaseResult<bool>> Register(RegisterUserDto dto)
        {
            // if (dto.Password != dto.PasswordConfirm)
            // {
            //     throw new ExceptionResult(
            //         ErrorCodes.PasswordNotEqualsPasswordConfirm,
            //         ErrorMessage.PasswordNotEqualsPasswordConfirm
            //     );
            // }

            try
            {
                var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Login == dto.Email);
                if (user != null)
                {
                    throw new ExceptionResult(
                        ErrorCodes.UserAlreadyExists,
                        ErrorMessage.UserAlreadyExists
                    );
                }

                var hashUserPassword = HashPassword(dto.Password);

                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        user = new User()
                        {
                            Login = dto.Email,
                            Password = hashUserPassword,
                        };
                        await _unitOfWork.Users.CreateAsync(user);
                        await _unitOfWork.SaveChangesAsync();

                        if (dto.RoleId == (long)Roles.Student)
                        {
                            var student = new Student()
                            {
                                Id = user.Id,
                                Firstname = dto.Firstname,
                                Lastname = dto.Lastname,
                                Middlename = dto.Middlename,
                                IsActive = true
                            };
                            await _unitOfWork.Students.CreateAsync(student);
                        }
                        else if (dto.RoleId == (long)Roles.Teacher)
                        {
                            var teacher = new Teacher()
                            {
                                Id = user.Id,
                                Firstname = dto.Firstname,
                                Lastname = dto.Lastname,
                                Middlename = dto.Middlename,
                                IsActive = true
                            };
                            await _unitOfWork.Teachers.CreateAsync(teacher);
                        }

                        await _unitOfWork.SaveChangesAsync();

                        var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.RoleId);
                        if (role == null)
                        {
                            throw new ExceptionResult(
                                ErrorCodes.RoleNotFound,
                                ErrorMessage.RoleNotFound
                            );
                        }

                        var userRole = new UserRole()
                        {
                            UserId = user.Id,
                            RoleId = role.Id,
                        };
                        await _unitOfWork.UserRoles.CreateAsync(userRole);

                        await _unitOfWork.SaveChangesAsync();

                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }

                return new BaseResult<bool>
                {
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);

                throw new ExceptionResult(
                    ErrorCodes.InternalServerError,
                    ErrorMessage.InternalServerError
                );
            }
        }
        
        private string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
        private bool IsVerifyPassword(string userPasswordHash,string userPassword)
        {
            var hash = HashPassword(userPassword);
            return userPasswordHash == hash;
        }
    }
}
