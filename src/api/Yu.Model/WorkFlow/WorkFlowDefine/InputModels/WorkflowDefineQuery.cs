namespace Yu.Model.WorkFlow.WorkFlowDefine.InputModels
{
    public class WorkflowDefineQuery
    {
        // 页码
        public int PageIndex { get; set; }

        // 页面大小
        public int PageSize { get; set; }

        // TypeId
        public string TypeId { get; set; }
    }
}
