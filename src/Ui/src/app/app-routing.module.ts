import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {LoginComponent} from "./screens/login/login.component";
import {DefaultLayoutComponent} from "./layouts/default-layout/default-layout.component";

const routes: Routes = [
  {path: '', redirectTo: 'campaign-definition', pathMatch: 'full'},
  {
    path: '', component: DefaultLayoutComponent,
    children: [
      {
        path: 'campaign-definition',
        loadChildren: () => import('./screens/campaign-definition-container/campaign-definition-container.module').then(m => m.CampaignDefinitionContainerModule)
      },
      {
        path: 'campaign-limits',
        loadChildren: () => import('./screens/campaign-limits-container/campaign-limits-container.module').then(m => m.CampaignLimitsContainerModule)
      },
      {
        path: 'target-definition',
        loadChildren: () => import('./screens/target-definition-container/target-definition-container.module').then(m => m.TargetDefinitionContainerModule)
      }
    ]
  },
  {
    path: 'login', component: LoginComponent
  },
  {path: '**', redirectTo: 'campaign-definition'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
