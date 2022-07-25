import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {CustomerDefinitionContainerComponent} from './customer-definition-container.component';
import {RouterModule, Routes} from "@angular/router";
import {NgxSmartModalModule} from "ngx-smart-modal";
import {SharedModule} from "../../modules/shared.module";

const routes: Routes = [
  {path: '', redirectTo: 'list', pathMatch: 'full'},
  {
    path: '', component: CustomerDefinitionContainerComponent,
    children: [
      {
        path: 'list',
        loadChildren: () => import('./customer-definition-list/customer-definition-list.module').then(m => m.CustomerDefinitionListModule)
      }
    ]
  },
  {path: '**', redirectTo: 'list'}
]

@NgModule({
  declarations: [
    CustomerDefinitionContainerComponent,
  ],
  exports: [],
  imports: [
    SharedModule,
    CommonModule,
    RouterModule.forChild(routes),
    NgxSmartModalModule.forRoot(),
  ]
})

export class CustomerDefinitionContainerModule {
}
