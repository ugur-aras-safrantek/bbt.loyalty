import {Component, OnInit} from '@angular/core';
import {Subject, takeUntil} from "rxjs";
import {AuthorizationModel} from "../../../../models/login.model";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {ToastrHandleService} from "../../../../services/toastr-handle.service";
import {LoginService} from "../../../../services/login.service";
import {ApproveService} from "../../../../services/approve.service";
import {ActivatedRoute, Router} from "@angular/router";
import {AngularEditorConfig} from "@kolkov/angular-editor";

@Component({
  selector: 'app-target-definition-awaiting-approval-detail',
  templateUrl: './target-definition-awaiting-approval-detail.component.html',
  styleUrls: ['./target-definition-awaiting-approval-detail.component.scss']
})

export class TargetDefinitionAwaitingApprovalDetailComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  currentUserAuthorizations: AuthorizationModel = new AuthorizationModel();

  id: any;

  editorConfig: AngularEditorConfig = {
    editable: false,
    showToolbar: false
  }

  targetDefinitionFormGroup: FormGroup;
  targetSourceFormGroup: FormGroup;

  history: any[];

  constructor(private fb: FormBuilder,
              private toastrHandleService: ToastrHandleService,
              private loginService: LoginService,
              private approveService: ApproveService,
              private router: Router,
              private route: ActivatedRoute) {
    this.currentUserAuthorizations = this.loginService.getCurrentUserAuthorizations().targetDefinitionModuleAuthorizations;

    this.route.paramMap.subscribe(paramMap => {
      this.id = paramMap.get('id');
    });

    this.targetDefinitionFormGroup = this.fb.group({
      id: '',
      name: '',
      title: '',
      isActive: false
    });
    this.targetDefinitionFormGroup.disable();

    this.targetSourceFormGroup = this.fb.group({
      targetSourceId: '',
      targetViewTypeId: '',

      flowName: '',
      totalAmount: '',
      numberOfTransaction: '',
      flowFrequency: '',
      additionalFlowTime: '',
      triggerTimeId: '',

      condition: '',
      query: '',
      verificationTimeId: '',

      targetDetailTr: '',
      targetDetailEn: '',
      descriptionTr: '',
      descriptionEn: '',
    });
    this.targetSourceFormGroup.disable();

    if (this.currentUserAuthorizations.approve) {
      this.TargetDefinitionApproveForm();
    }
  }

  ngOnInit(): void {
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  private populateTargetDefinitionForm(data) {
    this.targetDefinitionFormGroup.patchValue({
      id: data.id,
      name: data.name,
      title: data.title,
      isActive: data.isActive,
    })
  }

  private populateTargetSourceForm(data) {
    this.targetSourceFormGroup.patchValue({
      targetSourceId: data.targetSourceId,

      targetViewTypeId: data.targetViewTypeId,
      flowName: data.flowName,
      totalAmount: data.totalAmount,
      numberOfTransaction: data.numberOfTransaction,
      flowFrequency: data.flowFrequency,
      additionalFlowTime: data.additionalFlowTime,
      triggerTimeId: data.triggerTimeId,

      condition: data.condition,
      query: data.query,
      verificationTimeId: data.verificationTimeId,

      targetDetailEn: data.targetDetailEn,
      targetDetailTr: data.targetDetailTr,
      descriptionEn: data.descriptionEn,
      descriptionTr: data.descriptionTr,
    })
  }

  private TargetDefinitionApproveForm() {
    this.approveService.targetDefinitionApproveForm(this.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.populateTargetDefinitionForm(res.data);
            this.populateTargetSourceForm(res.data);
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
    this.approveService.targetDefinitionApproveState(choise)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.router.navigate(['/target-definition/awaiting-approval/list'], {relativeTo: this.route});
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
