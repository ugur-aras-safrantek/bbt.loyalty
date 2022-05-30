import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {CampaignDefinitionContainerComponent} from './campaign-definition-container.component';
import {RouterModule, Routes} from "@angular/router";
import {CampaignPreviewComponent} from "./campaign-definition/campaign-preview/campaign-preview.component";
import {NgxSmartModalModule} from "ngx-smart-modal";
import {AngularEditorModule} from "@kolkov/angular-editor";
import {SharedModule} from "../../modules/shared.module";

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
        path: 'awaiting-approval',
        loadChildren: () => import('./campaign-definition-awaiting-approval/campaign-definition-awaiting-approval.module').then(m => m.CampaignDefinitionAwaitingApprovalModule)
      },
      {
        path: 'create',
        loadChildren: () => import('./campaign-definition/campaign-definition.module').then(m => m.CampaignDefinitionModule)
      },
      {
        path: 'update/:id',
        loadChildren: () => import('./campaign-definition/campaign-definition.module').then(m => m.CampaignDefinitionModule)
      },
      {
        path: 'preview/:id',
        component: CampaignPreviewComponent
      },
    ]
  },
  {path: '**', redirectTo: 'list'}
]

@NgModule({
  declarations: [
    CampaignDefinitionContainerComponent,
    CampaignPreviewComponent
  ],
  exports: [],
  imports: [
    SharedModule,
    CommonModule,
    RouterModule.forChild(routes),
    NgxSmartModalModule.forRoot(),
    AngularEditorModule,
  ]
})

export class CampaignDefinitionContainerModule {
}
