import {Component, OnInit} from '@angular/core';
import {CampaignDefinitionService} from "../../../../services/campaign-definition.service";
import {StepService} from "../../../../services/step.service";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {NgxSmartModalService} from "ngx-smart-modal";
import {ActivatedRoute, Router} from "@angular/router";
import {GlobalVariable} from "../../../../global";
import {
  CampaignDefinitionGainsAddRequestModel,
  CampaignGainChannelModel,
  CampaignGainModel,
} from "../../../../models/campaign-definition";
import {DropdownListModel} from "../../../../models/dropdown-list.model";
import {Subject, takeUntil} from "rxjs";
import {ToastrService} from "ngx-toastr";

@Component({
  selector: 'app-campaign-gains',
  templateUrl: './campaign-gains.component.html',
  styleUrls: ['./campaign-gains.component.scss']
})

export class CampaignGainsComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  campaignGainChannelList: CampaignGainChannelModel[];
  selectedChannel: CampaignGainChannelModel = new CampaignGainChannelModel();

  formGroup: FormGroup;
  submitted = false;
  idEdited = false;

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

  addUpdateButtonText = 'Ekle';
  nextButtonText = 'Kaydet';
  nextButtonVisible = true;

  constructor(private modalService: NgxSmartModalService,
              private fb: FormBuilder,
              private stepService: StepService,
              private toastrService: ToastrService,
              private campaignDefinitionService: CampaignDefinitionService,
              private router: Router, private route: ActivatedRoute) {
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

      this.stepService.finish();
      this.nextButtonText = "Kaydet ve bitir";
      this.nextButtonVisible = false;
      if (this.campaignDefinitionService.isCampaignValuesChanged){
        this.nextButtonVisible = true;
      }
    }

    this.formGroup = this.fb.group({
      id: 0,
      typeId: 1,
      achievementTypeId: [{value: null, disabled: this.disabled}, Validators.required],
      actionOptionId: [{value: null, disabled: this.disabled}, Validators.required],
      titleTr: [{value: '', disabled: this.disabled}, Validators.required],
      titleEn: [{value: '', disabled: this.disabled}, Validators.required],
      descriptionTr: [{value: '', disabled: this.disabled}, Validators.required],
      descriptionEn: [{value: '', disabled: this.disabled}, Validators.required],
      currencyId: [{value: null, disabled: this.disabled}, Validators.required],
      maxAmount: [{value: '', disabled: this.disabled}, Validators.required],
      amount: [{value: '', disabled: this.disabled}, Validators.required],
      rate: [{value: '', disabled: this.disabled}],
      maxUtilization: [{value: '', disabled: this.disabled}, Validators.required],
    });
  }

  ngOnInit(): void {
    this.getCampaignDefinitionGainChannels();
    this.getCampaignDefinitionGainsGetInsertForm();
  }

  ngOnDestroy() {
    this.campaignDefinitionService.campaignFormChanged(false);

    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  get f() {
    return this.formGroup.controls;
  }

  typeIdChanged() {
    if (this.formGroup.get('typeId')?.value == 1) {
      this.f.currencyId.setValidators(Validators.required);
      this.f.currencyId.updateValueAndValidity();
      this.f.maxAmount.setValidators(Validators.required);
      this.f.maxAmount.updateValueAndValidity();
      this.f.amount.setValidators(Validators.required);
      this.f.amount.updateValueAndValidity();

      this.f.rate.clearValidators();
      this.f.rate.updateValueAndValidity();
    } else {
      this.f.rate.setValidators(Validators.required);
      this.f.rate.updateValueAndValidity();

      this.f.currencyId.clearValidators();
      this.f.currencyId.updateValueAndValidity();
      this.f.maxAmount.clearValidators();
      this.f.maxAmount.updateValueAndValidity();
      this.f.amount.clearValidators();
      this.f.amount.updateValueAndValidity();
    }
  }

  private populateForm(data) {
    this.formGroup.patchValue({
      id: data.id,
      typeId: data.typeId,
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

  private clearForm() {
    this.formGroup.patchValue({
      id: 0,
      typeId: 1,
      achievementTypeId: null,
      actionOptionId: null,
      titleTr: '',
      titleEn: '',
      descriptionTr: '',
      descriptionEn: '',
      currencyId: null,
      maxAmount: '',
      amount: '',
      rate: '',
      maxUtilization: '',
    })
  }

  private populateTableColumn(achievement: CampaignGainModel) {
    achievement.type = achievement.typeId == 1 ? 'Tutar' : 'Oran';
    achievement.achievementType = this.achievementTypeList.find(x => x.id == achievement.achievementTypeId)?.name;
    achievement.action = this.actionOptionList.find(x => x.id == achievement.actionOptionId)?.name;

    return achievement;
  }

  showAddModal(channel: CampaignGainChannelModel) {
    this.clearForm();
    this.submitted = false;
    this.idEdited = false;
    this.selectedChannel = channel;
    this.addUpdateButtonText = "Ekle";
    this.modalService.open('addUpdateModal');
  }

  showUpdateModal(channel: CampaignGainChannelModel, achievement: CampaignGainModel) {
    this.populateForm(achievement);
    this.submitted = false;
    this.idEdited = true;
    this.selectedChannel = channel;
    this.addUpdateButtonText = "Güncelle";
    this.modalService.open('addUpdateModal');
  }

  addAchievement(achievement: CampaignGainModel) {
    this.selectedChannel.achievementList.push(achievement);
  }

  updateAchievement(achievement: CampaignGainModel) {
    this.selectedChannel.achievementList.splice(
      this.selectedChannel.achievementList.findIndex(x => x.id == achievement.id),
      1,
      achievement
    );
  }

  deleteAchievement(channel: CampaignGainChannelModel, achievement: CampaignGainModel) {
    channel.achievementList.splice(channel.achievementList.findIndex(x => x == achievement), 1);
    this.nextButtonVisible = true;
    this.campaignDefinitionService.campaignFormChanged(true);
  }

  save() {
    this.submitted = true;
    if (this.formGroup.valid) {
      let formGroup = this.formGroup.getRawValue();
      formGroup = this.populateTableColumn(formGroup);
      this.idEdited ? this.updateAchievement(formGroup) : this.addAchievement(formGroup);
      this.modalService.close('addUpdateModal');
      this.nextButtonVisible = true;
      this.campaignDefinitionService.campaignFormChanged(true);
    }
  }

  continue() {
    this.campaignDefinitionGainsAdd();
  }

  copyCampaign(event){
    this.campaignDefinitionService.copyCampaign(event.id);
  }

  private getCampaignDefinitionGainChannels() {
    let campaignId = this.id ?? this.detailId;
    this.campaignDefinitionService.getCampaignDefinitionGainChannels(campaignId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.campaignGainChannelList = res.data.channelsAndAchievements;
          } else
            this.toastrService.error(res.errorMessage);
        },
        error: err => {
          if (err.error.hasError)
            this.toastrService.error(err.error.errorMessage);
        }
      });
  }

  private getCampaignDefinitionGainsGetInsertForm() {
    this.campaignDefinitionService.getCampaignDefinitionGainsGetInsertForm()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.achievementTypeList = res.data.achievementTypes;
            this.actionOptionList = res.data.actionOptions;
            this.currencyList = res.data.currencyList;
          } else
            this.toastrService.error(res.errorMessage);
        },
        error: err => {
          if (err.error.hasError)
            this.toastrService.error(err.error.errorMessage);
        }
      });
  }

  private campaignDefinitionGainsAdd() {
    let campaignId = this.id ?? this.detailId;
    let requestModel: CampaignDefinitionGainsAddRequestModel = {
      campaignId: campaignId,
      channelsAndAchievements: this.campaignGainChannelList
    };
    this.campaignDefinitionService.campaignDefinitionGainsAdd(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.router.navigate([GlobalVariable.finish, campaignId], {relativeTo: this.route});
            this.toastrService.success("İşlem başarılı");
          } else
            this.toastrService.error(res.errorMessage);
        },
        error: err => {
          if (err.error.hasError)
            this.toastrService.error(err.error.errorMessage);
        }
      });
  }
}
