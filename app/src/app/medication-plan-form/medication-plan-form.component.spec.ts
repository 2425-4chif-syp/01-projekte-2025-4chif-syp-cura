import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicationPlanFormComponent } from './medication-plan-form.component';

describe('MedicationPlanFormComponent', () => {
  let component: MedicationPlanFormComponent;
  let fixture: ComponentFixture<MedicationPlanFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MedicationPlanFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MedicationPlanFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
