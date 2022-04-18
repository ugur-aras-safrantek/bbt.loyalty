import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {TargetDefinitionComponent} from './target-definition.component';
import {SharedModule} from "../../../modules/shared.module";
import {RouterModule, Routes} from "@angular/router";
import {TargetSourceComponent} from './target-source/target-source.component';
import {TargetFinishComponent} from './target-finish/target-finish.component';
import {ReactiveFormsModule} from "@angular/forms";
import {AngularEditorModule} from "@kolkov/angular-editor";
import {HttpClientModule} from "@angular/common/http";
import {TargetPreviewComponent} from './target-preview/target-preview.component';
import {NgxSmartModalModule} from "ngx-smart-modal";
import {FormChangeCheckGuard} from 'src/app/guards/form-change-check.guard';

const routes: Routes = [
  {path: 'definition', component: TargetDefinitionComponent, canDeactivate: [FormChangeCheckGuard]},
  {path: 'source', component: TargetSourceComponent, canDeactivate: [FormChangeCheckGuard]},
  {path: 'source/:newId', component: TargetSourceComponent, canDeactivate: [FormChangeCheckGuard]},
  {path: 'finish', component: TargetFinishComponent},
  {path: 'finish/:newId', component: TargetFinishComponent},
]

@NgModule({
  declarations: [
    TargetDefinitionComponent,
    TargetSourceComponent,
    TargetFinishComponent,
    TargetPreviewComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    RouterModule.forChild(routes),
    ReactiveFormsModule,
    HttpClientModule,
    NgxSmartModalModule.forRoot(),
    AngularEditorModule
  ]
})

export class TargetDefinitionModule {
}
