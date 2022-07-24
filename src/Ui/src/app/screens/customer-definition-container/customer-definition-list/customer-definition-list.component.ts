import {Component, OnInit, ViewChild} from '@angular/core';
import {DropdownListModel} from "../../../models/dropdown-list.model";
import {Subject, takeUntil} from "rxjs";
import {saveAs} from 'file-saver';
import {UtilityService} from "../../../services/utility.service";
import {ToastrHandleService} from 'src/app/services/toastr-handle.service';
import {UserAuthorizationsModel} from "../../../models/login.model";
import * as _ from 'lodash';
import {LoginService} from 'src/app/services/login.service';
import {CustomerDefinitionService} from "../../../services/customer-definition.service";
import {
  CustomerDefinitionAddUpdateRequestModel,
  CustomerDefinitionListRequestModel
} from 'src/app/models/customer-definition';
import {NgxSmartModalService} from "ngx-smart-modal";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {PagingResponseModel} from "../../../models/paging.model";

@Component({
  selector: 'app-customer-definition-list',
  templateUrl: './customer-definition-list.component.html',
  styleUrls: ['./customer-definition-list.component.scss']
})

export class CustomerDefinitionListComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  currentUserAuthorizations: UserAuthorizationsModel = new UserAuthorizationsModel();

  campaignIdentityList: any[] = [];
  paging: PagingResponseModel = {
    currentPage: 1,
    totalPages: 1,
    totalItems: 0,
  };
  listHasError: any;
  listErrorMessage: any;

  campaignList: DropdownListModel[];
  identitySubTypeList: DropdownListModel[];

  filterForm = {
    campaignId: null,
    identitySubTypeId: null,
    identities: '',
  };

  formGroup: FormGroup;
  disableIdentity: boolean = false;
  submitted = false;
  @ViewChild('file') file;
  allowedFileTypes = [
    'application/vnd.ms-excel',
    'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
  ];

  constructor(private customerDefinitionService: CustomerDefinitionService,
              private fb: FormBuilder,
              private loginService: LoginService,
              private toastrHandleService: ToastrHandleService,
              private utilityService: UtilityService,
              private modalService: NgxSmartModalService) {
    this.currentUserAuthorizations = this.loginService.getCurrentUserAuthorizations();
  }

  ngOnInit(): void {
    this.getFilterForm();
    this.clearFilterForm();
    this.clearAddUpdateModalForm();
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  get f() {
    return this.formGroup.controls;
  }

  clearFilterForm() {
    this.filterForm = {
      campaignId: null,
      identitySubTypeId: null,
      identities: '',
    };

    this.clearList();

    this.customerDefinitionListGetByFilter();
  }

  clearAddUpdateModalForm() {
    this.formGroup = this.fb.group({
      campaignId: [null, Validators.required],
      identitySubTypeId: [null, Validators.required],
      identity: '',
      file: '',
    });

    this.f.identity.setValidators([
      Validators.required,
      Validators.minLength(10),
      this.utilityService.tcknValidator(),
      this.utilityService.vknValidator()
    ]);

    this.f.identity.updateValueAndValidity();

    this.disableIdentity = false;
    this.submitted = false;
  }

  customerDefinitionListGetByFilter() {
    let requestModel: CustomerDefinitionListRequestModel = {
      pageNumber: this.paging.currentPage,
      pageSize: 10,
      campaignId: this.filterForm.campaignId,
      identitySubTypeId: this.filterForm.identitySubTypeId,
      identities: this.filterForm.identities,
    };
    this.customerDefinitionService.customerDefinitionListGetByFilter(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data && res.data.campaignIdentityList.length > 0) {
            this.listHasError = false;
            this.listErrorMessage = '';
            this.campaignIdentityList = res.data.campaignIdentityList;
            this.paging = res.data.paging;
          } else {
            this.clearList();
            this.listHasError = true;
            this.listErrorMessage = "Listeleme için uygun kayıt bulunamadı";
          }
        },
        error: err => {
          if (err.error) {
            this.toastrHandleService.error(err.error);
          }
        }
      });
  }

  customerDefinitionListGetByFilterExcelFile() {
    let requestModel: CustomerDefinitionListRequestModel = {
      pageNumber: this.paging.currentPage,
      pageSize: 10,
      campaignId: this.filterForm.campaignId,
      identitySubTypeId: this.filterForm.identitySubTypeId,
      identities: this.filterForm.identities,
    };
    this.customerDefinitionService.customerDefinitionListGetByFilterExcelFile(requestModel)
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

  private customerDefinitionAddUpdate() {
    let formGroup = this.formGroup.getRawValue();
    let requestModel: CustomerDefinitionAddUpdateRequestModel = {
      campaignId: formGroup.campaignId,
      identitySubTypeId: formGroup.identitySubTypeId,
      isSingleIdentity: !this.disableIdentity,
      identity: formGroup.identity,
      file: formGroup.file,
    };
    this.customerDefinitionService.customerDefinitionAddUpdate(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.toastrHandleService.success();
            this.customerDefinitionListGetByFilter();
            this.modalService.getModal('customerDefinitionAddUpdateModal').close();
          } else
            this.toastrHandleService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrHandleService.error(err.error);
        }
      });
  }

  getFilterForm() {
    this.customerDefinitionService.getFilterFormByList()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.campaignList = res.data.campaignList;
            this.identitySubTypeList = res.data.identitySubTypeList;
          } else
            this.toastrHandleService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrHandleService.error(err.error);
        }
      });
  }

  openDeleteAlertModal() {
    this.modalService.getModal('customerDefinitionDeleteAlertModal').open();
  }

  closeDeleteAlertModal() {
    this.modalService.getModal('customerDefinitionDeleteAlertModal').close();
  }

  deleteAlertModalOk() {
    this.modalService.getModal('customerDefinitionDeleteAlertModal').close();
  }

  showAddUpdateModal() {
    this.clearAddUpdateModalForm();
    this.modalService.getModal('customerDefinitionAddUpdateModal').open();
  }

  addUpdate() {
    this.submitted = true;
    if (this.formGroup.valid) {
      this.customerDefinitionAddUpdate();
    }
  }

  clearList() {
    this.campaignIdentityList = [];
    this.paging = {
      currentPage: 1,
      totalPages: 1,
      totalItems: 0,
    };
    this.listHasError = false;
    this.listErrorMessage = '';
  }

  changePage(changeValue) {
    this.paging.currentPage = this.paging.currentPage + changeValue;
    this.customerDefinitionListGetByFilter();
  }

  counter(i: number) {
    return new Array(i);
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
        identity: '',
        file: '',
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

  identityClicked() {
    if (this.disableIdentity) {
      this.formGroup.patchValue({
        identity: '',
        file: '',
      });
      this.file.nativeElement.value = '';
      this.disableIdentity = false;
      this.f.identity.setValidators([
        Validators.required,
        Validators.minLength(10),
        this.utilityService.tcknValidator(),
        this.utilityService.vknValidator()
      ]);
      this.f.identity.updateValueAndValidity();
    }
  }
}
