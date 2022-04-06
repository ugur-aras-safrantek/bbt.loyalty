import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {CampaignLimitsListComponent} from './campaign-limits-list.component';
import {RouterModule, Routes} from "@angular/router";
import {SharedModule} from "../../../modules/shared.module";
import {ReactiveFormsModule} from "@angular/forms";
import {HttpClientModule} from "@angular/common/http";
import {AngularEditorModule} from "@kolkov/angular-editor";
import {NgxSmartModalModule} from "ngx-smart-modal";
import {AngularMyDatePickerModule} from "angular-mydatepicker";

const routes: Routes = [
  {
    path: '', component: CampaignLimitsListComponent
  }
]

@NgModule({
  declarations: [
    CampaignLimitsListComponent
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

export class CampaignLimitsListModule {
}
