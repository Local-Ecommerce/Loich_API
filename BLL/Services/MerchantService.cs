﻿using AutoMapper;
using BLL.Dtos;
using BLL.Constants;
using BLL.Dtos.Exception;
using BLL.Dtos.Merchant;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.UnitOfWork;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BLL.Services
{
    public class MerchantService : IMerchantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUtilService _utilService;
        private readonly IValidateDataService _validateDataService;
        private const string PREFIX = "MC_";

        public MerchantService(IUnitOfWork unitOfWork,
            ILogger logger,
            IMapper mapper,
            IUtilService utilService,
            IValidateDataService validateDataService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _utilService = utilService;
            _validateDataService = validateDataService;
        }


        /// <summary>
        /// Create Merchant
        /// </summary>
        /// <param name="merchantRequest"></param>
        /// <returns></returns>
        public async Task<BaseResponse<MerchantResponse>> CreateMerchant(MerchantRequest merchantRequest)
        {

            //biz rule


            Merchant merchant = _mapper.Map<Merchant>(merchantRequest);

            //check merchant's name
            if (!_validateDataService.IsValidName(merchant.MerchantName))
            {
                _logger.Error($"[Invalid Merchant's Name]: '{merchant.MerchantName}' ");

                throw new HttpStatusException(HttpStatusCode.OK,
                    new BaseResponse<MerchantResponse>
                    {
                        ResultCode = (int)MerchantStatus.INVALID_NAME_MERCHANT,
                        ResultMessage = MerchantStatus.INVALID_NAME_MERCHANT.ToString(),
                        Data = default
                    });
            }
            
            //check merchant's phone
            if (!_validateDataService.IsValidPhoneNumber(merchant.PhoneNumber))
            {
                _logger.Error($"[Invalid Merchant's Phone]: '{merchant.PhoneNumber}' ");

                throw new HttpStatusException(HttpStatusCode.OK,
                    new BaseResponse<MerchantResponse>
                    {
                        ResultCode = (int)MerchantStatus.INVALID_PHONE_NUMBER_MERCHANT,
                        ResultMessage = MerchantStatus.INVALID_PHONE_NUMBER_MERCHANT.ToString(),
                        Data = default
                    });
            }

            //Store Merchant To Database
            try
            {
                merchant.MerchantId = _utilService.CreateId(PREFIX);
                merchant.Status = (int)MerchantStatus.UNVERIFIED_CREATE_MERCHANT;
                merchant.LevelId = "L001";

                _unitOfWork.Repository<Merchant>().Add(merchant);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Error("[MerchantService.CreateMerchant()]: " + e.Message);

                throw new HttpStatusException(HttpStatusCode.OK,
                    new BaseResponse<MerchantResponse>
                    {
                        ResultCode = (int)CommonResponse.ERROR,
                        ResultMessage = CommonResponse.ERROR.ToString(),
                        Data = default
                    });
            }

            //Create response
            MerchantResponse merchantResponse = _mapper.Map<MerchantResponse>(merchant);

            return new BaseResponse<MerchantResponse>
            {
                ResultCode = (int)CommonResponse.SUCCESS,
                ResultMessage = CommonResponse.SUCCESS.ToString(),
                Data = merchantResponse
            };
        }


        /// <summary>
        /// Get Merchant By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BaseResponse<MerchantResponse>> GetMerchantById(string id)
        {

            //biz rule

            MerchantResponse merchantResponse;

            //Get Merchant From Database
            try
            {
                Merchant merchant = await _unitOfWork.Repository<Merchant>()
                                                   .FindAsync(merchant => merchant.MerchantId.Equals(id));
                merchantResponse = _mapper.Map<MerchantResponse>(merchant);
            }
            catch (Exception e)
            {
                _logger.Error("[MerchantService.GetMerchantById()]: " + e.Message);

                throw new HttpStatusException(HttpStatusCode.OK,
                    new BaseResponse<MerchantResponse>
                    {
                        ResultCode = (int)MerchantStatus.MERCHANT_NOT_FOUND,
                        ResultMessage = MerchantStatus.MERCHANT_NOT_FOUND.ToString(),
                        Data = default
                    });
            }


            return new BaseResponse<MerchantResponse>
            {
                ResultCode = (int)CommonResponse.SUCCESS,
                ResultMessage = CommonResponse.SUCCESS.ToString(),
                Data = merchantResponse
            };
        }


        /// <summary>
        /// Update Merchant By Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="merchantRequest"></param>
        /// <returns></returns>
        public async Task<BaseResponse<MerchantResponse>> UpdateMerchantById(string id, MerchantRequest merchantRequest)
        {

            Merchant merchant;

            //check merchant's name
            if (!_validateDataService.IsValidName(merchantRequest.MerchantName))
            {
                _logger.Error($"[Invalid Merchant's Name]: '{merchantRequest.MerchantName}' ");

                throw new HttpStatusException(HttpStatusCode.OK,
                    new BaseResponse<MerchantResponse>
                    {
                        ResultCode = (int)MerchantStatus.INVALID_NAME_MERCHANT,
                        ResultMessage = MerchantStatus.INVALID_NAME_MERCHANT.ToString(),
                        Data = default
                    });
            }

            //check merchant's phone
            if (!_validateDataService.IsValidPhoneNumber(merchantRequest.PhoneNumber))
            {
                _logger.Error($"[Invalid Merchant's Phone]: '{merchantRequest.PhoneNumber}' ");

                throw new HttpStatusException(HttpStatusCode.OK,
                    new BaseResponse<MerchantResponse>
                    {
                        ResultCode = (int)MerchantStatus.INVALID_PHONE_NUMBER_MERCHANT,
                        ResultMessage = MerchantStatus.INVALID_PHONE_NUMBER_MERCHANT.ToString(),
                        Data = default
                    });
            }

            //Check id
            try
            {
                merchant = await _unitOfWork.Repository<Merchant>().
                                             FindAsync(merchant => merchant.MerchantId.Equals(id));
            }
            catch (Exception e)
            {
                _logger.Error("[MerchantService.UpdateMerchantById()]: " + e.Message);

                throw new HttpStatusException(HttpStatusCode.OK,
                    new BaseResponse<MerchantResponse>
                    {
                        ResultCode = (int)MerchantStatus.MERCHANT_NOT_FOUND,
                        ResultMessage = MerchantStatus.MERCHANT_NOT_FOUND.ToString(),
                        Data = default
                    });
            }

            //update merchant
            try
            {
                merchant = _mapper.Map(merchantRequest, merchant);
                merchant.Status = (int)MerchantStatus.UNVERIFIED_UPDATE_MERCHANT;

                _unitOfWork.Repository<Merchant>().Update(merchant);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Error("[MerchantService.UpdateMerchantById()]: " + e.Message);

                throw new HttpStatusException(HttpStatusCode.OK,
                    new BaseResponse<MerchantResponse>
                    {
                        ResultCode = (int)CommonResponse.ERROR,
                        ResultMessage = CommonResponse.ERROR.ToString(),
                        Data = default
                    });
            }

            //Create Response
            MerchantResponse merchantResponse = _mapper.Map<MerchantResponse>(merchant);

            return new BaseResponse<MerchantResponse>
            {
                ResultCode = (int)CommonResponse.SUCCESS,
                ResultMessage = CommonResponse.SUCCESS.ToString(),
                Data = merchantResponse
            };
        }


        /// <summary>
        /// Delete Merchant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BaseResponse<MerchantResponse>> DeleteMerchant(string id)
        {

            //biz rule

            //Check id
            Merchant merchant;
            try
            {
                merchant = await _unitOfWork.Repository<Merchant>().
                                                      FindAsync(merchant => merchant.MerchantId.Equals(id));
            }
            catch (Exception e)
            {
                _logger.Error("[MerchantService.DeleteMerchant()]: " + e.Message);

                throw new HttpStatusException(HttpStatusCode.OK,
                    new BaseResponse<MerchantResponse>
                    {
                        ResultCode = (int)MerchantStatus.MERCHANT_NOT_FOUND,
                        ResultMessage = MerchantStatus.MERCHANT_NOT_FOUND.ToString(),
                        Data = default
                    });
            }

            //Delete Merchant
            try
            {
                merchant.Status = (int)MerchantStatus.DELETED_MERCHANT;

                _unitOfWork.Repository<Merchant>().Update(merchant);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Error("[MerchantService.DeleteMerchant()]: " + e.Message);

                throw new HttpStatusException(HttpStatusCode.OK,
                    new BaseResponse<MerchantResponse>
                    {
                        ResultCode = (int)CommonResponse.ERROR,
                        ResultMessage = CommonResponse.ERROR.ToString(),
                        Data = default
                    });
            }

            //Create Response
            MerchantResponse merchantResponse = _mapper.Map<MerchantResponse>(merchant);

            return new BaseResponse<MerchantResponse>
            {
                ResultCode = (int)CommonResponse.SUCCESS,
                ResultMessage = CommonResponse.SUCCESS.ToString(),
                Data = merchantResponse
            };
        }


        /// <summary>
        /// Get Merchant By Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<BaseResponse<MerchantResponse>> GetMerchantByName(string name)
        {
            //biz rule

            MerchantResponse merchantResponse;

            //Get Merchant From Database
            try
            {
                Merchant merchant = await _unitOfWork.Repository<Merchant>()
                                                   .FindAsync(merchant => merchant.MerchantName.Equals(name));
                merchantResponse = _mapper.Map<MerchantResponse>(merchant);
            }
            catch (Exception e)
            {
                _logger.Error("[MerchantService.GetMerchantByName()]: " + e.Message);

                throw new HttpStatusException(HttpStatusCode.OK,
                    new BaseResponse<MerchantResponse>
                    {
                        ResultCode = (int)MerchantStatus.MERCHANT_NOT_FOUND,
                        ResultMessage = MerchantStatus.MERCHANT_NOT_FOUND.ToString(),
                        Data = default
                    });
            }

            return new BaseResponse<MerchantResponse>
            {
                ResultCode = (int)CommonResponse.SUCCESS,
                ResultMessage = CommonResponse.SUCCESS.ToString(),
                Data = merchantResponse
            };
        }


        /// <summary>
        /// Get Merchant By Address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public async Task<BaseResponse<MerchantResponse>> GetMerchantByAddress(string address)
        {
            //biz rule

            MerchantResponse merchantResponse;

            //Get Merchant From Database
            try
            {
                Merchant merchant = await _unitOfWork.Repository<Merchant>()
                                                   .FindAsync(merchant => merchant.Address.Equals(address));
                merchantResponse = _mapper.Map<MerchantResponse>(merchant);
            }
            catch (Exception e)
            {
                _logger.Error("[MerchantService.GetMerchantByAddress()]: " + e.Message);

                throw new HttpStatusException(HttpStatusCode.OK,
                    new BaseResponse<MerchantResponse>
                    {
                        ResultCode = (int)MerchantStatus.MERCHANT_NOT_FOUND,
                        ResultMessage = MerchantStatus.MERCHANT_NOT_FOUND.ToString(),
                        Data = default
                    });
            }

            return new BaseResponse<MerchantResponse>
            {
                ResultCode = (int)CommonResponse.SUCCESS,
                ResultMessage = CommonResponse.SUCCESS.ToString(),
                Data = merchantResponse
            };
        }


        /// <summary>
        /// Get Merchant By Phone Number
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public async Task<BaseResponse<MerchantResponse>> GetMerchantByPhoneNumber(string number)
        {
            //biz rule

            MerchantResponse merchantResponse;

            //Get Merchant From Database
            try
            {
                Merchant merchant = await _unitOfWork.Repository<Merchant>()
                                                   .FindAsync(merchant => merchant.PhoneNumber.Equals(number));
                merchantResponse = _mapper.Map<MerchantResponse>(merchant);
            }
            catch (Exception e)
            {
                _logger.Error("[MerchantService.GetMerchantByPhoneNumber()]: " + e.Message);

                throw new HttpStatusException(HttpStatusCode.OK,
                    new BaseResponse<MerchantResponse>
                    {
                        ResultCode = (int)MerchantStatus.MERCHANT_NOT_FOUND,
                        ResultMessage = MerchantStatus.MERCHANT_NOT_FOUND.ToString(),
                        Data = default
                    });
            }

            return new BaseResponse<MerchantResponse>
            {
                ResultCode = (int)CommonResponse.SUCCESS,
                ResultMessage = CommonResponse.SUCCESS.ToString(),
                Data = merchantResponse
            };
        }


        /// <summary>
        /// Get Merchant By Account Id
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<BaseResponse<MerchantResponse>> GetMerchantByAccountId(string accountId)
        {
            MerchantResponse merchantResponse;

            //Get Merchant From Database

            try
            {
                Merchant merchants = await _unitOfWork.Repository<Merchant>()
                                        .FindAsync(merchant => merchant.AccountId.Equals(accountId));

                merchantResponse = _mapper.Map<MerchantResponse>(merchants);
            }
            catch (Exception e)
            {
                _logger.Error("[MarketManagerService.GetMerchantByAccountId()]: " + e.Message);

                throw new HttpStatusException(HttpStatusCode.OK,
                    new BaseResponse<MerchantResponse>
                    {
                        ResultCode = (int)MerchantStatus.MERCHANT_NOT_FOUND,
                        ResultMessage = MerchantStatus.MERCHANT_NOT_FOUND.ToString(),
                        Data = default
                    });
            }

            return new BaseResponse<MerchantResponse>
            {
                ResultCode = (int)CommonResponse.SUCCESS,
                ResultMessage = CommonResponse.SUCCESS.ToString(),
                Data = merchantResponse
            };
        }


        /// <summary>
        /// Get Merchants By Status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        /// <exception cref="HttpStatusException"></exception>
        public async Task<BaseResponse<List<MerchantResponse>>> GetMerchantsByStatus(int status)
        {
            List<MerchantResponse> merchantList = null;

            //get Merchant from database
            try
            {
                merchantList = _mapper.Map<List<MerchantResponse>>(
                    await _unitOfWork.Repository<Merchant>()
                                     .FindListAsync(merchant => merchant.Status == status));
            }
            catch (Exception e)
            {
                _logger.Error("[MerchantService.GetMerchantsByStatus()]: " + e.Message);

                throw new HttpStatusException(HttpStatusCode.OK,
                    new BaseResponse<MerchantResponse>
                    {
                        ResultCode = (int)MerchantStatus.MERCHANT_NOT_FOUND,
                        ResultMessage = MerchantStatus.MERCHANT_NOT_FOUND.ToString(),
                        Data = default
                    });
            }

            return new BaseResponse<List<MerchantResponse>>
            {
                ResultCode = (int)CommonResponse.SUCCESS,
                ResultMessage = CommonResponse.SUCCESS.ToString(),
                Data = merchantList
            };
        }

    }
}
