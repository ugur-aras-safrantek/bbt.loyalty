import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {CampaignDefinitionComponent} from "./campaign-definition.component";
import {SharedModule} from "../../../modules/shared.module";
import {RouterModule, Routes} from "@angular/router";
import {CampaignRulesComponent} from './campaign-rules/campaign-rules.component';
import {CampaignTargetSelectionComponent} from './campaign-target-selection/campaign-target-selection.component';
import {CampaignGainsComponent} from './campaign-gains/campaign-gains.component';
import {CampaignFinishComponent} from './campaign-finish/campaign-finish.component';
import {ReactiveFormsModule} from "@angular/forms";
import {HttpClientModule} from '@angular/common/http';
import {AngularEditorModule} from '@kolkov/angular-editor';
import {NgxSmartModalModule} from "ngx-smart-modal";
import {AngularMyDatePickerModule} from 'angular-mydatepicker';
import {FormChangeCheckGuard} from 'src/app/guards/form-change-check.guard';

const routes: Routes = [
  {path: 'definition', component: CampaignDefinitionComponent, canDeactivate: [FormChangeCheckGuard]},
  {path: 'rules', component: CampaignRulesComponent, canDeactivate: [FormChangeCheckGuard]},
  {path: 'rules/:newId', component: CampaignRulesComponent, canDeactivate: [FormChangeCheckGuard]},
  {path: 'target-selection', component: CampaignTargetSelectionComponent, canDeactivate: [FormChangeCheckGuard]},
  {path: 'target-selection/:newId', component: CampaignTargetSelectionComponent, canDeactivate: [FormChangeCheckGuard]},
  {path: 'gains', component: CampaignGainsComponent, canDeactivate: [FormChangeCheckGuard]},
  {path: 'gains/:newId', component: CampaignGainsComponent, canDeactivate: [FormChangeCheckGuard]},
  {path: 'finish', component: CampaignFinishComponent},
  {path: 'finish/:newId', component: CampaignFinishComponent},
]

@NgModule({
  declarations: [
    CampaignDefinitionComponent,
    CampaignRulesComponent,
    CampaignTargetSelectionComponent,
    CampaignGainsComponent,
    CampaignFinishComponent,
  ],
  imports: [
    SharedModule,
    CommonModule,
    RouterModule.forChild(routes),
    ReactiveFormsModule,
    HttpClientModule,
    AngularEditorModule,
    NgxSmartModalModule.forRoot(),
    AngularMyDatePickerModule,
  ]
})

export class CampaignDefinitionModule {
}
