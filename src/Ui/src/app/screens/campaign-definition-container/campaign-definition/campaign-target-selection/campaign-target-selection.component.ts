import {Component, OnInit} from '@angular/core';
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

@Component({
  selector: 'app-campaign-target-selection',
  templateUrl: './campaign-target-selection.component.html',
  styleUrls: ['./campaign-target-selection.component.scss']
})

export class CampaignTargetSelectionComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  stepData;
  repostData = this.campaignDefinitionService.repostData;
  id: any;
  detailId: any;
  repost: boolean = false;
  disabled: boolean = false;

  nextButtonText = 'Devam';
  nextButtonVisible = true;

  submitted = false;

  addTargetList: DropdownListModel[];
  formGroup: FormGroup;
  campaignTargetGroups: CampaignTargetGroup[] = new Array<CampaignTargetGroup>();

  selectedGroupId: any;
  selectedTargetId: any;

  constructor(private stepService: StepService,
              private toastrHandleService: ToastrHandleService,
              private modalService: NgxSmartModalService,
              private fb: FormBuilder,
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
    this.stepService.updateStep(3);
    this.stepData = this.stepService.stepData;
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
    }

    this.formGroup = this.fb.group({
      targets: [[], Validators.required]
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

  openModal() {
    this.submitted = false;
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
      campaignId: this.detailId,
      targetList: targetList
    };
    this.campaignDefinitionService.campaignTargetsAdd(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.router.navigate([GlobalVariable.gains, this.detailId], {relativeTo: this.route});
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
    let campaignId = this.id ?? this.detailId;
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
            this.router.navigate([`/campaign-definition/create/${this.id}/true/gains`], {relativeTo: this.route});
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
      id: this.createGuid(),
    };
    targetList.map(x => {
      targetGroup.targetList.push({
        id: x,
        name: this.addTargetList[this.addTargetList.findIndex(i => i.id == x)]?.name
      });
    });
    this.campaignTargetGroups.push(targetGroup);
    this.nextButtonVisible = true;
    this.campaignDefinitionService.campaignFormChanged(true);
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
    this.nextButtonVisible = true;
    this.campaignDefinitionService.campaignFormChanged(true);
  }

  createGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
      let r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }

  copyCampaign(event) {
    this.campaignDefinitionService.copyCampaign(event.id);
  }

  private getCampaignTargets() {
    let campaignId = this.id ?? this.detailId;
    this.campaignDefinitionService.getCampaignTargets(campaignId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.addTargetList = res.data.targetList;
            this.campaignTargetGroups = res.data.campaignTargetList?.targetGroupList ?? new Array<CampaignTargetGroup>();
            this.nextButtonText = "Kaydet ve ilerle";
            this.campaignDefinitionService.repostData.previewButtonVisible = !res.data.isInvisibleCampaign;
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
