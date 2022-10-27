import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportsByCustomerComponent } from './reports-by-customer.component';

describe('ReportsByCustomerComponent', () => {
  let component: ReportsByCustomerComponent;
  let fixture: ComponentFixture<ReportsByCustomerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReportsByCustomerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportsByCustomerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
