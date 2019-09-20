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

  // 此流程图对应的工作流
  wfDefine: WorkFlowDefine = new WorkFlowDefine();

  flowConnections: WorkflowFlowConnection[] = [];
  flowNodes: WorkflowFlowNode[] = [];

  constructor(private renderer: Renderer2,
    private modalService: NzModalService,
    private messageService: NzMessageService,
    private nzContextMenuService: NzContextMenuService,
    private routeInfo: ActivatedRoute,
    private _workflowDefineService: WorkFlowDefineService,
    private _workFlowFlowService: WorkFlowFlowService) { }

  ngOnInit() {

    // 取得工作流状态
    const id = this.routeInfo.snapshot.params['id'];
    this._workflowDefineService.getbyid(id).subscribe(result => {
      this.wfDefine = result;
      this.initFlowData();
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
    this._zoomController = pz.default(this._zoomContainer.nativeElement);
    this._zoomController.on('transform', (e) => {
      const transform = e.getTransform();
      this._jsPlumbInstance.setZoom(transform.scale);
    });
  }

  // 添加节点
  addNode() {
    console.log(this._jsPlumbInstance.getConnections());
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

  }

  // 删除节点
  deleteNode() {
    this._jsPlumbInstance.remove(this._rightClickElement);
  }

  // 确认选择
  confirm(value) {
    switch (value) {
      case 'startNode':
        this.addStartNode();
        break;
      case 'endNode':
        this.addEndNode();
        break;
      case 'workNode':
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

  //#region 添加节点操作

  // 工作节点
  addWorkNode(title: string, describe: string, id = `work-node-${Date.now().toString(36)}`, top = '200', left = '200') {

    // 渲染元素
    // 此处要写完整的值，render不会自动补全值（譬如不能设置0要设置为0px）
    let workNode = this.renderer.createElement("div");
    this.renderer.setStyle(workNode, "top", `${top}px`);
    this.renderer.setStyle(workNode, "left", `${left}px`);
    this.renderer.addClass(workNode, "node");
    this.renderer.addClass(workNode, "work-node");
    this.renderer.setAttribute(workNode, 'id', id);
    this.renderer.setAttribute(workNode, 'flownodetype', `workNode`);

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

    // 绑定右键菜单
    let that = this;
    this.renderer.listen(workNode, 'contextmenu', event => {
      that.nzContextMenuService.create(event, that._nzDropdownMenu);
      this._rightClickElement = workNode;
    });
    this.renderer.appendChild(this._diagram.nativeElement, workNode);

    // 设置元素拖拽
    this._jsPlumbInstance.draggable(workNode, {
      filter: '.work-node-describe',
      filterExclude: false,
    });

    // 配置为源
    this._jsPlumbInstance.makeSource(id, {
      anchor: 'Continuous',
      maxConnections: -1,
      allowLoopback: false,
      filter: (event, element) => {
        return event.target.classList.contains('work-node-title');
      }
    }, this.endpointOption);

    this._jsPlumbInstance.makeTarget(id, {
      anchor: 'Continuous',
      maxConnections: -1,
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
    this.renderer.setAttribute(startNode, 'flownodetype', `startNode`);

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
    this.renderer.setAttribute(endNode, 'flownodetype', `endNode`);

    let endNodeFlg = this.renderer.createElement("div");
    this.renderer.addClass(endNodeFlg, "end-node-flg");
    this.renderer.appendChild(endNode, endNodeFlg);

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
      maxConnections: 1,
      isSource: false,
      isTarget: true,
    }, this.endpointOption);

  }

  //#endregion 

  // 初始化流程图数据
  initFlowData() {
    this._workFlowFlowService.get(this.wfDefine.id).subscribe(result => {
      this.handleNodes(result.nodes);
      this.handleConnections(result.connections);
    })
  }

  // 处理节点数据
  handleNodes(nodes) {
    console.log(nodes);
    this.flowNodes = nodes;
    this.flowNodes.forEach(node => {
      switch (node.nodeType) {
        case 'startNode':
          this.addStartNode(node.top, node.left);
          break;
        case 'endNode':
          this.addEndNode(node.top, node.left);
          break;
        case 'workNode':
          this.addWorkNode("工作节点", "这是一个工作节点", node.nodeId, node.top, node.left);
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
    console.log(elements);
    for (let i = 0; i < elements.length; i++) {
      let element: any = elements[i];
      this.flowNodes.push({
        defineId: this.wfDefine.id,
        nodeId: element.id,
        nodeType: element.attributes.flownodetype.value,
        top: element.offsetTop,
        left: element.offsetLeft,
      })
    }

    this._workFlowFlowService.addOrUpdate({
      nodes: this.flowNodes,
      connections: this.flowConnections,
      defineId: this.wfDefine.id,
    }).subscribe(result => {
      this.messageService.success("保存成功！");
      this.isLoading = false;
    }, error => this.isLoading = false);


  }

}
