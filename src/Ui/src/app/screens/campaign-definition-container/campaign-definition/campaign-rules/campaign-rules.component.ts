import {Component, OnInit} from '@angular/core';
import {CampaignDefinitionService} from "../../../../services/campaign-definition.service";
import {StepService} from "../../../../services/step.service";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";
import {GlobalVariable} from "../../../../global";
import {DropdownListModel} from "../../../../models/dropdown-list.model";
import {CampaignRulesAddRequestModel} from "../../../../models/campaign-definition";
import {Subject, take, takeUntil} from 'rxjs';
import * as _ from 'lodash';
import {UtilityService} from "../../../../services/utility.service";
import {ToastrService} from "ngx-toastr";

// import {IDropdownSettings} from "ng-multiselect-dropdown";

@Component({
  selector: 'app-campaign-rules',
  templateUrl: './campaign-rules.component.html',
  styleUrls: ['./campaign-rules.component.scss']
})
export class CampaignRulesComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  // dropdownSettings: IDropdownSettings = {
  //   singleSelection: false,
  //   idField: 'id',
  //   textField: 'name',
  //   selectAllText: 'Hepsini seç',
  //   unSelectAllText: 'Hepsini kaldır ',
  //   itemsShowLimit: 5,
  //   allowSearchFilter: true,
  //   searchPlaceholderText: 'Ara...',
  // };

  stepData;
  repostData = this.campaignDefinitionService.repostData;
  allowedFileTypes = [
    'application/vnd.ms-excel',
    'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
  ];

  formGroup: FormGroup;

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
  detailId: any;
  repost: boolean = false;
  disabled: boolean = false;

  nextButtonText = 'Devam';
  nextButtonVisible = true;

  constructor(private fb: FormBuilder,
              private stepService: StepService,
              private toastrService: ToastrService,
              private campaignDefinitionService: CampaignDefinitionService,
              private utilityService: UtilityService,
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
    this.stepService.updateStep(2);
    this.stepData = this.stepService.stepData;

    this.formGroup = this.fb.group({
      joinTypeId: [{value: null, disabled: this.disabled}, Validators.required],
      identity: [{value: '', disabled: this.disabled}],
      businessLines: [{value: [], disabled: this.disabled}],
      file: [{value: ''}],
      branches: [{value: [], disabled: this.disabled}],
      customerTypes: [{value: [], disabled: this.disabled}],
      startTermId: [{value: null, disabled: this.disabled}, Validators.required]
    });

    if (this.id) {
      this.campaignDefinitionService.repostData.id = this.id;
      this.stepService.finish();
      this.getCampaignRules();
    } else {
      this.campaignRulesGetInsertForm();
    }
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
      case 1:
      case "1":
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
          } else
            this.toastrService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrService.error(err.error.title);
        }
      });
  }

  private getCampaignRules() {
    let campaignId = parseInt(this.id ?? this.detailId);
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
            }
            this.nextButtonText = "Kaydet ve ilerle";
            this.nextButtonVisible = false;
            this.formGroup.valueChanges
              .pipe(take(1))
              .subscribe(x => {
                this.nextButtonVisible = true;
                this.campaignDefinitionService.campaignFormChanged(true);
              });
          } else
            this.toastrService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrService.error(err.error.title);
        }
      });
  }

  private campaignRulesAdd() {
    let formGroup = this.formGroup.getRawValue();
    let requestModel: CampaignRulesAddRequestModel = {
      campaignId: this.detailId,
      joinTypeId: formGroup.joinTypeId,
      isSingleIdentity: !this.disableIdentity,
      identity: formGroup.identity,
      file: formGroup.file,
      businessLines: formGroup.businessLines,
      branches: formGroup.branches,
      customerTypes: formGroup.customerTypes,
      // businessLines: formGroup.businessLines.map(x => {
      //   return parseInt(x.id);
      // }),
      // branches: formGroup.branches.map(x => {
      //   return parseInt(x.id);
      // }),
      // customerTypes: formGroup.customerTypes.map(x => {
      //   return parseInt(x.id);
      // }),
      startTermId: formGroup.startTermId,
    };
    this.campaignDefinitionService.campaignRulesAdd(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.router.navigate([GlobalVariable.target, this.detailId], {relativeTo: this.route});
            this.toastrService.success("İşlem başarılı");
          } else
            this.toastrService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrService.error(err.error.title);
        }
      });
  }

  private campaignRulesUpdate() {
    let formGroup = this.formGroup.getRawValue();
    let requestModel: CampaignRulesAddRequestModel = {
      campaignId: this.id ?? this.detailId,
      joinTypeId: formGroup.joinTypeId,
      isSingleIdentity: !this.disableIdentity,
      identity: formGroup.identity,
      file: formGroup.file,
      businessLines: formGroup.businessLines,
      branches: formGroup.branches,
      customerTypes: formGroup.customerTypes,
      // businessLines: formGroup.businessLines.map(x => {
      //   return parseInt(x.id);
      // }),
      // branches: formGroup.branches.map(x => {
      //   return parseInt(x.id);
      // }),
      // customerTypes: formGroup.customerTypes.map(x => {
      //   return parseInt(x.id);
      // }),
      startTermId: formGroup.startTermId,
    };
    this.campaignDefinitionService.campaignRulesUpdate(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.campaignDefinitionService.isCampaignValuesChanged = true;
            this.router.navigate([`/campaign-definition/create/${this.id}/true/target-selection`], {relativeTo: this.route});
            this.toastrService.success("İşlem başarılı");
          } else
            this.toastrService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrService.error(err.error.title);
        }
      });
  }
}
