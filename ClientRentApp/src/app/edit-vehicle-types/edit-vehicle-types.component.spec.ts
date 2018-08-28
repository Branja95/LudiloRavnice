import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditVehicleTypesComponent } from './edit-vehicle-types.component';

describe('EditVehicleTypesComponent', () => {
  let component: EditVehicleTypesComponent;
  let fixture: ComponentFixture<EditVehicleTypesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditVehicleTypesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditVehicleTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
