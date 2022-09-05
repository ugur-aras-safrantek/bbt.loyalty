import {Component, OnInit, ViewChild} from '@angular/core';
import {CampaignDefinitionService} from "../../../../services/campaign-definition.service";
import {StepService} from "../../../../services/step.service";
import {ActivatedRoute, Router} from "@angular/router";
import {NgxSmartModalService} from "ngx-smart-modal";
import {GlobalVariable} from "../../../../global";
import {
  CampaignTarget,
  CampaignTargetGroup,
  CampaignTargetsAddRequestModel
} from "../../../../models/campaign-definition";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {DropdownListModel} from "../../../../models/dropdown-list.model";
import {Subject, takeUntil} from "rxjs";
import {ToastrHandleService} from 'src/app/services/toastr-handle.service';
import {FormChange} from "../../../../models/form-change";
import {FormChangeAlertComponent} from "../../../../components/form-change-alert/form-change-alert.component";
import {UtilityService} from "../../../../services/utility.service";
import {LoginService} from 'src/app/services/login.service';
import {AuthorizationModel} from 'src/app/models/login.model';

@Component({
  selector: 'app-campaign-target-selection',
  templateUrl: './campaign-target-selection.component.html',
  styleUrls: ['./campaign-target-selection.component.scss']
})

export class CampaignTargetSelectionComponent implements OnInit, FormChange {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  currentUserAuthorizations: AuthorizationModel = new AuthorizationModel();

  @ViewChild(FormChangeAlertComponent) formChangeAlertComponent: FormChangeAlertComponent;
  formChangeSubject: Subject<boolean> = new Subject<boolean>();
  formChangeState = false;

  stepData;
  repostData = this.campaignDefinitionService.repostData;
  id: any;
  newId: any;

  nextButtonText = 'Devam';
  nextButtonVisible = true;
  nextButtonAuthority = false;

  submitted = false;

  dropdownSettings = this.utilityService.dropdownSettings;

  addTargetList: DropdownListModel[];
  formGroup: FormGroup;
  campaignTargetGroups: CampaignTargetGroup[] = new Array<CampaignTargetGroup>();

  selectedGroupId: any;
  selectedTargetId: any;

  constructor(private stepService: StepService,
              private toastrHandleService: ToastrHandleService,
              private modalService: NgxSmartModalService,
              private utilityService: UtilityService,
              private loginService: LoginService,
              private fb: FormBuilder,
              private campaignDefinitionService: CampaignDefinitionService,
              private router: Router,
              private route: ActivatedRoute) {
    this.currentUserAuthorizations = this.loginService.getCurrentUserAuthorizations().campaignDefinitionModuleAuthorizations;

    this.route.paramMap.subscribe(paramMap => {
      this.id = paramMap.get('id');
      this.newId = paramMap.get('newId');
    });

    this.stepService.setSteps(this.campaignDefinitionService.stepData);
    this.stepService.updateStep(3);
    this.stepData = this.stepService.stepData;

    if (this.currentUserAuthorizations.view) {
      if (this.id) {
        this.campaignDefinitionService.repostData.id = this.id;
        this.stepService.finish();
        this.getCampaignTargets();

        this.nextButtonVisible = false;
        if (this.campaignDefinitionService.isCampaignValuesChanged) {
          this.nextButtonVisible = true;
        }
      } else {
        this.campaignTargetsGetInsertForm();
        this.formChangeState = true;
      }
    } else {
      this.setAuthorization(false);
    }

    this.formGroup = this.fb.group({
      targets: [[], Validators.required]
    });
  }

  private setAuthorization(authority: boolean) {
    this.nextButtonAuthority = authority;
    if (!authority) {
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

  openModal() {
    this.submitted = false;
    this.formGroup.patchValue({targets: []});
    this.modalService.open('addTargetModal');
  }

  openDeleteAlertModal(groupId: any, targetId: any) {
    this.selectedGroupId = groupId;
    this.selectedTargetId = targetId;
    this.modalService.open('deleteAlertModal');
  }

  closeDeleteAlertModal() {
    this.modalService.close('deleteAlertModal');
  }

  deleteAlertModalOk() {
    this.deleteItem();
    this.modalService.close("deleteAlertModal");
  }

  continue() {
    let targetList = new Array<number>();
    this.campaignTargetGroups.map(group => {
      group.targetList.map(target => {
        targetList.push(target.id);
      })
      targetList.push(0);
    })
    this.id ? this.campaignTargetsUpdate(targetList) : this.campaignTargetsAdd(targetList);
  }

  save() {
    this.submitted = true;
    if (this.formGroup.valid) {
      let formGroup = this.formGroup.getRawValue();
      this.addItem(formGroup.targets);
      this.modalService.close('addTargetModal');
    }
  }

  campaignTargetsGetInsertForm() {
    this.campaignDefinitionService.campaignTargetsGetInsertForm()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.addTargetList = res.data.targetList;
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

  campaignTargetsAdd(targetList: any[]) {
    let requestModel: CampaignTargetsAddRequestModel = {
      campaignId: this.newId,
      targetList: targetList
    };
    this.campaignDefinitionService.campaignTargetsAdd(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.formChangeState = false;
            this.router.navigate([GlobalVariable.gainChannels, this.newId], {relativeTo: this.route});
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

  campaignTargetsUpdate(targetList: any[]) {
    let campaignId = this.id;
    let requestModel: CampaignTargetsAddRequestModel = {
      campaignId: campaignId,
      targetList: targetList
    };
    this.campaignDefinitionService.campaignTargetsUpdate(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.campaignDefinitionService.isCampaignValuesChanged = true;
            this.formChangeState = false;
            this.router.navigate([`/campaign-definition/update/${res.data.campaignId}/gain-channels`], {relativeTo: this.route});
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

  addItem(targetList: any[]) {
    let targetGroup: CampaignTargetGroup = {
      targetList: new Array<CampaignTarget>(),
      id: this.utilityService.createGuid(),
    };
    targetList.map(x => {
      targetGroup.targetList.push({
        id: x.id,
        name: x.name
      });
    });
    this.campaignTargetGroups.push(targetGroup);
    this.formChangeState = true;
    this.nextButtonVisible = true;
  }

  deleteItem() {
    this.campaignTargetGroups.map(x => {
      if (x.id == this.selectedGroupId) {
        for (let i = 0; i < x.targetList.length; i++) {
          if (x.targetList[i].id == this.selectedTargetId) {
            x.targetList.splice(i, 1);
            break;
          }
        }
      }
    });
    for (let i = 0; i < this.campaignTargetGroups.length; i++) {
      if (this.campaignTargetGroups[i].targetList.length == 0) {
        this.campaignTargetGroups.splice(i, 1);
      }
    }
    this.formChangeState = true;
    this.nextButtonVisible = true;
  }

  copyCampaign(event) {
    this.campaignDefinitionService.copyCampaign(event.id);
  }

  private getCampaignTargets() {
    let campaignId = this.id;
    this.campaignDefinitionService.getCampaignTargets(campaignId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.addTargetList = res.data.targetList;
            this.campaignTargetGroups = res.data.campaignTargetList?.targetGroupList ?? new Array<CampaignTargetGroup>();
            this.nextButtonText = "Kaydet ve ilerle";
            this.campaignDefinitionService.repostData.previewButtonVisible = !res.data.isInvisibleCampaign;
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
}
