using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Model.WebAdmin.Api.InputModels
{
    public class ApiQuery
    {
        // 页码
        public int PageIndex { get; set; }

        // 页面大小
        public int PageSize { get; set; }

        // 查询字符串
        public string SearchText { get; set; }
    }
}
