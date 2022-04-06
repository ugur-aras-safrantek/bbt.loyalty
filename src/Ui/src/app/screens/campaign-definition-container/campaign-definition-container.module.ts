import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {CampaignDefinitionContainerComponent} from './campaign-definition-container.component';
import {RouterModule, Routes} from "@angular/router";

const routes: Routes = [
  {path: '', redirectTo: 'list', pathMatch: 'full'},
  {
    path: '', component: CampaignDefinitionContainerComponent,
    children: [
      {
        path: 'list',
        loadChildren: () => import('./campaign-definition-list/campaign-definition-list.module').then(m => m.CampaignDefinitionListModule)
      },
      {
        path: 'create',
        loadChildren: () => import('./campaign-definition/campaign-definition.module').then(m => m.CampaignDefinitionModule)
      },
      {
        path: 'create/:id',
        loadChildren: () => import('./campaign-definition/campaign-definition.module').then(m => m.CampaignDefinitionModule)
      },
      {
        path: 'create/:id/:repost',
        loadChildren: () => import('./campaign-definition/campaign-definition.module').then(m => m.CampaignDefinitionModule)
      }
    ]
  },
  {path: '**', redirectTo: 'list'}
]

@NgModule({
  declarations: [
    CampaignDefinitionContainerComponent
  ],
  exports: [
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ]
})
export class CampaignDefinitionContainerModule {
}
