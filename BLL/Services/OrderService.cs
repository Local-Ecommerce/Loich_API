﻿using AutoMapper;
using BLL.Constants;
using BLL.Dtos;
using BLL.Dtos.Exception;
using BLL.Dtos.Order;
using BLL.Dtos.OrderDetail;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisService _redisService;
        private readonly IMapper _mapper;
        private readonly IUtilService _utilService;
        private const string PREFIX = "OD_";
        private const string SUB_PREFIX = "ODD_";
        private const string CACHE_KEY = "Order";


        public OrderService(ILogger logger,
            IMapper mapper,
            IUtilService utilService,
            IRedisService redisService,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _mapper = mapper;
            _utilService = utilService;
            _unitOfWork = unitOfWork;
            _redisService = redisService;
        }


        /// <summary>
        /// CreateOrder
        /// </summary>
        /// <param name="orderDetailRequests"></param>
        /// <param name="residentId"></param>
        /// <returns></returns>
        public async Task<BaseResponse<List<OrderResponse>>> CreateOrder(List<OrderDetailRequest> orderDetailRequests, string residentId)
        {
            List<OrderResponse> orderResponses = new List<OrderResponse>();

            try
            {
                //create new  orders and order details
                foreach (OrderDetailRequest orderDetailRequest in orderDetailRequests)
                {
                    string orderId = _utilService.CreateId(PREFIX);

                    //Create order Detail
                    OrderDetail orderDetail = _mapper.Map<OrderDetail>(orderDetailRequest);
                    orderDetail.OrderDetailId = _utilService.CreateId(SUB_PREFIX);
                    orderDetail.OrderId = orderId;
                    orderDetail.FinalAmount = CaculateOrderDetailFinalAmount(orderDetail.UnitPrice, orderDetail.Quantity, orderDetail.Discount);
                    orderDetail.OrderDate = DateTime.Now;
                    orderDetail.Status = orderDetailRequest.Status;

                    //create order
                    Order order = new Order
                    {
                        OrderId = orderId,
                        DeliveryAddress = "",
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        TotalAmount = CaculateOrderTotalAmount(orderDetail),
                        Status = orderDetail.Status,
                        Discount = orderDetail.Discount,
                        ResidentId = residentId,
                        MerchantStoreId = orderDetailRequest.MerchantStoreId,
                    };

                    //add to db
                    _unitOfWork.Orders.Add(order);
                    _unitOfWork.OrderDetails.Add(orderDetail);

                    //map to response
                    OrderResponse orderResponse = _mapper.Map<OrderResponse>(order);
                    orderResponse.OrderDetails = new Collection<OrderDetailResponse>();
                    orderResponse.OrderDetails.Add(_mapper.Map<OrderDetailResponse>(orderDetail));
                    orderResponses.Add(orderResponse);

                }

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Error("[OrderService.CreateOrder()]: " + e.Message);

                throw new HttpStatusException(HttpStatusCode.OK,
                    new BaseResponse<OrderResponse>
                    {
                        ResultCode = (int)OrderStatus.ORDER_NOT_FOUND,
                        ResultMessage = OrderStatus.ORDER_NOT_FOUND.ToString(),
                        Data = default
                    });
            }

            return new BaseResponse<List<OrderResponse>>
            {
                ResultCode = (int)CommonResponse.SUCCESS,
                ResultMessage = CommonResponse.SUCCESS.ToString(),
                Data = orderResponses
            };
        }


        /// <summary>
        /// Get Order By Resident Id And Status
        /// </summary>
        /// <param name="residentId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<BaseResponse<List<OrderResponse>>> GetOrderByResidentIdAndStatus(string residentId, int status)
        {
            List<OrderResponse> orderResponses;
            try
            {
                orderResponses = _mapper.Map<List<OrderResponse>>(
                    await _unitOfWork.Orders.GetOrderByResidentIdAndStatus(residentId, status)
                );
            }
            catch (Exception e)
            {
                _logger.Error("[OrderService.GetOrderByResidentIdAndStatus()]: " + e.Message);

                throw new HttpStatusException(HttpStatusCode.OK,
                    new BaseResponse<OrderResponse>
                    {
                        ResultCode = (int)OrderStatus.ORDER_NOT_FOUND,
                        ResultMessage = OrderStatus.ORDER_NOT_FOUND.ToString(),
                        Data = default
                    });
            }

            return new BaseResponse<List<OrderResponse>>
            {
                ResultCode = (int)CommonResponse.SUCCESS,
                ResultMessage = CommonResponse.SUCCESS.ToString(),
                Data = orderResponses
            };
        }


        /// <summary>
        /// Get Order By Merchant Store Id
        /// </summary>
        /// <param name="merchantStoreId"></param>
        /// <returns></returns>
        public async Task<BaseResponse<List<OrderResponse>>> GetOrderByMerchantStoreId(string merchantStoreId)
        {
            List<OrderResponse> orderResponses;
            try
            {
                orderResponses = _mapper.Map<List<OrderResponse>>(
                    await _unitOfWork.Orders.GetOrdersByMerchantStoreId(merchantStoreId)
                );
            }
            catch (Exception e)
            {
                _logger.Error("[OrderService.GetOrderByMerchantStoreId()]: " + e.Message);

                throw new HttpStatusException(HttpStatusCode.OK,
                    new BaseResponse<OrderResponse>
                    {
                        ResultCode = (int)OrderStatus.ORDER_NOT_FOUND,
                        ResultMessage = OrderStatus.ORDER_NOT_FOUND.ToString(),
                        Data = default
                    });
            }

            return new BaseResponse<List<OrderResponse>>
            {
                ResultCode = (int)CommonResponse.SUCCESS,
                ResultMessage = CommonResponse.SUCCESS.ToString(),
                Data = orderResponses
            };
        }


        /// <summary>
        /// Delete Order By Order Id And Resident Id
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="residentId"></param>
        /// <returns></returns>
        public async Task<BaseResponse<OrderResponse>> DeleteOrderByOrderIdAndResidentId(string orderId, string residentId)
        {
            Order order;
            try
            {
                order = await _unitOfWork.Orders.GetOrderByOrderIdAndResidentId(orderId, residentId);
                order.Status = (int)OrderStatus.DELETED_ORDER;

                OrderDetail orderDetail = order.OrderDetails.FirstOrDefault();
                orderDetail.Status = (int)OrderStatus.DELETED_ORDER;

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Error("[OrderService.DeleteOrderByOrderIdAndResidentId()]: " + e.Message);

                throw new HttpStatusException(HttpStatusCode.OK,
                    new BaseResponse<OrderResponse>
                    {
                        ResultCode = (int)OrderStatus.ORDER_NOT_FOUND,
                        ResultMessage = OrderStatus.ORDER_NOT_FOUND.ToString(),
                        Data = default
                    });
            }

            //create response
            OrderResponse orderResponse = _mapper.Map<OrderResponse>(order);

            return new BaseResponse<OrderResponse>
            {
                ResultCode = (int)CommonResponse.SUCCESS,
                ResultMessage = CommonResponse.SUCCESS.ToString(),
                Data = orderResponse
            };
        }

        /// <summary>
        /// Caculate Final Amount
        /// </summary>
        /// <param name="price"></param>
        /// <param name="quantity"></param>
        /// <param name="discount"></param>
        /// <returns></returns>
        public double? CaculateOrderDetailFinalAmount(double? price, int? quantity, double? discount)
        {
            return price - price * discount;
        }


        /// <summary>
        /// Caculate Order Total Amount
        /// </summary>
        /// <param name="orderDetail"></param>
        /// <returns></returns>
        public double? CaculateOrderTotalAmount(OrderDetail orderDetail)
        {
            return orderDetail.FinalAmount * orderDetail.Quantity;
        }


    }
}