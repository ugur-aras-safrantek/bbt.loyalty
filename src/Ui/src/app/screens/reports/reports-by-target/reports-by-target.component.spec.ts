import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportsByTargetComponent } from './reports-by-target.component';

describe('ReportsByTargetComponent', () => {
  let component: ReportsByTargetComponent;
  let fixture: ComponentFixture<ReportsByTargetComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReportsByTargetComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportsByTargetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
