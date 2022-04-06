import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CampaignDefinitionContainerComponent } from './campaign-definition-container.component';

describe('CampaignDefinitionContainerComponent', () => {
  let component: CampaignDefinitionContainerComponent;
  let fixture: ComponentFixture<CampaignDefinitionContainerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CampaignDefinitionContainerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CampaignDefinitionContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
