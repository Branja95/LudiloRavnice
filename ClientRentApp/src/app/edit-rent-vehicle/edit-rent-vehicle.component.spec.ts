import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditRentVehicleComponent } from './edit-rent-vehicle.component';

describe('EditRentVehicleComponent', () => {
  let component: EditRentVehicleComponent;
  let fixture: ComponentFixture<EditRentVehicleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditRentVehicleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditRentVehicleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
