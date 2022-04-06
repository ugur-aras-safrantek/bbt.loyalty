import {Injectable} from '@angular/core';
import {Step} from "../models/step.model";

@Injectable({
  providedIn: 'root'
})
export class StepService {
  isFinished: boolean = false;
  stepData: Step[] = [];

  constructor() {
  }

  setSteps(steps: Step[]) {
    this.stepData = steps;
  }

  updateStep(step: number): void {
    this.isFinished = false;
    this.stepData.map(s => {
      s.isActive = false;
      s.passed = false
    });
    const find = this.stepData.find(s => s.id === step);
    if (find) {
      find.isActive = true;
    }
    this.stepData.filter(s => s.id < step).map(s => {
      s.isActive = false;
      s.passed = true
    });
  }

  finish() {
    this.isFinished = true;
    this.stepData.map(s => {
      s.passed = true
    });
  }
}
