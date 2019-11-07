import { Component, OnInit, Input, NgModule, Compiler, ViewChild, ViewContainerRef } from '@angular/core';
import { WorkFlowDefineService } from 'src/app/core/services/workflow/workflowdefine.service';
import { WorkFlowFormService } from 'src/app/core/services/workflow/workflowform.service';
import { WorkFlowFormElement } from '../../models/workflowFormElement';
import { NgZorroAntdModule, UploadFile, NzMessageService } from 'ng-zorro-antd';
import { WorkFlowInstanceService } from 'src/app/core/services/workflow/workflowinstance.service';
import { FormGroup, ReactiveFormsModule, FormBuilder, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { WorkflowFlowNodeElement } from '../../models/workflowFlowNodeElement';

import { HttpClient, HttpRequest, HttpResponse } from '@angular/common/http';
import { CoreModule } from 'src/app/core/core.module';
import { UriConstant } from 'src/app/core/constants/uri-constant';
import { stringify } from 'querystring';
import { Observable, Observer } from 'rxjs';

@Component({
  selector: 'app-job-edit',
  templateUrl: './job-edit.component.html',
  styleUrls: ['./job-edit.component.scss', '../../../../assets/workflow/style.css']
})
export class JobEditComponent implements OnInit {

  // 工作流定义id
  @Input()
  wfDefineId;

  // 工作流实例Id
  @Input()
  wfInstanceId;

  // html容器
  @ViewChild('htmlContainer', { static: true, read: ViewContainerRef })
  htmlContainer;

  // html数据
  dataModel = '';

  // 表单元素
  workflowFormElements: WorkFlowFormElement[] = [];

  // 工作流节点表单元素状态
  workflowFlowNodeElements: WorkflowFlowNodeElement[] = [];


  // 动态组件对象
  dynamicComponentRef;

  // 值
  values: { [code: string]: object } = {};

  // 上传文件初始值
  fileListMap: { [id: string]: UploadFile[] } = {};

  constructor(
    private fb: FormBuilder,
    private _compiler: Compiler,
    private _workflowDefineService: WorkFlowDefineService,
    private _workflowFormService: WorkFlowFormService,
    private _workflowInstanceService: WorkFlowInstanceService) { }

  ngOnInit() {
    this._workflowFormService.get(this.wfDefineId).subscribe(result => {
      this.dataModel = result.formContent ? result.formContent.editFormHtml : '';
      this.workflowFormElements = result.formElements;
      //let group: { [code: string]: [object] } = {};
      //this.workflowFormElements.forEach(wffe => group[wffe.id] = [null]);
      //this.editForm = this.fb.group(group);
      this._workflowInstanceService.getForm(this.wfInstanceId).subscribe(result => {
        this.workflowFormElements.forEach(wffe => {
          let form = result.find(f => f.elementId == wffe.id);
          if (wffe.type == 'timepicker' || wffe.type == 'datepicker') {
            this.values[wffe.elementId] = (form && form.value != '') ? new Date(form.value) : new Date();
          }
          else if (wffe.type == 'upload') {
            this.fileListMap[wffe.elementId] = [];
            var fileNames = form ? (form.value ? form.value.split(',') : []) : [];
            fileNames.forEach(fileName => {
              this.fileListMap[wffe.elementId].push({
                uid: (this.fileListMap[wffe.elementId].length + 1).toString(),
                name: fileName,
                status: 'done',
                size: 0,
                type: '',
                url: UriConstant.ServerUri + 'wf/' + fileName
              })
            });
          }
          else {
            this.values[wffe.elementId] = form ? form.value : null;
          }
        });

        this._workflowInstanceService.getFlowNodeElement(this.wfInstanceId).subscribe(
          result => {
            this.workflowFlowNodeElements = result;
            this.htmlEdit();
          }
        );


      });


    });
  }



  // 编辑html
  htmlEdit() {
    for (let i = 0; i < this.workflowFormElements.length; i++) {
      let element: any = this.workflowFormElements[i];

      let nodeElement = this.workflowFlowNodeElements.find(wffne => wffne.formElementId == element.id);

      // 正则替换真正页面的标签
      let newhtml = this.getEditHtml(element.elementId, element.type, element.width, element.line, element.options, nodeElement ? nodeElement.isEditable : true, nodeElement ? nodeElement.isVisible : true);
      //newhtml = '<form nz-form [formGroup]="editForm">'+ newhtml + '</form>';
      let regex = new RegExp(`<input id="${element.elementId}".*?/>`);
      this.dataModel = this.dataModel.replace(regex, newhtml);
    }
    this.createComponent(this.dataModel);
  }

  // 替换html的内容
  getEditHtml(id, key, width, rows, ops, editable, visible) {
    let style = width ? `style="width:${width}px;` : '';
    style = style.concat(visible ? '"' : 'display: none;"');
    let row = `rows="${rows ? rows : 1}"`;
    let options = '';
    ops.split(',').forEach(str => {
      options += ` <nz-option nzValue="${str}" nzLabel="${str}"></nz-option>`;
    })
    let html = '';
    switch (key) {
      case 'datepicker':
        html = `<nz-date-picker [(ngModel)]="values['${id}']" [disabled]="${!editable}" id="${id}" ${style}></nz-date-picker>`;
        break;
      case 'timepicker':
        html = `<nz-time-picker [(ngModel)]="values['${id}']" [disabled]="${!editable}"  id="${id}" ${style}></nz-time-picker>`;
        break;
      case 'inputnumber':
        html = `<nz-input-number [(ngModel)]="values['${id}']" [disabled]="${!editable}"  id="${id}" ${style}></nz-input-number>`;
        break;
      case 'input':
        html = `<input id="${id}" [(ngModel)]="values['${id}']" [disabled]="${!editable}"  nz-input ${style}/>`;
        break;
      case 'textarea':
        html = `<textarea [(ngModel)]="values['${id}']" [disabled]="${!editable}"  id="${id}" ${row} nz-input ${style}></textarea>`;
        break;
      case 'selecter':
        html = `<nz-select [(ngModel)]="values['${id}']" [disabled]="${!editable}"  id="${id}" ${style}>${options}</nz-select>`;
        break;
      case 'upload':
        html = `<nz-upload (nzChange)="handleChange($event)" [(nzFileList)]="fileListMap['${id}']" [nzBeforeUpload]="beforeUpload" [nzShowButton]="fileListMap['${id}'].length < 3" [nzAction]="uploadUrl" (click)="setId('${id}')" id="${id}">` +
          `<button nz-button><i nz-icon nzType="upload"></i><span>请选择上传文件</span></button>` +
          `</nz-upload>`;
        break;
    }
    return html;
  }

  // 创建动态组件
  createComponent(template) {
    @Component({ template })
    class TemplateComponent {

      currentId;

      uploadUrl = UriConstant.BaseApiUri + 'workFlowInstanceFormFile';

      values: { [code: string]: string } = {};

      // 上传文件列表
      fileListMap: { [id: string]: UploadFile[] } = {};

      constructor(private _workflowInstanceService: WorkFlowInstanceService,
        private _message:NzMessageService) { }

      beforeUpload = (file: File) => {
        const isLt2M = file.size / 1024 / 1024 < 2;
        if (!isLt2M) {
          this._message.error('文件必须小于 2MB!');
          return false;
        }
      };

      handleChange(info: any): void {
        switch (info.file.status) {
          case 'uploading':
            break;
          case 'done':
            let fileinfo = this.fileListMap[this.currentId].find(f => f.uid == info.file.uid);
            fileinfo.name = info.file.response.name;
            fileinfo.url = UriConstant.ServerUri + 'wf/' + info.file.response.name;
            break;
          case 'error':
            break;
          case 'removed':
            this._workflowInstanceService.deleteFormFile(info.file.name).toPromise();
            break;
        }
      }

      setId(id) {
        this.currentId = id;
      }
    }

    @NgModule({ declarations: [TemplateComponent], imports: [NgZorroAntdModule, FormsModule, CoreModule] })
    class TemplateModule { }

    const mod = this._compiler.compileModuleAndAllComponentsSync(TemplateModule);
    const factory = mod.componentFactories.find((comp) =>
      comp.componentType === TemplateComponent
    );

    this.htmlContainer.clear();
    let componentRef = this.htmlContainer.createComponent(factory);
    this.dynamicComponentRef = <TemplateComponent>componentRef;

    // 初始化动态组件的表单数据
    this.dynamicComponentRef.instance.values = this.values;
    this.dynamicComponentRef.instance.fileListMap = this.fileListMap;
  }

  // 保存内容
  saveContent() {
    let values = this.dynamicComponentRef.instance.values;
    let fileListMap = this.dynamicComponentRef.instance.fileListMap;
    let forms = [];
    let files = [];

    for (const element of this.workflowFormElements) {
      if (element.type != 'upload') {
        let value = values[element.elementId];
        forms.push({
          elementID: element.id,
          value: value
        });
      } else {
        let fileList = fileListMap[element.elementId];
        let names = [];
        fileList.forEach(element => {
          names.push(element.name);
        });
        forms.push({
          elementID: element.id,
          value: names.join(',')
        });
      }
    }
    return this._workflowInstanceService.putForm({
      instanceId: this.wfInstanceId,
      workFlowInstanceForms: forms,
    });
  }

}
