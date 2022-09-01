import {Component, OnInit} from '@angular/core';
import {FormBuilder} from "@angular/forms";
import {CampaignDefinitionService} from "../../../../services/campaign-definition.service";
import {ActivatedRoute, Router} from "@angular/router";
import {CampaignPreviewModel} from "../../../../models/campaign-definition";
import {Subject, takeUntil} from "rxjs";
import {AngularEditorConfig} from "@kolkov/angular-editor";
import {ToastrHandleService} from 'src/app/services/toastr-handle.service';
import {UtilityService} from "../../../../services/utility.service";

@Component({
  selector: 'app-campaign-preview',
  templateUrl: './campaign-preview.component.html',
  styleUrls: ['./campaign-preview.component.scss']
})

export class CampaignPreviewComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  campaign: CampaignPreviewModel = new CampaignPreviewModel();
  campaignTarget: any = {};
  campaignAchievement: any;
  contractFileUrl: any;

  targetColorList: any[] = [];


  editorConfig: AngularEditorConfig = {
    editable: false,
    showToolbar: false
  }

  constructor(private fb: FormBuilder,
              private campaignDefinitionService: CampaignDefinitionService,
              private toastrHandleService: ToastrHandleService,
              private utilityService: UtilityService,
              private router: Router,
              private route: ActivatedRoute) {
    this.route.paramMap.subscribe(paramMap => {
      this.campaign.id = paramMap.get('id');
    });

    this.getCampaignInfo();
  }

  ngOnInit(): void {
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  getCampaignInfo() {
    this.campaignDefinitionService.getCampaignInfo(this.campaign.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.campaign = res.data.campaign;
            this.campaignTarget = res.data.campaignTarget;
            this.setRandomColorList();
            this.campaignAchievement = res.data.campaignAchievementList;
            let document = res.data.contractFile?.document;
            if (document) {
              let blob = this.utilityService.convertBase64ToFile(document.data, document.documentName, document.mimeType);
              let url = window.URL.createObjectURL(blob);
              this.contractFileUrl = url;
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

  setRandomColorList() {
    const letters = '0123456789ABCDEF';
    for (let i = 0; i < 20; i++) {
      let color = '#';
      let randomValue = window.crypto.getRandomValues(new Uint8Array(6));
      for (let j = 0; j < 6; j++) {
        color += letters[randomValue[j] % 16];
      }
      this.targetColorList.push(color);
    }
  }
}
