import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddRentVehicleComponent } from './add-rent-vehicle.component';

describe('AddRentVehicleComponent', () => {
  let component: AddRentVehicleComponent;
  let fixture: ComponentFixture<AddRentVehicleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddRentVehicleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddRentVehicleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
