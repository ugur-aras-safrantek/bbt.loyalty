import {Component, Input, OnInit} from '@angular/core';
import {StepService} from "../../services/step.service";

@Component({
  selector: 'app-step',
  templateUrl: './step.component.html',
  styleUrls: ['./step.component.scss']
})
export class StepComponent implements OnInit {
  @Input('data') data: any;
  @Input('id') id: any;
  isFinished;

  constructor(private stepService: StepService) {
    this.isFinished = this.stepService.isFinished;
  }

  ngOnInit(): void {
  }

}
