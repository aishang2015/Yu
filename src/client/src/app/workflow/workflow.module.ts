import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '../core/core.module';
import { FlowComponent } from './flow/flow.component';
import { RouterModule } from '@angular/router';
import { SubPathGuard } from '../core/services/sub-path-guard.service';


@NgModule({
  declarations: [
    FlowComponent
  ],
  imports: [
    CommonModule,
    CoreModule,
    RouterModule.forChild([
      { path: 'flow', component: FlowComponent },
    ])
  ]
})
export class WorkflowModule { }
