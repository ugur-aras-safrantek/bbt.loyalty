import {Component, OnInit} from '@angular/core';
import {StepService} from "../../../../services/step.service";
import {CampaignLimitsService} from "../../../../services/campaign-limits.service";
import {GlobalVariable} from "../../../../global";
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-campaign-limits-finish',
  templateUrl: './campaign-limits-finish.component.html',
  styleUrls: ['./campaign-limits-finish.component.scss']
})

export class CampaignLimitsFinishComponent implements OnInit {
  stepData;

  limitList = GlobalVariable.limitList;
  newLimit = GlobalVariable.limitCreate;

  id: any;
  finishComponentSubTitle = '';

  constructor(private stepService: StepService,
              private campaignLimitsService: CampaignLimitsService,
              private route: ActivatedRoute) {
    this.route.paramMap.subscribe(paramMap => {
      this.id = paramMap.get('id');
    });

    this.finishComponentSubTitle = this.id
      ? 'Kampanya çatı limiti güncellemesi başarılı bir şekilde onaya gönderilmiştir.'
      : 'Kampanya çatı limiti başarılı bir şekilde onaya gönderilmiştir.';

    this.stepService.setSteps(this.campaignLimitsService.stepData);
    this.stepService.updateStep(5);
    this.stepData = this.stepService.stepData;
    this.stepService.finish();
  }

  ngOnInit(): void {
  }
}
