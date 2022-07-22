import {Component, OnInit,} from '@angular/core';
import {DropdownListModel} from "../../../models/dropdown-list.model";
import {Subject, takeUntil} from "rxjs";
import {saveAs} from 'file-saver';
import {UtilityService} from "../../../services/utility.service";
import {ToastrHandleService} from 'src/app/services/toastr-handle.service';
import {UserAuthorizationsModel} from "../../../models/login.model";
import {LoginService} from 'src/app/services/login.service';
import {CustomerDefinitionService} from "../../../services/customer-definition.service";
import { CustomerDefinitionListRequestModel } from 'src/app/models/customer-definition';
import {NgxSmartModalService} from "ngx-smart-modal";
import {FormGroup} from "@angular/forms";
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

  campaignList: DropdownListModel[];
  identitySubTypeList: DropdownListModel[];

  formGroup: FormGroup;

  listHasError: any;
  listErrorMessage: any;

  filterForm = {
    campaignId: null,
    identitySubTypeId: null,
    identities: '',
  };

  constructor(private customerDefinitionService: CustomerDefinitionService,
              private loginService: LoginService,
              private toastrHandleService: ToastrHandleService,
              private utilityService: UtilityService,
              private modalService: NgxSmartModalService) {
    this.currentUserAuthorizations = this.loginService.getCurrentUserAuthorizations();
  }

  ngOnInit(): void {
    this.getFilterForm();
    this.clear();
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  clear() {
    this.filterForm = {
      campaignId: null,
      identitySubTypeId: null,
      identities: '',
    };

    this.clearList();

    this.customerDefinitionListGetByFilter();
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
    this.modalService.getModal('customerDefinitionAddUpdateModal').open();
  }

  addUpdate(){

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
}
