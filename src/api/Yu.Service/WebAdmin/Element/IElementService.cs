using System;
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
        Task DeleteElementAsync(Guid elementId);

        /// <summary>
        /// 创建新元素
        /// </summary>
        /// <param name="elementDetail">元素内容</param>
        Task CreateElementAsync(ElementDetail elementDetail);

        /// <summary>
        /// 更新元素
        /// </summary>
        /// <param name="elementDetail">元素内容</param>
        Task UpdateElementAsync(ElementDetail elementDetail);


        /// <summary>
        /// 检查元素唯一识别（创建元素）
        /// </summary>
        List<string> HaveSameIdentification(string identification);

        /// <summary>
        /// 检查元素唯一识别（更新元素）
        /// </summary>
        List<string> HaveSameIdentification(Guid elementId, string identification);
    }
}
