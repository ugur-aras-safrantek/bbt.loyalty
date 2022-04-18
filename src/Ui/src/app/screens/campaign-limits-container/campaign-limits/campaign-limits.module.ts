import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {CampaignLimitsComponent} from "./campaign-limits.component";
import {SharedModule} from "../../../modules/shared.module";
import {RouterModule, Routes} from "@angular/router";
import {CampaignLimitsFinishComponent} from './campaign-limits-finish/campaign-limits-finish.component';
import {ReactiveFormsModule} from "@angular/forms";
import {HttpClientModule} from "@angular/common/http";
import {NgxSmartModalModule} from "ngx-smart-modal";
import {FormChangeCheckGuard} from 'src/app/guards/form-change-check.guard';

const routes: Routes = [
  {path: 'limit', component: CampaignLimitsComponent, canDeactivate: [FormChangeCheckGuard]},
  {path: 'finish', component: CampaignLimitsFinishComponent},
]

@NgModule({
  declarations: [
    CampaignLimitsComponent,
    CampaignLimitsFinishComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    RouterModule.forChild(routes),
    ReactiveFormsModule,
    HttpClientModule,
    NgxSmartModalModule.forRoot(),
  ]
})

export class CampaignLimitsModule {
}
