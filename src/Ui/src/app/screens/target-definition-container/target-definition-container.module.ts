import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {TargetDefinitionContainerComponent} from './target-definition-container.component';
import {RouterModule, Routes} from "@angular/router";

const routes: Routes = [
  {path: '', redirectTo: 'list', pathMatch: 'full'},
  {
    path: '', component: TargetDefinitionContainerComponent,
    children: [
      {
        path: 'list',
        loadChildren: () => import('./target-definition-list/target-definition-list.module').then(m => m.TargetDefinitionListModule)
      },
      {
        path: 'awaiting-approval',
        loadChildren: () => import('./target-definition-awaiting-approval/target-definition-awaiting-approval.module').then(m => m.TargetDefinitionAwaitingApprovalModule)
      },
      {
        path: 'create',
        loadChildren: () => import('./target-definition/target-definition.module').then(m => m.TargetDefinitionModule)
      },
      {
        path: 'update/:id',
        loadChildren: () => import('./target-definition/target-definition.module').then(m => m.TargetDefinitionModule)
      }
    ]
  },
  {path: '**', redirectTo: 'list'}
]

@NgModule({
  declarations: [
    TargetDefinitionContainerComponent
  ],
  exports: [
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ]
})

export class TargetDefinitionContainerModule {
}
