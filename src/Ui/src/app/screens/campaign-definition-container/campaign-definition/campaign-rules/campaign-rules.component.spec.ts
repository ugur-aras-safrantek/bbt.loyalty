import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CampaignRulesComponent } from './campaign-rules.component';

describe('CampaignRulesComponent', () => {
  let component: CampaignRulesComponent;
  let fixture: ComponentFixture<CampaignRulesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CampaignRulesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CampaignRulesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
