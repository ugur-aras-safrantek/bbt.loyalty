import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CampaignDefinitionListComponent } from './campaign-definition-list.component';

describe('CampaignDefinitionListComponent', () => {
  let component: CampaignDefinitionListComponent;
  let fixture: ComponentFixture<CampaignDefinitionListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CampaignDefinitionListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CampaignDefinitionListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
