import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CampaignTargetSelectionComponent } from './campaign-target-selection.component';

describe('CampaignTargetSelectionComponent', () => {
  let component: CampaignTargetSelectionComponent;
  let fixture: ComponentFixture<CampaignTargetSelectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CampaignTargetSelectionComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CampaignTargetSelectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
