import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CampaignDefinitionComponent } from './campaign-definition.component';

describe('CampaignDefinitionComponent', () => {
  let component: CampaignDefinitionComponent;
  let fixture: ComponentFixture<CampaignDefinitionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CampaignDefinitionComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CampaignDefinitionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
