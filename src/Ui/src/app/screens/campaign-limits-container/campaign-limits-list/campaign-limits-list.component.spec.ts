import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CampaignLimitsListComponent } from './campaign-limits-list.component';

describe('CampaignLimitsListComponent', () => {
  let component: CampaignLimitsListComponent;
  let fixture: ComponentFixture<CampaignLimitsListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CampaignLimitsListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CampaignLimitsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
