import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportsByEarningComponent } from './reports-by-earning.component';

describe('ReportsByEarningComponent', () => {
  let component: ReportsByEarningComponent;
  let fixture: ComponentFixture<ReportsByEarningComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReportsByEarningComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportsByEarningComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
