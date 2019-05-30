import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '../core/core.module';
import { Router, RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { LoginGuard } from '../core/services/login-guard.service';

@NgModule({
  declarations: [
    LoginComponent
  ],
  imports: [
    CommonModule,
    CoreModule,
    RouterModule.forChild([
      { path: '', component: LoginComponent, canActivate: [LoginGuard] }
    ])
  ]
})
export class AccountModule { }
