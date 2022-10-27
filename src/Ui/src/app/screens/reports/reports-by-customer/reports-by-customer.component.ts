import {Component, OnInit} from '@angular/core';
import {Subject, takeUntil} from "rxjs";
import {saveAs} from 'file-saver';
import {DropdownListModel} from "../../../models/dropdown-list.model";
import {ReportsService} from "../../../services/reports.service";
import {ToastrHandleService} from "../../../services/toastr-handle.service";
import {UtilityService} from "../../../services/utility.service";
import {ListService} from "../../../services/list.service";
import {CustomerReportRequestModel} from "../../../models/reports";
import {NgxSmartModalService} from "ngx-smart-modal";

@Component({
  selector: 'app-reports-by-customer',
  templateUrl: './reports-by-customer.component.html',
  styleUrls: ['./reports-by-customer.component.scss']
})

export class ReportsByCustomerComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  columns = [
    {columnName: 'Kampanya Kodu', propertyName: 'campaignCode', isBoolean: false, sortDir: null},
    {columnName: 'Kampanya Adı', propertyName: 'campaignName', isBoolean: false, sortDir: null},
    {columnName: 'Aktif', propertyName: 'isActive', isBoolean: true, sortDir: null},
    {columnName: 'Birleştirilebilir', propertyName: 'isBundle', isBoolean: true, sortDir: null},
    {columnName: 'Kampanyaya Katıldığı Tarih', propertyName: 'joinDateStr', isBoolean: false, sortDir: null},
    {columnName: 'Müşteri No', propertyName: 'customerIdentifier', isBoolean: false, sortDir: null},
    {columnName: 'TCKN', propertyName: 'customerCode', isBoolean: false, sortDir: null},
    {columnName: 'Müşteri Tipi', propertyName: 'customerTypeName', isBoolean: false, sortDir: null},
    {columnName: 'Şube', propertyName: 'branchName', isBoolean: false, sortDir: null},
    {columnName: 'İş Kolu', propertyName: 'businessLineName', isBoolean: false, sortDir: null},
    {columnName: 'Hedef Dönemi', propertyName: 'term', isBoolean: false, sortDir: null},
    {columnName: 'Kazanıma Hak Kazandığı Tarih', propertyName: 'earningReachDateStr', isBoolean: false, sortDir: null},
    {columnName: 'Kazanım Tutarı', propertyName: 'achievementAmountStr', isBoolean: false, sortDir: null},
    {columnName: 'Kazanım Oranı', propertyName: 'achievementRateStr', isBoolean: false, sortDir: null},
    {columnName: 'Kazanım Tipi', propertyName: 'achievementTypeName', isBoolean: false, sortDir: null},
    {columnName: 'Kazanımdan Yararlanılan Tarih', propertyName: 'achievementDateStr', isBoolean: false, sortDir: null},
  ];

  customerTypeList: DropdownListModel[];
  campaignStartTermList: DropdownListModel[];
  achievementTypes: DropdownListModel[];
  businessLineList: DropdownListModel[];

  customerDetailCampaignTarget: any = {};
  customerDetailTargetGroupList: any[] = [];

  filterForm = {
    customerCode: '',
    customerIdentifier: '',
    customerTypeId: null,
    campaignStartTermId: null,
    branchCode: '',
    achievementTypeId: null,
    businessLineId: null,
    isActive: null,
    campaignCode : ''
  };

  constructor(private reportsService: ReportsService,
              private modalService: NgxSmartModalService,
              private toastrHandleService: ToastrHandleService,
              private utilityService: UtilityService,
              private listService: ListService) {
  }

  ngOnInit(): void {
    this.getCustomerReportFilterForm();
    this.clear();
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  clear() {
    this.filterForm = {
      customerCode: '',
      customerIdentifier: '',
      customerTypeId: null,
      campaignStartTermId: null,
      branchCode: '',
      achievementTypeId: null,
      businessLineId: null,
      isActive: null,
      campaignCode : ''
    };

    this.listService.clearList();

    this.customerReportGetByFilter();
  }

  customerReportGetByFilter() {
    this.listService.clearTable();
    let requestModel: CustomerReportRequestModel = {
      pageNumber: this.listService.paging.currentPage,
      pageSize: 10,
      sortBy: this.listService.currentSortBy,
      sortDir: this.listService.currentSortDir,
      customerCode: this.filterForm.customerCode,
      customerIdentifier: this.filterForm.customerIdentifier,
      customerTypeId: this.filterForm.customerTypeId,
      campaignStartTermId: this.filterForm.campaignStartTermId,
      branchCode: this.filterForm.branchCode,
      achievementTypeId: this.filterForm.achievementTypeId,
      businessLineId: this.filterForm.businessLineId,
      isActive: this.filterForm.isActive,
      campaignCode: this.filterForm.campaignCode
    };
    this.reportsService.customerReportGetByFilter(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data && res.data.customerCampaignList.length > 0) {
            this.listService.setList(this.columns, res.data.customerCampaignList, res.data.paging);
          } else {
            this.listService.setError("Listeleme için uygun kayıt bulunamadı");
          }
        },
        error: err => {
          if (err.error) {
            this.toastrHandleService.error(err.error);
          }
        }
      });
  }

  customerReportGetByFilterExcelFile() {
    let requestModel: CustomerReportRequestModel = {
      pageNumber: this.listService.paging.currentPage,
      pageSize: 10,
      sortBy: this.listService.currentSortBy,
      sortDir: this.listService.currentSortDir,
      customerCode: this.filterForm.customerCode,
      customerIdentifier: this.filterForm.customerIdentifier,
      customerTypeId: this.filterForm.customerTypeId,
      campaignStartTermId: this.filterForm.campaignStartTermId,
      branchCode: this.filterForm.branchCode,
      achievementTypeId: this.filterForm.achievementTypeId,
      businessLineId: this.filterForm.businessLineId,
      isActive: this.filterForm.isActive,
      campaignCode: this.filterForm.campaignCode
    };
    this.reportsService.customerReportGetByFilterExcelFile(requestModel)
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

  getCustomerReportFilterForm() {
    this.reportsService.getCustomerReportFilterForm()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.customerTypeList = res.data.customerTypeList;
            this.campaignStartTermList = res.data.campaignStartTermList;
            this.achievementTypes = res.data.achievementTypes;
            this.businessLineList = res.data.businessLineList;
          } else
            this.toastrHandleService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrHandleService.error(err.error);
        }
      });
  }

  showDetail(event) {
    this.reportsService.getCustomerDetail({customerCode: event.customerCode, campaignId: event.campaignCode})
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data?.campaignTarget) {
            this.customerDetailCampaignTarget = res.data.campaignTarget;
            this.customerDetailTargetGroupList = res.data.campaignTarget.targetGroupList;
            this.modalService.getModal('customerReportModal').open();
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
