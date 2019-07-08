import { Component, OnInit, ViewChild } from '@angular/core';
import { Role } from '../models/role';
import { NzModalRef, NzModalService, NzTreeNodeOptions } from 'ng-zorro-antd';
import { ApiService } from 'src/app/core/services/api.service';
import { ElementService } from 'src/app/core/services/element.service';
import { RuleService } from 'src/app/core/services/rule.service';

@Component({
  selector: 'app-role-manage',
  templateUrl: './role-manage.component.html',
  styleUrls: ['./role-manage.component.scss']
})
export class RoleManageComponent implements OnInit {

  // 搜索内容
  searchText = '';

  // 表格数据
  listOfData: Role[] = [];

  // 分页数据
  pageIndex = 1;
  total = 0;
  pageSize = 20;

  // 编辑中的数据
  editedRole: Role = new Role();

  // 是否提交
  isSubmit = false;

  // 编辑模板
  @ViewChild("editTpl")
  editTpl;

  // 页面元素选择数据
  elementsNodes = [];

  // 数据规则
  dataRules = [];

  // 模态框
  nzModal: NzModalRef;

  constructor(private _modalService: NzModalService,
    private _elementService: ElementService,
    private _dataRuleService: RuleService) { }

  ngOnInit() {
    this.initElementsNodes();
    this.initDataRules();
  }

  // 搜索数据
  searchData() {
  }

  // 添加角色
  addRole() {
    this.editedRole = new Role();
    this.nzModal = this._modalService.create({
      nzTitle: null,
      nzContent: this.editTpl,
      nzFooter: null,
    });
  }

  // 页面大小发生变化
  pageSizeChange($event) {
    this.pageSize = $event;
    this.pageIndex = 1;
  }

  // 页码发生变化
  pageIndexChange() {
  }

  // 提交表单
  submit(form) {
    this.isSubmit = true;
    if (form.valid) {

    }
  }

  // 取消提交表单
  cancel(form) {
    this.nzModal.close();
  }


  // 初始化页面元素数据
  private initElementsNodes() {
    this._elementService.getAllElement().subscribe(result => {
      this.makeNodes(result);
    });
  }

  // 初始化数据规则下拉框数据
  private initDataRules() {
    this._dataRuleService.getAllRuleGroup().subscribe(result => {
      result.forEach(rule => {
        this.dataRules.push({ name: rule.name, value: rule.id });
      });
    });
  }

  // 以下方法取自menu-manage.component.ts
  // 构建树
  private makeNodes(elements) {

    this.elementsNodes = [];

    // 第一级节点
    elements.forEach(element => {
      if (!element.upId) {
        this.elementsNodes.push(
          { title: element.name, key: element.id, isLeaf: this.isLeaf(element.elementType) }
        );
      }
    });

    // 循环查找下级节点
    let loopKeys: string[] = this.elementsNodes.map(n => n.key);
    while (loopKeys.length > 0) {

      let newLoopKeys: string[] = [];

      loopKeys.forEach(key => {

        // 查找以当前节点为父节点的节点
        const childs = elements.filter(element => element.upId == key);

        // 查找treenodeoption
        this.GetTreeNodeOptionByKey(this.elementsNodes, key);

        // 设置子节点
        this.findTreeNode.children = [];

        // 查找到的节点推入节点的子节点
        childs.forEach(element => {
          this.findTreeNode.children.push(
            { title: element.name, key: element.id, isLeaf: this.isLeaf(element.elementType) }
          );
          newLoopKeys.push(element.id);
        });

      });

      // 查找下一轮的子节点
      loopKeys = [];
      Object.assign(loopKeys, newLoopKeys);
    }
  }
  private isLeaf(elementType: number): boolean {
    return (elementType == 2 || elementType == 3);
  }
  private findTreeNode: NzTreeNodeOptions;
  private GetTreeNodeOptionByKey(nodes: NzTreeNodeOptions[], key: string, ) {
    if (nodes.find(n => n.key == key)) {
      this.findTreeNode = nodes.find(n => n.key == key);
    } else {
      nodes.forEach(n => {
        if (n.children) {
          if (n.children.length != 0) {
            this.GetTreeNodeOptionByKey(n.children, key);
          }
        }
      });
    }
  }
  // 以上方法取自menu-manage.component.ts



}
