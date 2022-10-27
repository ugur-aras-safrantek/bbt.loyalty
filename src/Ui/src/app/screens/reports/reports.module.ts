import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ReportsByCampaignComponent} from './reports-by-campaign/reports-by-campaign.component';
import {ReportsByEarningComponent} from './reports-by-earning/reports-by-earning.component';
import {RouterModule, Routes} from "@angular/router";
import {SharedModule} from "../../modules/shared.module";
import {ReactiveFormsModule} from "@angular/forms";
import {HttpClientModule} from "@angular/common/http";
import {NgxSmartModalModule} from "ngx-smart-modal";
import {AngularMyDatePickerModule} from "angular-mydatepicker";
import {ReportsByTargetComponent} from './reports-by-target/reports-by-target.component';

const routes: Routes = [
  {path: '', redirectTo: 'reports-by-campaign', pathMatch: 'full'},
  {path: 'reports-by-campaign', component: ReportsByCampaignComponent},
  {path: 'reports-by-earning', component: ReportsByEarningComponent},
  {path: 'reports-by-target', component: ReportsByTargetComponent},
]

@NgModule({
  declarations: [
    ReportsByCampaignComponent,
    ReportsByCustomerComponent,
    ReportsByTargetComponent,
  ],
  imports: [
    SharedModule,
    CommonModule,
    RouterModule.forChild(routes),
    ReactiveFormsModule,
    HttpClientModule,
    NgxSmartModalModule.forRoot(),
    AngularMyDatePickerModule,
  ]
})

export class ReportsModule {
}
