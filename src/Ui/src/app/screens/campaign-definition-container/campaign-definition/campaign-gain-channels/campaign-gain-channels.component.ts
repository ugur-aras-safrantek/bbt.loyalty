import {Component, OnInit, ViewChild} from '@angular/core';
import {CampaignDefinitionService} from "../../../../services/campaign-definition.service";
import {StepService} from "../../../../services/step.service";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";
import {CampaignDefinitionGainChannelsAddUpdateRequestModel} from "../../../../models/campaign-definition";
import {DropdownListModel} from "../../../../models/dropdown-list.model";
import {Subject, take, takeUntil} from "rxjs";
import {ToastrHandleService} from 'src/app/services/toastr-handle.service';
import {FormChange} from "../../../../models/form-change";
import {FormChangeAlertComponent} from "../../../../components/form-change-alert/form-change-alert.component";

@Component({
  selector: 'app-campaign-gain-channels',
  templateUrl: './campaign-gain-channels.component.html',
  styleUrls: ['./campaign-gain-channels.component.scss']
})

export class CampaignGainChannelsComponent implements OnInit, FormChange {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  @ViewChild(FormChangeAlertComponent) formChangeAlertComponent: FormChangeAlertComponent;
  formChangeSubject: Subject<boolean> = new Subject<boolean>();
  formChangeState = false;

  formGroup: FormGroup;
  submitted = false;

  channelCodeList: DropdownListModel[];

  id: any;
  newId: any;
  stepData;
  repostData = this.campaignDefinitionService.repostData;

  nextButtonVisible = true;
  nextButtonText = 'Devam';

  constructor(private fb: FormBuilder,
              private stepService: StepService,
              private toastrHandleService: ToastrHandleService,
              private campaignDefinitionService: CampaignDefinitionService,
              private router: Router,
              private route: ActivatedRoute) {
    this.route.paramMap.subscribe(paramMap => {
      this.id = paramMap.get('id');
      this.newId = paramMap.get('newId');
    });

    this.stepService.setSteps(this.campaignDefinitionService.stepData);
    this.stepService.updateStep(4);
    this.stepData = this.stepService.stepData;

    this.campaignDefinitionGainChannelsGetUpdateForm();

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
      campaignChannelCodeList: [[], Validators.required],
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

  save() {
    this.submitted = true;
    if (this.formGroup.valid) {
      this.campaignDefinitionGainChannelsUpdate();
    }
  }

  copyCampaign(event) {
    this.campaignDefinitionService.copyCampaign(event.id);
  }

  private campaignDefinitionGainChannelsGetUpdateForm() {
    let campaignId = parseInt(this.id);
    this.campaignDefinitionService.campaignDefinitionGainChannelsGetUpdateForm(campaignId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.channelCodeList = res.data.channelCodeList;
            this.formGroup.patchValue({
              campaignChannelCodeList: res.data.campaignChannelCodeList,
            });
            this.nextButtonText = this.newId ? "Devam" : "Kaydet ve ilerle";
            this.formGroup.valueChanges
              .pipe(take(1))
              .subscribe(x => {
                this.formChangeState = true;
                this.nextButtonVisible = true;
              });
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

  private campaignDefinitionGainChannelsUpdate() {
    let requestModel = new CampaignDefinitionGainChannelsAddUpdateRequestModel();
    requestModel.campaignId = this.id ?? this.newId;
    requestModel.campaignChannelCodeList = this.formGroup.getRawValue().campaignChannelCodeList;
    this.campaignDefinitionService.campaignDefinitionGainChannelsUpdate(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.toastrHandleService.success();
            this.formChangeState = false;
            this.newId
              ? this.router.navigate([`/campaign-definition/create/gains/${this.newId}`], {relativeTo: this.route})
              : this.router.navigate([`/campaign-definition/update/${this.id}/gains`], {relativeTo: this.route});
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

