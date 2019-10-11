import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '../core/core.module';
import { FlowComponent } from './flow/flow.component';
import { RouterModule } from '@angular/router';
import { WfdefinitionComponent } from './wfdefinition/wfdefinition.component';
import { WfformComponent } from './wfform/wfform.component';
import { EditorModule } from '@tinymce/tinymce-angular';
import { HandleComponent } from './handle/handle.component';
import { JobComponent } from './job/job.component';
import { RecyclebinComponent } from './recyclebin/recyclebin.component';


@NgModule({
  declarations: [
    FlowComponent,
    WfdefinitionComponent,
    WfformComponent,
    HandleComponent,
    JobComponent,
    RecyclebinComponent
  ],
  imports: [
    CommonModule,
    CoreModule,
    EditorModule,
    RouterModule.forChild([
      { path: 'definition', component: WfdefinitionComponent },
      { path: 'flow/:id', component: FlowComponent },
      { path: 'form/:id', component: WfformComponent },
      { path: 'wfform', component: WfformComponent },
      { path: 'handle', component: HandleComponent },
      { path: 'job', component: JobComponent },
      { path: 'recycle', component: RecyclebinComponent },
    ])
  ],
})
export class WorkflowModule { }
