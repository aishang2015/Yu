namespace Yu.Model.WebAdmin.Entity.InputModels
{
    public class EntityGetQuery
    {
        // 页码
        public int PageIndex { get; set; }

        // 页面大小
        public int PageSize { get; set; }

        // 查询字符串
        public string SearchText { get; set; }
    }
}
