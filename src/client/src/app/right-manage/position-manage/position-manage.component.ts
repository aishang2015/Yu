import { Component, OnInit, ViewChild } from '@angular/core';
import { PositionService } from 'src/app/core/services/rightmanage/position.service';
import { NzModalService, NzMessageService } from 'ng-zorro-antd';
import { PositionEntity } from '../models/position';

@Component({
  selector: 'app-position-manage',
  templateUrl: './position-manage.component.html',
  styleUrls: ['./position-manage.component.scss']
})
export class PositionManageComponent implements OnInit {

  searchText: string = '';

  positionList: PositionEntity[] = [];

  isLoading = false;

  // 分页
  pageIndex = 1;
  pageSize = 20;
  total = 0;

  // 编辑中的数据
  editedPosition: PositionEntity = new PositionEntity();

  // 模态框
  nzmodal;

  // 编辑模板
  @ViewChild('editTpl', { static: false })
  positionFormTpl;

  constructor(private _positionService: PositionService,
    private _modalService: NzModalService,
    private _messageService: NzMessageService) { }

  ngOnInit() {
    this.initPositions();
  }

  // 初始化数据
  initPositions() {
    this._positionService.get(this.pageIndex, this.pageSize, this.searchText).subscribe(result => {
      this.positionList = [];
      this.positionList = result.data;
      this.total = result.total;
    });
  }

  // 搜索数据
  searchData() {
    this.initPositions();
  }

  // 添加岗位
  addPosition() {
    this.editedPosition = new PositionEntity();
    this.nzmodal = this._modalService.create({
      nzContent: this.positionFormTpl,
      nzFooter: null,
      nzTitle: null,
      nzMaskClosable: false,
      nzClosable: false
    });
  }

  // 删除岗位
  deletePosition(position) {
    this._positionService.delete(position.id).subscribe(result => {
      this._messageService.success('删除成功！');
      this.initPositions();
    });
  }

  // 编辑岗位
  editPosition(position) {
    Object.assign(this.editedPosition, position);
    this.nzmodal = this._modalService.create({
      nzContent: this.positionFormTpl,
      nzFooter: null,
      nzTitle: null,
      nzMaskClosable: false,
      nzClosable: false
    });
  }

  // 提交数据
  submit(positionForm) {
    if (positionForm.valid) {
      this.isLoading = true;
      let handler = () => {
        this.isLoading = false;
        this.initPositions();
        this.nzmodal.close();
      }
      if (this.editedPosition.id) {
        this._positionService.update(this.editedPosition).subscribe(
          result => handler(),
          error => this.isLoading = false);
      } else {
        this._positionService.add(this.editedPosition).subscribe(
          result => handler(),
          error => this.isLoading = false);
      }
    }
  }

  // 取消编辑
  cancel(){
    this.nzmodal.close();
  }

  // 页面大小发生变化
  pageSizeChange($event) {
    this.pageIndex = 1;
    this.pageSize = $event;
    this.initPositions();
  }

  // 页面发生变化
  pageIndexChange() {
    this.initPositions();
  }

}
