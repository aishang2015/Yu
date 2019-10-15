import { Component, OnInit, ViewChild, ElementRef, Renderer2, ComponentFactoryResolver, ViewContainerRef, NgModule, Compiler } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { NgZorroAntdModule, NzMessageService } from 'ng-zorro-antd';
import { element } from 'protractor';
import { WorkFlowFormElement } from '../models/workflowFormElement';
import { WorkFlowFormService } from 'src/app/core/services/workflow/workflowform.service';
import { ActivatedRoute } from '@angular/router';
import { WorkFlowDefineService } from 'src/app/core/services/workflow/workflowdefine.service';
import { WorkFlowDefine } from '../models/workflowDefine';

declare var tinymce: any;

@Component({
  selector: 'app-wfform',
  templateUrl: './wfform.component.html',
  styleUrls: ['./wfform.component.scss']
})
export class WfformComponent implements OnInit {

  // 预览内容容器
  @ViewChild('htmlContainer', { static: true, read: ViewContainerRef })
  private htmlContainer: ViewContainerRef;

  // 数据
  dataModel = '';

  // 模态框
  nzModal;

  // 光标位置
  bookmark: any;

  // 按钮加载
  isLoading: boolean;

  // 工作流定义
  wfDefine: WorkFlowDefine = new WorkFlowDefine();

  // 表单元素
  workflowFormElements: WorkFlowFormElement[] = [];

  // 编辑表单元素
  editedWorkFlowFormElement: WorkFlowFormElement = new WorkFlowFormElement();

  // 全部组件
  components = this._workflowFormService.components;

  // tinymce初始化配置
  tinymceSetting = {
    language_url: '../../assets/tinymce/langs/zh_CN.js',
    language: 'zh_CN',
    height: 800,
    content_css: '../../assets/workflow/style.css',
    plugins: [
      'advlist autolink lists link image charmap print preview anchor',
      'searchreplace visualblocks code fullscreen',
      'insertdatetime media table paste code help wordcount'
    ],
    toolbar: 'undo redo | formatselect | bold italic backcolor | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | removeformat | help'
  }

  @ViewChild('#editor', { static: true })
  tinymceEditor;

  constructor(private renderer: Renderer2,
    private sanitizer: DomSanitizer,
    private resolveFactory: ComponentFactoryResolver,
    private routeInfo: ActivatedRoute,
    private compiler: Compiler,
    private messageService: NzMessageService,
    private _workflowDefineService: WorkFlowDefineService,
    private _workflowFormService: WorkFlowFormService) { }

  ngOnInit() {

    // 取得工作流状态
    const id = this.routeInfo.snapshot.params['id'];
    this._workflowDefineService.getbyid(id).subscribe(result => {
      this.wfDefine = result;
      this.initForm();
    });

  }

  // 初始化表单编辑
  initForm() {
    this._workflowFormService.get(this.wfDefine.id).subscribe(result => {
      this.dataModel = result.formContent ? result.formContent.editFormHtml : '';
      this.workflowFormElements = result.formElements;
      setInterval(() => this.initClickEvent(), 3000);
    });
  }

  // 创建动态组件
  createComponent(template) {

    @Component({ template })
    class TemplateComponent { }

    @NgModule({ declarations: [TemplateComponent], imports: [NgZorroAntdModule] })
    class TemplateModule { }

    const mod = this.compiler.compileModuleAndAllComponentsSync(TemplateModule);
    const factory = mod.componentFactories.find((comp) =>
      comp.componentType === TemplateComponent
    );

    this.htmlContainer.clear();
    this.htmlContainer.createComponent(factory);
  }

  // 变化事件
  handleChange($event) {
  }

  // 点击添加控件
  addComponent(typeKey) {
    let elementid = Date.now().toString(36);
    let html = this.makehtml(elementid, typeKey);

    tinymce.execCommand("mceInsertContent", false, html);
    tinymce.get('editor').focus();
    this.bindClick(elementid);

    this.workflowFormElements.push({
      elementId: elementid,
      defineId: this.wfDefine.id,
      name: '',
      type: typeKey,
      width: 120,
      options: '',
      line: 3
    })
  }

  // 关闭模态框
  closeModal() {
    this.nzModal.close();
  }

  // 绑定单击
  bindClick(id) {
    let contentDocument: any = document.getElementById('editor_ifr');
    let element: HTMLElement = contentDocument.contentDocument.getElementById(id);
    element.onclick = event => {
      let e = this.workflowFormElements.find(e => e.elementId == id);
      if (e) {
        Object.assign(this.editedWorkFlowFormElement, e);
      }
    };
  }

  // 初始化点击事件
  initClickEvent() {
    let contentDocument: any = document.getElementById('editor_ifr');
    let elements: HTMLCollection = contentDocument.contentDocument.getElementsByTagName('input');
    for (let i = 0; i < elements.length; i++) {
      let element: any = elements[i];
      element.onclick = event => {
        let e = this.workflowFormElements.find(e => e.elementId == element.id);
        if (e) {
          Object.assign(this.editedWorkFlowFormElement, e);
        }
      };
    }
  }

  // 生成html
  makehtml(elementid, elementtype) {
    let html = '';
    switch (elementtype) {
      case 'datepicker':
        html = `<input id="${elementid}" class="input" readonly="readonly" value="日期选择"/>`;
        break;
      case 'timepicker':
        html = `<input id="${elementid}" class="input" readonly="readonly" value="时间选择"/>`;
        break;
      case 'inputnumber':
        html = `<input id="${elementid}" class="input" readonly="readonly" value="数字输入框"/>`;
        break;
      case 'input':
        html = `<input id="${elementid}" class="input" readonly="readonly" value="输入框"/>`;
        break;
      case 'textarea':
        html = `<input id="${elementid}" class="input" readonly="readonly" value="文本域"/>`;
        break;
      case 'selecter':
        html = `<input id="${elementid}" class="input" readonly="readonly" value="选择器"/>`;
        break;
      case 'upload':
        html = `<input id="${elementid}" class="input" readonly="readonly" value="文件上传"/>`;
        break;
    }
    return html;
  }

  // 获取控件类型名称
  getElementName(key) {
    return this._workflowFormService.components.find(c => c.key == key) ?
      this._workflowFormService.components.find(c => c.key == key).describe : '';
  }

  // 保存控件属性
  saveElement() {
    let element = this.workflowFormElements.find(e => e.elementId == this.editedWorkFlowFormElement.elementId);
    if (element) {
      Object.assign(element, this.editedWorkFlowFormElement);
      this.editedWorkFlowFormElement = new WorkFlowFormElement();

      // 设置元素的宽度
      let contentDocument: any = document.getElementById('editor_ifr');
      let htmlElement: HTMLElement = contentDocument.contentDocument.getElementById(element.elementId);
      if (htmlElement) {
        htmlElement.style.width = `${element.width}px`;
      }
    }
  }

  // 保存表单
  saveForm() {
    this.isLoading = true;
    let contentDocument: any = document.getElementById('editor_ifr');
    for (let i = 0; i < this.workflowFormElements.length; i++) {
      let htmlelement = contentDocument.contentDocument.getElementById(this.workflowFormElements[i].elementId);
      if (!htmlelement) {
        this.workflowFormElements.splice(i, 1);
        i--;
      }
    }

    this._workflowFormService.addOrUpdate({
      defineId: this.wfDefine.id,
      formContent: {
        defineId: this.wfDefine.id,
        editFormHtml: this.dataModel
      },
      formElements: this.workflowFormElements
    }).subscribe(result => {
      this.messageService.success("保存成功！");
      this.isLoading = false;
    }, error => this.isLoading = false);


  }

  // 取得编辑状态的html
  getEditHtml(id, key) {
    let html = '';
    switch (key) {
      case 'datepicker':
        html = `<nz-date-picker id="${id}"></nz-date-picker>`;
        break;
      case 'timepicker':
        html = `<nz-time-picker id="${id}"></nz-time-picker>`;
        break;
      case 'inputnumber':
        html = `<nz-input-number id="${id}"></nz-input-number>`;
        break;
      case 'input':
        html = `<input id="${id}" nz-input/>`;
        break;
      case 'textarea':
        html = `<textarea id="${id}" rows="4" nz-input></textarea>`;
        break;
      case 'selecter':
        html = `<nz-select style="width: 120px;></nz-select>`;
        break;
      case 'upload':
        html = `<nz-upload>'
                  <button nz-button><i nz-icon nzType="upload"></i><span>Click to Upload</span></button>
                </nz-upload>`;
        break;
    }
    return html;
  }

}
