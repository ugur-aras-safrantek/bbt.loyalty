import {Component, OnInit} from '@angular/core';
import {DropdownListModel} from "../../../../models/dropdown-list.model";
import {AngularEditorConfig} from "@kolkov/angular-editor";
import {Subject, takeUntil} from "rxjs";
import {ToastrHandleService} from "../../../../services/toastr-handle.service";
import {TargetDefinitionService} from "../../../../services/target-definition.service";

@Component({
  selector: 'app-target-preview',
  templateUrl: './target-preview.component.html',
  styleUrls: ['./target-preview.component.scss']
})

export class TargetPreviewComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  editorConfig: AngularEditorConfig = {
    editable: false,
    showToolbar: false
  }

  previewForm = {
    isActive: false,

    name: '',
    title: '',
    id: '',
    targetSourceId: null,

    targetViewTypeId: null,

    flowName: '',
    totalAmount: null,
    numberOfTransaction: '',
    flowFrequency: '',
    additionalFlowTime: '',
    triggerTimeId: null,

    condition: '',
    query: '',
    verificationTimeId: null,

    targetDetailTr: '',
    targetDetailEn: '',
    descriptionTr: '',
    descriptionEn: '',
  };

  targetSourceList: DropdownListModel[];
  targetViewTypeList: DropdownListModel[];
  triggerTimeList: DropdownListModel[];
  verificationTimeList: DropdownListModel[];

  constructor(private toastrHandleService: ToastrHandleService,
              private targetDefinitionService: TargetDefinitionService) {
  }

  ngOnInit(): void {
  }

  ngOnDestroy(): void {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  private populateLists(data: any) {
    this.targetSourceList = data.targetSourceList;
    this.targetViewTypeList = data.targetViewTypeList;
    this.triggerTimeList = data.triggerTimeList;
    this.verificationTimeList = data.verificationTimeList;
  }

  private populateForm(data) {
    this.previewForm.isActive = data.isActive;

    this.previewForm.name = data.name;
    this.previewForm.title = data.title;
    this.previewForm.id = data.id;
    this.previewForm.targetSourceId = data.targetDetail.targetSourceId;

    this.previewForm.targetViewTypeId = data.targetDetail.targetViewTypeId;

    this.previewForm.flowName = data.targetDetail.flowName;
    this.previewForm.totalAmount = data.targetDetail.totalAmount;
    this.previewForm.numberOfTransaction = data.targetDetail.numberOfTransaction;
    this.previewForm.flowFrequency = data.targetDetail.flowFrequency;
    this.previewForm.additionalFlowTime = data.targetDetail.additionalFlowTime;
    this.previewForm.triggerTimeId = data.targetDetail.triggerTimeId;

    this.previewForm.condition = data.targetDetail.condition;
    this.previewForm.query = data.targetDetail.query;
    this.previewForm.verificationTimeId = data.targetDetail.verificationTimeId;

    this.previewForm.targetDetailEn = data.targetDetail.targetDetailEn;
    this.previewForm.targetDetailTr = data.targetDetail.targetDetailTr;
    this.previewForm.descriptionEn = data.targetDetail.descriptionEn;
    this.previewForm.descriptionTr = data.targetDetail.descriptionTr;
  }

  getTargetInfo(targetId) {
    this.targetDefinitionService.getTargetInfo(targetId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.populateLists(res.data);
            if (res.data.target) {
              this.populateForm(res.data.target);
            }
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
