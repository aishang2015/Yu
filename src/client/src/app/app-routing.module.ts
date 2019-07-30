import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginGuard } from './core/services/login-guard.service';

const routes: Routes = [
  { path: 'dashboard', loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule), canActivate: [LoginGuard] },
  { path: 'login', loadChildren: () => import('./account/account.module').then(m => m.AccountModule), canActivate: [LoginGuard] },
  { path: 'account', loadChildren: () => import('./account/account.module').then(m => m.AccountModule), canActivate: [LoginGuard] },
  { path: 'right', loadChildren: () => import('./right-manage/right-manage.module').then(m => m.RightManageModule), canActivate: [LoginGuard] },
  { path: '', pathMatch: 'full', redirectTo: 'login' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
