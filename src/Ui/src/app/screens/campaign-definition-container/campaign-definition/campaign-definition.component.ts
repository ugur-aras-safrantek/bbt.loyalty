import {Component, OnInit, ViewChild} from '@angular/core';
import {CampaignDefinitionService} from "../../../services/campaign-definition.service";
import {StepService} from "../../../services/step.service";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";
import {IAngularMyDpOptions} from 'angular-mydatepicker';
import {AngularEditorConfig} from "@kolkov/angular-editor";
import {GlobalVariable} from "../../../global";
import {
  CampaignDefinitionAddRequestModel,
  CampaignDefinitionUpdateRequestModel
} from "../../../models/campaign-definition";
import {DropdownListModel} from "../../../models/dropdown-list.model";
import {Subject, take, takeUntil} from "rxjs";
import {UtilityService} from "../../../services/utility.service";
import {ToastrHandleService} from 'src/app/services/toastr-handle.service';
import {FormChange} from 'src/app/models/form-change';
import {FormChangeAlertComponent} from "../../../components/form-change-alert/form-change-alert.component";
import {LoginService} from 'src/app/services/login.service';
import {AuthorizationModel} from 'src/app/models/login.model';

@Component({
  selector: 'app-campaign-definition',
  templateUrl: './campaign-definition.component.html',
  styleUrls: ['./campaign-definition.component.scss']
})

export class CampaignDefinitionComponent implements OnInit, FormChange {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  currentUserAuthorizations: AuthorizationModel = new AuthorizationModel();

  @ViewChild(FormChangeAlertComponent) formChangeAlertComponent: FormChangeAlertComponent;
  formChangeSubject: Subject<boolean> = new Subject<boolean>();
  formChangeState = false;

  regex = '(https?://)?([\\da-z.-]+)\\.([a-z.]{2,6})[/\\w .-]*/?';

  contractDocumentId: any;
  contractDocument: any = null;
  contractIdDisable: boolean = false;
  formGroup: FormGroup;

  programTypeList: DropdownListModel[];
  viewOptionList: DropdownListModel[];
  orderList: DropdownListModel[];
  sectorList: DropdownListModel[];
  participationTypeList: DropdownListModel[];

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
  newId: any;
  submitted = false;

  nextButtonText = 'Devam';
  nextButtonVisible = true;
  nextButtonAuthority = false;

  editorConfig: AngularEditorConfig = {
    editable: true
  }

  constructor(private fb: FormBuilder,
              private stepService: StepService,
              private toastrHandleService: ToastrHandleService,
              private utilityService: UtilityService,
              private loginService: LoginService,
              private campaignDefinitionService: CampaignDefinitionService,
              private router: Router,
              private route: ActivatedRoute) {
    this.currentUserAuthorizations = this.loginService.getCurrentUserAuthorizations().campaignDefinitionModuleAuthorizations;

    this.route.paramMap.subscribe(paramMap => {
      this.id = paramMap.get('id');
      this.newId = paramMap.get('newId');
    });

    this.stepService.setSteps(this.campaignDefinitionService.stepData);
    this.stepService.updateStep(1);
    this.stepData = this.stepService.stepData;

    this.formGroup = this.fb.group({
      isActive: false,
      isBundle: false,
      isContract: false,
      contractId: '',
      name: ['', Validators.required],
      code: '',
      descriptionTr: ['', Validators.required],
      descriptionEn: ['', Validators.required],
      titleTr: ['', Validators.required],
      titleEn: ['', Validators.required],
      summaryTr: ['', Validators.required],
      summaryEn: ['', Validators.required],
      contentTr: '',
      contentEn: '',
      detailTr: '',
      detailEn: '',
      startDate: null,
      endDate: null,
      campaignListImageUrl: ['', Validators.pattern(this.regex)],
      campaignListImageDownloadUrl: '',
      campaignDetailImageUrl: ['', Validators.pattern(this.regex)],
      campaignDetailImageDownloadUrl: '',
      order: null,
      maxNumberOfUser: '',
      programTypeId: [null, Validators.required],
      participationTypeId: [null, Validators.required],
      sectorId: null,
      viewOptionId: [null, Validators.required],
    });

    this.formGroup.controls.startDate.setValidators([
      Validators.required,
      this.utilityService.EndDateGreaterThanStartDateValidator(this.formGroup)
    ]);
    this.formGroup.controls.endDate.setValidators([
      Validators.required,
      this.utilityService.EndDateGreaterThanStartDateValidator(this.formGroup)
    ]);

    if (this.currentUserAuthorizations.view) {
      if (this.id) {
        this.campaignDefinitionService.repostData.id = this.id;
        this.stepService.finish();
        this.CampaignDefinitionGetUpdateForm();

        this.nextButtonVisible = false;
        if (this.campaignDefinitionService.isCampaignValuesChanged) {
          this.nextButtonVisible = true;
        }
      } else {
        this.campaignDefinitionGetInsertForm();
        this.formChangeState = true;
      }
    } else {
      this.setAuthorization(false);
    }
  }

  private setAuthorization(authority: boolean) {
    this.nextButtonAuthority = authority;
    if (!authority) {
      this.editorConfig = {
        editable: false,
        showToolbar: false
      };
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
      participationTypeId: data.participationTypeId,
      sectorId: data.sectorId,
      viewOptionId: data.viewOptionId,
    })
  }

  populateLists(data) {
    this.programTypeList = data.programTypeList;
    this.viewOptionList = data.viewOptionList;
    this.orderList = data.orderList;
    this.sectorList = data.sectorList;
    this.participationTypeList = data.participationTypeList;
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

  orderStatusChanged() {
    if (this.formGroup.get('isActive')?.value && !this.formGroup.get('isBundle')?.value) {
      this.f.order.setValidators(Validators.required);
    } else {
      this.formGroup.patchValue({order: null});
      this.f.order.clearValidators();
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

  startDateChanged() {
    this.formGroup.controls.endDate.updateValueAndValidity();
  }

  endDateChanged() {
    this.formGroup.controls.startDate.updateValueAndValidity();
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
    this.orderStatusChanged();
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
            this.populateLists(res.data);
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

  private CampaignDefinitionGetUpdateForm() {
    this.campaignDefinitionService.CampaignDefinitionGetUpdateForm(this.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.populateLists(res.data);
            this.populateForm(res.data.campaign);
            this.contractDocument = res.data.contractFile?.document;
            this.contractIdDisable = true;
            this.contractDocumentId = res.data.campaign.contractId;
            this.formGroup.patchValue({contractId: res.data.contractFile?.document.documentName});
            this.changedMethodsTrigger();
            this.nextButtonText = "Kaydet ve ilerle";
            this.formGroup.valueChanges
              .pipe(take(1))
              .subscribe(x => {
                this.formChangeState = true;
                this.nextButtonVisible = true;
              });
            this.campaignDefinitionService.repostData.previewButtonVisible = res.data.campaign?.viewOptionId == 4 ? false : true;
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
      order: formGroup.order,
      maxNumberOfUser: parseInt(formGroup.maxNumberOfUser),
      sectorId: formGroup.sectorId,
      viewOptionId: formGroup.viewOptionId,
      isActive: formGroup.isActive,
      isContract: formGroup.isContract,
      isBundle: formGroup.isBundle,
      contractId: this.contractIdDisable ? this.contractDocumentId : parseInt(formGroup.contractId),
      programTypeId: formGroup.programTypeId,
      participationTypeId: formGroup.participationTypeId,
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
            this.newId = res.data.id;
            this.formChangeState = false;
            this.router.navigate([GlobalVariable.rules, this.newId], {relativeTo: this.route});
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
      order: formGroup.order,
      maxNumberOfUser: parseInt(formGroup.maxNumberOfUser),
      sectorId: formGroup.sectorId,
      viewOptionId: formGroup.viewOptionId,
      isActive: formGroup.isActive,
      isContract: formGroup.isContract,
      isBundle: formGroup.isBundle,
      contractId: this.contractIdDisable ? this.contractDocumentId : parseInt(formGroup.contractId),
      programTypeId: formGroup.programTypeId,
      participationTypeId: formGroup.participationTypeId,
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
            this.formChangeState = false;
            this.router.navigate([`/campaign-definition/update/${res.data.id}/rules`], {relativeTo: this.route});
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
              this.contractDocumentId = contractId;
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
      const fileURL = URL.createObjectURL(file);
      window.open(fileURL, '_blank');
    } else {
      this.toastrHandleService.warning("Sözleşme bulunamadı.");
    }
  }
}
