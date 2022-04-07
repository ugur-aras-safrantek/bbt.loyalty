import {Component, OnInit} from '@angular/core';
import {CampaignDefinitionService} from "../../../../services/campaign-definition.service";
import {StepService} from "../../../../services/step.service";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";
import {GlobalVariable} from "../../../../global";
import {CampaignDefinitionGainsAddUpdateRequestModel} from "../../../../models/campaign-definition";
import {DropdownListModel} from "../../../../models/dropdown-list.model";
import {Subject, take, takeUntil} from "rxjs";
import {ToastrHandleService} from 'src/app/services/toastr-handle.service';

@Component({
  selector: 'app-campaign-gains',
  templateUrl: './campaign-gains.component.html',
  styleUrls: ['./campaign-gains.component.scss']
})

export class CampaignGainsComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  formGroup: FormGroup;
  submitted = false;

  channelCodeList: DropdownListModel[];
  achievementTypeList: DropdownListModel[];
  actionOptionList: DropdownListModel[];
  currencyList: DropdownListModel[];

  id: any;
  detailId: any;
  repost: boolean = false;
  disabled: boolean = false;
  stepData;
  repostData = this.campaignDefinitionService.repostData;

  preview = GlobalVariable.preview;

  nextButtonVisible = true;
  isInvisibleCampaign = false;
  previewButtonVisibleState = false;
  buttonTypeIsContinue = false;

  constructor(private fb: FormBuilder,
              private stepService: StepService,
              private toastrHandleService: ToastrHandleService,
              private campaignDefinitionService: CampaignDefinitionService,
              private router: Router,
              private route: ActivatedRoute) {
    this.route.paramMap.subscribe(paramMap => {
      this.id = paramMap.get('id');
      this.detailId = paramMap.get('detailId');
      if (paramMap.get('repost')) {
        this.repost = paramMap.get('repost') === 'true';
      }
      this.disabled = this.id && !this.repost;
    });

    this.stepService.setSteps(this.campaignDefinitionService.stepData);
    this.stepService.updateStep(4);
    this.stepData = this.stepService.stepData;

    if (this.id) {
      this.campaignDefinitionService.repostData.id = this.id;

      this.getCampaignDefinitionGain();

      this.stepService.finish();

      this.nextButtonVisible = false;
      if (this.campaignDefinitionService.isCampaignValuesChanged) {
        this.nextButtonVisible = true;
      }
    } else {
      this.getCampaignDefinitionGainsGetInsertForm();
    }

    this.formGroup = this.fb.group({
      campaignChannelCodeList: [{value: [], disabled: this.disabled}, Validators.required],
      type: 1,
      achievementTypeId: [{value: null, disabled: this.disabled}, Validators.required],
      actionOptionId: [{value: null, disabled: this.disabled}],
      titleTr: [{value: '', disabled: this.disabled}],
      titleEn: [{value: '', disabled: this.disabled}],
      descriptionTr: [{value: '', disabled: this.disabled}],
      descriptionEn: [{value: '', disabled: this.disabled}],
      currencyId: [{value: 1, disabled: this.disabled}],
      maxAmount: [{value: '', disabled: this.disabled}],
      amount: [{value: '', disabled: this.disabled}],
      rate: [{value: '', disabled: this.disabled}],
      maxUtilization: [{value: '', disabled: this.disabled}],
    });
  }

  ngOnInit(): void {
  }

  ngOnDestroy() {
    this.campaignDefinitionService.campaignFormChanged(false);

    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  get f() {
    return this.formGroup.controls;
  }

  typeChanged() {
    if (this.formGroup.get('type')?.value == 1) {
      this.f.amount.setValidators(Validators.required);

      this.formGroup.patchValue({rate: null});
      this.f.rate.clearValidators();
    } else {
      this.f.rate.setValidators(Validators.required);

      this.formGroup.patchValue({
        currencyId: 1,
        amount: null,
        maxAmount: null,
      });
      this.f.amount.clearValidators();
    }
    Object.keys(this.f).forEach(key => {
      this.formGroup.controls[key].updateValueAndValidity();
    });
  }

  private campaignViewingStateActions(state: boolean) {
    this.isInvisibleCampaign = state;
    if (state) {
      this.formGroup.patchValue({
        titleTr: null,
        titleEn: null,
        descriptionTr: null,
        descriptionEn: null,
      });
      this.f.titleTr.clearValidators();
      this.f.titleEn.clearValidators();
      this.f.descriptionTr.clearValidators();
      this.f.descriptionEn.clearValidators();
    } else {
      this.f.titleTr.setValidators(Validators.required);
      this.f.titleEn.setValidators(Validators.required);
      this.f.descriptionTr.setValidators(Validators.required);
      this.f.descriptionEn.setValidators(Validators.required);
    }
    Object.keys(this.f).forEach(key => {
      this.formGroup.controls[key].updateValueAndValidity();
    });
  }

  private populateForm(data) {
    this.formGroup.patchValue({
      campaignChannelCodeList: data.channelCodeList,
      type: data.type,
      achievementTypeId: data.achievementTypeId,
      actionOptionId: data.actionOptionId,
      titleTr: data.titleTr,
      titleEn: data.titleTr,
      descriptionTr: data.descriptionTr,
      descriptionEn: data.descriptionEn,
      currencyId: data.currencyId,
      maxAmount: data.maxAmount,
      amount: data.amount,
      rate: data.rate,
      maxUtilization: data.maxUtilization,
    })
  }

  private populateLists(data: any) {
    this.channelCodeList = data.channelCodeList;
    this.achievementTypeList = data.achievementTypes;
    this.actionOptionList = data.actionOptions;
    this.currencyList = data.currencyList;
  }

  private createRequestModel() {
    let formGroup = this.formGroup.getRawValue();
    let requestModel = new CampaignDefinitionGainsAddUpdateRequestModel();

    requestModel.campaignId = this.id ?? this.detailId;
    requestModel.campaignChannelCodeList = formGroup.campaignChannelCodeList;
    requestModel.type = formGroup.type;
    requestModel.achievementTypeId = formGroup.achievementTypeId;
    requestModel.actionOptionId = formGroup.actionOptionId;
    requestModel.titleTr = formGroup.titleTr;
    requestModel.titleEn = formGroup.titleEn;
    requestModel.descriptionTr = formGroup.descriptionTr;
    requestModel.descriptionEn = formGroup.descriptionEn;
    requestModel.maxUtilization = parseInt(formGroup.maxUtilization);
    switch (formGroup.type) {
      case 1:
      case "1":
        requestModel.currencyId = formGroup.currencyId;
        requestModel.amount = formGroup.amount;
        requestModel.maxAmount = formGroup.maxAmount;
        break;
      case 2:
      case "2":
        requestModel.rate = formGroup.rate;
        break;
    }
    return requestModel;
  }

  save() {
    this.submitted = true;
    if (this.formGroup.valid) {
      this.id ? this.campaignDefinitionGainsUpdate() : this.campaignDefinitionGainsAdd();

    }
  }

  continue() {
    this.id
      ? this.router.navigate([`/campaign-definition/create/${this.id}/true/finish`], {relativeTo: this.route})
      : this.router.navigate(['./finish'], {relativeTo: this.route});
  }

  finish(id) {
    this.preview = `${this.preview}/${id}`;
    this.previewButtonVisibleState = true;
    this.buttonTypeIsContinue = true;
  }

  copyCampaign(event) {
    this.campaignDefinitionService.copyCampaign(event.id);
  }

  private getCampaignDefinitionGainsGetInsertForm() {
    let campaignId = parseInt(this.id ?? this.detailId);
    this.campaignDefinitionService.getCampaignDefinitionGainsGetInsertForm(campaignId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.populateLists(res.data);
            this.campaignViewingStateActions(res.data.isInvisibleCampaign);
            this.typeChanged();
          } else
            this.toastrHandleService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrHandleService.error(err.error);
        }
      });
  }

  private getCampaignDefinitionGain() {
    let campaignId = parseInt(this.id ?? this.detailId);
    this.campaignDefinitionService.getCampaignDefinitionGain(campaignId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.populateLists(res.data);
            if (res.data.campaignAchievement) {
              this.populateForm(res.data.campaignAchievement);
              this.campaignViewingStateActions(res.data.isInvisibleCampaign);
              this.typeChanged();
            }
            this.formGroup.valueChanges
              .pipe(take(1))
              .subscribe(x => {
                this.nextButtonVisible = true;
                this.campaignDefinitionService.campaignFormChanged(true);
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

  private campaignDefinitionGainsAdd() {
    let requestModel = this.createRequestModel();
    this.campaignDefinitionService.campaignDefinitionGainsAdd(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.finish(res.data.campaignId);
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

  private campaignDefinitionGainsUpdate() {
    let requestModel = this.createRequestModel();
    this.campaignDefinitionService.campaignDefinitionGainsUpdate(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.finish(res.data.campaignId);
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
