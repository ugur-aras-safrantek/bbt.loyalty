import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CampaignLimitsFinishComponent } from './campaign-limits-finish.component';

describe('CampaignLimitsFinishComponent', () => {
  let component: CampaignLimitsFinishComponent;
  let fixture: ComponentFixture<CampaignLimitsFinishComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CampaignLimitsFinishComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CampaignLimitsFinishComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
