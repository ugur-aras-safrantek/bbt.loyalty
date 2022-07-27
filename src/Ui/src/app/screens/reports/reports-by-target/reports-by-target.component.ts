import {Component, OnInit} from '@angular/core';
import {Subject, takeUntil} from "rxjs";
import {saveAs} from 'file-saver';
import {IAngularMyDpOptions} from "angular-mydatepicker";
import {DropdownListModel} from "../../../models/dropdown-list.model";
import {ToastrHandleService} from "../../../services/toastr-handle.service";
import {UtilityService} from "../../../services/utility.service";
import {ListService} from "../../../services/list.service";
import {ReportsService} from "../../../services/reports.service";
import {TargetReportRequestModel} from "../../../models/reports";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";

@Component({
  selector: 'app-reports-by-target',
  templateUrl: './reports-by-target.component.html',
  styleUrls: ['./reports-by-target.component.scss']
})

export class ReportsByTargetComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  dpOptions: IAngularMyDpOptions = {
    dateRange: false,
    dateFormat: 'dd-mm-yyyy',
  };
  locale: string = 'tr';

  columns = [
    {columnName: 'Hedef Adı', propertyName: 'targetName', isBoolean: false, sortDir: null},
    {columnName: 'Kampanya / Program', propertyName: 'campaignName', isBoolean: false, sortDir: null},
    {columnName: 'Programa Dahil Mi?', propertyName: 'isJoin', isBoolean: true, sortDir: null},
    {columnName: 'Müşteri No', propertyName: 'customerCode', isBoolean: false, sortDir: null},
    {columnName: 'Alt Kırılım', propertyName: 'identitySubTypeName', isBoolean: false, sortDir: null},
    {columnName: 'Harcama Hedefi', propertyName: 'targetAmountStr', isBoolean: false, sortDir: null},
    {columnName: 'Hedef Tuttu Mu?', propertyName: 'isTargetSuccess', isBoolean: true, sortDir: null},
    {columnName: 'Kalan Harcama', propertyName: 'remainAmountStr', isBoolean: false, sortDir: null},
    {
      columnName: 'Hedefin Gerçekleştiği Tarih',
      propertyName: 'targetSuccessDateStr',
      isBoolean: false,
      sortDir: null
    },
  ];

  campaignList: DropdownListModel[];
  targetList: DropdownListModel[];
  identitySubTypeList: DropdownListModel[];

  formGroup: FormGroup;
  targetSuccessStartDate: any;
  targetSuccessEndDate: any;

  submitted = false;

  constructor(private reportsService: ReportsService,
              private fb: FormBuilder,
              private toastrHandleService: ToastrHandleService,
              private utilityService: UtilityService,
              private listService: ListService) {
    this.formGroup = this.fb.group({
      campaignId: null,
      targetId: [null, Validators.required],
      identitySubTypeId: null,
      isJoin: null,
      customerCode: '',
    });
  }

  ngOnInit(): void {
    this.getTargetReportFilterForm();
    this.clear();
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  get f() {
    return this.formGroup.controls;
  }

  clear() {
    this.formGroup.patchValue({
      campaignId: null,
      targetId: null,
      identitySubTypeId: null,
      isJoin: null,
      customerCode: '',
    });
    this.targetSuccessStartDate = '';
    this.targetSuccessEndDate = '';

    this.submitted = false;

    this.listService.clearList();
  }

  targetReportGetByFilter() {
    this.submitted = true;
    if (this.formGroup.valid) {
      let formGroup = this.formGroup.getRawValue();
      let requestModel: TargetReportRequestModel = {
        pageNumber: this.listService.paging.currentPage,
        pageSize: 10,
        sortBy: this.listService.currentSortBy,
        sortDir: this.listService.currentSortDir,
        campaignId: formGroup.campaignId,
        targetId: formGroup.targetId,
        identitySubTypeId: formGroup.identitySubTypeId,
        isJoin: formGroup.isJoin,
        customerCode: formGroup.customerCode,
        targetSuccessStartDate: this.targetSuccessStartDate?.singleDate?.formatted,
        targetSuccessEndDate: this.targetSuccessEndDate?.singleDate?.formatted,
      };
      this.reportsService.targetReportGetByFilter(requestModel)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: res => {
            if (!res.hasError && res.data && res.data.targetReportList.length > 0) {
              this.listService.setList(this.columns, res.data.targetReportList, res.data.paging);
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
  }

  targetReportGetByFilterExcelFile() {
    let formGroup = this.formGroup.getRawValue();
    let requestModel: TargetReportRequestModel = {
      pageNumber: this.listService.paging.currentPage,
      pageSize: 10,
      sortBy: this.listService.currentSortBy,
      sortDir: this.listService.currentSortDir,
      campaignId: formGroup.campaignId,
      targetId: formGroup.targetId,
      identitySubTypeId: formGroup.identitySubTypeId,
      isJoin: formGroup.isJoin,
      customerCode: formGroup.customerCode,
      targetSuccessStartDate: this.targetSuccessStartDate?.singleDate?.formatted,
      targetSuccessEndDate: this.targetSuccessEndDate?.singleDate?.formatted,
    };
    this.reportsService.targetReportGetByFilterExcelFile(requestModel)
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

  getTargetReportFilterForm() {
    this.reportsService.getTargetReportFilterForm()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.campaignList = res.data.campaignList;
            this.targetList = res.data.targetList;
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
}
