﻿using BLL.Dtos;
using BLL.Dtos.Account;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IAccountService
    {
        /// <summary>
        /// Create Account
        /// </summary>
        /// <param name="ExtendAccountResponse"></param>
        /// <returns></returns>
        Task<BaseResponse<ExtendAccountResponse>> Register(AccountRegisterRequest accountRegisterRequest);


        /// <summary>
        /// Login
        /// </summary>
        /// <param name="accountLoginRequest"></param>
        /// <returns></returns>
        Task<BaseResponse<ExtendAccountResponse>> Login(AccountLoginRequest accountLoginRequest);

        /// <summary>
        /// Get Account by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BaseResponse<ExtendAccountResponse>> GetAccountById(string id);

        /// <summary>
        /// Update Account
        /// </summary>
        /// <param name="accountRequest"></param>
        /// <returns></returns>
        Task<BaseResponse<ExtendAccountResponse>> UpdateAccount(string id);


        /// <summary>
        /// Delete Account by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BaseResponse<ExtendAccountResponse>> DeleteAccount(string id);

        /// <summary>
        /// Check valid confirm password
        /// </summary>
        /// <param name="password"></param>
        /// <param name="confirmPassword"></param>
        bool IsValidConfirmPassword(string password, string confirmPassword);


        /// <summary>
        /// Change Resident Type By Account Id
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="residentType"></param>
        /// <returns></returns>
        Task<BaseResponse<ExtendAccountResponse>> ChangeResidentTypeByAccountId(string accountId, string residentType);
    }
}
