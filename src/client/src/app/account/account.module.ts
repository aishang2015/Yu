import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '../core/core.module';
import { Router, RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { LoginGuard } from '../core/services/login-guard.service';
import { UserInfoComponent } from './user-info/user-info.component';

@NgModule({
  declarations: [
    LoginComponent,
    UserInfoComponent
  ],
  imports: [
    CommonModule,
    CoreModule,
    RouterModule.forChild([
      { path: '', component: LoginComponent },
      { path: 'userInfo', component: UserInfoComponent }
    ])
  ]
})
export class AccountModule { }
