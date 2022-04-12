import {Component, OnInit} from '@angular/core';
import {StepService} from "../../../../services/step.service";
import {TargetDefinitionService} from "../../../../services/target-definition.service";
import {GlobalVariable} from "../../../../global";
import {ActivatedRoute} from '@angular/router';

@Component({
  selector: 'app-target-finish',
  templateUrl: './target-finish.component.html',
  styleUrls: ['./target-finish.component.scss']
})

export class TargetFinishComponent implements OnInit {
  stepData;

  targetList = GlobalVariable.targetList;
  newTarget = GlobalVariable.targetCreate;

  id: any;
  finishComponentSubTitle = '';

  constructor(private stepService: StepService,
              private targetDefinitionService: TargetDefinitionService,
              private route: ActivatedRoute) {
    this.route.paramMap.subscribe(paramMap => {
      this.id = paramMap.get('id');
    });

    this.finishComponentSubTitle = this.id
      ? 'Hedef tanımı güncellemesi başarılı bir şekilde onaya gönderilmiştir.'
      : 'Hedef tanımı başarılı bir şekilde onaya gönderilmiştir.';

    this.stepService.setSteps(this.targetDefinitionService.stepData);
    this.stepService.updateStep(3);
    this.stepData = this.stepService.stepData;
    this.stepService.finish();
  }

  ngOnInit(): void {
  }
}
