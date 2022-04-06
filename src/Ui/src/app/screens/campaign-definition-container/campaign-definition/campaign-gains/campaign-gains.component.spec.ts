import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CampaignGainsComponent } from './campaign-gains.component';

describe('CampaignGainsComponent', () => {
  let component: CampaignGainsComponent;
  let fixture: ComponentFixture<CampaignGainsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CampaignGainsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CampaignGainsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
