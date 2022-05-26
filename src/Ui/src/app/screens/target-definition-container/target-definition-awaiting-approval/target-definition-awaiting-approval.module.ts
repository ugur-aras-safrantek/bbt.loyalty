import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RouterModule, Routes} from "@angular/router";
import {SharedModule} from "../../../modules/shared.module";
import {ReactiveFormsModule} from "@angular/forms";
import {HttpClientModule} from "@angular/common/http";
import {AngularEditorModule} from "@kolkov/angular-editor";
import {NgxSmartModalModule} from "ngx-smart-modal";
import {AngularMyDatePickerModule} from "angular-mydatepicker";
import {
  TargetDefinitionAwaitingApprovalListComponent
} from './target-definition-awaiting-approval-list/target-definition-awaiting-approval-list.component';
import {
  TargetDefinitionAwaitingApprovalDetailComponent
} from './target-definition-awaiting-approval-detail/target-definition-awaiting-approval-detail.component';

const routes: Routes = [
  {path: '', redirectTo: 'list', pathMatch: 'full'},
  {path: 'list', component: TargetDefinitionAwaitingApprovalListComponent},
  {path: 'detail/:id', component: TargetDefinitionAwaitingApprovalDetailComponent}
]

@NgModule({
  declarations: [
    TargetDefinitionAwaitingApprovalListComponent,
    TargetDefinitionAwaitingApprovalDetailComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    SharedModule,
    ReactiveFormsModule,
    HttpClientModule,
    AngularEditorModule,
    NgxSmartModalModule.forRoot(),
    AngularMyDatePickerModule,
  ]
})

export class TargetDefinitionAwaitingApprovalModule {
}
