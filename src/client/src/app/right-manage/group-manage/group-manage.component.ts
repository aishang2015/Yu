import { Component, OnInit } from '@angular/core';
import { Group } from '../models/group';
import { NzTreeNodeOptions, NzMessageService, NzModalService } from 'ng-zorro-antd';
import { GroupService } from 'src/app/core/services/group.service';

@Component({
  selector: 'app-group-manage',
  templateUrl: './group-manage.component.html',
  styleUrls: ['./group-manage.component.scss']
})
export class GroupManageComponent implements OnInit {

  // 是否编辑模式
  isEditMode = false;

  // 树节点
  nodes = [];

  // 全部元素
  groups: Group[] = [];

  // 编辑的组织
  editedGroup: Group = new Group();

  // 提交
  isSubmit: boolean = false;

  constructor(private _groupService: GroupService,
    private _messageService: NzMessageService,
    private _modalService: NzModalService) { }

  ngOnInit() {
    this.initData();
  }

  // 创建group
  createGroup() {
    this.isEditMode = true;
    this.editedGroup = { id: '', upId: this.editedGroup.id, groupName: '', remark: '' }
  }

  // 编辑group
  editGroup() {
    this.isEditMode = true;
  }

  // 删除group
  deleteGroup() {
    if (!this.isEditMode && this.editedGroup.id) {
      this._modalService.confirm({
        nzTitle: '是否要删除该组织？',
        nzContent: '删除该组织会删除所有子组织',
        nzOnOk: () => {
          this._groupService.deleteGroup(this.editedGroup.id).subscribe(
            result => {
              this._messageService.success("删除成功!");
              this.initData();

              this.editedGroup = new Group();
            }
          )
        }
      });
    }

  }


  // 点击树节点
  treeClick($event) {

    if ($event.eventName = 'click' && !this.isEditMode) {
      if ($event.node.isSelected) {
        Object.assign(this.editedGroup, this.groups.find(element => element.id == $event.node.key));
      } else {
        this.editedGroup = new Group();
      }
    }

  }

  // 获取组织名称
  getGroupName(id) {
    if (id) {
      return this.getNzTreeNodeOption(id).title;
    } return '';
  }

  // 提交
  submit(form) {

    this.isSubmit = true;
    if (form.valid) {
      this.isSubmit = false;

      if (this.editedGroup.id) {
        this._groupService.updateGroup(this.editedGroup).subscribe(
          result => {
            this._messageService.success("修改成功!");
            this.initData();

            // 重置编辑表单
            this.cancelEdit(form);
          }
        );
      } else {
        this._groupService.addGroup(this.editedGroup).subscribe(
          result => {
            this._messageService.success("添加成功!");
            this.initData();

            // 重置编辑表单
            this.cancelEdit(form);
          }
        );
      }
    }

  }

  // 取消编辑
  cancelEdit(form) {
    form.reset();
    this.isEditMode = false;
  }

  // 初始化数据
  private initData() {
    this._groupService.getGroups().subscribe(
      result => {
        this.groups = result;
        this._messageService.success("数据初始化完毕。");
        this.makeNodes();
      }
    )
  }


  // 构建树
  private makeNodes() {

    this.nodes = [];

    // 第一级节点
    this.groups.forEach(group => {
      if (!group.upId) {
        this.nodes.push(
          { title: group.groupName, key: group.id, icon: 'cluster' }
        );
      }
    });


    // 循环查找下级节点
    let loopKeys: string[] = this.nodes.map(n => n.key);
    while (loopKeys.length > 0) {

      let newLoopKeys: string[] = [];

      loopKeys.forEach(key => {

        // 查找以当前节点为父节点的节点
        const childs = this.groups.filter(element => element.upId == key);

        // 查找treenodeoption
        this.GetTreeNodeOptionByKey(this.nodes, key);

        // 设置子节点
        this.findTreeNode.children = [];

        // 查找到的节点推入节点的子节点
        childs.forEach(element => {
          this.findTreeNode.children.push(
            { title: element.groupName, key: element.id, icon: 'cluster' }
          );
          newLoopKeys.push(element.id);
        });

      });

      // 查找下一轮的子节点
      loopKeys = [];
      Object.assign(loopKeys, newLoopKeys);
    }
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
