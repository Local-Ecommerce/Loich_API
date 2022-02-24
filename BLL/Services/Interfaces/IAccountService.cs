﻿using BLL.Dtos.Account;
using BLL.Dtos.RefreshToken;
using DAL.Models;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IAccountService
    {
        /// <summary>
        /// Create Account
        /// </summary>
        /// <param name="account"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        Task<Account> Register(Account account, string uid);


        /// <summary>
        /// Login
        /// </summary>
        /// <param name="accountRequest"></param>
        /// <returns></returns>
        Task<ExtendAccountResponse> Login(AccountRequest accountRequest);


        /// <summary>
        /// Get Account by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ExtendAccountResponse> GetAccountById(string id);


        /// <summary>
        /// Update Account
        /// </summary>
        /// <param name="accountRequest"></param>
        /// <returns></returns>
        Task<ExtendAccountResponse> UpdateAccount(string id);


        /// <summary>
        /// Delete Account by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AccountResponse> DeleteAccount(string id);


        /// <summary>
        /// Refresh Token
        /// </summary>
        /// <param name="refreshTokenDto"></param>
        /// <returns></returns>
        Task<string> RefreshToken(RefreshTokenDto refreshTokenDto);


        /// <summary>
        /// Change Resident Type By Account Id
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="residentType"></param>
        /// <returns></returns>
        Task<ExtendAccountResponse> ChangeResidentTypeByAccountId(string accountId, string residentType);
    }
}
