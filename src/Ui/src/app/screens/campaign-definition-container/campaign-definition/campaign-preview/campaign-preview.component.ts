import {Component, OnInit} from '@angular/core';
import {FormBuilder} from "@angular/forms";
import {CampaignDefinitionService} from "../../../../services/campaign-definition.service";
import {ActivatedRoute, Router} from "@angular/router";
import {CampaignPreviewModel} from "../../../../models/campaign-definition";
import {Subject, takeUntil} from "rxjs";
import {AngularEditorConfig} from "@kolkov/angular-editor";
import {ToastrService} from "ngx-toastr";

@Component({
  selector: 'app-campaign-preview',
  templateUrl: './campaign-preview.component.html',
  styleUrls: ['./campaign-preview.component.scss']
})

export class CampaignPreviewComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  campaign: CampaignPreviewModel = new CampaignPreviewModel();

  editorConfig: AngularEditorConfig = {
    editable: false,
    showToolbar: false
  }

  constructor(private fb: FormBuilder,
              private campaignDefinitionService: CampaignDefinitionService,
              private toastrService: ToastrService,
              private router: Router,
              private route: ActivatedRoute) {
    this.route.paramMap.subscribe(paramMap => {
      this.campaign.id = paramMap.get('detailId');
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
