import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportsByCampaignComponent } from './reports-by-campaign.component';

describe('ReportsByCampaignComponent', () => {
  let component: ReportsByCampaignComponent;
  let fixture: ComponentFixture<ReportsByCampaignComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReportsByCampaignComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportsByCampaignComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
