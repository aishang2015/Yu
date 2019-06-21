import { Component, OnInit, ViewChild } from '@angular/core';
import { Element } from '../models/element';
import { EnumConstant } from 'src/app/core/constants/enum-constant';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { NzTreeComponent, NzTreeNodeOptions, NzModalService } from 'ng-zorro-antd';
import { element } from '@angular/core/src/render3';

@Component({
  selector: 'app-menu-manage',
  templateUrl: './menu-manage.component.html',
  styleUrls: ['./menu-manage.component.scss']
})
export class MenuManageComponent implements OnInit {

  // nodes = [
  //   {
  //     title: '菜单1', key: '100', icon: 'menu', children: [
  //       { title: '按钮', key: '1001', icon: 'plus-square' },
  //       { title: '链接', key: '1002', icon: 'link' },
  //     ]
  //   }
  // ];

  nodes = [];

  // 全部元素
  elements: Element[] = [];

  // 编辑中的元素
  editedElement: Element = new Element();

  // 选中的节点
  selectedNode;

  // 当前是否在编辑状态
  isEditMode: boolean = false;

  // 元素类型
  elementTypes = EnumConstant.elementTypes;

  // 是否提交
  isSubmit: boolean = false;

  constructor(private _modalService: NzModalService) { }

  ngOnInit() {
    this.elements = [
      { id: '0001', upId: '', name: '权限管理', elementType: 1, identification: 'rightmanagemenu', route: '' },
      { id: '00001', upId: '0001', name: '用户管理', elementType: 1, identification: 'rightmanagemenu', route: '' },
      { id: '00002', upId: '0001', name: '角色管理', elementType: 1, identification: 'rightmanagemenu', route: '' },
      { id: '00003', upId: '0001', name: 'API管理', elementType: 1, identification: 'rightmanagemenu', route: '' },
      { id: '00004', upId: '0001', name: '页面管理', elementType: 1, identification: 'rightmanagemenu', route: '' },
      { id: '00005', upId: '0001', name: '组织管理', elementType: 1, identification: 'rightmanagemenu', route: '' },
      { id: '000001', upId: '00001', name: '删除用户', elementType: 2, identification: 'rightmanagemenu', route: '' },
    ];

    this.makeNodes();

  }

  // 创建子元素
  createMenu() {

    // 只有选择菜单的情况下可以创建子元素
    if (this.editedElement.elementType != 2 && this.editedElement.elementType != 3) {
      this.isEditMode = true;
      this.editedElement = { id: '', upId: this.editedElement.id, name: '', elementType: 1, identification: '', route: '' }
    }
  }

  // 删除菜单
  deleteMenu() {
    if (!this.isEditMode) {
      this._modalService.confirm({
        nzTitle: '是否要删除该元素？',
        nzContent: '删除该元素会删除所有子元素'
      });
    }
  }

  // 编辑菜单
  editMenu() {
    this.isEditMode = true;
  }

  // 提交表单
  submit(form) {
    this.isSubmit = true;
    if (form.valid) {
      this.isSubmit = false;
      let name = form.controls['name'].value;

      // todo
      this.editedElement.id = '1122';
      this.elements.push(this.editedElement);
      this.makeNodes();
    }
  }

  // 取消编辑表单
  cancelEdit(form) {
    form.reset();
    this.isEditMode = false;
  }

  // 点击树，设置选中节点
  treeClick($event) {
    if ($event.eventName = 'click' && !this.isEditMode) {
      if ($event.node.isSelected) {
        this.selectedNode = $event.node;
        Object.assign(this.editedElement, this.elements.find(element => element.id == this.selectedNode.key));
      } else {
        this.selectedNode = null;
        this.editedElement = new Element();
      }
    }
  }

  // 取得元素名称
  getElementName(id) {
    if (id) {
      return this.getNzTreeNodeOption(id).title;
    } return '';
  }

  // 构建树
  private makeNodes() {

    this.nodes = [];

    // 第一级节点
    this.elements.forEach(element => {
      if (!element.upId) {
        this.nodes.push(
          { title: element.name, key: element.id, icon: this.getIcon(element.elementType), isLeaf: this.isLeaf(element.elementType) }
        );
      }
    });

    // 循环查找下级节点
    let loopKeys: string[] = this.nodes.map(n => n.key);
    while (loopKeys.length > 0) {

      let newLoopKeys: string[] = [];

      loopKeys.forEach(key => {

        // 查找以当前节点为父节点的节点
        const childs = this.elements.filter(element => element.upId == key);

        // 查找treenodeoption
        this.GetTreeNodeOptionByKey(this.nodes, key);

        // 设置子节点
        this.findTreeNode.children = [];

        // 查找到的节点推入节点的子节点
        childs.forEach(element => {
          this.findTreeNode.children.push(
            { title: element.name, key: element.id, icon: this.getIcon(element.elementType), isLeaf: this.isLeaf(element.elementType) }
          );
          newLoopKeys.push(element.id);
        });

      });

      // 查找下一轮的子节点
      loopKeys = [];
      Object.assign(loopKeys, newLoopKeys);
    }
  }

  // 获取图标
  private getIcon(elementType: number): string {
    switch (elementType) {
      case 1:
        return 'menu';
      case 2:
        return 'plus-square';
      case 3:
        return 'link';
    }
  }

  // 是否是子叶节点
  private isLeaf(elementType: number): boolean {
    return (elementType == 2 || elementType == 3);
  }

  // 递归查找TreeNodeOption
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

  // 合并
  private getNzTreeNodeOption(id): NzTreeNodeOptions {
    this.GetTreeNodeOptionByKey(this.nodes, id);
    return this.findTreeNode;
  }


}
