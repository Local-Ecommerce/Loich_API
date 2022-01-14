using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DAL.Repositories
{
    public class SystemCategoryRepository : Repository<SystemCategory>, ISystemCategoryRepository
    {
        public SystemCategoryRepository(LoichDBContext context) : base(context) { }

        private const int THREE = 3;

        /// <summary>
        /// Get All Level On eAnd Two System Category
        /// </summary>
        /// <returns></returns>
        public async Task<List<SystemCategory>> GetAllLevelOneAndTwoSystemCategory()
        {
            List<SystemCategory> systemCategories = await _context.SystemCategories
                                                            .Where(sc => sc.CategoryLevel != THREE)
                                                            .OrderBy(sc => sc.CategoryLevel)
                                                            .ToListAsync();
            return systemCategories;
        }


        /// <summary>
        /// Get All System Category Include Inverse Belong To
        /// </summary>
        /// <returns></returns>
        public async Task<List<SystemCategory>> GetAllSystemCategoryIncludeInverseBelongTo()
        {
            List<SystemCategory> systemCategories = await _context.SystemCategories
                                                            .Where(sc => sc.BelongTo == null)
                                                            .Include(sc => sc.InverseBelongToNavigation)
                                                            .ThenInclude(sc => sc.InverseBelongToNavigation)
                                                            .ToListAsync();

            return systemCategories;
        }
    }
}