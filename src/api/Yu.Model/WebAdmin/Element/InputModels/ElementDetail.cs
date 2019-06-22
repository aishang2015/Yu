using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Model.WebAdmin.Element.InputModels
{
    public class ElementDetail
    {     
        // 元素Id
        public string Id { get; set; }

        // 上级元素Id
        public string UpId { get; set; }

        // 元素名称
        public string Name { get; set; }

        // 元素类型
        public int ElementType { get; set; }

        // 元素唯一标识
        public string Identification { get; set; }

        // 路由
        public string Route { get; set; }
    }
}
