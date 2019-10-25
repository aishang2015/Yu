import { Component, OnInit, ViewChild, ElementRef, Renderer2 } from '@angular/core';
import * as jp from 'jsplumb';
import * as pz from 'panzoom';
import { NzModalService, NzDropdownMenuComponent, NzContextMenuService, NzMessageService } from 'ng-zorro-antd';
import { ActivatedRoute } from '@angular/router';
import { WorkFlowDefineService } from 'src/app/core/services/workflow/workflowdefine.service';
import { WorkFlowDefine } from '../models/workflowDefine';
import { WorkFlowFlowService } from 'src/app/core/services/workflow/workflowflow.service';
import { WorkflowFlowConnection } from '../models/workflowFlowConnection';
import { WorkflowFlowNode } from '../models/workflowFlowNode';
import { element } from 'protractor';
import { WorkflowFlowNodeInfo } from '../models/workflowFlowNodeInfo';
import { WorkFlowFormService } from 'src/app/core/services/workflow/workflowform.service';
import { WorkFlowFormElement } from '../models/workflowFormElement';
import { WorkflowFlowNodeElement } from '../models/workflowFlowNodeElement';
import { WorkflowFlowNodeHandle } from '../models/workflowFlowNodeHandle';
import { UserService } from 'src/app/core/services/rightmanage/user.service';
import { PositionService } from 'src/app/core/services/rightmanage/position.service';

@Component({
  selector: 'app-flow',
  templateUrl: './flow.component.html',
  styleUrls: ['./flow.component.scss']
})
export class FlowComponent implements OnInit {

  // 流程图
  @ViewChild('diagram', { static: true })
  private _diagram: ElementRef;

  // 缩放容器
  @ViewChild('zoomContainer', { static: true })
  private _zoomContainer: ElementRef;

  // 节点类型选择器
  @ViewChild('nodeSelectModal', { static: true })
  private _nodeSelectTemplate;

  // 右键菜单
  @ViewChild('menu', { static: true })
  private _nzDropdownMenu;

  // 节点属性编辑模态框
  @ViewChild('editTpl', { static: true })
  private _nodeEditTpl;

  // 工作流定义id
  private id;

  // 右键
  private _rightClickElement;

  // 缩放控制器
  private _zoomController;

  // 模态框
  private _modal;

  // 是否加载中
  isLoading: boolean = false;

  selected = '2';

  // 节点id下标
  private _index: number = 1;

  // jsplumb 对象
  private _jsPlumb = jp.jsPlumb;

  // jsplubm 实例
  private _jsPlumbInstance;

  // 端点选项
  private endpointOption = {
    maxConnections: 5,
    parameters: {},
    reattachConnections: true,
    type: 'Dot',
    connector: 'Flowchart',
    isSource: true,
    isTarget: true,
    paintStyle: { fill: 'transparent', stroke: 'transparent', radius: 5, strokeWidth: 1 },
    hoverPaintStyle: { fill: 'rgba(95, 158, 160, 1)', stroke: 'rgba(95, 158, 160, 1)', strokeWidth: 2 },
    connectorStyle: { stroke: 'rgba(102, 96, 255, 0.9)', strokeWidth: 3 },
    connectorHoverStyle: { strokeWidth: 4, cursor: 'pointer' },
  };

  // 用户列表
  userList = [];

  // 职位数据
  positions = [];

  // 此流程图对应的工作流
  private wfDefine: WorkFlowDefine = new WorkFlowDefine();

  // 流程图表单元素合集
  private wfFormElements: WorkFlowFormElement[] = [];

  // 流程图节点和表单元素信息
  private flowNodeElements: WorkflowFlowNodeElement[] = [];

  // 编辑节点信息
  nodeInfo: WorkflowFlowNodeInfo = new WorkflowFlowNodeInfo();

  // 工作流节点的基本信息
  private nodeInfos: WorkflowFlowNodeInfo[] = [];

  // 编辑中办理信息
  nodeHandle: WorkflowFlowNodeHandle = new WorkflowFlowNodeHandle();

  // 工作流节点办理人员信息
  private nodeHandles: WorkflowFlowNodeHandle[] = [];

  // 视图模型
  private flowConnections: WorkflowFlowConnection[] = [];
  private flowNodes: WorkflowFlowNode[] = [];

  constructor(private renderer: Renderer2,
    private modalService: NzModalService,
    private messageService: NzMessageService,
    private nzContextMenuService: NzContextMenuService,
    private routeInfo: ActivatedRoute,
    private _workflowDefineService: WorkFlowDefineService,
    private _workFlowFlowService: WorkFlowFlowService,
    private _workFlowFormService: WorkFlowFormService,
    private _userService: UserService,
    private _positionService: PositionService) { }

  ngOnInit() {

    // 取得工作流状态
    this.id = this.routeInfo.snapshot.params['id'];
    this._workflowDefineService.getbyid(this.id).subscribe(result => {
      this.wfDefine = result;
      this.initFlowData();
    });

    // 初始化表单元素集合
    this._workFlowFormService.getFormElement(this.id).subscribe(result => {
      this.wfFormElements = result;
    });

    // 初始化岗位信息
    this._positionService.getPositions().subscribe(result => {
      this.positions = result;
    });

    // 初始化jsplumb
    this._jsPlumbInstance = this._jsPlumb.getInstance({

      DragOptions: { cursor: 'move', zIndex: 2000 },
      ConnectionOverlays: [
        [
          'PlainArrow', {
            location: 1,
            visible: true,
            width: 10,
            length: 7,
            id: 'ARROW',
            events: {
              click() { }
            }
          }
        ],
        [
          'Label', {
            location: 0.1,
            id: 'label',
            cssClass: 'aLabel',
            events: {
              tap() { }
            }
          }
        ]
      ],
      Container: "diagramContainer"
    });

    // 点击删除连接
    this._jsPlumbInstance.bind('click', (conn, orignalEvent) => {
      this._jsPlumbInstance.deleteConnection(conn);
    });

    // 初始化缩放区域
    this._zoomController = pz.default(this._zoomContainer.nativeElement,
      {
        zoomDoubleClickSpeed: 1,
      });
    this._zoomController.on('transform', (e) => {
      const transform = e.getTransform();
      this._jsPlumbInstance.setZoom(transform.scale);
    });
  }

  // 添加节点
  addNode() {
    this._modal = this.modalService.create({
      nzTitle: null,
      nzContent: this._nodeSelectTemplate,
      nzFooter: null,
      nzClosable: false,
      nzMaskClosable: false
    });
  }

  //编辑节点
  editNode() {
    let id = this._rightClickElement.id;
    this.nodeInfo = this.nodeInfos.find(n => n.nodeId == id);
    this.nodeHandle = this.nodeHandles.find(n => n.nodeId == id);
    this._userService.getUserOutlinesById(this.nodeHandle.handlePeople.join(',')).subscribe(
      result => this.userList = result
    );
    this._modal = this.modalService.create({
      nzTitle: null,
      nzContent: this._nodeEditTpl,
      nzFooter: null,
      nzClosable: false,
      nzMaskClosable: false
    });
  }

  // 删除节点
  deleteNode() {
    this._jsPlumbInstance.remove(this._rightClickElement);
  }

  // 确认选择
  confirm(value) {
    switch (value) {
      case '0':
        this.addStartNode();
        break;
      case '99':
        this.addEndNode();
        break;
      case '1':
        this.addWorkNode("工作节点", "这是一个工作节点");
        break;
    }
    this._modal.close();
    this._modal = null;
  }

  // 取消选择
  cancel() {
    this._modal.close();
    this._modal = null;
  }

  // 放大
  zoomInClick() {
    this._zoomController.smoothZoom(20, 20, 1.25);
  }
  // 缩小
  zoomOutClick() {
    this._zoomController.smoothZoom(20, 20, 0.8);
  }
  // 适应
  fitClick() {
    this._zoomController.moveTo(0, 0);
    this._zoomController.zoomAbs(0, 0, 1);
  }

  //#region 添加节点操作合集

  // 工作节点
  addWorkNode(title, describe, id = `work-node-${Date.now().toString(36)}`, top = '200', left = '200', people = []) {

    // 渲染元素
    // 此处要写完整的值，render不会自动补全值（譬如不能设置0要设置为0px）
    let workNode = this.renderer.createElement("div");
    this.renderer.setStyle(workNode, "top", `${top}px`);
    this.renderer.setStyle(workNode, "left", `${left}px`);
    this.renderer.addClass(workNode, "node");
    this.renderer.addClass(workNode, "work-node");
    this.renderer.setAttribute(workNode, 'id', id);
    this.renderer.setAttribute(workNode, 'flownodetype', `1`);

    let workNodeTitle = this.renderer.createElement("div");
    this.renderer.addClass(workNodeTitle, "work-node-title");
    let titleEl = this.renderer.createText(title);
    this.renderer.appendChild(workNodeTitle, titleEl);
    this.renderer.appendChild(workNode, workNodeTitle);

    let workNodeDescribe = this.renderer.createElement("div");
    this.renderer.addClass(workNodeDescribe, "work-node-describe");
    let describeEl = this.renderer.createText(describe);
    this.renderer.appendChild(workNodeDescribe, describeEl);
    this.renderer.appendChild(workNode, workNodeDescribe);

    // 节点基本信息
    this.nodeInfos.push({ nodeId: id, name: '工作节点', describe: '这是一个工作节点' });

    // 节点人员信息
    this.nodeHandles.push({ nodeId: id, handleType: 1, handlePeople: people, positionGroup: 1, positionId: null });

    // 绑定右键菜单
    let that = this;
    this.renderer.listen(workNode, 'contextmenu', event => {
      that.nzContextMenuService.create(event, that._nzDropdownMenu);
      this._rightClickElement = workNode;
    });
    this.renderer.appendChild(this._diagram.nativeElement, workNode);

    // 绑定双击
    this.renderer.listen(workNode, 'dblclick', event => {
      that.nodeInfo = that.nodeInfos.find(n => n.nodeId == id);
      that.nodeHandle = that.nodeHandles.find(n => n.nodeId == id);
      that._userService.getUserOutlinesById(that.nodeHandle.handlePeople.join(',')).subscribe(
        result => that.userList = result
      );
      that._modal = that.modalService.create({
        nzTitle: null,
        nzContent: that._nodeEditTpl,
        nzFooter: null,
        nzClosable: false,
        nzMaskClosable: false
      });
    });

    // 设置元素拖拽
    this._jsPlumbInstance.draggable(workNode, {
      filter: '.work-node-describe',
      filterExclude: false,
    });

    // 配置为源
    this._jsPlumbInstance.makeSource(id, {
      anchor: 'Continuous',
      maxConnections: 1,
      allowLoopback: false,
      filter: (event, element) => {
        return event.target.classList.contains('work-node-title');
      }
    }, this.endpointOption);

    this._jsPlumbInstance.makeTarget(id, {
      anchor: 'Continuous',
      maxConnections: 1,
      allowLoopback: false,
      filter: (event, element) => {
        return event.target.classList.contains('work-node-title');
      }
    }, this.endpointOption);
  }

  // 开始节点
  addStartNode(top = '200', left = '200') {

    let startNode = this.renderer.createElement("div");
    this.renderer.setStyle(startNode, "top", `${top}px`);
    this.renderer.setStyle(startNode, "left", `${left}px`);
    this.renderer.addClass(startNode, "node");
    this.renderer.addClass(startNode, "start-node");
    this.renderer.setAttribute(startNode, 'id', `start-node`);
    this.renderer.setAttribute(startNode, 'flownodetype', `0`);

    let startNodeFlg = this.renderer.createElement("div");
    this.renderer.addClass(startNodeFlg, "start-node-flg");

    this.renderer.appendChild(startNode, startNodeFlg);

    // 绑定右键菜单
    let that = this;
    this.renderer.listen(startNode, 'contextmenu', event => {
      that.nzContextMenuService.create(event, that._nzDropdownMenu);
      this._rightClickElement = startNode;
    });
    this.renderer.appendChild(this._diagram.nativeElement, startNode);
    
    // 节点基本信息
    this.nodeInfos.push({ nodeId: 'start-node', name: '起始节点', describe: '' });

    // 设置元素拖拽
    this._jsPlumbInstance.draggable(startNode, {
      filter: '.start-node',
    });

    // 配置为源
    this._jsPlumbInstance.makeSource(`start-node`, {
      anchor: 'Continuous',
      isTarget: false,
      maxConnections: 1,
      allowLoopback: false,
      filter: (event, element) => {
        return event.target.classList.contains('start-node-flg');
      }
    }, this.endpointOption);
  }

  // 结束节点
  addEndNode(top = '200', left = '200') {
    let endNode = this.renderer.createElement("div");
    this.renderer.setStyle(endNode, "top", `${top}px`);
    this.renderer.setStyle(endNode, "left", `${left}px`);
    this.renderer.addClass(endNode, "node");
    this.renderer.addClass(endNode, "end-node");
    this.renderer.setAttribute(endNode, 'id', `end-node`);
    this.renderer.setAttribute(endNode, 'flownodetype', `99`);

    let endNodeFlg = this.renderer.createElement("div");
    this.renderer.addClass(endNodeFlg, "end-node-flg");
    this.renderer.appendChild(endNode, endNodeFlg);
    
    // 节点基本信息
    this.nodeInfos.push({ nodeId: 'end-node', name: '结束节点', describe: '' });

    // 绑定右键菜单
    let that = this;
    this.renderer.listen(endNode, 'contextmenu', event => {
      that.nzContextMenuService.create(event, that._nzDropdownMenu);
      this._rightClickElement = endNode;
    });
    this.renderer.appendChild(this._diagram.nativeElement, endNode);

    this._jsPlumbInstance.draggable(endNode, {
      filter: '.end-node',
    });

    this._jsPlumbInstance.makeTarget(endNode, {
      anchor: 'Continuous',
      maxConnections: 1,
      isSource: false,
      isTarget: true,
    }, this.endpointOption);

  }

  //#endregion 

  //#region 初始化操作合集
  // 初始化流程图数据
  initFlowData() {
    this._workFlowFlowService.get(this.wfDefine.id).subscribe(result => {
      this.handleNodes(result.nodes);
      this.handleConnections(result.connections);
      this.handleNodeElements(result.nodeElements);
    })
  }

  // 处理节点数据
  handleNodes(nodes) {
    this.flowNodes = nodes;
    this.flowNodes.forEach(node => {
      switch (node.nodeType.toString()) {
        case '0':
          this.addStartNode(node.top, node.left);
          break;
        case '99':
          this.addEndNode(node.top, node.left);
          break;
        case '1':
          this.addWorkNode(node.name, node.describe, node.nodeId, node.top, node.left, node.handlePeoples.split(','));
          break;
      }
    });
  }

  // 处理连接数据
  handleConnections(connections) {
    this.flowConnections = connections;
    this.flowConnections.forEach(connection => {
      this._jsPlumbInstance.connect({
        source: connection.sourceId,
        target: connection.targetId,
        anchor: 'Continuous'
      });
    });
  }

  // 处理节点元素关联设置
  handleNodeElements(nodeElements) {
    this.flowNodeElements = nodeElements;
  }
  //#endregion

  // 取得节点元素关联设置对象
  getNodeElement(nodeid, elementid) {
    let nodeElement = this.flowNodeElements.find(wffne => wffne.flowNodeId == nodeid && wffne.formElementId == elementid);
    if (!nodeElement) {
      this.flowNodeElements.push({
        defineId: this.id,
        flowNodeId: nodeid,
        formElementId: elementid,
        isVisible: true,
        isEditable: true
      })
      nodeElement = this.flowNodeElements.find(wffne => wffne.flowNodeId == nodeid && wffne.formElementId == elementid);
    }
    return nodeElement;
  }

  // 取得表单元素类型
  getElementName(key) {
    return this._workFlowFormService.components.find(c => c.key == key).describe;
  }

  // 保存流程图
  saveFlow() {
    this.isLoading = true;
    this.flowConnections = [];
    let connections = this._jsPlumbInstance.getAllConnections();
    connections.forEach(element => {
      this.flowConnections.push({
        defineId: this.wfDefine.id,
        sourceId: element.sourceId,
        targetId: element.targetId,
      });
    });

    this.flowNodes = [];
    let elements = document.getElementsByClassName('node');
    for (let i = 0; i < elements.length; i++) {
      let element: any = elements[i];
      let nodeInfo = this.nodeInfos.find(info => info.nodeId == element.id);
      let nodeHandle = this.nodeHandles.find(info => info.nodeId == element.id);      
      let nodeType = element.attributes.flownodetype.value;
      this.flowNodes.push({
        defineId: this.wfDefine.id,
        nodeId: element.id,
        nodeType: nodeType,
        top: element.offsetTop,
        left: element.offsetLeft,
        name: nodeInfo ? nodeInfo.name : '',
        describe: nodeInfo ? nodeInfo.describe : '',
        handleType: nodeHandle ? nodeHandle.handleType : 1,
        handlePeoples: nodeHandle ? nodeHandle.handlePeople.join(',') : '',
        positionId: nodeHandle ? nodeHandle.positionId : '',
        positionGroup: nodeHandle ? nodeHandle.positionGroup : 1
      })
    }

    this._workFlowFlowService.addOrUpdate({
      nodes: this.flowNodes,
      connections: this.flowConnections,
      nodeElements: this.flowNodeElements,
      defineId: this.wfDefine.id,
    }).subscribe(result => {
      this.messageService.success("保存成功！");
      this.isLoading = false;
    }, error => this.isLoading = false);


  }

  // 提交数据
  basicInfoSubmit(form) {
    let element = document.getElementById(this.nodeInfo.nodeId);

    // 设置名称
    let titleElement = element.getElementsByClassName('work-node-title')[0];
    if (titleElement) {
      titleElement.innerHTML = this.nodeInfo.name;
    }

    // 设置描述
    let describeElement = element.getElementsByClassName('work-node-describe')[0];
    if (describeElement) {
      describeElement.innerHTML = this.nodeInfo.describe;
    }
    this._modal.close();

    this.saveFlow();
  }

  saveChange() {
    this._modal.close();
    this._modal = null;
    this.saveFlow();
  }

  // 搜索人员
  searchPeople(value) {
    this._userService.getUserOutlines(1, 100, value).subscribe(
      result => {
        this.userList = result.data;
      }
    );
  }

  // 取得显示内容
  getUserLabel(user) {
    return `${user.fullName}:${user.groupName}:${user.positionName}(${user.userName})`;
  }

}
