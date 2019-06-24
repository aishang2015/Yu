namespace Yu.Model.WebAdmin.Group.InputModels
{
    public class GroupDetail
    {
        // 组织Id
        public string Id { get; set; }

        // 上级组织Id
        public string UpId { get; set; }

        // 组织名称
        public string GroupName { get; set; }

        // 备注
        public string Remark { get; set; }
    }
}
