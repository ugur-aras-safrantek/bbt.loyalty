import {Component, OnInit} from '@angular/core';
import {Subject, takeUntil} from "rxjs";
import {AuthorizationModel} from "../../../../models/login.model";
import {FormBuilder, FormGroup} from "@angular/forms";
import {AngularEditorConfig} from "@kolkov/angular-editor";
import {ToastrHandleService} from "../../../../services/toastr-handle.service";
import {UtilityService} from "../../../../services/utility.service";
import {LoginService} from "../../../../services/login.service";
import {saveAs} from 'file-saver';
import {ActivatedRoute, Router} from "@angular/router";
import {CampaignTargetGroup} from "../../../../models/campaign-definition";
import {ApproveService} from "../../../../services/approve.service";
import {CampaignDefinitionService} from "../../../../services/campaign-definition.service";

@Component({
  selector: 'app-campaign-definition-awaiting-approval-detail',
  templateUrl: './campaign-definition-awaiting-approval-detail.component.html',
  styleUrls: ['./campaign-definition-awaiting-approval-detail.component.scss']
})

export class CampaignDefinitionAwaitingApprovalDetailComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  currentUserAuthorizations: AuthorizationModel = new AuthorizationModel();

  id: any;

  editorConfig: AngularEditorConfig = {
    editable: false,
    showToolbar: false
  }

  campaignDefinitionFormGroup: FormGroup;

  campaignRulesFormGroup: FormGroup;
  campaignRuleId: any;
  campaignRulesDocument: boolean = false;
  showForCustomer: boolean = false;
  showBusinessLines: boolean = false;
  showBranches: boolean = false;
  showCustomerTypes: boolean = false;

  campaignTargetGroups: CampaignTargetGroup[] = new Array<CampaignTargetGroup>();

  campaignGainsFormGroup: FormGroup;

  campaignAchievementList: any[];

  history: any[];

  campaignUpdateFields: any = {};
  campaignUpdatePages: any = {};

  constructor(private fb: FormBuilder,
              private toastrHandleService: ToastrHandleService,
              private utilityService: UtilityService,
              private loginService: LoginService,
              private campaignDefinitionService: CampaignDefinitionService,
              private approveService: ApproveService,
              private router: Router,
              private route: ActivatedRoute) {
    this.currentUserAuthorizations = this.loginService.getCurrentUserAuthorizations().campaignDefinitionModuleAuthorizations;

    this.route.paramMap.subscribe(paramMap => {
      this.id = paramMap.get('id');
    });

    this.campaignDefinitionFormGroup = this.fb.group({
      isActive: false,
      isBundle: false,
      isContract: false,
      contractId: '',
      contractName: '',
      programTypeName: '',
      viewOptionName: '',
      name: '',
      code: '',
      summaryTr: '',
      summaryEn: '',
      contentTr: '',
      contentEn: '',
      campaignListImageUrl: '',
      campaignListImageDownloadUrl: '',
      campaignDetailImageUrl: '',
      campaignDetailImageDownloadUrl: '',
      startDateStr: '',
      endDateStr: '',
      maxNumberOfUser: '',
      order: '',
    });
    this.campaignDefinitionFormGroup.disable();

    this.campaignRulesFormGroup = this.fb.group({
      isEmployeeIncluded: false,
      isPrivateBanking: false,
      joinTypeName: '',
      identityNumber: '',
      ruleBusinessLinesStr: '',
      ruleBranchesStr: '',
      ruleCustomerTypesStr: '',
      startTermName: '',
    });
    this.campaignRulesFormGroup.disable();

    this.campaignGainsFormGroup = this.fb.group({
      campaignGainChannelList: '',
    });
    this.campaignGainsFormGroup.disable();

    if (this.currentUserAuthorizations.approve) {
      this.CampaignDefinitionApproveForm();
    }
  }

  ngOnInit(): void {
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  get campaignDefinitionF() {
    return this.campaignDefinitionFormGroup.controls;
  }

  private populateCampaignDefinitionForm(data, campaignDetail) {
    this.campaignDefinitionFormGroup.patchValue({
      isActive: data.isActive,
      isBundle: data.isBundle,
      isContract: data.isContract,
      contractId: data.contractId,
      contractName: data.contractId + '-Sözleşme.html',
      programTypeName: data.programTypeName,
      viewOptionName: data.viewOptionName,
      name: data.name,
      code: data.code,
      summaryTr: campaignDetail.summaryTr,
      summaryEn: campaignDetail.summaryEn,
      contentTr: campaignDetail.contentTr,
      contentEn: campaignDetail.contentEn,
      campaignListImageUrl: campaignDetail.campaignListImageUrl,
      campaignListImageDownloadUrl: campaignDetail.campaignListImageUrl,
      campaignDetailImageUrl: campaignDetail.campaignDetailImageUrl,
      campaignDetailImageDownloadUrl: campaignDetail.campaignDetailImageUrl,
      startDateStr: data.startDateStr,
      endDateStr: data.endDateStr,
      maxNumberOfUser: data.maxNumberOfUser,
      order: data.order,
    })
  }

  private populateCampaignRulesForm(data) {
    this.campaignRulesFormGroup.patchValue({
      isEmployeeIncluded: data.isEmployeeIncluded,
      isPrivateBanking: data.isPrivateBanking,
      joinTypeName: data.joinType?.name,
      identityNumber: data.identityNumber ?? data.documentName,
      ruleBusinessLinesStr: data.ruleBusinessLinesStr,
      ruleBranchesStr: data.ruleBranchesStr,
      ruleCustomerTypesStr: data.ruleCustomerTypesStr,
      startTermName: data.campaignStartTerm?.name,
    });
    this.campaignRulesDocument = !!data.documentName;
    switch (data.joinTypeId) {
      case 2:
        this.showForCustomer = true;
        break;
      case 3:
        this.showBusinessLines = true;
        break;
      case 4:
        this.showBranches = true;
        break;
      case 5:
        this.showCustomerTypes = true;
        break;
    }
  }

  private populateCampaignGainsForm(data) {
    this.campaignGainsFormGroup.patchValue({
      campaignGainChannelList: data
    })
  }

  showDocumentFile() {
    let contractId = this.campaignDefinitionFormGroup.getRawValue().contractId;
    if (contractId && contractId > 0) {
      this.campaignDefinitionService.campaignDefinitionGetContractFile(contractId)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: res => {
            if (!res.hasError && res.data?.document) {
              let document = res.data?.document;
              if (document) {
                let file = this.utilityService.convertBase64ToFile(document.data, document.documentName, document.mimeType);
                const fileURL = URL.createObjectURL(file);
                window.open(fileURL, '_blank');
              } else {
                this.toastrHandleService.warning("Sözleşme bulunamadı.");
              }
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
      this.toastrHandleService.warning("Sözleşme bulunamadı.");
    }
  }

  campaignRuleDocumentDownload() {
    this.campaignDefinitionService.campaignRuleDocumentDownload(this.campaignRuleId)
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

  private CampaignDefinitionApproveForm() {
    this.approveService.campaignDefinitionApproveForm(this.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.populateCampaignDefinitionForm(res.data.campaign, res.data.campaignDetail);
            this.populateCampaignRulesForm(res.data.campaignRule);
            this.campaignRuleId = res.data.campaignRule?.id;
            this.campaignTargetGroups = res.data.campaignTargetList?.targetGroupList ?? new Array<CampaignTargetGroup>();
            this.populateCampaignGainsForm(res.data.campaignChannelCodeList);
            this.campaignAchievementList = res.data.campaignAchievementList;
            this.history = res.data.historyList;
            this.campaignUpdateFields = res.data.campaignUpdateFields;
            this.campaignUpdatePages = res.data.campaignUpdatePages;
          } else
            this.toastrHandleService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrHandleService.error(err.error);
        }
      });
  }

  approveState(choise: boolean) {
    choise ? this.approve(this.id) : this.disapprove(this.id);
  }

  private approve(id) {
    this.approveService.campaignDefinitionApprove(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.router.navigate(['/campaign-definition/awaiting-approval/list'], {relativeTo: this.route});
          } else
            this.toastrHandleService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrHandleService.error(err.error);
        }
      });
  }

  private disapprove(id) {
    this.approveService.campaignDefinitionDisapprove(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.router.navigate(['/campaign-definition/awaiting-approval/list'], {relativeTo: this.route});
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
