﻿using AutoMapper;
using DAL.Constants;
using BLL.Dtos.Exception;
using BLL.Dtos.News;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class NewsService : INewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUtilService _utilService;
        private const string PREFIX = "NS_";

        public NewsService(IUnitOfWork unitOfWork,
            ILogger logger,
            IMapper mapper,
            IUtilService utilService
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _utilService = utilService;
        }

        /// <summary>
        /// Create news
        /// </summary>
        /// <param name="newsRequest"></param>
        /// <returns></returns>
        public async Task<NewsResponse> CreateNews(NewsRequest newsRequest)
        {
            News news = _mapper.Map<News>(newsRequest);

            try
            {
                news.NewsId = _utilService.CreateId(PREFIX);
                news.ReleaseDate = DateTime.Now;
                news.Status = (int)NewsStatus.ACTIVE_NEWS;

                _unitOfWork.News.Add(news);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Error("[NewsService.CreateNews()]: " + e.Message);

                throw;
            }

            return _mapper.Map<NewsResponse>(news);
        }


        /// <summary>
        ///  Update News By Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newsUpdateRequest"></param>
        /// <returns></returns>
        public async Task<NewsResponse> UpdateNewsById(string id, NewsUpdateRequest newsUpdateRequest)
        {
            News news;
            try
            {
                news = await _unitOfWork.News.FindAsync(local => local.NewsId.Equals(id));
            }
            catch (Exception e)
            {
                _logger.Error("[NewsService.UpdateNewsById()]: " + e.Message);

                throw new EntityNotFoundException(typeof(News), id);
            }

            //Update News to DB
            try
            {
                news = _mapper.Map(newsUpdateRequest, news);

                _unitOfWork.News.Update(news);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Error("[NewsService.UpdateNewsById()]: " + e.Message);

                throw;
            }

            return _mapper.Map<NewsResponse>(news);
        }


        /// <summary>
        /// Delete News
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<NewsResponse> DeleteNewsById(string id)
        {
            //Check id
            News news;
            try
            {
                news = await _unitOfWork.News.FindAsync(local => local.NewsId.Equals(id));
            }
            catch (Exception e)
            {
                _logger.Error("[NewsService.DeleteNewsById()]: " + e.Message);

                throw new EntityNotFoundException(typeof(News), id);
            }

            //Delete News
            try
            {
                news.Status = (int)NewsStatus.INACTIVE_NEWS;

                _unitOfWork.News.Update(news);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Error("[NewsService.DeleteNewsById()]: " + e.Message);

                throw;
            }

            return _mapper.Map<ExtendNewsResponse>(news);
        }


        /// <summary>
        /// Get News
        /// </summary>
        /// <param name="id"></param>
        /// <param name="apartmentId"></param>
        /// <param name="date"></param>
        /// <param name="search"></param>
        /// <param name="status"></param>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <param name="sort"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public async Task<object> GetNews(
            string id, string apartmentId,
            DateTime date, string search, int?[] status,
            int? limit, int? page,
            string sort, string[] include)
        {
            PagingModel<News> news;
            string propertyName = default;
            bool isAsc = false;

            if (!string.IsNullOrEmpty(sort))
            {
                isAsc = sort[0].ToString().Equals("+");
                propertyName = _utilService.UpperCaseFirstLetter(sort[1..]);
            }
            for (int i = 0; i < include.Length; i++)
            {
                include[i] = !string.IsNullOrEmpty(include[i]) ? _utilService.UpperCaseFirstLetter(include[i]) : null;
            }

            try
            {
                news = await _unitOfWork.News
                        .GetNews(id, apartmentId, date, search, status, limit, page, isAsc, propertyName, include);

                if (_utilService.IsNullOrEmpty(news.List))
                    throw new EntityNotFoundException(typeof(Menu), "in the url");
            }
            catch (Exception e)
            {
                _logger.Error("[NewsService.GetNews()]" + e.Message);
                throw;
            }

            return new PagingModel<ExtendNewsResponse>
            {
                List = _mapper.Map<List<ExtendNewsResponse>>(news.List),
                Page = news.Page,
                LastPage = news.LastPage,
                Total = news.Total,
            };
        }
    }
}
