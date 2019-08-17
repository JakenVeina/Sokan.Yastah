﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Sokan.Yastah.Business.Authentication;
using Sokan.Yastah.Business.Authorization;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Business.Users
{
    public interface IUsersOperations
    {
        Task<OperationResult<UserDetailViewModel>> GetDetailAsync(
            ulong userId,
            CancellationToken cancellationToken);

        Task<OperationResult<IReadOnlyCollection<UserOverviewViewModel>>> GetOverviewsAsync(
            CancellationToken cancellationToken);

        Task<OperationResult> UpdateAsync(
            ulong userId,
            UserUpdateModel updateModel,
            CancellationToken cancellationToken);
    }

    public class UsersOperations
        : IUsersOperations
    {
        public UsersOperations(
            IAuthenticationService authenticationService,
            IAuthorizationService authorizationService,
            IUsersRepository usersRepository,
            IUsersService usersService)
        {
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _usersRepository = usersRepository;
            _usersService = usersService;
        }

        public async Task<OperationResult<UserDetailViewModel>> GetDetailAsync(
            ulong userId,
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)AdministrationPermission.ManageRoles);

            return authResult.IsFailure
                ? authResult.Error.ToError<UserDetailViewModel>()
                : await _usersRepository.ReadDetailAsync(
                    userId,
                    cancellationToken);
        }

        public async Task<OperationResult<IReadOnlyCollection<UserOverviewViewModel>>> GetOverviewsAsync(
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)AdministrationPermission.ManageRoles);

            return authResult.IsFailure
                ? authResult.Error.ToError<IReadOnlyCollection<UserOverviewViewModel>>()
                : (await _usersRepository.ReadOverviewsAsync(cancellationToken))
                    .ToSuccess();
        }
        public async Task<OperationResult> UpdateAsync(
            ulong userId,
            UserUpdateModel updateModel,
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)AdministrationPermission.ManageRoles);

            var performedById = _authenticationService.CurrentTicket.UserId;

            return authResult.IsFailure
                ? authResult
                : await _usersService.UpdateAsync(userId, updateModel, performedById, cancellationToken);
        }

        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUsersRepository _usersRepository;
        private readonly IUsersService _usersService;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services.AddScoped<IUsersOperations, UsersOperations>();
    }
}