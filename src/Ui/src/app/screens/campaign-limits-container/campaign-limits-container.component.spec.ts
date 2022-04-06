import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CampaignLimitsContainerComponent } from './campaign-limits-container.component';

describe('CampaignLimitsContainerComponent', () => {
  let component: CampaignLimitsContainerComponent;
  let fixture: ComponentFixture<CampaignLimitsContainerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CampaignLimitsContainerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CampaignLimitsContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
