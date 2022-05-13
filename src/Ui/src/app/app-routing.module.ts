import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {LoginComponent} from "./screens/login/login.component";
import {DefaultLayoutComponent} from "./layouts/default-layout/default-layout.component";
import {AuthGuard} from './guards/auth.guard';
import {AuthorityGuard} from "./guards/authority.guard";

const routes: Routes = [
  {path: '', redirectTo: 'campaign-definition', pathMatch: 'full'},
  {
    path: '', component: DefaultLayoutComponent,
    children: [
      {
        path: 'campaign-definition',
        loadChildren: () => import('./screens/campaign-definition-container/campaign-definition-container.module').then(m => m.CampaignDefinitionContainerModule),
        data: {module: 'campaign-definition', moduleName: 'Kampanya Tanımı'},
        canActivate: [AuthorityGuard]
      },
      {
        path: 'campaign-limits',
        loadChildren: () => import('./screens/campaign-limits-container/campaign-limits-container.module').then(m => m.CampaignLimitsContainerModule),
        data: {module: 'campaign-limits', moduleName: 'Kampanya Çatı Limitleri'},
        canActivate: [AuthorityGuard]
      },
      {
        path: 'target-definition',
        loadChildren: () => import('./screens/target-definition-container/target-definition-container.module').then(m => m.TargetDefinitionContainerModule),
        data: {module: 'target-definition', moduleName: 'Hedef Tanımı'},
        canActivate: [AuthorityGuard]
      }
    ],
    canActivate: [AuthGuard]
  },
  {
    path: 'login', component: LoginComponent
  },
  {path: '**', redirectTo: 'campaign-definition'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {useHash: true})],
  exports: [RouterModule]
})

export class AppRoutingModule {
}
