import {Component, OnInit} from '@angular/core';
import {StepService} from "../../../services/step.service";
import {TargetDefinitionService} from "../../../services/target-definition.service";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";
import {Subject, take, takeUntil} from "rxjs";
import {UtilityService} from "../../../services/utility.service";
import {
  TargetDefinitionAddRequestModel,
  TargetDefinitionUpdateRequestModel
} from "../../../models/target-definition";
import {ToastrService} from "ngx-toastr";

@Component({
  selector: 'app-target-definition',
  templateUrl: './target-definition.component.html',
  styleUrls: ['./target-definition.component.scss']
})

export class TargetDefinitionComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  formGroup: FormGroup;

  stepData;
  repostData = this.targetDefinitionService.repostData;
  id: any;
  submitted = false;

  nextButtonText = 'Devam';
  nextButtonVisible = true;

  constructor(private fb: FormBuilder,
              private stepService: StepService,
              private toastrService: ToastrService,
              private utilityService: UtilityService,
              private targetDefinitionService: TargetDefinitionService,
              private router: Router,
              private route: ActivatedRoute) {
    this.route.paramMap.subscribe(paramMap => {
      this.id = paramMap.get('detailId');
    });

    this.stepService.setSteps(this.targetDefinitionService.stepData);
    this.stepService.updateStep(1);
    this.stepData = this.stepService.stepData;

    this.formGroup = this.fb.group({
      id: [null],
      name: ['', Validators.required],
      title: ['', Validators.required],
      isActive: [false]
    })

    if (this.id) {
      this.targetDefinitionService.repostData.id = this.id;
      this.stepService.finish();
      this.getTargetDetail();
    }
  }

  ngOnInit(): void {
  }

  ngOnDestroy() {
    this.targetDefinitionService.targetFormChanged(false);

    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  get f() {
    return this.formGroup.controls;
  }

  populateForm(data) {
    this.formGroup.patchValue({
      id: data.id,
      name: data.name,
      title: data.title,
      isActive: data.isActive,
    })
  }

  continue() {
    this.submitted = true;
    if (this.formGroup.valid) {
      this.id ? this.targetDefinitionUpdate() : this.targetDefinitionAdd();
    }
  }

  copyTarget(event) {
    this.targetDefinitionService.copyTarget(event.id);
  }

  private getTargetDetail() {
    this.targetDefinitionService.getTargetDetail(this.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.populateForm(res.data);
            this.nextButtonText = "Kaydet ve ilerle";
            this.nextButtonVisible = false;
            this.formGroup.valueChanges
              .pipe(take(1))
              .subscribe(x => {
                this.nextButtonVisible = true;
                this.targetDefinitionService.targetFormChanged(true);
              });
          } else
            this.toastrService.error(res.errorMessage);
        },
        error: err => {
          if (err.error.hasError)
            this.toastrService.error(err.error.errorMessage);
        }
      });
  }

  private targetDefinitionAdd() {
    let formGroup = this.formGroup.getRawValue();
    let requestModel: TargetDefinitionAddRequestModel = {
      name: formGroup.name,
      title: formGroup.title,
      isActive: formGroup.isActive,
    };
    this.targetDefinitionService.targetDefinitionAdd(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.router.navigate([`/target-definition/create/source/${res.data.id}`], {relativeTo: this.route});
            this.toastrService.success("İşlem başarılı");
          } else
            this.toastrService.error(res.errorMessage);
        },
        error: err => {
          if (err.error.hasError)
            this.toastrService.error(err.error.errorMessage);
        }
      });
  }

  private targetDefinitionUpdate() {
    let formGroup = this.formGroup.getRawValue();
    let requestModel: TargetDefinitionUpdateRequestModel = {
      id: this.id,
      name: formGroup.name,
      title: formGroup.title,
      isActive: formGroup.isActive,
    }
    this.targetDefinitionService.targetDefinitionUpdate(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.targetDefinitionService.isTargetValuesChanged = true;
            this.router.navigate([`/target-definition/update/${res.data.id}/source`], {relativeTo: this.route});
            this.toastrService.success("İşlem başarılı");
          } else
            this.toastrService.error(res.errorMessage);
        },
        error: err => {
          if (err.error.hasError)
            this.toastrService.error(err.error.errorMessage);
        }
      });
  }
}
