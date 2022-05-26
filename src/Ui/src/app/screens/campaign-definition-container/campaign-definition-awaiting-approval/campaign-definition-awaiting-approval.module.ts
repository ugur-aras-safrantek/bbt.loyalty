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
  CampaignDefinitionAwaitingApprovalListComponent
} from "./campaign-definition-awaiting-approval-list/campaign-definition-awaiting-approval-list.component";
import {
  CampaignDefinitionAwaitingApprovalDetailComponent
} from './campaign-definition-awaiting-approval-detail/campaign-definition-awaiting-approval-detail.component';

const routes: Routes = [
  {path: '', redirectTo: 'list', pathMatch: 'full'},
  {path: 'list', component: CampaignDefinitionAwaitingApprovalListComponent},
  {path: 'detail/:id', component: CampaignDefinitionAwaitingApprovalDetailComponent}
]

@NgModule({
  declarations: [
    CampaignDefinitionAwaitingApprovalListComponent,
    CampaignDefinitionAwaitingApprovalDetailComponent
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

export class CampaignDefinitionAwaitingApprovalModule {
}
