import {Component, OnInit} from '@angular/core';
import {CampaignDefinitionService} from "../../../../services/campaign-definition.service";
import {StepService} from "../../../../services/step.service";
import {GlobalVariable} from "../../../../global";

@Component({
  selector: 'app-campaign-finish',
  templateUrl: './campaign-finish.component.html',
  styleUrls: ['./campaign-finish.component.scss']
})
export class CampaignFinishComponent implements OnInit {
  stepData;
  createLink;

  constructor(private stepService: StepService, private campaignDefinitionService: CampaignDefinitionService) {
    this.stepService.setSteps(this.campaignDefinitionService.stepData);
    this.stepService.updateStep(5);
    this.stepData = this.stepService.stepData;
    this.stepService.finish();
    this.createLink = GlobalVariable.definition;
  }

  ngOnInit(): void {
  }
}
