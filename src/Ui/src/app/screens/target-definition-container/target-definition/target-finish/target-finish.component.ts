import {Component, OnInit} from '@angular/core';
import {StepService} from "../../../../services/step.service";
import {TargetDefinitionService} from "../../../../services/target-definition.service";
import {GlobalVariable} from "../../../../global";

@Component({
  selector: 'app-target-finish',
  templateUrl: './target-finish.component.html',
  styleUrls: ['./target-finish.component.scss']
})

export class TargetFinishComponent implements OnInit {
  stepData;

  targetList = GlobalVariable.targetList;
  newTarget = GlobalVariable.targetCreate;

  constructor(private stepService: StepService, private targetDefinitionService: TargetDefinitionService) {
    this.stepService.setSteps(this.targetDefinitionService.stepData);
    this.stepService.updateStep(3);
    this.stepData = this.stepService.stepData;
    this.stepService.finish();
  }

  ngOnInit(): void {
  }
}
