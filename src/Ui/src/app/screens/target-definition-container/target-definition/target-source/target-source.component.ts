import {Component, OnInit} from '@angular/core';
import {StepService} from "../../../../services/step.service";
import {TargetDefinitionService} from "../../../../services/target-definition.service";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";
import {Subject, take, takeUntil} from "rxjs";
import {DropdownListModel} from "../../../../models/dropdown-list.model";
import {AngularEditorConfig} from "@kolkov/angular-editor";
import {TargetSourceAddUpdateRequestModel} from "../../../../models/target-definition";
import {ToastrHandleService} from 'src/app/services/toastr-handle.service';

@Component({
  selector: 'app-target-source',
  templateUrl: './target-source.component.html',
  styleUrls: ['./target-source.component.scss']
})

export class TargetSourceComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  editorConfig: AngularEditorConfig = {
    editable: true
  }

  formGroup: FormGroup;
  submitted = false;

  targetSourceList: DropdownListModel[];
  targetViewTypeList: DropdownListModel[];
  triggerTimeList: DropdownListModel[];
  verificationTimeList: DropdownListModel[];

  id: any;
  newTargetId: any;
  stepData;
  repostData = this.targetDefinitionService.repostData;

  nextButtonVisible = true;

  constructor(private fb: FormBuilder,
              private stepService: StepService,
              private toastrHandleService: ToastrHandleService,
              private targetDefinitionService: TargetDefinitionService,
              private router: Router,
              private route: ActivatedRoute) {
    this.route.paramMap.subscribe(paramMap => {
      this.id = paramMap.get('detailId');
      this.newTargetId = paramMap.get('newId');
    });

    this.stepService.setSteps(this.targetDefinitionService.stepData);
    this.stepService.updateStep(2);
    this.stepData = this.stepService.stepData;

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
    }

    this.formGroup = this.fb.group({
      targetSourceId: [null, Validators.required],

      targetViewTypeId: null,
      flowName: '',
      totalAmount: '',
      numberOfTransaction: '',
      flowFrequency: '',
      additionalFlowTime: '',
      triggerTimeId: null,

      condition: '',
      query: '',
      verificationTimeId: null,

      targetDetailEn: '',
      targetDetailTr: '',
      descriptionEn: '',
      descriptionTr: '',
    });
  }

  ngOnInit(): void {
  }

  ngOnDestroy() {
    this.targetDefinitionService.targetFormChanged(false);

    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  get f() {
    return this.formGroup.controls;
  }

  targetSourceChanged() {
    if (this.formGroup.get('targetSourceId')?.value == 1) {
      this.f.targetViewTypeId.setValidators(Validators.required);
      this.f.flowName.setValidators(Validators.required);
      this.f.totalAmount.setValidators(Validators.required);
      this.f.numberOfTransaction.setValidators(Validators.required);
      this.f.flowFrequency.setValidators(Validators.required);
      this.f.triggerTimeId.setValidators(Validators.required);

      this.f.condition.clearValidators();
      this.f.query.clearValidators();
      this.f.verificationTimeId.clearValidators();
    } else {
      this.f.condition.setValidators(Validators.required);
      this.f.query.setValidators(Validators.required);
      this.f.verificationTimeId.setValidators(Validators.required);

      this.f.targetViewTypeId.clearValidators();
      this.f.flowName.clearValidators();
      this.f.totalAmount.clearValidators();
      this.f.numberOfTransaction.clearValidators();
      this.f.flowFrequency.clearValidators();
      this.f.triggerTimeId.clearValidators();
    }
    Object.keys(this.f).forEach(key => {
      this.formGroup.controls[key].updateValueAndValidity();
    });
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
    requestModel.targetDetailEn = formGroup.targetDetailEn;
    requestModel.targetDetailTr = formGroup.targetDetailTr;
    requestModel.descriptionEn = formGroup.descriptionEn;
    requestModel.descriptionTr = formGroup.descriptionTr;
    switch (formGroup.targetSourceId) {
      case 1:
      case "1":
        requestModel.targetViewTypeId = formGroup.targetViewTypeId;
        requestModel.flowName = formGroup.flowName;
        requestModel.totalAmount = parseFloat(formGroup.totalAmount);
        requestModel.numberOfTransaction = parseInt(formGroup.numberOfTransaction);
        requestModel.flowFrequency = formGroup.flowFrequency;
        requestModel.additionalFlowTime = parseInt(formGroup.additionalFlowTime);
        requestModel.triggerTimeId = parseInt(formGroup.triggerTimeId);
        break;
      case 2:
      case "2":
        requestModel.targetViewTypeId = formGroup.targetViewTypeId;
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
      this.newTargetId ? this.targetSourceAdd() : this.targetSourceUpdate();
    }
  }

  private targetSourceGetInsertForm() {
    this.targetDefinitionService.targetSourceGetInsertForm()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.populateLists(res.data);
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
    let targetId = this.id;
    this.targetDefinitionService.getTargetSource(targetId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.populateLists(res.data);
            if (res.data.targetDetail) {
              this.populateForm(res.data.targetDetail);
            }
            this.formGroup.valueChanges
              .pipe(take(1))
              .subscribe(x => {
                this.nextButtonVisible = true;
                this.targetDefinitionService.targetFormChanged(true);
              });
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
            this.router.navigate([`/target-definition/create/finish`], {relativeTo: this.route});
            this.toastrHandleService.success("İşlem başarılı");
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
            this.router.navigate(['../finish'], {relativeTo: this.route});
            this.toastrHandleService.success("İşlem başarılı");
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
}
