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
  campaignDefinitionContractDocument: any = null;

  campaignRulesFormGroup: FormGroup;
  showForCustomer: boolean = false;
  showBusinessLines: boolean = false;
  showBranches: boolean = false;
  showCustomerTypes: boolean = false;

  campaignTargetGroups: CampaignTargetGroup[] = new Array<CampaignTargetGroup>();

  campaignGainsFormGroup: FormGroup;

  campaignAchievementList: any[];

  history: any[];

  constructor(private fb: FormBuilder,
              private toastrHandleService: ToastrHandleService,
              private utilityService: UtilityService,
              private loginService: LoginService,
              private approveService: ApproveService,
              private router: Router,
              private route: ActivatedRoute) {
    this.currentUserAuthorizations = this.loginService.getCurrentUserAuthorizations().campaignDefinitionModuleAuthorizations;

    this.route.paramMap.subscribe(paramMap => {
      this.id = paramMap.get('id');
    });

    this.campaignDefinitionFormGroup = this.fb.group({
      isActive: false,
      isContract: false,
      contractId: '',
      programTypeId: '',
      viewOptionId: '',
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
      startDate: '',
      endDate: '',
      maxNumberOfUser: '',
      order: '',
    });
    this.campaignDefinitionFormGroup.disable();

    this.campaignRulesFormGroup = this.fb.group({
      joinTypeId: '',
      documentName: '',
      businessLines: '',
      branches: '',
      customerTypes: '',
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

  private populateCampaignDefinitionForm(data) {
    this.campaignDefinitionFormGroup.patchValue({
      isActive: data.isActive,
      isContract: data.isContract,
      contractId: data.contractId,
      programTypeId: data.programTypeId,
      viewOptionId: data.viewOptionId,
      name: data.name,
      code: data.code,
      summaryTr: data.campaignDetail.summaryTr,
      summaryEn: data.campaignDetail.summaryEn,
      contentTr: data.campaignDetail.contentTr,
      contentEn: data.campaignDetail.contentEn,
      campaignListImageUrl: data.campaignDetail.campaignListImageUrl,
      campaignListImageDownloadUrl: data.campaignDetail.campaignListImageUrl,
      campaignDetailImageUrl: data.campaignDetail.campaignDetailImageUrl,
      campaignDetailImageDownloadUrl: data.campaignDetail.campaignDetailImageUrl,
      startDate: data.startDate,
      endDate: data.endDate,
      maxNumberOfUser: data.maxNumberOfUser,
      order: data.order,
    })
  }

  private populateCampaignRulesForm(data) {
    this.campaignRulesFormGroup.patchValue({
      joinTypeId: data.joinTypeId,
      documentName: data.documentName,
      businessLines: data.businessLines,
      branches: data.branches,
      customerTypes: data.customerTypes,
    })
  }

  private populateCampaignGainsForm(data) {
    this.campaignGainsFormGroup.patchValue({
      campaignGainChannelList: data.campaignGainChannelList
    })
  }

  showDocumentFile() {
    let document = this.campaignDefinitionContractDocument;
    if (document) {
      let file = this.utilityService.convertBase64ToFile(document.data, document.documentName, document.mimeType);
      const fileURL = URL.createObjectURL(file);
      window.open(fileURL, '_blank');
    } else {
      this.toastrHandleService.warning("Sözleşme bulunamadı.");
    }
  }

  campaignRuleDocumentDownload() {
    this.approveService.campaignRuleDocumentDownload(this.id)
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
            this.populateCampaignDefinitionForm(res.data.campaign);
            this.campaignDefinitionContractDocument = res.data.contractFile?.document;
            this.populateCampaignRulesForm(res.data.campaignRules);
            this.campaignTargetGroups = res.data.campaignTargetList?.targetGroupList ?? new Array<CampaignTargetGroup>();
            this.populateCampaignGainsForm(res.data.campaignGains);
            this.campaignAchievementList = res.data.campaignAchievementList;
            this.history = res.data.history;
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

  approve(id){
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

  disapprove(id){
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
