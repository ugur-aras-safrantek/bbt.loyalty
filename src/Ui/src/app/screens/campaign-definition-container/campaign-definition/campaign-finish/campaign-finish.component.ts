import {Component, OnInit} from '@angular/core';
import {CampaignDefinitionService} from "../../../../services/campaign-definition.service";
import {StepService} from "../../../../services/step.service";
import {GlobalVariable} from "../../../../global";
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-campaign-finish',
  templateUrl: './campaign-finish.component.html',
  styleUrls: ['./campaign-finish.component.scss']
})
export class CampaignFinishComponent implements OnInit {
  stepData;
  createLink;

  id: any;
  newId: any;

  finishComponentSubTitle = '';

  constructor(private stepService: StepService,
              private campaignDefinitionService: CampaignDefinitionService,
              private route: ActivatedRoute) {
    this.route.paramMap.subscribe(paramMap => {
      this.id = paramMap.get('id');
      this.newId = paramMap.get('newId');
    });

    this.finishComponentSubTitle = this.id
      ? 'Kampanya güncellemesi onaya gönderilmiştir.'
      : 'Kampanya girişi başarılı bir şekilde onaya gönderilmiştir.';

    this.stepService.setSteps(this.campaignDefinitionService.stepData);
    this.stepService.updateStep(5);
    this.stepData = this.stepService.stepData;

    this.stepService.finish();
    this.createLink = GlobalVariable.definition;
  }

  ngOnInit(): void {
  }
}
