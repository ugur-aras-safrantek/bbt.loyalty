import {Component, OnInit, ViewChild} from '@angular/core';
import {CampaignDefinitionService} from "../../../../services/campaign-definition.service";
import {StepService} from "../../../../services/step.service";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";
import {GlobalVariable} from "../../../../global";
import {
  CampaignDefinitionGainModel,
  CampaignDefinitionGainsAddUpdateRequestModel
} from "../../../../models/campaign-definition";
import {DropdownListModel} from "../../../../models/dropdown-list.model";
import {Subject, takeUntil} from "rxjs";
import {ToastrHandleService} from 'src/app/services/toastr-handle.service';
import {NgxSmartModalService} from "ngx-smart-modal";
import {FormChange} from "../../../../models/form-change";
import {FormChangeAlertComponent} from "../../../../components/form-change-alert/form-change-alert.component";
import {UtilityService} from "../../../../services/utility.service";

@Component({
  selector: 'app-campaign-gains',
  templateUrl: './campaign-gains.component.html',
  styleUrls: ['./campaign-gains.component.scss']
})

export class CampaignGainsComponent implements OnInit, FormChange {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  @ViewChild(FormChangeAlertComponent) formChangeAlertComponent: FormChangeAlertComponent;
  formChangeSubject: Subject<boolean> = new Subject<boolean>();
  formChangeState = false;

  formGroup: FormGroup;
  submitted = false;
  idEdited = false;

  campaignAchievementList: any[];
  // allAchievementTypeList: DropdownListModel[];
  achievementTypeList: DropdownListModel[];
  actionOptionList: DropdownListModel[];
  currencyList: DropdownListModel[];

  id: any;
  newId: any;
  stepData;
  repostData = this.campaignDefinitionService.repostData;

  previewLink = GlobalVariable.preview;

  deletedAchievement: any;

  addUpdateButtonText = 'Ekle';
  nextButtonVisible = true;
  isInvisibleCampaign = false;
  buttonTypeIsContinue = false;

  alertModalText = '';

  constructor(private fb: FormBuilder,
              private stepService: StepService,
              private modalService: NgxSmartModalService,
              private utilityService: UtilityService,
              private toastrHandleService: ToastrHandleService,
              private campaignDefinitionService: CampaignDefinitionService,
              private router: Router,
              private route: ActivatedRoute) {
    this.route.paramMap.subscribe(paramMap => {
      this.id = paramMap.get('id');
      this.newId = paramMap.get('newId');
    });

    this.stepService.setSteps(this.campaignDefinitionService.stepData);
    this.stepService.updateStep(5);
    this.stepData = this.stepService.stepData;

    this.campaignDefinitionGainsGetUpdateForm();

    if (this.id) {
      this.campaignDefinitionService.repostData.id = this.id;

      this.stepService.finish();

      this.nextButtonVisible = false;
      if (this.campaignDefinitionService.isCampaignValuesChanged) {
        this.nextButtonVisible = true;
      }
    } else {
      this.formChangeState = true;
    }

    this.formGroup = this.fb.group({
      id: 0,
      fakeId: null,
      type: 1,
      achievementTypeId: [null, Validators.required],
      actionOptionId: null,
      titleTr: '',
      titleEn: '',
      descriptionTr: '',
      descriptionEn: '',
      currencyId: 1,
      maxAmount: null,
      amount: null,
      rate: null,
      maxUtilization: '',
    });
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
      this.f.amount.setValidators(Validators.required);

      this.formGroup.patchValue({rate: null});
      this.f.rate.clearValidators();
    } else {
      this.f.rate.setValidators([Validators.required, Validators.max(100)]);

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
    this.campaignDefinitionService.repostData.previewButtonVisible = !state;
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

  save() {
    if (this.campaignAchievementList.length > 0) {
      this.campaignDefinitionGainsUpdate();
    } else {
      this.toastrHandleService.warning("Kazanım girilmelidir.");
    }
  }

  finish(id) {
    this.previewLink = `${this.previewLink}/${id}`;
    this.buttonTypeIsContinue = true;
  }

  continue() {
    this.alertModalText = this.id
      ? 'Yaptığınız değişiklikleri kaydetmeyi onaylıyor musunuz?'
      : 'Yeni kampanyayı kaydetmeyi onaylıyor musunuz?';
    this.modalService.open("campaignGainsApproveAlertModal");
  }

  alertModalOk() {
    this.formChangeState = false;
    this.newId
      ? this.router.navigate([`/campaign-definition/create/finish/${this.newId}`], {relativeTo: this.route})
      : this.router.navigate([`/campaign-definition/update/${this.id}/finish`], {relativeTo: this.route});
  }

  private clearForm() {
    this.formGroup.patchValue({
      id: 0,
      fakeId: this.utilityService.createGuid(),
      type: 1,
      achievementTypeId: null,
      actionOptionId: null,
      titleTr: '',
      titleEn: '',
      descriptionTr: '',
      descriptionEn: '',
      currencyId: 1,
      maxAmount: null,
      amount: null,
      rate: null,
      maxUtilization: '',
    })
  }

  private populateForm(data) {
    this.formGroup.patchValue({
      id: data.id,
      fakeId: data.fakeId,
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
    // this.allAchievementTypeList = data.achievementTypes;
    this.achievementTypeList = data.achievementTypes;
    this.actionOptionList = data.actionOptions;
    this.currencyList = data.currencyList;
  }

  private createAchievement() {
    let formGroup = this.formGroup.getRawValue();
    let achievement = new CampaignDefinitionGainModel();

    achievement.id = this.idEdited ? formGroup.id : 0;
    achievement.fakeId = formGroup.fakeId;
    achievement.campaignId = this.id ?? this.newId;
    achievement.type = formGroup.type;
    achievement.achievementTypeId = formGroup.achievementTypeId;
    achievement.actionOptionId = formGroup.actionOptionId;
    achievement.titleTr = formGroup.titleTr;
    achievement.titleEn = formGroup.titleEn;
    achievement.descriptionTr = formGroup.descriptionTr;
    achievement.descriptionEn = formGroup.descriptionEn;
    achievement.maxUtilization = formGroup.maxUtilization == "" || formGroup.maxUtilization == null
      ? null
      : parseInt(formGroup.maxUtilization);
    switch (formGroup.type) {
      case 1:
      case "1":
        achievement.currencyId = formGroup.currencyId;
        achievement.amount = formGroup.amount;
        achievement.maxAmount = formGroup.maxAmount;
        break;
      case 2:
      case "2":
        achievement.rate = formGroup.rate;
        break;
    }
    return achievement;
  }

  private populateTableColumn(achievement) {
    achievement.rule = achievement.type == 1 ? {id: 1, name: 'Tutar'} : {id: 2, name: 'Oran'};
    achievement.achievementType = this.achievementTypeList.find(x => x.id == achievement.achievementTypeId);
    achievement.actionOption = this.actionOptionList.find(x => x.id == achievement.actionOptionId);

    return achievement;
  }

  // private populateAchievementTypeList() {
  //   let newList: DropdownListModel[] = new Array();
  //   this.allAchievementTypeList.map(x => {
  //     if (this.campaignAchievementList.findIndex(y => y.achievementTypeId == x.id) < 0){
  //       newList.push(x);
  //     }
  //   })
  //   return newList;
  // }

  showAddModal() {
    // this.achievementTypeList = this.populateAchievementTypeList();
    this.clearForm();
    this.typeChanged();
    this.submitted = false;
    this.idEdited = false;
    this.addUpdateButtonText = "Ekle";
    this.modalService.getModal('campaignGainsAddUpdateModal').open();
  }

  showUpdateModal(achievement) {
    // this.achievementTypeList = this.populateAchievementTypeList();
    this.populateForm(achievement);
    this.typeChanged();
    this.submitted = false;
    this.idEdited = true;
    this.addUpdateButtonText = "Güncelle";
    this.modalService.getModal('campaignGainsAddUpdateModal').open();
  }

  update() {
    this.submitted = true;
    if (this.formGroup.valid) {
      let achievement = this.createAchievement();
      achievement = this.populateTableColumn(achievement);
      this.idEdited ? this.updateAchievement(achievement) : this.addAchievement(achievement);
      this.modalService.getModal('campaignGainsAddUpdateModal').close();
      this.nextButtonVisible = true;
      this.formChangeState = true;
    }
  }

  addAchievement(achievement) {
    this.campaignAchievementList.push(achievement);
  }

  updateAchievement(achievement) {
    this.campaignAchievementList.splice(
      this.campaignAchievementList.findIndex(x => x.fakeId == achievement.fakeId), 1, achievement);
  }

  openDeleteAlertModal(achievement) {
    this.deletedAchievement = achievement;
    this.modalService.getModal('campaignGainsDeleteAlertModal').open();
  }

  closeDeleteAlertModal() {
    this.modalService.getModal('campaignGainsDeleteAlertModal').close();
  }

  deleteAlertModalOk() {
    this.deleteAchievement();
    this.modalService.getModal('campaignGainsDeleteAlertModal').close();
  }

  deleteAchievement() {
    this.campaignAchievementList.splice(this.campaignAchievementList.findIndex(x => x == this.deletedAchievement), 1);
    this.nextButtonVisible = true;
    this.formChangeState = true;
  }

  copyCampaign(event) {
    this.campaignDefinitionService.copyCampaign(event.id);
  }

  private campaignDefinitionGainsGetUpdateForm() {
    let campaignId = this.id ?? this.newId;
    this.campaignDefinitionService.campaignDefinitionGainsGetUpdateForm(campaignId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.populateLists(res.data);
            this.campaignAchievementList = res.data.campaignAchievementList;
            this.campaignAchievementList.map(x => x.fakeId = this.utilityService.createGuid());
            this.campaignViewingStateActions(res.data.isInvisibleCampaign);
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
    let requestModel: CampaignDefinitionGainsAddUpdateRequestModel = {
      campaignId: this.id ?? this.newId,
      campaignAchievementList: this.campaignAchievementList
    };
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
