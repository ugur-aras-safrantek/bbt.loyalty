import {Component, OnInit, ViewChild} from '@angular/core';
import {CampaignDefinitionService} from "../../../../services/campaign-definition.service";
import {StepService} from "../../../../services/step.service";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";
import {GlobalVariable} from "../../../../global";
import {DropdownListModel} from "../../../../models/dropdown-list.model";
import {CampaignRulesAddRequestModel} from "../../../../models/campaign-definition";
import {Subject, take, takeUntil} from 'rxjs';
import * as _ from 'lodash';
import {saveAs} from 'file-saver';
import {UtilityService} from "../../../../services/utility.service";
import {ToastrHandleService} from 'src/app/services/toastr-handle.service';
import {FormChange} from "../../../../models/form-change";
import {FormChangeAlertComponent} from "../../../../components/form-change-alert/form-change-alert.component";
import {LoginService} from 'src/app/services/login.service';
import {AuthorizationModel} from 'src/app/models/login.model';

@Component({
  selector: 'app-campaign-rules',
  templateUrl: './campaign-rules.component.html',
  styleUrls: ['./campaign-rules.component.scss']
})

export class CampaignRulesComponent implements OnInit, FormChange {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  currentUserAuthorizations: AuthorizationModel = new AuthorizationModel();

  @ViewChild(FormChangeAlertComponent) formChangeAlertComponent: FormChangeAlertComponent;
  formChangeSubject: Subject<boolean> = new Subject<boolean>();
  formChangeState = false;

  stepData;
  repostData = this.campaignDefinitionService.repostData;
  allowedFileTypes = [
    'application/vnd.ms-excel',
    'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
  ];

  formGroup: FormGroup;
  documentName: null;

  businessLineList: DropdownListModel[];
  joinTypeList: DropdownListModel[];
  branchList: DropdownListModel[];
  customerTypeList: DropdownListModel[];
  campaignStartTermList: DropdownListModel[];

  disableIdentity: boolean = false;
  showForCustomer: boolean = false;
  showBusinessLines: boolean = false;
  showBranches: boolean = false;
  showCustomerTypes: boolean = false;

  submitted = false;
  id: any;
  newId: any;

  nextButtonText = 'Devam';
  nextButtonVisible = true;
  nextButtonAuthority = false;

  constructor(private fb: FormBuilder,
              private stepService: StepService,
              private toastrHandleService: ToastrHandleService,
              private campaignDefinitionService: CampaignDefinitionService,
              private utilityService: UtilityService,
              private loginService: LoginService,
              private router: Router,
              private route: ActivatedRoute) {
    this.currentUserAuthorizations = this.loginService.getCurrentUserAuthorizations().campaignDefinitionModuleAuthorizations;

    this.route.paramMap.subscribe(paramMap => {
      this.id = paramMap.get('id');
      this.newId = paramMap.get('newId');
    });

    this.stepService.setSteps(this.campaignDefinitionService.stepData);
    this.stepService.updateStep(2);
    this.stepData = this.stepService.stepData;

    this.formGroup = this.fb.group({
      joinTypeId: [null, Validators.required],
      identity: '',
      businessLines: [],
      file: '',
      branches: [],
      customerTypes: [],
      startTermId: [null, Validators.required]
    });

    if (this.currentUserAuthorizations.view) {
      if (this.id) {
        this.campaignDefinitionService.repostData.id = this.id;
        this.stepService.finish();
        this.getCampaignRules();

        this.nextButtonVisible = false;
        if (this.campaignDefinitionService.isCampaignValuesChanged) {
          this.nextButtonVisible = true;
        }
      } else {
        this.campaignRulesGetInsertForm();
        this.formChangeState = true;
      }
    } else {
      this.setAuthorization(false);
    }
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

  joinTypeChange() {
    this.showForCustomer = false;
    this.showBusinessLines = false;
    this.showBranches = false;
    this.showCustomerTypes = false;

    this.formGroup.patchValue({
      identity: '',
      file: '',
      businessLines: [],
      branches: [],
      customerTypes: []
    });

    this.f.identity.clearValidators();
    this.f.businessLines.clearValidators();
    this.f.branches.clearValidators();
    this.f.customerTypes.clearValidators();

    switch (this.formGroup.get('joinTypeId')?.value) {
      case 2:
      case "2":
        this.f.identity.setValidators([
          Validators.required,
          Validators.minLength(10),
          this.utilityService.tcknValidator(),
          this.utilityService.vknValidator()
        ]);
        this.showForCustomer = true;
        break;
      case 3:
      case "3":
        this.f.businessLines.setValidators(Validators.required);
        this.showBusinessLines = true;
        break;
      case 4:
      case "4":
        this.f.branches.setValidators(Validators.required);
        this.showBranches = true;
        break;
      case 5:
      case "5":
        this.f.customerTypes.setValidators(Validators.required);
        this.showCustomerTypes = true;
        break;
    }

    Object.keys(this.f).forEach(key => {
      this.formGroup.controls[key].updateValueAndValidity();
    });
  }

  continue() {
    this.submitted = true;
    if (this.formGroup.valid) {
      this.id ? this.campaignRulesUpdate() : this.campaignRulesAdd();
    }
  }

  private convertFileToBase64ForFileString(file: File) {
    const reader = new FileReader();
    reader.readAsDataURL(file as Blob);
    reader.onloadend = () => {
      let result = reader.result as string;
      this.formGroup.patchValue({
        file: result.split(",")[1]
      });
    };
  }

  fileSelected(e: Event) {
    const element = e.currentTarget as HTMLInputElement;
    let fileList: FileList | null = element.files;
    if (fileList!.length > 0) {
      this.formGroup.patchValue({
        identity: fileList![0].name
      });
      this.disableIdentity = true;
      this.f.identity.clearValidators();
      if (!_.includes(this.allowedFileTypes, fileList![0].type)) {
        this.f.identity.setValidators([
          this.utilityService.fileTypeValidator()
        ]);
      } else {
        this.convertFileToBase64ForFileString(fileList![0]);
      }
    } else {
      this.formGroup.patchValue({
        identity: ''
      });
      this.disableIdentity = false;
      this.f.identity.setValidators([
        Validators.required,
        Validators.minLength(10),
        this.utilityService.tcknValidator(),
        this.utilityService.vknValidator()
      ]);
    }
    this.f.identity.updateValueAndValidity();
  }

  private populateLists(data: any) {
    this.joinTypeList = data.joinTypeList;
    this.businessLineList = data.businessLineList;
    this.branchList = data.branchList;
    this.customerTypeList = data.customerTypeList;
    this.campaignStartTermList = data.campaignStartTermList;
  }

  copyCampaign(event) {
    this.campaignDefinitionService.copyCampaign(event.id);
  }

  private campaignRulesGetInsertForm() {
    this.campaignDefinitionService.campaignRulesGetInsertForm()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.populateLists(res.data);
            this.documentName = null;
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

  private getCampaignRules() {
    let campaignId = parseInt(this.id);
    this.campaignDefinitionService.getCampaignRules(campaignId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.populateLists(res.data);
            if (res.data.campaignRule) {
              this.formGroup.patchValue({
                joinTypeId: res.data.campaignRule.joinTypeId,
                startTermId: res.data.campaignRule.campaignStartTermId
              });
              this.joinTypeChange();
              this.formGroup.patchValue({
                identity: res.data.campaignRule.identityNumber,
                businessLines: res.data.campaignRule.ruleBusinessLines,
                branches: res.data.campaignRule.ruleBranches,
                customerTypes: res.data.campaignRule.ruleCustomerTypes
              });
              this.documentName = res.data.campaignRule.documentName
            }
            this.nextButtonText = "Kaydet ve ilerle";
            this.formGroup.valueChanges
              .pipe(take(1))
              .subscribe(x => {
                this.formChangeState = true;
                this.nextButtonVisible = true;
              });
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

  private campaignRulesAdd() {
    let formGroup = this.formGroup.getRawValue();
    let requestModel: CampaignRulesAddRequestModel = {
      campaignId: this.newId,
      joinTypeId: formGroup.joinTypeId,
      isSingleIdentity: !this.disableIdentity,
      identity: formGroup.identity,
      file: formGroup.file,
      businessLines: formGroup.businessLines,
      branches: formGroup.branches,
      customerTypes: formGroup.customerTypes,
      startTermId: formGroup.startTermId,
    };
    this.campaignDefinitionService.campaignRulesAdd(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.formChangeState = false;
            this.router.navigate([GlobalVariable.target, this.newId], {relativeTo: this.route});
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

  private campaignRulesUpdate() {
    let formGroup = this.formGroup.getRawValue();
    let requestModel: CampaignRulesAddRequestModel = {
      campaignId: this.id,
      joinTypeId: formGroup.joinTypeId,
      isSingleIdentity: !this.disableIdentity,
      identity: formGroup.identity,
      file: formGroup.file,
      businessLines: formGroup.businessLines,
      branches: formGroup.branches,
      customerTypes: formGroup.customerTypes,
      startTermId: formGroup.startTermId,
    };
    this.campaignDefinitionService.campaignRulesUpdate(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.campaignDefinitionService.isCampaignValuesChanged = true;
            this.formChangeState = false;
            this.router.navigate([`/campaign-definition/update/${res.data.campaignId}/target-selection`], {relativeTo: this.route});
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

  campaignRuleDocumentDownload() {
    this.campaignDefinitionService.campaignRuleDocumentDownload(this.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data?.document) {
            let document = res.data.document;
            let file = this.utilityService.convertBase64ToFile(document.data, document.documentName, document.mimeType);
            saveAs(file, res.data?.document.documentName);
            this.toastrHandleService.success();
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
  }
}
