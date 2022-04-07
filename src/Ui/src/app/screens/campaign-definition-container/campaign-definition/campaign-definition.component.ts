import {Component, OnInit} from '@angular/core';
import {CampaignDefinitionService} from "../../../services/campaign-definition.service";
import {StepService} from "../../../services/step.service";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";
import {IAngularMyDpOptions} from 'angular-mydatepicker';
import {AngularEditorConfig} from "@kolkov/angular-editor";
import {GlobalVariable} from "../../../global";
import {saveAs} from 'file-saver';
import {
  CampaignDefinitionAddRequestModel,
  CampaignDefinitionUpdateRequestModel
} from "../../../models/campaign-definition";
import {DropdownListModel} from "../../../models/dropdown-list.model";
import {Subject, take, takeUntil} from "rxjs";
import {UtilityService} from "../../../services/utility.service";
import {ToastrHandleService} from 'src/app/services/toastr-handle.service';

@Component({
  selector: 'app-campaign-definition',
  templateUrl: './campaign-definition.component.html',
  styleUrls: ['./campaign-definition.component.scss']
})

export class CampaignDefinitionComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  regex = '(https?://)?([\\da-z.-]+)\\.([a-z.]{2,6})[/\\w .-]*/?';

  contractDocument: any = null;
  contractIdDisable: boolean = false;
  formGroup: FormGroup;
  programTypeList: DropdownListModel[];
  viewOptionList: DropdownListModel[];
  sectorList: DropdownListModel[];

  yesterday = new Date(new Date().getTime() - 24 * 60 * 60 * 1000);
  dpOptions: IAngularMyDpOptions = {
    dateRange: false,
    dateFormat: 'dd-mm-yyyy',
    disableUntil: {
      year: this.yesterday.getFullYear(),
      month: this.yesterday.getMonth() + 1,
      day: this.yesterday.getDate()
    },
  };
  locale: string = 'tr';

  stepData;
  repostData = this.campaignDefinitionService.repostData;
  id: any;
  detailId: any;
  repost: boolean = false;
  disabled: boolean = false;
  submitted = false;

  nextButtonText = 'Devam';
  nextButtonVisible = true;

  editorConfig: AngularEditorConfig = {
    editable: true
  }

  constructor(private fb: FormBuilder,
              private stepService: StepService,
              private toastrHandleService: ToastrHandleService,
              private utilityService: UtilityService,
              private campaignDefinitionService: CampaignDefinitionService,
              private router: Router,
              private route: ActivatedRoute) {
    this.campaignDefinitionGetInsertForm();

    this.route.paramMap.subscribe(paramMap => {
      this.id = paramMap.get('id');
      this.detailId = paramMap.get('detailId');
      if (paramMap.get('repost')) {
        this.repost = paramMap.get('repost') === 'true';
      }
      this.disabled = this.id && !this.repost;
    });

    this.editorConfig.editable = !this.disabled;

    this.stepService.setSteps(this.campaignDefinitionService.stepData);
    this.stepService.updateStep(1);
    this.stepData = this.stepService.stepData;

    this.formGroup = this.fb.group({
      isActive: [{value: false, disabled: this.disabled}],
      isBundle: [{value: false, disabled: this.disabled}],
      isContract: [{value: false, disabled: this.disabled}],
      contractId: [{value: '', disabled: this.disabled}],
      name: [{value: '', disabled: this.disabled}, Validators.required],
      code: [{value: '', disabled: this.disabled}],
      descriptionTr: [{value: '', disabled: this.disabled}, Validators.required],
      descriptionEn: [{value: '', disabled: this.disabled}, Validators.required],
      titleTr: [{value: '', disabled: this.disabled}, Validators.required],
      titleEn: [{value: '', disabled: this.disabled}, Validators.required],
      summaryTr: [{value: '', disabled: this.disabled}, Validators.required],
      summaryEn: [{value: '', disabled: this.disabled}, Validators.required],
      contentTr: [{value: '', disabled: this.disabled}],
      contentEn: [{value: '', disabled: this.disabled}],
      detailTr: [{value: '', disabled: this.disabled}],
      detailEn: [{value: '', disabled: this.disabled}],
      startDate: [{value: null, disabled: this.disabled}],
      endDate: [{value: null, disabled: this.disabled}],
      campaignListImageUrl: [{value: '', disabled: this.disabled}, Validators.pattern(this.regex)],
      campaignListImageDownloadUrl: '',
      campaignDetailImageUrl: [{value: '', disabled: this.disabled}, Validators.pattern(this.regex)],
      campaignDetailImageDownloadUrl: '',
      order: [{value: '', disabled: this.disabled}, Validators.required],
      maxNumberOfUser: [{value: '', disabled: this.disabled}],
      programTypeId: [{value: null, disabled: this.disabled}, Validators.required],
      sectorId: [{value: null, disabled: this.disabled}],
      viewOptionId: [{value: null, disabled: this.disabled}, Validators.required],
    });

    this.formGroup.controls.startDate.setValidators([
      Validators.required,
      this.utilityService.EndDateGreaterThanStartDateValidator(this.formGroup)
    ]);
    this.formGroup.controls.endDate.setValidators([
      Validators.required,
      this.utilityService.EndDateGreaterThanStartDateValidator(this.formGroup)
    ]);

    if (this.id) {
      this.campaignDefinitionService.repostData.id = this.id;
      this.stepService.finish();
      this.getCampaignDetail();
    }

    if (this.detailId) {
      this.getCampaignDetail();
    }
  }

  ngOnInit(): void {
  }

  ngOnDestroy() {
    this.campaignDefinitionService.campaignFormChanged(false);

    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  populateForm(data) {
    this.formGroup.patchValue({
      isActive: data.isActive,
      isBundle: data.isBundle,
      isContract: data.isContract,
      contractId: data.contractId,
      name: data.name,
      code: data.code,
      titleTr: data.titleTr,
      titleEn: data.titleEn,
      descriptionTr: data.descriptionTr,
      descriptionEn: data.descriptionEn,
      summaryTr: data.campaignDetail.summaryTr,
      summaryEn: data.campaignDetail.summaryEn,
      contentTr: data.campaignDetail.contentTr,
      contentEn: data.campaignDetail.contentEn,
      detailTr: data.campaignDetail.detailTr,
      detailEn: data.campaignDetail.detailEn,
      startDate: this.setDate(data.startDate),
      endDate: this.setDate(data.endDate),
      campaignListImageUrl: data.campaignDetail.campaignListImageUrl,
      campaignListImageDownloadUrl: data.campaignDetail.campaignListImageUrl,
      campaignDetailImageUrl: data.campaignDetail.campaignDetailImageUrl,
      campaignDetailImageDownloadUrl: data.campaignDetail.campaignDetailImageUrl,
      order: data.order,
      maxNumberOfUser: data.maxNumberOfUser,
      programTypeId: data.programTypeId,
      sectorId: data.sectorId,
      viewOptionId: data.viewOptionId,
    })
  }

  setDate(date: string) {
    let dateParts = date.split("-");
    return {
      isRange: false,
      singleDate: {
        date: {
          year: parseInt(dateParts[2]),
          month: parseInt(dateParts[1]),
          day: parseInt(dateParts[0])
        }
      }
    };
  }

  private setRequestDate(date) {
    return `${date.day}-${date.month}-${date.year}`;
  }

  get f() {
    return this.formGroup.controls;
  }

  contractIdClicked() {
    if (this.contractIdDisable) {
      this.contractIdDisable = false;
      this.formGroup.patchValue({contractId: ''});
    }
  }

  isBundleChanged() {
    if (this.formGroup.get('isBundle')?.value) {
      this.formGroup.patchValue({order: ''});
      this.f.order.clearValidators();
    } else {
      this.f.order.setValidators(Validators.required);
    }
    this.f.order.updateValueAndValidity();
  }

  isContractChanged() {
    if (this.formGroup.get('isContract')?.value) {
      this.f.contractId.setValidators(Validators.required);
    } else {
      this.contractDocument = null;
      this.contractIdDisable = false;
      this.formGroup.patchValue({contractId: ''});
      this.f.contractId.clearValidators();
    }
    this.f.contractId.updateValueAndValidity();
  }

  programTypeChanged() {
    if (this.formGroup.get('programTypeId')?.value == 1) {
      this.formGroup.patchValue({
        sectorId: null,
        viewOptionId: null,
      });
      this.f.viewOptionId.clearValidators();
    } else {
      this.f.viewOptionId.setValidators(Validators.required);
    }
    this.f.viewOptionId.updateValueAndValidity();
  }

  viewOptionChanged() {
    if (this.formGroup.get('viewOptionId')?.value == 4) {
      this.formGroup.patchValue({
        titleTr: '',
        titleEn: '',
        contentTr: '',
        contentEn: '',
        detailTr: '',
        detailEn: '',
        campaignListImageUrl: '',
        campaignDetailImageUrl: ''
      });
      this.f.titleTr.clearValidators();
      this.f.titleEn.clearValidators();
      this.f.campaignListImageUrl.clearValidators();
      this.f.campaignDetailImageUrl.clearValidators();
    } else {
      this.f.titleTr.setValidators(Validators.required);
      this.f.titleEn.setValidators(Validators.required);
      this.f.campaignListImageUrl.setValidators(Validators.pattern(this.regex));
      this.f.campaignDetailImageUrl.setValidators(Validators.pattern(this.regex));
    }
    this.f.titleTr.updateValueAndValidity();
    this.f.titleEn.updateValueAndValidity();
    this.f.campaignListImageUrl.updateValueAndValidity();
    this.f.campaignDetailImageUrl.updateValueAndValidity();
  }

  contentTrChanged(value: string) {
    if (value == '' || value == null) {
      this.formGroup.patchValue({contentEn: value});
      this.f.contentEn.clearValidators();
    } else {
      this.f.contentEn.setValidators(Validators.required);
    }
    this.f.contentEn.updateValueAndValidity();
  }

  contentEnChanged(value: string) {
    if (value != '' && value != null) {
      this.f.contentTr.setValidators(Validators.required);
    } else {
      this.f.contentTr.clearValidators();
    }
    this.f.contentTr.updateValueAndValidity();
  }

  detailTrChanged(value: string) {
    if (value == '' || value == null) {
      this.formGroup.patchValue({detailEn: value});
      this.f.detailEn.clearValidators();
    } else {
      this.f.detailEn.setValidators(Validators.required);
    }
    this.f.detailEn.updateValueAndValidity();
  }

  detailEnChanged(value: string) {
    if (value != '' && value != null) {
      this.f.detailTr.setValidators(Validators.required);
    } else {
      this.f.detailTr.clearValidators();
    }
    this.f.detailTr.updateValueAndValidity();
  }

  private changedMethodsTrigger() {
    this.isBundleChanged();
    this.isContractChanged();
    this.programTypeChanged();
    this.viewOptionChanged();
  }

  continue() {
    this.submitted = true;
    if (this.formGroup.valid) {
      this.id ? this.campaignDefinitionUpdate() : this.campaignDefinitionAdd();
    }
  }

  copyCampaign(event) {
    this.campaignDefinitionService.copyCampaign(event.id);
  }

  private campaignDefinitionGetInsertForm() {
    this.campaignDefinitionService.campaignDefinitionGetInsertForm()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.programTypeList = res.data.programTypeList;
            this.viewOptionList = res.data.viewOptionList;
            this.sectorList = res.data.sectorList;
          } else
            this.toastrHandleService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrHandleService.error(err.error);
        }
      });
  }

  private getCampaignDetail() {
    this.campaignDefinitionService.getCampaignDetail(this.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.populateForm(res.data);
            this.changedMethodsTrigger();
            this.nextButtonText = "Kaydet ve ilerle";
            this.nextButtonVisible = false;
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

  private campaignDefinitionAdd() {
    let formGroup = this.formGroup.getRawValue();
    let requestModel: CampaignDefinitionAddRequestModel = {
      name: formGroup.name,
      descriptionTr: formGroup.descriptionTr,
      descriptionEn: formGroup.descriptionEn,
      titleTr: formGroup.titleTr,
      titleEn: formGroup.titleEn,
      startDate: this.setRequestDate(formGroup.startDate?.singleDate?.date),
      endDate: this.setRequestDate(formGroup.endDate?.singleDate?.date),
      order: parseInt(formGroup.order),
      maxNumberOfUser: parseInt(formGroup.maxNumberOfUser),
      sectorId: formGroup.sectorId,
      viewOptionId: formGroup.viewOptionId,
      isActive: formGroup.isActive,
      isContract: formGroup.isContract,
      isBundle: formGroup.isBundle,
      contractId: parseInt(formGroup.contractId),
      programTypeId: formGroup.programTypeId,
      campaignDetail: {
        campaignListImageUrl: formGroup.campaignListImageUrl,
        campaignDetailImageUrl: formGroup.campaignDetailImageUrl,
        summaryTr: formGroup.summaryTr,
        summaryEn: formGroup.summaryEn,
        contentTr: formGroup.contentTr,
        contentEn: formGroup.contentEn,
        detailTr: formGroup.detailTr,
        detailEn: formGroup.detailEn,
      }
    }
    this.campaignDefinitionService.campaignDefinitionAdd(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.detailId = res.data.id;
            this.router.navigate([GlobalVariable.rules, this.detailId], {relativeTo: this.route});
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

  private campaignDefinitionUpdate() {
    let formGroup = this.formGroup.getRawValue();
    let requestModel: CampaignDefinitionUpdateRequestModel = {
      id: this.id,
      name: formGroup.name,
      descriptionTr: formGroup.descriptionTr,
      descriptionEn: formGroup.descriptionEn,
      titleTr: formGroup.titleTr,
      titleEn: formGroup.titleEn,
      startDate: this.setRequestDate(formGroup.startDate?.singleDate?.date),
      endDate: this.setRequestDate(formGroup.endDate?.singleDate?.date),
      order: parseInt(formGroup.order),
      maxNumberOfUser: parseInt(formGroup.maxNumberOfUser),
      sectorId: formGroup.sectorId,
      viewOptionId: formGroup.viewOptionId,
      isActive: formGroup.isActive,
      isContract: formGroup.isContract,
      isBundle: formGroup.isBundle,
      contractId: parseInt(formGroup.contractId),
      programTypeId: formGroup.programTypeId,
      campaignDetail: {
        campaignListImageUrl: formGroup.campaignListImageUrl,
        campaignDetailImageUrl: formGroup.campaignDetailImageUrl,
        summaryTr: formGroup.summaryTr,
        summaryEn: formGroup.summaryEn,
        contentTr: formGroup.contentTr,
        contentEn: formGroup.contentEn,
        detailTr: formGroup.detailTr,
        detailEn: formGroup.detailEn,
      }
    }
    this.campaignDefinitionService.campaignDefinitionUpdate(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.campaignDefinitionService.isCampaignValuesChanged = true;
            this.router.navigate([`/campaign-definition/create/${this.id}/true/rules`], {relativeTo: this.route});
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

  getContractFile() {
    let contractId = this.formGroup.getRawValue().contractId;
    if (contractId != "") {
      this.campaignDefinitionService.campaignDefinitionGetContractFile(contractId)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: res => {
            if (!res.hasError && res.data?.document) {
              this.contractDocument = res.data.document;
              this.contractIdDisable = true;
              this.formGroup.patchValue({contractId: res.data.document.documentName});
              this.toastrHandleService.success(`Sözleşme ID'si ${contractId} olan ${res.data.document.documentName} getirildi.`);
            } else {
              this.toastrHandleService.error(res.errorMessage);
            }
          },
          error: err => {
            if (err.error) {
              this.toastrHandleService.error(err.error);
            }
          }
        });
    } else {
      this.toastrHandleService.warning("Sözleşme ID girilmelidir.");
    }
  }

  showDocumentFile() {
    let document = this.contractDocument;
    if (document) {
      let file = this.utilityService.convertBase64ToFile(document.data, document.documentName, document.mimeType);
      saveAs(file, document.documentName);
    } else {
      this.toastrHandleService.warning("Sözleşme bulunamadı.");
    }
  }
}
