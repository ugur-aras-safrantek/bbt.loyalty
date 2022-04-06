import {Component, OnInit} from '@angular/core';
import {StepService} from "../../../services/step.service";
import {CampaignLimitsService} from "../../../services/campaign-limits.service";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";
import {DropdownListModel} from "../../../models/dropdown-list.model";
import {Subject, take, takeUntil} from "rxjs";
import {CampaignLimitAddRequestModel, CampaignLimitUpdateRequestModel} from "../../../models/campaign-limits";

@Component({
  selector: 'app-campaign-limits',
  templateUrl: './campaign-limits.component.html',
  styleUrls: ['./campaign-limits.component.scss']
})

export class CampaignLimitsComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  formGroup: FormGroup;

  campaignList: DropdownListModel[];
  achievementFrequencyList: DropdownListModel[];
  currencyList: DropdownListModel[];

  stepData;
  repostData = this.campaignLimitsService.repostData;
  id: any;
  submitted = false;

  nextButtonVisible = true;

  constructor(private fb: FormBuilder,
              private stepService: StepService,
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
      campaignIds: [[], Validators.required],
      achievementFrequencyId: [null, Validators.required],
      type: 1,
      currencyId: [1, Validators.required],
      maxTopLimitAmount: ['', Validators.required],
      maxTopLimitRate: '',
      maxTopLimitUtilization: '',
    });

    if (this.id) {
      this.campaignLimitsService.repostData.id = this.id;
      this.stepService.finish();
      this.getLimitDetail();
    } else {
      this.campaignLimitGetInsertForm();
    }
  }

  ngOnInit(): void {
  }

  ngOnDestroy() {
    this.campaignLimitsService.limitFormChanged(false);

    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  get f() {
    return this.formGroup.controls;
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
    requestModel.campaignIds = formGroup.campaignIds;
    requestModel.achievementFrequencyId = formGroup.achievementFrequencyId;
    requestModel.type = formGroup.type;
    requestModel.maxTopLimitUtilization = parseInt(formGroup.maxTopLimitUtilization);
    switch (formGroup.type) {
      case 1:
      case "1":
        requestModel.currencyId = formGroup.currencyId;
        requestModel.maxTopLimitAmount = parseInt(formGroup.maxTopLimitAmount);
        requestModel.maxTopLimitRate = null;
        break;
      case 2:
      case "2":
        requestModel.currencyId = null;
        requestModel.maxTopLimitAmount = null;
        requestModel.maxTopLimitRate = parseInt(formGroup.maxTopLimitRate);
        break;
      default:
        requestModel.currencyId = null;
        requestModel.maxTopLimitAmount = null;
        requestModel.maxTopLimitRate = null;
        break;
    }
    return requestModel;
  }

  typeChanged() {
    if (this.formGroup.get('type')?.value == 1) {
      this.f.currencyId.setValidators(Validators.required);
      this.f.currencyId.updateValueAndValidity();
      this.f.maxTopLimitAmount.setValidators(Validators.required);
      this.f.maxTopLimitAmount.updateValueAndValidity();

      this.formGroup.patchValue({maxTopLimitRate: ''});
      this.f.maxTopLimitRate.clearValidators();
      this.f.maxTopLimitRate.updateValueAndValidity();
    } else {
      this.f.maxTopLimitRate.setValidators(Validators.required);
      this.f.maxTopLimitRate.updateValueAndValidity();

      this.formGroup.patchValue({
        currencyId: 1,
        maxTopLimitAmount: ''
      });
      this.f.currencyId.clearValidators();
      this.f.currencyId.updateValueAndValidity();
      this.f.maxTopLimitAmount.clearValidators();
      this.f.maxTopLimitAmount.updateValueAndValidity();
    }
  }

  continue() {
    this.submitted = true;
    if (this.formGroup.valid) {
      this.id ? this.campaignLimitUpdate() : this.campaignLimitAdd();
    }
  }

  private campaignLimitGetInsertForm() {
    this.campaignLimitsService.campaignLimitGetInsertForm()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.populateLists(res.data);
          } else
            console.error("Hata oluştu");
        },
        error: err => {
          if (err.error.hasError)
            console.error(err.error.errorMessage);
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
                campaignIds: res.data.campaignTopLimit.campaigns.map(x => {
                  return x.id
                })
              });
              this.typeChanged();
            }
            this.nextButtonVisible = false;
            this.formGroup.valueChanges
              .pipe(take(1))
              .subscribe(x => {
                this.nextButtonVisible = true;
                this.campaignLimitsService.limitFormChanged(true);
              });
          } else
            console.error("Hata oluştu");
        },
        error: err => {
          if (err.error.hasError)
            console.error(err.error.errorMessage);
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
            this.router.navigate(['../finish'], {relativeTo: this.route});
          } else
            console.error("Hata oluştu");
        },
        error: err => {
          if (err.error.hasError)
            console.error(err.error.errorMessage);
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
            this.router.navigate(['./finish'], {relativeTo: this.route});
          } else
            console.error("Hata oluştu");
        },
        error: err => {
          if (err.error.hasError)
            console.error(err.error.errorMessage);
        }
      });
  }

  copyLimit(event) {
    this.campaignLimitsService.copyLimit(event.id);
  }
}
