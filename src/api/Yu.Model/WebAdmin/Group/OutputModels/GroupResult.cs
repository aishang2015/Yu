namespace Yu.Model.WebAdmin.Group.OutputModels
{
    public class GroupResult
    {
        // 元素Id
        public string Id { get; set; }

        // 上级元素Id
        public string UpId { get; set; }

        // 元素名称
        public string GroupName { get; set; }

        // 备注
        public string Remark { get; set; }
    }
}
