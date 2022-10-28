import { Component, OnInit } from '@angular/core';
import { Subject, takeUntil } from "rxjs";
import { saveAs } from 'file-saver';
import { DropdownListModel } from "../../../models/dropdown-list.model";
import { ReportsService } from "../../../services/reports.service";
import { ToastrHandleService } from "../../../services/toastr-handle.service";
import { UtilityService } from "../../../services/utility.service";
import { ListService } from "../../../services/list.service";
import { CustomerReportRequestModel } from "../../../models/reports";
import { NgxSmartModalService } from "ngx-smart-modal";

@Component({
  selector: 'app-reports-by-customer',
  templateUrl: './reports-by-customer.component.html',
  styleUrls: ['./reports-by-customer.component.scss']
})

export class ReportsByCustomerComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  columns = [
    { columnName: 'Kampanya Kodu', propertyName: 'campaignCode', isBoolean: false, sortDir: null },
    { columnName: 'Kampanya Adı', propertyName: 'campaignName', isBoolean: false, sortDir: null },
    { columnName: 'Aktif', propertyName: 'isActive', isBoolean: true, sortDir: null },
    { columnName: 'Birleştirilebilir', propertyName: 'isBundle', isBoolean: true, sortDir: null },
    { columnName: 'TCKN', propertyName: 'customerCode', isBoolean: false, sortDir: null },
    { columnName: 'Ayrıldı mı?', propertyName: 'IsExited', isBoolean: true, sortDir: null },
    { columnName: 'Kampanyaya Başlangıç Tarihi', propertyName: 'startDateStr', isBoolean: false, sortDir: null },
    { columnName: 'Kampanyaya Katıldığı Tarih', propertyName: 'joinDateStr', isBoolean: false, sortDir: null },
    { columnName: 'Kampanyadan Ayrıldığı Tarih', propertyName: 'exitDateStr', isBoolean: false, sortDir: null },
  ];

  customerTypeList: DropdownListModel[];
  campaignStartTermList: DropdownListModel[];
  achievementTypes: DropdownListModel[];
  businessLineList: DropdownListModel[];

  customerDetailCampaignTarget: any = {};
  customerDetailTargetGroupList: any[] = [];

  filterForm = {
    customerIdentifier: '',
    isActive: null,
    isExited: null,
    campaignCode: ''
  };

  constructor(private reportsService: ReportsService,
    private modalService: NgxSmartModalService,
    private toastrHandleService: ToastrHandleService,
    private utilityService: UtilityService,
    private listService: ListService) {
  }

  ngOnInit(): void {
    //this.getCustomerReportFilterForm();
    this.clear();
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  clear() {
    this.filterForm = {
      customerIdentifier: '',
      isActive: null,
      isExited: null,
      campaignCode: ''
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
      customerIdentifier: this.filterForm.customerIdentifier,
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
      customerIdentifier: this.filterForm.customerIdentifier,
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
}
