export class WorkflowFlowNodeHandle {
    nodeId: string;                 // 节点ID

    handleType: number = 1;         // 办理方式类型
    handlePeople: string[] = [];    // 办理人id
    positionId: string;              // 岗位id
    positionGroup: number = 1;      // 岗位部门
}