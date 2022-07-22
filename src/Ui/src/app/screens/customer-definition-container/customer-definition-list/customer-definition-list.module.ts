import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {CustomerDefinitionListComponent} from './customer-definition-list.component';
import {RouterModule, Routes} from "@angular/router";
import {SharedModule} from "../../../modules/shared.module";
import {ReactiveFormsModule} from "@angular/forms";
import {HttpClientModule} from "@angular/common/http";
import {NgxSmartModalModule} from "ngx-smart-modal";

const routes: Routes = [
  {
    path: '', component: CustomerDefinitionListComponent
  }
]

@NgModule({
  declarations: [
    CustomerDefinitionListComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    SharedModule,
    ReactiveFormsModule,
    HttpClientModule,
    NgxSmartModalModule.forRoot(),
  ]
})

export class CustomerDefinitionListModule {
}
