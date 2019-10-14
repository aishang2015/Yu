import { Component, OnInit, ViewChild } from '@angular/core';
import { NzModalService } from 'ng-zorro-antd';

@Component({
  selector: 'app-job',
  templateUrl: './job.component.html',
  styleUrls: ['./job.component.scss']
})
export class JobComponent implements OnInit {

  // 模态框对象
  private _nzModal;

  // 新建工作流模态框
  @ViewChild('createTpl', { static: true })
  createTpl;

  constructor(private _modalService: NzModalService) { }

  ngOnInit() {
  }

  // 创建工作流
  addJob() {
    this._nzModal = this._modalService.create({
      nzTitle: null,
      nzContent: this.createTpl,
      nzFooter: null,
      nzMaskClosable: false,
      nzClosable: false
    });
  }

  // 确认创建
  comfirmAdd(define) {
    console.log(define);
    this._nzModal.close();
    this._nzModal = null;
  }

  // 取消编辑
  cancel() {
    this._nzModal.close();
    this._nzModal = null;
  }

}
