﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yu.Model.WebAdmin.Element.InputModels;
using Yu.Model.WebAdmin.Element.OutputModels;

namespace Yu.Service.WebAdmin.Element
{
    public interface IElementService
    {
        /// <summary>
        /// 取得所有元素
        /// </summary>
        IEnumerable<ElementResult> GetAllElement();

        /// <summary>
        /// 删除元素
        /// </summary>
        /// <param name="elementId">元素ID</param>
        Task DeleteElement(Guid elementId);

        /// <summary>
        /// 创建新元素
        /// </summary>
        /// <param name="elementDetail">元素内容</param>
        Task CreateElement(ElementDetail elementDetail);

        /// <summary>
        /// 更新元素
        /// </summary>
        /// <param name="elementDetail">元素内容</param>
        Task UpdateElement(ElementDetail elementDetail);
    }
}
