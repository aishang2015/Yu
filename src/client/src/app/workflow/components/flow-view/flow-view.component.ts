import { Component, OnInit, ViewChild, ElementRef, Renderer2 } from '@angular/core';
import { NzModalService, NzContextMenuService, NzMessageService, NzModalRef } from 'ng-zorro-antd';
import { ActivatedRoute } from '@angular/router';
import { WorkFlowDefineService } from 'src/app/core/services/workflow/workflowdefine.service';
import { WorkFlowFlowService } from 'src/app/core/services/workflow/workflowflow.service';
import { WorkFlowFormService } from 'src/app/core/services/workflow/workflowform.service';
import { UserService } from 'src/app/core/services/rightmanage/user.service';
import { PositionService } from 'src/app/core/services/rightmanage/position.service';
import { WorkFlowDefine } from '../../models/workflowDefine';
import * as jp from 'jsplumb';
import * as pz from 'panzoom';
import { WorkflowFlowConnection } from '../../models/workflowFlowConnection';
import { WorkflowFlowNode } from '../../models/workflowFlowNode';
import { WorkflowFlowNodeElement } from '../../models/workflowFlowNodeElement';
import { WorkflowFlowNodeInfo } from '../../models/workflowFlowNodeInfo';
import { WorkflowFlowNodeHandle } from '../../models/workflowFlowNodeHandle';

@Component({
  selector: 'app-flow-view',
  templateUrl: './flow-view.component.html',
  styleUrls: ['./flow-view.component.scss']
})
export class FlowViewComponent implements OnInit {

  // 流程图
  @ViewChild('diagram', { static: true })
  private _diagram: ElementRef;

  // 缩放容器
  @ViewChild('zoomContainer', { static: true })
  private _zoomContainer: ElementRef;

  @ViewChild('handlePeopleModal', { static: true })
  private _handlePeopleModal;

  userList = [];

  private _id;
  private _wfDefine: WorkFlowDefine = new WorkFlowDefine();
  private _positions = [];
  private _jsPlumb = jp.jsPlumb;
  private _jsPlumbInstance;
  private _zoomController;
  private _flowConnections: WorkflowFlowConnection[] = [];
  private _flowNodes: WorkflowFlowNode[] = [];
  private _flowNodeElements: WorkflowFlowNodeElement[] = [];
  private _nodeInfo: WorkflowFlowNodeInfo;
  private _nodeInfos: WorkflowFlowNodeInfo[] = [];
  private _nodeHandle: WorkflowFlowNodeHandle;
  private _nodeHandles: WorkflowFlowNodeHandle[] = [];
  private _nzModal: NzModalRef;
  private _endpointOption = {
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
    this._id = this.routeInfo.snapshot.params['id'];
    this._workflowDefineService.getbyid(this._id).subscribe(result => {
      this._wfDefine = result;
      this.initFlowData();
    });

    // 初始化岗位信息
    this._positionService.getPositions().subscribe(result => {
      this._positions = result;
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

  initFlowData() {
    this._workFlowFlowService.get(this._wfDefine.id).subscribe(result => {
      this.handleNodes(result.nodes);
      this.handleConnections(result.connections);
      this.handleNodeElements(result.nodeElements);
    })
  }

  // 处理节点数据
  handleNodes(nodes) {
    this._flowNodes = nodes;
    this._flowNodes.forEach(node => {
      switch (node.nodeType) {
        case 'startNode':
          this.addStartNode(node.top, node.left);
          break;
        case 'endNode':
          this.addEndNode(node.top, node.left);
          break;
        case 'workNode':
          this.addWorkNode(node.name, node.describe, node.nodeId, node.top, node.left, node.handlePeoples.split(','));
          break;
      }
    });
  }

  // 处理连接数据
  handleConnections(connections) {
    this._flowConnections = connections;
    this._flowConnections.forEach(connection => {
      this._jsPlumbInstance.connect({
        source: connection.sourceId,
        target: connection.targetId,
        anchor: 'Continuous'
      });
    });
  }

  // 处理节点元素关联设置
  handleNodeElements(nodeElements) {
    this._flowNodeElements = nodeElements;
  }

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

    this.renderer.appendChild(this._diagram.nativeElement, workNode);

    // 节点基本信息
    this._nodeInfos.push({ nodeId: id, name: '工作节点', describe: '这是一个工作节点' });

    // 节点人员信息
    this._nodeHandles.push({ nodeId: id, handleType: 1, handlePeople: people, positionGroup: 1, positionId: null });

    let that = this;

    // 绑定双击
    this.renderer.listen(workNode, 'dblclick', event => {
      that._nodeInfo = that._nodeInfos.find(n => n.nodeId == id);
      that._nodeHandle = that._nodeHandles.find(n => n.nodeId == id);

      if (this._nodeHandle.handleType == 1) {
        that._userService.getUserOutlinesById(that._nodeHandle.handlePeople.join(',')).subscribe(
          result => that.userList = result
        );
      }else if(this._nodeHandle.handleType == 2) {
        // TODO根据当前用户的数据取得对应的办理人信息。

      }
      that._nzModal = that.modalService.create({
        nzTitle: null,
        nzContent: that._handlePeopleModal,
        nzFooter: null,
        nzClosable: false,
        nzMaskClosable: false
      });
    });

    // 配置为源
    this._jsPlumbInstance.makeSource(id, {
      anchor: 'Continuous',
      maxConnections: -1,
      allowLoopback: false,
      filter: (event, element) => {
        return event.target.classList.contains('none');
      }
    }, this._endpointOption);

    this._jsPlumbInstance.makeTarget(id, {
      anchor: 'Continuous',
      maxConnections: -1,
      allowLoopback: false,
      filter: (event, element) => {
        return event.target.classList.contains('none');
      }
    }, this._endpointOption);
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

    this.renderer.appendChild(this._diagram.nativeElement, startNode);

    // 配置为源
    this._jsPlumbInstance.makeSource(`start-node`, {
      anchor: 'Continuous',
      isTarget: false,
      maxConnections: 1,
      allowLoopback: false,
      filter: (event, element) => {
        return event.target.classList.contains('start-node-flg');
      }
    }, this._endpointOption);
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

    this.renderer.appendChild(this._diagram.nativeElement, endNode);

    this._jsPlumbInstance.makeTarget(endNode, {
      anchor: 'Continuous',
      maxConnections: 1,
      isSource: false,
      isTarget: true,
    }, this._endpointOption);

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

  close() {
    this._nzModal.close();
    this._nzModal = null;
  }

}
