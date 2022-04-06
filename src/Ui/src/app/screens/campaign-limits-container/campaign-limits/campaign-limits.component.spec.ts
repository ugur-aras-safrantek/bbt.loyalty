import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CampaignLimitsComponent } from './campaign-limits.component';

describe('CampaignLimitsComponent', () => {
  let component: CampaignLimitsComponent;
  let fixture: ComponentFixture<CampaignLimitsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CampaignLimitsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CampaignLimitsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
