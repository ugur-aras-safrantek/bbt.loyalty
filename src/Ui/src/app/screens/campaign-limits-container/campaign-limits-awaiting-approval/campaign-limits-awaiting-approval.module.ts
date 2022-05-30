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
  CampaignLimitsAwaitingApprovalListComponent
} from './campaign-limits-awaiting-approval-list/campaign-limits-awaiting-approval-list.component';
import {
  CampaignLimitsAwaitingApprovalDetailComponent
} from './campaign-limits-awaiting-approval-detail/campaign-limits-awaiting-approval-detail.component';

const routes: Routes = [
  {path: '', redirectTo: 'list', pathMatch: 'full'},
  {path: 'list', component: CampaignLimitsAwaitingApprovalListComponent},
  {path: 'detail/:id', component: CampaignLimitsAwaitingApprovalDetailComponent}
]

@NgModule({
  declarations: [
    CampaignLimitsAwaitingApprovalListComponent,
    CampaignLimitsAwaitingApprovalDetailComponent
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

export class CampaignLimitsAwaitingApprovalModule {
}
