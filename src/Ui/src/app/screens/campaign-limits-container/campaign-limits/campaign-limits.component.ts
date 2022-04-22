import {Component, OnInit, ViewChild} from '@angular/core';
import {StepService} from "../../../services/step.service";
import {CampaignLimitsService} from "../../../services/campaign-limits.service";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";
import {DropdownListModel} from "../../../models/dropdown-list.model";
import {Subject, take, takeUntil} from "rxjs";
import {CampaignLimitAddRequestModel, CampaignLimitUpdateRequestModel} from "../../../models/campaign-limits";
import {ToastrHandleService} from 'src/app/services/toastr-handle.service';
import {NgxSmartModalService} from 'ngx-smart-modal';
import {FormChangeAlertComponent} from "../../../components/form-change-alert/form-change-alert.component";
import {FormChange} from "../../../models/form-change";
import {UtilityService} from 'src/app/services/utility.service';

@Component({
  selector: 'app-campaign-limits',
  templateUrl: './campaign-limits.component.html',
  styleUrls: ['./campaign-limits.component.scss']
})

export class CampaignLimitsComponent implements OnInit, FormChange {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  @ViewChild(FormChangeAlertComponent) formChangeAlertComponent: FormChangeAlertComponent;
  formChangeSubject: Subject<boolean> = new Subject<boolean>();
  formChangeState = false;

  formGroup: FormGroup;

  dropdownSettings = this.utilityService.dropdownSettings;

  campaignList: DropdownListModel[];
  achievementFrequencyList: DropdownListModel[];
  currencyList: DropdownListModel[];

  stepData;
  repostData = this.campaignLimitsService.repostData;
  id: any;
  submitted = false;

  alertModalText = '';

  nextButtonVisible = true;

  constructor(private fb: FormBuilder,
              private stepService: StepService,
              private modalService: NgxSmartModalService,
              private utilityService: UtilityService,
              private toastrHandleService: ToastrHandleService,
              private campaignLimitsService: CampaignLimitsService,
              private router: Router,
              private route: ActivatedRoute) {
    this.route.paramMap.subscribe(paramMap => {
      this.id = paramMap.get('id');
    });

    this.stepService.setSteps(this.campaignLimitsService.stepData);
    this.stepService.updateStep(1);
    this.stepData = this.stepService.stepData;

    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      isActive: false,
      campaignIds: [[], [Validators.required, Validators.minLength(2)]],
      achievementFrequencyId: [1, Validators.required],
      type: 1,
      currencyId: [1, Validators.required],
      maxTopLimitAmount: [null, Validators.required],
      maxTopLimitRate: null,
      maxTopLimitUtilization: '',
    });

    if (this.id) {
      this.campaignLimitsService.repostData.id = this.id;
      this.stepService.finish();
      this.getLimitDetail();
    } else {
      this.campaignLimitGetInsertForm();
      this.formChangeState = true;
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

  typeChanged() {
    if (this.formGroup.get('type')?.value == 1) {
      this.f.currencyId.setValidators(Validators.required);
      this.f.maxTopLimitAmount.setValidators(Validators.required);

      this.formGroup.patchValue({maxTopLimitRate: null});
      this.f.maxTopLimitRate.clearValidators();
    } else {
      this.f.maxTopLimitRate.setValidators(Validators.required);

      this.formGroup.patchValue({
        currencyId: null,
        maxTopLimitAmount: null
      });
      this.f.currencyId.clearValidators();
      this.f.maxTopLimitAmount.clearValidators();
    }

    Object.keys(this.f).forEach(key => {
      this.formGroup.controls[key].updateValueAndValidity();
    });
  }

  private populateForm(data) {
    this.formGroup.patchValue({
      name: data.name,
      isActive: data.isActive,
      campaignIds: data.campaignIds,
      achievementFrequencyId: data.achievementFrequencyId,
      type: data.type,
      currencyId: data.currencyId,
      maxTopLimitAmount: data.maxTopLimitAmount,
      maxTopLimitRate: data.maxTopLimitRate,
      maxTopLimitUtilization: data.maxTopLimitUtilization,
    })
  }

  private populateLists(data: any) {
    this.campaignList = data.campaignList;
    this.achievementFrequencyList = data.achievementFrequencyList;
    this.currencyList = data.currencyList;
  }

  private createRequestModel(id: any) {
    let formGroup = this.formGroup.getRawValue();
    let requestModel;
    if (id) {
      requestModel = new CampaignLimitUpdateRequestModel();
      requestModel.id = id;
    } else {
      requestModel = new CampaignLimitAddRequestModel();
    }
    requestModel.name = formGroup.name;
    requestModel.isActive = formGroup.isActive;
    requestModel.campaignIds = formGroup.campaignIds.map(x => {
      return parseInt(x.id);
    });
    requestModel.achievementFrequencyId = formGroup.achievementFrequencyId;
    requestModel.type = formGroup.type;
    requestModel.maxTopLimitUtilization = parseInt(formGroup.maxTopLimitUtilization);
    switch (formGroup.type) {
      case 1:
      case "1":
        requestModel.currencyId = formGroup.currencyId;
        requestModel.maxTopLimitAmount = formGroup.maxTopLimitAmount;
        requestModel.maxTopLimitRate = null;
        break;
      case 2:
      case "2":
        requestModel.currencyId = null;
        requestModel.maxTopLimitAmount = null;
        requestModel.maxTopLimitRate = formGroup.maxTopLimitRate;
        break;
      default:
        requestModel.currencyId = null;
        requestModel.maxTopLimitAmount = null;
        requestModel.maxTopLimitRate = null;
        break;
    }
    return requestModel;
  }

  continue() {
    this.submitted = true;
    if (this.formGroup.valid) {
      this.alertModalText = this.id
        ? 'Yaptığınız değişiklikleri kaydetmeyi onaylıyor musunuz?'
        : 'Yeni çatı limitini kaydetmeyi onaylıyor musunuz?';
      this.modalService.open("campaignLimitsApproveAlertModal");
    }
  }

  alertModalOk() {
    this.id ? this.campaignLimitUpdate() : this.campaignLimitAdd();
  }

  private campaignLimitGetInsertForm() {
    this.campaignLimitsService.campaignLimitGetInsertForm()
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

  private getLimitDetail() {
    this.campaignLimitsService.getLimitDetail(this.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.populateLists(res.data);
            if (res.data.campaignTopLimit) {
              this.populateForm(res.data.campaignTopLimit);
              this.formGroup.patchValue({
                campaignIds: res.data.campaignTopLimit.campaigns
              });
              this.typeChanged();
            }
            this.nextButtonVisible = false;
            this.formGroup.valueChanges
              .pipe(take(1))
              .subscribe(x => {
                this.formChangeState = true;
                this.nextButtonVisible = true;
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

  private campaignLimitAdd() {
    let requestModel = this.createRequestModel(null);
    this.campaignLimitsService.campaignLimitAdd(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.formChangeState = false;
            this.router.navigate([`/campaign-limits/create/finish`], {relativeTo: this.route})
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

  private campaignLimitUpdate() {
    let requestModel = this.createRequestModel(this.id);
    this.campaignLimitsService.campaignLimitUpdate(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.formChangeState = false;
            this.router.navigate([`/campaign-limits/update/${this.id}/finish`], {relativeTo: this.route});
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

  copyLimit(event) {
    this.campaignLimitsService.copyLimit(event.id);
  }
}
