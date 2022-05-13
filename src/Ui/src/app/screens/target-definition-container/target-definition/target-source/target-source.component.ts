import {Component, OnInit, ViewChild} from '@angular/core';
import {StepService} from "../../../../services/step.service";
import {TargetDefinitionService} from "../../../../services/target-definition.service";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";
import {Subject, take, takeUntil} from "rxjs";
import {DropdownListModel} from "../../../../models/dropdown-list.model";
import {AngularEditorConfig} from "@kolkov/angular-editor";
import {TargetSourceAddUpdateRequestModel} from "../../../../models/target-definition";
import {ToastrHandleService} from 'src/app/services/toastr-handle.service';
import {TargetPreviewComponent} from "../target-preview/target-preview.component";
import {NgxSmartModalService} from "ngx-smart-modal";
import {FormChange} from "../../../../models/form-change";
import {FormChangeAlertComponent} from "../../../../components/form-change-alert/form-change-alert.component";
import {LoginService} from 'src/app/services/login.service';
import {AuthorizationModel} from 'src/app/models/login.model';

@Component({
  selector: 'app-target-source',
  templateUrl: './target-source.component.html',
  styleUrls: ['./target-source.component.scss']
})

export class TargetSourceComponent implements OnInit, FormChange {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  currentUserAuthorizations: AuthorizationModel = new AuthorizationModel();

  @ViewChild(FormChangeAlertComponent) formChangeAlertComponent: FormChangeAlertComponent;
  formChangeSubject: Subject<boolean> = new Subject<boolean>();
  formChangeState = false;

  @ViewChild(TargetPreviewComponent) targetPreviewComponent: TargetPreviewComponent;

  editorConfig: AngularEditorConfig = {
    editable: true
  }

  formGroup: FormGroup;
  submitted = false;

  totalAmountDisabled = false;
  numberOfTransactionDisabled = false;

  alertModalText = '';

  targetSourceList: DropdownListModel[];
  targetViewTypeList: DropdownListModel[];
  triggerTimeList: DropdownListModel[];
  verificationTimeList: DropdownListModel[];

  id: any;
  newTargetId: any;
  stepData;
  repostData = this.targetDefinitionService.repostData;

  nextButtonVisible = true;
  nextButtonAuthority = false;

  constructor(private fb: FormBuilder,
              private stepService: StepService,
              private toastrHandleService: ToastrHandleService,
              private modalService: NgxSmartModalService,
              private loginService: LoginService,
              private targetDefinitionService: TargetDefinitionService,
              private router: Router,
              private route: ActivatedRoute) {
    this.currentUserAuthorizations = this.loginService.getCurrentUserAuthorizations().targetDefinitionModuleAuthorizations;

    this.route.paramMap.subscribe(paramMap => {
      this.id = paramMap.get('id');
      this.newTargetId = paramMap.get('newId');
    });

    this.stepService.setSteps(this.targetDefinitionService.stepData);
    this.stepService.updateStep(2);
    this.stepData = this.stepService.stepData;

    if (this.currentUserAuthorizations.view) {
      if (this.id) {
        this.targetDefinitionService.repostData.id = this.id;

        this.stepService.finish();

        this.nextButtonVisible = false;
        if (this.targetDefinitionService.isTargetValuesChanged) {
          this.nextButtonVisible = true;
        }

        this.getTargetSource();
      } else {
        this.targetSourceGetInsertForm();
        this.formChangeState = true;
      }
    } else {
      this.setAuthorization(false);
    }

    this.formGroup = this.fb.group({
      targetSourceId: [null, Validators.required],
      targetViewTypeId: [null, Validators.required],

      flowName: '',
      totalAmount: null,
      numberOfTransaction: '',
      flowFrequency: '',
      additionalFlowTime: '',
      triggerTimeId: null,

      condition: '',
      query: '',
      verificationTimeId: null,

      targetDetailTr: '',
      targetDetailEn: '',
      descriptionTr: '',
      descriptionEn: '',
    });
  }

  private setAuthorization(authority: boolean) {
    this.nextButtonAuthority = authority;
    if (!authority) {
      this.editorConfig = {
        editable: false,
        showToolbar: false
      };
      this.formGroup.disable();
      this.formChangeState = false;
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

  targetSourceChanged() {
    if (this.formGroup.get('targetSourceId')?.value == 1) {
      this.f.flowName.setValidators(Validators.required);
      this.f.totalAmount.setValidators([Validators.required, Validators.min(0.01)]);
      this.f.numberOfTransaction.setValidators([Validators.required, Validators.min(1)]);
      this.f.flowFrequency.setValidators(Validators.required);
      this.f.triggerTimeId.setValidators(Validators.required);

      this.formGroup.patchValue({
        condition: '',
        query: '',
        verificationTimeId: null,
      });
      this.f.condition.clearValidators();
      this.f.query.clearValidators();
      this.f.verificationTimeId.clearValidators();
    } else if (this.formGroup.get('targetSourceId')?.value == 2) {
      this.f.condition.setValidators(Validators.required);
      this.f.query.setValidators(Validators.required);
      this.f.verificationTimeId.setValidators(Validators.required);

      this.formGroup.patchValue({
        flowName: '',
        totalAmount: null,
        numberOfTransaction: '',
        flowFrequency: '',
        triggerTimeId: null,
      });
      this.f.flowName.clearValidators();
      this.f.totalAmount.clearValidators();
      this.f.numberOfTransaction.clearValidators();
      this.f.flowFrequency.clearValidators();
      this.f.triggerTimeId.clearValidators();
    } else {
      this.formGroup.patchValue({
        targetViewTypeId: null,

        flowName: '',
        totalAmount: null,
        numberOfTransaction: '',
        flowFrequency: '',
        additionalFlowTime: null,
        triggerTimeId: null,

        condition: '',
        query: '',
        verificationTimeId: null,
      });

      this.f.flowName.clearValidators();
      this.f.totalAmount.clearValidators();
      this.f.numberOfTransaction.clearValidators();
      this.f.flowFrequency.clearValidators();
      this.f.triggerTimeId.clearValidators();

      this.f.condition.clearValidators();
      this.f.query.clearValidators();
      this.f.verificationTimeId.clearValidators();
    }
    Object.keys(this.f).forEach(key => {
      this.formGroup.controls[key].updateValueAndValidity();
    });
  }

  targetViewTypeIdChanged(value: any) {
    if (value == 3) {
      this.formGroup.patchValue({
        targetDetailTr: '',
        targetDetailEn: '',
        descriptionTr: '',
        descriptionEn: '',
      });
    }
  }

  totalAmountChanged() {
      let value = this.formGroup.get('totalAmount')?.value;
      if (value && value >= 0) {
          this.numberOfTransactionDisabled = true;
          this.f.numberOfTransaction.clearValidators();
          this.formGroup.patchValue({numberOfTransaction: ''});
      } else {
          this.f.numberOfTransaction.setValidators([Validators.required, Validators.min(1)]);
          this.numberOfTransactionDisabled = false;
      }
      this.f.numberOfTransaction.updateValueAndValidity();
  }

  numberOfTransactionChanged() {
      let value = this.formGroup.get('numberOfTransaction')?.value;
      if (value && value != '') {
          this.totalAmountDisabled = true;
          this.f.totalAmount.clearValidators();
          this.formGroup.patchValue({totalAmount: null});
      } else {
          this.totalAmountDisabled = false;
          this.f.totalAmount.setValidators([Validators.required, Validators.min(0.01)]);
      }
      this.f.totalAmount.updateValueAndValidity();
  }

  targetDetailTrChanged(value: string) {
    if (value == '' || value == null) {
      this.formGroup.patchValue({targetDetailEn: value});
      this.f.targetDetailEn.clearValidators();
    } else {
      this.f.targetDetailEn.setValidators(Validators.required);
    }
    this.f.targetDetailEn.updateValueAndValidity();
  }

  targetDetailEnChanged(value: string) {
    if (value != '' && value != null) {
      this.f.targetDetailTr.setValidators(Validators.required);
    } else {
      this.f.targetDetailTr.clearValidators();
    }
    this.f.targetDetailTr.updateValueAndValidity();
  }

  descriptionTrChanged(value: string) {
    if (value == '' || value == null) {
      this.formGroup.patchValue({descriptionEn: value});
      this.f.descriptionEn.clearValidators();
    } else {
      this.f.descriptionEn.setValidators(Validators.required);
    }
    this.f.descriptionEn.updateValueAndValidity();
  }

  descriptionEnChanged(value: string) {
    if (value != '' && value != null) {
      this.f.descriptionTr.setValidators(Validators.required);
    } else {
      this.f.descriptionTr.clearValidators();
    }
    this.f.descriptionTr.updateValueAndValidity();
  }

  private populateForm(data) {
    this.formGroup.patchValue({
      targetSourceId: data.targetSourceId,

      targetViewTypeId: data.targetViewTypeId,
      flowName: data.flowName,
      totalAmount: data.totalAmount,
      numberOfTransaction: data.numberOfTransaction,
      flowFrequency: data.flowFrequency,
      additionalFlowTime: data.additionalFlowTime,
      triggerTimeId: data.triggerTimeId,

      condition: data.condition,
      query: data.query,
      verificationTimeId: data.verificationTimeId,

      targetDetailEn: data.targetDetailEn,
      targetDetailTr: data.targetDetailTr,
      descriptionEn: data.descriptionEn,
      descriptionTr: data.descriptionTr,
    })
  }

  private populateLists(data: any) {
    this.targetSourceList = data.targetSourceList;
    this.targetViewTypeList = data.targetViewTypeList;
    this.triggerTimeList = data.triggerTimeList;
    this.verificationTimeList = data.verificationTimeList;
  }

  private createRequestModel() {
    let formGroup = this.formGroup.getRawValue();
    let requestModel = new TargetSourceAddUpdateRequestModel();

    requestModel.targetId = this.id ?? this.newTargetId;
    requestModel.targetSourceId = formGroup.targetSourceId;
    requestModel.targetViewTypeId = formGroup.targetViewTypeId;
    requestModel.targetDetailTr = formGroup.targetDetailTr;
    requestModel.targetDetailEn = formGroup.targetDetailEn;
    requestModel.descriptionTr = formGroup.descriptionTr;
    requestModel.descriptionEn = formGroup.descriptionEn;
    switch (formGroup.targetSourceId) {
      case 1:
      case "1":
        requestModel.flowName = formGroup.flowName;
        requestModel.totalAmount = formGroup.totalAmount;
        requestModel.numberOfTransaction = parseInt(formGroup.numberOfTransaction);
        requestModel.flowFrequency = formGroup.flowFrequency;
        requestModel.additionalFlowTime = formGroup.additionalFlowTime;
        requestModel.triggerTimeId = formGroup.triggerTimeId;
        break;
      case 2:
      case "2":
        requestModel.condition = formGroup.condition;
        requestModel.query = formGroup.query;
        requestModel.verificationTimeId = formGroup.verificationTimeId;
        break;
    }
    return requestModel;
  }

  continue() {
    this.submitted = true;
    if (this.formGroup.valid) {
      this.alertModalText = this.id
        ? 'Yaptığınız değişiklikleri kaydetmeyi onaylıyor musunuz?'
        : 'Yeni hedef tanımını kaydetmeyi onaylıyor musunuz?';
      this.modalService.open("campaignTargetsApproveAlertModal");
    }
  }

  alertModalOk() {
    this.newTargetId ? this.targetSourceAdd() : this.targetSourceUpdate();
  }

  private targetSourceGetInsertForm() {
    this.targetDefinitionService.targetSourceGetInsertForm()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.populateLists(res.data);
            this.setAuthorization(this.currentUserAuthorizations.create);
          } else
            this.toastrHandleService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrHandleService.error(err.error);
        }
      });
  }

  private getTargetSource() {
    this.targetDefinitionService.getTargetSource(this.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.populateLists(res.data);
            if (res.data.targetDetail) {
              this.populateForm(res.data.targetDetail);
              this.totalAmountChanged();
              this.numberOfTransactionChanged();
            }
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

  private targetSourceAdd() {
    let requestModel = this.createRequestModel();
    this.targetDefinitionService.targetSourceAdd(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.formChangeState = false;
            this.router.navigate([`/target-definition/create/finish/${this.newTargetId}`], {relativeTo: this.route});
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

  private targetSourceUpdate() {
    let requestModel = this.createRequestModel();
    this.targetDefinitionService.targetSourceUpdate(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.formChangeState = false;
            this.router.navigate([`/target-definition/update/${this.id}/finish`], {relativeTo: this.route});
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

  copyTarget(event) {
    this.targetDefinitionService.copyTarget(event.id);
  }

  previewTarget(event) {
    this.targetPreviewComponent.getTargetInfo(event.id);
    this.modalService.open("previewModal");
  }
}
