import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TestDataComponent } from './test-data/test-data.component';
import { CoreModule } from '../core/core.module';
import { RouterModule } from '@angular/router';
import { SubPathGuard } from '../core/services/sub-path-guard.service';

@NgModule({
  declarations: [
    TestDataComponent
  ],
  imports: [
    CommonModule,
    CoreModule,
    RouterModule.forChild([
      { path: 'testdata', component: TestDataComponent, canActivate: [SubPathGuard] },
    ])
  ]
})
export class TestModule { }
