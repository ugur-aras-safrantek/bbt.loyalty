import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {CampaignLimitsContainerComponent} from './campaign-limits-container.component';
import {RouterModule, Routes} from "@angular/router";

const routes: Routes = [
  {path: '', redirectTo: 'list', pathMatch: 'full'},
  {
    path: '', component: CampaignLimitsContainerComponent,
    children: [
      {
        path: 'list',
        loadChildren: () => import('./campaign-limits-list/campaign-limits-list.module').then(m => m.CampaignLimitsListModule)
      },
      {
        path: 'create',
        loadChildren: () => import('./campaign-limits/campaign-limits.module').then(m => m.CampaignLimitsModule)
      },
      {
        path: 'update/:id',
        loadChildren: () => import('./campaign-limits/campaign-limits.module').then(m => m.CampaignLimitsModule)
      }
    ]
  },
  {path: '**', redirectTo: 'list'}
]

@NgModule({
  declarations: [
    CampaignLimitsContainerComponent
  ],
  exports: [
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ]
})

export class CampaignLimitsContainerModule {
}
