import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ReportsByCampaignComponent} from './reports-by-campaign/reports-by-campaign.component';
import {ReportsByCustomerComponent} from './reports-by-customer/reports-by-customer.component';
import {RouterModule, Routes} from "@angular/router";
import {SharedModule} from "../../modules/shared.module";
import {ReactiveFormsModule} from "@angular/forms";
import {HttpClientModule} from "@angular/common/http";
import {NgxSmartModalModule} from "ngx-smart-modal";
import {AngularMyDatePickerModule} from "angular-mydatepicker";

const routes: Routes = [
  {path: '', redirectTo: 'reports-by-campaign', pathMatch: 'full'},
  {path: 'reports-by-campaign', component: ReportsByCampaignComponent},
  {path: 'reports-by-customer', component: ReportsByCustomerComponent}
]

@NgModule({
  declarations: [
    ReportsByCampaignComponent,
    ReportsByCustomerComponent
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
