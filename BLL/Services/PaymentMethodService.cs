﻿using AutoMapper;
using DAL.Constants;
using BLL.Dtos.Exception;
using BLL.Dtos.PaymentMethod;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUtilService _utilService;
        private const string PREFIX = "PMM_";

        public PaymentMethodService(IUnitOfWork unitOfWork,
            ILogger logger,
            IMapper mapper,
            IUtilService utilService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _utilService = utilService;
        }


        /// <summary>
        /// Create Payment Method
        /// </summary>
        /// <param name="paymentMethodRequest"></param>
        /// <returns></returns>
        public async Task<PaymentMethodResponse> CreatePaymentMethod(PaymentMethodRequest paymentMethodRequest)
        {
            //biz rule

            //Store PaymentMethod To Dabatabase
            PaymentMethod paymentMethod = _mapper.Map<PaymentMethod>(paymentMethodRequest);

            try
            {
                paymentMethod.PaymentMethodId = _utilService.CreateId(PREFIX);
                paymentMethod.Status = (int)PaymentMethodStatus.ACTIVE_PAYMENT_METHOD;

                _unitOfWork.PaymentMethods.Add(paymentMethod);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Error("[PaymentMethodService.CreatePaymentMethod()]: " + e.Message);

                throw;
            }

            return _mapper.Map<PaymentMethodResponse>(paymentMethod);
        }


        /// <summary>
        /// Delete Payment Method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PaymentMethodResponse> DeletePaymentMethod(string id)
        {
            //biz rule

            //Check id
            PaymentMethod paymentMethod;
            try
            {
                paymentMethod = await _unitOfWork.PaymentMethods.FindAsync(pmm => pmm.PaymentMethodId.Equals(id));
            }
            catch (Exception e)
            {
                _logger.Error("[PaymentMethodService.DeletePaymentMethod()]: " + e.Message);

                throw new EntityNotFoundException(typeof(PaymentMethod), id);
            }

            //Delete PaymentMethod
            try
            {
                paymentMethod.Status = (int)PaymentMethodStatus.DELETED_PAYMENT_METHOD;

                _unitOfWork.PaymentMethods.Update(paymentMethod);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Error("[PaymentMethodService.DeletePaymentMethod()]: " + e.Message);

                throw;
            }

            return _mapper.Map<PaymentMethodResponse>(paymentMethod);
        }


        /// <summary>
        /// Get Payment Method By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PaymentMethodResponse> GetPaymentMethodById(string id)
        {
            //biz rule


            PaymentMethodResponse paymentMethodResponse;

            //Get PaymentMethod From Database

            try
            {
                PaymentMethod paymentMethod = await _unitOfWork.PaymentMethods.FindAsync(pmm => pmm.PaymentMethodId.Equals(id));

                paymentMethodResponse = _mapper.Map<PaymentMethodResponse>(paymentMethod);
            }
            catch (Exception e)
            {
                _logger.Error("[PaymentMethodService.GetPaymentMethodById()]: " + e.Message);

                throw new EntityNotFoundException(typeof(PaymentMethod), id);
            }

            return paymentMethodResponse;
        }


        /// <summary>
        /// Get Payment Method By Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<List<PaymentMethodResponse>> GetAllPaymentMethod()
        {
            //biz rule


            List<PaymentMethodResponse> paymentMethodResponses;

            //Get All PaymentMethod From Database

            try
            {
                paymentMethodResponses = _mapper.Map<List<PaymentMethodResponse>>(
                    await _unitOfWork.PaymentMethods.FindListAsync(pmm => pmm.PaymentMethodId != null));
            }
            catch (Exception e)
            {
                _logger.Error("[PaymentMethodService.GetAllPaymentMethod()]: " + e.Message);

                throw new EntityNotFoundException(typeof(PaymentMethod), "all");
            }

            return paymentMethodResponses;
        }


        /// <summary>
        /// Update Payment Method By Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="paymentMethodRequest"></param>
        /// <returns></returns>
        public async Task<PaymentMethodResponse> UpdatePaymentMethodById(string id, PaymentMethodRequest paymentMethodRequest)
        {
            //biz ruie

            //Check id
            PaymentMethod paymentMethod;
            try
            {
                paymentMethod = await _unitOfWork.PaymentMethods.FindAsync(pmm => pmm.PaymentMethodId.Equals(id));
            }
            catch (Exception e)
            {
                _logger.Error("[PaymentMethodService.UpdatePaymentMethodById()]: " + e.Message);

                throw new EntityNotFoundException(typeof(PaymentMethod), id);
            }

            //Update PaymentMethod To DB
            try
            {
                paymentMethod = _mapper.Map(paymentMethodRequest, paymentMethod);
                paymentMethod.Status = (int)PaymentMethodStatus.ACTIVE_PAYMENT_METHOD;

                _unitOfWork.PaymentMethods.Update(paymentMethod);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Error("[PaymentMethodService.UpdatePaymentMethodById()]: " + e.Message);

                throw;
            }

            return _mapper.Map<PaymentMethodResponse>(paymentMethod);
        }
    }
}
