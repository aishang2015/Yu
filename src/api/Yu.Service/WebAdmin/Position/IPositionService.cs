
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yu.Data.Entities;
using Yu.Data.Entities.WebAdmin;
 
namespace Yu.Service.WebAdmin.Positions
{

	public interface IPositionService
	{
        /// <summary>
        /// 取得数据
        /// </summary>
		PagedData<Position> GetPositions(int pageIndex, int pageSize, string searchText);

        /// <summary>
        /// 取得数据
        /// </summary>
		List<Position> GetAllPositions();

        /// <summary>
        /// 删除数据
        /// </summary>
        Task DeletePositionAsync(Guid id);

        /// <summary>
        /// 添加数据
        /// </summary>
        Task AddPositionAsync(Position entity);

        /// <summary>
        /// 更新数据
        /// </summary>
        Task UpdatePositionAsync(Position entity);

        /// <summary>
        /// 检查名称重复
        /// </summary>
        bool HaveRepeatName(Guid id, string positionName);

        /// <summary>
        /// 获取类型名称
        /// </summary>
        string GetPositionNameById(Guid id);

    }
}

