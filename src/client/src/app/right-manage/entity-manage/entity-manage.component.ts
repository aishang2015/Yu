import { Component, OnInit, ViewChild } from '@angular/core';
import { Entity } from '../models/entity';
import { NzModalService, NzModalRef, NzMessageService } from 'ng-zorro-antd';
import { findSafariExecutable } from 'selenium-webdriver/safari';
import { EntityService } from 'src/app/core/services/entity.service';

@Component({
  selector: 'app-entity-manage',
  templateUrl: './entity-manage.component.html',
  styleUrls: ['./entity-manage.component.scss']
})
export class EntityManageComponent implements OnInit {

  // 搜索文字列
  searchText = '';

  // 数据
  listOfData: Entity[] = [];

  // 编辑中数据
  editedEntity: Entity = new Entity();

  // 模态框
  nzModal: NzModalRef;

  // 是否已经提交
  isSubmit = false;

  // 是否等待
  isLoading = false;

  @ViewChild('editTpl')
  editTpl;

  // 分页
  pageIndex = 1;
  total = 0;
  pageSize = 20;

  constructor(private _modalService: NzModalService,
    private _messageService: NzMessageService,
    private _entityService: EntityService) { }

  ngOnInit() {
    this.initData();
  }

  // 添加实体
  addEntity() {
    this.isSubmit = false;
    this.editedEntity = new Entity();
    this.nzModal = this._modalService.create({
      nzContent: this.editTpl,
      nzFooter: null,
      nzTitle: null,
      nzMaskClosable: false,
      nzClosable: false
    });
  }

  // 删除实体
  deleteEntity(entity) {
    this._modalService.confirm({
      nzTitle: '是否删除该实体？',
      nzContent: '删除该实体可能会导致发生系统错误，请认真确认后再进行操作！',
      nzOnOk: () => {
        this._entityService.deleteEntity(entity.id).subscribe(result => {
          this._messageService.success("操作成功");
          this.initData();
        });
      }
    })
  }

  // 编辑实体
  editEntity(entity) {
    Object.assign(this.editedEntity, entity);
    this.nzModal = this._modalService.create({
      nzContent: this.editTpl,
      nzFooter: null,
      nzTitle: null,
      nzMaskClosable: false,
      nzClosable: false
    });
  }

  // 保存修改
  submit(form) {
    this.isSubmit = true;
    this.isLoading = true;
    if (form.valid) {
      if (this.editedEntity.id) {
        this._entityService.updateEntity(this.editedEntity).subscribe(
          result => {
            this._messageService.success("更新成功");
            this.nzModal.close();
            this.initData();

            this.isLoading = false;
          },
          error => { this.isLoading = false; }
        );
      } else {
        this._entityService.addEntity(this.editedEntity).subscribe(
          result => {
            this._messageService.success("创建成功");
            this.nzModal.close();
            this.initData();

            this.isLoading = false;
          },
          error => { this.isLoading = false; }
        );
      }
    }
  }

  // 取消修改
  cancel(form) {
    this.nzModal.close();
  }

  // 搜索内容
  searchData() {
    this.initData();
  }

  // 页面大小发生变化
  pageSizeChange($event) {
    this.pageIndex = 1;
    this.pageSize = $event;
    this.initData();
  }

  // 页面发生变化
  pageIndexChange() {
    this.initData();
  }

  // 初始化数据
  private initData() {
    this._entityService.getEntity(this.pageIndex, this.pageSize, this.searchText).subscribe(result => {
      this.listOfData = result.data;
      this.total = result.total;
    });
  }

}
