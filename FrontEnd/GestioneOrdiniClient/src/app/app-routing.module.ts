import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './auth/guard/auth-guard.guard';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { LoginGuard } from './auth/guard/login.guard';

const routes: Routes = [
  { path: '', loadChildren: () => import('./auth/auth.module').then(m => m.AuthModule), canActivate: [LoginGuard] }, // Applica LoginGuard
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] }, // Carica il componente direttamente
  { path: '**', redirectTo: '/login'}];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
