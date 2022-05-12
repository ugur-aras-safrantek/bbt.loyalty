import {Component, OnInit, ViewChild} from '@angular/core';
import {StepService} from "../../../services/step.service";
import {TargetDefinitionService} from "../../../services/target-definition.service";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";
import {Subject, take, takeUntil} from "rxjs";
import {UtilityService} from "../../../services/utility.service";
import {
  TargetDefinitionAddRequestModel,
  TargetDefinitionUpdateRequestModel
} from "../../../models/target-definition";
import {ToastrHandleService} from 'src/app/services/toastr-handle.service';
import {NgxSmartModalService} from "ngx-smart-modal";
import {TargetPreviewComponent} from "./target-preview/target-preview.component";
import {FormChange} from "../../../models/form-change";
import {FormChangeAlertComponent} from "../../../components/form-change-alert/form-change-alert.component";
import {LoginService} from 'src/app/services/login.service';
import {AuthorizationModel} from 'src/app/models/login.model';

@Component({
  selector: 'app-target-definition',
  templateUrl: './target-definition.component.html',
  styleUrls: ['./target-definition.component.scss']
})

export class TargetDefinitionComponent implements OnInit, FormChange {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  currentUserAuthorizations: AuthorizationModel = new AuthorizationModel();

  @ViewChild(FormChangeAlertComponent) formChangeAlertComponent: FormChangeAlertComponent;
  formChangeSubject: Subject<boolean> = new Subject<boolean>();
  formChangeState = false;

  @ViewChild(TargetPreviewComponent) targetPreviewComponent: TargetPreviewComponent;

  formGroup: FormGroup;

  stepData;
  repostData = this.targetDefinitionService.repostData;
  id: any;
  submitted = false;

  nextButtonText = 'Devam';
  nextButtonVisible = true;
  nextButtonAuthority = false;

  constructor(private fb: FormBuilder,
              private stepService: StepService,
              private toastrHandleService: ToastrHandleService,
              private utilityService: UtilityService,
              private loginService: LoginService,
              private modalService: NgxSmartModalService,
              private targetDefinitionService: TargetDefinitionService,
              private router: Router,
              private route: ActivatedRoute) {
    this.currentUserAuthorizations = this.loginService.getCurrentUserAuthorizations().targetDefinitionModuleAuthorizations;

    this.route.paramMap.subscribe(paramMap => {
      this.id = paramMap.get('id');
    });

    this.stepService.setSteps(this.targetDefinitionService.stepData);
    this.stepService.updateStep(1);
    this.stepData = this.stepService.stepData;

    this.formGroup = this.fb.group({
      id: null,
      name: ['', Validators.required],
      title: ['', Validators.required],
      isActive: false
    })

    if (this.currentUserAuthorizations.view) {
      if (this.id) {
        this.targetDefinitionService.repostData.id = this.id;
        this.stepService.finish();
        this.getTargetDetail();
      } else {
        this.formChangeState = true;
        this.setAuthorization(this.currentUserAuthorizations.create);
      }
    } else {
      this.setAuthorization(false);
    }
  }

  private setAuthorization(authority: boolean) {
    this.nextButtonAuthority = authority;
    if (!authority) {
      this.formGroup.disable();
    }
  }

  openFormChangeAlertModal() {
    this.formChangeAlertComponent.openAlertModal();
    this.formChangeSubject = this.formChangeAlertComponent.subject;
  }

  ngOnInit(): void {
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
    this.formChangeSubject.unsubscribe();
  }

  get f() {
    return this.formGroup.controls;
  }

  populateForm(data) {
    this.formGroup.patchValue({
      id: data.id,
      name: data.name,
      title: data.title,
      isActive: data.isActive,
    })
  }

  continue() {
    this.submitted = true;
    if (this.formGroup.valid) {
      this.id ? this.targetDefinitionUpdate() : this.targetDefinitionAdd();
    }
  }

  copyTarget(event) {
    this.targetDefinitionService.copyTarget(event.id);
  }

  previewTarget(event) {
    this.targetPreviewComponent.getTargetInfo(event.id);
    this.modalService.open("previewModal");
  }

  private getTargetDetail() {
    this.targetDefinitionService.getTargetDetail(this.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.populateForm(res.data);
            this.nextButtonText = "Kaydet ve ilerle";
            this.nextButtonVisible = false;
            this.formGroup.valueChanges
              .pipe(take(1))
              .subscribe(x => {
                this.formChangeState = true;
                this.nextButtonVisible = true;
              });
            this.targetDefinitionService.repostData.previewButtonVisible = res.data.targetDetail?.targetViewTypeId == 3 ? false : true;
            this.setAuthorization(this.currentUserAuthorizations.update);
          } else
            this.toastrHandleService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrHandleService.error(err.error);
        }
      });
  }

  private targetDefinitionAdd() {
    let formGroup = this.formGroup.getRawValue();
    let requestModel: TargetDefinitionAddRequestModel = {
      name: formGroup.name,
      title: formGroup.title,
      isActive: formGroup.isActive,
    };
    this.targetDefinitionService.targetDefinitionAdd(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.formChangeState = false;
            this.router.navigate([`/target-definition/create/source/${res.data.id}`], {relativeTo: this.route});
            this.toastrHandleService.success();
          } else
            this.toastrHandleService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrHandleService.error(err.error);
        }
      });
  }

  private targetDefinitionUpdate() {
    let formGroup = this.formGroup.getRawValue();
    let requestModel: TargetDefinitionUpdateRequestModel = {
      id: this.id,
      name: formGroup.name,
      title: formGroup.title,
      isActive: formGroup.isActive,
    }
    this.targetDefinitionService.targetDefinitionUpdate(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.targetDefinitionService.isTargetValuesChanged = true;
            this.formChangeState = false;
            this.router.navigate([`/target-definition/update/${res.data.id}/source`], {relativeTo: this.route});
            this.toastrHandleService.success();
          } else
            this.toastrHandleService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrHandleService.error(err.error);
        }
      });
  }
}
