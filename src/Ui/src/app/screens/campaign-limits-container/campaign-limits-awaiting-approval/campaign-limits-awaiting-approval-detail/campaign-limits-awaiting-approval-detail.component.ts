import {Component, OnInit} from '@angular/core';
import {Subject, takeUntil} from "rxjs";
import {AuthorizationModel} from "../../../../models/login.model";
import {FormBuilder, FormGroup} from "@angular/forms";
import {ToastrHandleService} from "../../../../services/toastr-handle.service";
import {LoginService} from "../../../../services/login.service";
import {ApproveService} from "../../../../services/approve.service";
import {ActivatedRoute, Router} from "@angular/router";

@Component({
  selector: 'app-campaign-limits-awaiting-approval-detail',
  templateUrl: './campaign-limits-awaiting-approval-detail.component.html',
  styleUrls: ['./campaign-limits-awaiting-approval-detail.component.scss']
})

export class CampaignLimitsAwaitingApprovalDetailComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  currentUserAuthorizations: AuthorizationModel = new AuthorizationModel();

  id: any;

  campaignLimitsFormGroup: FormGroup;

  history: any[];

  constructor(private fb: FormBuilder,
              private toastrHandleService: ToastrHandleService,
              private loginService: LoginService,
              private approveService: ApproveService,
              private router: Router,
              private route: ActivatedRoute) {
    this.currentUserAuthorizations = this.loginService.getCurrentUserAuthorizations().campaignLimitsModuleAuthorizations;

    this.route.paramMap.subscribe(paramMap => {
      this.id = paramMap.get('id');
    });

    this.campaignLimitsFormGroup = this.fb.group({
      name: '',
      isActive: false,
      campaignIds: '',
      achievementFrequencyId: '',
      type: 1,
      currencyId: '',
      maxTopLimitAmount: '',
      maxTopLimitRate: '',
      maxTopLimitUtilization: '',
    });
    this.campaignLimitsFormGroup.disable();

    if (this.currentUserAuthorizations.approve) {
      this.CampaignLimitsApproveForm();
    }
  }

  ngOnInit(): void {
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  private populateCampaignLimitsForm(data) {
    this.campaignLimitsFormGroup.patchValue({
      name: data.name,
      isActive: data.isActive,
      campaignIds: data.campaignIds,
      achievementFrequencyId: data.achievementFrequencyId,
      type: data.type,
      currencyId: data.currencyId,
      maxTopLimitAmount: data.maxTopLimitAmount,
      maxTopLimitRate: data.maxTopLimitRate,
      maxTopLimitUtilization: data.maxTopLimitUtilization,
    })
  }

  private CampaignLimitsApproveForm() {
    this.approveService.campaignLimitsApproveForm(this.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.populateCampaignLimitsForm(res.data);
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
    this.approveService.campaignLimitsApproveState(choise)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.router.navigate(['/campaign-limits/awaiting-approval/list'], {relativeTo: this.route});
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
