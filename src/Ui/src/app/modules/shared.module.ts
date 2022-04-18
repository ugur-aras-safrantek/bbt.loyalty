import {NgModule} from '@angular/core';
import {MainContentComponent} from "../components/main-content/main-content.component";
import {CommonModule, DecimalPipe} from "@angular/common";
import {BackButtonDirective} from "../directives/back-button.directive";
import {RouterModule} from "@angular/router";
import {OnlyNumberDirective} from "../directives/only-number.directive";
import {StepComponent} from "../components/step/step.component";
import {UiSwitchModule} from 'ngx-ui-switch';
import {FinishComponent} from "../components/finish/finish.component";
import {TurkishLiraDirective} from "../directives/turkish-lira.directive";
import {ListComponent} from "../components/list/list.component";
import {NgMultiSelectDropDownModule} from 'ng-multiselect-dropdown';
import {RepostComponent} from "../components/repost/repost.component";
import {NgxSmartModalModule} from "ngx-smart-modal";
import {CurrencyMaskInputMode, NgxCurrencyModule} from "ngx-currency";
import {SafePipe} from "../pipes/safe.pipe";
import {FormChangeAlertComponent} from "../components/form-change-alert/form-change-alert.component";

export const customCurrencyMaskConfig = {
  thousands: ".",
  decimal: ",",
  precision: 2,
  prefix: "",
  suffix: "",
  align: "left",
  allowNegative: false,
  allowZero: true,
  nullable: true,
  inputMode: CurrencyMaskInputMode.NATURAL,
};

@NgModule({
  declarations: [
    MainContentComponent,
    BackButtonDirective,
    OnlyNumberDirective,
    SafePipe,
    StepComponent,
    FinishComponent,
    ListComponent,
    RepostComponent,
    FormChangeAlertComponent,
    TurkishLiraDirective
  ],
  imports: [
    CommonModule,
    RouterModule,
    NgMultiSelectDropDownModule,
    UiSwitchModule,
    NgxSmartModalModule.forRoot(),
    NgxCurrencyModule.forRoot(customCurrencyMaskConfig),
  ],
  exports: [
    MainContentComponent,
    BackButtonDirective,
    OnlyNumberDirective,
    SafePipe,
    StepComponent,
    NgMultiSelectDropDownModule,
    UiSwitchModule,
    NgxCurrencyModule,
    FinishComponent,
    ListComponent,
    RepostComponent,
    FormChangeAlertComponent,
    TurkishLiraDirective
  ],
  providers: [DecimalPipe]
})

export class SharedModule {
}
